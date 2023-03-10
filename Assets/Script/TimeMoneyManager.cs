using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ð��� ���� ���� Ŭ����
/// </summary>
public class TimeMoneyManager : MonoBehaviour
{
    /// <summary>
    /// �߰� ��� �ð��� ���� ���� �ݾ�
    /// </summary>
    public LargeVariable mediumTimeMoney { get { return myAsset.TimePerEarning * (revenueMagnification / 100f); } }
    /// <summary>
    /// ���� �ð��� ���� �ݾ�
    /// </summary>
    public LargeVariable finalTimeMoney { get { return mediumTimeMoney * levelManager.RevenueMagnification * ExternalCoefficient; } }
    /// <summary>
    /// ������ �� �ܺ� ��� ���
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
    /// ���� ������
    /// </summary>
    private ulong revenueMagnification { get { return (ulong)companyReputationManager.RevenueMagnification; } }
    /// <summary>
    /// �ֱ� ���� �ð�
    /// </summary>
    public DateTime recentConnectedTime { get { return PlayManager.instance.playData.recentConnectedTime; } set { PlayManager.instance.playData.recentConnectedTime = value; } }

    /// <summary>
    /// �ִ� ���� ���� �ð�: 120��
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
    /// �ð��� ���� ���� �ڷ�ƾ
    /// </summary>
    IEnumerator TimeEarningTimer()
    {
        yield return new WaitForSeconds(0.5f);

        AssetMoneyCalculator.instance.ArithmeticOperation(finalTimeMoney.lowUnit, finalTimeMoney.highUnit, true);

        StartCoroutine(TimeEarningTimer());
    }

    /// <summary>
    /// �ֱ� ���� �ð� ������Ʈ
    /// </summary>
    public void SetRecentConnectedTime()
    {
        recentConnectedTime = DateTime.Now;
    }

    /// <summary>
    /// �������� ���� ���� ����
    /// </summary>
    private void ProvideOfflineRevenue()
    {
        TimeSpan dateDiff = DateTime.Now - recentConnectedTime;
        int diffSeconds = (int)dateDiff.TotalSeconds;
        if (diffSeconds > 0)
        {
            // �ִ� �ð� ����
            if (diffSeconds > maxRewardTime)
                diffSeconds = maxRewardTime;

            LargeVariable revenue = finalTimeMoney * (diffSeconds / 2);
            if (!revenue.Equals(LargeVariable.zero))
            {
                string revenueLow = "", revenueHigh = "";
                PlayManager.ArrangeUnit(revenue.lowUnit, revenue.highUnit, ref revenueLow, ref revenueHigh, true);
                AssetMoneyCalculator.instance.ArithmeticOperation(revenue, true);
                string title = "�ð��� ���� �߻� ����";
                string revenueReportMsg;
                if (diffSeconds == maxRewardTime)
                    revenueReportMsg = "4�ð� �̻� �ڸ��� ���� ���̿� <color=green>" + revenueHigh + revenueLow + "$</color>��ŭ�� ������ �߻��Ͽ����ϴ�.\n<size=25>�ִ� ���� �ð�: 240��(4�ð�)</size>";
                else if(diffSeconds >= 60)
                    revenueReportMsg = "�� " + (diffSeconds / 60) + "�� ���� �ڸ��� ���� ���̿� <color=green>" + revenueHigh + revenueLow + "$</color>��ŭ�� ������ �߻��Ͽ����ϴ�.\n<size=25>�ִ� ���� �ð�: 240��(4�ð�)</size>";
                else
                    revenueReportMsg = "�� " + diffSeconds + "�� ���� �ڸ��� ���� ���̿� <color=green>" + revenueHigh + revenueLow + "$</color>��ŭ�� ������ �߻��Ͽ����ϴ�.\n<size=25>�ִ� ���� �ð�: 240��(4�ð�)</size>";
                messageManager.ShowRevenueReport(title, revenueReportMsg);
                DataManager.instance.SaveAll();
            }
        }
    }
}
