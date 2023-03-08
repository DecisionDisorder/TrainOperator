using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 피버 시스템 관리 클래스
/// </summary>
public class FeverManager : MonoBehaviour
{
    /// <summary>
    /// 피버가 활성화 중인지 여부
    /// </summary>
    public bool feverActived = false;

    /// <summary>
    /// 피버 스택 수치
    /// </summary>
    public int FeverStack {
        set
        {
            MyAsset.instance.myAssetData.feverStack = value;
            SetFeverStackImage(GetFeverRatio(), true);
            if (FeverStack >= targetFeverStack)
            {
                ActiveFeverMode();
            }
        }
        get { return MyAsset.instance.myAssetData.feverStack; } 
    }

    /// <summary>
    /// 목표 피버 스택 수치
    /// </summary>
    public int targetFeverStack = 500;
    /// <summary>
    /// 1초당 획득할 수 있는 최대 피버 스택 수치
    /// </summary>
    public int feverStackLimitPerSecond = 25;
    /// <summary>
    /// 초당 피버 스택 제한량을 계산을 위한 임시 변수
    /// </summary>
    private int feverStackLimitCache;

    /// <summary>
    /// 피버 지속시간
    /// </summary>
    public float duration = 10f;
    /// <summary>
    /// 선택된 피버 모드 종류
    /// </summary>
    private FeverMode selectedFeverMode;
    /// <summary>
    /// 피버 모드 열거형 (자동 터치, 수익량 증가)
    /// </summary>
    private enum FeverMode { AutoTouching, MassiveIncome }
    /// <summary>
    /// 피버 모드 떄의 자동 초당 터치 수
    /// </summary>
    public int autoTouchAmount = 20;
    /// <summary>
    /// 자동 터치 발생 확률
    /// </summary>
    public int possibilityOfAutoTouching = 30;
    /// <summary>
    /// 수익량 증대 효과 (배율)
    /// </summary>
    public int massiveIncomeAmount = 5;

    /// <summary>
    /// 적용된 터치형 수익 증가 수치
    /// </summary>
    private ulong activedTouchAbility;
    /// <summary>
    /// 적용된 시간형 수익 증가 수치
    /// </summary>
    public ulong activedTimeAbility;

    /// <summary>
    /// 피버 스택이 채워지는 상태의 색상
    /// </summary>
    public Color fillingColor;
    /// <summary>
    /// 피버 스택이 모두 채워진 상태의 색상
    /// </summary>
    public Color filledColor;
    /// <summary>
    /// 메시지 배경 색상
    /// </summary>
    public Color messageBackgroundColor;

    /// <summary>
    /// 피버 아이콘 이미지
    /// </summary>
    public Image feverImg;
    /// <summary>
    /// 피버 모드 발동 버튼
    /// </summary>
    public Button feverButton;

    /// <summary>
    /// 피버 시작 애니메이션
    /// </summary>
    public Animation feverStartAnimation;
    /// <summary>
    /// 피버 경계 이미지
    /// </summary>
    public Image feverOuterLineImg;
    /// <summary>
    /// 피버 경계 이미지 스프라이트 리소스 배열
    /// </summary>
    public Sprite[] feverOuterLineSprites;

    /// <summary>
    /// 현재 적용중인 스프라이트의 인덱스
    /// </summary>
    private int spriteIndex = 0;

    public TouchEarning touchEarning;
    public ItemManager itemManager;
    public MessageManager messageManager;

    private void Start()
    {
        SetFeverStackImage(GetFeverRatio(), true);
        StartCoroutine(StackLimitTimer());
    }

    /// <summary>
    /// 초당 스택 제한량 초기화 타이머
    /// </summary>
    /// <returns></returns>
    IEnumerator StackLimitTimer()
    {
        yield return new WaitForSeconds(1);

        feverStackLimitCache = 0;

        StartCoroutine(StackLimitTimer());
    }

    /// <summary>
    /// 피버 스택 추가
    /// </summary>
    public void AddFeverStack()
    {
        // 초당 제한량에 걸리지 않으면 피버 스택을 더한다
        feverStackLimitCache++;
        if(feverStackLimitCache < feverStackLimitPerSecond)
        {
            FeverStack++;
        }
    }

    /// <summary>
    /// 피버 비율 계산
    /// </summary>
    /// <returns>피버 스택을 모은 비율</returns>
    private float GetFeverRatio()
    {
        return (float)FeverStack / targetFeverStack;
    }

    /// <summary>
    /// 피버 스택 이미지 설정
    /// </summary>
    /// <param name="ratio">피버 스택 비율</param>
    /// <param name="isAdding">더해지고 있는지 여부</param>
    private void SetFeverStackImage(float ratio, bool isAdding)
    {
        if(isAdding)
        {
            if (ratio < 1)
                feverImg.color = fillingColor;
            else
                feverImg.color = filledColor;
        }
        feverImg.fillAmount = ratio;
    }

    /// <summary>
    /// 피버 모드 활성화
    /// </summary>
    private void ActiveFeverMode()
    {
        // 활성화 효과
        feverButton.image.raycastTarget = true;
        feverButton.enabled = true;
    }

    /// <summary>
    /// 피버 버튼 비활성화
    /// </summary>
    private void DisableFeverButton()
    {
        feverButton.image.raycastTarget = false;
        feverButton.enabled = false;
        feverButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// 피버 효과 비활성화
    /// </summary>
    private void DisableFeverMode()
    {
        feverActived = false;
        SetFeverStackImage(0, true);
        feverButton.gameObject.SetActive(true);
        feverStartAnimation.gameObject.SetActive(false);

        // 이펙트 비활성화
        if (selectedFeverMode == FeverMode.MassiveIncome)
        {
            // 수익율 원상복구
            itemManager.SetMassiveIncomeDisable(activedTouchAbility, activedTimeAbility);
            //itemManager.SetActiveColorCard(true);
        }
        else
        {
            //itemManager.SetActiveRareCard(true);
        }
    }

    /// <summary>
    /// 피버 모드 시작
    /// </summary>
    public void StartFeverMode()
    {
        int rand = Random.Range(0, 100);

        // 확률에 따라 자동 터치 혹은 수익량 증대 효과 시작
        if(rand < possibilityOfAutoTouching)
        {
            selectedFeverMode = FeverMode.AutoTouching;
            //itemManager.SetActiveRareCard(false);
            messageManager.ShowMessage("자동 수금 피버 타임이 발동하였습니다!", messageBackgroundColor, 3.0f);
            StartAutoTouchingFever();
        }
        else
        {
            selectedFeverMode = FeverMode.MassiveIncome;
            //itemManager.SetActiveColorCard(false);
            messageManager.ShowMessage("수익률 증가 피버 타임이 발동하였습니다!", messageBackgroundColor, 3.0f);
            StartMassiveIncomeFever();
        }
        FeverStack = 0;
        feverActived = true;
        DisableFeverButton();
        StartFeverStartEffect();
    }

    /// <summary>
    /// 자동 터치 피버 효과 발동
    /// </summary>
    private void StartAutoTouchingFever()
    {
        // 이펙트 활성화
        StartCoroutine(AutoTouching(autoTouchAmount, duration));
    }

    /// <summary>
    /// 수익량 증대 피버 효과 발동
    /// </summary>
    private void StartMassiveIncomeFever()
    {
        // 터치 및 시간형 수익 증가
        itemManager.SetMassiveIncomeEnable((int)duration, (ulong)massiveIncomeAmount, (ulong)massiveIncomeAmount, ref activedTouchAbility, ref activedTimeAbility);
        // 이펙트 활성화
        StartCoroutine(FeverModeTimer(duration));
    }

    /// <summary>
    /// 피버 모드 타이머
    /// </summary>
    /// <param name="timeLeft">남은 시간</param>
    /// <param name="interval">업데이트 간격</param>
    /// <returns></returns>
    IEnumerator FeverModeTimer(float timeLeft, float interval = 0.2f)
    {
        yield return new WaitForSeconds(interval);

        timeLeft -= interval;

        // 남은 피버 시간 이미지 비율 반영
        SetFeverStackImage(timeLeft / duration, false);

        if(timeLeft > 0)
            StartCoroutine(FeverModeTimer(timeLeft));
        else
            DisableFeverMode();
    }

    /// <summary>
    /// 오토터치 효과 코루틴
    /// </summary>
    /// <param name="touchAmountPerSecond">초당 터치 횟수</param>
    /// <param name="timeLeft">남은 시간</param>
    IEnumerator AutoTouching(float touchAmountPerSecond, float timeLeft)
    {
        float delay = 1f / touchAmountPerSecond;
        timeLeft -= delay;
        SetFeverStackImage(timeLeft / duration, false);

        yield return new WaitForSeconds(delay);

        touchEarning.TouchIncome();

        if (timeLeft > 0)
            StartCoroutine(AutoTouching(touchAmountPerSecond, timeLeft));
        else
            DisableFeverMode();
    }

    /// <summary>
    /// 피버 효과 애니메이션 효과 시작
    /// </summary>
    private void StartFeverStartEffect()
    {
        feverStartAnimation.gameObject.SetActive(true);
        feverStartAnimation.Play();
        PlayManager.instance.WaitAnimation(feverStartAnimation, StartFeverSpriteChange);
    }

    /// <summary>
    /// 피버 스프라이트 이미지 교체 효과 시작
    /// </summary>
    private void StartFeverSpriteChange()
    {
        StartCoroutine(FeverSpriteChange(0.4f));
    }

    /// <summary>
    /// 피버 스프라이트 이미지 교체 효과 코루틴
    /// </summary>
    /// <param name="interval">교체 간격</param>
    IEnumerator FeverSpriteChange(float interval)
    {
        yield return new WaitForSeconds(interval);

        feverOuterLineImg.sprite = feverOuterLineSprites[(spriteIndex++) % 2];

        if (feverActived)
            StartCoroutine(FeverSpriteChange(interval));
    }
}
