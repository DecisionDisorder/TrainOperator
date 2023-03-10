using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� �ý��� ���� Ŭ����
/// </summary>
public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// ����� ���� ������ ������Ʈ
    /// </summary>
    public LevelData levelData;
    /// <summary>
    /// ���� ����
    /// </summary>
    public int Level { get { return levelData.level; } set { levelData.level = value; } }
    /// <summary>
    /// �ִ� ����
    /// </summary>
    public int MaxLevel { get { return requiredTouch.Length * 10; } }
    /// <summary>
    /// ���� ����ġ ��
    /// </summary>
    public int Exp
    {
        get { return levelData.exp; }
        set
        {
            if (value >= RequiredExp)
                LevelUp(value);
            else
                levelData.exp = value;
            UpdateLevelInfo();
        }
    }

    /// <summary>
    /// ù �߰� ���ͷ�(2����) ���� ��ġ
    /// </summary>
    public float firstRevenueAdd;
    /// <summary>
    /// 10���� ������ ����ġ ����
    /// </summary>
    public float[] expRatio;
    /// <summary>
    /// �� �뼱�� �䱸 ��ġ��
    /// </summary>
    public int[] requiredTouch;
    /// <summary>
    /// �������� �뼱 ��� ������ ���� ���� �߰� ����ġ ����
    /// </summary>
    public float[] lowLevelBonus;
    /// <summary>
    /// �䱸 ��ġ�� �ݿ� ����
    /// </summary>
    public float requireTouchRatio;
    /// <summary>
    /// �� �뼱������ �߰� ���ͷ�
    /// </summary>
    public float revenueMagnificationPerLine;
    /// <summary>
    /// ����ġ �⺻ ���޷�
    /// </summary>
    public int expProvision = 10;
    /// <summary>
    /// ������ �䱸 ����ġ��
    /// </summary>
    public int RequiredExp { get { return (int)(requiredTouch[(Level - 1) / 10] * expRatio[(Level - 1) % 10] * requireTouchRatio) * expProvision; } }
    /// <summary>
    /// �߰� ���� ����
    /// </summary>
    public float RevenueMagnification { get { return GetRevenueMagnification(); } }
    /// <summary>
    /// �ִ� �߰� ���� ����
    /// </summary>
    public float maximumRevenue;
    /// <summary>
    /// ���� ���� ���� �뼱
    /// </summary>
    private int CurrentLine { get { return (int)lineManager.GetRecentlyOpenedLine(); } }

    /// <summary>
    /// ���� ����ġ �����̴�
    /// </summary>
    public Slider levelExpSlider;
    /// <summary>
    /// ���� ���� �ؽ�Ʈ
    /// </summary>
    public Text levelInfoText;
    /// <summary>
    /// ������ �ִϸ��̼� ȿ��
    /// </summary>
    public Animation levelUpAni;

    /// <summary>
    /// ����ġ ���� �ִϸ��̼� ȿ�� ��� ��⿭
    /// </summary>
    private Queue<IEnumerator> expSliderQueue = new Queue<IEnumerator>();
    /// <summary>
    /// ����ġ �����̴��� �����ϰ� �ִ��� ����
    /// </summary>
    private bool isExpSliderMoving = false;
    /// <summary>
    /// ����ġ �����̴� ���� �ӵ�
    /// </summary>
    public float gageSpeed;

    public LineManager lineManager;
    public MessageManager messageManager;

    private void Start()
    {
        levelExpSlider.maxValue = RequiredExp;
        levelExpSlider.value = Exp;
        UpdateLevelInfo();
    }

    /// <summary>
    /// ���� ���� ������Ʈ
    /// </summary>
    private void UpdateLevelInfo()
    {
        StartGageEffect();
        if (Level < MaxLevel)
            levelInfoText.text = string.Format("Lv.{0} ({1:0.00}%)", Level, 100f * Exp / RequiredExp);
        else
            levelInfoText.text = "Lv." + Level + " (MAX)";
    }

    /// <summary>
    /// ����ġ ����
    /// </summary>
    public void AddExp()
    {
        // �ִ� ����ġ �̸��� ���
        if (!Level.Equals(MaxLevel))
        {
            // ������ ����ġ�� ���
            int addExp = 0;
            if (CurrentLine * 10 < Level && Level <= (CurrentLine + 1) * 10)
                addExp = expProvision;
            else if (Level <= CurrentLine * 10)
            {
                int lowLevelIndex = CurrentLine - (Level - 1) / 10 - 1;
                if(lowLevelIndex >= 5)
                    addExp = (int)(expProvision * (1 + lowLevelBonus[4]));
                else
                    addExp = (int)(expProvision * (1 + lowLevelBonus[lowLevelIndex]));
            }
            else
                addExp = expProvision / 10;

            // ����ġ ������ ȿ�� ��� ��⿭ ���
            if (Exp + addExp >= RequiredExp)
                expSliderQueue.Enqueue(ExpGageEffect(Exp, RequiredExp, RequiredExp));
            else
                expSliderQueue.Enqueue(ExpGageEffect(Exp, Exp + addExp, RequiredExp));
            Exp += addExp;
        }
    }

    /// <summary>
    /// ������ ó��
    /// </summary>
    /// <param name="exp">������ ó�� ���� ����ġ</param>
    private void LevelUp(int exp)
    {
        // ����ġ ������ ���� ȿ�� ��⿭ ���
        expSliderQueue.Enqueue(ExpGageEffect(RequiredExp, 0, RequiredExp));
        // ������ ����ġ �䱸����ŭ ����ġ ����
        int to = exp - RequiredExp;
        // ������ ó�� �� ���� ȿ�� ���
        Level++;
        levelUpAni.Play();

        // �ִ� ���� ���� ���� Ȯ�� �� �ȳ� �޽��� ���
        if (Level.Equals(MaxLevel))
        {
            Exp = 0;
            messageManager.ShowMessage("�ִ� ������ �����ϼ̽��ϴ�. ���ϵ帳�ϴ�!\n<size=28>(�ִ� ���� ������ ���Ŀ��� ����ġ�� �������� �ʽ��ϴ�.)</size>", 5.0f);
        }
        else
        {
            expSliderQueue.Enqueue(ExpGageEffect(0, to, RequiredExp));
            Exp = to;

            // ���� ������ Ȯ��
            if (Exp >= RequiredExp)
                LevelUp(Exp);
        }
    }

    /// <summary>
    /// ����ġ ������ ȿ�� ����
    /// </summary>
    private void StartGageEffect()
    {
        if (expSliderQueue.Count > 0 && !isExpSliderMoving)
        {
            isExpSliderMoving = true;
            IEnumerator cor = expSliderQueue.Dequeue();
            StartCoroutine(cor);
        }
    }

    /// <summary>
    /// ���� ��� �߰� ���� ������ ���
    /// </summary>
    /// <returns>�߰� ���� ������</returns>
    private float GetRevenueMagnification()
    {
        if (Level.Equals(1))
            return 1f;
        else if (Level.Equals(2))
            return firstRevenueAdd + expRatio[(Level - 1) % 10] * revenueMagnificationPerLine;
        else
        {
            float result = 1 + revenueMagnificationPerLine * ((Level - 1) / 10) + GetCumulativeMagnification((Level - 1) % 10);
            if (result > maximumRevenue)
                return maximumRevenue;
            else
                return result;
        }
            
    }

    /// <summary>
    /// ���� ���� ������
    /// </summary>
    /// <param name="index">�뼱 �ε���</param>
    /// <returns>Ư�� �뼱 �ܰ������ ���� ������</returns>
    private float GetCumulativeMagnification(int index)
    {
        float result = 0;
        for (int i = 0; i <= index; i++)
        {
            result += expRatio[i] * revenueMagnificationPerLine;
        }
        return result;
    }

    /// <summary>
    /// ����ġ �����̴� ���� ȿ�� ó�� �ڷ�ƾ
    /// </summary>
    /// <param name="from">���� ��ġ</param>
    /// <param name="to">���� ��ġ</param>
    /// <param name="maxValue">�ִ� ��</param>
    IEnumerator ExpGageEffect(int from, int to, float maxValue)
    {
        yield return new WaitForEndOfFrame();

        float currentPercentage;
        float deltaSpeed;
        // ����ġ �����̴� �ִ밪 ������Ʈ
        if (maxValue != levelExpSlider.maxValue)
            levelExpSlider.maxValue = maxValue;

        // ���� ȿ�� ó��
        if (from < to)
        {
            currentPercentage = (levelExpSlider.value - from) / (to - from) * 100f;

            // ��⿭ ������ ���� ������ �̵� �ӵ� ����
            if(expSliderQueue.Count > 30)
                deltaSpeed = (to - from) * gageSpeed * 5 * Time.deltaTime;
            else if(expSliderQueue.Count > 15)
                deltaSpeed = (to - from) * gageSpeed * 2 * Time.deltaTime;
            else if (currentPercentage < 90)
                deltaSpeed = (to - from) * gageSpeed * Time.deltaTime;
            else
                deltaSpeed = (to - from) / 2f * gageSpeed * Time.deltaTime;

            // ������ �� ���
            levelExpSlider.value += deltaSpeed;
            if (levelExpSlider.value > to)
                levelExpSlider.value = to;

            // ���� �� ���
            if (levelExpSlider.value != to && levelExpSlider.value < levelExpSlider.maxValue)
                StartCoroutine(ExpGageEffect(from, to, maxValue));
            // ��⿭�� ��ϵ� ���� ����ġ ����ȿ�� ����
            else if (expSliderQueue.Count > 0)
                StartCoroutine(expSliderQueue.Dequeue());
            // ����ġ ���� ȿ�� ����
            else
                isExpSliderMoving = false;
        }
        // �״���� ��� �ƹ� ȿ�� ����
        else if (from == to)
        {
            levelExpSlider.value = to;
            isExpSliderMoving = false;
        }
        // ���� ȿ�� ó��
        else
        {
            currentPercentage = levelExpSlider.value / (from + to) * 100f;

            // ���� ������ ���� �ӵ� ����
            if (currentPercentage > 10)
                deltaSpeed = (from - to) * Time.deltaTime;
            else
                deltaSpeed = (from - to) / 2f * Time.deltaTime;

            // ������ �� ���
            levelExpSlider.value -= deltaSpeed;
            if (levelExpSlider.value < to)
                levelExpSlider.value = to;

            // ���� �� ���
            if (levelExpSlider.value != to && levelExpSlider.value > levelExpSlider.minValue)
                StartCoroutine(ExpGageEffect(from, to, maxValue));
            // ��⿭�� ��ϵ� ���� ����ġ ����ȿ�� ����
            else if (expSliderQueue.Count > 0)
                StartCoroutine(expSliderQueue.Dequeue());
            // ����ġ ���� ȿ�� ����
            else
                isExpSliderMoving = false;
        }
    }
}