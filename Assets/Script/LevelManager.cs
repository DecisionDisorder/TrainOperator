using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 레벨 시스템 관리 클래스
/// </summary>
public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// 사용자 레벨 데이터 오브젝트
    /// </summary>
    public LevelData levelData;
    /// <summary>
    /// 현재 레벨
    /// </summary>
    public int Level { get { return levelData.level; } set { levelData.level = value; } }
    /// <summary>
    /// 최대 레벨
    /// </summary>
    public int MaxLevel { get { return requiredTouch.Length * 10; } }
    /// <summary>
    /// 현재 경험치 량
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
    /// 첫 추가 수익률(2레벨) 보정 수치
    /// </summary>
    public float firstRevenueAdd;
    /// <summary>
    /// 10레벨 마다의 경험치 비율
    /// </summary>
    public float[] expRatio;
    /// <summary>
    /// 각 노선별 요구 터치량
    /// </summary>
    public int[] requiredTouch;
    /// <summary>
    /// 진행중인 노선 대비 레벨이 낮을 떄의 추가 경험치 배율
    /// </summary>
    public float[] lowLevelBonus;
    /// <summary>
    /// 요구 터치량 반영 비율
    /// </summary>
    public float requireTouchRatio;
    /// <summary>
    /// 한 노선마다의 추가 수익률
    /// </summary>
    public float revenueMagnificationPerLine;
    /// <summary>
    /// 경험치 기본 지급량
    /// </summary>
    public int expProvision = 10;
    /// <summary>
    /// 레벨업 요구 경험치량
    /// </summary>
    public int RequiredExp { get { return (int)(requiredTouch[(Level - 1) / 10] * expRatio[(Level - 1) % 10] * requireTouchRatio) * expProvision; } }
    /// <summary>
    /// 추가 수익 배율
    /// </summary>
    public float RevenueMagnification { get { return GetRevenueMagnification(); } }
    /// <summary>
    /// 최대 추가 수익 배율
    /// </summary>
    public float maximumRevenue;
    /// <summary>
    /// 현재 진행 중인 노선
    /// </summary>
    private int CurrentLine { get { return (int)lineManager.GetRecentlyOpenedLine(); } }

    /// <summary>
    /// 레벨 경험치 슬라이더
    /// </summary>
    public Slider levelExpSlider;
    /// <summary>
    /// 레벨 정보 텍스트
    /// </summary>
    public Text levelInfoText;
    /// <summary>
    /// 레벨업 애니메이션 효과
    /// </summary>
    public Animation levelUpAni;

    /// <summary>
    /// 경험치 증감 애니메이션 효과 재생 대기열
    /// </summary>
    private Queue<IEnumerator> expSliderQueue = new Queue<IEnumerator>();
    /// <summary>
    /// 경험치 슬라이더가 증감하고 있는지 여부
    /// </summary>
    private bool isExpSliderMoving = false;
    /// <summary>
    /// 경험치 슬라이더 증감 속도
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
    /// 레벨 정보 업데이트
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
    /// 경험치 지급
    /// </summary>
    public void AddExp()
    {
        // 최대 경험치 미만인 경우
        if (!Level.Equals(MaxLevel))
        {
            // 지급할 경험치량 계산
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

            // 경험치 게이지 효과 재생 대기열 등록
            if (Exp + addExp >= RequiredExp)
                expSliderQueue.Enqueue(ExpGageEffect(Exp, RequiredExp, RequiredExp));
            else
                expSliderQueue.Enqueue(ExpGageEffect(Exp, Exp + addExp, RequiredExp));
            Exp += addExp;
        }
    }

    /// <summary>
    /// 레벨업 처리
    /// </summary>
    /// <param name="exp">레벨업 처리 직전 경험치</param>
    private void LevelUp(int exp)
    {
        // 경험치 게이지 감소 효과 대기열 등록
        expSliderQueue.Enqueue(ExpGageEffect(RequiredExp, 0, RequiredExp));
        // 레벨업 경험치 요구량만큼 경험치 차감
        int to = exp - RequiredExp;
        // 레벨업 처리 및 관련 효과 재생
        Level++;
        levelUpAni.Play();

        // 최대 레벨 도달 여부 확인 후 안내 메시지 출력
        if (Level.Equals(MaxLevel))
        {
            Exp = 0;
            messageManager.ShowMessage("최대 레벨에 도달하셨습니다. 축하드립니다!\n<size=28>(최대 레벨 도달한 이후에는 경험치가 누적되지 않습니다.)</size>", 5.0f);
        }
        else
        {
            expSliderQueue.Enqueue(ExpGageEffect(0, to, RequiredExp));
            Exp = to;

            // 연속 레벨업 확인
            if (Exp >= RequiredExp)
                LevelUp(Exp);
        }
    }

    /// <summary>
    /// 경험치 게이지 효과 시작
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
    /// 레벨 비례 추가 수익 증가율 계산
    /// </summary>
    /// <returns>추가 수익 증가율</returns>
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
    /// 누적 수익 증가율
    /// </summary>
    /// <param name="index">노선 인덱스</param>
    /// <returns>특정 노선 단계까지의 수익 증가율</returns>
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
    /// 경험치 슬라이더 증감 효과 처리 코루틴
    /// </summary>
    /// <param name="from">시작 수치</param>
    /// <param name="to">종료 수치</param>
    /// <param name="maxValue">최대 값</param>
    IEnumerator ExpGageEffect(int from, int to, float maxValue)
    {
        yield return new WaitForEndOfFrame();

        float currentPercentage;
        float deltaSpeed;
        // 경험치 슬라이더 최대값 업데이트
        if (maxValue != levelExpSlider.maxValue)
            levelExpSlider.maxValue = maxValue;

        // 증가 효과 처리
        if (from < to)
        {
            currentPercentage = (levelExpSlider.value - from) / (to - from) * 100f;

            // 대기열 개수에 따라 게이지 이동 속도 조절
            if(expSliderQueue.Count > 30)
                deltaSpeed = (to - from) * gageSpeed * 5 * Time.deltaTime;
            else if(expSliderQueue.Count > 15)
                deltaSpeed = (to - from) * gageSpeed * 2 * Time.deltaTime;
            else if (currentPercentage < 90)
                deltaSpeed = (to - from) * gageSpeed * Time.deltaTime;
            else
                deltaSpeed = (to - from) / 2f * gageSpeed * Time.deltaTime;

            // 게이지 값 계산
            levelExpSlider.value += deltaSpeed;
            if (levelExpSlider.value > to)
                levelExpSlider.value = to;

            // 다음 값 계산
            if (levelExpSlider.value != to && levelExpSlider.value < levelExpSlider.maxValue)
                StartCoroutine(ExpGageEffect(from, to, maxValue));
            // 대기열에 등록된 다음 경험치 증감효과 시작
            else if (expSliderQueue.Count > 0)
                StartCoroutine(expSliderQueue.Dequeue());
            // 경험치 증감 효과 종료
            else
                isExpSliderMoving = false;
        }
        // 그대로인 경우 아무 효과 없음
        else if (from == to)
        {
            levelExpSlider.value = to;
            isExpSliderMoving = false;
        }
        // 감소 효과 처리
        else
        {
            currentPercentage = levelExpSlider.value / (from + to) * 100f;

            // 진행 비율에 따라 속도 조절
            if (currentPercentage > 10)
                deltaSpeed = (from - to) * Time.deltaTime;
            else
                deltaSpeed = (from - to) / 2f * Time.deltaTime;

            // 게이지 값 계산
            levelExpSlider.value -= deltaSpeed;
            if (levelExpSlider.value < to)
                levelExpSlider.value = to;

            // 다음 값 계산
            if (levelExpSlider.value != to && levelExpSlider.value > levelExpSlider.minValue)
                StartCoroutine(ExpGageEffect(from, to, maxValue));
            // 대기열에 등록된 다음 경험치 증감효과 시작
            else if (expSliderQueue.Count > 0)
                StartCoroutine(expSliderQueue.Dequeue());
            // 경험치 증감 효과 종료
            else
                isExpSliderMoving = false;
        }
    }
}