using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 경전철 관리 시스템 관리 클래스
/// </summary>
public class LightRailControlManager : MonoBehaviour
{
    /// <summary>
    /// 열차 확장 메뉴 오브젝트
    /// </summary>
    public GameObject trainExpandMenu;
    /// <summary>
    /// 노선 업그레이드 메뉴 오브젝트
    /// </summary>
    public GameObject upgradeLineMenu;

    /// <summary>
    /// 특정 노선 업그레이드 메뉴 오브젝트
    /// </summary>
    public GameObject upgradeMenu;
    /// <summary>
    /// 업그레이드 대상 노선 텍스트
    /// </summary>
    public Text targetLineText;

    /// <summary>
    /// 항목 별 업그레이드 단계를 나타내는 슬라이더
    /// </summary>
    public Slider[] upgradeSliders;
    /// <summary>
    /// 항목 별 업그레이드 비용 텍스트
    /// </summary>
    public Text[] upgradePriceTexts;
    /// <summary>
    /// 항목 별 업그레이드 보상(승객 수) 텍스트
    /// </summary>
    public Text[] upgradePassengerTexts;

    /// <summary>
    /// 항목별 비용/보상 비율
    /// </summary>

    public float[] division = { 0.1f, 0.15f, 0.2f, 0.25f, 0.3f};
    
    /// <summary>
    /// 대상 경전철 노선
    /// </summary>

    private LightRailLine targetLine;

    /// <summary>
    /// 업그레이드 최대 레벨
    /// </summary>
    public const int maxLevel = 5;

    public PriceData[] lightRailPriceData;
    public LineManager lineManager;
    public MessageManager messageManager;

    /// <summary>
    /// 열차 확장 메뉴 활성화/비활성화
    /// </summary>
    /// <param name="active">활성화 여부</param>
    public void SetTrainExpandMenu(bool active)
    {
        trainExpandMenu.SetActive(active);
    }
    /// <summary>
    /// 노선 업그레이드 메뉴 활성화/비활성화
    /// </summary>
    /// <param name="active">활성화 여부</param>
    public void SetUpgradeLineMenu(bool active)
    {
        upgradeLineMenu.SetActive(active);
    }

    /// <summary>
    /// 업그레이드 대상 노선 선택
    /// </summary>
    /// <param name="index">경전철 노선 인덱스</param>
    public void SelectTargetLine(int index)
    {
        targetLine = (LightRailLine)index;
        targetLineText.text = "대상 노선: " + lineManager.lineCollections[GetEntireLineIndex((LightRailLine)index)].purchaseTrain.lineName;
        UpdateUpgradeState();
        upgradeMenu.SetActive(true);
    }

    /// <summary>
    /// 업그레이드 메뉴 비활성화
    /// </summary>
    public void CloseUpgradeMenu()
    {
        upgradeMenu.SetActive(false);
    }

    /// <summary>
    /// 업그레이드 상태 갱신
    /// </summary>
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
    
    /// <summary>
    /// 제어 시스템 업그레이드
    /// </summary>
    /// <param name="product">업그레이드 할 상품</param>
    public void UpgradeControlSystem(int product)
    {
        // 업그레이드 할 상품이 최대 레벨을 넘지 않으면
        if (GetLineControlLevel(product) < maxLevel)
        {
            // 비용 지불 처리
            if (AssetMoneyCalculator.instance.ArithmeticOperation(GetPrice(product), false))
            {
                // 업그레이드 보상 지급 처리
                LargeVariable passenger = GetPassenger(product);
                TouchMoneyManager.ArithmeticOperation(passenger.lowUnit, passenger.highUnit, true);
                CompanyReputationManager.instance.RenewPassengerBase();
                AddLineControlLevel(product);
                UpdateUpgradeState();
            }
            // 비용 부족 메시지 출력
            else
                messageManager.ShowMessage("돈이 부족하여 업그레이드할 수 없습니다.");
        }
        // 최대 업그레이드 레벨 메시지 출력
        else
        {
            messageManager.ShowMessage("최대 업그레이드 레벨입니다.");
        }
    }
    
    /// <summary>
    /// 업그레이드 비용 찾기
    /// </summary>
    /// <param name="product">대상 상품</param>
    /// <returns>업그레이드 비용</returns>
    private LargeVariable GetPrice(int product)
    {
        int level = GetLineControlLevel(product);
        return GetPrice(product, level);
    }

    /// <summary>
    /// 특정 레벨에 대한 업그레이드 비용 찾기
    /// </summary>
    /// <param name="product">대상 상품</param>
    /// <param name="level">대상 레벨</param>
    /// <returns>업그레이드 비용</returns>
    public LargeVariable GetPrice(int product, int level)
    {
        if (level < 5)
        {
            LargeVariable variable = lightRailPriceData[(int)targetLine].LineControlUpgradePrice[product] * division[level];
            variable.detailed = true;
            return variable;
        }
        else
            return LargeVariable.zero;
    }

    /// <summary>
    /// 업그레이드에 대한 승객 수 보상 계산
    /// </summary>
    /// <param name="product">대상 상품</param>
    /// <returns>승객 수</returns>
    private LargeVariable GetPassenger(int product)
    {
        int level = GetLineControlLevel(product);
        return GetPassenger(product, level);
    }

    /// <summary>
    /// 특정 레벨에 대해 업그레이드 승객 수 보상 계산
    /// </summary>
    /// <param name="product">대상 상품</param>
    /// <param name="level">승객 수</param>
    /// <returns></returns>
    public LargeVariable GetPassenger(int product, int level)
    {
        if (level < 5)
        {
            LargeVariable variable = lightRailPriceData[(int)targetLine].LineControlUpgradePassenger[product] * division[level];
            variable.detailed = true;
            return variable;
        }
        else
            return LargeVariable.zero;
    }

    /// <summary>
    /// 특정 상품의 업그레이드 레벨 찾기
    /// </summary>
    /// <param name="product">대상 상품</param>
    /// <returns></returns>
    public int GetLineControlLevel(int product)
    {
        return lineManager.lineCollections[GetEntireLineIndex(targetLine)].LineControlLevels[product];
    }

    /// <summary>
    /// 특정 상품의 업그레이드 레벨 증가
    /// </summary>
    /// <param name="product">대상 상품</param>
    private void AddLineControlLevel(int product)
    {
        lineManager.lineCollections[GetEntireLineIndex(targetLine)].lineData.lineControlLevels[product]++;
    }

    /// <summary>
    /// 경전철의 전체 노선상에서의 노선 인덱스
    /// </summary>
    /// <param name="lightRailIndex">경전철 상의 노선 인덱스</param>
    /// <returns>전체 노선 인덱스</returns>
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

    /// <summary>
    /// 총합 레벨 계산
    /// </summary>
    /// <param name="lineIndex">전체 노선 인덱스</param>
    /// <returns></returns>
    public int GetTotalLevel(int lineIndex)
    {
        int level = 0;
        for (int i = 0; i < lineManager.lineCollections[lineIndex].LineControlLevels.Length; i++)
            level += lineManager.lineCollections[lineIndex].LineControlLevels[i];
        return level;
    }
}
