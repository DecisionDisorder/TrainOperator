using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 은행 데이터 클래스
/// </summary>
[System.Serializable]
public class BankData
{
    /// <summary>
    /// 예치 금액: 일반 상품
    /// </summary>
    public ulong[] savedMoneyNormal;
    /// <summary>
    /// 이자 금액: 일반 상품
    /// </summary>
    public ulong[] addedMoneyNormal;
    /// <summary>
    /// 계약 시간: 일반 상품
    /// </summary>
    public int[] contractTimesNormal;
    /// <summary>
    /// 가입 여부: 일반 상품
    /// </summary>
    public bool[] isRegisteredNormal;

    /// <summary>
    /// 예치 금액: 스페셜 상품
    /// </summary>
    public ulong[] savedMoneySpecial;
    /// <summary>
    /// 이자 금액-Low 단위(0~9999조): 스페셜 상품
    /// </summary>
    public ulong[] addedMoneySpecialLow;
    /// <summary>
    /// 이자 금액-High 단위(1경~): 스페셜 상품
    /// </summary>
    public ulong[] addedMoneySpecialHigh;
    /// <summary>
    /// 계약 시간: 스페셜 상품 
    /// </summary>
    public int[] contractTimesSpecial;
    /// <summary>
    /// 가입 여부: 스페셜 상품
    /// </summary>
    public bool[] isRegisteredSpecial;

    /// <summary>
    /// 남은 이자 지급 시간: 스페셜 상품
    /// </summary>
    public int timer;

    public BankData()
    {
        savedMoneyNormal = new ulong[3];
        addedMoneyNormal = new ulong[3];
        contractTimesNormal = new int[3];
        isRegisteredNormal = new bool[3];

        savedMoneySpecial = new ulong[3];
        addedMoneySpecialHigh = new ulong[3];
        addedMoneySpecialLow = new ulong[3];
        contractTimesSpecial = new int[3];
        isRegisteredSpecial = new bool[3];
    }
}
