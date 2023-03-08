using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 노선 콘텐츠 관련 버튼 색상 관리 클래스(노선별 할당)
/// </summary>
public class ButtonColorManager : MonoBehaviour
{
    /// <summary>
    /// 열차 구매 버튼 이미지
    /// </summary>
    public Image buyTrainImg;
    /// <summary>
    /// 차량기지 구매 버튼 이미지
    /// </summary>
    public Image vehicleBaseImg;
    /// <summary>
    /// 차량기지 확장 버튼 이미지
    /// </summary>
    public Image vehicleBaseExImg;
    /// <summary>
    /// 구간 별 확장권 구매 버튼 이미지
    /// </summary>
    public Image[] expandImgs;
    /// <summary>
    /// 구간 별 노선 연결 구매 버튼 이미지
    /// </summary>
    public Image[] connectImgs;
    /// <summary>
    /// 구간 별 스크린도어 구매 버튼 이미지
    /// </summary>
    public Image[] screendoorImgs;

    /// <summary>
    /// 노선 색상
    /// </summary>
    public Color lineColor;

    public LineCollection lineCollection;
    public PriceData priceData;

    /// <summary>
    /// 특정 메뉴가 활성화 되었을 때 정보를 업데이트를 도와주는 인스턴스
    /// </summary>
    public UpdateDisplay buyTrainUpdateDisplay;
    public UpdateDisplay vehicleBaseUpdateDiaplay;
    public UpdateDisplay expandUpdateDisplay;
    public UpdateDisplay connectionUpdateDisplay;
    public UpdateDisplay screenDoorUpdateDisplay;

    private void Start()
    {
        buyTrainUpdateDisplay.onEnableUpdate += SetTrainColor;
        vehicleBaseUpdateDiaplay.onEnableUpdate += SetVehicleBaseColor;
        expandUpdateDisplay.onEnableUpdate += SetColorExpand;
        connectionUpdateDisplay.onEnableUpdate += SetColorConnection;
        screenDoorUpdateDisplay.onEnableUpdate += SetColorScreenDoor;
    }

    /// <summary>
    /// 열차 구매 가능 여부에 따라 색상 지정
    /// </summary>
    public void SetTrainColor()
    {
        LargeVariable price = LargeVariable.zero;
        priceData.GetTrainPrice(lineCollection.lineData.numOfTrain, ref price);
        if (MyAsset.instance.Money >= price && lineCollection.lineData.numOfTrain < lineCollection.lineData.limitTrain)
        {
            if(TouchMoneyManager.CheckLimitValid(priceData.GetTrainPassenger(lineCollection.lineData.numOfTrain), 0))
            {
                buyTrainImg.color = lineColor;
                return;
            }
        }

        buyTrainImg.color = Color.gray;
    }

    /// <summary>
    /// 차량기지 구매/확장 가능 여부에 따라 색상 지정
    /// </summary>
    public void SetVehicleBaseColor()
    {
        if (IsEnoughMoney(priceData.GetVehicleBasePrice(lineCollection.lineData.numOfBase)) && IsExpandedAtLeast() && lineCollection.lineData.numOfBase < lineCollection.purchaseTrain.BaseLimit)
            vehicleBaseImg.color = lineColor;
        else
            vehicleBaseImg.color = Color.gray;

        if (IsEnoughMoney(priceData.GetVehicleBaseExPrice(lineCollection.lineData.numOfBaseEx)) && IsExpandedAtLeast() && lineCollection.lineData.numOfBaseEx < lineCollection.lineData.numOfBase * 3)
            vehicleBaseExImg.color = lineColor;
        else
            vehicleBaseExImg.color = Color.gray;
    }

    /// <summary>
    /// 해당 노선의 노선 확장권들 중에 하나라도 보유했는지 여부 확인
    /// </summary>
    private bool IsExpandedAtLeast()
    {
        for(int i = 0; i < lineCollection.lineData.sectionExpanded.Length; i++)
        {
            if (lineCollection.lineData.sectionExpanded[i])
                return true;
        }
        
        return false;
    }

    /// <summary>
    /// 특정 가격에 대해 충분한 돈을 가졌는지
    /// </summary>
    /// <param name="price">가격</param>
    /// <returns>구매 가능 여부</returns>
    private bool IsEnoughMoney(ulong price)
    {
        if (priceData.IsLargeUnit)
        {
            if (MyAsset.instance.MoneyHigh < price)
                return false;
            else
                return true;
        }
        else
        {
            if (MyAsset.instance.MoneyHigh > 0)
                return true;
            else if (MyAsset.instance.MoneyLow < price)
                return false;
            else
                return true;
        }
    }

    /// <summary>
    /// 모든 구간에 대해 확장권 구매 가능 여부에 따라 색상 지정
    /// </summary>
    public void SetColorExpand()
    {
        lineCollection.expandPurchase.SetQualification();
        for(int i = 0; i < expandImgs.Length; i++)
        {
            SetColorExpand(i);
        }
    }
    /// <summary>
    /// 특정 구간의 확장권 구매 가능 여부에 따라 색상 지정
    /// </summary>
    /// <param name="section">색상을 지정할 구간 번호</param>
    public void SetColorExpand(int section)
    {
        if (lineCollection.lineData.sectionExpanded[section])
            expandImgs[section].color = lineColor;
        else if (IsEnoughMoney(priceData.ConnectPrice[section]))
            expandImgs[section].color = Color.white;
        else
            expandImgs[section].color = Color.gray;
    }

    /// <summary>
    /// 모든 구간에 대해 노선 연결 가능 여부에 따라 색상 지정
    /// </summary>
    public void SetColorConnection()
    {
        for (int i = 0; i < connectImgs.Length; i++)
        {
            SetColorConnection(i);
        }
    }
    /// <summary>
    /// 특정 구간의 노선 연결 가능 여부에 따라 색상 지정
    /// </summary>
    /// <param name="section">색상을 지정할 구간 번호</param>
    public void SetColorConnection(int section)
    {
        if (!lineCollection.lineData.hasAllStations[section])
            lineCollection.lineConnection.CheckHasAllStations();

        if (lineCollection.lineData.connected[section])
            connectImgs[section].color = lineColor;
        else if (lineCollection.lineData.hasAllStations[section] && IsEnoughMoney(priceData.ConnectPrice[section]))
            connectImgs[section].color = Color.white;
        else
            connectImgs[section].color = Color.gray;
    }
    /// <summary>
    /// 모든 구간에 대해 스크린도어 설치 가능 여부에 따라 색상 지정
    /// </summary>
    public void SetColorScreenDoor()
    {
        for(int i = 0; i < screendoorImgs.Length; i++)
        {
            SetColorScreenDoor(i);
        }
    }
    /// <summary>
    /// 특정 구간의 스크린도어 설치 가능 여부에 따라 색상 지정
    /// </summary>
    /// <param name="section">색상을 지정할 구간 번호</param>
    public void SetColorScreenDoor(int section)
    {
        if(!lineCollection.lineData.hasAllStations[section])
            lineCollection.lineConnection.CheckHasAllStations();

        if (lineCollection.lineData.installed[section])
            screendoorImgs[section].color = lineColor;
        else if (lineCollection.lineData.hasAllStations[section] && IsEnoughMoney(priceData.ConnectPrice[section]))
            screendoorImgs[section].color = Color.white;
        else
            screendoorImgs[section].color = Color.gray;
    }
}
