using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 고객 만족도 시스템 매니저 클래스
/// </summary>
public class CompanyReputationManager : MonoBehaviour {

    /// <summary>
    /// 고객 만족도 시스템 관리 싱글톤 인스턴스
    /// </summary>
    public static CompanyReputationManager instance;

    /// <summary>
    /// 회사 관리 데이터
    /// </summary>
    public CompanyData companyData;

    /// <summary>
    /// 구간 별 명성도 단계 데이터
    /// </summary>
    public ReputationSet[] reputationSets;

    /// <summary>
    /// 고객 만족도 수치
    /// </summary>
	public int ReputationValue
    {
        get
        {
            return companyData.reputationTotalValue;
        }
        set
        {
            companyData.reputationTotalValue = value; 
            AssetInfoUpdater.instance.UpdateText();
        }
    }

    /// <summary>
    /// 고객 만족도에 의한 수익 변화율
    /// </summary>
    public int RevenueMagnification { get { return companyData.additionalPercentage; } set { companyData.additionalPercentage = value; } }
    /// <summary>
    /// 터치형 수익에 계산되는 추가 수익 배율
    /// </summary>
	private ulong UAdditionalPercentage;

    public TimeMoneyManager timeMoneyManager;
    public UpdateDisplay reputationConditionUpdateDisplay;

    private void Awake()
    {
        instance = this;
    }

    void Start ()
    {
        // 고객 만족도 관련 텍스트 정보 업데이트
        RenewReputation();
        // 고객 만족도 업데이트 함수 등록
        reputationConditionUpdateDisplay.onEnableUpdate += AssetInfoUpdater.instance.UpdateText;
    }

    /// <summary>
    /// 고객 만족도에 따른 수익 배율 계산 후 승객 수 계산에 반영
    /// </summary>
	public void CalculateReputation()
	{
        int setIndex = GetReputationSetIndex(ReputationValue);
        RevenueMagnification = (int)(reputationSets[setIndex].revenue * ((float)(ReputationValue - reputationSets[setIndex].from) / reputationSets[setIndex].interval)) 
            + reputationSets[setIndex].stepRevenue + reputationSets[setIndex].priorCumRevenue + 100;
        if (RevenueMagnification > 350)
            RevenueMagnification = 350;

        if(instance != null)
            RenewPassengerBase();
    }
    /// <summary>
    /// 고객 만족도 수치로 고객 만족도 단계 구하기
    /// </summary>
    /// <param name="reputation">만족도 수치</param>
    /// <returns>만족 단계</returns>
    private int GetReputationSetIndex(int reputation)
    {
        for(int i = 0; i < reputationSets.Length; i++)
        {
            if (reputation < reputationSets[i].to)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// 고객만족도 변경에 따른 자산 정보 업데이트
    /// </summary>
    public void RenewReputation()
    {
        AssetInfoUpdater.instance.UpdateText();
    }

    /// <summary>
    /// 고객만족도 변경에 따른 수익 배율 변경분 반영
    /// </summary>
    public void RenewPassengerBase()
    {
        if (MyAsset.instance.PassengersLow >= 10 || MyAsset.instance.PassengersHigh > 0)
        {
            UAdditionalPercentage = (ulong)RevenueMagnification;
            TouchMoneyManager.PercentCalculation(UAdditionalPercentage, MyAsset.instance.PassengersLow, MyAsset.instance.PassengersHigh, ref TouchMoneyManager.PassengersBaseLow, ref TouchMoneyManager.PassengersBaseHigh);
        }
        RenewPassengerRandom();
    }

    /// <summary>
    /// 승객 수 기준치 변경에 따른 특정 범위 내의 무작위 승객 수 수치 변경
    /// </summary>
    public void RenewPassengerRandom()
    {
        if (MyAsset.instance.PassengersLow >= 10 || MyAsset.instance.PassengersHigh > 0)
        {
            ulong passengersRandomLow = 0, passengersRandomHigh = 0;
            TouchMoneyManager.PercentCalculation(TouchEarning.PassengerRandomFactor, TouchMoneyManager.PassengersBaseLow, TouchMoneyManager.PassengersBaseHigh, ref passengersRandomLow, ref passengersRandomHigh);
            TouchMoneyManager.PassengersRandomLow = passengersRandomLow;
            TouchMoneyManager.PassengersRandomHigh = passengersRandomHigh;
        }
        else
        {
            TouchMoneyManager.PassengersRandomLow = TouchMoneyManager.PassengersBaseLow = MyAsset.instance.PassengersLow;
        }
        RenewPassengerMoney();
    }

    /// <summary>
    /// 특정 범위 내의 무작위 승객수를 터치형 수익에 적용
    /// </summary>
    public void RenewPassengerMoney()
    {
        TouchMoneyManager.TouchMoneyLow = TouchMoneyManager.PassengersRandomLow;
        TouchMoneyManager.TouchMoneyHigh = TouchMoneyManager.PassengersRandomHigh;
    }
}

/// <summary>
/// 구간 별 명성도 단계 데이터 클래스
/// </summary>
[System.Serializable]
public class ReputationSet
{
    /// <summary>
    /// 시작 수치 (이상)
    /// </summary>
    public int from;
    /// <summary>
    /// 종점 수치 (미만)
    /// </summary>
    public int to;
    /// <summary>
    /// 고객만족도 단계 간격
    /// </summary>
    public int interval;
    /// <summary>
    /// 수익 배율 (1% 단위)
    /// </summary>
    public int revenue;
    /// <summary>
    /// 직전 단계의 수익 배율
    /// </summary>
    public int priorCumRevenue;
    /// <summary>
    /// 단계별로 추가되는 수익 배율
    /// </summary>
    public int stepRevenue;
}