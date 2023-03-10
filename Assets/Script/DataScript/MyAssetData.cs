using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 자산 데이터 클래스
/// </summary>
[System.Serializable]
public class MyAssetData
{
    /// <summary>
    /// 보유 금액 (Low: 0~9999조)
    /// </summary>
    public ulong moneyLow;
    /// <summary>
    /// 보유 금액 (High: 1경~)
    /// </summary>
    public ulong moneyHigh;

    /// <summary>
    /// 구매한 역 개수
    /// </summary>
    public int numOfStations;

    /// <summary>
    /// 승객 수 (Low)
    /// </summary>
    public ulong passengersLow;
    /// <summary>
    /// 승객 수 (High)
    /// </summary>
    public ulong passengersHigh;
    /// <summary>
    /// 승객 수 제한량 (Low)
    /// </summary>
    public ulong passengersLimitLow;
    /// <summary>
    /// 승객 수 제한량 (High)
    /// </summary>
    public ulong passengersLimitHigh;

    /// <summary>
    /// 시간형 수익 (Low)
    /// </summary>
    public ulong timePerEarningLow; 
    /// <summary>
    /// 시간형 수익 (High)
    /// </summary>
    public ulong timePerEarningHigh;

    /// <summary>
    /// 터치로 쌓은 피버 스택 수치
    /// </summary>
    public int feverStack;

    public MyAssetData()
    {
        passengersLow = 1;
        numOfStations = 3;
        passengersLimitLow = 100;
    }
}
