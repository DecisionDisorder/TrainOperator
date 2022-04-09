using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

[System.Serializable]
public class ExpandTrain : MonoBehaviour
{
    public GameObject CheckMenu;
    public Text lengthText;
    public Text priceText;
    public Text passengerText;
    public Text trainExpandStatusText;
    public InputField expandAmountInputField;

    public MessageManager messageManager;
    public PriceData priceData;
    public LineCollection lineCollection;
    public LineDataManager lineDataManager;
    public LineManager lineManager;

    public UpdateDisplay trainConditionUpdateDisplay;

    private static int[,] fromToLength = { { 4, 6 }, { 4, 8 }, { 4, 10 }, { 6, 8 }, { 6, 10 }, { 8, 10 } };
    private int targetType;

    public GameObject lineMenu;

    public GameObject guide;

    private ulong totalPrice;
    private ulong totalPassengerLow;
    private ulong totalPassengerHigh;
    private int expandAmount;
    public AudioSource purchaseSound;

    void Start()
    {
        trainConditionUpdateDisplay.onEnableUpdate += SetTrainExpandText;
    }

    public void SetLineMenu(bool onOff)
    {
        lineMenu.SetActive(onOff);
    }

    public static string GetExpandTargetLength(int type, int maxAmount)
    {
        return string.Format("{0}량 ▶▶ {1}량 ({2}개 확장 가능)", fromToLength[type, 0], fromToLength[type, 1], maxAmount);
    }

    public void SetUpgradeType(int type)
    {
        targetType = type;
        int maxAmount = lineCollection.lineData.trainExpandStatus[GetIndexFromLength(fromToLength[targetType, 0])];
        CheckMenu.SetActive(true);
        lineManager.currentLine = lineCollection.line;
        lengthText.text = string.Format("{0}량 ▶▶ {1}량 ({2}개 확장 가능)", fromToLength[type, 0], fromToLength[type, 1], maxAmount);
        if (guide != null)
            guide.SetActive(true);
    }

    public void ExpandTrainProcess(bool apply)
    {
        if(apply)
        { 
            if(expandAmount > lineCollection.lineData.trainExpandStatus[GetIndexFromLength(fromToLength[targetType, 0])])
                SetAmountAll();

            if (TouchMoneyManager.CheckLimitValid(totalPassengerLow, totalPassengerHigh))
            {
                bool result;
                if(priceData.IsLargeUnit)
                    result = AssetMoneyCalculator.instance.ArithmeticOperation(0, totalPrice, false);
                else
                    result = AssetMoneyCalculator.instance.ArithmeticOperation(totalPrice, 0, false);

                if (result)
                {
                    ApplyExpandTrain();
                    TouchMoneyManager.ArithmeticOperation(totalPassengerLow, totalPassengerHigh, true);
                    CompanyReputationManager.instance.RenewPassengerBase();
                    ExpandSuccess(expandAmount);
                    SetTrainExpandText();
                    DataManager.instance.SaveAll();
                    if (!totalPrice.Equals(0))
                        purchaseSound.Play();
                }
                else
                    PlayManager.instance.LackOfMoney();
            }
            else
                PlayManager.instance.LackOfPassengerLimit();
        }

        expandAmount = 0;
        totalPrice = 0;
        expandAmountInputField.text = expandAmount.ToString();
        priceText.text = "가격: 0$";
        passengerText.text = "승객 수+ 0명";
        CheckMenu.SetActive(false);

        if (guide != null)
            guide.SetActive(false);
    }

    public void SetExpandAmount(int amount)
    {
        // 노선 각각의 스크립트도 수정해야 함
        // + n량 ->-> m량 에 괄호로 최대 몇 량 가능한지도 써주기
        expandAmount = amount;
        int maximumAmount = lineCollection.lineData.trainExpandStatus[GetIndexFromLength(fromToLength[targetType, 0])];
        if (expandAmount > maximumAmount)
            expandAmount = maximumAmount;

        SetTotalPrice();
        SetCheckTexts();
    }

    public void SetAmountAll()
    {
        SetExpandAmount(lineCollection.lineData.trainExpandStatus[GetIndexFromLength(fromToLength[targetType, 0])]);
    }

    private void SetCheckTexts()
    {
        string m1 = "", m2 = "", p1 = "", p2 = "";
        if (priceData.IsLargeUnit)
        {
            PlayManager.ArrangeUnit(0, totalPrice, ref m1, ref m2, true);
        }
        else
        {
            PlayManager.ArrangeUnit(totalPrice, 0, ref m1, ref m2, true);
        }

        PlayManager.ArrangeUnit(totalPassengerLow, totalPassengerHigh, ref p1, ref p2, true);

        if (m1.Equals(m2) && m1.Equals(""))
        {
            priceText.text = "가격: 0$";
            passengerText.text = "승객 수 +0명";
        }
        else
        {
            priceText.text = "가격: " + m2 + m1 + "$";
            passengerText.text = "승객 수 +" + p2 + p1 + "명";
        }
        expandAmountInputField.text = expandAmount.ToString();
    }

    public void SetTrainExpandText()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Clear();
        for (int j = 0; j < 4; j++)
        {
            stringBuilder.Append(4 + j * 2);
            stringBuilder.Append("량(칸): ");
            stringBuilder.Append(lineCollection.lineData.trainExpandStatus[j]);
            stringBuilder.Append("개\n");
        }
        trainExpandStatusText.text = stringBuilder.ToString();
    }

    private int GetIndexFromLength(int num)
    {
        return (num - 4) / 2;
    }

    private void ApplyExpandTrain()
    {
        lineCollection.lineData.trainExpandStatus[GetIndexFromLength(fromToLength[targetType, 0])] -= expandAmount;
        lineCollection.lineData.trainExpandStatus[GetIndexFromLength(fromToLength[targetType, 1])] += expandAmount;
    }

    private void SetTotalPrice()
    {
        totalPrice = priceData.TrainExpandPrice * ((ulong)(fromToLength[targetType, 1] - fromToLength[targetType, 0]) / 2) * (ulong)expandAmount;
        totalPassengerLow = priceData.TrainExapndPassenger * ((ulong)(fromToLength[targetType, 1] - fromToLength[targetType, 0]) / 2) * (ulong)expandAmount;
        totalPassengerHigh = 0;
        MoneyUnitTranslator.Arrange(ref totalPassengerLow, ref totalPassengerHigh);
    }
    private void ExpandSuccess(int expand)
    {
        messageManager.ShowMessage("차량 " + expand + "대를 확장하였습니다.");
    }
}
