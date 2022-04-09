using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemCardType { Red, Orange, Yellow, Green };
[System.Serializable]
public class ItemData
{
    public int[] cardAmounts;
    public int[] rareCardAmounts;

    public int silverCardAmount;
    public int rareCardPackAmount;

    public int cardPoint;

    public ItemData()
    {
        cardAmounts = new int[5];
        rareCardAmounts = new int[6];
    }
}
