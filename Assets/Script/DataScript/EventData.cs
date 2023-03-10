using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �̺�Ʈ ������ Ŭ����
/// </summary>
[System.Serializable]
public class EventData
{
    /// <summary>
    /// ���̽��� ���ƿ� ���� ���� ����
    /// </summary>
    public bool isRecommended;
    /// <summary>
    /// �������� ���� ���� ����
    /// </summary>
    public bool isSurveyed;
    /// <summary>
    /// ������Ʈ �������� ���� ���� ����
    /// </summary>
    public bool isUpdateSurveyed;
    /// <summary>
    /// ��� �������� ���� ���� ����
    /// </summary>
    public bool didNormalSurvey;
}
