using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 기관사 데이터 클래스
/// </summary>
[System.Serializable]
public class DriverData
{
    /// <summary>
    /// 각 등급별 기관사 구매 횟수
    /// </summary>
    public int[] numOfDrivers;
    /// <summary>
    /// 월급 지출 남은 시간
    /// </summary>
    public int salaryTimer;

    public DriverData()
    {
        numOfDrivers = new int[11];
    }
}
