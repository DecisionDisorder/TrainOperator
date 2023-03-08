using UnityEngine;
using System.Collections;

/// <summary>
/// 자산 계산기 클래스
/// </summary>
public class AssetMoneyCalculator : MonoBehaviour {
    /// <summary>
    /// 자산 계산기 싱글톤 인스턴스
    /// </summary>
    public static AssetMoneyCalculator instance;

    /// <summary>
    /// 기본적 최대 단위(1경)
    /// </summary>
    public static readonly ulong standard_maximum = 10000000000000000;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 보유 중인 돈을 더하거나 빼는 Helper 메소드
    /// </summary>
    /// <param name="lowUnit">0~9999조</param>
    /// <param name="HighUnit">1경 이상</param>
    /// <param name="plus">덧셈/뺄셈 여부</param>
    /// <returns>연산 성공 여부</returns>
    public bool ArithmeticOperation(ulong lowUnit, ulong HighUnit, bool plus)
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

    /// <summary>
    /// 보유 중인 돈을 더하거나 빼는 Helper 메소드
    /// </summary>
    /// <param name="variable">더하거나 뺼 금액</param>
    /// <param name="plus">덧셈/뺄셈 여부</param>
    /// <returns></returns>
    public bool ArithmeticOperation(LargeVariable variable, bool plus)
    {
        return ArithmeticOperation(variable.lowUnit, variable.highUnit, plus);
    }

    /// <summary>
    /// 강제로 자산을 빼면서 0보다 낮아질 경우 0으로 지정한다.
    /// </summary>
    /// <param name="lowUnit">0~9999조</param>
    /// <param name="highUnit">1경 이상</param>
    public void ForceSubtract(ulong lowUnit, ulong highUnit)
    {
        ulong moneyLow = MyAsset.instance.MoneyLow;
        ulong moneyHigh = MyAsset.instance.MoneyHigh;
        if (!MoneyUnitTranslator.Subtract(lowUnit, highUnit, ref moneyLow, ref moneyHigh))
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
    
    /// <summary>
    /// 자산에 퍼센트 계산
    /// </summary>
    /// <param name="percentage">정수 단위의 퍼센트(xx%)</param>
    public void PercentOperation(ulong percentage)
    {
        ulong moneyLow = MyAsset.instance.MoneyLow;
        ulong moneyHigh = MyAsset.instance.MoneyHigh;

        MoneyUnitTranslator.CalculatePercent(ref moneyLow, ref moneyHigh, percentage);

        MyAsset.instance.MoneyLow = moneyLow;
        MyAsset.instance.MoneyHigh = moneyHigh;
    }

    /// <summary>
    /// 자산에 퍼센트 계산
    /// </summary>
    /// <param name="percentage">소수 단위의 비율(0.xx)</param>
    public void PercentOperation(float percentage)
    {
        ulong moneyLow = MyAsset.instance.MoneyLow;
        ulong moneyHigh = MyAsset.instance.MoneyHigh;

        MoneyUnitTranslator.Multiply(ref moneyLow, ref moneyHigh, percentage);

        MyAsset.instance.MoneyLow = moneyLow;
        MyAsset.instance.MoneyHigh = moneyHigh;
    }

    /// <summary>
    /// 구매 가능한지 여부를 확인
    /// </summary>
    /// <param name="priceLow">가격의 0~9999조 단위</param>
    /// <param name="priceHigh">가격의 1경 이상 단위</param>
    /// <returns></returns>
    public bool CheckPuchasable(ulong priceLow, ulong priceHigh)
    {
        int result = MoneyUnitTranslator.Compare(MyAsset.instance.MoneyLow, MyAsset.instance.MoneyHigh, priceLow, priceHigh);

        if (result >= 0)
            return true;
        else
            return false;
    }
}
