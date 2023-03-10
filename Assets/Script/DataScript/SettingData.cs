using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 설정 데이터 클래스
/// </summary>
[System.Serializable]
public class SettingData
{
    /// <summary>
    /// 효과음 사운드 볼륨
    /// </summary>
    public float soundVolume = 0.5f;
    /// <summary>
    /// 플레이 타임 표기 여부
    /// </summary>
    public bool playTimeActive;
    /// <summary>
    /// 패치노트 알림 여부
    /// </summary>
    public bool updateAlarm;
    /// <summary>
    /// 미니게임 활성화 여부
    /// </summary>
    public bool peaceEventGameActive;
    /// <summary>
    /// 간편 구매 타입
    /// </summary>
    public int easyPurchaseType;
    /// <summary>
    /// 터치 수익 효과 여부
    /// </summary>
    public bool addedMoneyEffect;
    /// <summary>
    /// 백업 추천 알림 여부
    /// </summary>
    public int backupRecommend;
    /// <summary>
    /// 현재 배경에 적용 중인 노선 인덱스
    /// </summary>
    public int currentLineIndex;
    /// <summary>
    /// 현재 배경에 적용 중인 역 인덱스
    /// </summary>
    public int currentStationIndex;
    /// <summary>
    /// 노선 방향
    /// </summary>
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
