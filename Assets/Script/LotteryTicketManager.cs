using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LotteryTicketManager : MonoBehaviour
{
    public DrawSet[] drawSets;
    public LotteryTicket[] lotteryTickets = new LotteryTicket[5];

    private enum DrawProduct { Jackpot, Normal, Simple }
    private DrawProduct selectedDrawProduct;

    private int selectedTicket = 0;
    public Text selectedTicketText;
    private bool[] selectedStatus;
    private int numOfSelected = 0;

    public Button[] numberButtons;
    public Color selectedColor;
    public Text[] statusOnPurchaseTexts;

    public GameObject selectDrawProductMenu;
    public GameObject purchaseTicketMenu;

    private bool timerStop = false;

    public Color purchaseBackgroundColor;
    public MessageManager messageManager;

    private void Start()
    {
        StartCoroutine(DrawTimer(10f));
    }

    IEnumerator DrawTimer(float timeLeft, float interval = 1f)
    {
        yield return new WaitForSeconds(interval);

        if(!timerStop)
            timeLeft -= interval;

        // 타이머 텍스트 갱신

        if(timeLeft <= 0)
        {
            //CompareDraws(GetDrawNumbers(drawSets[(int)selectedDrawProduct]));
        }

        // TODO : 번호 뽑고 난 뒤에는 일정 딜레이 후 재시작 시키기
        StartCoroutine(DrawTimer(timeLeft));
    }

    public void SetSelectProductMenu(bool active)
    {
        selectDrawProductMenu.SetActive(active);
    }

    public void SelectDrawProduct(int index)
    {
        timerStop = true;
        selectedDrawProduct = (DrawProduct)index;
        purchaseTicketMenu.SetActive(true);
        InitStatusOnPurchaseTexts();
        InitNumberButtons();
    }

    public void SelectTicket(int index)
    {
        InitNumberButtons();
        selectedTicketText.text = "티켓 번호: " + (index + 1);
        selectedTicket = index;
        if (lotteryTickets[index].selectedNumbers != null)
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
        numOfSelected = 0;
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
            statusOnPurchaseTexts[i].text = (i + 1) + "번: 0 0 0 0 0";
            if (drawSets[(int)selectedDrawProduct].numberOfPick.Equals(6))
                statusOnPurchaseTexts[i].text += " 0";
        }
    }

    public void NumberClick(int num)
    {
        if(selectedStatus[num])
        {
            numOfSelected--;
            selectedStatus[num] = false;
            numberButtons[num].image.color = Color.white;
        }
        else
        {
            if (numOfSelected < drawSets[(int)selectedDrawProduct].numberOfPick)
            {
                numOfSelected++;
                selectedStatus[num] = true;
                numberButtons[num].image.color = selectedColor;
            }
        }
    }

    public void SelectNumberOnTicket()
    {
        if (numOfSelected == drawSets[(int)selectedDrawProduct].numberOfPick)
        {
            int[] selectedNumbers = new int[drawSets[(int)selectedDrawProduct].numberOfPick];
            int selectedCount = 0;
            statusOnPurchaseTexts[selectedTicket].text = (selectedTicket + 1) + "번: ";
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
        }
        else
        {
            messageManager.ShowMessage(drawSets[(int)selectedDrawProduct].numberOfPick + "개의 숫자를 골라주세요.");
        }
    }

    public void CheckPurchase()
    {
        string title = "복권 구매 확인";
        string product = "제품: " + drawSets[(int)selectedDrawProduct].name;
        string amount = "수량: "; // TODO : 수량 구하기
        string price = "가격: "; // TODO : 가격 산정하기
        messageManager.SetPurchaseCheckMenu(title, product, amount, price, purchaseBackgroundColor, BuyTickets, Cancel);
    }

    public void BuyTickets()
    {
        purchaseTicketMenu.SetActive(false);
        timerStop = false;
    }

    private void Cancel() { }

    public void CloseTicket()
    {
        lotteryTickets = new LotteryTicket[5];
        purchaseTicketMenu.SetActive(false);
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
    public int[] selectedNumbers = null;
    public ulong purchaseAmount;
}