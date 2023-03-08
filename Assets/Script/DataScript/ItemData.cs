using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷� ������ ī�� ���� ������
/// </summary>
public enum ItemCardType { Red, Orange, Yellow, Green };

/// <summary>
/// ������ ������ Ŭ����
/// </summary>
[System.Serializable]
public class ItemData
{
    /// <summary>
    /// �� ������ �÷� ī�� ������
    /// </summary>
    public int[] cardAmounts;
    /// <summary>
    /// �� ������ ���� ī�� ������
    /// </summary>
    public int[] rareCardAmounts;

    /// <summary>
    /// �÷� ī���� ������
    /// </summary>
    public int silverCardAmount;
    /// <summary>
    /// ���� ī���� ������
    /// </summary>
    public int rareCardPackAmount;

    /// <summary>
    /// ī�� ����Ʈ ������
    /// </summary>
    public int cardPoint;
    /// <summary>
    /// �ǹ� ���� ���� ������
    /// </summary>
    public int feverRefillAmount;

    public ItemData()
    {
        cardAmounts = new int[5];
        rareCardAmounts = new int[6];
    }
}
