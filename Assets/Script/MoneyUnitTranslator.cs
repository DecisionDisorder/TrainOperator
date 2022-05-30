public class MoneyUnitTranslator
{
    private static readonly ulong max = 10000000000000000000;
    private static readonly ulong criteria = 10000000000000000;


    public static void Arrange(ref ulong lowUnit, ref ulong highUnit)
    {
        if(lowUnit >= criteria)
        {
            ulong tmp = lowUnit / criteria;
            highUnit += tmp;
            lowUnit -= tmp * criteria;
        }
    }

    public static bool Add(ulong lowAdd, ulong highAdd, ref ulong lowUnit, ref ulong highUnit)
    {
        if (lowAdd < max)
        {
            if (lowAdd + lowUnit < max)
            {
                lowUnit += lowAdd;
                highUnit += highAdd;
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

    public static bool Subtract(ulong lowSubt, ulong highSubt, ref ulong lowUnit, ref ulong highUnit)
    {
        ulong tmpLowUnit = lowUnit;
        ulong tmpHighUnit = highUnit;

        if (highUnit >= highSubt)
        {
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

                lowUnit = tmpLowUnit; // commit
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
    /// ������ ũ�� 1, �������� ũ�� -1, ������ 0
    /// </summary>
    /// <param name="lowUnit"></param>
    /// <param name="highUnit"></param>
    /// <param name="lowCompare"></param>
    /// <param name="highCompare"></param>
    /// <returns></returns>
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

    public static void Multiply(ref ulong lowUnit, ref ulong highUnit, ulong multiple)
    {
        decimal dMultiple = multiple;
        decimal totalUnit = highUnit + lowUnit / (decimal)criteria;

        totalUnit *= dMultiple;
        highUnit = (ulong)totalUnit;

        totalUnit -= highUnit;
        lowUnit = (ulong)(totalUnit * criteria);
    }

    private static ulong Power10(int n)
    {
        ulong res = 1;
        for (int i = 0; i < n; i++)
            res *= 10;
        return res;
    }

    /// <summary>
    /// �� ���� ���� 1000���� �Ѵ��� Ȯ�����ִ� �Լ�
    /// </summary>
    /// <param name="multiple1">��1</param>
    /// <param name="multiple2">��2</param>
    /// <returns></returns>
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
    /// �ۼ�Ʈ ���
    /// </summary>
    /// <param name="lowUnit">���� ����</param>
    /// <param name="highUnit">���� ����</param>
    /// <param name="percentage">����% (100%) ��Ÿ���� �ۼ�Ƽ��</param>
    /// <returns></returns>
    public static void CalculatePercent(ref ulong lowUnit, ref ulong highUnit, ulong percentage)
    {
        /*ulong tmp = highUnit % 100;
        highUnit -= highUnit % 100;
        if (highUnit >= 100)
        {
            tmp += 100;
            highUnit -= 100;
        }
        lowUnit += tmp * criteria;

        
        highUnit = (ulong)(highUnit * (percentage / 100f));
        lowUnit = (ulong)(lowUnit * (percentage / 100f));

        Arrange(ref lowUnit, ref highUnit);*/
        Multiply(ref lowUnit, ref highUnit, percentage / 100f);
    }

    /// <summary>
    /// (�Ҽ� ��°¥�� ���� ����) �ۼ�Ʈ ���
    /// </summary>
    /// <param name="lowUnit">���� ����</param>
    /// <param name="highUnit">���� ����</param>
    /// <param name="percentage">�Ҽ�% (100%) ��Ÿ���� �ۼ�Ƽ��</param>
    public static void Multiply(ref ulong lowUnit, ref ulong highUnit, float multiple)
    {
        /*ulong div = 1000;
        decimal p = (decimal)percentage;
        ulong multiple = (ulong)(p * div);
        ulong a = multiple / div, b = multiple % div;

        lowUnit = lowUnit * a + lowUnit * b / div;
        ulong tmp = (highUnit * b) % div;
        highUnit = highUnit * a + highUnit * b / div;
        lowUnit += tmp * criteria / div;

        Arrange(ref lowUnit, ref highUnit);*/

        decimal dMultiple = (decimal)multiple;
        decimal totalUnit = highUnit + lowUnit / (decimal)criteria;

        totalUnit *= dMultiple;
        highUnit = (ulong)totalUnit;

        totalUnit -= highUnit;
        lowUnit = (ulong)(totalUnit * criteria);
    }

    /// <summary>
    /// ���������� ���� ��� �Լ�
    /// </summary>
    /// <param name="amount">����</param>
    /// <param name="initialTerm">ù° ��</param>
    /// <param name="commonDifference">����</param>
    public static ulong GetSumOfIncreasing(ulong amount, ulong initialTerm, ulong commonDifference)
    {
        return amount * (2 * initialTerm + (amount - 1) * commonDifference) / 2;
    }
}

[System.Serializable]
public struct LargeVariable
{
    public static LargeVariable zero { get { return new LargeVariable(0, 0); } }

    public ulong lowUnit;
    public ulong highUnit;


    public LargeVariable(ulong lowUnit, ulong highUnit)
    {
        this.lowUnit = lowUnit;
        this.highUnit = highUnit;
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
}