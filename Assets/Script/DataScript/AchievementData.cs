using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������� ������ Ŭ����
/// </summary>
[System.Serializable]
public class AchievementData
{
    /// <summary>
    /// �� ���������� ����
    /// </summary>
    public int[] achvClearLevels;
    /// <summary>
    /// ����ڰ� �����ߴ� �ִ��� ��
    /// </summary>
    public LargeVariable maxMoney;
    /// <summary>
    /// ����ڰ� ��ġ�޴��� ��ġ�� Ƚ��
    /// </summary>
    public int touchCount;
    /// <summary>
    /// ����ڰ� ������ ���� ����
    /// </summary>
    public int totalStationAmount;
    /// <summary>
    /// ����ڰ� ȹ���� ���� ����
    /// </summary>
    public LargeVariable cumulativeInterest;

    public AchievementData()
    {
        achvClearLevels = new int[9];
        maxMoney = LargeVariable.zero;
        cumulativeInterest = LargeVariable.zero;
    }
}
