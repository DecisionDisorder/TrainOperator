using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightRailControlManager : MonoBehaviour
{
    public GameObject trainExpandMenu;
    public GameObject upgradeLineMenu;

    public GameObject upgradeMenu;
    public Text targetLineText;

    public Slider[] upgradeSliders;
    public Text[] upgradePriceTexts;
    public Text[] upgradePassengerTexts;

    public float[] division = { 0.1f, 0.15f, 0.2f, 0.25f, 0.3f};

    private LightRailLine targetLine;

    public int maxLevel = 5;

    public PriceData[] lightRailPriceData;
    public LineManager lineManager;
    public MessageManager messageManager;

    public void SetTrainExpandMenu(bool active)
    {
        trainExpandMenu.SetActive(active);
    }
    public void SetUpgradeLineMenu(bool active)
    {
        upgradeLineMenu.SetActive(active);
    }

    public void SelectTargetLine(int index)
    {
        targetLine = (LightRailLine)index;
        targetLineText.text = "대상 노선: " + lineManager.lineCollections[GetEntireLineIndex((LightRailLine)index)].purchaseTrain.lineName;
        UpdateUpgradeState();
        upgradeMenu.SetActive(true);
    }

    private void UpdateUpgradeState()
    {
        for(int i = 0; i < upgradeSliders.Length; i++)
        {
            int level = GetLineControlLevel(i);
            upgradeSliders[i].value = level;

            if (level < maxLevel)
            {
                upgradePriceTexts[i].text = "가격: " + GetPrice(i).ToString() + "$";
                upgradePassengerTexts[i].text = "승객 수 +" + GetPassenger(i).ToString() + "명";
            }
            else
            {
                upgradePriceTexts[i].text = "";
                upgradePassengerTexts[i].text = "";
            }
        }
    }
    
    public void UpgradeControlSystem(int product)
    {
        if (GetLineControlLevel(product) < maxLevel)
        {
            if (AssetMoneyCalculator.instance.ArithmeticOperation(GetPrice(product), false))
            {
                AddLineControlLevel(product);
                UpdateUpgradeState();
            }
            else
                messageManager.ShowMessage("돈이 부족하여 업그레이드할 수 없습니다.");
        }
        else
        {
            messageManager.ShowMessage("최대 업그레이드 레벨입니다.");
        }
    }
    
    private LargeVariable GetPrice(int product)
    {
        int level = GetLineControlLevel(product);
        if (level < 5)
            return lightRailPriceData[(int)targetLine].LineControlUpgradePrice[product] * division[level];
        else
            return LargeVariable.zero;
    }

    private LargeVariable GetPassenger(int product)
    {
        int level = GetLineControlLevel(product);
        if (level < 5)
            return lightRailPriceData[(int)targetLine].LineControlUpgradePassenger[product] * division[level];
        else
            return LargeVariable.zero;
    }

    private int GetLineControlLevel(int product)
    {
        return lineManager.lineCollections[GetEntireLineIndex(targetLine)].lineData.lineControlLevels[product];
    }

    private void AddLineControlLevel(int product)
    {
        lineManager.lineCollections[GetEntireLineIndex(targetLine)].lineData.lineControlLevels[product]++;
    }

    private int GetEntireLineIndex(LightRailLine lightRailIndex)
    {
        for(int i = 0; i < lineManager.lineCollections.Length; i++)
        {
            if (((Line)i).ToString().Equals(lightRailIndex.ToString()))
            {
                return i;
            }
        }

        return -1;
    }
}
