using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingData
{
    public float soundVolume = 0.5f;
    public bool playTimeActive;
    public bool updateAlarm;
    public bool peaceEventGameActive;
    public int easyPurchaseType;
    public bool addedMoneyEffect;
    /// <summary>
    /// 1: true, 2: false
    /// </summary>
    public int backupRecommend;
    public int currentLineIndex;
    public int currentStationIndex;
    public bool isStationReversed;

    public SettingData()
    {
        soundVolume = 0.5f;
        playTimeActive = true;
        updateAlarm = true;
        peaceEventGameActive = true;
        easyPurchaseType = 1;
        addedMoneyEffect = true;
    }
}
