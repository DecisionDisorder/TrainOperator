/// <summary>
/// 화폐 단위 계산/변환기
/// </summary>
public class MoneyUnitTranslator
{
    /// <summary>
    /// 한번에 처리 가능한 최대값 (1000경)
    /// </summary>
    private static readonly ulong max = 10000000000000000000;
    /// <summary>
    /// 화폐 단위 변환 기준 (1경)
    /// </summary>
    private static readonly ulong criteria = 10000000000000000;

    /// <summary>
    /// 단위 정리 (경 단위 넘어가는 수치 정리)
    /// </summary>
    /// <param name="lowUnit">1경 미만 단위</param>
    /// <param name="highUnit">1경 이상 단위</param>
    public static void Arrange(ref ulong lowUnit, ref ulong highUnit)
    {
        if(lowUnit >= criteria)
        {
            ulong tmp = lowUnit / criteria;
            highUnit += tmp;
            lowUnit -= tmp * criteria;
        }
    }

    /// <summary>
    /// 덧셈 연산
    /// </summary>
    /// <param name="lowAdd">1경 미만의 더할 값</param>
    /// <param name="highAdd">1경 이상의 더할 값</param>
    /// <param name="lowUnit">1경 미만의 계산 대상 값</param>
    /// <param name="highUnit">1경 이상의 계산 대상 값</param>
    /// <returns></returns>
    public static bool Add(ulong lowAdd, ulong highAdd, ref ulong lowUnit, ref ulong highUnit)
    {
        // 더할 값이 처리 가능한 값인지 확인
        if (lowAdd < max)
        {
            // 기존 값에 더했을 때 처리 가능한 값인지 확인
            if (lowAdd + lowUnit < max)
            {
                lowUnit += lowAdd;
                highUnit += highAdd;
                // 더한 결과가 경 단위를 넘었을 때
                if (lowUnit >= criteria)
                {
                    ulong tmp = lowUnit / criteria;
                    highUnit += tmp;
                    lowUnit -= tmp * criteria;
                }
                return true;
            }
            else
            {
                ulong tmpHigh = lowAdd / criteria;
                ulong tmpLow = lowAdd % criteria;
                highUnit += highAdd + tmpHigh;
                lowUnit += tmpLow;
                return true;
            }
        }

        return false; // Unable to process
    }

    /// <summary>
    /// 뺄셈 연산
    /// </summary>
    /// <param name="lowSubt">1경 미만의 뺄 값</param>
    /// <param name="highSubt">1경 이상의 뺼 값</param>
    /// <param name="lowUnit">1경 미만의 계산 대상 값</param>
    /// <param name="highUnit">1경 이상의 계산 대상 값</param>
    /// <returns></returns>
    public static bool Subtract(ulong lowSubt, ulong highSubt, ref ulong lowUnit, ref ulong highUnit)
    {
        ulong tmpLowUnit = lowUnit;
        ulong tmpHighUnit = highUnit;

        // 뺐을 때 음수가 되지 않는지 확인
        if (highUnit >= highSubt)
        {
            // 큰 단위 뺼셈 진행 후 낮은 단위 뺄셈 진행
            tmpHighUnit -= highSubt;
            ulong tmp;
            if (lowSubt % criteria > tmpLowUnit)
                tmp = (lowSubt / criteria) + 1;
            else
                tmp = lowSubt / criteria;

            if (tmpHighUnit >= tmp)
            {
                tmpHighUnit -= tmp;
                tmpLowUnit += tmp * criteria;
                tmpLowUnit -= lowSubt;

                // 값 적용 (Commit)
                lowUnit = tmpLowUnit; 
                highUnit = tmpHighUnit;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
            return false;
    }

    /// <summary>
    /// 두가지 대형 값의 비교
    /// </summary>
    /// <param name="lowUnit">1경 미만의 기준 값</param>
    /// <param name="highUnit">1경 이상의 기준 값</param>
    /// <param name="lowCompare">1경 미만의 비교 값</param>
    /// <param name="highCompare">1경 이상의 비교 값</param>
    /// <returns>왼쪽이 크면 1, 오른쪽이 크면 -1, 같으면 0</returns>
    public static int Compare(ulong lowUnit, ulong highUnit, ulong lowCompare, ulong highCompare)
    {
        if (lowCompare >= criteria)
        {
            ulong tmp = lowCompare / criteria;
            lowCompare = lowCompare % criteria;
            highCompare += tmp;
        }
        if(lowUnit >= criteria)
        {
            ulong tmp = lowUnit / criteria;
            lowUnit = lowUnit % criteria;
            highUnit += tmp;
        }

        if (highUnit.Equals(highCompare) && lowUnit.Equals(lowCompare))
            return 0;
        else
        {
            if (highUnit > highCompare)
                return 1;
            else if (highUnit.Equals(highCompare))
            {
                if (lowUnit > lowCompare)
                    return 1;
                else
                    return -1;
            }
            else
                return -1;
        }
    }

    /// <summary>
    /// 곱셈 연산
    /// </summary>
    /// <param name="lowUnit">1경 미만의 계산 대상 값</param>
    /// <param name="highUnit">1경 이상의 계산 대상 값</param>
    /// <param name="multiple">곱할 값</param>
    public static void Multiply(ref ulong lowUnit, ref ulong highUnit, ulong multiple)
    {
        decimal dMultiple = multiple;
        decimal totalUnit = highUnit + lowUnit / (decimal)criteria;

        totalUnit *= dMultiple;
        highUnit = (ulong)totalUnit;

        totalUnit -= highUnit;
        lowUnit = (ulong)(totalUnit * criteria);
    }

    /// <summary>
    /// 두 수의 곱이 1000경을 넘는지 확인해주는 함수
    /// </summary>
    /// <param name="multiple1">곱1</param>
    /// <param name="multiple2">곱2</param>
    /// <returns>계산 가능 여부</returns>
    public static bool CheckComputable(ulong multiple1, ulong multiple2)
    {
        int digit1 = 0, digit2 = 0;
        for (; multiple1 >= 10; digit1++)
            multiple1 /= 10;
        for (; multiple2 >= 10; digit2++)
            multiple2 /= 10;

        if (multiple1 * multiple2 > 9)
            digit1++;

        if (digit1 + digit2 >= 16)
            return false;

        return true;
    }

    /// <summary>
    /// 퍼센트 계산
    /// </summary>
    /// <param name="lowUnit">1경 미만의 계산 대상 값</param>
    /// <param name="highUnit">1경 이상의 계산 대상 값</param>
    /// <param name="percentage">정수% (100%) 나타내는 퍼센티지</param>
    /// <returns></returns>
    public static void CalculatePercent(ref ulong lowUnit, ref ulong highUnit, ulong percentage)
    {
        Multiply(ref lowUnit, ref highUnit, percentage / 100f);
    }

    /// <summary>
    /// (소수 둘째짜리 까지 지원) 퍼센트 계산
    /// </summary>
    /// <param name="lowUnit">1경 미만의 계산 대상 값</param>
    /// <param name="highUnit">1경 이상의 계산 대상 값</param>
    /// <param name="percentage">소수% (100%) 나타내는 퍼센티지</param>
    public static void Multiply(ref ulong lowUnit, ref ulong highUnit, float multiple)
    {
        decimal dMultiple = (decimal)multiple;
        decimal totalUnit = highUnit + lowUnit / (decimal)criteria;

        totalUnit *= dMultiple;
        highUnit = (ulong)totalUnit;

        totalUnit -= highUnit;
        lowUnit = (ulong)(totalUnit * criteria);
    }

    /// <summary>
    /// 등차수열의 합을 얻는 함수
    /// </summary>
    /// <param name="amount">개수</param>
    /// <param name="initialTerm">첫째 항</param>
    /// <param name="commonDifference">공차</param>
    public static ulong GetSumOfIncreasing(ulong amount, ulong initialTerm, ulong commonDifference)
    {
        return amount * (2 * initialTerm + (amount - 1) * commonDifference) / 2;
    }
}

/// <summary>
/// 16바이트 연산과 유사하게 작동할 수 있는 변수형 구조체
/// </summary>
[System.Serializable]
public struct LargeVariable
{
    /// <summary>
    /// 0 값
    /// </summary>
    public static LargeVariable zero { get { return new LargeVariable(0, 0); } }

    /// <summary>
    /// 1경 미만의 낮은 단위 값
    /// </summary>
    public ulong lowUnit;
    /// <summary>
    /// 1경 이상의 높은 단위 값
    /// </summary>
    public ulong highUnit;
    /// <summary>
    /// ToString할 때 자세히 표시할 것인지에 대한 여부
    /// </summary>
    public bool detailed;


    public LargeVariable(ulong lowUnit, ulong highUnit)
    {
        this.lowUnit = lowUnit;
        this.highUnit = highUnit;
        detailed = false;
    }

    public LargeVariable(ulong lowUnit, ulong highUnit, bool detailed)
    {
        this.lowUnit = lowUnit;
        this.highUnit = highUnit;
        this.detailed = detailed;
    }

    public static LargeVariable operator +(LargeVariable v1, LargeVariable v2)
    {
        ulong lowUnit = v1.lowUnit, highUnit = v1.highUnit;
        MoneyUnitTranslator.Add(v2.lowUnit, v2.highUnit, ref lowUnit, ref highUnit);
        return new LargeVariable(lowUnit, highUnit);
    }

    public static LargeVariable operator -(LargeVariable v1, LargeVariable v2)
    {
        ulong lowUnit = v1.lowUnit, highUnit = v1.highUnit;
        MoneyUnitTranslator.Subtract(v2.lowUnit, v2.highUnit, ref lowUnit, ref highUnit);
        return new LargeVariable(lowUnit, highUnit);
    }

    public static LargeVariable operator *(LargeVariable variable, float mul)
    {
        ulong lowUnit = variable.lowUnit, highUnit = variable.highUnit;
        MoneyUnitTranslator.Multiply(ref lowUnit, ref highUnit, mul);
        return new LargeVariable(lowUnit, highUnit);
    }

    public static LargeVariable operator *(LargeVariable variable, ulong mul)
    {
        ulong lowUnit = variable.lowUnit, highUnit = variable.highUnit;
        MoneyUnitTranslator.Multiply(ref lowUnit, ref highUnit, mul);
        return new LargeVariable(lowUnit, highUnit);
    }

    public static bool operator < (LargeVariable variable1, LargeVariable variable2)
    {
        int result = MoneyUnitTranslator.Compare(variable1.lowUnit, variable1.highUnit, variable2.lowUnit, variable2.highUnit);
        if (result < 0)
            return true;
        else
            return false;
    }
    public static bool operator >(LargeVariable variable1, LargeVariable variable2)
    {
        int result = MoneyUnitTranslator.Compare(variable1.lowUnit, variable1.highUnit, variable2.lowUnit, variable2.highUnit);
        if (result > 0)
            return true;
        else
            return false;
    }
    public static bool operator <=(LargeVariable variable1, LargeVariable variable2)
    {
        int result = MoneyUnitTranslator.Compare(variable1.lowUnit, variable1.highUnit, variable2.lowUnit, variable2.highUnit);
        if (result <= 0)
            return true;
        else
            return false;
    }
    public static bool operator >=(LargeVariable variable1, LargeVariable variable2)
    {
        int result = MoneyUnitTranslator.Compare(variable1.lowUnit, variable1.highUnit, variable2.lowUnit, variable2.highUnit);
        if (result >= 0)
            return true;
        else
            return false;
    }
    public static bool operator ==(LargeVariable variable1, LargeVariable variable2)
    {
        int result = MoneyUnitTranslator.Compare(variable1.lowUnit, variable1.highUnit, variable2.lowUnit, variable2.highUnit);
        if (result == 0)
            return true;
        else
            return false;
    }
    public static bool operator !=(LargeVariable variable1, LargeVariable variable2)
    {
        int result = MoneyUnitTranslator.Compare(variable1.lowUnit, variable1.highUnit, variable2.lowUnit, variable2.highUnit);
        if (result != 0)
            return true;
        else
            return false;
    }

    public override string ToString()
    {
        string low = "", high = "";
        PlayManager.ArrangeUnit(lowUnit, highUnit, ref low, ref high, detailed);
        return high + low;
    }
}