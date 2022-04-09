using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AssetInfoUpdater : MonoBehaviour {

    public static AssetInfoUpdater instance;

	public Text moneyText;
	public Text touchMoneyText;
	public Text showTimePerMoneyText;
	public Text passengerText;

    public TimeMoneyManager timeMoneyManager;
	//-----------------------------------------------------------------------------

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        UpdateTimeMoneyText();
    }

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
}
