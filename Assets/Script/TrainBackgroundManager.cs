using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 배경 열차 진출입 애니메이션 효과 관리 클래스
/// </summary>
public class TrainBackgroundManager : MonoBehaviour
{
    /// <summary>
    /// 열차 이동 애니메이션
    /// </summary>
    public Animation trainAnimation;
    /// <summary>
    /// 열차 출입문 개폐 애니메이션
    /// </summary>
    public Animation[] trainDoorAnimations;
    /// <summary>
    /// 일반 승강장에서의 개폐 애니메이션 클립 배열
    /// </summary>
    public AnimationClip[] normalDoorClips;
    /// <summary>
    /// 스크린도어 승강장에서의 개폐 애니메이션 클립 배열
    /// </summary>
    public AnimationClip[] screendoorClips;
    /// <summary>
    /// 열차 이동 애니메이션 클립 배열
    /// </summary>
    public AnimationClip[] trainClips;

    /// <summary>
    /// 애니메이션용 출입문 그룹 오브젝트
    /// </summary>
    public GameObject doors;

    /// <summary>
    /// 일반 승강장에서의 열차 위치 Y
    /// </summary>
    public float normalPlatformTrainY;
    /// <summary>
    /// 스크린도어 승강장에서의 열차 위치 Y
    /// </summary>
    public float screendoorTrainY;

    public BackgroundImageManager backgroundImageManager;
    public TouchMoneyImageControl touchMoneyImageControl;

    /// <summary>
    /// 열차에 타고 있는 많은 사람들을 표현하는 오브젝트
    /// </summary>
    public GameObject massPeople;
    /// <summary>
    /// 열차에 타고 있는 승객 이미지 배열
    /// </summary>
    public Image[] peopleInTrainImgs;
    /// <summary>
    /// 승객 많음 상태의 이미지 스프라이트 리소스 배열
    /// </summary>
    public Sprite[] massSprites;
    /// <summary>
    /// 출입문이 열려있는지 여부
    /// </summary>
    private bool isDoorOpened;

    /// <summary>
    /// 배경 테스트 모드 여부
    /// </summary>
    public bool isTestMode = false;


    private void Start()
    {
        StartWaitTrainEntrance();
    }

    /// <summary>
    /// 열차 진입 애니메이션 대기 시작
    /// </summary>
    private void StartWaitTrainEntrance()
    {
        if(!isTestMode)
            StartCoroutine(TrainEntranceTimer(Random.Range(30f, 50f)));
        else
            StartCoroutine(TrainEntranceTimer(Random.Range(3f, 5f)));
    }

    /// <summary>
    /// 일반/스크린도어 승강장에 따라 배경 설정 적용
    /// </summary>
    public void SetPlatformSettings()
    {
        if (backgroundImageManager.backgroundType.Equals(0))
        {
            trainDoorAnimations[1].gameObject.SetActive(false);
            doors.transform.localPosition = new Vector3(doors.transform.localPosition.x, normalPlatformTrainY);
            trainAnimation.transform.localPosition = new Vector3(trainAnimation.transform.localPosition.x, normalPlatformTrainY);
        }
        else
        {
            trainDoorAnimations[1].gameObject.SetActive(true);
            SyncScreendoorAnimation();
            doors.transform.localPosition = new Vector3(doors.transform.localPosition.x, screendoorTrainY);
            trainAnimation.transform.localPosition = new Vector3(trainAnimation.transform.localPosition.x, screendoorTrainY);
        }
    }

    /// <summary>
    /// 재생 중인 출입문 개페 애니메이션 인덱스를 구한다
    /// </summary>
    private int GetPlayingDoorIndex()
    {
        if (trainDoorAnimations[0].clip.Equals(normalDoorClips[0]))
            return 0;
        else
            return 1;
    }

    /// <summary>
    /// 스크린도어 애니메이션 타이밍 동기화
    /// </summary>
    private void SyncScreendoorAnimation()
    {
        if (trainDoorAnimations[0].isPlaying)
        {
            int playingIndex = GetPlayingDoorIndex();
            trainDoorAnimations[1].clip = screendoorClips[playingIndex];
            trainDoorAnimations[1].Play();
            trainDoorAnimations[1][screendoorClips[playingIndex].name].time = trainDoorAnimations[0][normalDoorClips[playingIndex].name].time;
        }
    }

    /// <summary>
    /// 열차 진입 애니메이션 대기 코루틴
    /// </summary>
    /// <param name="delay">대기 시간</param>
    IEnumerator TrainEntranceTimer(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 열차 진입 준비 세팅
        SetPlatformSettings();

        // 열차 진입 애니메이션 시작
        trainAnimation.gameObject.SetActive(true);
        trainAnimation.clip = trainClips[0];
        trainAnimation.Play();
        // 진입 후 출입문 개페 애니메이션 대기
        PlayManager.instance.WaitAnimation(trainAnimation, StartWaitDoorOpen);
    }

    /// <summary>
    /// 출입문 개페 애니메이션 대기
    /// </summary>
    private void StartWaitDoorOpen()
    {
        StartCoroutine(DoorOpen(Random.Range(1f,3f)));
    }

    /// <summary>
    /// 일정 시간 대기 후 출입문 개페 애니메이션 시작해주는 코루틴
    /// </summary>
    /// <param name="delay">개폐 대기 시간</param>
    IEnumerator DoorOpen(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 출입문 열림 적용
        doors.SetActive(true);
        isDoorOpened = true;

        // 스크린도어일 때 스크린도어 애니메이션도 시작
        if(backgroundImageManager.backgroundType.Equals(1))
        {
            trainDoorAnimations[1].clip = screendoorClips[0];
            trainDoorAnimations[1].Play();
        }

        // 승객 수가 많을 때의 효과 시작
        if(TouchEarning.passengerRandomFactor >= 100)
        {
            massPeople.SetActive(true);
            StartCoroutine(MassPeopleEffect(0.7f));
        }

        // 출입문 열기 애니메이션 효과 시작
        trainDoorAnimations[0].clip = normalDoorClips[0];
        trainDoorAnimations[0].Play();
        // 출입문 닫기 애니메이션 대기
        PlayManager.instance.WaitAnimation(trainDoorAnimations[0], StartWaitDoorClose);
    }

    /// <summary>
    /// 출입문 닫기 애니메이션 효과 대기
    /// </summary>
    private void StartWaitDoorClose()
    {
        StartCoroutine(DoorClose(Random.Range(4f,8f)));
    }

    /// <summary>
    /// 출입문 닫기 대기 후 재생해주는 코루틴
    /// </summary>
    /// <param name="delay">대기 시간</param>
    IEnumerator DoorClose(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 스크린도어 일 때 클립 적용
        if (backgroundImageManager.backgroundType.Equals(1))
        {
            trainDoorAnimations[1].clip = screendoorClips[1];
            trainDoorAnimations[1].Play();
        }
        // 닫기 애니에미션 시작
        trainDoorAnimations[0].clip = normalDoorClips[1];
        trainDoorAnimations[0].Play();
        // 열차 출발 애니메이션 대기
        PlayManager.instance.WaitAnimation(trainDoorAnimations[0], StartWaitTrainExit);
    }

    /// <summary>
    /// 열차 출발 애니메이션 대기
    /// </summary>
    private void StartWaitTrainExit()
    {
        isDoorOpened = false;
        doors.SetActive(false);
        StartCoroutine(TrainExitTimer(Random.Range(1f,3f)));
    }

    /// <summary>
    /// 일정 시간 대기 후 열차 출발 애니메이션을 시작하는 코루틴
    /// </summary>
    /// <param name="delay">대기 시간</param>
    IEnumerator TrainExitTimer(float delay)
    {
        yield return new WaitForSeconds(delay);

        trainAnimation.clip = trainClips[1];
        trainAnimation.Play();
        PlayManager.instance.WaitAnimation(trainAnimation, DisableTrain);
    }

    /// <summary>
    /// 열차 출발 이후에 관련 오브젝트 그룹 비활성화 및 다음 열차 애니메이션 효과 대기 시작
    /// </summary>
    private void DisableTrain()
    {
        trainAnimation.gameObject.SetActive(false);
        StartWaitTrainEntrance();
    }

    /// <summary>
    /// 사람이 많을 때의 이미지 전환 효과 코루틴
    /// </summary>
    /// <param name="delay">전환 대기 시간</param>
    IEnumerator MassPeopleEffect(float delay)
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < peopleInTrainImgs.Length; i++)
            peopleInTrainImgs[i].sprite = massSprites[touchMoneyImageControl.currentIndex];

        if(isDoorOpened)
            StartCoroutine(MassPeopleEffect(delay));
        else
            massPeople.SetActive(false);
    }
}
