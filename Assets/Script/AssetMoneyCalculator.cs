using UnityEngine;
using System.Collections;

public class AssetMoneyCalculator : MonoBehaviour {

    public static AssetMoneyCalculator instance;

    public static readonly ulong standard_maximum = 10000000000000000;

    private void Awake()
    {
        instance = this;
    }

    public bool ArithmeticOperation(ulong lowUnit, ulong HighUnit, bool plus)//외부 스크립트 호출용
    {
        ulong moneyLow = MyAsset.instance.MoneyLow;
        ulong moneyHigh = MyAsset.instance.MoneyHigh;
        bool result;
        if (plus)
            result = MoneyUnitTranslator.Add(lowUnit, HighUnit, ref moneyLow, ref moneyHigh);
        else
            result = MoneyUnitTranslator.Subtract(lowUnit, HighUnit, ref moneyLow, ref moneyHigh);

        MyAsset.instance.MoneyLow = moneyLow;
        MyAsset.instance.MoneyHigh = moneyHigh;

        return result;
    }

    public bool ArithmeticOperation(LargeVariable variable, bool plus)
    {
        return ArithmeticOperation(variable.lowUnit, variable.highUnit, plus);
    }

    public void ForceSubtract(ulong firstUnit, ulong secondUnit)
    {
        ulong moneyLow = MyAsset.instance.MoneyLow;
        ulong moneyHigh = MyAsset.instance.MoneyHigh;
        if (!MoneyUnitTranslator.Subtract(firstUnit, secondUnit, ref moneyLow, ref moneyHigh))
        {
            MyAsset.instance.MoneyLow = 0;
            MyAsset.instance.MoneyHigh = 0;
        }
        else
        {
            MyAsset.instance.MoneyLow = moneyLow;
            MyAsset.instance.MoneyHigh = moneyHigh;
        }
    }
    
    public void PercentOperation(ulong percentage)
    {
        ulong moneyLow = MyAsset.instance.MoneyLow;
        ulong moneyHigh = MyAsset.instance.MoneyHigh;

        MoneyUnitTranslator.CalculatePercent(ref moneyLow, ref moneyHigh, percentage);

        MyAsset.instance.MoneyLow = moneyLow;
        MyAsset.instance.MoneyHigh = moneyHigh;
    }

    public void PercentOperation(float percentage)
    {
        ulong moneyLow = MyAsset.instance.MoneyLow;
        ulong moneyHigh = MyAsset.instance.MoneyHigh;

        MoneyUnitTranslator.Multiply(ref moneyLow, ref moneyHigh, percentage);

        MyAsset.instance.MoneyLow = moneyLow;
        MyAsset.instance.MoneyHigh = moneyHigh;
    }

    public bool CheckPuchasable(ulong priceLow, ulong priceHigh)
    {
        int result = MoneyUnitTranslator.Compare(MyAsset.instance.MoneyLow, MyAsset.instance.MoneyHigh, priceLow, priceHigh);

        if (result >= 0)
            return true;
        else
            return false;
    }
    /*
    public static void SaveMoney()
    {
        PlayerPrefs.SetString("Now_Money2", "" + now_money_2);
    }
    public static void LoadMoney()
    {
        now_money_2 = ulong.Parse(PlayerPrefs.GetString("Now_Money2","0"));
    }
    public static void ResetMoney()
    {
        now_money_2 = 0;
    }*/
}
