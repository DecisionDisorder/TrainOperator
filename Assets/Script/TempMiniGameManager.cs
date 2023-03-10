using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 열차 온도 조절 미니게임 관리 클래스
/// </summary>
public class TempMiniGameManager : MonoBehaviour
{
    /// <summary>
    /// 보정 배수에 클리어 시간을 곱한 값
    /// </summary>
    public float[] rewardFactors;
    /// <summary>
    /// 선택된 미니게임의 난이도 (0~2: 쉬움/보통/어려움)
    /// </summary>
    public int difficulty;

    /// <summary>
    /// 최대 온도
    /// </summary>
    public float maxTemp = 35;
    /// <summary>
    /// 최소 온도
    /// </summary>
    public float minTemp = 15;

    /// <summary>
    /// 온도가 높을 때의 열차 색상
    /// </summary>
    public Color hotColor;
    /// <summary>
    /// 온도가 낮을 때의 열차 색상
    /// </summary>
    public Color coldColor;
    /// <summary>
    /// 조절이 완료되었을 때의 열차 색상
    /// </summary>
    public Color completeColor;

    /// <summary>
    /// 플로팅 메시지 효과
    /// </summary>
    public Animation[] floatingMessages;
    /// <summary>
    /// 성공 플로팅 이모티콘 효과
    /// </summary>
    public Animation[] floatingSuccessEmoticon;
    /// <summary>
    /// 실패 플로팅 이모티콘 효과
    /// </summary>
    public Animation[] floatingFailEmoticon;

    /// <summary>
    /// 남은 시간 타이머 이미지
    /// </summary>
    public Image timerImage;
    /// <summary>
    /// 미션 제한 시간
    /// </summary>
    public float missionTime;
    /// <summary>
    /// 제한 시간 최소치
    /// </summary>
    public float missionTimeMin;
    /// <summary>
    /// 제한 시간 최대치
    /// </summary>
    public float missionTimeMax;
    /// <summary>
    /// 남은시간 차감 코루틴
    /// </summary>
    private IEnumerator timer;

    /// <summary>
    /// 온도 미니게임 그룹 오브젝트
    /// </summary>
    public GameObject temperatureGameObj;
    /// <summary>
    /// 플레이 확인 오브젝트
    /// </summary>
    public GameObject playConfirmObj;
    /// <summary>
    /// 보상 화면 오브젝트
    /// </summary>
    public GameObject rewardObj;
    /// <summary>
    /// 보상 정보 텍스트
    /// </summary>
    public Text rewardText;
    /// <summary>
    /// 미션 실패 화면 오브젝트
    /// </summary>
    public GameObject failObj;
    /// <summary>
    /// 미니게임 난이도 드롭다운
    /// </summary>
    public Dropdown difficultyDropdown;

    /// <summary>
    /// 클릭 오디오 효과음
    /// </summary>
    public AudioSource clickAudio;
    /// <summary>
    /// 성공 오디오 효과음
    /// </summary>
    public AudioSource successAudio;
    /// <summary>
    /// 실패 오디오 효과음
    /// </summary>
    public AudioSource failedAudio;

    /// <summary>
    /// 난이도에 따른 열차 그룹 오브젝트
    /// </summary>
    public GameObject[] trainDifficulty;
    /// <summary>
    /// 쉬움 난이도 온도 컨트롤러
    /// </summary>
    public TemperatureController[] easyTempControllers;
    /// <summary>
    /// 보통 난이도 온도 컨트롤러
    /// </summary>
    public TemperatureController[] normalTempControllers;
    /// <summary>
    /// 어려움 난이도 온도 컨트롤러
    /// </summary>
    public TemperatureController[] hardTempControllers;


    /// <summary>
    /// 미니게임을 위한 타이머 코루틴
    /// </summary>
    /// <param name="delay">타이머 딜레이</param>
    /// <param name="timeLeft">타임아웃 하기까지 남은 시간</param>
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
    /// 시간차를 두고 플로팅 이펙트 효과 시작 해주는 코루틴
    /// </summary>
    /// <param name="animations">플로팅 애니메이션</param>
    /// <param name="index">현재 애니메이션 인덱스 (dafault=0)</param>
    IEnumerator Floating(Animation[] animations, int index = 0)
    {
        yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));

        animations[index].Play();

        if (index + 1 < animations.Length)
            StartCoroutine(Floating(animations, index + 1));
    }

    /// <summary>
    /// 온도 조절 미니게임을 초기화하고 시작
    /// </summary>
    public void InitTempControlGame()
    {
        StartCoroutine(Floating(floatingMessages));
        playConfirmObj.SetActive(true);
        temperatureGameObj.SetActive(true);
    }

    /// <summary>
    /// 시작 버튼을 눌러서 미니게임 시작 처리
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
    /// 온도 조절 미니게임 취소
    /// </summary>
    public void Decline()
    {
        temperatureGameObj.SetActive(false);
    }

    /// <summary>
    /// 모든 컨트롤러가 완료되었는지 확인
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
    /// 모든 온도 조절 컨트롤이 완료되어 클리어 처리
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

        rewardText.text = "성과금: " + money2 + money1 + "$";
        StartCoroutine(SuccessWait(0.75f));
    }
    
    /// <summary>
    /// 성공 처리 대기 코루틴
    /// </summary>
    /// <param name="delay">대기 시간</param>
    IEnumerator SuccessWait(float delay)
    {
        yield return new WaitForSeconds(delay);

        successAudio.Play();
        rewardObj.SetActive(true);
    }

    /// <summary>
    /// 타임아웃으로 게임 오버 화면 활성화
    /// </summary>
    private void TimeOver()
    {
        StartCoroutine(Floating(floatingFailEmoticon));
        failedAudio.Play();
        failObj.SetActive(true);
    }

    /// <summary>
    /// 미니게임 UI 비활성화
    /// </summary>
    public void DisableGame()
    {
        trainDifficulty[difficulty].SetActive(false);
        rewardObj.SetActive(false);
        failObj.SetActive(false);
        temperatureGameObj.SetActive(false);
    }
}
