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
        // Ȱ��ȭ ȿ��
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

        // ����Ʈ ��Ȱ��ȭ
        if (selectedFeverMode == FeverMode.MassiveIncome)
        {
            // ������ ���󺹱�
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
            messageManager.ShowMessage("�ڵ� ���� �ǹ� Ÿ���� �ߵ��Ͽ����ϴ�!", messageBackgroundColor, 3.0f);
            StartAutoTouchingFever();
        }
        else
        {
            selectedFeverMode = FeverMode.MassiveIncome;
            //itemManager.SetActiveColorCard(false);
            messageManager.ShowMessage("���ͷ� ���� �ǹ� Ÿ���� �ߵ��Ͽ����ϴ�!", messageBackgroundColor, 3.0f);
            StartMassiveIncomeFever();
        }
        FeverStack = 0;
        feverActived = true;
        DisableFeverButton();
        StartFeverStartEffect();
    }

    private void StartAutoTouchingFever()
    {
        // ����Ʈ Ȱ��ȭ
        StartCoroutine(AutoTouching(autoTouchAmount, duration));
    }

    private void StartMassiveIncomeFever()
    {
        // ��ġ �� �ð��� ���� ����
        itemManager.SetMassiveIncomeEnable((int)duration, (ulong)massiveIncomeAmount, (ulong)massiveIncomeAmount, ref activedTouchAbility, ref activedTimeAbility);
        // ����Ʈ Ȱ��ȭ
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
