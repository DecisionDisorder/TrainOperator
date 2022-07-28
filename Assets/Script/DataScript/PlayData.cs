using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class PlayData
{
    public int playTimeHour;
    public int playTimeMin;
    public int playTimeSec;

    public bool isConverted;
    public bool isRevised;
    public bool isRevised_3_1_4;

    public bool notTutorialPlayed;
    /// <summary>
    /// not이 아니라 new임
    /// </summary>
    public bool rentTutorialPlayed;
    public bool notBankTutorialPlayed;

    public int backgroundType;
    public int patchNoteVersionCode;

    public DateTime recentConnectedTime;

    public PlayData()
    {
        notTutorialPlayed = true;
        notBankTutorialPlayed = true;
    }
}
