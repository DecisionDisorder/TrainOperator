using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 시설 임대 데이터 클래스
/// </summary>
[System.Serializable]
public class RentData
{
    /// <summary>
    /// 보유한 임대 시설의 개수
    /// </summary>
    public int[] numOfFacilities;
    /// <summary>
    /// 각 시설별 누적 시간형 수익
    /// </summary>
    public ulong[] cumulatedFacilityTimeMoney;
    /// <summary>
    /// 광고를 시청한 횟수
    /// </summary>
    public int numOfADWatch;

    /* Unused Variables */
    public int totalAccepted;
    public int rentSpaceAmount;
    public int checkEmpty;
    public int[] numOfRented;
    public ulong[] waitingRentTimeMoney = { 100, 1000, 5000, 20000};
    public string[] waitingRentNames;
    public string[] waitingRentTypes;
    public int quickRentCoolTime;

    public RentData()
    {
        quickRentCoolTime = 120;

        numOfFacilities = new int[4];
        cumulatedFacilityTimeMoney = new ulong[4];

        numOfRented = new int[7];
        waitingRentTimeMoney = new ulong[5];
        waitingRentNames = new string[5];
        waitingRentTypes = new string[5];
    }
}
