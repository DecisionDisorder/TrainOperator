using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� �µ� ���� �̴ϰ��� ���� Ŭ����
/// </summary>
public class TempMiniGameManager : MonoBehaviour
{
    /// <summary>
    /// ���� ����� Ŭ���� �ð��� ���� ��
    /// </summary>
    public float[] rewardFactors;
    /// <summary>
    /// ���õ� �̴ϰ����� ���̵� (0~2: ����/����/�����)
    /// </summary>
    public int difficulty;

    /// <summary>
    /// �ִ� �µ�
    /// </summary>
    public float maxTemp = 35;
    /// <summary>
    /// �ּ� �µ�
    /// </summary>
    public float minTemp = 15;

    /// <summary>
    /// �µ��� ���� ���� ���� ����
    /// </summary>
    public Color hotColor;
    /// <summary>
    /// �µ��� ���� ���� ���� ����
    /// </summary>
    public Color coldColor;
    /// <summary>
    /// ������ �Ϸ�Ǿ��� ���� ���� ����
    /// </summary>
    public Color completeColor;

    /// <summary>
    /// �÷��� �޽��� ȿ��
    /// </summary>
    public Animation[] floatingMessages;
    /// <summary>
    /// ���� �÷��� �̸�Ƽ�� ȿ��
    /// </summary>
    public Animation[] floatingSuccessEmoticon;
    /// <summary>
    /// ���� �÷��� �̸�Ƽ�� ȿ��
    /// </summary>
    public Animation[] floatingFailEmoticon;

    /// <summary>
    /// ���� �ð� Ÿ�̸� �̹���
    /// </summary>
    public Image timerImage;
    /// <summary>
    /// �̼� ���� �ð�
    /// </summary>
    public float missionTime;
    /// <summary>
    /// ���� �ð� �ּ�ġ
    /// </summary>
    public float missionTimeMin;
    /// <summary>
    /// ���� �ð� �ִ�ġ
    /// </summary>
    public float missionTimeMax;
    /// <summary>
    /// �����ð� ���� �ڷ�ƾ
    /// </summary>
    private IEnumerator timer;

    /// <summary>
    /// �µ� �̴ϰ��� �׷� ������Ʈ
    /// </summary>
    public GameObject temperatureGameObj;
    /// <summary>
    /// �÷��� Ȯ�� ������Ʈ
    /// </summary>
    public GameObject playConfirmObj;
    /// <summary>
    /// ���� ȭ�� ������Ʈ
    /// </summary>
    public GameObject rewardObj;
    /// <summary>
    /// ���� ���� �ؽ�Ʈ
    /// </summary>
    public Text rewardText;
    /// <summary>
    /// �̼� ���� ȭ�� ������Ʈ
    /// </summary>
    public GameObject failObj;
    /// <summary>
    /// �̴ϰ��� ���̵� ��Ӵٿ�
    /// </summary>
    public Dropdown difficultyDropdown;

    /// <summary>
    /// Ŭ�� ����� ȿ����
    /// </summary>
    public AudioSource clickAudio;
    /// <summary>
    /// ���� ����� ȿ����
    /// </summary>
    public AudioSource successAudio;
    /// <summary>
    /// ���� ����� ȿ����
    /// </summary>
    public AudioSource failedAudio;

    /// <summary>
    /// ���̵��� ���� ���� �׷� ������Ʈ
    /// </summary>
    public GameObject[] trainDifficulty;
    /// <summary>
    /// ���� ���̵� �µ� ��Ʈ�ѷ�
    /// </summary>
    public TemperatureController[] easyTempControllers;
    /// <summary>
    /// ���� ���̵� �µ� ��Ʈ�ѷ�
    /// </summary>
    public TemperatureController[] normalTempControllers;
    /// <summary>
    /// ����� ���̵� �µ� ��Ʈ�ѷ�
    /// </summary>
    public TemperatureController[] hardTempControllers;


    /// <summary>
    /// �̴ϰ����� ���� Ÿ�̸� �ڷ�ƾ
    /// </summary>
    /// <param name="delay">Ÿ�̸� ������</param>
    /// <param name="timeLeft">Ÿ�Ӿƿ� �ϱ���� ���� �ð�</param>
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
    /// �ð����� �ΰ� �÷��� ����Ʈ ȿ�� ���� ���ִ� �ڷ�ƾ
    /// </summary>
    /// <param name="animations">�÷��� �ִϸ��̼�</param>
    /// <param name="index">���� �ִϸ��̼� �ε��� (dafault=0)</param>
    IEnumerator Floating(Animation[] animations, int index = 0)
    {
        yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));

        animations[index].Play();

        if (index + 1 < animations.Length)
            StartCoroutine(Floating(animations, index + 1));
    }

    /// <summary>
    /// �µ� ���� �̴ϰ����� �ʱ�ȭ�ϰ� ����
    /// </summary>
    public void InitTempControlGame()
    {
        StartCoroutine(Floating(floatingMessages));
        playConfirmObj.SetActive(true);
        temperatureGameObj.SetActive(true);
    }

    /// <summary>
    /// ���� ��ư�� ������ �̴ϰ��� ���� ó��
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
    /// �µ� ���� �̴ϰ��� ���
    /// </summary>
    public void Decline()
    {
        temperatureGameObj.SetActive(false);
    }

    /// <summary>
    /// ��� ��Ʈ�ѷ��� �Ϸ�Ǿ����� Ȯ��
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
    /// ��� �µ� ���� ��Ʈ���� �Ϸ�Ǿ� Ŭ���� ó��
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

        rewardText.text = "������: " + money2 + money1 + "$";
        StartCoroutine(SuccessWait(0.75f));
    }
    
    /// <summary>
    /// ���� ó�� ��� �ڷ�ƾ
    /// </summary>
    /// <param name="delay">��� �ð�</param>
    IEnumerator SuccessWait(float delay)
    {
        yield return new WaitForSeconds(delay);

        successAudio.Play();
        rewardObj.SetActive(true);
    }

    /// <summary>
    /// Ÿ�Ӿƿ����� ���� ���� ȭ�� Ȱ��ȭ
    /// </summary>
    private void TimeOver()
    {
        StartCoroutine(Floating(floatingFailEmoticon));
        failedAudio.Play();
        failObj.SetActive(true);
    }

    /// <summary>
    /// �̴ϰ��� UI ��Ȱ��ȭ
    /// </summary>
    public void DisableGame()
    {
        trainDifficulty[difficulty].SetActive(false);
        rewardObj.SetActive(false);
        failObj.SetActive(false);
        temperatureGameObj.SetActive(false);
    }
}
