using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// �÷��� ������ Ŭ����
/// </summary>
[System.Serializable]
public class PlayData
{
    /// <summary>
    /// �÷��� Ÿ��(�ð�)
    /// </summary>
    public int playTimeHour;
    /// <summary>
    /// �÷��� Ÿ��(��)
    /// </summary>
    public int playTimeMin;
    /// <summary>
    /// �÷��� Ÿ��(��)
    /// </summary>
    public int playTimeSec;

    /// <summary>
    /// �뷱�� ������Ʈ�� ���� �����Ͱ� ��ȯ �Ǿ����� ����
    /// </summary>
    public bool isConverted;
    /// <summary>
    /// �뷱�� ������Ʈ�� ���� ������ ������ �Ǿ����� ����
    /// </summary>
    public bool isRevised;
    /// <summary>
    /// 3.1.4 ���� ������Ʈ�� ���� ������ ������ �Ǿ����� ����
    /// </summary>
    public bool isRevised_3_1_4;

    /// <summary>
    /// Ʃ�丮���� ���� �ʾҴ��� ����
    /// </summary>
    public bool notTutorialPlayed;

    /// <summary>
    /// �Ӵ� �ý��� Ʃ�丮���� ���� �ʾҴ��� ����
    /// </summary>
    public bool rentTutorialPlayed;
    /// <summary>
    /// ���� �ý��� Ʃ�丮���� ���� �ʾҴ��� ����
    /// </summary>
    public bool notBankTutorialPlayed;

    /// <summary>
    /// ��� Ÿ�� (�Ϲ�/��ũ������)
    /// </summary>
    public int backgroundType;
    /// <summary>
    /// ��ġ��Ʈ�� ���� �ڵ� (������ ������Ʈ �Ǿ����� Ȯ�� �뵵)
    /// </summary>
    public int patchNoteVersionCode;

    /// <summary>
    /// 3.1.4 ������ �뼱 ������ ����
    /// </summary>
    public bool didLineAdd314 = false;

    /// <summary>
    /// �ֱ� ���� �ð� ���
    /// </summary>
    public DateTime recentConnectedTime;

    public PlayData()
    {
        notTutorialPlayed = true;
        notBankTutorialPlayed = true;
    }
}
