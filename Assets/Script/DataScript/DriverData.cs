using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����� ������ Ŭ����
/// </summary>
[System.Serializable]
public class DriverData
{
    /// <summary>
    /// �� ��޺� ����� ���� Ƚ��
    /// </summary>
    public int[] numOfDrivers;
    /// <summary>
    /// ���� ���� ���� �ð�
    /// </summary>
    public int salaryTimer;

    public DriverData()
    {
        numOfDrivers = new int[11];
    }
}
