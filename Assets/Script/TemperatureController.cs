using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TemperatureController
{
    /// <summary>
    /// Dial for each train
    /// </summary>
    public Dial dial;
    /// <summary>
    /// Mini Game Manager
    /// </summary>
    public TempMiniGameManager tempMiniGameManager;
    /// <summary>
    /// Train Image
    /// </summary>
    public Image trainImage;

    /// <summary>
    /// Dial rotation speed
    /// </summary>
    private float dialSpeed;

    [SerializeField]
    private float temperature = 30;
    /// <summary>
    /// Current temperature between minTemp and maxTemp
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
    /// Target temperature
    /// </summary>
    public float bestTemperature = 24;
    /// <summary>
    /// Ensure target temperature is reached
    /// </summary>
    public bool isCompleted = false;

    public delegate void CheckCompleted();
    private CheckCompleted checkCompleted;

    /// <summary>
    /// Initialize Mini Game
    /// </summary>
    /// <param name="bestTemp">best temperature</param>
    /// <param name="checkCompletedFunc">Check Completed Function</param>
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
    /// Modify temperature according to angle
    /// </summary>
    /// <param name="angle">Dial's rotated angle</param>
    public void SetTemperature(float angle)
    {
        if (bestTemperature - 0.25f > Temperature || Temperature > bestTemperature + 0.25f)
        {
            Temperature += -angle * dialSpeed;

            SetTempColor();
        }
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
    /// Set train color according to temperature
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
