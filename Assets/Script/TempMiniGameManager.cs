using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempMiniGameManager : MonoBehaviour
{
    /// <summary>
    /// Maximum temperature
    /// </summary>
    public float maxTemp = 35;
    /// <summary>
    /// Minimum temperature
    /// </summary>
    public float minTemp = 15;

    /// <summary>
    /// Train's color when temperature is high
    /// </summary>
    public Color hotColor;
    /// <summary>
    /// Train's color when temperature is low
    /// </summary>
    public Color coldColor;

    public Image timerImage;
    public float missionTime;
    private IEnumerator timer;

    public GameObject temperatureGameObj;
    public GameObject playConfirmObj;
    public TemperatureController[] temperatureControllers;

    private void Start()
    {
        InitTempControlGame();
    }

    IEnumerator Timer(float delay, float timeLeft)
    {
        yield return new WaitForSeconds(delay);

        timeLeft -= delay;
        timerImage.fillAmount = 1 - timeLeft / missionTime;

        if (timeLeft > 0)
            StartCoroutine(timer = Timer(delay, timeLeft));
        else
            TimeOver();
    }

    /// <summary>
    /// Initialize Temperature Control Mini Game and start
    /// </summary>
    private void InitTempControlGame()
    {
        for (int i = 0; i < temperatureControllers.Length; i++)
        {
            temperatureControllers[i].Init(Random.Range(21, 26), Random.Range(minTemp, maxTemp), Random.Range(0.005f, 0.02f), CheckCompleted);
        }
        missionTime = Random.Range(10f, 15f);

        playConfirmObj.SetActive(true);
        temperatureGameObj.SetActive(true);
    }

    public void StartGame()
    {
        playConfirmObj.SetActive(false);
        StartCoroutine(timer = Timer(0.1f, missionTime));
    }

    public void Decline()
    {
        temperatureGameObj.SetActive(false);
    }

    /// <summary>
    /// Check if all of controllers are completed
    /// </summary>
    private void CheckCompleted()
    {
        for(int i = 0; i < temperatureControllers.Length; i++)
        {
            if (!temperatureControllers[i].isCompleted)
                return;
        }
        TempControlCompleted();
    }

    /// <summary>
    /// Temperature Controlling has been completed
    /// </summary>
    private void TempControlCompleted()
    {
        if (timer != null)
            StopCoroutine(timer);
        Debug.Log("Complete!");
    }

    private void TimeOver()
    {
        Debug.Log("Time over!");
    }
}
