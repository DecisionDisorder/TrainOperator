using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CompanyReputationManager : MonoBehaviour {

    public static CompanyReputationManager instance;
	public Text Additional_Touch;
	public Text Total_Reputation;

    public CompanyData companyData;

	public int ReputationValue
    {
        get
        {
            return companyData.reputationTotalValue;
        }
        set
        {
            companyData.reputationTotalValue = value; 
            UpdateText();
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
        reputationConditionUpdateDisplay.onEnableUpdate += UpdateText;
    }

    public void UpdateText()
    {
        CalculateReputation();

        string passengerLow = "", passengerHigh = "", timeMoneyLow = "", timeMoneyHigh = "";
        ulong additionalPassengerLow = TouchMoneyManager.PassengersBaseLow - MyAsset.instance.PassengersLow, additionalPassengerHigh = TouchMoneyManager.PassengersBaseHigh - MyAsset.instance.PassengersHigh;
        ulong additionalTimeMoneyLow = timeMoneyManager.mediumTimeMoney.lowUnit - MyAsset.instance.TimePerEarningLow, additionalTimeMoneyHigh = timeMoneyManager.mediumTimeMoney.highUnit - MyAsset.instance.TimePerEarningHigh;

        PlayManager.ArrangeUnit(additionalPassengerLow, additionalPassengerHigh, ref passengerLow, ref passengerHigh, true);
        PlayManager.ArrangeUnit(additionalTimeMoneyLow, additionalTimeMoneyHigh, ref timeMoneyLow, ref timeMoneyHigh, true);

        Additional_Touch.text = "수익 증가율: +" + (revenueMagnification - 100) + "%\n";
        Additional_Touch.text += string.Format("시간형 수익 증가량: +{0}{1}$\n터치형 수익 증가량: +{2}{3}$", timeMoneyHigh, timeMoneyLow, passengerHigh, passengerLow);

        string rT = string.Format("{0:#,##0}", ReputationValue);
        Total_Reputation.text = rT + "P";
    }

	private void CalculateReputation()
	{
        revenueMagnification = 100 + (int)(revenueMagnificationInterval * ((float)ReputationValue / reputationInterval));
        if (revenueMagnification > 300)
            revenueMagnification = 300;

        if(instance != null)
            RenewPassengerBase();
    }

    public void RenewReputation()
    {
        UpdateText();
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
