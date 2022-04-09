using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DriverData
{
    public int[] numOfDrivers;
    public int salaryTimer;

    public DriverData()
    {
        numOfDrivers = new int[11];
    }
}
