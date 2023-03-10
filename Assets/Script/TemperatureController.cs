using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 온도 관리 제어 클래스
/// </summary>
[System.Serializable]
public class TemperatureController
{
    /// <summary>
    /// 각 열차 칸 마다의 다이얼
    /// </summary>
    public Dial dial;
    /// <summary>
    /// 온도 관리 미니게임 관리 클래스
    /// </summary>
    public TempMiniGameManager tempMiniGameManager;
    /// <summary>
    /// 열차 이미지
    /// </summary>
    public Image trainImage;

    /// <summary>
    /// 다이얼 회전 속도
    /// </summary>
    private float dialSpeed;

    /// <summary>
    /// 최소, 최대 온도 사이의 현재 온도
    /// </summary>
    [SerializeField]
    private float temperature = 30;
    /// <summary>
    /// 최소, 최대 온도 사이의 현재 온도
    /// </summary>
    public float Temperature
    {
        get { return temperature; }
        set
        {
            if (value > tempMiniGameManager.maxTemp)
                temperature = tempMiniGameManager.maxTemp;
            else if (value < tempMiniGameManager.minTemp)
                temperature = tempMiniGameManager.minTemp;
            else
                temperature = value;
        }
    }

    /// <summary>
    /// 목표 온도
    /// </summary>
    public float bestTemperature = 24;
    /// <summary>
    /// 목표 온도에 도달했는지 여부
    /// </summary>
    public bool isCompleted = false;

    public delegate void CheckCompleted();
    private CheckCompleted checkCompleted;

    /// <summary>
    /// 온도 조절 미니게임 초기화 작업
    /// </summary>
    /// <param name="bestTemp">목표 온도</param>
    /// <param name="checkCompletedFunc">조절이 완료되었는지 확인하는 함수</param>
    public void Init(float bestTemp, float temp, float dialSpeed, CheckCompleted checkCompletedFunc)
    {
        dial.Rotate(Random.Range(0, 360));
        dial.onAngleChanged = SetTemperature;
        dial.interactable = true;
        isCompleted = false;
        bestTemperature = bestTemp;
        Temperature = temp;
        this.dialSpeed = dialSpeed;
        checkCompleted = checkCompletedFunc;
        SetTempColor();
    }

    /// <summary>
    /// 다이얼 회전에 따라 온도 값 변화 적용
    /// </summary>
    /// <param name="angle">회전한 다이얼의 각도</param>
    public void SetTemperature(float angle)
    {
        if (bestTemperature - 0.25f > Temperature || Temperature > bestTemperature + 0.25f)
        {
            Temperature += -angle * dialSpeed;

            SetTempColor();
        }
        // 특정 온도 범위 내에 들어오면 클리어 처리
        else
        {
            dial.interactable = false;
            isCompleted = true;
            trainImage.color = tempMiniGameManager.completeColor;
            tempMiniGameManager.clickAudio.Play();
            checkCompleted();
        }
    }

    /// <summary>
    /// 온도에 따라서 열차 색상 변경
    /// </summary>
    public void SetTempColor()
    {
        if (Temperature < bestTemperature)
        {
            var color = tempMiniGameManager.coldColor;
            color.a = 1 - (Temperature - tempMiniGameManager.minTemp) / (bestTemperature - tempMiniGameManager.minTemp);
            trainImage.color = color;
        }
        else
        {
            var color = tempMiniGameManager.hotColor;
            color.a = 1 - (tempMiniGameManager.maxTemp - Temperature) / (tempMiniGameManager.maxTemp - bestTemperature);
            trainImage.color = color;
        }
    }
}
