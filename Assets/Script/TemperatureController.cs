using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �µ� ���� ���� Ŭ����
/// </summary>
[System.Serializable]
public class TemperatureController
{
    /// <summary>
    /// �� ���� ĭ ������ ���̾�
    /// </summary>
    public Dial dial;
    /// <summary>
    /// �µ� ���� �̴ϰ��� ���� Ŭ����
    /// </summary>
    public TempMiniGameManager tempMiniGameManager;
    /// <summary>
    /// ���� �̹���
    /// </summary>
    public Image trainImage;

    /// <summary>
    /// ���̾� ȸ�� �ӵ�
    /// </summary>
    private float dialSpeed;

    /// <summary>
    /// �ּ�, �ִ� �µ� ������ ���� �µ�
    /// </summary>
    [SerializeField]
    private float temperature = 30;
    /// <summary>
    /// �ּ�, �ִ� �µ� ������ ���� �µ�
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
    /// ��ǥ �µ�
    /// </summary>
    public float bestTemperature = 24;
    /// <summary>
    /// ��ǥ �µ��� �����ߴ��� ����
    /// </summary>
    public bool isCompleted = false;

    public delegate void CheckCompleted();
    private CheckCompleted checkCompleted;

    /// <summary>
    /// �µ� ���� �̴ϰ��� �ʱ�ȭ �۾�
    /// </summary>
    /// <param name="bestTemp">��ǥ �µ�</param>
    /// <param name="checkCompletedFunc">������ �Ϸ�Ǿ����� Ȯ���ϴ� �Լ�</param>
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
    /// ���̾� ȸ���� ���� �µ� �� ��ȭ ����
    /// </summary>
    /// <param name="angle">ȸ���� ���̾��� ����</param>
    public void SetTemperature(float angle)
    {
        if (bestTemperature - 0.25f > Temperature || Temperature > bestTemperature + 0.25f)
        {
            Temperature += -angle * dialSpeed;

            SetTempColor();
        }
        // Ư�� �µ� ���� ���� ������ Ŭ���� ó��
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
    /// �µ��� ���� ���� ���� ����
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
