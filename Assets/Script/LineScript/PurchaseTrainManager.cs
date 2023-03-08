using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 열차 및 차량기지 구매 관리 클래스
/// </summary>
[System.Serializable]
public class PurchaseTrainManager : MonoBehaviour, IContinuousPurchase
{
    /// <summary>
    /// 노선 이름
    /// </summary>
    public string lineName;

    /// <summary>
    /// 경전철 노선의 차량기지 구매 제한
    /// </summary>
    private int lightRailBaseLimit = 1;
    /// <summary>
    /// 일반 노선의 차량기지 구매 제한
    /// </summary>
    private int normalLineBaseLimit = 20;
    /// <summary>
    /// 해당 노선의 차량기지 구매 제한량
    /// </summary>
    public int BaseLimit { get { return priceData.IsLightRail ? lightRailBaseLimit : normalLineBaseLimit; } }

    /// <summary>
    /// 차량기지 구매 시, 확장되는 열차 구매 제한량
    /// </summary>
    public static int baseAdd = 10;
    /// <summary>
    /// 차량기지 확장 시, 확장되는 열차 구매 제한량
    /// </summary>
    public static int baseExpandAdd = 5;

    /// <summary>
    /// 열차 구매 가격 정보 텍스트
    /// </summary>
    public Text trainPrice_text;
    /// <summary>
    /// 열차 구매 보상 정보 텍스트
    /// </summary>
    public Text trainPassenger_text;
    /// <summary>
    /// 차량기지 구매 가격 정보 텍스트
    /// </summary>
    public Text basePrice_text;
    /// <summary>
    /// 차량기지 확장 가격 정보 텍스트
    /// </summary>
    public Text baseExPrice_text;

    /// <summary>
    /// 차량기지 현황 정보 텍스트
    /// </summary>
    public Text trainStatus_text;

    /// <summary>
    /// 열차 구매가 언락되었는지 여부를 안내하는 이미지 오브젝트
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
    /// 열차 구매 처리
    /// </summary>
    public void PurchaseTrain()
    {
        // 구매 가능 조건 확인 (열차 구매 제한량 및 승객 수)
        if (lineCollection.lineData.numOfTrain < lineCollection.lineData.limitTrain && TouchMoneyManager.CheckLimitValid(priceData.GetTrainPassenger(lineCollection.lineData.numOfTrain), 0))
        {
            // 가격 정보 계산
            LargeVariable price = LargeVariable.zero;
            priceData.GetTrainPrice(lineCollection.lineData.numOfTrain, ref price);

            // 비용 지불 처리
            bool result = AssetMoneyCalculator.instance.ArithmeticOperation(price.lowUnit, price.highUnit, false);

            // 비용 지불 성공 시,
            if (result)
            {
                // 구매 처리 및 보상 지급 후 UI 업데이트 및 저장
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
            // 비용 부족 관련 메시지 출력
            else
                PlayManager.instance.LackOfMoney();
        }
        // 차량기지 공간 부족 메시지 출력
        else if (lineCollection.lineData.numOfTrain >= lineCollection.lineData.limitTrain)
            LackOfBase();
        // 승객 수 제한 부족 메시지 출력
        else
            PlayManager.instance.LackOfPassengerLimit();
    }

    /// <summary>
    /// 간편구매 시작 이벤트 리스너
    /// </summary>
    public void StartPurchase()
    {
        continuousPurchase.StartPurchase(PurchaseTrain);
    }
    /// <summary>
    /// 간편구매 종료 이벤트 리스너
    /// </summary>
    public void StopPurchase()
    {
        continuousPurchase.StopPurchase();
    }

    /// <summary>
    /// 확장권을 보유하고 있는지 여부를 확인
    /// </summary>
    /// <returns>해당 노선 확장권 보유 여부</returns>
    private bool IsExpanded()
    {
        for (int i = 0; i < lineCollection.lineData.sectionExpanded.Length; i++)
            if (lineCollection.lineData.sectionExpanded[i])
                return true;

        return false;
    }

    /// <summary>
    /// 차량기지 구매 처리
    /// </summary>
    /// <param name="isExpand">차량기지 확장/구매 여부</param>
    public void PurchaseVehicleBase(bool isExpand)
    {
        // 차량기지 구매 요청 시
        if(!isExpand)
        {
            // 확장권 보유 여부 검사
            if (IsExpanded())
            {
                // 차량기지 구매 제한량 검사
                if (lineCollection.lineData.numOfBase < BaseLimit)
                {
                    // 비용 지불 처리
                    bool result;
                    if (priceData.IsLargeUnit)
                        result = AssetMoneyCalculator.instance.ArithmeticOperation(0, priceData.GetVehicleBasePrice(lineCollection.lineData.numOfBase), false);
                    else
                        result = AssetMoneyCalculator.instance.ArithmeticOperation(priceData.GetVehicleBasePrice(lineCollection.lineData.numOfBase), 0, false);

                    // 비용 지불 성공 시 제한량 확장, UI 업데이트 및 저장
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
                    // 비용 부족 관련 메시지 출력
                    else
                        PlayManager.instance.LackOfMoney();
                }
                // 차량기지 최대치 안내 메시지 출력
                else
                    BaseMax();
            }
            // 확장권 구매 메시지 출력
            else
                ExpandFirst();
        }
        // 차량기지 확장 요청시
        else
        {
            // 차량기지 개수 확인
            if (lineCollection.lineData.numOfBase > 0)
            {
                // 차량기지 개수 대비 확장 개수 확인
                if (lineCollection.lineData.numOfBaseEx < lineCollection.lineData.numOfBase * 3)
                {
                    // 비용 지불 처리
                    bool result;
                    if (priceData.IsLargeUnit)
                        result = AssetMoneyCalculator.instance.ArithmeticOperation(0, priceData.GetVehicleBaseExPrice(lineCollection.lineData.numOfBaseEx), false);
                    else
                        result = AssetMoneyCalculator.instance.ArithmeticOperation(priceData.GetVehicleBaseExPrice(lineCollection.lineData.numOfBaseEx), 0, false);

                    // 비용 지불 성공 시 차량기지 확장 처리 및 UI 업데이트
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
                    // 비용 부족 관련 메시지 출력
                    else
                        PlayManager.instance.LackOfMoney();
                }
                // 차량기지 확장 최대치 안내 메시지 출력
                else
                    BaseMax();
            }
            // 차량기지 보유 메시지 출력
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

    /// <summary>
    /// 열차 차량기지 현황 텍스트 업데이트
    /// </summary>
    public void UpdateTrainStatusText()
    {
        trainStatus_text.text = lineName + ": " + lineCollection.lineData.numOfTrain + "/" + lineCollection.lineData.limitTrain + "개";
    }
    /// <summary>
    /// 차량기지 보관 부족 메시지
    /// </summary>
    private void LackOfBase()
    {
        messageManager.ShowMessage("차량기지에 보관할 수 있는 공간이 부족합니다.");
    }
    /// <summary>
    /// 확장권 구매 권유 메시지
    /// </summary>
    private void ExpandFirst()
    {
        messageManager.ShowMessage("해당노선 확장권을 먼저 구입하여 주십시오.");
    }
    /// <summary>
    /// 차량기지 구매 권유 메시지
    /// </summary>
    private void NoBase()
    {
        messageManager.ShowMessage("확장할 수 있는 차량기지가 없습니다.");
    }
    /// <summary>
    /// 차량기지 구매 최대치 메시지
    /// </summary>
    private void BaseMax()
    {
        messageManager.ShowMessage("구매 할 수있는 최대치입니다.");
    }
    /// <summary>
    /// 차량기지 구매 메시지
    /// </summary>
    /// <param name="line">노선 이름</param>
    private void PurchaseMessage(string line)
    {
        messageManager.ShowMessage(line + " 차량기지를 구입하셨습니다.");
    }
    /// <summary>
    /// 차량기지 확장 메시지
    /// </summary>
    /// <param name="line"></param>
    private void ExpandMessage(string line)
    {
        messageManager.ShowMessage(line + " 차량기지를 확장하셨습니다.");
    }
}
