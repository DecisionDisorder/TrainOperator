using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeMoneyManager : MonoBehaviour
{
    public LargeVariable mediumTimeMoney { get { return myAsset.TimePerEarning * (revenueMagnification / 100f); } }
    public LargeVariable finalTimeMoney { get { return mediumTimeMoney * levelManager.RevenueMagnification * ExternalCoefficient; } }
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

    private ulong revenueMagnification { get { return (ulong)companyReputationManager.revenueMagnification; } }
    public DateTime recentConnectedTime { get { return PlayManager.instance.playData.recentConnectedTime; } set { PlayManager.instance.playData.recentConnectedTime = value; } }

    /// <summary>
    /// �ִ� ���� ���� �ð�: 120��
    /// </summary>
    public readonly int maxRewardTime = 7200;

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

    IEnumerator TimeEarningTimer()
    {
        yield return new WaitForSeconds(0.5f);

        AssetMoneyCalculator.instance.ArithmeticOperation(finalTimeMoney.lowUnit, finalTimeMoney.highUnit, true);

        StartCoroutine(TimeEarningTimer());
    }

    public void SetRecentConnectedTime()
    {
        recentConnectedTime = DateTime.Now;
    }

    private void ProvideOfflineRevenue()
    {
        TimeSpan dateDiff = DateTime.Now - recentConnectedTime;
        int diffSeconds = (int)dateDiff.TotalSeconds;
        if (diffSeconds > 0)
        {
            if (diffSeconds > maxRewardTime)
                diffSeconds = maxRewardTime;

            LargeVariable revenue = finalTimeMoney * (diffSeconds / 2);
            if (!revenue.Equals(LargeVariable.zero))
            {
                string revenueLow = "", revenueHigh = "";
                PlayManager.ArrangeUnit(revenue.lowUnit, revenue.highUnit, ref revenueLow, ref revenueHigh, true);
                AssetMoneyCalculator.instance.ArithmeticOperation(revenue, true);
                string title = "�ð��� ���� �߻� ����";
                string revenueReportMsg = "�ڸ��� ���� ���̿� <color=green>" + revenueHigh + revenueLow + "$</color>��ŭ�� ������ �߻��Ͽ����ϴ�.";
                messageManager.ShowRevenueReport(title, revenueReportMsg);
                DataManager.instance.SaveAll();
            }
        }
    }
}
