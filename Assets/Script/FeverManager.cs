using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeverManager : MonoBehaviour
{
    public bool feverActived = false;

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
    public int targetFeverStack = 500;
    public int feverStackLimitPerSecond = 25;
    private int feverStackLimitCache;

    public float duration = 10f;
    private FeverMode selectedFeverMode;
    private enum FeverMode { AutoTouching, MassiveIncome }
    public int autoTouchAmount = 20;
    public int possibilityOfAutoTouching = 30;
    public int massiveIncomeAmount = 5;

    private ulong activedTouchAbility;
    public ulong activedTimeAbility;

    public Color fillingColor;
    public Color filledColor;
    public Color messageBackgroundColor;

    public Image feverImg;
    public Button feverButton;

    public Animation feverStartAnimation;
    public Image feverOuterLineImg;
    public Sprite[] feverOuterLineSprites;

    private int spriteIndex = 0;

    public TouchEarning touchEarning;
    public ItemManager itemManager;
    public MessageManager messageManager;

    private void Start()
    {
        SetFeverStackImage(GetFeverRatio(), true);
        StartCoroutine(StackLimitTimer());
    }

    IEnumerator StackLimitTimer()
    {
        yield return new WaitForSeconds(1);

        feverStackLimitCache = 0;

        StartCoroutine(StackLimitTimer());
    }

    public void AddFeverStack()
    {
        feverStackLimitCache++;
        if(feverStackLimitCache < feverStackLimitPerSecond)
        {
            FeverStack++;
        }
    }

    private float GetFeverRatio()
    {
        return (float)FeverStack / targetFeverStack;
    }

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

    private void ActiveFeverMode()
    {
        // 활성화 효과
        feverButton.image.raycastTarget = true;
        feverButton.enabled = true;
    }

    private void DisableFeverButton()
    {
        feverButton.image.raycastTarget = false;
        feverButton.enabled = false;
        feverButton.gameObject.SetActive(false);
    }

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

    public void StartFeverMode()
    {
        int rand = Random.Range(0, 100);

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

    private void StartAutoTouchingFever()
    {
        // 이펙트 활성화
        StartCoroutine(AutoTouching(autoTouchAmount, duration));
    }

    private void StartMassiveIncomeFever()
    {
        // 터치 및 시간형 수익 증가
        itemManager.SetMassiveIncomeEnable((int)duration, (ulong)massiveIncomeAmount, (ulong)massiveIncomeAmount, ref activedTouchAbility, ref activedTimeAbility);
        // 이펙트 활성화
        StartCoroutine(FeverModeTimer(duration));
    }

    IEnumerator FeverModeTimer(float timeLeft, float interval = 0.2f)
    {
        yield return new WaitForSeconds(interval);

        timeLeft -= interval;

        SetFeverStackImage(timeLeft / duration, false);

        if(timeLeft > 0)
            StartCoroutine(FeverModeTimer(timeLeft));
        else
            DisableFeverMode();
    }

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

    private void StartFeverStartEffect()
    {
        feverStartAnimation.gameObject.SetActive(true);
        feverStartAnimation.Play();
        PlayManager.instance.WaitAnimation(feverStartAnimation, StartFeverSpreteChange);
    }

    private void StartFeverSpreteChange()
    {
        StartCoroutine(FeverSpriteChange(0.4f));
    }

    IEnumerator FeverSpriteChange(float interval)
    {
        yield return new WaitForSeconds(interval);

        feverOuterLineImg.sprite = feverOuterLineSprites[(spriteIndex++) % 2];

        if (feverActived)
            StartCoroutine(FeverSpriteChange(interval));
    }
}
