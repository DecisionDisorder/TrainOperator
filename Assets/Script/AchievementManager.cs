using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AchievementRewardType { CardPoint, FeverCoupon, ColorCardPack, RareCardPack }

public class AchievementManager : MonoBehaviour
{
    public Achievement[] achievements;

    public int maxAchvLevel = 5;

    public int[] achvClearLevels = new int[9];
    public LargeVariable maxMoney;
    public int touchCount;
    public int totalStationAmount;
    public LargeVariable cumulativeInterest;
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
        totalStationAmount = CountTotalStationAmount(); // TODO: 최초 1회만
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

    public void ReceiveReward(int index)
    {
        if (CheckMeetConditions(index))
        {
            if (achievements[index].rewardType == AchievementRewardType.CardPoint)
            {
                itemManager.CardPoint += achievements[index].rewardAmounts[achvClearLevels[index]];
            }
            else if (achievements[index].rewardType == AchievementRewardType.FeverCoupon)
            {
                // 피버 충전 쿠폰 구현 후 추가 += achievements[index].rewardAmounts[achvClearLevels[index]];
            }
            else if (achievements[index].rewardType == AchievementRewardType.ColorCardPack)
            {
                itemManager.ColorPackAmount += achievements[index].rewardAmounts[achvClearLevels[index]];
            }
            else if (achievements[index].rewardType == AchievementRewardType.RareCardPack)
            {
                itemManager.RarePackAmount += achievements[index].rewardAmounts[achvClearLevels[index]];
            }
            achvClearLevels[index]++;
            UpdateRateAndSliders();
        }
        else
            messageManager.ShowMessage("아직 달성 조건에 도달하지 못했습니다.");
    }

    public void UpdateRateAndSliders()
    {
        for(int i = 0; i < achievements.Length; i++)
        {
            if (achvClearLevels[i] < maxAchvLevel)
            {
                TargetVariable criteriaVariable = GetCriteriaAmount(i);
                TargetVariable currentVariable = GetTargetVariable(i);
                if (criteriaVariable.returnType == TargetVariable.ReturnType.Int)
                {
                    achievements[i].achivementSlider.maxValue = achievements[i].criteriaInt[achvClearLevels[i]];
                    achievements[i].achivementSlider.value = currentVariable.TargetInt;
                    achievements[i].rateText.text = currentVariable.TargetInt + "/" + achievements[i].criteriaInt[achvClearLevels[i]];
                }
                else
                {
                    if (achievements[i].criteriaLargeVariable[achvClearLevels[i]].highUnit == 0)
                    {
                        achievements[i].achivementSlider.maxValue = achievements[i].criteriaLargeVariable[achvClearLevels[i]].lowUnit;
                        achievements[i].achivementSlider.value = currentVariable.TargetLargeVariable.lowUnit;
                    }
                    else
                    {
                        achievements[i].achivementSlider.maxValue = achievements[i].criteriaLargeVariable[achvClearLevels[i]].highUnit;
                        achievements[i].achivementSlider.value = currentVariable.TargetLargeVariable.highUnit;
                    }
                    string current = PlayManager.GetSimpleUnit(currentVariable.TargetLargeVariable.lowUnit, currentVariable.TargetLargeVariable.highUnit);
                    string criteria = PlayManager.GetSimpleUnit(criteriaVariable.TargetLargeVariable.lowUnit, criteriaVariable.TargetLargeVariable.highUnit);
                    achievements[i].rateText.text = current + "/" + criteria;
                }
                achievements[i].rewardAmountText.text = achievements[i].rewardAmounts[achvClearLevels[i]].ToString();
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
                if (MyAsset.instance.Money > maxMoney)
                    maxMoney = MyAsset.instance.Money;
                targetVariable.TargetLargeVariable = maxMoney;
                break;
            case 1:
                targetVariable.TargetInt = touchCount;
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
                targetVariable.TargetInt = totalStationAmount;
                break;
            case 6:
                targetVariable.TargetInt = TotalTrainExpandAmount;
                break;
            case 7:
                targetVariable.TargetInt = TotalRentAmount;
                break;
            case 8:
                targetVariable.TargetLargeVariable = cumulativeInterest;
                break;
        }

        return targetVariable;
    }

    private TargetVariable GetCriteriaAmount(int index)
    {
        TargetVariable targetVariable = new TargetVariable();
        if (achievements[index].criteriaInt.Length > 0)
            targetVariable.TargetInt = achievements[index].criteriaInt[achvClearLevels[index]];
        else
            targetVariable.TargetLargeVariable = achievements[index].criteriaLargeVariable[achvClearLevels[index]];
        return targetVariable;
    }

    private bool CheckMeetConditions(int index)
    {
        TargetVariable targetVariable = GetTargetVariable(index);
        if(targetVariable.returnType == TargetVariable.ReturnType.Int)
        {
            if (targetVariable.TargetInt >= achievements[index].criteriaInt[achvClearLevels[index]])
                return true;
            else
                return false;
        }
        else
        {
            if (targetVariable.TargetLargeVariable >= achievements[index].criteriaLargeVariable[achvClearLevels[index]])
                return true;
            else
                return false;
        }
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