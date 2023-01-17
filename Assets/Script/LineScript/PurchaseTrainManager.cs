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
    /// 열차 가격 및 승객수 데이터 표시
    /// </summary>
    /// <param name="isLargeNumber">경단위의 숫자인지 아닌지</param>
    public void CheckTrain()
    {
        trainImgLockObj.SetActive(!lineCollection.IsExpanded());

        string money1 = "", money2 = "";
        LargeVariable price = LargeVariable.zero;
        priceData.GetTrainPrice(lineCollection.lineData.numOfTrain, ref price);

        PlayManager.ArrangeUnit(price.lowUnit, price.highUnit, ref money1, ref money2, true);
        trainPrice_text.text = "가격: " + money2 + money1 + "$";

        PlayManager.ArrangeUnit(priceData.GetTrainPassenger(lineCollection.lineData.numOfTrain), 0, ref money1, ref money2, true);
        trainPassenger_text.text = "승객 수 +" + money2 + money1 + "명";
    }

    /// <summary>
    /// 차량기지 가격 데이터 표시
    /// </summary>
    /// <param name="isLargeNumber">경 단위의 돈인지 아닌지</param>
    public void CheckVehicleBase()
    {
        string money1 = "", money2 = "";
        if (priceData.IsLargeUnit)
            PlayManager.ArrangeUnit(0, priceData.GetVehicleBasePrice(lineCollection.lineData.numOfBase), ref money1, ref money2, true);
        else
            PlayManager.ArrangeUnit(priceData.GetVehicleBasePrice(lineCollection.lineData.numOfBase), 0, ref money1, ref money2, true);
        basePrice_text.text = "가격: " + money2 + money1 + "$";

        if (priceData.IsLargeUnit)
            PlayManager.ArrangeUnit(0, priceData.GetVehicleBaseExPrice(lineCollection.lineData.numOfBaseEx), ref money1, ref money2, true);
        else
            PlayManager.ArrangeUnit(priceData.GetVehicleBaseExPrice(lineCollection.lineData.numOfBaseEx), 0, ref money1, ref money2, true);
        baseExPrice_text.text = "가격: " + money2 + money1 + "$";
    }

    public void UpdateTrainStatusText()
    {
        trainStatus_text.text = lineName + ": " + lineCollection.lineData.numOfTrain + "/" + lineCollection.lineData.limitTrain + "개";
    }
    private void LackOfBase()
    {
        messageManager.ShowMessage("차량기지에 보관할 수 있는 공간이 부족합니다.");
    }
    private void ExpandFirst()
    {
        messageManager.ShowMessage("해당노선 확장권을 먼저 구입하여 주십시오.");
    }
    private void NoBase()
    {
        messageManager.ShowMessage("확장할 수 있는 차량기지가 없습니다.");
    }
    private void BaseMax()
    {
        messageManager.ShowMessage("구매 할 수있는 최대치입니다.");
    }
    private void PurchaseMessage(string line)
    {
        messageManager.ShowMessage(line + " 차량기지를 구입하셨습니다.");
    }
    private void ExpandMessage(string line)
    {
        messageManager.ShowMessage(line + " 차량기지를 확장하셨습니다.");
    }
}
