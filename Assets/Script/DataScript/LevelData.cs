using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������ Ŭ����
/// </summary>
[System.Serializable]
public class LevelData
{
    /// <summary>
    /// �÷��̾��� ����
    /// </summary>
    public int level;
    /// <summary>
    /// �÷��̾��� ����ġ
    /// </summary>
    public int exp;

    public LevelData()
    {
        level = 1;
    }
}
