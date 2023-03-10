using UnityEngine;

/// <summary>
/// 가격 정보 (Scriptable Object)
/// </summary>
[CreateAssetMenu(fileName = "Price Data", menuName = "Scriptable Object/Price Data", order = int.MaxValue)]
public class PriceData : ScriptableObject
{
    [SerializeField]
    private bool isLightRail;
    /// <summary>
    /// 경전철 여부
    /// </summary>
    public bool IsLightRail { get { return isLightRail; } }

    [SerializeField]
    private float[] trainPriceFactors;
    /// <summary>
    /// 열차 가격 결정 상수
    /// </summary>
    public float[] TrainPriceFactors { get { return trainPriceFactors; } }

    [SerializeField]
    private ulong trainPassenger;
    /// <summary>
    /// 열차 구매 보상(승객 수)
    /// </summary>
    public ulong TrainPassenger { get { return trainPassenger; } }

    [SerializeField]
    private ulong trainPassengerAdd;
    /// <summary>
    /// 구매 마다 증가하는 열차 구매 보상 수치 (승객 수)
    /// </summary>
    public ulong TrainPassengerAdd { get { return trainPassengerAdd; } }

    [SerializeField]
    private float totalTouch;
    /// <summary>
    /// 이전 노선의 예상 총합 터치 수 (float - 경 단위)
    /// </summary>
    public float TotalTouch { get { return totalTouch; } }

    [SerializeField]
    private ulong uTotalTouch;
    /// <summary>
    /// 이전 노선의 예상 총합 터치 수 (ulong)
    /// </summary>
    public ulong UTotalTouch { get { return uTotalTouch; } }

    [SerializeField]
    private ulong basePrice;
    /// <summary>
    /// 차량기지 가격
    /// </summary>
    public ulong BasePrice { get { return basePrice; } }

    [SerializeField]
    private ulong basePriceEx;
    /// <summary>
    /// 차량기지 확장 비용
    /// </summary>
    public ulong BasePriceEx { get { return basePriceEx; } }

    [SerializeField]
    private ulong basePriceAdd;
    /// <summary>
    /// 차량기지 구매 마다 증가하는 가격 수치
    /// </summary>
    public ulong BasePriceAdd { get { return basePriceAdd; } }

    [SerializeField]
    private ulong basePriceExAdd;
    /// <summary>
    /// 차량기지 확장 마다 증가하는 가격 수치
    /// </summary>
    public ulong BasePriceExAdd { get { return basePriceExAdd; } }

    [SerializeField]
    private ulong stationPrice;
    /// <summary>
    /// 역 가격
    /// </summary>
    public ulong StationPrice { get { return stationPrice; } }

    [SerializeField]
    private Section[] sections;
    /// <summary>
    /// 노선에 속한 구간 데이터
    /// </summary>
    public Section[] Sections { get { return sections; } }

    [SerializeField]
    private ulong[] expandPrice;
    /// <summary>
    /// 확장권 구매 비용
    /// </summary>
    public ulong[] ExpandPrice { get { return expandPrice; } }

    [SerializeField]
    private ulong[] connectPrice;
    /// <summary>
    /// 노선 연결 비용
    /// </summary>
    public ulong[] ConnectPrice { get { return connectPrice; } }

    [SerializeField]
    private ulong[] connectTimeMoney;
    /// <summary>
    /// 노선 연결 보상 (시간형 수익) 증가 수치
    /// </summary>
    public ulong[] ConnectTimeMoney { get { return connectTimeMoney; } }

    [SerializeField]
    private bool connectTMLargeUnit;
    /// <summary>
    /// 노선 연결의 시간형 수익 보상이 large unit(High)인지 여부
    /// </summary>
    public bool ConnectTMLargeUnit { get { return connectTMLargeUnit; } }

    [SerializeField]
    private ulong trainExpandPrice;
    /// <summary>
    /// 열차 1량 당 확장 비용
    /// </summary>
    public ulong TrainExpandPrice { get { return trainExpandPrice; } }

    [SerializeField]
    private ulong trainExpandPassenger;
    /// <summary>
    /// 열차 1량 당 보상 (승객 수)
    /// </summary>
    public ulong TrainExapndPassenger { get { return trainExpandPassenger; } }

    [SerializeField]
    private bool trainExPsngrLargeUnit;
    /// <summary>
    /// 열차 확장의 승객 수 보상이 large unit(High)인지 여부
    /// </summary>
    public bool TrainExPsngrLargeUnit { get { return trainExPsngrLargeUnit; } }

    [SerializeField]
    private LargeVariable[] lineControlUpgradePrice;
    /// <summary>
    /// 노선 제어 업그레이드 비용
    /// </summary>
    public LargeVariable[] LineControlUpgradePrice { get { return lineControlUpgradePrice; } }

    [SerializeField]
    private LargeVariable[] lineControlUpgradePassenger;
    /// <summary>
    /// 노선 제어 업그레이드 보상(승객 수)
    /// </summary>
    public LargeVariable[] LineControlUpgradePassenger { get { return lineControlUpgradePassenger; } }

    [SerializeField]
    private ulong[] screenDoorPrice;
    /// <summary>
    /// 스크린도어 설치 비용
    /// </summary>
    public ulong[] ScreenDoorPrice { get { return screenDoorPrice; } }

    [SerializeField]
    private int[] screenDoorReputation;
    /// <summary>
    /// 스크린도어 설치 보상(고객 만족도 증가 수치)
    /// </summary>
    public int[] ScreenDoorReputation { get { return screenDoorReputation; } }

    [SerializeField]
    private bool isLargeUnit;
    /// <summary>
    /// 기본 단위라 large unit(high) 인지 여부
    /// </summary>
    public bool IsLargeUnit { get { return isLargeUnit; } }

    /// <summary>
    /// 열차 구매 가격 정보 계산
    /// </summary>
    /// <param name="numOfTrain">기준이 되는 열차의 개수</param>
    /// <param name="price">계산 반영될 가격</param>
    public void GetTrainPrice(int numOfTrain, ref LargeVariable price)
    {
        // ulong / float 타입 중 선택하여 열차 가격 상수를 이용하여 가격 계산
        if (UTotalTouch != 0)
        {
            ulong x = (ulong)numOfTrain;
            ulong xf = x + 1;

            ulong touch = x * (2 * TrainPassenger + (x - 1) * TrainPassengerAdd) / 2 + UTotalTouch;
            price = new LargeVariable((ulong)((TrainPriceFactors[0] * xf * xf + TrainPriceFactors[1] * xf + TrainPriceFactors[2]) * touch), 0);
        }
        else
        {
            ulong criteriaUnit = 10000000000000000;
            int x = numOfTrain;
            ulong xf = (ulong)x + 1;

            float touch = x * (2 * (float)TrainPassenger / criteriaUnit + (x - 1) * (float)TrainPassengerAdd / criteriaUnit) / 2 + TotalTouch;
            float difficulty = (TrainPriceFactors[0] * xf * xf + TrainPriceFactors[1] * xf + TrainPriceFactors[2]);
            ulong highUnit = (ulong)(difficulty * touch);
            float fLowUnit = (difficulty * touch) - highUnit;
            ulong lowUnit = (ulong)(fLowUnit * criteriaUnit);
            price = new LargeVariable(lowUnit, highUnit);
        }
    }
    /// <summary>
    /// 열차 구매 보상 정보 계산
    /// </summary>
    /// <param name="numOfTrain">기준이 되는 열차의 개수</param>
    /// <returns>보상 승객 수</returns>
    public ulong GetTrainPassenger(int numOfTrain)
    {
        return TrainPassenger + TrainPassengerAdd * (ulong)numOfTrain;
    }

    /// <summary>
    /// 차량기지 구매 비용 계산
    /// </summary>
    /// <param name="numOfBase">구매한 차량기지 개수</param>
    /// <returns>최종 비용</returns>
    public ulong GetVehicleBasePrice(int numOfBase)
    {
        return basePrice + basePriceAdd * (ulong)numOfBase;
    }
    /// <summary>
    /// 차량기지 확장 비용 계산
    /// </summary>
    /// <param name="numOfBaseEx">확장한 횟수</param>
    /// <returns>최종 비용</returns>
    public ulong GetVehicleBaseExPrice(int numOfBaseEx)
    {
        return basePriceEx + basePriceExAdd * (ulong)numOfBaseEx;
    }
}

/// <summary>
/// 구간 정보 구조체
/// </summary>
[System.Serializable]
public struct Section
{
    /// <summary>
    /// 구간 이름 (xx역~yy역)
    /// </summary>
    public string name;
    /// <summary>
    /// 시작 index (포함)
    /// </summary>
    public int from;
    /// <summary>
    /// 종료 index (포함)
    /// </summary>
    public int to;
}