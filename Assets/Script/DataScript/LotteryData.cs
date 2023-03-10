using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������ Ŭ����
/// </summary>

[System.Serializable]
public class LotteryData
{
    /// <summary>
    /// ������ �ζ� Ƽ�� ������
    /// </summary>
    public LotteryTicket[] lotteryTickets = new LotteryTicket[5];
    /// <summary>
    /// ��� TRO ���� ���
    /// </summary>
    public List<LotteryRecord> jackpotLotteryRecords = new List<LotteryRecord>(); 
    /// <summary>
    /// �Ϲ��� TRO ���� ���
    /// </summary>
    public List<LotteryRecord> normalLotteryRecords = new List<LotteryRecord>();
    /// <summary>
    /// �ҹ��� TRO ���� ���
    /// </summary>
    public List<LotteryRecord> simpleLotteryRecords = new List<LotteryRecord>();

    /// <summary>
    /// ��� TRO ���� ��÷ ���
    /// </summary>
    public List<LotteryRecord> jackpotWinRecords = new List<LotteryRecord>();
    /// <summary>
    /// �Ϲ��� TRO ���� ��÷ ���
    /// </summary>
    public List<LotteryRecord> normalWinRecords = new List<LotteryRecord>();
    /// <summary>
    /// �ҹ��� TRO ���� ��÷ ���
    /// </summary>
    public List<LotteryRecord> simpleWinRecords = new List<LotteryRecord>();

    /// <summary>
    /// �������� ���� ��÷ �ݾ�
    /// </summary>
    public LargeVariable unreceivedReward;

    /// <summary>
    /// ����ڰ� ������ ���� ��ǰ�� ����
    /// </summary>
    public DrawProduct selectedProduct;
    /// <summary>
    /// ����ڰ� ������ ������ �ݾ�
    /// </summary>
    public LargeVariable purchaseAmount;

    /// <summary>
    /// ���� ��÷���� ���� �ð�
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
