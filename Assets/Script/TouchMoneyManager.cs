using UnityEngine;
using System.Collections;

/// <summary>
/// 시간형 수익 계산 관리 시스템 클래스
/// </summary>
public class TouchMoneyManager : MonoBehaviour
{
    /// <summary>
    /// 1경 미만의 승객 수 기준치
    /// </summary>
    public static ulong PassengersBaseLow;

    /// <summary>
    /// 1경 미만의 승객 수 랜덤 적용 수치
    /// </summary>
    private static ulong passengersRandomLow;

    /// <summary>
    /// 1경 미만의 승객 수 랜덤 적용 수치
    /// </summary>
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

    /// <summary>
    /// 1경 이상의 승객 수 기준치
    /// </summary>
    public static ulong PassengersBaseHigh;
    /// <summary>
    /// 1경 이상의 승객 수 랜덤 적용 수치
    /// </summary>
    private static ulong passengersRandomHigh;
    /// <summary>
    /// 1경 이상의 승객 수 랜덤 적용 수치
    /// </summary>
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

    /// <summary>
    /// 1경 미만의 최종 터치형 수익
    /// </summary>
    private static ulong touchMoneyLow;
    /// <summary>
    /// 1경 미만의 최종 터치형 수익
    /// </summary>
    public static ulong TouchMoneyLow
    {
        get { return touchMoneyLow; }
        set
        {
            touchMoneyLow = value;
            if (AssetInfoUpdater.instance != null)
                AssetInfoUpdater.instance.UpdateTouchMoneyText();
        }
    }
    /// <summary>
    /// 1경 이상의 최종 터치형 수익
    /// </summary>
    private static ulong touchMoneyHigh;
    /// <summary>
    /// 1경 이상의 최종 터치형 수익
    /// </summary>
    public static ulong TouchMoneyHigh { 
        get { return touchMoneyHigh; }
        set
        {
            touchMoneyHigh = value;

            if (AssetInfoUpdater.instance != null)
                AssetInfoUpdater.instance.UpdateTouchMoneyText();
        }
    }

    /// <summary>
    /// Low/High 단위의 기준치 (1경)
    /// </summary>
    public static ulong standardMaximum = 10000000000000000;

    void Start()
    {
        // 불러온 데이터의 Low/High 단위 정리
        ulong passengersLow = MyAsset.instance.myAssetData.passengersLow;
        ulong passengersHigh = MyAsset.instance.myAssetData.passengersHigh;

        MoneyUnitTranslator.Arrange(ref passengersLow, ref passengersHigh);

        MyAsset.instance.myAssetData.passengersLow = passengersLow;
        MyAsset.instance.myAssetData.passengersHigh = passengersHigh;
    }


    /// <summary>
    /// 승객 수에 대한 덧셈/뺄셈 연산
    /// </summary>
    /// <param name="firstunit">1경 미만의 단위</param>
    /// <param name="secondunit">1경 이상의 단위</param>
    /// <param name="plus">덧셈 여부</param>
    public static void ArithmeticOperation(ulong firstunit, ulong secondunit, bool plus) 
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
    /// <summary>
    /// 승객 수 제한량에 대한 덧셈 연산
    /// </summary>
    /// <param name="lowUnit">1경 미만의 단위</param>
    /// <param name="highUnit">1경 이상의 단위</param>
    public static void AddPassengerLimit(ulong lowUnit, ulong highUnit)
    {
        ulong passengersLimitLow = MyAsset.instance.myAssetData.passengersLimitLow;
        ulong passengersLimitHigh = MyAsset.instance.myAssetData.passengersLimitHigh;
        MoneyUnitTranslator.Add(lowUnit, highUnit, ref passengersLimitLow, ref passengersLimitHigh);

        MyAsset.instance.myAssetData.passengersLimitLow = passengersLimitLow;
        MyAsset.instance.myAssetData.passengersLimitHigh = passengersLimitHigh;

        AssetInfoUpdater.instance.UpdatePassengerText();
    }

    /// <summary>
    /// 승객 수 제한을 초과하지 않는 지에 대한 비교
    /// </summary>
    /// <param name="passengerToAddLow">더해질 1경 미만의 승객 수</param>
    /// <param name="passengerToAddHigh">더해질 1경 이상의 승객 수</param>
    /// <returns>더했을 때 제한 수치를 넘기지 않는지에 대한 여부</returns>
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
    /// <summary>
    /// 승객 수에 대한 백분율 연산
    /// </summary>
    /// <param name="percentage">적용할 백분율</param>
    /// <param name="fu">1경 미만의 기준치</param>
    /// <param name="su">1경 이상의 기준치</param>
    /// <param name="lowUnit">1경 미만의 계산 결과</param>
    /// <param name="highUnit">1경 이상의 계산 결과</param>
    public static void PercentCalculation(ulong percentage , ulong fu, ulong su, ref ulong lowUnit, ref ulong highUnit)
    {
        lowUnit = fu;
        highUnit = su;
        MoneyUnitTranslator.CalculatePercent(ref lowUnit, ref highUnit, percentage);
    }

    /// <summary>
    /// 보상 계산을 위한 승객 수에 대해 곱 연산
    /// </summary>
    /// <param name="mag">곱할 수치</param>
    /// <param name="lowUnit">1경 미만의 계산 결과</param>
    /// <param name="highUnit">1경 이상의 계산 결과</param>
    public static void Multiply(int mag, ref ulong lowUnit, ref ulong highUnit)
    {
        lowUnit = PassengersRandomLow;
        highUnit = PassengersRandomHigh;
        lowUnit *= (ulong)mag;
        highUnit *= (ulong)mag;
        MoneyUnitTranslator.Arrange(ref lowUnit, ref highUnit);
    }
}
