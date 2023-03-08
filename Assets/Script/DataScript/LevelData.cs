using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 레벨 데이터 클래스
/// </summary>
[System.Serializable]
public class LevelData
{
    /// <summary>
    /// 플레이어의 레벨
    /// </summary>
    public int level;
    /// <summary>
    /// 플레이어의 경험치
    /// </summary>
    public int exp;

    public LevelData()
    {
        level = 1;
    }
}
