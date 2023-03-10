using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이벤트 데이터 클래스
/// </summary>
[System.Serializable]
public class EventData
{
    /// <summary>
    /// 페이스북 좋아요 보상 수령 여부
    /// </summary>
    public bool isRecommended;
    /// <summary>
    /// 설문조사 보상 수령 여부
    /// </summary>
    public bool isSurveyed;
    /// <summary>
    /// 업데이트 설문조사 보상 수령 여부
    /// </summary>
    public bool isUpdateSurveyed;
    /// <summary>
    /// 상시 설문조사 보상 수령 여부
    /// </summary>
    public bool didNormalSurvey;
}
