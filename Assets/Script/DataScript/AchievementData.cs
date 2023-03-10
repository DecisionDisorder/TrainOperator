using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 도전과제 데이터 클래스
/// </summary>
[System.Serializable]
public class AchievementData
{
    /// <summary>
    /// 각 도전과제의 레벨
    /// </summary>
    public int[] achvClearLevels;
    /// <summary>
    /// 사용자가 보유했던 최대의 돈
    /// </summary>
    public LargeVariable maxMoney;
    /// <summary>
    /// 사용자가 터치메뉴를 터치한 횟수
    /// </summary>
    public int touchCount;
    /// <summary>
    /// 사용자가 구매한 역의 개수
    /// </summary>
    public int totalStationAmount;
    /// <summary>
    /// 사용자가 획득한 누적 이자
    /// </summary>
    public LargeVariable cumulativeInterest;

    public AchievementData()
    {
        achvClearLevels = new int[9];
        maxMoney = LargeVariable.zero;
        cumulativeInterest = LargeVariable.zero;
    }
}
