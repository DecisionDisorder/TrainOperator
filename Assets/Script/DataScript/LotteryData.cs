using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LotteryData
{
    public LotteryTicket[] lotteryTickets = new LotteryTicket[5];
    public List<LotteryRecord> jackpotLotteryRecords = new List<LotteryRecord>(); 
    public List<LotteryRecord> normalLotteryRecords = new List<LotteryRecord>();
    public List<LotteryRecord> simpleLotteryRecords = new List<LotteryRecord>();

    public List<LotteryRecord> jackpotWinRecords = new List<LotteryRecord>();
    public List<LotteryRecord> normalWinRecords = new List<LotteryRecord>();
    public List<LotteryRecord> simpleWinRecords = new List<LotteryRecord>();

    public LargeVariable unreceivedReward;

    public float drawTimeLeft;
}
