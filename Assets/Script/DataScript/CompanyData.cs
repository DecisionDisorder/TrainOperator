using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ȸ�� ���� ������ Ŭ����
/// </summary>
[System.Serializable]
public class CompanyData
{
    /// <summary>
    /// �� ������ ��ġ
    /// </summary>
    public int reputationTotalValue;
    /// <summary>
    /// ���� ������
    /// </summary>
    public int additionalPercentage;
    /// <summary>
    /// (Unused) �� ������ ������
    /// </summary>
    public int reputationPercentage;

    /// <summary>
    /// ������ ��ġ
    /// </summary>
    public int averageSanitoryCondition;
    
    /// <summary>
    /// ġ�� ��ġ
    /// </summary>
    public int peaceValue;
    /// <summary>
    /// ġ�� ��ǰ ���� ��Ÿ��
    /// </summary>
    public int peaceCoolTime;
    /// <summary>
    /// ���� ��ǰ ���� ��Ÿ��
    /// </summary>
    public int sanitoryCoolTime;

    /// <summary>
    /// (Unused)�� ��ġ ����
    /// </summary>
    public int valuePoint;
    /// <summary>
    /// (Unused)�����ذ� ���� Ƚ��
    /// </summary>
    public int numOfSuccess;

    /// <summary>
    /// ���� ���� ���� �ð�
    /// </summary>
    public int inspectSanitoryTime;

    public CompanyData()
    {
        reputationPercentage = 100;
        reputationPercentage = 10;
        averageSanitoryCondition = 20;
        peaceValue = 20;
        peaceCoolTime = 5;
        sanitoryCoolTime = 5;
        inspectSanitoryTime = 240;
    }
}
