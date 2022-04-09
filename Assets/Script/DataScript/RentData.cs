using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RentData
{
    public int[] numOfFacilities;
    public ulong[] cumulatedFacilityTimeMoney;
    public int numOfADWatch;
 
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
