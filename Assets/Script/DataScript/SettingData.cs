using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������ Ŭ����
/// </summary>
[System.Serializable]
public class SettingData
{
    /// <summary>
    /// ȿ���� ���� ����
    /// </summary>
    public float soundVolume = 0.5f;
    /// <summary>
    /// �÷��� Ÿ�� ǥ�� ����
    /// </summary>
    public bool playTimeActive;
    /// <summary>
    /// ��ġ��Ʈ �˸� ����
    /// </summary>
    public bool updateAlarm;
    /// <summary>
    /// �̴ϰ��� Ȱ��ȭ ����
    /// </summary>
    public bool peaceEventGameActive;
    /// <summary>
    /// ���� ���� Ÿ��
    /// </summary>
    public int easyPurchaseType;
    /// <summary>
    /// ��ġ ���� ȿ�� ����
    /// </summary>
    public bool addedMoneyEffect;
    /// <summary>
    /// ��� ��õ �˸� ����
    /// </summary>
    public int backupRecommend;
    /// <summary>
    /// ���� ��濡 ���� ���� �뼱 �ε���
    /// </summary>
    public int currentLineIndex;
    /// <summary>
    /// ���� ��濡 ���� ���� �� �ε���
    /// </summary>
    public int currentStationIndex;
    /// <summary>
    /// �뼱 ����
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
