using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ڻ� ������ Ŭ����
/// </summary>
[System.Serializable]
public class MyAssetData
{
    /// <summary>
    /// ���� �ݾ� (Low: 0~9999��)
    /// </summary>
    public ulong moneyLow;
    /// <summary>
    /// ���� �ݾ� (High: 1��~)
    /// </summary>
    public ulong moneyHigh;

    /// <summary>
    /// ������ �� ����
    /// </summary>
    public int numOfStations;

    /// <summary>
    /// �°� �� (Low)
    /// </summary>
    public ulong passengersLow;
    /// <summary>
    /// �°� �� (High)
    /// </summary>
    public ulong passengersHigh;
    /// <summary>
    /// �°� �� ���ѷ� (Low)
    /// </summary>
    public ulong passengersLimitLow;
    /// <summary>
    /// �°� �� ���ѷ� (High)
    /// </summary>
    public ulong passengersLimitHigh;

    /// <summary>
    /// �ð��� ���� (Low)
    /// </summary>
    public ulong timePerEarningLow; 
    /// <summary>
    /// �ð��� ���� (High)
    /// </summary>
    public ulong timePerEarningHigh;

    /// <summary>
    /// ��ġ�� ���� �ǹ� ���� ��ġ
    /// </summary>
    public int feverStack;

    public MyAssetData()
    {
        passengersLow = 1;
        numOfStations = 3;
        passengersLimitLow = 100;
    }
}
