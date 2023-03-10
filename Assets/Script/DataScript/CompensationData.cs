using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������ Ŭ����
/// </summary>
[System.Serializable]
public class CompensationData
{
    /// <summary>
    /// �λ� �뼱 ���� ���� ���� ����
    /// </summary>
    public bool bsCompensationChecked;
    /// <summary>
    /// ���� Ȯ�� ���� ���� ���� ����
    /// </summary>
    public bool expandTrainChecked;
    /// <summary>
    /// ���� Ȯ�� �°��� ���� ����
    /// </summary>
    public bool expandTrainPassengerChecked;
    /// <summary>
    /// 3.0.1 ���� ���� ����
    /// </summary>
    public bool version3_0_1_checked;
    /// <summary>
    /// ���� Ȯ�� ���� ���� ���� ����
    /// </summary>
    public bool allExpandErrorChecked;
    /// <summary>
    /// �� ������ ��ġ ��ȯ ����
    /// </summary>
    public bool reviseReputation;
    /// <summary>
    /// ����ö ���� ���� ����
    /// </summary>
    public bool lightRailCompensationChecked;
    /// <summary>
    /// ���� ���� ���� ����
    /// </summary>
    public bool lotteryErrorRecovered = false;
}
