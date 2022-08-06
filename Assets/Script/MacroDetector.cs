using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MacroDetector : MonoBehaviour
{
    public GameObject warnObj;

    private DateTime priorTouchTime;
    private int[] intervalRecords = new int[100];
    private int recordedIndex = 0;

    private int tmtSeconds;
    private int mtSeconds;

    private void Start()
    {
        priorTouchTime = DateTime.Now;
    }

    public void DetectTouchAmount(int touchPerSecond)
    {
        DetectMuchTouch(touchPerSecond);
        DetectTooMuchTouch(touchPerSecond);
    }

    /// <summary>
    /// 1�ʿ� 50ȸ �̻��� 45�� �̻� ������
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
    /// 1�ʿ� 30ȸ �̻� 3�� �̻� ������
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

    public void CloseDetectedWarn()
    {
        warnObj.SetActive(false);
    }
}
