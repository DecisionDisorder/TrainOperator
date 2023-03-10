using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ��ũ�� ���� �ý��� ���� Ŭ����
/// </summary>
public class MacroDetector : MonoBehaviour
{
    /// <summary>
    /// ��ũ�� ��� ��� ȭ�� ������Ʈ
    /// </summary>
    public GameObject warnObj;

    /// <summary>
    /// ���� ��ġ �ð�
    /// </summary>
    private DateTime priorTouchTime;
    /// <summary>
    /// �ֱ� 100ȸ ������ ��ġ ���� ���
    /// </summary>
    private int[] intervalRecords = new int[100];
    /// <summary>
    /// ���� ��� �ε���
    /// </summary>
    private int recordedIndex = 0;

    /// <summary>
    /// TooMuchTouch(�����ð� �ſ� ���� ��ġ �� �ݺ�) ���� ���� �ð�
    /// </summary>
    private int tmtSeconds;
    /// <summary>
    /// MuchTouch(��ð� ���� ��ġ �� �ݺ�) ���� ���� �ð�
    /// </summary>
    private int mtSeconds;

    private void Start()
    {
        priorTouchTime = DateTime.Now;
    }

    /// <summary>
    /// ��ġ Ƚ�� �˻�
    /// </summary>
    /// <param name="touchPerSecond">�ʴ� ��ġ Ƚ��</param>
    public void DetectTouchAmount(int touchPerSecond)
    {
        DetectMuchTouch(touchPerSecond);
        DetectTooMuchTouch(touchPerSecond);
    }

    /// <summary>
    /// 1�ʿ� 100ȸ �̻��� 60�� �̻� ������ ��ũ�� ���� ó��
    /// </summary>
    private void DetectTooMuchTouch(int touchPerSecond)
    {
        if(touchPerSecond >= 100)
        {
            tmtSeconds++;
            if(tmtSeconds >= 60)
            {
                warnObj.SetActive(true);
            }
        }
        else
        {
            tmtSeconds = 0;
        }
    }

    /// <summary>
    /// 1�ʿ� 40ȸ �̻� 3�� �̻� ������ ��ũ�� ���� ó��
    /// </summary>
    private void DetectMuchTouch(int touchPerSecond)
    {
        if (touchPerSecond > 40)
        {
            mtSeconds++;
            if (mtSeconds >= 180)
            {
                warnObj.SetActive(true);
            }
        }
        else
        {
            mtSeconds = 0;
        }
    }

    /// <summary>
    /// 100ȸ ���� �̻� ������ �ð� �������� ��ġ�ϴ��� Ȯ��
    /// </summary>
    public void DetectInterval()
    {
        // ms ������ ������ �������� ������ ��ġ �ϴ��� Ȯ�� �� ���â Ȱ��ȭ
        TimeSpan timeSpan = DateTime.Now - priorTouchTime;
        int interval = (int)(timeSpan.TotalMilliseconds * 100);
        if (recordedIndex != 0)
        {
            if (intervalRecords[recordedIndex - 1].Equals(interval))
            {
                intervalRecords[recordedIndex++] = interval;
                if (recordedIndex.Equals(intervalRecords.Length))
                {
                    warnObj.SetActive(true);
                    recordedIndex = 0;
                }
            }
            else
                recordedIndex = 0;
        }
        else
        {
            intervalRecords[recordedIndex++] = interval;
        }
        priorTouchTime = DateTime.Now;
    }
    /// <summary>
    /// ��� �޴� ��Ȱ��ȭ
    /// </summary>
    public void CloseDetectedWarn()
    {
        warnObj.SetActive(false);
    }
}
