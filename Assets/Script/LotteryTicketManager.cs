using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LotteryTicketManager : MonoBehaviour
{
    public DrawSet[] drawSets;
    public LotteryTicket[] lotteryTickets = new LotteryTicket[5];
    public Text[] purchasedNumberTicketTexts;
    public Text purchasedTicketPriceText;

    public float lotteryIntervalTime;
    public float lotteryRestartIntervalTime;

    private enum DrawProduct { Jackpot, Normal, Simple }
    private DrawProduct selectedDrawProduct;

    private int numOfFilledTickets = 0;
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

    private bool timerStop = false;
    public Text timerText;

    public Color purchaseBackgroundColor;
    public MessageManager messageManager;
    public AudioSource purchaseAudioSource;

    private void Start()
    {
        StartCoroutine(DrawTimer(lotteryIntervalTime));
        InitStatusOnPurchaseTexts();
        SetPurchasedTicketText();
    }

    IEnumerator DrawTimer(float timeLeft, float interval = 1f)
    {
        yield return new WaitForSeconds(interval);

        if(!timerStop)
            timeLeft -= interval;

        timerText.text = "���� ��÷ �ð�: " + timeLeft + "��";

        if(timeLeft <= 0)
        {
            DecideWin();
            
            //timeLeft = ;
            // TODO : ��ȣ �̰� �� �ڿ��� ���� ������ �� ����� ��Ű��
            yield return new WaitForSeconds(lotteryRestartIntervalTime);

            timeLeft = lotteryIntervalTime;
        }

        StartCoroutine(DrawTimer(timeLeft));
    }

    private void DecideWin()
    {
        for (int i = 0; i < lotteryTickets.Length; i++)
        {
            if (lotteryTickets[i].selectedNumbers.Length > 0)
            {
                int correctAmount = CompareDraws(GetDrawNumbers(drawSets[(int)selectedDrawProduct]), lotteryTickets[i].selectedNumbers);
                int maxAmount = drawSets[(int)selectedDrawProduct].numberOfPick;
                if (correctAmount == maxAmount)
                {
                    // 1��
                }
                else if (correctAmount == maxAmount - 1)
                {
                    // 2��
                }
                else if (correctAmount == maxAmount - 2)
                {
                    // 3��
                }
            }
        }
    }

    public void SetSelectProductMenu(bool active)
    {
        selectDrawProductMenu.SetActive(active);
    }

    public void SelectDrawProduct(int index)
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

    private bool CheckAlreadyPurchasedTicket()
    {
        for (int i = 0; i < lotteryTickets.Length; i++)
            if (lotteryTickets[i].selectedNumbers.Length > 0)
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
        if (lotteryTickets[index].selectedNumbers.Length > 0)
        {
            for (int i = 0; i < lotteryTickets[index].selectedNumbers.Length; i++)
            {
                numberButtons[lotteryTickets[index].selectedNumbers[i] - 1].image.color = selectedColor;
                selectedStatus[lotteryTickets[index].selectedNumbers[i] - 1] = true;
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
        numOfFilledTickets = 0;
        for (int i = 0; i < statusOnPurchaseTexts.Length; i++)
        {
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
            lotteryTickets[selectedTicket].selectedNumbers = selectedNumbers;
            numOfFilledTickets++;
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
            if (lotteryTickets[i].selectedNumbers.Length > 0)
            {
                purchasedNumberTicketTexts[i].text = (i + 1) + "��: ";
                for (int j = 0; j < lotteryTickets[i].selectedNumbers.Length; j++)
                    purchasedNumberTicketTexts[i].text += lotteryTickets[i].selectedNumbers[j] + " ";
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
            SetPurchasedTicketText();
            SetTicketPriceText(purchasedTicketPriceText, "���� ���� �ݾ�: ", GetPricePerTicket());
            purchaseAudioSource.Play();
            messageManager.ShowMessage("������ " + numOfFilledTickets + "�� �����߽��ϴ�.");
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
        lotteryTickets = new LotteryTicket[5];
        purchaseTicketMenu.SetActive(false);
        totalPriceText.text = "";
        timerStop = false;
    }

    private int[] GetDrawNumbers(DrawSet drawSet)
    {
        int size = drawSet.drawSize;
        int pick = drawSet.numberOfPick;
        int[] pickedNumbers = new int[pick];

        for(int i = 0; i < pick; i++)
        {
            pickedNumbers[i] = Random.Range(1, size + 1);
        }

        return pickedNumbers;
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
    public ulong purchaseAmount;
}