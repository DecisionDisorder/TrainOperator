using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 매크로 감지 시스템 관리 클래스
/// </summary>
public class MacroDetector : MonoBehaviour
{
    /// <summary>
    /// 매크로 사용 경고 화면 오브젝트
    /// </summary>
    public GameObject warnObj;

    /// <summary>
    /// 직전 터치 시간
    /// </summary>
    private DateTime priorTouchTime;
    /// <summary>
    /// 최근 100회 동안의 터치 간격 기록
    /// </summary>
    private int[] intervalRecords = new int[100];
    /// <summary>
    /// 현재 기록 인덱스
    /// </summary>
    private int recordedIndex = 0;

    /// <summary>
    /// TooMuchTouch(일정시간 매우 힘든 터치 수 반복) 감지 중인 시간
    /// </summary>
    private int tmtSeconds;
    /// <summary>
    /// MuchTouch(긴시간 힘든 터치 수 반복) 감지 중인 시간
    /// </summary>
    private int mtSeconds;

    private void Start()
    {
        priorTouchTime = DateTime.Now;
    }

    /// <summary>
    /// 터치 횟수 검사
    /// </summary>
    /// <param name="touchPerSecond">초당 터치 횟수</param>
    public void DetectTouchAmount(int touchPerSecond)
    {
        DetectMuchTouch(touchPerSecond);
        DetectTooMuchTouch(touchPerSecond);
    }

    /// <summary>
    /// 1초에 100회 이상을 60초 이상 유지시 매크로 감지 처리
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
    /// 1초에 40회 이상 3분 이상 유지시 매크로 감지 처리
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
    /// 100회 연속 이상 일정한 시간 간격으로 터치하는지 확인
    /// </summary>
    public void DetectInterval()
    {
        // ms 단위로 동일한 간격으로 여러번 터치 하는지 확인 후 경고창 활성화
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
    /// 경고 메뉴 비활성화
    /// </summary>
    public void CloseDetectedWarn()
    {
        warnObj.SetActive(false);
    }
}
