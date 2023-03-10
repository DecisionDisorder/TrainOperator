using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ǹ� �ý��� ���� Ŭ����
/// </summary>
public class FeverManager : MonoBehaviour
{
    /// <summary>
    /// �ǹ��� Ȱ��ȭ ������ ����
    /// </summary>
    public bool feverActived = false;

    /// <summary>
    /// �ǹ� ���� ��ġ
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
    /// ��ǥ �ǹ� ���� ��ġ
    /// </summary>
    public int targetFeverStack = 500;
    /// <summary>
    /// 1�ʴ� ȹ���� �� �ִ� �ִ� �ǹ� ���� ��ġ
    /// </summary>
    public int feverStackLimitPerSecond = 25;
    /// <summary>
    /// �ʴ� �ǹ� ���� ���ѷ��� ����� ���� �ӽ� ����
    /// </summary>
    private int feverStackLimitCache;

    /// <summary>
    /// �ǹ� ���ӽð�
    /// </summary>
    public float duration = 10f;
    /// <summary>
    /// ���õ� �ǹ� ��� ����
    /// </summary>
    private FeverMode selectedFeverMode;
    /// <summary>
    /// �ǹ� ��� ������ (�ڵ� ��ġ, ���ͷ� ����)
    /// </summary>
    private enum FeverMode { AutoTouching, MassiveIncome }
    /// <summary>
    /// �ǹ� ��� ���� �ڵ� �ʴ� ��ġ ��
    /// </summary>
    public int autoTouchAmount = 20;
    /// <summary>
    /// �ڵ� ��ġ �߻� Ȯ��
    /// </summary>
    public int possibilityOfAutoTouching = 30;
    /// <summary>
    /// ���ͷ� ���� ȿ�� (����)
    /// </summary>
    public int massiveIncomeAmount = 5;

    /// <summary>
    /// ����� ��ġ�� ���� ���� ��ġ
    /// </summary>
    private ulong activedTouchAbility;
    /// <summary>
    /// ����� �ð��� ���� ���� ��ġ
    /// </summary>
    public ulong activedTimeAbility;

    /// <summary>
    /// �ǹ� ������ ä������ ������ ����
    /// </summary>
    public Color fillingColor;
    /// <summary>
    /// �ǹ� ������ ��� ä���� ������ ����
    /// </summary>
    public Color filledColor;
    /// <summary>
    /// �޽��� ��� ����
    /// </summary>
    public Color messageBackgroundColor;

    /// <summary>
    /// �ǹ� ������ �̹���
    /// </summary>
    public Image feverImg;
    /// <summary>
    /// �ǹ� ��� �ߵ� ��ư
    /// </summary>
    public Button feverButton;

    /// <summary>
    /// �ǹ� ���� �ִϸ��̼�
    /// </summary>
    public Animation feverStartAnimation;
    /// <summary>
    /// �ǹ� ��� �̹���
    /// </summary>
    public Image feverOuterLineImg;
    /// <summary>
    /// �ǹ� ��� �̹��� ��������Ʈ ���ҽ� �迭
    /// </summary>
    public Sprite[] feverOuterLineSprites;

    /// <summary>
    /// ���� �������� ��������Ʈ�� �ε���
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
    /// �ʴ� ���� ���ѷ� �ʱ�ȭ Ÿ�̸�
    /// </summary>
    /// <returns></returns>
    IEnumerator StackLimitTimer()
    {
        yield return new WaitForSeconds(1);

        feverStackLimitCache = 0;

        StartCoroutine(StackLimitTimer());
    }

    /// <summary>
    /// �ǹ� ���� �߰�
    /// </summary>
    public void AddFeverStack()
    {
        // �ʴ� ���ѷ��� �ɸ��� ������ �ǹ� ������ ���Ѵ�
        feverStackLimitCache++;
        if(feverStackLimitCache < feverStackLimitPerSecond)
        {
            FeverStack++;
        }
    }

    /// <summary>
    /// �ǹ� ���� ���
    /// </summary>
    /// <returns>�ǹ� ������ ���� ����</returns>
    private float GetFeverRatio()
    {
        return (float)FeverStack / targetFeverStack;
    }

    /// <summary>
    /// �ǹ� ���� �̹��� ����
    /// </summary>
    /// <param name="ratio">�ǹ� ���� ����</param>
    /// <param name="isAdding">�������� �ִ��� ����</param>
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
    /// �ǹ� ��� Ȱ��ȭ
    /// </summary>
    private void ActiveFeverMode()
    {
        // Ȱ��ȭ ȿ��
        feverButton.image.raycastTarget = true;
        feverButton.enabled = true;
    }

    /// <summary>
    /// �ǹ� ��ư ��Ȱ��ȭ
    /// </summary>
    private void DisableFeverButton()
    {
        feverButton.image.raycastTarget = false;
        feverButton.enabled = false;
        feverButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// �ǹ� ȿ�� ��Ȱ��ȭ
    /// </summary>
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

    /// <summary>
    /// �ǹ� ��� ����
    /// </summary>
    public void StartFeverMode()
    {
        int rand = Random.Range(0, 100);

        // Ȯ���� ���� �ڵ� ��ġ Ȥ�� ���ͷ� ���� ȿ�� ����
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

    /// <summary>
    /// �ڵ� ��ġ �ǹ� ȿ�� �ߵ�
    /// </summary>
    private void StartAutoTouchingFever()
    {
        // ����Ʈ Ȱ��ȭ
        StartCoroutine(AutoTouching(autoTouchAmount, duration));
    }

    /// <summary>
    /// ���ͷ� ���� �ǹ� ȿ�� �ߵ�
    /// </summary>
    private void StartMassiveIncomeFever()
    {
        // ��ġ �� �ð��� ���� ����
        itemManager.SetMassiveIncomeEnable((int)duration, (ulong)massiveIncomeAmount, (ulong)massiveIncomeAmount, ref activedTouchAbility, ref activedTimeAbility);
        // ����Ʈ Ȱ��ȭ
        StartCoroutine(FeverModeTimer(duration));
    }

    /// <summary>
    /// �ǹ� ��� Ÿ�̸�
    /// </summary>
    /// <param name="timeLeft">���� �ð�</param>
    /// <param name="interval">������Ʈ ����</param>
    /// <returns></returns>
    IEnumerator FeverModeTimer(float timeLeft, float interval = 0.2f)
    {
        yield return new WaitForSeconds(interval);

        timeLeft -= interval;

        // ���� �ǹ� �ð� �̹��� ���� �ݿ�
        SetFeverStackImage(timeLeft / duration, false);

        if(timeLeft > 0)
            StartCoroutine(FeverModeTimer(timeLeft));
        else
            DisableFeverMode();
    }

    /// <summary>
    /// ������ġ ȿ�� �ڷ�ƾ
    /// </summary>
    /// <param name="touchAmountPerSecond">�ʴ� ��ġ Ƚ��</param>
    /// <param name="timeLeft">���� �ð�</param>
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
    /// �ǹ� ȿ�� �ִϸ��̼� ȿ�� ����
    /// </summary>
    private void StartFeverStartEffect()
    {
        feverStartAnimation.gameObject.SetActive(true);
        feverStartAnimation.Play();
        PlayManager.instance.WaitAnimation(feverStartAnimation, StartFeverSpriteChange);
    }

    /// <summary>
    /// �ǹ� ��������Ʈ �̹��� ��ü ȿ�� ����
    /// </summary>
    private void StartFeverSpriteChange()
    {
        StartCoroutine(FeverSpriteChange(0.4f));
    }

    /// <summary>
    /// �ǹ� ��������Ʈ �̹��� ��ü ȿ�� �ڷ�ƾ
    /// </summary>
    /// <param name="interval">��ü ����</param>
    IEnumerator FeverSpriteChange(float interval)
    {
        yield return new WaitForSeconds(interval);

        feverOuterLineImg.sprite = feverOuterLineSprites[(spriteIndex++) % 2];

        if (feverActived)
            StartCoroutine(FeverSpriteChange(interval));
    }
}
