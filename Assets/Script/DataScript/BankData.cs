using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������ Ŭ����
/// </summary>
[System.Serializable]
public class BankData
{
    /// <summary>
    /// ��ġ �ݾ�: �Ϲ� ��ǰ
    /// </summary>
    public ulong[] savedMoneyNormal;
    /// <summary>
    /// ���� �ݾ�: �Ϲ� ��ǰ
    /// </summary>
    public ulong[] addedMoneyNormal;
    /// <summary>
    /// ��� �ð�: �Ϲ� ��ǰ
    /// </summary>
    public int[] contractTimesNormal;
    /// <summary>
    /// ���� ����: �Ϲ� ��ǰ
    /// </summary>
    public bool[] isRegisteredNormal;

    /// <summary>
    /// ��ġ �ݾ�: ����� ��ǰ
    /// </summary>
    public ulong[] savedMoneySpecial;
    /// <summary>
    /// ���� �ݾ�-Low ����(0~9999��): ����� ��ǰ
    /// </summary>
    public ulong[] addedMoneySpecialLow;
    /// <summary>
    /// ���� �ݾ�-High ����(1��~): ����� ��ǰ
    /// </summary>
    public ulong[] addedMoneySpecialHigh;
    /// <summary>
    /// ��� �ð�: ����� ��ǰ 
    /// </summary>
    public int[] contractTimesSpecial;
    /// <summary>
    /// ���� ����: ����� ��ǰ
    /// </summary>
    public bool[] isRegisteredSpecial;

    /// <summary>
    /// ���� ���� ���� �ð�: ����� ��ǰ
    /// </summary>
    public int timer;

    public BankData()
    {
        savedMoneyNormal = new ulong[3];
        addedMoneyNormal = new ulong[3];
        contractTimesNormal = new int[3];
        isRegisteredNormal = new bool[3];

        savedMoneySpecial = new ulong[3];
        addedMoneySpecialHigh = new ulong[3];
        addedMoneySpecialLow = new ulong[3];
        contractTimesSpecial = new int[3];
        isRegisteredSpecial = new bool[3];
    }
}
