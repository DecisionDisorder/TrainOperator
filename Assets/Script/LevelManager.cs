using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public LevelData levelData;
    public int Level { get { return levelData.level; } set { levelData.level = value; } }
    public int MaxLevel { get { return requiredTouch.Length * 10; } }
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

    public float firstRevenueAdd;
    public float[] expRatio;
    public int[] requiredTouch;
    public float[] lowLevelBonus;
    public float requireTouchRatio;
    public float revenueMagnificationPerLine;
    public int expProvision = 10;
    public int RequiredExp { get { return (int)(requiredTouch[(Level - 1) / 10] * expRatio[(Level - 1) % 10] * requireTouchRatio) * expProvision; } }
    public float RevenueMagnification { get { return GetRevenueMagnification(); } }
    public float maximumRevenue;
    private int CurrentLine { get { return (int)lineManager.GetRecentlyOpenedLine(); } }

    public Slider levelExpSlider;
    public Text levelInfoText;
    public Animation levelUpAni;

    private Queue<IEnumerator> expSliderQueue = new Queue<IEnumerator>();
    private bool isExpSliderMoving = false;
    public float gageSpeed;

    public LineManager lineManager;

    private void Start()
    {
        levelExpSlider.maxValue = RequiredExp;
        levelExpSlider.value = Exp;
        UpdateLevelInfo();
    }

    private void UpdateLevelInfo()
    {
        StartGageEffect();
        levelInfoText.text = string.Format("Lv.{0} ({1:0.00}%)", Level, 100f * Exp / RequiredExp);
    }


    public void AddExp()
    {
        if (!Level.Equals(MaxLevel))
        {
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

            if (Exp + addExp >= RequiredExp)
                expSliderQueue.Enqueue(ExpGageEffect(Exp, RequiredExp, RequiredExp));
            else
                expSliderQueue.Enqueue(ExpGageEffect(Exp, Exp + addExp, RequiredExp));
            Exp += addExp;
            //Debug.Log(addExp);
        }
    }

    private void LevelUp(int exp)
    {
        expSliderQueue.Enqueue(ExpGageEffect(RequiredExp, 0, RequiredExp));
        int to = exp - RequiredExp;
        Level++;
        levelUpAni.Play();

        if (Level.Equals(MaxLevel))
            Exp = 0;
        else
        {
            expSliderQueue.Enqueue(ExpGageEffect(0, to, RequiredExp));
            Exp = to;

            if (Exp >= RequiredExp)
                LevelUp(Exp);
        }
    }

    private void StartGageEffect()
    {
        if (expSliderQueue.Count > 0 && !isExpSliderMoving)
        {
            isExpSliderMoving = true;
            IEnumerator cor = expSliderQueue.Dequeue();
            StartCoroutine(cor);
        }
    }

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
    private float GetCumulativeMagnification(int index)
    {
        float result = 0;
        for (int i = 0; i <= index; i++)
        {
            result += expRatio[i] * revenueMagnificationPerLine;
        }
        return result;
    }

    IEnumerator ExpGageEffect(int from, int to, float maxValue)
    {
        yield return new WaitForEndOfFrame();

        float currentPercentage;
        float deltaSpeed;
        if (maxValue != levelExpSlider.maxValue)
            levelExpSlider.maxValue = maxValue;

        if (from < to)
        {
            currentPercentage = (levelExpSlider.value - from) / (to - from) * 100f;

            if(expSliderQueue.Count > 30)
                deltaSpeed = (to - from) * gageSpeed * 5 * Time.deltaTime;
            else if(expSliderQueue.Count > 15)
                deltaSpeed = (to - from) * gageSpeed * 2 * Time.deltaTime;
            else if (currentPercentage < 90)
                deltaSpeed = (to - from) * gageSpeed * Time.deltaTime;
            else
                deltaSpeed = (to - from) / 2f * gageSpeed * Time.deltaTime;

            levelExpSlider.value += deltaSpeed;
            if (levelExpSlider.value > to)
                levelExpSlider.value = to;

            if (levelExpSlider.value != to && levelExpSlider.value < levelExpSlider.maxValue)
                StartCoroutine(ExpGageEffect(from, to, maxValue));
            else if (expSliderQueue.Count > 0)
            {
                StartCoroutine(expSliderQueue.Dequeue());
            }
            else
                isExpSliderMoving = false;
        }
        else if (from == to)
        {
            levelExpSlider.value = to;
            isExpSliderMoving = false;
        }
        else
        {
            currentPercentage = levelExpSlider.value / (from + to) * 100f;

            if (currentPercentage > 10)
                deltaSpeed = (from - to) * Time.deltaTime;
            else
                deltaSpeed = (from - to) / 2f * Time.deltaTime;

            levelExpSlider.value -= deltaSpeed;
            if (levelExpSlider.value < to)
                levelExpSlider.value = to;

            if (levelExpSlider.value != to && levelExpSlider.value > levelExpSlider.minValue)
                StartCoroutine(ExpGageEffect(from, to, maxValue));
            else if (expSliderQueue.Count > 0)
            {
                StartCoroutine(expSliderQueue.Dequeue());
            }
            else
                isExpSliderMoving = false;
        }
    }
}