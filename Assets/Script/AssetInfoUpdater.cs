using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 자산 정보 업데이트 관리 클래스
/// </summary>
public class AssetInfoUpdater : MonoBehaviour {

    /// <summary>
    /// 자산 정보 업데이터의 싱글톤 인스턴스
    /// </summary>
    public static AssetInfoUpdater instance;

    /// <summary>
    /// 현재 보유중인 돈 텍스트
    /// </summary>
	public Text moneyText;
    /// <summary>
    /// 터치형 수익 정보 텍스트
    /// </summary>
	public Text touchMoneyText;
    /// <summary>
    /// 시간형 수익 정보 텍스트
    /// </summary>
	public Text showTimePerMoneyText;
    /// <summary>
    /// 승객 수 정보 텍스트
    /// </summary>
	public Text passengerText;

    public TimeMoneyManager timeMoneyManager;
    
    /// <summary>
    /// (수익률 조회 메뉴)터치형 수익 증가율 텍스트
    /// </summary>
    public Text touchRevenueText;
    /// <summary>
    /// (수익률 조회 메뉴)터치형 수익 증가량 텍스트
    /// </summary>
    public Text touchRevenueAmountText;
    /// <summary>
    /// (수익률 조회 메뉴)시간형 수익 증가율 텍스트
    /// </summary>
    public Text timeRevenueText;
    /// <summary>
    /// (수익률 조회 메뉴)시간형 수익 증가량 텍스트
    /// </summary>
    public Text timeRevenueAmountText;
    /// <summary>
    /// (수익률 조회 메뉴)고객만족도 텍스트
    /// </summary>
    public Text reputationText;
    /// <summary>
    /// (수익률 조회 메뉴)전체 수익 증가율 텍스트
    /// </summary>
    public Text reputationRevenueText;
    /// <summary>
    /// (수익률 조회 메뉴)사용자 레벨 텍스트
    /// </summary>
    public Text levelText;
    /// <summary>
    /// (수익률 조회 메뉴)레벨로 인한 시간형 수익 증가율 텍스트
    /// </summary>
    public Text levelRevenueText;

    public CompanyReputationManager companyReputationManager;
    public LevelManager levelManager;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        UpdateTimeMoneyText();
    }

    /// <summary>
    /// 현재 보유 중인 돈에 대한 자산 정보 업데이트
    /// </summary>
    public void UpdateMoneyText()
    {
        string lowUnit = "";
        string highUnit = "";
        moneyText.text = "돈: ";
        if (MyAsset.instance.MoneyLow == 0 && MyAsset.instance.MoneyHigh == 0)
        {
            moneyText.fontSize = 51;
            moneyText.text += "0$";
        }
        else
        {
            PlayManager.ArrangeUnit(MyAsset.instance.MoneyLow, MyAsset.instance.MoneyHigh, ref lowUnit, ref highUnit, true);
            if (MyAsset.instance.MoneyHigh > 0)
            {
                moneyText.fontSize = 45;
                moneyText.text += highUnit + lowUnit + "$";
            }
            else
            {
                moneyText.fontSize = 51;
                moneyText.text += lowUnit + "$";
            }
            if (MyAsset.instance.MoneyHigh > 100000000)
            {
                moneyText.fontSize = 39;
            }
        }
    }

    /// <summary>
    /// 승객 수 정보 업데이트
    /// </summary>
    public void UpdatePassengerText()
    {
        passengerText.text = "승객 수: ";
        if (MyAsset.instance.PassengersLow == 0)
            passengerText.text += "0명 / " + MyAsset.instance.myAssetData.passengersLimitLow + "명";
        else
        {
            string passengersHigh = "", passengersLow = "", passengerLimitHigh = "", passengerLimitLow = "";
            PlayManager.ArrangeUnit(MyAsset.instance.PassengersLow, MyAsset.instance.PassengersHigh, ref passengersLow, ref passengersHigh);

            ulong passengersLimitLow = MyAsset.instance.myAssetData.passengersLimitLow;
            ulong passengersLimitHigh = MyAsset.instance.myAssetData.passengersLimitHigh;
            MoneyUnitTranslator.Arrange(ref passengersLimitLow, ref passengersLimitHigh);
            MyAsset.instance.myAssetData.passengersLimitLow = passengersLimitLow;
            MyAsset.instance.myAssetData.passengersLimitHigh = passengersLimitHigh;

            PlayManager.ArrangeUnit(passengersLimitLow, passengersLimitHigh, ref passengerLimitLow, ref passengerLimitHigh);
            passengerText.text += passengersHigh + passengersLow + "명" + " / " + passengerLimitHigh + passengerLimitLow + "명";
        }
    }

    /// <summary>
    /// 터치형 수익 정보 업데이트
    /// </summary>
    public void UpdateTouchMoneyText()
    {
        string money2 = "", money1 = "";
        PlayManager.ArrangeUnit(TouchMoneyManager.TouchMoneyLow, TouchMoneyManager.TouchMoneyHigh, ref money1, ref money2, true);
        if (TouchMoneyManager.TouchMoneyHigh > 0)
        {
            touchMoneyText.text = "터치당 " + money2 + money1 + "$";
        }
        else
        {
            touchMoneyText.text = "터치당 " + money1 + "$";
        }
    }

    /// <summary>
    /// 시간형 수익 정보 업데이트
    /// </summary>
    public void UpdateTimeMoneyText()
    {
        if (timeMoneyManager.finalTimeMoney.lowUnit == 0 && timeMoneyManager.finalTimeMoney.highUnit == 0)
        {
            showTimePerMoneyText.text = "초당 0$";
        }
        else
        {
            string lowUnit = "", highUnit = "";
            PlayManager.ArrangeUnit(timeMoneyManager.finalTimeMoney.lowUnit, timeMoneyManager.finalTimeMoney.highUnit, ref lowUnit, ref highUnit, true);
            showTimePerMoneyText.text = "초당 " + highUnit + lowUnit + "$";
        }
    }

    /// <summary>
    /// 수익률 조회 메뉴 정보 업데이트
    /// </summary>
    public void UpdateText()
    {
        companyReputationManager.CalculateReputation();

        string passengerLow = "", passengerHigh = "", timeMoneyLow = "", timeMoneyHigh = "";
        ulong additionalPassengerLow = TouchMoneyManager.PassengersBaseLow - MyAsset.instance.PassengersLow, additionalPassengerHigh = TouchMoneyManager.PassengersBaseHigh - MyAsset.instance.PassengersHigh;
        ulong additionalTimeMoneyLow = timeMoneyManager.finalTimeMoney.lowUnit - MyAsset.instance.TimePerEarningLow, additionalTimeMoneyHigh = timeMoneyManager.finalTimeMoney.highUnit - MyAsset.instance.TimePerEarningHigh;

        PlayManager.ArrangeUnit(additionalPassengerLow, additionalPassengerHigh, ref passengerLow, ref passengerHigh, true);
        PlayManager.ArrangeUnit(additionalTimeMoneyLow, additionalTimeMoneyHigh, ref timeMoneyLow, ref timeMoneyHigh, true);

        touchRevenueText.text = "터치형 수익 <color=red>" + companyReputationManager.RevenueMagnification + "%</color> (" + string.Format("{0:0.###}", companyReputationManager.RevenueMagnification / 100f) + "배)";
        touchRevenueAmountText.text = "+" + passengerHigh + passengerLow + "$";

        timeRevenueText.text = "시간형 수익 <color=red>" + (companyReputationManager.RevenueMagnification / 100f * levelManager.RevenueMagnification * 100) + "%</color> (" + string.Format("{0:0.###}", companyReputationManager.RevenueMagnification / 100f * levelManager.RevenueMagnification) + "배)";
        timeRevenueAmountText.text = "+" + timeMoneyHigh + timeMoneyLow + "$";

        string rT = string.Format("{0:#,##0}", companyReputationManager.ReputationValue);
        reputationText.text = "고객 만족도: " + rT + "P";
        reputationRevenueText.text = "전체 수익 변화율: " + companyReputationManager.RevenueMagnification+ "%";

        levelText.text = "레벨: " + levelManager.Level;
        levelRevenueText.text = "시간형 수익 변화율: " + string.Format("{0:0.###}", levelManager.RevenueMagnification * 100) + "%";
    }
}
