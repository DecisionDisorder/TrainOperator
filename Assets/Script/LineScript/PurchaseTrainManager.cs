using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PurchaseTrainManager : MonoBehaviour, IContinuousPurchase
{
    public string lineName;

    private int lightRailBaseLimit = 1;
    private int normalLineBaseLimit = 20;
    public int BaseLimit { get { return priceData.IsLightRail ? lightRailBaseLimit : normalLineBaseLimit; } }
    public static int baseAdd = 10;
    public static int baseExpandAdd = 5;

    public Text trainPrice_text;
    public Text trainPassenger_text;
    public Text basePrice_text;
    public Text baseExPrice_text;

    public Text trainStatus_text;

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

    public void PurchaseTrain()
    {
        if (lineCollection.lineData.numOfTrain < lineCollection.lineData.limitTrain && TouchMoneyManager.CheckLimitValid(priceData.GetTrainPassenger(lineCollection.lineData.numOfTrain), 0))
        {
            LargeVariable price = LargeVariable.zero;
            priceData.GetTrainPrice(lineCollection.lineData.numOfTrain, ref price);

            bool result = AssetMoneyCalculator.instance.ArithmeticOperation(price.lowUnit, price.highUnit, false);

            if (result)
            {
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
            else
                PlayManager.instance.LackOfMoney();
        }
        else if (lineCollection.lineData.numOfTrain >= lineCollection.lineData.limitTrain)
            LackOfBase();
        else
            PlayManager.instance.LackOfPassengerLimit();
    }

    public void StartPurchase()
    {
        continuousPurchase.StartPurchase(PurchaseTrain);
    }
    public void StopPurchase()
    {
        continuousPurchase.StopPurchase();
    }

    private bool IsExpanded()
    {
        for (int i = 0; i < lineCollection.lineData.sectionExpanded.Length; i++)
            if (lineCollection.lineData.sectionExpanded[i])
                return true;

        return false;
    }

    public void PurchaseVehicleBase(bool isExpand)
    {
        if(!isExpand)
        {
            if (IsExpanded())
            {
                if (lineCollection.lineData.numOfBase < BaseLimit)
                {
                    bool result;
                    if (priceData.IsLargeUnit)
                        result = AssetMoneyCalculator.instance.ArithmeticOperation(0, priceData.GetVehicleBasePrice(lineCollection.lineData.numOfBase), false);
                    else
                        result = AssetMoneyCalculator.instance.ArithmeticOperation(priceData.GetVehicleBasePrice(lineCollection.lineData.numOfBase), 0, false);

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
                    else
                        PlayManager.instance.LackOfMoney();
                }
                else
                    BaseMax();
            }
            else
                ExpandFirst();
        }
        else
        {
            if (lineCollection.lineData.numOfBase > 0)
            {
                if (lineCollection.lineData.numOfBaseEx < lineCollection.lineData.numOfBase * 3)
                {
                    bool result;
                    if (priceData.IsLargeUnit)
                        result = AssetMoneyCalculator.instance.ArithmeticOperation(0, priceData.GetVehicleBaseExPrice(lineCollection.lineData.numOfBaseEx), false);
                    else
                        result = AssetMoneyCalculator.instance.ArithmeticOperation(priceData.GetVehicleBaseExPrice(lineCollection.lineData.numOfBaseEx), 0, false);

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
                    else
                        PlayManager.instance.LackOfMoney();
                }
                else
                    BaseMax();
            }
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

    public void UpdateTrainStatusText()
    {
        trainStatus_text.text = lineName + ": " + lineCollection.lineData.numOfTrain + "/" + lineCollection.lineData.limitTrain + "��";
    }
    private void LackOfBase()
    {
        messageManager.ShowMessage("���������� ������ �� �ִ� ������ �����մϴ�.");
    }
    private void ExpandFirst()
    {
        messageManager.ShowMessage("�ش�뼱 Ȯ����� ���� �����Ͽ� �ֽʽÿ�.");
    }
    private void NoBase()
    {
        messageManager.ShowMessage("Ȯ���� �� �ִ� ���������� �����ϴ�.");
    }
    private void BaseMax()
    {
        messageManager.ShowMessage("���� �� ���ִ� �ִ�ġ�Դϴ�.");
    }
    private void PurchaseMessage(string line)
    {
        messageManager.ShowMessage(line + " ���������� �����ϼ̽��ϴ�.");
    }
    private void ExpandMessage(string line)
    {
        messageManager.ShowMessage(line + " ���������� Ȯ���ϼ̽��ϴ�.");
    }
}
