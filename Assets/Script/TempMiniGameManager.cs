using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempMiniGameManager : MonoBehaviour
{
    /// <summary>
    /// Compensation multiples multiplied by clear time
    /// </summary>
    public float[] rewardFactors;
    /// <summary>
    /// Difficulty of mini game (0~2: Easy/Normal/Hard)
    /// </summary>
    public int difficulty;

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
    /// <summary>
    /// Train's color when controlling is done
    /// </summary>
    public Color completeColor;

    public Animation[] floatingMessages;
    public Animation[] floatingSuccessEmoticon;
    public Animation[] floatingFailEmoticon;

    public Image timerImage;
    public float missionTime;
    public float missionTimeMin;
    public float missionTimeMax;
    private IEnumerator timer;

    public GameObject temperatureGameObj;
    public GameObject playConfirmObj;
    public GameObject rewardObj;
    public Text rewardText;
    public GameObject failObj;
    public Dropdown difficultyDropdown;

    public AudioSource clickAudio;
    public AudioSource successAudio;
    public AudioSource failedAudio;

    public GameObject[] trainDifficulty;
    public TemperatureController[] easyTempControllers;
    public TemperatureController[] normalTempControllers;
    public TemperatureController[] hardTempControllers;


    /// <summary>
    /// Timer for timeout for mini-game
    /// </summary>
    /// <param name="delay">Timer delay</param>
    /// <param name="timeLeft">Time remaining before timeout</param>
    /// <returns></returns>
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
    /// Start plotting effect with time difference
    /// </summary>
    /// <param name="animations">Floating animations</param>
    /// <param name="index">current animation index (dafault=0)</param>
    /// <returns></returns>
    IEnumerator Floating(Animation[] animations, int index = 0)
    {
        yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));

        animations[index].Play();

        if (index + 1 < animations.Length)
            StartCoroutine(Floating(animations, index + 1));
    }

    /// <summary>
    /// Initialize Temperature Control Mini Game and start
    /// </summary>
    public void InitTempControlGame()
    {
        StartCoroutine(Floating(floatingMessages));
        playConfirmObj.SetActive(true);
        temperatureGameObj.SetActive(true);
    }

    /// <summary>
    /// Temperature Control mini game start (Start button pressed)
    /// </summary>
    public void StartGame()
    {
        difficulty = difficultyDropdown.value;
        trainDifficulty[difficulty].SetActive(true);
        missionTime = Random.Range(missionTimeMin, missionTimeMax);
        if(difficulty.Equals(0))
        {
            for (int i = 0; i < easyTempControllers.Length; i++)
                easyTempControllers[i].Init(Random.Range(21, 26), Random.Range(minTemp, maxTemp), Random.Range(0.01f, 0.03f), CheckCompleted);
        }
        else if (difficulty.Equals(1))
        {
            for (int i = 0; i < normalTempControllers.Length; i++)
                normalTempControllers[i].Init(Random.Range(21, 26), Random.Range(minTemp, maxTemp), Random.Range(0.005f, 0.015f), CheckCompleted);
        }
        else
        {
            for (int i = 0; i < hardTempControllers.Length; i++)
                hardTempControllers[i].Init(Random.Range(21, 26), Random.Range(minTemp, maxTemp), Random.Range(0.003f, 0.01f), CheckCompleted);
        }

        playConfirmObj.SetActive(false);
        StartCoroutine(timer = Timer(0.1f, missionTime));
    }

    /// <summary>
    /// Decline Temp Control mini game
    /// </summary>
    public void Decline()
    {
        temperatureGameObj.SetActive(false);
    }

    /// <summary>
    /// Check if all of controllers are completed
    /// </summary>
    private void CheckCompleted()
    {
        if (difficulty.Equals(0))
        {
            for (int i = 0; i < easyTempControllers.Length; i++)
            {
                if (!easyTempControllers[i].isCompleted)
                    return;
            }
        }
        else if (difficulty.Equals(1))
        {
            for (int i = 0; i < normalTempControllers.Length; i++)
            {
                if (!normalTempControllers[i].isCompleted)
                    return;
            }
        }
        else
        {
            for (int i = 0; i < hardTempControllers.Length; i++)
            {
                if (!hardTempControllers[i].isCompleted)
                    return;
            }
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
        StartCoroutine(Floating(floatingSuccessEmoticon));

        ulong addedMoneyLow = 0, addedMoneyHigh = 0;
        string money1 = "", money2 = "";
        PlayManager.SetMoneyReward(ref addedMoneyLow, ref addedMoneyHigh, (int)(missionTimeMin * rewardFactors[difficulty]));
        AssetMoneyCalculator.instance.ArithmeticOperation(addedMoneyLow, addedMoneyHigh, true);
        PlayManager.ArrangeUnit(addedMoneyLow, addedMoneyHigh, ref money1, ref money2);

        rewardText.text = "¼º°ú±Ý: " + money2 + money1 + "$";
        StartCoroutine(SuccessWait(0.75f));
    }
    
    IEnumerator SuccessWait(float delay)
    {
        yield return new WaitForSeconds(delay);

        successAudio.Play();
        rewardObj.SetActive(true);
    }

    /// <summary>
    /// Active Fail UI after timeout
    /// </summary>
    private void TimeOver()
    {
        StartCoroutine(Floating(floatingFailEmoticon));
        failedAudio.Play();
        failObj.SetActive(true);
    }

    /// <summary>
    /// Close temp control mini game UI
    /// </summary>
    public void DisableGame()
    {
        trainDifficulty[difficulty].SetActive(false);
        rewardObj.SetActive(false);
        failObj.SetActive(false);
        temperatureGameObj.SetActive(false);
    }
}
