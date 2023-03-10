using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 플레이어 자산 관리 클래스
/// </summary>
public class MyAsset : MonoBehaviour {

	/// <summary>
	/// 플레이어 자산 싱글톤 인스턴스
	/// </summary>
	public static MyAsset instance;

	/// <summary>
	/// 플레이어 자산 데이터 오브젝트
	/// </summary>
	public MyAssetData myAssetData;
	public AssetInfoUpdater assetInfoUpdater;
	/// <summary>
	/// 1경 미만 단위의 보유 중인 돈
	/// </summary>
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
	/// <summary>
	/// 1경 이상 단위의 보유 중인 돈
	/// </summary>
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
	/// <summary>
	/// 보유 중인 돈
	/// </summary>
	public LargeVariable Money { get { return new LargeVariable(MoneyLow, MoneyHigh); } set { MoneyLow = value.lowUnit; MoneyHigh = value.highUnit; } }

	/// <summary>
	/// 보유 중인 역 개수
	/// </summary>
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

	/// <summary>
	/// 1경 미만 단위의 시간형 수익
	/// </summary>
	public ulong TimePerEarningLow
	{
		get { return myAssetData.timePerEarningLow; }
		set { myAssetData.timePerEarningLow = value; assetInfoUpdater.UpdateTimeMoneyText(); }
	}
	/// <summary>
	/// 1경 이상 단위의 시간형 수익
	/// </summary>
	public ulong TimePerEarningHigh
	{
		get { return myAssetData.timePerEarningHigh; }
		set { myAssetData.timePerEarningHigh = value; assetInfoUpdater.UpdateTimeMoneyText(); }
	}
	/// <summary>
	/// 시간형 수익 수치
	/// </summary>
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

	/// <summary>
	/// 1경 미만 단위의 승객 수
	/// </summary>
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
	/// <summary>
	/// 1경 이상 단위의 승객 수
	/// </summary>
	public ulong PassengersHigh
	{
		get { return myAssetData.passengersHigh; }
		set
		{
			myAssetData.passengersHigh = value;
			assetInfoUpdater.UpdatePassengerText();
		}
	}
	/// <summary>
	/// 1경 미만 단위의 승객 수 제한량
	/// </summary>
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
	/// <summary>
	/// 1경 이상 단위의 승객 수 제한량
	/// </summary>
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

	/// <summary>
	/// 총합 수익수치 계산
	/// </summary>
	/// <param name="lowUnit">반환될 1경 미만 단위의 총합 수익</param>
	/// <param name="highUnit">반환될 1경 이상 단위의 총합 수익</param>
	public void GetTotalRevenue(ref ulong lowUnit, ref ulong highUnit)
    {
		ulong tmLow = TimePerEarningLow, tmHigh = TimePerEarningHigh;

		MoneyUnitTranslator.Multiply(ref tmLow, ref tmHigh, 0.15f);

		lowUnit = tmLow + PassengersLow;
		highUnit = tmHigh + PassengersHigh;
    }

	/// <summary>
	/// 총합 수익 수치 계산
	/// </summary>
	/// <returns>총합 수익 수치</returns>
	public LargeVariable GetTotalRevenue()
    {
		LargeVariable variable = TimePerEarning;
		LargeVariable passenger = new LargeVariable(PassengersLow, PassengersHigh);
		variable *= 0.15f;
		variable += passenger;

		return variable;
    }

	/// <summary>
	/// 시간형 수익 추가/삭감
	/// </summary>
	/// <param name="firstUnit">1경 미만의 낮은 단위</param>
	/// <param name="secondUnit">1경 이상의 높은 단위</param>
	/// <param name="plus">추가 여부</param>
	/// <returns>처리 성공 여부</returns>
	public bool TimeEarningOperator(ulong firstUnit, ulong secondUnit, bool plus)
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

    /// <summary>
    /// (테스트용) 시간형 수익 설정
    /// </summary>
    /// <param name="lowUnit">1경 미만의 낮은 단위</param>
    /// <param name="highUnit">1경 이상의 높은 단위</param>
    public void SetTimeEarning(ulong lowUnit, ulong highUnit)
	{
		MoneyUnitTranslator.Arrange(ref lowUnit, ref highUnit);
		TimePerEarningLow = lowUnit;
		TimePerEarningHigh = highUnit;
    }
}
