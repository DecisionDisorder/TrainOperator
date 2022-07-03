using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeverManager : MonoBehaviour
{
    public bool feverActived = false;

    public int feverStack;
    public int targetFeverStack = 500;
    public int feverStackLimitPerSecond = 25;
    private int feverStackLimitCache;

    public float duration = 10f;
    private FeverMode selectedFeverMode;
    private enum FeverMode { AutoTouching, MassiveIncome }
    public int autoTouchAmount = 20;
    public int possibilityOfAutoTouching = 30;
    public int massiveIncomeAmount = 5;

    public Color fillingColor;
    public Color filledColor;

    public Image feverImg;
    public Button feverButton;

    public TouchEarning touchEarning;
    public ItemManager itemManager;

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
            feverStack++;
            SetFeverStackImage(GetFeverRatio(), true);
            if(feverStack >= targetFeverStack)
            {
                ActiveFeverMode();
            }
        }
    }

    private float GetFeverRatio()
    {
        return (float)feverStack / targetFeverStack;
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
    }

    private void DisableFeverMode()
    {
        feverStack = 0;
        feverActived = false;
        SetFeverStackImage(0, true);

        // 이펙트 비활성화
        if(selectedFeverMode == FeverMode.MassiveIncome)
        {
            // 수익율 원상복구
            itemManager.SetMassiveIncomeDisable();
            itemManager.SetActiveColorCard(true);
        }
        else
        {
            itemManager.SetActiveRareCard(true);
        }
    }

    public void StartFeverMode()
    {
        int rand = Random.Range(0, 100);

        if(rand < possibilityOfAutoTouching)
        {
            selectedFeverMode = FeverMode.AutoTouching;
            itemManager.SetActiveRareCard(false);
            StartAutoTouchingFever();
        }
        else
        {
            selectedFeverMode = FeverMode.MassiveIncome;
            itemManager.SetActiveColorCard(false);
            StartMassiveIncomeFever();
        }
        feverActived = true;
        DisableFeverButton();
    }

    private void StartAutoTouchingFever()
    {
        // 이펙트 활성화
        StartCoroutine(AutoTouching(autoTouchAmount, duration));
    }

    private void StartMassiveIncomeFever()
    {
        // 터치 및 시간형 수익 증가
        itemManager.SetMassiveIncomeEnable((int)duration, (ulong)massiveIncomeAmount, (ulong)massiveIncomeAmount);
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
}
