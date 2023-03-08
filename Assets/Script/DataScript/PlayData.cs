using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 플레이 데이터 클래스
/// </summary>
[System.Serializable]
public class PlayData
{
    /// <summary>
    /// 플레이 타임(시간)
    /// </summary>
    public int playTimeHour;
    /// <summary>
    /// 플레이 타임(분)
    /// </summary>
    public int playTimeMin;
    /// <summary>
    /// 플레이 타임(초)
    /// </summary>
    public int playTimeSec;

    /// <summary>
    /// 밸런스 업데이트로 인한 데이터가 변환 되었는지 여부
    /// </summary>
    public bool isConverted;
    /// <summary>
    /// 밸런스 업데이트로 인한 데이터 보정이 되었는지 여부
    /// </summary>
    public bool isRevised;
    /// <summary>
    /// 3.1.4 버전 업데이트로 인한 데이터 보정이 되었는지 여부
    /// </summary>
    public bool isRevised_3_1_4;

    /// <summary>
    /// 튜토리얼을 하지 않았는지 여부
    /// </summary>
    public bool notTutorialPlayed;

    /// <summary>
    /// 임대 시스템 튜토리얼을 하지 않았는지 여부
    /// </summary>
    public bool rentTutorialPlayed;
    /// <summary>
    /// 은행 시스템 튜토리얼을 하지 않았는지 여부
    /// </summary>
    public bool notBankTutorialPlayed;

    /// <summary>
    /// 배경 타입 (일반/스크린도어)
    /// </summary>
    public int backgroundType;
    /// <summary>
    /// 패치노트의 버전 코드 (버전이 업데이트 되었는지 확인 용도)
    /// </summary>
    public int patchNoteVersionCode;

    /// <summary>
    /// 3.1.4 버전의 노선 데이터 보정
    /// </summary>
    public bool didLineAdd314 = false;

    /// <summary>
    /// 최근 접속 시간 기록
    /// </summary>
    public DateTime recentConnectedTime;

    public PlayData()
    {
        notTutorialPlayed = true;
        notBankTutorialPlayed = true;
    }
}
