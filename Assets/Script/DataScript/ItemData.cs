using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 컬러 아이템 카드 종류 열거형
/// </summary>
public enum ItemCardType { Red, Orange, Yellow, Green };

/// <summary>
/// 아이템 데이터 클래스
/// </summary>
[System.Serializable]
public class ItemData
{
    /// <summary>
    /// 각 종류별 컬러 카드 보유량
    /// </summary>
    public int[] cardAmounts;
    /// <summary>
    /// 각 종류별 레어 카드 보유량
    /// </summary>
    public int[] rareCardAmounts;

    /// <summary>
    /// 컬러 카드팩 보유량
    /// </summary>
    public int silverCardAmount;
    /// <summary>
    /// 레어 카드팩 보유량
    /// </summary>
    public int rareCardPackAmount;

    /// <summary>
    /// 카드 포인트 보유량
    /// </summary>
    public int cardPoint;
    /// <summary>
    /// 피버 리필 쿠폰 보유량
    /// </summary>
    public int feverRefillAmount;

    public ItemData()
    {
        cardAmounts = new int[5];
        rareCardAmounts = new int[6];
    }
}
