using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CompanyReputationManager : MonoBehaviour {

    public static CompanyReputationManager instance;

    public CompanyData companyData;

    public ReputationSet[] reputationSets;

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

    public int revenueMagnificationInterval;
    public int reputationInterval;

    public int revenueMagnification { get { return companyData.additionalPercentage; } set { companyData.additionalPercentage = value; } }
    public static string SAdditionalPercentage;
	public static ulong UAdditionalPercentage;

	public static int Over0 = 1;

    public TimeMoneyManager timeMoneyManager;
    public UpdateDisplay reputationConditionUpdateDisplay;

    private void Awake()
    {
        instance = this;
    }

    void Start ()
    {
        RenewReputation ();
        reputationConditionUpdateDisplay.onEnableUpdate += AssetInfoUpdater.instance.UpdateText;
    }

	public void CalculateReputation()
	{
        int setIndex = GetReputationSetIndex(ReputationValue);
        revenueMagnification = (int)(reputationSets[setIndex].revenue * ((float)(ReputationValue - reputationSets[setIndex].from) / reputationSets[setIndex].interval)) 
            + reputationSets[setIndex].stepRevenue + reputationSets[setIndex].priorCumRevenue + 100;
        if (revenueMagnification > 350)
            revenueMagnification = 350;

        if(instance != null)
            RenewPassengerBase();
    }
    private int GetReputationSetIndex(int reputation)
    {
        for(int i = 0; i < reputationSets.Length; i++)
        {
            if (reputation < reputationSets[i].to)
                return i;
        }
        return -1;
    }

    public void RenewReputation()
    {
        AssetInfoUpdater.instance.UpdateText();
    }

    public void RenewPassengerBase()
    {
        if (MyAsset.instance.PassengersLow >= 10 || MyAsset.instance.PassengersHigh > 0)
        {
            UAdditionalPercentage = (ulong)revenueMagnification;
            TouchMoneyManager.PercentCalculation(UAdditionalPercentage, MyAsset.instance.PassengersLow, MyAsset.instance.PassengersHigh, ref TouchMoneyManager.PassengersBaseLow, ref TouchMoneyManager.PassengersBaseHigh);
        }
        RenewPassengerRandom();
    }

    public void RenewPassengerRandom()
    {
        if (MyAsset.instance.PassengersLow >= 10 || MyAsset.instance.PassengersHigh > 0)
        {
            ulong passengersRandomLow = 0, passengersRandomHigh = 0;
            //TODO: 이 함수도 수정하기
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

    public void RenewPassengerMoney()
    {
        TouchMoneyManager.TouchMoneyLow = TouchMoneyManager.PassengersRandomLow;
        TouchMoneyManager.TouchMoneyHigh = TouchMoneyManager.PassengersRandomHigh;
    }
    /*
	public static void SaveData()
	{
		PlayerPrefs.SetInt("Rep_Totalvalue",reputation_Totalvalue);
		PlayerPrefs.SetInt("Rep_Percentage",additional_Percentage);
		PlayerPrefs.SetInt ("Rep_REP_Percentage",Rep_Percentage);
	}
	public static void LoadData()
	{
		reputation_Totalvalue = PlayerPrefs.GetInt("Rep_Totalvalue",500);
		additional_Percentage = PlayerPrefs.GetInt("Rep_Percentage",100);
		Rep_Percentage = PlayerPrefs.GetInt ("Rep_REP_Percentage", 10);
	}
	public static void ResetData()
	{
		reputation_Totalvalue = 500;
		additional_Percentage = 100;
	}*/
}

[System.Serializable]
public class ReputationSet
{
    public int from;
    public int to;
    public int interval;
    public int revenue;
    public int priorCumRevenue;
    public int stepRevenue;
}