using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� �� �������� ���� ���� Ŭ����
/// </summary>
[System.Serializable]
public class PurchaseTrainManager : MonoBehaviour, IContinuousPurchase
{
    /// <summary>
    /// �뼱 �̸�
    /// </summary>
    public string lineName;

    /// <summary>
    /// ����ö �뼱�� �������� ���� ����
    /// </summary>
    private int lightRailBaseLimit = 1;
    /// <summary>
    /// �Ϲ� �뼱�� �������� ���� ����
    /// </summary>
    private int normalLineBaseLimit = 20;
    /// <summary>
    /// �ش� �뼱�� �������� ���� ���ѷ�
    /// </summary>
    public int BaseLimit { get { return priceData.IsLightRail ? lightRailBaseLimit : normalLineBaseLimit; } }

    /// <summary>
    /// �������� ���� ��, Ȯ��Ǵ� ���� ���� ���ѷ�
    /// </summary>
    public static int baseAdd = 10;
    /// <summary>
    /// �������� Ȯ�� ��, Ȯ��Ǵ� ���� ���� ���ѷ�
    /// </summary>
    public static int baseExpandAdd = 5;

    /// <summary>
    /// ���� ���� ���� ���� �ؽ�Ʈ
    /// </summary>
    public Text trainPrice_text;
    /// <summary>
    /// ���� ���� ���� ���� �ؽ�Ʈ
    /// </summary>
    public Text trainPassenger_text;
    /// <summary>
    /// �������� ���� ���� ���� �ؽ�Ʈ
    /// </summary>
    public Text basePrice_text;
    /// <summary>
    /// �������� Ȯ�� ���� ���� �ؽ�Ʈ
    /// </summary>
    public Text baseExPrice_text;

    /// <summary>
    /// �������� ��Ȳ ���� �ؽ�Ʈ
    /// </summary>
    public Text trainStatus_text;

    /// <summary>
    /// ���� ���Ű� ����Ǿ����� ���θ� �ȳ��ϴ� �̹��� ������Ʈ
    /// </summary>
    public GameObject trainImgLockObj;

    public PriceData priceData;
    public LineCollection lineCollection;
    public LineDataManager lineDataManager;
    public ExpandTrain expandTrain;
    public ButtonColorManager buttonColorManager;
    public MessageManager messageManager;
    public UpdateDisplay trainPurchaseUpdateDisplay;
    public UpdateDisplay trainConditionUpdateDisplay;
    public UpdateDisplay vehicleBaseUpdateDisplay;

    public ContinuousPurchase continuousPurchase;

    void Start()
    {
        trainConditionUpdateDisplay.onEnable += UpdateTrainStatusText;
        trainPurchaseUpdateDisplay.onEnable += CheckTrain;
        vehicleBaseUpdateDisplay.onEnable += CheckVehicleBase;
    }

    /// <summary>
    /// ���� ���� ó��
    /// </summary>
    public void PurchaseTrain()
    {
        // ���� ���� ���� Ȯ�� (���� ���� ���ѷ� �� �°� ��)
        if (lineCollection.lineData.numOfTrain < lineCollection.lineData.limitTrain && TouchMoneyManager.CheckLimitValid(priceData.GetTrainPassenger(lineCollection.lineData.numOfTrain), 0))
        {
            // ���� ���� ���
            LargeVariable price = LargeVariable.zero;
            priceData.GetTrainPrice(lineCollection.lineData.numOfTrain, ref price);

            // ��� ���� ó��
            bool result = AssetMoneyCalculator.instance.ArithmeticOperation(price.lowUnit, price.highUnit, false);

            // ��� ���� ���� ��,
            if (result)
            {
                // ���� ó�� �� ���� ���� �� UI ������Ʈ �� ����
                lineCollection.lineData.numOfTrain++;
                TouchMoneyManager.ArithmeticOperation(priceData.GetTrainPassenger(lineCollection.lineData.numOfTrain), 0, true);
                CompanyReputationManager.instance.RenewPassengerBase();
                if (!priceData.IsLightRail)
                {
                    lineCollection.lineData.trainExpandStatus[0]++;
                    expandTrain.SetTrainExpandText();
                }
                CheckTrain();
                UpdateTrainStatusText();
                DataManager.instance.SaveAll();
                buttonColorManager.SetTrainColor();
            }
            // ��� ���� ���� �޽��� ���
            else
                PlayManager.instance.LackOfMoney();
        }
        // �������� ���� ���� �޽��� ���
        else if (lineCollection.lineData.numOfTrain >= lineCollection.lineData.limitTrain)
            LackOfBase();
        // �°� �� ���� ���� �޽��� ���
        else
            PlayManager.instance.LackOfPassengerLimit();
    }

    /// <summary>
    /// ������ ���� �̺�Ʈ ������
    /// </summary>
    public void StartPurchase()
    {
        continuousPurchase.StartPurchase(PurchaseTrain);
    }
    /// <summary>
    /// ������ ���� �̺�Ʈ ������
    /// </summary>
    public void StopPurchase()
    {
        continuousPurchase.StopPurchase();
    }

    /// <summary>
    /// Ȯ����� �����ϰ� �ִ��� ���θ� Ȯ��
    /// </summary>
    /// <returns>�ش� �뼱 Ȯ��� ���� ����</returns>
    private bool IsExpanded()
    {
        for (int i = 0; i < lineCollection.lineData.sectionExpanded.Length; i++)
            if (lineCollection.lineData.sectionExpanded[i])
                return true;

        return false;
    }

    /// <summary>
    /// �������� ���� ó��
    /// </summary>
    /// <param name="isExpand">�������� Ȯ��/���� ����</param>
    public void PurchaseVehicleBase(bool isExpand)
    {
        // �������� ���� ��û ��
        if(!isExpand)
        {
            // Ȯ��� ���� ���� �˻�
            if (IsExpanded())
            {
                // �������� ���� ���ѷ� �˻�
                if (lineCollection.lineData.numOfBase < BaseLimit)
                {
                    // ��� ���� ó��
                    bool result;
                    if (priceData.IsLargeUnit)
                        result = AssetMoneyCalculator.instance.ArithmeticOperation(0, priceData.GetVehicleBasePrice(lineCollection.lineData.numOfBase), false);
                    else
                        result = AssetMoneyCalculator.instance.ArithmeticOperation(priceData.GetVehicleBasePrice(lineCollection.lineData.numOfBase), 0, false);

                    // ��� ���� ���� �� ���ѷ� Ȯ��, UI ������Ʈ �� ����
                    if (result)
                    {
                        lineCollection.lineData.limitTrain += baseAdd;
                        lineCollection.lineData.numOfBase++;
                        CheckVehicleBase();
                        UpdateTrainStatusText();
                        DataManager.instance.SaveAll();
                        PurchaseMessage(lineName);
                        buttonColorManager.SetVehicleBaseColor();
                    }
                    // ��� ���� ���� �޽��� ���
                    else
                        PlayManager.instance.LackOfMoney();
                }
                // �������� �ִ�ġ �ȳ� �޽��� ���
                else
                    BaseMax();
            }
            // Ȯ��� ���� �޽��� ���
            else
                ExpandFirst();
        }
        // �������� Ȯ�� ��û��
        else
        {
            // �������� ���� Ȯ��
            if (lineCollection.lineData.numOfBase > 0)
            {
                // �������� ���� ��� Ȯ�� ���� Ȯ��
                if (lineCollection.lineData.numOfBaseEx < lineCollection.lineData.numOfBase * 3)
                {
                    // ��� ���� ó��
                    bool result;
                    if (priceData.IsLargeUnit)
                        result = AssetMoneyCalculator.instance.ArithmeticOperation(0, priceData.GetVehicleBaseExPrice(lineCollection.lineData.numOfBaseEx), false);
                    else
                        result = AssetMoneyCalculator.instance.ArithmeticOperation(priceData.GetVehicleBaseExPrice(lineCollection.lineData.numOfBaseEx), 0, false);

                    // ��� ���� ���� �� �������� Ȯ�� ó�� �� UI ������Ʈ
                    if (result)
                    {
                        lineCollection.lineData.limitTrain += baseExpandAdd;
                        lineCollection.lineData.numOfBaseEx++;
                        CheckVehicleBase();
                        UpdateTrainStatusText();
                        DataManager.instance.SaveAll();
                        ExpandMessage(lineName);
                        buttonColorManager.SetVehicleBaseColor();
                    }
                    // ��� ���� ���� �޽��� ���
                    else
                        PlayManager.instance.LackOfMoney();
                }
                // �������� Ȯ�� �ִ�ġ �ȳ� �޽��� ���
                else
                    BaseMax();
            }
            // �������� ���� �޽��� ���
            else
                NoBase();
        }
    }

    /// <summary>
    /// ���� ���� �� �°��� ������ ǥ��
    /// </summary>
    /// <param name="isLargeNumber">������� �������� �ƴ���</param>
    public void CheckTrain()
    {
        trainImgLockObj.SetActive(!lineCollection.IsExpanded());

        string money1 = "", money2 = "";
        LargeVariable price = LargeVariable.zero;
        priceData.GetTrainPrice(lineCollection.lineData.numOfTrain, ref price);

        PlayManager.ArrangeUnit(price.lowUnit, price.highUnit, ref money1, ref money2, true);
        trainPrice_text.text = "����: " + money2 + money1 + "$";

        PlayManager.ArrangeUnit(priceData.GetTrainPassenger(lineCollection.lineData.numOfTrain), 0, ref money1, ref money2, true);
        trainPassenger_text.text = "�°� �� +" + money2 + money1 + "��";
    }

    /// <summary>
    /// �������� ���� ������ ǥ��
    /// </summary>
    /// <param name="isLargeNumber">�� ������ ������ �ƴ���</param>
    public void CheckVehicleBase()
    {
        string money1 = "", money2 = "";
        if (priceData.IsLargeUnit)
            PlayManager.ArrangeUnit(0, priceData.GetVehicleBasePrice(lineCollection.lineData.numOfBase), ref money1, ref money2, true);
        else
            PlayManager.ArrangeUnit(priceData.GetVehicleBasePrice(lineCollection.lineData.numOfBase), 0, ref money1, ref money2, true);
        basePrice_text.text = "����: " + money2 + money1 + "$";

        if (priceData.IsLargeUnit)
            PlayManager.ArrangeUnit(0, priceData.GetVehicleBaseExPrice(lineCollection.lineData.numOfBaseEx), ref money1, ref money2, true);
        else
            PlayManager.ArrangeUnit(priceData.GetVehicleBaseExPrice(lineCollection.lineData.numOfBaseEx), 0, ref money1, ref money2, true);
        baseExPrice_text.text = "����: " + money2 + money1 + "$";
    }

    /// <summary>
    /// ���� �������� ��Ȳ �ؽ�Ʈ ������Ʈ
    /// </summary>
    public void UpdateTrainStatusText()
    {
        trainStatus_text.text = lineName + ": " + lineCollection.lineData.numOfTrain + "/" + lineCollection.lineData.limitTrain + "��";
    }
    /// <summary>
    /// �������� ���� ���� �޽���
    /// </summary>
    private void LackOfBase()
    {
        messageManager.ShowMessage("���������� ������ �� �ִ� ������ �����մϴ�.");
    }
    /// <summary>
    /// Ȯ��� ���� ���� �޽���
    /// </summary>
    private void ExpandFirst()
    {
        messageManager.ShowMessage("�ش�뼱 Ȯ����� ���� �����Ͽ� �ֽʽÿ�.");
    }
    /// <summary>
    /// �������� ���� ���� �޽���
    /// </summary>
    private void NoBase()
    {
        messageManager.ShowMessage("Ȯ���� �� �ִ� ���������� �����ϴ�.");
    }
    /// <summary>
    /// �������� ���� �ִ�ġ �޽���
    /// </summary>
    private void BaseMax()
    {
        messageManager.ShowMessage("���� �� ���ִ� �ִ�ġ�Դϴ�.");
    }
    /// <summary>
    /// �������� ���� �޽���
    /// </summary>
    /// <param name="line">�뼱 �̸�</param>
    private void PurchaseMessage(string line)
    {
        messageManager.ShowMessage(line + " ���������� �����ϼ̽��ϴ�.");
    }
    /// <summary>
    /// �������� Ȯ�� �޽���
    /// </summary>
    /// <param name="line"></param>
    private void ExpandMessage(string line)
    {
        messageManager.ShowMessage(line + " ���������� Ȯ���ϼ̽��ϴ�.");
    }
}
