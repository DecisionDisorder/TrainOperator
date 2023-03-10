using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 보상 데이터 클래스
/// </summary>
[System.Serializable]
public class CompensationData
{
    /// <summary>
    /// 부산 노선 오류 보상 수령 여부
    /// </summary>
    public bool bsCompensationChecked;
    /// <summary>
    /// 열차 확장 개수 오류 복구 여부
    /// </summary>
    public bool expandTrainChecked;
    /// <summary>
    /// 열차 확장 승객수 복구 여부
    /// </summary>
    public bool expandTrainPassengerChecked;
    /// <summary>
    /// 3.0.1 보상 수령 여부
    /// </summary>
    public bool version3_0_1_checked;
    /// <summary>
    /// 열차 확장 오류 보상 수령 여부
    /// </summary>
    public bool allExpandErrorChecked;
    /// <summary>
    /// 고객 만족도 수치 변환 여부
    /// </summary>
    public bool reviseReputation;
    /// <summary>
    /// 경전철 보상 수령 여부
    /// </summary>
    public bool lightRailCompensationChecked;
    /// <summary>
    /// 복권 오류 복원 여부
    /// </summary>
    public bool lotteryErrorRecovered = false;
}
