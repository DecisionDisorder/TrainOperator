using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �������� ���� ����
/// </summary>
public enum AchievementRewardType { CardPoint, FeverCoupon, ColorCardPack, RareCardPack }
/// <summary>
/// �������� �ý��� ���� Ŭ����
/// </summary>

public class AchievementManager : MonoBehaviour
{
    /// <summary>
    /// �������� ������ ������Ʈ �迭
    /// </summary>
    public Achievement[] achievements;

    /// <summary>
    /// �ִ� �޼� ����
    /// </summary>
    public int maxAchvLevel = 5;

    /// <summary>
    /// �������� ���� ������ ������Ʈ
    /// </summary>
    public AchievementData achievementData;

    /// <summary>
    /// �� ���������� ���� ����
    /// </summary>
    public int[] AchvClearLevels { set { achievementData.achvClearLevels = value; } get { return achievementData.achvClearLevels; } }
    /// <summary>
    /// ����ڰ� �����ߴ� �ִ��� ��
    /// </summary>
    public LargeVariable MaxMoney { set { achievementData.maxMoney = value; } get { return achievementData.maxMoney; } }
    /// <summary>
    /// ����ڰ� ��ġ�޴��� ��ġ�� Ƚ��
    /// </summary>
    public int TouchCount { set { achievementData.touchCount = value; } get { return achievementData.touchCount; } }
    /// <summary>
    /// ����ڰ� ������ ���� ����
    /// </summary>
    public int TotalStationAmount { set { achievementData.totalStationAmount = value; } get { return achievementData.totalStationAmount; } }
    /// <summary>
    /// ����ڰ� ȹ���� ���� ����
    /// </summary>
    public LargeVariable CumulativeInterest { set { achievementData.cumulativeInterest = value; } get { return achievementData.cumulativeInterest; } }
    /// <summary>
    /// ����ڰ� ������ ��� ������ ����
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
    /// ����ڰ� ������ ��� ������� ��
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
    /// ����ڰ� Ȯ���� ���� ĭ�� ����
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
    /// ����ڰ� ������ �Ӵ� �ü��� ����
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
    /// �������� �޴� ������Ʈ
    /// </summary>
    public GameObject achivementMenu;
    /// <summary>
    /// �������� �˸� �̹��� ������Ʈ
    /// </summary>
    public GameObject alarmObj;

    public LevelManager levelManager;
    public DriversManager driversManager;
    public LineManager lineManager;
    public RentManager rentManager;
    public ItemManager itemManager;
    public MessageManager messageManager;

    /// <summary>
    /// �������� ���� ��ġ�� ������ ��� ����ü
    /// </summary>
    struct TargetVariable
    {
        /// <summary>
        /// ���� ���� ��ġ�� Ÿ�� ����
        /// </summary>��
        public enum ReturnType { Int, LargeVariable }
        /// <summary>
        /// ���� ���� ��ġ�� Ÿ��
        /// </summary>
        public ReturnType returnType { private set; get; }


        private int targetInt;
        /// <summary>
        /// ��ǥ ��ġ(int)
        /// </summary>
        public int TargetInt { set { targetInt = value; returnType = ReturnType.Int; } get { return targetInt; } }

        private LargeVariable targetLargeVariable;
        /// <summary>
        /// ��ǥ ��ġ(LargeVariable)
        /// </summary>
        public LargeVariable TargetLargeVariable { set { targetLargeVariable = value; returnType = ReturnType.LargeVariable; } get { return targetLargeVariable; } }

    }

    void Start()
    {
        // ���� ������ �ѹ��� ������� ���� ���, ���
        if(TotalStationAmount == 0 && MaxMoney == LargeVariable.zero)
            TotalStationAmount = CountTotalStationAmount();
        // �˶� �������� �ڷ�ƾ ����
        StartCoroutine(UpdateAlarm());
    }

    /// <summary>
    /// �������� �޴� Ȱ��ȭ/��Ȱ��ȭ
    /// </summary>
    /// <param name="active">Ȱ��ȭ ����</param>
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
    /// �������� UI ������Ʈ �ڷ�ƾ
    /// </summary>
    IEnumerator AchivementUpdater()
    {
        yield return new WaitForSeconds(1f);

        // �������� UI�� Ȱ��ȭ �Ǿ����� �� �������� ������� �ݿ� (1�ʸ��� �˻�)
        UpdateRateAndSliders();

        if(achivementMenu.activeInHierarchy)
            StartCoroutine(AchivementUpdater());
    }

    /// <summary>
    /// �������� ���� �˶� ������Ʈ �ڷ�ƾ
    /// </summary>
    IEnumerator UpdateAlarm()
    {
        yield return new WaitForSeconds(3);

        // �ϳ��� �������� ������ ���� ������ �Ǹ� �˶� ������Ʈ�� Ȱ��ȭ (3�ʸ��� �˻�)
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
    /// �������� ���� ����
    /// </summary>
    /// <param name="index">�������� �ε���</param>
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
            messageManager.ShowMessage("���� �޼� ���ǿ� �������� ���߽��ϴ�.");
    }
    /// <summary>
    /// �������� ���� ������ �����̴� ������Ʈ
    /// </summary>
    public void UpdateRateAndSliders()
    {
        // ��� �������� �׸� ����
        for(int i = 0; i < achievements.Length; i++)
        {
            // �ִ� ������ ���� �ʾ�����
            if (AchvClearLevels[i] < maxAchvLevel)
            {
                // ���ذ��� ���� ���� ���Ͽ�
                TargetVariable criteriaVariable = GetCriteriaAmount(i);
                TargetVariable currentVariable = GetTargetVariable(i);
                // ���� ���� Ÿ���� int�� �� 
                if (criteriaVariable.returnType == TargetVariable.ReturnType.Int)
                {
                    // �����̴��� �ִ� ��, ���� ���� ��� �Ͽ� ������ ������Ʈ �Ѵ�
                    achievements[i].achivementSlider.maxValue = achievements[i].criteriaInt[AchvClearLevels[i]];
                    achievements[i].achivementSlider.value = currentVariable.TargetInt;
                    string currentVal = string.Format("{0:#,##0}", currentVariable.TargetInt), criteriaVal = string.Format("{0:#,##0}", achievements[i].criteriaInt[AchvClearLevels[i]]);
                    achievements[i].rateText.text =currentVal + "/" + criteriaVal;
                }
                // ���� ���� Ÿ���� LargeVariable �� ��
                else
                {
                    // ���� high/low�� ����� ���� ����Ͽ� ������ �����̴��� ������Ʈ �Ѵ�.
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
            // �ִ� ������ �Ѿ����� Ŭ���� �ߴٴ� ������ ������Ʈ �Ѵ�.
            else
            {
                achievements[i].achivementSlider.value = achievements[i].achivementSlider.maxValue;
                achievements[i].rateText.text = "Clear!";
                achievements[i].receiveRewardButton.gameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// Ư�� ���������� ���� ���� ��Ȳ ���� ã�Ƽ� �����Ѵ�.
    /// </summary>
    /// <param name="index">��� �������� �ε���</param>
    /// <returns>���� ���� ��Ȳ ��</returns>
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
    /// Ư�� ���������� ��ǥ ���� ã�Ƽ� ����
    /// </summary>
    /// <param name="index">�������� �ε���</param>
    /// <returns>��ǥ ��</returns>
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
    /// Ư�� ���������� �������� ���� ���� ���ؿ� �����ϴ��� Ȯ��
    /// </summary>
    /// <param name="index">�������� �ε���</param>
    /// <returns>���� ���� ����</returns>
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
    /// ����ڰ� ������ ���� ���� ���
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
/// �������� Ŭ����
/// </summary>
[System.Serializable]
public class Achievement
{
    /// <summary>
    /// ���� ���� �ؽ�Ʈ
    /// </summary>
    public Text rateText;
    /// <summary>
    /// ���뵵 �����̴�(���α׷��� ��)
    /// </summary>
    public Slider achivementSlider;
    /// <summary>
    /// ���� ���� ��ư
    /// </summary>
    public Button receiveRewardButton;
    /// <summary>
    /// ���� ���� ��Ʈ
    /// </summary>
    public Text rewardAmountText;

    /// <summary>
    /// ���� ���� ��(int)
    /// </summary>
    public int[] criteriaInt;
    /// <summary>
    /// ���� ���� ��(LargeVariable)
    /// </summary>
    public LargeVariable[] criteriaLargeVariable;
    /// <summary>
    /// �������� ���� Ÿ��
    /// </summary>
    public AchievementRewardType rewardType;
    /// <summary>
    /// �������� ���� ����
    /// </summary>
    public int[] rewardAmounts;
}