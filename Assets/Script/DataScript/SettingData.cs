using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingData
{
    public float soundVolume;
    public bool playTimeActive;
    public bool updateAlarm;
    public bool peaceEventGameActive;
    public bool peddlerEventGameActive;
    public int easyPurchaseType;
    public bool addedMoneyEffect;
    /// <summary>
    /// 1: true, 2: false
    /// </summary>
    public int backupRecommend;
    public int currentStationIndex;
    public bool isStationReversed;

    public SettingData()
    {
        soundVolume = 1f;
        playTimeActive = true;
        updateAlarm = true;
        peaceEventGameActive = true;
        peddlerEventGameActive = true;
        easyPurchaseType = 1;
        addedMoneyEffect = true;
    }
}
