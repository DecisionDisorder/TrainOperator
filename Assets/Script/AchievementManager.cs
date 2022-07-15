using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AchievementRewardType { CardPoint, FeverCoupon, ColorCardPack, RareCardPack }

public class AchievementManager : MonoBehaviour
{
    public Achievement[] achievements;

    void Start()
    {
        
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