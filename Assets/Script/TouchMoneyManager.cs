using UnityEngine;
using System.Collections;

public class TouchMoneyManager : MonoBehaviour
{
    public static ulong PassengersBaseLow;
    private static ulong passengersRandomLow;
    public static ulong PassengersRandomLow
    {
        get { return passengersRandomLow; }
        set
        {
            passengersRandomLow = value;
            if (AssetInfoUpdater.instance != null)
                AssetInfoUpdater.instance.UpdatePassengerText();
        }
    }

    public static ulong PassengersBaseHigh;
    private static ulong passengersRandomHigh;
    public static ulong PassengersRandomHigh
    {
        get { return passengersRandomHigh; }
        set
        {
            passengersRandomHigh = value;
            if (AssetInfoUpdater.instance != null)
                AssetInfoUpdater.instance.UpdatePassengerText();
        }
    }

    private static ulong touchMoneyLow;
    public static ulong TouchMoneyLow //기준값
    {
        get { return touchMoneyLow; }
        set
        {
            touchMoneyLow = value;
            if (AssetInfoUpdater.instance != null)
                AssetInfoUpdater.instance.UpdateTouchMoneyText();
        }
    }

    private static ulong touchMoneyHigh;
    public static ulong TouchMoneyHigh { 
        get { return touchMoneyHigh; }
        set
        {
            touchMoneyHigh = value;

            if (AssetInfoUpdater.instance != null)
                AssetInfoUpdater.instance.UpdateTouchMoneyText();
        }
    }

    public static ulong standardMaximum = 10000000000000000;

    public static ulong divided_SN;

    public static string Nums;

    void Start()
    {
        ulong passengersLow = MyAsset.instance.myAssetData.passengersLow;
        ulong passengersHigh = MyAsset.instance.myAssetData.passengersHigh;

        MoneyUnitTranslator.Arrange(ref passengersLow, ref passengersHigh);

        MyAsset.instance.myAssetData.passengersLow = passengersLow;
        MyAsset.instance.myAssetData.passengersHigh = passengersHigh;
    }


    public static void ArithmeticOperation(ulong firstunit, ulong secondunit, bool plus) //외부 스크립트 호출용
    {
        ulong passengersLow = MyAsset.instance.myAssetData.passengersLow;
        ulong passengersHigh = MyAsset.instance.myAssetData.passengersHigh;
        if (plus)
            MoneyUnitTranslator.Add(firstunit, secondunit, ref passengersLow, ref passengersHigh);
        else
            MoneyUnitTranslator.Subtract(firstunit, secondunit, ref passengersLow, ref passengersHigh);

        MyAsset.instance.myAssetData.passengersLow = passengersLow;
        MyAsset.instance.myAssetData.passengersHigh = passengersHigh;
    }

    public static void AddPassengerLimit(ulong lowUnit, ulong highUnit)
    {
        ulong passengersLimitLow = MyAsset.instance.myAssetData.passengersLimitLow;
        ulong passengersLimitHigh = MyAsset.instance.myAssetData.passengersLimitHigh;
        MoneyUnitTranslator.Add(lowUnit, highUnit, ref passengersLimitLow, ref passengersLimitHigh);

        MyAsset.instance.myAssetData.passengersLimitLow = passengersLimitLow;
        MyAsset.instance.myAssetData.passengersLimitHigh = passengersLimitHigh;

        AssetInfoUpdater.instance.UpdatePassengerText();
    }

    public static bool CheckLimitValid(ulong passengerToAddLow, ulong passengerToAddHigh)
    {
        ulong passengerLow = MyAsset.instance.PassengersLow + passengerToAddLow;
        ulong passengerHigh = MyAsset.instance.PassengersHigh + passengerToAddHigh;

        MoneyUnitTranslator.Arrange(ref passengerLow, ref passengerHigh);
        int result = MoneyUnitTranslator.Compare(MyAsset.instance.PassengersLimitLow, MyAsset.instance.PassengersLimitHigh, passengerLow, passengerHigh);

        if (result >= 0)
            return true;
        else
            return false;
    }

    public static void PercentCalculation(ulong percentage , ulong fu, ulong su, ref ulong lowUnit, ref ulong highUnit)
    {
        lowUnit = fu;
        highUnit = su;
        MoneyUnitTranslator.CalculatePercent(ref lowUnit, ref highUnit, percentage);
    }

    public static void Multiply(int mag, ref ulong lowUnit, ref ulong highUnit)
    {
        lowUnit = PassengersRandomLow;
        highUnit = PassengersRandomHigh;
        lowUnit *= (ulong)mag;
        highUnit *= (ulong)mag;
        MoneyUnitTranslator.Arrange(ref lowUnit, ref highUnit);
    }
}
