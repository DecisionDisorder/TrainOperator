using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

/// <summary>
/// ���� Ȯ�� ���� Ŭ����
/// </summary>
[System.Serializable]
public class ExpandTrain : MonoBehaviour
{
    /// <summary>
    /// ���� Ȯ�� �޴� ������Ʈ
    /// </summary>
    public GameObject CheckMenu;
    /// <summary>
    /// Ȯ���� ���� �ؽ�Ʈ
    /// </summary>
    public Text lengthText;
    /// <summary>
    /// Ȯ�� ��� �ؽ�Ʈ
    /// </summary>
    public Text priceText;
    /// <summary>
    /// Ȯ�� ��, �����ϴ� �°� �� �ȳ� �ؽ�Ʈ
    /// </summary>
    public Text passengerText;
    /// <summary>
    /// �������� ��Ȳ������ ���� ���� �� ��Ȳ �ؽ�Ʈ
    /// </summary>
    public Text trainExpandStatusText;
    /// <summary>
    /// Ȯ�� ���� �Է� �ʵ�
    /// </summary>
    public InputField expandAmountInputField;

    public MessageManager messageManager;
    public PriceData priceData;
    public LineCollection lineCollection;
    public LineDataManager lineDataManager;
    public LineManager lineManager;
    public UpdateDisplay trainConditionUpdateDisplay;

    /// <summary>
    /// Ȯ�� ������ Pair
    /// </summary>
    private static int[,] fromToLength = { { 4, 6 }, { 4, 8 }, { 4, 10 }, { 6, 8 }, { 6, 10 }, { 8, 10 } };
    /// <summary>
    /// Ȯ�� ����
    /// </summary>
    private int targetType;

    /// <summary>
    /// �뼱 �޴� ������Ʈ
    /// </summary>
    public GameObject lineMenu;

    /// <summary>
    /// ���̵� ������Ʈ (1ȣ�� ����)
    /// </summary>
    public GameObject guide;

    /// <summary>
    /// ���� ����
    /// </summary>
    private ulong totalPrice;
    /// <summary>
    /// ���� �°� �� ������(Low ����)
    /// </summary>
    private ulong totalPassengerLow;
    /// <summary>
    /// ���� �°� �� ������(High ����)
    /// </summary>
    private ulong totalPassengerHigh;
    /// <summary>
    /// Ȯ�� ����
    /// </summary>
    private int expandAmount;
    /// <summary>
    /// ���� ȿ����
    /// </summary>
    public AudioSource purchaseSound;

    void Start()
    {
        trainConditionUpdateDisplay.onEnableUpdate += SetTrainExpandText;
    }

    /// <summary>
    /// �뼱 �޴� Ȱ��ȭ/��Ȱ��ȭ
    /// </summary>
    /// <param name="onOff">Ȱ��ȭ ����</param>
    public void SetLineMenu(bool onOff)
    {
        lineMenu.SetActive(onOff);
    }

    /// <summary>
    /// Ȯ�� �ȳ� �ؽ�Ʈ Formatting
    /// </summary>
    /// <param name="type">Ȯ�� ���� �ε���</param>
    /// <param name="maxAmount">Ȯ�� ������ �ִ� ����</param>
    /// <returns>�ȳ� �ؽ�Ʈ</returns>
    public string GetExpandTargetLength(int type, int maxAmount)
    {
        return string.Format("{0}�� ���� {1}�� ({2}�� Ȯ�� ����)", fromToLength[type, 0], fromToLength[type, 1], maxAmount);
    }

    /// <summary>
    /// ���� Ȯ�� ���� ����
    /// </summary>
    /// <param name="type">Ȯ�� ���� �ε���</param>
    public void SetUpgradeType(int type)
    {
        targetType = type;
        int maxAmount = lineCollection.lineData.trainExpandStatus[GetIndexFromLength(fromToLength[targetType, 0])];
        CheckMenu.SetActive(true);
        lineManager.currentLine = lineCollection.line;
        lengthText.text = GetExpandTargetLength(type, maxAmount);
        if (guide != null)
            guide.SetActive(true);
    }

    /// <summary>
    /// ���� Ȯ�� ó��
    /// </summary>
    /// <param name="apply">Ȯ�� Ȯ�� ����</param>

    public void ExpandTrainProcess(bool apply)
    {
        // Ȯ�� Ȯ���� ���
        if(apply)
        { 
            // Ȯ�� ���� �ִ�ġ�� �ʰ��ϴ� ��쿡 ���� ���� ó��
            if(expandAmount > lineCollection.lineData.trainExpandStatus[GetIndexFromLength(fromToLength[targetType, 0])])
                SetAmountAll();

            // �°� �� ������ �ʰ����� �ʴ��� Ȯ��
            if (TouchMoneyManager.CheckLimitValid(totalPassengerLow, totalPassengerHigh))
            {
                // ��� ���� ó��
                bool result;
                if(priceData.IsLargeUnit)
                    result = AssetMoneyCalculator.instance.ArithmeticOperation(0, totalPrice, false);
                else
                    result = AssetMoneyCalculator.instance.ArithmeticOperation(totalPrice, 0, false);

                // ���� ó�� ���� ��, ���� Ȯ�� ����
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
                // ��� �������� ���� �޽��� ���
                else
                    PlayManager.instance.LackOfMoney();
            }
            // �°� �� �������� ���� ���� �Ұ� �޽��� ���
            else
                PlayManager.instance.LackOfPassengerLimit();
        }

        // ���� UI �� �� �ʱ�ȭ �� ��Ȱ��ȭ
        expandAmount = 0;
        totalPrice = 0;
        expandAmountInputField.text = expandAmount.ToString();
        priceText.text = "����: 0$";
        passengerText.text = "�°� ��+ 0��";
        CheckMenu.SetActive(false);

        if (guide != null)
            guide.SetActive(false);
    }

    /// <summary>
    /// Ȯ�� ���� ����
    /// </summary>
    /// <param name="amount">����</param>
    public void SetExpandAmount(int amount)
    {
        // �ִ� ������ ���� �ʴ��� Ȯ��
        expandAmount = amount;
        int maximumAmount = lineCollection.lineData.trainExpandStatus[GetIndexFromLength(fromToLength[targetType, 0])];
        if (expandAmount > maximumAmount)
            expandAmount = maximumAmount;

        // ���� �� �ؽ�Ʈ ����
        SetTotalPrice();
        SetCheckTexts();
    }
    
    /// <summary>
    /// Ȯ�� ������ ��� ������ Ȯ���ϵ��� ����
    /// </summary>
    public void SetAmountAll()
    {
        SetExpandAmount(lineCollection.lineData.trainExpandStatus[GetIndexFromLength(fromToLength[targetType, 0])]);
    }

    /// <summary>
    /// Ȯ�� ��� �� ���� Ȯ�� �ؽ�Ʈ ����
    /// </summary>
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
            priceText.text = "����: 0$";
            passengerText.text = "�°� �� +0��";
        }
        else
        {
            priceText.text = "����: " + m2 + m1 + "$";
            passengerText.text = "�°� �� +" + p2 + p1 + "��";
        }
        expandAmountInputField.text = expandAmount.ToString();
    }

    /// <summary>
    /// ���� Ȯ�� ��Ȳ �ؽ�Ʈ ����
    /// </summary>
    public void SetTrainExpandText()
    {
        if (!priceData.IsLightRail)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Clear();
            for (int j = 0; j < 4; j++)
            {
                stringBuilder.Append(4 + j * 2);
                stringBuilder.Append("��(ĭ): ");
                stringBuilder.Append(lineCollection.lineData.trainExpandStatus[j]);
                stringBuilder.Append("��\n");
            }
            trainExpandStatusText.text = stringBuilder.ToString();
        }
    }

    /// <summary>
    /// ���� ĭ ���� �ε��� ���
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private int GetIndexFromLength(int num)
    {
        return (num - 4) / 2;
    }

    /// <summary>
    /// ���� ���� ������ ���� ����
    /// </summary>
    private void ApplyExpandTrain()
    {
        lineCollection.lineData.trainExpandStatus[GetIndexFromLength(fromToLength[targetType, 0])] -= expandAmount;
        lineCollection.lineData.trainExpandStatus[GetIndexFromLength(fromToLength[targetType, 1])] += expandAmount;
    }

    /// <summary>
    /// ���� ���� �� ���� ���
    /// </summary>
    private void SetTotalPrice()
    {
        totalPrice = priceData.TrainExpandPrice * ((ulong)(fromToLength[targetType, 1] - fromToLength[targetType, 0]) / 2) * (ulong)expandAmount;
        totalPassengerLow = priceData.TrainExapndPassenger * ((ulong)(fromToLength[targetType, 1] - fromToLength[targetType, 0]) / 2) * (ulong)expandAmount;
        totalPassengerHigh = 0;
        MoneyUnitTranslator.Arrange(ref totalPassengerLow, ref totalPassengerHigh);
    }
    /// <summary>
    /// Ȯ�� ���� �޽��� ���
    /// </summary>
    /// <param name="expand"></param>
    private void ExpandSuccess(int expand)
    {
        messageManager.ShowMessage("���� " + expand + "�븦 Ȯ���Ͽ����ϴ�.");
    }
}
