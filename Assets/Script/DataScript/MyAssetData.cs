using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MyAssetData
{
    public ulong moneyLow;
    public ulong moneyHigh;

    public int numOfStations;

    public ulong passengersLow;
    public ulong passengersHigh;
    public ulong passengersLimitLow;
    public ulong passengersLimitHigh;

    public ulong timePerEarningLow; // timemoney
    public ulong timePerEarningHigh;

    public MyAssetData()
    {
        passengersLow = 1;
        numOfStations = 3;
        passengersLimitLow = 100;
    }
}
