using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 시간형 수익 관리 클래스
/// </summary>
public class TimeMoneyManager : MonoBehaviour
{
    /// <summary>
    /// 중간 계산 시간형 수익 기준 금액
    /// </summary>
    public LargeVariable mediumTimeMoney { get { return myAsset.TimePerEarning * (revenueMagnification / 100f); } }
    /// <summary>
    /// 최종 시간형 수익 금액
    /// </summary>
    public LargeVariable finalTimeMoney { get { return mediumTimeMoney * levelManager.RevenueMagnification * ExternalCoefficient; } }
    /// <summary>
    /// 아이템 등 외부 계산 계수
    /// </summary>
    public ulong externalCoefficient = 0;
    private ulong ExternalCoefficient
    {
        get
        {
            if (externalCoefficient < 1)
                return 1;
            else
                return externalCoefficient;
        }
    }

    /// <summary>
    /// 수익 증가율
    /// </summary>
    private ulong revenueMagnification { get { return (ulong)companyReputationManager.RevenueMagnification; } }
    /// <summary>
    /// 최근 접속 시간
    /// </summary>
    public DateTime recentConnectedTime { get { return PlayManager.instance.playData.recentConnectedTime; } set { PlayManager.instance.playData.recentConnectedTime = value; } }

    /// <summary>
    /// 최대 수익 인정 시간: 120분
    /// </summary>
    public readonly int maxRewardTime = 14400;

    public MyAsset myAsset;
    public CompanyReputationManager companyReputationManager;
    public LevelManager levelManager;
    public ItemManager itemManager;
    public MessageManager messageManager;

    void Start()
    {
        StartCoroutine(TimeEarningTimer());
        ProvideOfflineRevenue();
    }

    /// <summary>
    /// 시간형 수익 지급 코루틴
    /// </summary>
    IEnumerator TimeEarningTimer()
    {
        yield return new WaitForSeconds(0.5f);

        AssetMoneyCalculator.instance.ArithmeticOperation(finalTimeMoney.lowUnit, finalTimeMoney.highUnit, true);

        StartCoroutine(TimeEarningTimer());
    }

    /// <summary>
    /// 최근 접속 시간 업데이트
    /// </summary>
    public void SetRecentConnectedTime()
    {
        recentConnectedTime = DateTime.Now;
    }

    /// <summary>
    /// 오프라인 수익 정산 지급
    /// </summary>
    private void ProvideOfflineRevenue()
    {
        TimeSpan dateDiff = DateTime.Now - recentConnectedTime;
        int diffSeconds = (int)dateDiff.TotalSeconds;
        if (diffSeconds > 0)
        {
            // 최대 시간 제한
            if (diffSeconds > maxRewardTime)
                diffSeconds = maxRewardTime;

            LargeVariable revenue = finalTimeMoney * (diffSeconds / 2);
            if (!revenue.Equals(LargeVariable.zero))
            {
                string revenueLow = "", revenueHigh = "";
                PlayManager.ArrangeUnit(revenue.lowUnit, revenue.highUnit, ref revenueLow, ref revenueHigh, true);
                AssetMoneyCalculator.instance.ArithmeticOperation(revenue, true);
                string title = "시간형 수익 발생 보고서";
                string revenueReportMsg;
                if (diffSeconds == maxRewardTime)
                    revenueReportMsg = "4시간 이상 자리를 비우신 사이에 <color=green>" + revenueHigh + revenueLow + "$</color>만큼의 수익이 발생하였습니다.\n<size=25>최대 적용 시간: 240분(4시간)</size>";
                else if(diffSeconds >= 60)
                    revenueReportMsg = "약 " + (diffSeconds / 60) + "분 동안 자리를 비우신 사이에 <color=green>" + revenueHigh + revenueLow + "$</color>만큼의 수익이 발생하였습니다.\n<size=25>최대 적용 시간: 240분(4시간)</size>";
                else
                    revenueReportMsg = "약 " + diffSeconds + "초 동안 자리를 비우신 사이에 <color=green>" + revenueHigh + revenueLow + "$</color>만큼의 수익이 발생하였습니다.\n<size=25>최대 적용 시간: 240분(4시간)</size>";
                messageManager.ShowRevenueReport(title, revenueReportMsg);
                DataManager.instance.SaveAll();
            }
        }
    }
}
