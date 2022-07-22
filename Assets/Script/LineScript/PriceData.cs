using UnityEngine;

[CreateAssetMenu(fileName = "Price Data", menuName = "Scriptable Object/Price Data", order = int.MaxValue)]
public class PriceData : ScriptableObject
{
    [SerializeField]
    private float[] trainPriceFactors;
    public float[] TrainPriceFactors { get { return trainPriceFactors; } }

    [SerializeField]
    private ulong trainPassenger;
    public ulong TrainPassenger { get { return trainPassenger; } }

    [SerializeField]
    private ulong trainPassengerAdd;
    public ulong TrainPassengerAdd { get { return trainPassengerAdd; } }

    [SerializeField]
    private float totalTouch;
    public float TotalTouch { get { return totalTouch; } }

    [SerializeField]
    private ulong uTotalTouch;
    public ulong UTotalTouch { get { return uTotalTouch; } }

    [SerializeField]
    private ulong basePrice;
    public ulong BasePrice { get { return basePrice; } }

    [SerializeField]
    private ulong basePriceEx;
    public ulong BasePriceEx { get { return basePriceEx; } }

    [SerializeField]
    private ulong basePriceAdd;
    public ulong BasePriceAdd { get { return basePriceAdd; } }

    [SerializeField]
    private ulong basePriceExAdd;
    public ulong BasePriceExAdd { get { return basePriceExAdd; } }

    [SerializeField]
    private ulong stationPrice;
    public ulong StationPrice { get { return stationPrice; } }

    [SerializeField]
    private Section[] sections;
    public Section[] Sections { get { return sections; } }

    [SerializeField]
    private ulong[] expandPrice;
    public ulong[] ExpandPrice { get { return expandPrice; } }

    [SerializeField]
    private ulong[] connectPrice;
    public ulong[] ConnectPrice { get { return connectPrice; } }

    [SerializeField]
    private ulong[] connectTimeMoney;
    public ulong[] ConnectTimeMoney { get { return connectTimeMoney; } }

    [SerializeField]
    private bool connectTMLargeUnit;
    /// <summary>
    /// 노선 연결의 시간형 수익 보상이 large unit인지
    /// </summary>
    public bool ConnectTMLargeUnit { get { return connectTMLargeUnit; } }

    [SerializeField]
    private ulong trainExpandPrice;
    public ulong TrainExpandPrice { get { return trainExpandPrice; } }

    [SerializeField]
    private ulong trainExpandPassenger;
    public ulong TrainExapndPassenger { get { return trainExpandPassenger; } }

    [SerializeField]
    private LargeVariable[] lineControlUpgradePrice;
    public LargeVariable[] LineControlUpgradePrice;

    [SerializeField]
    private LargeVariable[] lineControlUpgradePassenger;
    public LargeVariable[] LineControlUpgradePassenger;

    [SerializeField]
    private bool trainExPsngrLargeUnit;
    /// <summary>
    /// 열차 확장의 승객 수 보상이 large unit인지
    /// </summary>
    public bool TrainExPsngrLargeUnit { get { return trainExPsngrLargeUnit; } }

    [SerializeField]
    private ulong[] screenDoorPrice;
    public ulong[] ScreenDoorPrice { get { return screenDoorPrice; } }

    [SerializeField]
    private int[] screenDoorReputation;
    public int[] ScreenDoorReputation { get { return screenDoorReputation; } }

    [SerializeField]
    private bool isLargeUnit;
    public bool IsLargeUnit { get { return isLargeUnit; } }

    public void GetTrainPrice(int numOfTrain, ref LargeVariable price)
    {
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
    public ulong GetTrainPassenger(int numOfTrain)
    {
        return TrainPassenger + TrainPassengerAdd * (ulong)numOfTrain;
    }

    public ulong GetVehicleBasePrice(int numOfBase)
    {
        return basePrice + basePriceAdd * (ulong)numOfBase;
    }
    public ulong GetVehicleBaseExPrice(int numOfBaseEx)
    {
        return basePriceEx + basePriceExAdd * (ulong)numOfBaseEx;
    }
}

[System.Serializable]
public struct Section
{
    public string name;
    public int from;
    public int to;
}