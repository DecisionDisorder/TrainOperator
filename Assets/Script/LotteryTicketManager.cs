using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DrawProduct { Jackpot, Normal, Simple }

public class LotteryTicketManager : MonoBehaviour
{
    public LotteryData lotteryData;

    public DrawSet[] drawSets;
    public LotteryTicket[] LotteryTickets { set { lotteryData.lotteryTickets = value; } get { return lotteryData.lotteryTickets; } }
    public List<LotteryRecord> JackpotLotteryRecords { set { lotteryData.jackpotLotteryRecords = value; } get { return lotteryData.jackpotLotteryRecords; } }
    public List<LotteryRecord> NormalLotteryRecords { set { lotteryData.normalLotteryRecords = value; } get { return lotteryData.normalLotteryRecords; } }
    public List<LotteryRecord> SimpleLotteryRecords { set { lotteryData.simpleLotteryRecords = value; } get { return lotteryData.simpleLotteryRecords; } }
    public List<LotteryRecord> JackpotWinRecords { set { lotteryData.jackpotWinRecords= value; } get { return lotteryData.jackpotWinRecords; } }
    public List<LotteryRecord> NormalWinRecords { set { lotteryData.normalWinRecords = value; } get { return lotteryData.normalWinRecords; } }
    public List<LotteryRecord> SimpleWinRecords { set { lotteryData.simpleWinRecords = value; } get { return lotteryData.simpleWinRecords; } }

    public LargeVariable UnreceivedReward { set { lotteryData.unreceivedReward = value; UpdateUnreceivedReward(); } get { return lotteryData.unreceivedReward; } }
    public int DrawTimeLeft { set { lotteryData.drawTimeLeft = value; } get { return lotteryData.drawTimeLeft; } }

    public Text[] purchasedNumberTicketTexts;
    public Text purchasedTicketPriceText;

    public int lotteryIntervalTime;
    public int lotteryRestartIntervalTime;
    public int lotteryDrawDelayTime;

    private DrawProduct selectedDrawProduct;

    [SerializeField]
    private bool[] filledTickets = new bool[5];
    private int numOfFilledTickets { 
        get
        {
            int count = 0;
            for (int i = 0; i < filledTickets.Length; i++)
                if (filledTickets[i])
                    count++;
            return count;
        } 
    }
    private int selectedTicket = 0;
    public Text selectedTicketText;
    private bool[] selectedStatus;
    private int numOfSelectedNumber = 0;

    public ulong ticketPriceMultiply = 50;
    public Text ticketPriceText;
    public Text totalPriceText;

    public Button[] numberButtons;
    public Color selectedColor;
    public Text[] statusOnPurchaseTexts;

    public GameObject selectDrawProductMenu;
    public GameObject purchaseTicketMenu;
    public GameObject ticketStatusMenu;

    public Text pickAnnouncementText;
    public GameObject Numbers5Group;
    public Text[] numbers5Texts;
    public GameObject[] number5Balls;
    public GameObject Numbers6Group;
    public Text[] numbers6Texts;
    public GameObject[] number6Balls;
    private LargeVariable rewardMoney;

    private bool isDrawing = false;
    private bool timerStop = false;
    public Text timerText;

    public GameObject lotteryTicketRecordsMenu;
    public Text recordTitleText;
    public Text lotteryRecordText;
    public RectTransform lotteryRecordRect;
    public Text winRecordText;
    public RectTransform winRecordRect;
    public Text unreceivedRewardText;
    private int openedRecordIndex;

    public Color purchaseBackgroundColor;
    public MessageManager messageManager;
    public AudioSource purchaseAudioSource;

    private void Start()
    {
        if (LotteryTickets[0].selectedNumbers == null)
            ResetLotteryTickets();

        StartCoroutine(DrawTimer());
        InitStatusOnPurchaseTexts();
        SetPurchasedTicketText();
    }

    IEnumerator DrawTimer(int interval = 1)
    {
        yield return new WaitForSeconds(interval);

        if (!timerStop)
        {
            DrawTimeLeft -= interval;
            if (DrawTimeLeft == 10)
                messageManager.ShowPopupMessage("���� ��÷���� 10�� ���ҽ��ϴ�!");
        }

        timerText.text = "���� ��÷���� " + DrawTimeLeft + "�� ��...";

        if(DrawTimeLeft <= 0)
        {
            DisableBalls();
            isDrawing = true;
            timerText.text = "����� ��ȣ ��÷ ��";

            yield return new WaitForSeconds(lotteryDrawDelayTime);

            if((!ticketStatusMenu.activeInHierarchy && !lotteryTicketRecordsMenu.activeInHierarchy) && (numOfFilledTickets > 0))
                messageManager.ShowPopupMessage("���� ��ȣ�� ��÷�Ǿ����ϴ�! ����� Ȯ���غ�����!");
            DecideWin();
            ResetLotteryTickets();
            timerText.text = "���� ȸ�� �غ� ��";

            yield return new WaitForSeconds(lotteryRestartIntervalTime);

            isDrawing = false;
            DrawTimeLeft = lotteryIntervalTime;
        }

        StartCoroutine(DrawTimer());
    }

    private void StartShowingBalls()
    {
        if(LotteryTicket.selectedProduct == DrawProduct.Jackpot)
        {
            StartCoroutine(ShowingBalls(number6Balls));
        }
        else
        {
            StartCoroutine(ShowingBalls(number5Balls));
        }
        
    }

    IEnumerator ShowingBalls(GameObject[] balls, int index = 0)
    {
        yield return new WaitForSeconds(0.8f);

        balls[index].SetActive(true);

        if (index < balls.Length - 1)
            StartCoroutine(ShowingBalls(balls, index + 1));
    }

    private void DisableBalls()
    {
        Numbers6Group.SetActive(false);
        for (int i = 0; i < number6Balls.Length; i++)
            number6Balls[i].SetActive(false);

        Numbers5Group.SetActive(false);
        for (int i = 0; i < number5Balls.Length; i++)
            number5Balls[i].SetActive(false);
    }

    private void DecideWin()
    {
        pickAnnouncementText.gameObject.SetActive(false);

        DrawAllProducts();
        StartShowingBalls();
        List<LotteryRecord> lotteryRecords = GetLotteryRecordByIndex((int)LotteryTicket.selectedProduct);
        int[] pickedNumbers = lotteryRecords[lotteryRecords.Count - 1].pickedNumbers;
        SetPickedNumbersText(pickedNumbers);

        for (int i = 0; i < LotteryTickets.Length; i++)
        {
            if (LotteryTickets[i].selectedNumbers.Length > 0)
            {
                int correctAmount = CompareDraws(pickedNumbers, LotteryTickets[i].selectedNumbers);
                int maxAmount = drawSets[(int)LotteryTicket.selectedProduct].numberOfPick;
                for (int k = 0; k < 3; k++)
                {
                    if (correctAmount == maxAmount - k)
                    {
                        rewardMoney = LotteryTicket.purchaseAmount * drawSets[(int)LotteryTicket.selectedProduct].rewardMultiple[k];
                        UnreceivedReward += rewardMoney;
                        LotteryRecord winRecord = new LotteryRecord(lotteryRecords.Count, LotteryTickets[i].selectedNumbers);
                        GetWinRecordByIndex((int)LotteryTicket.selectedProduct).Add(winRecord);
                        winRecord.SetPastWinData(rewardMoney, k + 1);
                        break;
                    }
                }
                SetWinRecordText(openedRecordIndex);

            }
        }
    }

    public void SetSelectProductMenu(bool active)
    {
        selectDrawProductMenu.SetActive(active);
    }

    public void SetTicketStatusMenu(bool active)
    {
        if (active)
        {
            InitPurchasedNumbersText();
            SetPurchasedTicketText();
        }
        ticketStatusMenu.SetActive(active);
    }

    public void SelectDrawProduct(int index)
    {
        if (!isDrawing)
        {
            if (!CheckAlreadyPurchasedTicket())
            {
                timerStop = true;
                selectedDrawProduct = (DrawProduct)index;
                purchaseTicketMenu.SetActive(true);
                InitStatusOnPurchaseTexts();
                InitNumberButtons();
                SetTicketPriceText();
            }
            else
                messageManager.ShowMessage("�̹� �̹� ȸ���� ������ �����Ͽ� ���̻� ���Ű� �Ұ����մϴ�.");
        }
        else
            messageManager.ShowMessage("��÷ �߿� ���Ű� �Ұ����մϴ�.\n��� �Ŀ� �̿��� �ֽñ� �ٶ��ϴ�.");
    }

    private bool CheckAlreadyPurchasedTicket()
    {
        for (int i = 0; i < LotteryTickets.Length; i++)
            if (LotteryTickets[i].selectedNumbers.Length > 0)
                return true;

        return false;
    }

    private void SetTicketPriceText()
    {
        SetTicketPriceText(ticketPriceText, "1��� ", GetPricePerTicket());
    }

    private void SetTotalPriceText()
    {
        SetTicketPriceText(totalPriceText, "�Ѿ�: ", GetTotalPrice());
    }

    public void SelectTicket(int index)
    {
        InitNumberButtons();
        selectedTicketText.text = "Ƽ�� ��ȣ: " + (index + 1);
        selectedTicket = index;
        if (LotteryTickets[index].selectedNumbers.Length > 0)
        {
            numOfSelectedNumber = drawSets[(int)selectedDrawProduct].numberOfPick;
            for (int i = 0; i < LotteryTickets[index].selectedNumbers.Length; i++)
            {
                numberButtons[LotteryTickets[index].selectedNumbers[i] - 1].image.color = selectedColor;
                selectedStatus[LotteryTickets[index].selectedNumbers[i] - 1] = true;
            }
        }

    }

    private void InitNumberButtons()
    {
        numOfSelectedNumber = 0;
        selectedStatus = new bool[drawSets[(int)selectedDrawProduct].drawSize];
        for(int i = 0; i < numberButtons.Length; i++)
        {
            numberButtons[i].image.color = Color.white;

            if (i >= drawSets[(int)selectedDrawProduct].drawSize)
                numberButtons[i].gameObject.SetActive(false);
            else
                numberButtons[i].gameObject.SetActive(true);
        }
    }

    private void InitStatusOnPurchaseTexts()
    {
        for (int i = 0; i < statusOnPurchaseTexts.Length; i++)
        {
            filledTickets[i] = false;
            statusOnPurchaseTexts[i].text = (i + 1) + "��: 0 0 0 0 0";
            if (drawSets[(int)selectedDrawProduct].numberOfPick.Equals(6))
                statusOnPurchaseTexts[i].text += " 0";
        }
    }

    private void InitPurchasedNumbersText()
    {
        for (int i = 0; i < purchasedNumberTicketTexts.Length; i++)
        {
            purchasedNumberTicketTexts[i].text = (i + 1) + "��: 0 0 0 0 0";
            if (drawSets[(int)selectedDrawProduct].numberOfPick.Equals(6))
                purchasedNumberTicketTexts[i].text += " 0";
        }
    }

    public void NumberClick(int num)
    {
        if(selectedStatus[num])
        {
            numOfSelectedNumber--;
            selectedStatus[num] = false;
            numberButtons[num].image.color = Color.white;
        }
        else
        {
            if (numOfSelectedNumber < drawSets[(int)selectedDrawProduct].numberOfPick)
            {
                numOfSelectedNumber++;
                selectedStatus[num] = true;
                numberButtons[num].image.color = selectedColor;
            }
        }
    }

    public void SelectNumberOnTicket()
    {
        if (numOfSelectedNumber == drawSets[(int)selectedDrawProduct].numberOfPick)
        {
            int[] selectedNumbers = new int[drawSets[(int)selectedDrawProduct].numberOfPick];
            int selectedCount = 0;
            statusOnPurchaseTexts[selectedTicket].text = (selectedTicket + 1) + "��: ";
            for (int i = 0; i < drawSets[(int)selectedDrawProduct].drawSize; i++)
            {
                if (selectedStatus[i])
                {
                    selectedNumbers[selectedCount] = i + 1;
                    statusOnPurchaseTexts[selectedTicket].text += selectedNumbers[selectedCount] + " ";
                    selectedCount++;
                }
            }
            LotteryTickets[selectedTicket].selectedNumbers = selectedNumbers;
            LotteryTicket.purchaseAmount = GetPricePerTicket();
            filledTickets[selectedTicket] = true;
            SetTotalPriceText();
        }
        else
        {
            messageManager.ShowMessage(drawSets[(int)selectedDrawProduct].numberOfPick + "���� ���ڸ� ����ּ���.");
        }
    }

    public void CheckPurchase()
    {
        if (numOfFilledTickets > 0)
        {
            string title = "���� ���� Ȯ��";
            string product = "��ǰ: " + drawSets[(int)selectedDrawProduct].name;
            string amount = "����: " + numOfFilledTickets + "��";
            string priceLow = "", priceHigh = "";
            LargeVariable priceVariable = GetTotalPrice();
            PlayManager.ArrangeUnit(priceVariable.lowUnit, priceVariable.highUnit, ref priceLow, ref priceHigh, true);
            string price = "����: " + priceHigh + priceLow + "$";
            messageManager.SetPurchaseCheckMenu(title, product, amount, price, purchaseBackgroundColor, BuyTickets, Cancel);
        }
        else
        {
            messageManager.ShowMessage("������ ���� ��ȣ�� �����ϴ�.");
        }
    }

    private void SetPurchasedTicketText()
    {
        for (int i = 0; i < purchasedNumberTicketTexts.Length; i++)
        {
            if (LotteryTickets[i].selectedNumbers.Length > 0)
            {
                purchasedNumberTicketTexts[i].text = (i + 1) + "��: ";
                for (int j = 0; j < LotteryTickets[i].selectedNumbers.Length; j++)
                    purchasedNumberTicketTexts[i].text += LotteryTickets[i].selectedNumbers[j] + " ";
            }
        }
    }
    private LargeVariable GetPricePerTicket()
    {
        return MyAsset.instance.GetTotalRevenue() * ticketPriceMultiply;
    }

    private LargeVariable GetTotalPrice()
    {
        return GetPricePerTicket() * numOfFilledTickets;
    }

    public void BuyTickets()
    {
        if (AssetMoneyCalculator.instance.ArithmeticOperation(GetTotalPrice(), false))
        {
            purchaseTicketMenu.SetActive(false);
            timerStop = false;
            LotteryTicket.selectedProduct = selectedDrawProduct;
            SetTicketPriceText(purchasedTicketPriceText, "���� ���� �ݾ�: ", GetPricePerTicket());
            purchaseAudioSource.Play();
            messageManager.ShowMessage("������ " + numOfFilledTickets + "�� �����߽��ϴ�.");
            DisableBalls();
        }
        else
            PlayManager.instance.LackOfMoney();
    }

    private void SetTicketPriceText(Text text, string title, LargeVariable price)
    {
        string priceLow = "", priceHigh = "";
        PlayManager.ArrangeUnit(price.lowUnit, price.highUnit, ref priceLow, ref priceHigh, true);
        text.text = title + priceHigh + priceLow + "$";
    }

    private void Cancel() { }

    public void CloseTicket()
    {
        purchaseTicketMenu.SetActive(false);
        ResetLotteryTickets();
        totalPriceText.text = "";
        timerStop = false;
    }

    public void DeleteTicket(int ticketIndex)
    {
        if (LotteryTickets[ticketIndex].selectedNumbers.Length > 0)
        {
            filledTickets[ticketIndex] = false;
            LotteryTickets[ticketIndex].selectedNumbers = new int[0];
            statusOnPurchaseTexts[ticketIndex].text = (ticketIndex + 1) + "��: 0 0 0 0 0 ";
            if (drawSets[(int)selectedDrawProduct].drawSize > 5)
                statusOnPurchaseTexts[selectedTicket].text += "0 ";
            SetTotalPriceText();
        }
    }

    private void ResetLotteryTickets()
    {
        LotteryTickets = new LotteryTicket[5];
        for (int i = 0; i < LotteryTickets.Length; i++)
        {
            LotteryTickets[i] = new LotteryTicket();
            LotteryTickets[i].selectedNumbers = new int[0];
        }
    }

    private void DrawAllProducts()
    {
        List<int[]> pickedNumbers = new List<int[]>();
        for (int i = 0; i < 3; i++)
        {
            pickedNumbers.Add(GetTestDrawNumber(drawSets[i]));
            //pickedNumbers.Add(GetDrawNumbers(drawSets[i]));
            List<LotteryRecord> lotteryRecords = GetLotteryRecordByIndex(i);
            lotteryRecords.Add(new LotteryRecord(lotteryRecords.Count + 1, pickedNumbers[i]));
        }
        SetLotteryRecordText(openedRecordIndex);
    }

    private int[] GetTestDrawNumber(DrawSet drawset)
    {
        int[] test = new int[drawset.numberOfPick];
        for (int i = 1; i <= drawset.numberOfPick; i++)
            test[i - 1] = i;
        return test;
    }

    private int[] GetDrawNumbers(DrawSet drawSet)
    {
        int size = drawSet.drawSize;
        int pick = drawSet.numberOfPick;
        int[] pickedNumbers = new int[pick];

        for(int i = 0; i < pick; i++)
        {
            pickedNumbers[i] = GetRandomNumber(pickedNumbers, size);
        }
        System.Array.Sort(pickedNumbers);

        return pickedNumbers;
    }

    private int GetRandomNumber(int[] checkArr, int max)
    {
        int newNum = Random.Range(1, max + 1);

        while (IsOverlap(checkArr, newNum))
        {
            newNum = Random.Range(1, max + 1);
        }

        return newNum;
    }

    /// <summary>
    /// �ߺ��� �����ϴ��� Ȯ��
    /// </summary>
    /// <returns>�ߺ��Ǹ� true, �ߺ����� ������ false</returns>
    private bool IsOverlap(int[] checkArr, int newNum)
    {
        for(int i = 0; i < checkArr.Length; i++)
        {
            if (checkArr[i] == newNum)
                return true;
        }

        return false;
    }

    private void SetPickedNumbersText(int[] pickedNumbers)
    {
        if(pickedNumbers.Length == 5)
        {
            Numbers6Group.SetActive(false);
            Numbers5Group.SetActive(true);
            for (int i = 0; i < pickedNumbers.Length; i++)
                numbers5Texts[i].text = pickedNumbers[i] + "";
        }
        else if(pickedNumbers.Length == 6)
        {
            Numbers5Group.SetActive(false);
            Numbers6Group.SetActive(true);
            for (int i = 0; i < pickedNumbers.Length; i++)
                numbers6Texts[i].text = pickedNumbers[i] + "";
        }
    }

    public void ConfirmTicketStatus()
    {
        if(rewardMoney != LargeVariable.zero)
        {
            string rewardLow = "", rewardHigh = "";
            PlayManager.ArrangeUnit(rewardMoney.lowUnit, rewardMoney.highUnit, ref rewardLow, ref rewardHigh, true);
            AssetMoneyCalculator.instance.ArithmeticOperation(rewardMoney, true);
            UnreceivedReward -= rewardMoney;
            rewardMoney = LargeVariable.zero;
            string title = "���� ��÷!";
            string reward = "���ϵ帳�ϴ�!!\n���� ��÷ �ݾ����� <color=green>" + rewardHigh + rewardLow + "$</color>�� �����߽��ϴ�!";
            messageManager.ShowRevenueReport(title, reward);
        }
        SetTicketStatusMenu(false);
    }
    
    private int CompareDraws(int[] pickedNumbers, int[] selectedNumbers)
    {
        int numOfCorrect = 0;

        for(int i = 0; i < selectedNumbers.Length; i++)
        {
            for(int j = 0; j < pickedNumbers.Length; j++)
            {
                if(selectedNumbers[i] == pickedNumbers[j])
                {
                    numOfCorrect++;
                    break;
                }
            }
        }

        return numOfCorrect;
    }

    public void OpenLotteryRecords(int productIndex)
    {
        lotteryTicketRecordsMenu.SetActive(true);
        recordTitleText.text = drawSets[productIndex].name;
        openedRecordIndex = productIndex;
        SetLotteryRecordText(productIndex);
        SetWinRecordText(productIndex);
    }

    public void CloseLotteryRecords()
    {
        lotteryTicketRecordsMenu.SetActive(false);
    }

    private List<LotteryRecord> GetLotteryRecordByIndex(int index)
    {
        if((DrawProduct)index == DrawProduct.Jackpot)
        {
            return JackpotLotteryRecords;
        }
        else if((DrawProduct)index == DrawProduct.Normal)
        {
            return NormalLotteryRecords;
        }
        else
        {
            return SimpleLotteryRecords;
        }
    }

    private List<LotteryRecord> GetWinRecordByIndex(int index)
    {
        if ((DrawProduct)index == DrawProduct.Jackpot)
        {
            return JackpotWinRecords;
        }
        else if ((DrawProduct)index == DrawProduct.Normal)
        {
            return NormalWinRecords;
        }
        else
        {
            return SimpleWinRecords;
        }
    }

    private void SetLotteryRecordText(int productIndex)
    {
        List<LotteryRecord> lotteryRecords = GetLotteryRecordByIndex(productIndex);
        if (lotteryRecords.Count > 0)
        {
            lotteryRecordText.text = "";
            for (int i = lotteryRecords.Count - 1; i >= 0; i--)
            {
                lotteryRecordText.text += lotteryRecords[i].index + "ȸ��: ";
                for (int j = 0; j < lotteryRecords[i].pickedNumbers.Length; j++)
                    lotteryRecordText.text += lotteryRecords[i].pickedNumbers[j] + " ";
                lotteryRecordText.text += "\n";
            }

            var rectSize = lotteryRecordRect.sizeDelta;
            rectSize.y = lotteryRecordText.preferredHeight;
            lotteryRecordRect.sizeDelta = rectSize;
        }
        else
        {
            lotteryRecordText.text = "��÷ ����� �����ϴ�.";
        }
    }

    private void SetWinRecordText(int productIndex)
    {
        List<LotteryRecord> winRecords = GetWinRecordByIndex(productIndex);
        if (winRecords.Count > 0)
        {
            winRecordText.text = "";
            for (int i = winRecords.Count - 1; i >= 0; i--)
            {
                winRecordText.text += winRecords[i].index + "ȸ��: ";
                for (int j = 0; j < winRecords[i].pickedNumbers.Length; j++)
                    winRecordText.text += winRecords[i].pickedNumbers[j] + " ";
                winRecordText.text += "\n";
                winRecordText.text += "(" + winRecords[i].pastRank + "��) " + winRecords[i].pastReward.ToString() + "$\n";
            }

            var rectSize = winRecordRect.sizeDelta;
            rectSize.y = winRecordText.preferredHeight;
            winRecordRect.sizeDelta = rectSize;
        }
        else
        {
            winRecordText.text = "��÷ ����� �����ϴ�.";
        }
    }

    private void UpdateUnreceivedReward()
    {
        string rewardLow = "", rewardHigh = "";
        PlayManager.ArrangeUnit(UnreceivedReward.lowUnit, UnreceivedReward.highUnit, ref rewardLow, ref rewardHigh);
        unreceivedRewardText.text = "�̼��� ��÷��: " + rewardHigh + rewardLow + "$";
    }

    public void ReceiveUnreceivedReward()
    {
        if(UnreceivedReward > LargeVariable.zero)
        {
            string rewardLow = "", rewardHigh = "";
            PlayManager.ArrangeUnit(UnreceivedReward.lowUnit, UnreceivedReward.highUnit, ref rewardLow, ref rewardHigh);
            AssetMoneyCalculator.instance.ArithmeticOperation(UnreceivedReward, true);
            UnreceivedReward = LargeVariable.zero;
            rewardMoney = LargeVariable.zero;
            string title = "�̼��� ��÷�� �ޱ�";
            string msg = "��÷�� ���ϵ帳�ϴ�!\n������ ��÷��: <color=green>" + rewardHigh + rewardLow + "$</color>";
            messageManager.ShowRevenueReport(title, msg);
        }
        else
        {
            messageManager.ShowMessage("�������� ���� ��÷���� �����ϴ�.");
        }
    }
}


[System.Serializable]
public class DrawSet
{
    public string name;
    public int drawSize;
    public int numberOfPick;
    public int[] rewardMultiple;
}

[System.Serializable]
public class LotteryTicket
{
    public int[] selectedNumbers;
    public static DrawProduct selectedProduct;
    public static LargeVariable purchaseAmount;
}

[System.Serializable]
public class LotteryRecord
{
    public int index;
    public int[] pickedNumbers;
    public LargeVariable pastReward;
    public int pastRank;

    public LotteryRecord(int index, int[] pickedNumbers)
    {
        this.index = index;
        this.pickedNumbers = pickedNumbers;
    }

    public void SetPastWinData(LargeVariable pastReward, int pastRank)
    {
        this.pastReward = pastReward;
        this.pastRank = pastRank;
    }
}