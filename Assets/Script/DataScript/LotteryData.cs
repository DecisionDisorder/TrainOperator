using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 복권 데이터 클래스
/// </summary>

[System.Serializable]
public class LotteryData
{
    /// <summary>
    /// 구매한 로또 티켓 데이터
    /// </summary>
    public LotteryTicket[] lotteryTickets = new LotteryTicket[5];
    /// <summary>
    /// 대박 TRO 복권 기록
    /// </summary>
    public List<LotteryRecord> jackpotLotteryRecords = new List<LotteryRecord>(); 
    /// <summary>
    /// 일반형 TRO 복권 기록
    /// </summary>
    public List<LotteryRecord> normalLotteryRecords = new List<LotteryRecord>();
    /// <summary>
    /// 소박한 TRO 복권 기록
    /// </summary>
    public List<LotteryRecord> simpleLotteryRecords = new List<LotteryRecord>();

    /// <summary>
    /// 대박 TRO 복권 당첨 기록
    /// </summary>
    public List<LotteryRecord> jackpotWinRecords = new List<LotteryRecord>();
    /// <summary>
    /// 일반형 TRO 복권 당첨 기록
    /// </summary>
    public List<LotteryRecord> normalWinRecords = new List<LotteryRecord>();
    /// <summary>
    /// 소박한 TRO 복권 당첨 기록
    /// </summary>
    public List<LotteryRecord> simpleWinRecords = new List<LotteryRecord>();

    /// <summary>
    /// 수령하지 않은 당첨 금액
    /// </summary>
    public LargeVariable unreceivedReward;

    /// <summary>
    /// 사용자가 구매한 복권 상품의 종류
    /// </summary>
    public DrawProduct selectedProduct;
    /// <summary>
    /// 사용자가 구매한 복권의 금액
    /// </summary>
    public LargeVariable purchaseAmount;

    /// <summary>
    /// 복권 추첨까지 남은 시간
    /// </summary>
    public int drawTimeLeft;

    public LotteryData()
    {
        drawTimeLeft = 600;
        lotteryTickets = new LotteryTicket[5];
        for (int i = 0; i < lotteryTickets.Length; i++)
        {
            lotteryTickets[i] = new LotteryTicket();
            lotteryTickets[i].selectedNumbers = new int[0];
        }
    }
}
