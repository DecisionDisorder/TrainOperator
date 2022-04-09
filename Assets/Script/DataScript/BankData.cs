using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BankData
{
    public ulong[] savedMoneyNormal;
    public ulong[] addedMoneyNormal;
    public int[] contractTimesNormal;
    public bool[] isRegisteredNormal;

    public ulong[] savedMoneySpecial;
    public ulong[] addedMoneySpecialLow;
    public ulong[] addedMoneySpecialHigh;
    public int[] contractTimesSpecial;
    public bool[] isRegisteredSpecial;

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
