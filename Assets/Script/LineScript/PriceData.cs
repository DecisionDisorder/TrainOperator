using UnityEngine;

/// <summary>
/// ���� ���� (Scriptable Object)
/// </summary>
[CreateAssetMenu(fileName = "Price Data", menuName = "Scriptable Object/Price Data", order = int.MaxValue)]
public class PriceData : ScriptableObject
{
    [SerializeField]
    private bool isLightRail;
    /// <summary>
    /// ����ö ����
    /// </summary>
    public bool IsLightRail { get { return isLightRail; } }

    [SerializeField]
    private float[] trainPriceFactors;
    /// <summary>
    /// ���� ���� ���� ���
    /// </summary>
    public float[] TrainPriceFactors { get { return trainPriceFactors; } }

    [SerializeField]
    private ulong trainPassenger;
    /// <summary>
    /// ���� ���� ����(�°� ��)
    /// </summary>
    public ulong TrainPassenger { get { return trainPassenger; } }

    [SerializeField]
    private ulong trainPassengerAdd;
    /// <summary>
    /// ���� ���� �����ϴ� ���� ���� ���� ��ġ (�°� ��)
    /// </summary>
    public ulong TrainPassengerAdd { get { return trainPassengerAdd; } }

    [SerializeField]
    private float totalTouch;
    /// <summary>
    /// ���� �뼱�� ���� ���� ��ġ �� (float - �� ����)
    /// </summary>
    public float TotalTouch { get { return totalTouch; } }

    [SerializeField]
    private ulong uTotalTouch;
    /// <summary>
    /// ���� �뼱�� ���� ���� ��ġ �� (ulong)
    /// </summary>
    public ulong UTotalTouch { get { return uTotalTouch; } }

    [SerializeField]
    private ulong basePrice;
    /// <summary>
    /// �������� ����
    /// </summary>
    public ulong BasePrice { get { return basePrice; } }

    [SerializeField]
    private ulong basePriceEx;
    /// <summary>
    /// �������� Ȯ�� ���
    /// </summary>
    public ulong BasePriceEx { get { return basePriceEx; } }

    [SerializeField]
    private ulong basePriceAdd;
    /// <summary>
    /// �������� ���� ���� �����ϴ� ���� ��ġ
    /// </summary>
    public ulong BasePriceAdd { get { return basePriceAdd; } }

    [SerializeField]
    private ulong basePriceExAdd;
    /// <summary>
    /// �������� Ȯ�� ���� �����ϴ� ���� ��ġ
    /// </summary>
    public ulong BasePriceExAdd { get { return basePriceExAdd; } }

    [SerializeField]
    private ulong stationPrice;
    /// <summary>
    /// �� ����
    /// </summary>
    public ulong StationPrice { get { return stationPrice; } }

    [SerializeField]
    private Section[] sections;
    /// <summary>
    /// �뼱�� ���� ���� ������
    /// </summary>
    public Section[] Sections { get { return sections; } }

    [SerializeField]
    private ulong[] expandPrice;
    /// <summary>
    /// Ȯ��� ���� ���
    /// </summary>
    public ulong[] ExpandPrice { get { return expandPrice; } }

    [SerializeField]
    private ulong[] connectPrice;
    /// <summary>
    /// �뼱 ���� ���
    /// </summary>
    public ulong[] ConnectPrice { get { return connectPrice; } }

    [SerializeField]
    private ulong[] connectTimeMoney;
    /// <summary>
    /// �뼱 ���� ���� (�ð��� ����) ���� ��ġ
    /// </summary>
    public ulong[] ConnectTimeMoney { get { return connectTimeMoney; } }

    [SerializeField]
    private bool connectTMLargeUnit;
    /// <summary>
    /// �뼱 ������ �ð��� ���� ������ large unit(High)���� ����
    /// </summary>
    public bool ConnectTMLargeUnit { get { return connectTMLargeUnit; } }

    [SerializeField]
    private ulong trainExpandPrice;
    /// <summary>
    /// ���� 1�� �� Ȯ�� ���
    /// </summary>
    public ulong TrainExpandPrice { get { return trainExpandPrice; } }

    [SerializeField]
    private ulong trainExpandPassenger;
    /// <summary>
    /// ���� 1�� �� ���� (�°� ��)
    /// </summary>
    public ulong TrainExapndPassenger { get { return trainExpandPassenger; } }

    [SerializeField]
    private bool trainExPsngrLargeUnit;
    /// <summary>
    /// ���� Ȯ���� �°� �� ������ large unit(High)���� ����
    /// </summary>
    public bool TrainExPsngrLargeUnit { get { return trainExPsngrLargeUnit; } }

    [SerializeField]
    private LargeVariable[] lineControlUpgradePrice;
    /// <summary>
    /// �뼱 ���� ���׷��̵� ���
    /// </summary>
    public LargeVariable[] LineControlUpgradePrice { get { return lineControlUpgradePrice; } }

    [SerializeField]
    private LargeVariable[] lineControlUpgradePassenger;
    /// <summary>
    /// �뼱 ���� ���׷��̵� ����(�°� ��)
    /// </summary>
    public LargeVariable[] LineControlUpgradePassenger { get { return lineControlUpgradePassenger; } }

    [SerializeField]
    private ulong[] screenDoorPrice;
    /// <summary>
    /// ��ũ������ ��ġ ���
    /// </summary>
    public ulong[] ScreenDoorPrice { get { return screenDoorPrice; } }

    [SerializeField]
    private int[] screenDoorReputation;
    /// <summary>
    /// ��ũ������ ��ġ ����(�� ������ ���� ��ġ)
    /// </summary>
    public int[] ScreenDoorReputation { get { return screenDoorReputation; } }

    [SerializeField]
    private bool isLargeUnit;
    /// <summary>
    /// �⺻ ������ large unit(high) ���� ����
    /// </summary>
    public bool IsLargeUnit { get { return isLargeUnit; } }

    /// <summary>
    /// ���� ���� ���� ���� ���
    /// </summary>
    /// <param name="numOfTrain">������ �Ǵ� ������ ����</param>
    /// <param name="price">��� �ݿ��� ����</param>
    public void GetTrainPrice(int numOfTrain, ref LargeVariable price)
    {
        // ulong / float Ÿ�� �� �����Ͽ� ���� ���� ����� �̿��Ͽ� ���� ���
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
    /// ���� ���� ���� ���� ���
    /// </summary>
    /// <param name="numOfTrain">������ �Ǵ� ������ ����</param>
    /// <returns>���� �°� ��</returns>
    public ulong GetTrainPassenger(int numOfTrain)
    {
        return TrainPassenger + TrainPassengerAdd * (ulong)numOfTrain;
    }

    /// <summary>
    /// �������� ���� ��� ���
    /// </summary>
    /// <param name="numOfBase">������ �������� ����</param>
    /// <returns>���� ���</returns>
    public ulong GetVehicleBasePrice(int numOfBase)
    {
        return basePrice + basePriceAdd * (ulong)numOfBase;
    }
    /// <summary>
    /// �������� Ȯ�� ��� ���
    /// </summary>
    /// <param name="numOfBaseEx">Ȯ���� Ƚ��</param>
    /// <returns>���� ���</returns>
    public ulong GetVehicleBaseExPrice(int numOfBaseEx)
    {
        return basePriceEx + basePriceExAdd * (ulong)numOfBaseEx;
    }
}

/// <summary>
/// ���� ���� ����ü
/// </summary>
[System.Serializable]
public struct Section
{
    /// <summary>
    /// ���� �̸� (xx��~yy��)
    /// </summary>
    public string name;
    /// <summary>
    /// ���� index (����)
    /// </summary>
    public int from;
    /// <summary>
    /// ���� index (����)
    /// </summary>
    public int to;
}