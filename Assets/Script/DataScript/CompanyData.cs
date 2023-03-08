using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 회사 관리 데이터 클래스
/// </summary>
[System.Serializable]
public class CompanyData
{
    /// <summary>
    /// 고객 만족도 수치
    /// </summary>
    public int reputationTotalValue;
    /// <summary>
    /// 수익 증가율
    /// </summary>
    public int additionalPercentage;
    /// <summary>
    /// (Unused) 고객 만족도 증가율
    /// </summary>
    public int reputationPercentage;

    /// <summary>
    /// 위생도 수치
    /// </summary>
    public int averageSanitoryCondition;
    
    /// <summary>
    /// 치안 수치
    /// </summary>
    public int peaceValue;
    /// <summary>
    /// 치안 상품 구매 쿨타임
    /// </summary>
    public int peaceCoolTime;
    /// <summary>
    /// 위생 상품 구매 쿨타임
    /// </summary>
    public int sanitoryCoolTime;

    /// <summary>
    /// (Unused)역 가치 점수
    /// </summary>
    public int valuePoint;
    /// <summary>
    /// (Unused)범죄해결 성공 횟수
    /// </summary>
    public int numOfSuccess;

    /// <summary>
    /// 위생 점검 남은 시간
    /// </summary>
    public int inspectSanitoryTime;

    public CompanyData()
    {
        reputationPercentage = 100;
        reputationPercentage = 10;
        averageSanitoryCondition = 20;
        peaceValue = 20;
        peaceCoolTime = 5;
        sanitoryCoolTime = 5;
        inspectSanitoryTime = 240;
    }
}
