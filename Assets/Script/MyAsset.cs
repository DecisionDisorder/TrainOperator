using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MyAsset : MonoBehaviour {

	public static MyAsset instance;

	public MyAssetData myAssetData;
	public AssetInfoUpdater assetInfoUpdater;
	public ulong MoneyLow
	{
		get { return myAssetData.moneyLow; }
		set
		{
			if (value < 17000000000000000000)
				myAssetData.moneyLow = value;
			assetInfoUpdater.UpdateMoneyText();
		}
	}
	public ulong MoneyHigh
	{
		get { return myAssetData.moneyHigh; }
		set
		{
			if (value < 17000000000000000000)
				myAssetData.moneyHigh = value;
			assetInfoUpdater.UpdateMoneyText();
		}
	}
	public LargeVariable Money { get { return new LargeVariable(MoneyLow, MoneyHigh); } set { MoneyLow = value.lowUnit; MoneyHigh = value.highUnit; } }

	public int NumOfStations
	{
		get { return myAssetData.numOfStations; }
		set
		{
			if (value < 3)
				myAssetData.numOfStations = 3;
			else
				myAssetData.numOfStations = value;
		}
	}

	public ulong TimePerEarningLow
	{
		get { return myAssetData.timePerEarningLow; }
		set { myAssetData.timePerEarningLow = value; assetInfoUpdater.UpdateTimeMoneyText(); }
	}
	public ulong TimePerEarningHigh
	{
		get { return myAssetData.timePerEarningHigh; }
		set { myAssetData.timePerEarningHigh = value; assetInfoUpdater.UpdateTimeMoneyText(); }
	}
	public LargeVariable TimePerEarning
	{
		get { return new LargeVariable(myAssetData.timePerEarningLow, myAssetData.timePerEarningHigh); }
		set
		{
			myAssetData.timePerEarningLow = value.lowUnit;
			myAssetData.timePerEarningHigh = value.highUnit;
			assetInfoUpdater.UpdateTimeMoneyText();
		}
	}

	public ulong PassengersLow
    {
		get { return myAssetData.passengersLow; }
		set
		{
			if(value < 1)
				myAssetData.passengersLow = 1;
			else
				myAssetData.passengersLow = value;
			assetInfoUpdater.UpdatePassengerText();
		}
    }
	public ulong PassengersHigh
	{
		get { return myAssetData.passengersHigh; }
		set
		{
			myAssetData.passengersHigh = value;
			assetInfoUpdater.UpdatePassengerText();
		}
	}
	public ulong PassengersLimitLow
	{
		get { return myAssetData.passengersLimitLow; }
		set
		{
			if (value > 100)
				myAssetData.passengersLimitLow = value;
			else
				myAssetData.passengersLimitLow = 100;
			assetInfoUpdater.UpdatePassengerText();
		}
	}
	public ulong PassengersLimitHigh
	{
		get { return myAssetData.passengersLimitHigh; }
		set
		{
			myAssetData.passengersLimitHigh = value;
			assetInfoUpdater.UpdatePassengerText();
		}
	}


    private void Awake()
    {
		instance = this;
	}

    void Start ()
    {
        EncryptedPlayerPrefs.keys = new string[5];
        EncryptedPlayerPrefs.keys[0] = "d45a4dww";
        EncryptedPlayerPrefs.keys[1] = "SW5213s4";
        EncryptedPlayerPrefs.keys[2] = "#WEw52da";
        EncryptedPlayerPrefs.keys[3] = "as2DFs5w";
        EncryptedPlayerPrefs.keys[4] = "Wq28t3Sf";
	}

	public void GetTotalRevenue(ref ulong lowUnit, ref ulong highUnit)
    {
		ulong tmLow = TimePerEarningLow, tmHigh = TimePerEarningHigh;

		MoneyUnitTranslator.Multiply(ref tmLow, ref tmHigh, 0.15f);

		lowUnit = tmLow + PassengersLow;
		highUnit = tmHigh + PassengersHigh;
    }

	public bool TimeEarningOperator(ulong firstUnit, ulong secondUnit, bool plus)//외부 스크립트 호출용
	{
		ulong tineMoneyLow = TimePerEarningLow;
		ulong timeMoneyHigh = TimePerEarningHigh;
		bool result;
		if (plus)
			result = MoneyUnitTranslator.Add(firstUnit, secondUnit, ref tineMoneyLow, ref timeMoneyHigh);
		else
			result = MoneyUnitTranslator.Subtract(firstUnit, secondUnit, ref tineMoneyLow, ref timeMoneyHigh);

		TimePerEarningLow = tineMoneyLow;
		TimePerEarningHigh = timeMoneyHigh;

		return result;
	}

	public void SetTimeEarning(ulong lowUnit, ulong highUnit)
	{
		MoneyUnitTranslator.Arrange(ref lowUnit, ref highUnit);
		TimePerEarningLow = lowUnit;
		TimePerEarningHigh = highUnit;
    }
}
