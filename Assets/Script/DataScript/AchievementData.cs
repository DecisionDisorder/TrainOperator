using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AchievementData
{
    public int[] achvClearLevels;
    public LargeVariable maxMoney;
    public int touchCount;
    public int totalStationAmount;
    public LargeVariable cumulativeInterest;

    public AchievementData()
    {
        achvClearLevels = new int[9];
        maxMoney = LargeVariable.zero;
        cumulativeInterest = LargeVariable.zero;
    }
}
