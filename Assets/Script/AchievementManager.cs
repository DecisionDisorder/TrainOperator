using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 도전과제 보상 종류
/// </summary>
public enum AchievementRewardType { CardPoint, FeverCoupon, ColorCardPack, RareCardPack }
/// <summary>
/// 도전과제 시스템 관리 클래스
/// </summary>

public class AchievementManager : MonoBehaviour
{
    /// <summary>
    /// 도전과제 데이터 오브젝트 배열
    /// </summary>
    public Achievement[] achievements;

    /// <summary>
    /// 최대 달성 레벨
    /// </summary>
    public int maxAchvLevel = 5;

    /// <summary>
    /// 도전과제 성취 데이터 오브젝트
    /// </summary>
    public AchievementData achievementData;

    /// <summary>
    /// 각 도전과제별 성취 레벨
    /// </summary>
    public int[] AchvClearLevels { set { achievementData.achvClearLevels = value; } get { return achievementData.achvClearLevels; } }
    /// <summary>
    /// 사용자가 보유했던 최대의 돈
    /// </summary>
    public LargeVariable MaxMoney { set { achievementData.maxMoney = value; } get { return achievementData.maxMoney; } }
    /// <summary>
    /// 사용자가 터치메뉴를 터치한 횟수
    /// </summary>
    public int TouchCount { set { achievementData.touchCount = value; } get { return achievementData.touchCount; } }
    /// <summary>
    /// 사용자가 구매한 역의 개수
    /// </summary>
    public int TotalStationAmount { set { achievementData.totalStationAmount = value; } get { return achievementData.totalStationAmount; } }
    /// <summary>
    /// 사용자가 획득한 누적 이자
    /// </summary>
    public LargeVariable CumulativeInterest { set { achievementData.cumulativeInterest = value; } get { return achievementData.cumulativeInterest; } }
    /// <summary>
    /// 사용자가 보유한 모든 열차의 개수
    /// </summary>
    public int TotalTrainAmount {
        get
        {
            int count = 0;
            for (int i = 0; i < lineManager.lineCollections.Length; i++)
                count += lineManager.lineCollections[i].lineData.numOfTrain;
            return count;
        }
    }
    /// <summary>
    /// 사용자가 보유한 모든 기관사의 수
    /// </summary>
    public int TotalNumOfDrivers
    {
        get
        {
            int count = 0;
            for (int i = 0; i < driversManager.numOfDrivers.Length; i++)
                count += driversManager.numOfDrivers[i];
            return count;
        }
    }
    /// <summary>
    /// 사용자가 확장한 열차 칸의 개수
    /// </summary>
    public int TotalTrainExpandAmount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < lineManager.lineCollections.Length; i++)
                if(lineManager.lineCollections[i].lineData.trainExpandStatus.Length > 0)
                    count += lineManager.lineCollections[i].lineData.trainExpandStatus[1] + lineManager.lineCollections[i].lineData.trainExpandStatus[2] * 2 + lineManager.lineCollections[i].lineData.trainExpandStatus[3] * 3;
            return count;
        }
    }
    /// <summary>
    /// 사용자가 보유한 임대 시설의 개수
    /// </summary>
    public int TotalRentAmount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < rentManager.NumOfFacilities.Length; i++)
                count += rentManager.NumOfFacilities[i];
            return count;
        }
    }

    /// <summary>
    /// 도전과제 메뉴 오브젝트
    /// </summary>
    public GameObject achivementMenu;
    /// <summary>
    /// 도전과제 알림 이미지 오브젝트
    /// </summary>
    public GameObject alarmObj;

    public LevelManager levelManager;
    public DriversManager driversManager;
    public LineManager lineManager;
    public RentManager rentManager;
    public ItemManager itemManager;
    public MessageManager messageManager;

    /// <summary>
    /// 도전과제 성취 수치의 정보를 담는 구조체
    /// </summary>
    struct TargetVariable
    {
        /// <summary>
        /// 진행 정보 수치의 타입 종류
        /// </summary>ㅂ
        public enum ReturnType { Int, LargeVariable }
        /// <summary>
        /// 진행 정보 수치의 타입
        /// </summary>
        public ReturnType returnType { private set; get; }


        private int targetInt;
        /// <summary>
        /// 목표 수치(int)
        /// </summary>
        public int TargetInt { set { targetInt = value; returnType = ReturnType.Int; } get { return targetInt; } }

        private LargeVariable targetLargeVariable;
        /// <summary>
        /// 목표 수치(LargeVariable)
        /// </summary>
        public LargeVariable TargetLargeVariable { set { targetLargeVariable = value; returnType = ReturnType.LargeVariable; } get { return targetLargeVariable; } }

    }

    void Start()
    {
        // 역의 개수를 한번도 계산하지 않은 경우, 계산
        if(TotalStationAmount == 0 && MaxMoney == LargeVariable.zero)
            TotalStationAmount = CountTotalStationAmount();
        // 알람 업데이터 코루틴 시작
        StartCoroutine(UpdateAlarm());
    }

    /// <summary>
    /// 도전과제 메뉴 활성화/비활성화
    /// </summary>
    /// <param name="active">활성화 여부</param>
    public void SetAchivementMenu(bool active)
    {
        if (active)
        {
            UpdateRateAndSliders();
            StartCoroutine(AchivementUpdater());
        }

        achivementMenu.SetActive(active);
    }

    /// <summary>
    /// 도전과제 UI 업데이트 코루틴
    /// </summary>
    IEnumerator AchivementUpdater()
    {
        yield return new WaitForSeconds(1f);

        // 도전과제 UI가 활성화 되어있을 때 도전과제 변경사항 반영 (1초마다 검사)
        UpdateRateAndSliders();

        if(achivementMenu.activeInHierarchy)
            StartCoroutine(AchivementUpdater());
    }

    /// <summary>
    /// 도전과제 성취 알람 업데이트 코루틴
    /// </summary>
    IEnumerator UpdateAlarm()
    {
        yield return new WaitForSeconds(3);

        // 하나라도 도전과제 보상을 받을 조건이 되면 알람 오브젝트를 활성화 (3초마다 검사)
        bool active = false;
        for (int i = 0; i < achievements.Length; i++)
        {
            if (CheckMeetConditions(i))
            {
                active = true;
                break;
            }
        }
        alarmObj.SetActive(active);

        StartCoroutine(UpdateAlarm());
    }

    /// <summary>
    /// 도전과제 보상 수령
    /// </summary>
    /// <param name="index">도전과제 인덱스</param>
    public void ReceiveReward(int index)
    {
        if (CheckMeetConditions(index))
        {
            if (achievements[index].rewardType == AchievementRewardType.CardPoint)
            {
                itemManager.CardPoint += achievements[index].rewardAmounts[AchvClearLevels[index]];
            }
            else if (achievements[index].rewardType == AchievementRewardType.FeverCoupon)
            {
                itemManager.FeverRefillAmount += achievements[index].rewardAmounts[AchvClearLevels[index]];
            }
            else if (achievements[index].rewardType == AchievementRewardType.ColorCardPack)
            {
                itemManager.ColorPackAmount += achievements[index].rewardAmounts[AchvClearLevels[index]];
            }
            else if (achievements[index].rewardType == AchievementRewardType.RareCardPack)
            {
                itemManager.RarePackAmount += achievements[index].rewardAmounts[AchvClearLevels[index]];
            }
            AchvClearLevels[index]++;
            UpdateRateAndSliders();
        }
        else
            messageManager.ShowMessage("아직 달성 조건에 도달하지 못했습니다.");
    }
    /// <summary>
    /// 도전과제 성취 비율과 슬라이더 업데이트
    /// </summary>
    public void UpdateRateAndSliders()
    {
        // 모든 도전과제 항목에 대해
        for(int i = 0; i < achievements.Length; i++)
        {
            // 최대 레벨을 넘지 않았으면
            if (AchvClearLevels[i] < maxAchvLevel)
            {
                // 기준값과 현재 값을 구하여
                TargetVariable criteriaVariable = GetCriteriaAmount(i);
                TargetVariable currentVariable = GetTargetVariable(i);
                // 기준 값의 타입이 int일 때 
                if (criteriaVariable.returnType == TargetVariable.ReturnType.Int)
                {
                    // 슬라이더의 최대 값, 현재 값을 계산 하여 비율을 업데이트 한다
                    achievements[i].achivementSlider.maxValue = achievements[i].criteriaInt[AchvClearLevels[i]];
                    achievements[i].achivementSlider.value = currentVariable.TargetInt;
                    string currentVal = string.Format("{0:#,##0}", currentVariable.TargetInt), criteriaVal = string.Format("{0:#,##0}", achievements[i].criteriaInt[AchvClearLevels[i]]);
                    achievements[i].rateText.text =currentVal + "/" + criteriaVal;
                }
                // 기준 값의 타입이 LargeVariable 일 때
                else
                {
                    // 값의 high/low의 경우의 수를 고려하여 비율과 슬라이더를 업데이트 한다.
                    if (achievements[i].criteriaLargeVariable[AchvClearLevels[i]].highUnit == 0 && currentVariable.TargetLargeVariable.highUnit > 0)
                    {
                        achievements[i].achivementSlider.maxValue = achievements[i].criteriaLargeVariable[AchvClearLevels[i]].lowUnit;
                        achievements[i].achivementSlider.value = achievements[i].achivementSlider.maxValue;
                    }
                    else
                    {
                        if (achievements[i].criteriaLargeVariable[AchvClearLevels[i]].highUnit == 0)
                        {
                            achievements[i].achivementSlider.maxValue = achievements[i].criteriaLargeVariable[AchvClearLevels[i]].lowUnit;
                            achievements[i].achivementSlider.value = currentVariable.TargetLargeVariable.lowUnit;
                        }
                        else
                        {
                            achievements[i].achivementSlider.maxValue = achievements[i].criteriaLargeVariable[AchvClearLevels[i]].highUnit;
                            achievements[i].achivementSlider.value = currentVariable.TargetLargeVariable.highUnit;
                        }
                    }
                    string current = PlayManager.GetSimpleUnit(currentVariable.TargetLargeVariable.lowUnit, currentVariable.TargetLargeVariable.highUnit);
                    string criteria = PlayManager.GetSimpleUnit(criteriaVariable.TargetLargeVariable.lowUnit, criteriaVariable.TargetLargeVariable.highUnit);
                    achievements[i].rateText.text = current + "/" + criteria;
                }
                achievements[i].rewardAmountText.text = achievements[i].rewardAmounts[AchvClearLevels[i]].ToString();
            }
            // 최대 레벨을 넘었으면 클리어 했다는 정보를 업데이트 한다.
            else
            {
                achievements[i].achivementSlider.value = achievements[i].achivementSlider.maxValue;
                achievements[i].rateText.text = "Clear!";
                achievements[i].receiveRewardButton.gameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// 특정 도전과제의 현재 진행 상황 값을 찾아서 리턴한다.
    /// </summary>
    /// <param name="index">대상 도전과제 인덱스</param>
    /// <returns>현재 진행 상황 값</returns>
    private TargetVariable GetTargetVariable(int index)
    {
        TargetVariable targetVariable = new TargetVariable();
        switch(index)
        {
            case 0:
                if (MyAsset.instance.Money > MaxMoney)
                    MaxMoney = MyAsset.instance.Money;
                targetVariable.TargetLargeVariable = MaxMoney;
                break;
            case 1:
                targetVariable.TargetInt = TouchCount;
                break;
            case 2:
                targetVariable.TargetInt = levelManager.Level;
                break;
            case 3:
                targetVariable.TargetInt = TotalTrainAmount;
                break;
            case 4:
                targetVariable.TargetInt = TotalNumOfDrivers;
                break;
            case 5:
                targetVariable.TargetInt = TotalStationAmount;
                break;
            case 6:
                targetVariable.TargetInt = TotalTrainExpandAmount;
                break;
            case 7:
                targetVariable.TargetInt = TotalRentAmount;
                break;
            case 8:
                targetVariable.TargetLargeVariable = CumulativeInterest;
                break;
        }

        return targetVariable;
    }

    /// <summary>
    /// 특정 도전과제의 목표 값을 찾아서 리턴
    /// </summary>
    /// <param name="index">도전과제 인덱스</param>
    /// <returns>목표 값</returns>
    private TargetVariable GetCriteriaAmount(int index)
    {
        TargetVariable targetVariable = new TargetVariable();
        if (achievements[index].criteriaInt.Length > 0)
            targetVariable.TargetInt = achievements[index].criteriaInt[AchvClearLevels[index]];
        else
            targetVariable.TargetLargeVariable = achievements[index].criteriaLargeVariable[AchvClearLevels[index]];
        return targetVariable;
    }

    /// <summary>
    /// 특정 도전과제가 도전과제 보상 수령 기준에 충족하는지 확인
    /// </summary>
    /// <param name="index">도전과제 인덱스</param>
    /// <returns>조건 충족 여부</returns>
    private bool CheckMeetConditions(int index)
    {
        if (AchvClearLevels[index] < maxAchvLevel)
        {
            TargetVariable targetVariable = GetTargetVariable(index);
            if (targetVariable.returnType == TargetVariable.ReturnType.Int)
            {
                if (targetVariable.TargetInt >= achievements[index].criteriaInt[AchvClearLevels[index]])
                    return true;
                else
                    return false;
            }
            else
            {
                if (targetVariable.TargetLargeVariable >= achievements[index].criteriaLargeVariable[AchvClearLevels[index]])
                    return true;
                else
                    return false;
            }
        }
        else
            return false;
    }

    /// <summary>
    /// 사용자가 보유한 역의 개수 계산
    /// </summary>
    private int CountTotalStationAmount()
    {
        int count = 0;
        for (int i = 0; i < lineManager.lineCollections.Length; i++)
            for (int j = 0; j < lineManager.lineCollections[i].lineData.hasStation.Length; j++)
                if (lineManager.lineCollections[i].lineData.hasStation[j])
                    count++;
        return count;
    }
}

/// <summary>
/// 도전과제 클래스
/// </summary>
[System.Serializable]
public class Achievement
{
    /// <summary>
    /// 성취 비율 텍스트
    /// </summary>
    public Text rateText;
    /// <summary>
    /// 성취도 슬라이더(프로그래스 바)
    /// </summary>
    public Slider achivementSlider;
    /// <summary>
    /// 보상 수령 버튼
    /// </summary>
    public Button receiveRewardButton;
    /// <summary>
    /// 보상 수량 텏트
    /// </summary>
    public Text rewardAmountText;

    /// <summary>
    /// 성취 기준 값(int)
    /// </summary>
    public int[] criteriaInt;
    /// <summary>
    /// 성취 기준 값(LargeVariable)
    /// </summary>
    public LargeVariable[] criteriaLargeVariable;
    /// <summary>
    /// 도전과제 보상 타입
    /// </summary>
    public AchievementRewardType rewardType;
    /// <summary>
    /// 도전과제 보상 수량
    /// </summary>
    public int[] rewardAmounts;
}