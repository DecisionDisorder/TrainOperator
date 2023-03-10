using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ��ȯ�� ���� ���̽��� �׽�Ʈ �ڵ�
/// </summary>
public class ValidTester: MonoBehaviour
{
    public MyAsset myAsset;
    public LineManager lineManager;
    public DriversManager driversManager;
    public RentManager rentManager;
    public BankManager bankManager;
    public BankSpecialManager bankSpecialManager;
    public CompanyReputationManager companyReputationManager;

    /// <summary>
    /// �뷱�� ������ ���� ���� Ŭ����
    /// </summary>
    public BalanceReviser balanceReviser;
    /// <summary>
    /// ��ȯ ��ȿ�� Ȯ�� ��� ������ ���� ������Ʈ
    /// </summary>
    private BalanceValidData data;

    private void Start()
    {
        ExecuteTest();
    }

    /// <summary>
    /// �׽�Ʈ ����
    /// </summary>
    public void ExecuteTest()
    {
        LoadJsonFile("reviseData");
        ApplyData();
        balanceReviser.IsRevised = false;
        balanceReviser.Revise();
        CheckValidity();
    }

    /// <summary>
    /// �׽�Ʈ ��� �뷱�� ������ �ҷ�����
    /// </summary>
    /// <param name="fileName">��� ������ ���� �̸�</param>
    private void LoadJsonFile(string fileName)
    {
        TextAsset mytxtData = Resources.Load<TextAsset>("Json/ReviseTest/" + fileName);
        string txt = mytxtData.text;
        if (txt != "" && txt != null)
        {
            string dataAsJson = txt;
            data = JsonUtility.FromJson<BalanceValidData>(dataAsJson);
        }
    }

    /// <summary>
    /// �ҷ��� �����͸� �ΰ��ӿ� ����
    /// </summary>
    private void ApplyData()
    {
        for (int i = 0; i < 3; i++)
        {
            bankManager.SavedMoney[i] = data.bankRevise[i].deposit;
            bankManager.AddedMoney[i] = data.bankRevise[i].interest;
            bankManager.IsRegistered[i] = data.bankRevise[i].isRegistered;
        }
        for (int i = 0; i < 3; i++)
        {
            bankSpecialManager.SavedMoneyHigh[i] = data.bankRevise[i + 3].deposit;
            bankSpecialManager.AddedMoneyHigh[i] = data.bankRevise[i + 3].interest;
            bankSpecialManager.IsRegistered[i] = data.bankRevise[i + 3].isRegistered;
        }

        myAsset.MoneyLow = data.normalRevise.moneyLowPrior;
        myAsset.MoneyHigh = data.normalRevise.moneyHighPrior;

        for (int i = 0; i < driversManager.numOfDrivers.Length; i++)
            driversManager.numOfDrivers[i] = data.driverRevise[i].numOfDrivers;

        for (int i = 0; i < data.lineSectionRevise.Length; i++)
        {
            int line = data.lineSectionRevise[i].line;
            for (int s = data.lineSectionRevise[i].sectionFrom; s <= data.lineSectionRevise[i].sectionTo; s++)
            {
                lineManager.lineCollections[line].lineData.installed[s] = data.lineSectionRevise[i].installed;
                lineManager.lineCollections[line].lineData.connected[s] = data.lineSectionRevise[i].connected;
                lineManager.lineCollections[line].lineData.sectionExpanded[s] = data.lineSectionRevise[i].expanded;
            }
        }
    }

    /// <summary>
    /// ��ȯ�� �����Ϳ��� �ս��� ������ Ȯ���Ͽ� �α� ���ڿ��� ��� �� �α�
    /// </summary>
    private void CheckValidity()
    {
        string log = "";
        if(data.normalRevise.moneyLowResult * 0.999f <= myAsset.MoneyLow && myAsset.MoneyLow <= data.normalRevise.moneyLowResult * 1.001f)
            log += "money low: <color=lime>OK</color>\n";
        else
            log += string.Format("money low: <color=red>FAILED</color> (result: {0}, expected: {1})\n", myAsset.MoneyLow, data.normalRevise.moneyLowResult);

        if (data.normalRevise.moneyHighResult * 0.999f <= myAsset.MoneyHigh && myAsset.MoneyHigh <= data.normalRevise.moneyHighResult * 1.001f)
            log += "money high: <color=lime>OK</color>\n";
        else
            log += string.Format("money high: <color=red>FAILED</color> (result: {0}, expected: {1})\n", myAsset.MoneyHigh, data.normalRevise.moneyHighResult);


        if (myAsset.TimePerEarningLow.Equals(data.normalRevise.timeMoneyLowResult))
            log += "time money low: <color=lime>OK</color>\n";
        else
            log += string.Format("time money low: <color=red>FAILED</color> (result: {0}, expected: {1})\n", myAsset.TimePerEarningLow, data.normalRevise.timeMoneyLowResult);

        if (myAsset.TimePerEarningHigh.Equals(data.normalRevise.timeMoneyHighResult))
            log += "time money high: <color=lime>OK</color>\n";
        else
            log += string.Format("time money high: <color=red>FAILED</color> (result: {0}, expected: {1})\n", myAsset.TimePerEarningHigh, data.normalRevise.timeMoneyHighResult);


        if (myAsset.PassengersLimitLow.Equals(data.normalRevise.passengerLimitLowResult))
            log += "passenger limit low: <color=lime>OK</color>\n";
        else
            log += string.Format("passenger limit low: <color=red>FAILED</color> (result: {0}, expected: {1})\n", myAsset.PassengersLimitLow, data.normalRevise.passengerLimitLowResult);

        if (myAsset.PassengersLimitHigh.Equals(data.normalRevise.passengerLimitHighResult))
            log += "passenger limit high: <color=lime>OK</color>\n";
        else
            log += string.Format("passenger limit high: <color=red>FAILED</color> (result: {0}, expected: {1})\n", myAsset.PassengersLimitHigh, data.normalRevise.passengerLimitHighResult);


        /*if (myAsset.PassengersLow.Equals(data.normalRevise.passengerLowResult))
            log += "passenger low: <color=lime>OK</color>\n";
        else
            log += string.Format("passenger low: <color=red>FAILED</color> (result: {0}, expected: {1})\n", myAsset.PassengersLow, data.normalRevise.passengerLowResult);

        if (myAsset.PassengersHigh.Equals(data.normalRevise.passengerHighResult))
            log += "passenger high: <color=lime>OK</color>\n";
        else
            log += string.Format("passenger high: <color=red>FAILED</color> (result: {0}, expected: {1})\n", myAsset.PassengersHigh, data.normalRevise.passengerHighResult);
        */

        if (companyReputationManager.ReputationValue.Equals(data.normalRevise.reputationResult))
            log += "reputation: <color=lime>OK</color>\n";
        else
            log += string.Format("reputation: <color=red>FAILED</color> (result: {0}, expected: {1})\n", companyReputationManager.ReputationValue, data.normalRevise.reputationResult);
        Debug.Log(log);
    }
}

/// <summary>
/// Json���� �ҷ��� �뷱�� ������ ������ Ŭ����
/// </summary>
[System.Serializable]
public class BalanceValidData
{
    public BankRevise[] bankRevise;
    public NormalRevise normalRevise;

    public DriverRevise[] driverRevise;
    public LineSectionRevise[] lineSectionRevise;
}

/// <summary>
/// Json���� �ҷ��� ���� ������ ������ Ŭ����
/// </summary>
[System.Serializable]
public class BankRevise
{
    public ulong deposit;
    public ulong interest;
    public bool isRegistered;
}

/// <summary>
/// Json���� �ҷ��� �Ϲ� ������ ������ Ŭ����
/// </summary>
[System.Serializable]
public class NormalRevise
{
    public ulong moneyLowPrior;
    public ulong moneyHighPrior;
    public ulong moneyLowResult;
    public ulong moneyHighResult;
    public ulong timeMoneyLowResult;
    public ulong timeMoneyHighResult;
    public ulong passengerLimitLowResult;
    public ulong passengerLimitHighResult;
    public ulong passengerLowResult;
    public ulong passengerHighResult;
    public int reputationResult;
}

/// <summary>
/// Json���� �ҷ��� ����� ������ ������ Ŭ����
/// </summary>
[System.Serializable]
public class DriverRevise
{
    public int numOfDrivers;
}

/// <summary>
/// Json���� �ҷ��� �뼱 ������ ������ Ŭ����
/// </summary>
[System.Serializable]
public class LineSectionRevise
{
    public int line;
    public int sectionFrom;
    public int sectionTo;
    public bool installed;
    public bool connected;
    public bool expanded;
}