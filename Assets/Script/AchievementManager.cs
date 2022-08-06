using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AchievementRewardType { CardPoint, FeverCoupon, ColorCardPack, RareCardPack }

public class AchievementManager : MonoBehaviour
{
    public Achievement[] achievements;

    public int maxAchvLevel = 5;

    public AchievementData achievementData;

    public int[] AchvClearLevels { set { achievementData.achvClearLevels = value; } get { return achievementData.achvClearLevels; } }
    public LargeVariable MaxMoney { set { achievementData.maxMoney = value; } get { return achievementData.maxMoney; } }
    public int TouchCount { set { achievementData.touchCount = value; } get { return achievementData.touchCount; } }
    public int TotalStationAmount { set { achievementData.totalStationAmount = value; } get { return achievementData.totalStationAmount; } }
    public LargeVariable CumulativeInterest { set { achievementData.cumulativeInterest = value; } get { return achievementData.cumulativeInterest; } }
    public int TotalTrainAmount {
        get
        {
            int count = 0;
            for (int i = 0; i < lineManager.lineCollections.Length; i++)
                count += lineManager.lineCollections[i].lineData.numOfTrain;
            return count;
        }
    }
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

    public GameObject achivementMenu;
    public GameObject alarmObj;

    public LevelManager levelManager;
    public DriversManager driversManager;
    public LineManager lineManager;
    public RentManager rentManager;
    public ItemManager itemManager;
    public MessageManager messageManager;

    struct TargetVariable
    {
        public enum ReturnType { Int, LargeVariable }
        public ReturnType returnType { private set; get; }
        private int targetInt;
        private LargeVariable targetLargeVariable;

        public int TargetInt { set { targetInt = value; returnType = ReturnType.Int; } get { return targetInt; } }
        public LargeVariable TargetLargeVariable { set { targetLargeVariable = value; returnType = ReturnType.LargeVariable; } get { return targetLargeVariable; } }

    }

    void Start()
    {
        if(TotalStationAmount == 0 && MaxMoney == LargeVariable.zero)
            TotalStationAmount = CountTotalStationAmount();
        StartCoroutine(UpdateAlarm());
    }

    public void SetAchivementMenu(bool active)
    {
        if (active)
        {
            UpdateRateAndSliders();
            StartCoroutine(AchivementUpdater());
        }

        achivementMenu.SetActive(active);
    }

    IEnumerator AchivementUpdater()
    {
        yield return new WaitForSeconds(1f);

        UpdateRateAndSliders();

        if(achivementMenu.activeInHierarchy)
            StartCoroutine(AchivementUpdater());
    }

    IEnumerator UpdateAlarm()
    {
        yield return new WaitForSeconds(3);

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

    public void UpdateRateAndSliders()
    {
        for(int i = 0; i < achievements.Length; i++)
        {
            if (AchvClearLevels[i] < maxAchvLevel)
            {
                TargetVariable criteriaVariable = GetCriteriaAmount(i);
                TargetVariable currentVariable = GetTargetVariable(i);
                if (criteriaVariable.returnType == TargetVariable.ReturnType.Int)
                {
                    achievements[i].achivementSlider.maxValue = achievements[i].criteriaInt[AchvClearLevels[i]];
                    achievements[i].achivementSlider.value = currentVariable.TargetInt;
                    string currentVal = string.Format("{0:#,##0}", currentVariable.TargetInt), criteriaVal = string.Format("{0:#,##0}", achievements[i].criteriaInt[AchvClearLevels[i]]);
                    achievements[i].rateText.text =currentVal + "/" + criteriaVal;
                }
                else
                {
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
            else
            {
                achievements[i].achivementSlider.value = achievements[i].achivementSlider.maxValue;
                achievements[i].rateText.text = "Clear!";
                achievements[i].receiveRewardButton.gameObject.SetActive(false);
            }
        }
    }
    
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

    private TargetVariable GetCriteriaAmount(int index)
    {
        TargetVariable targetVariable = new TargetVariable();
        if (achievements[index].criteriaInt.Length > 0)
            targetVariable.TargetInt = achievements[index].criteriaInt[AchvClearLevels[index]];
        else
            targetVariable.TargetLargeVariable = achievements[index].criteriaLargeVariable[AchvClearLevels[index]];
        return targetVariable;
    }

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

[System.Serializable]
public class Achievement
{
    public Text rateText;
    public Slider achivementSlider;
    public Button receiveRewardButton;
    public Text rewardAmountText;

    public int[] criteriaInt;
    public LargeVariable[] criteriaLargeVariable;
    public AchievementRewardType rewardType;
    public int[] rewardAmounts;
}