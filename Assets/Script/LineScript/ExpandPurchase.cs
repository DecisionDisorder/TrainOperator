using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 노선 확장권 구매 관리 클래스
/// </summary>
[System.Serializable]
public class ExpandPurchase : MonoBehaviour
{
    public MessageManager messageManager;
    public ButtonColorManager buttonColorManager;

    /// <summary>
    /// 일반 노선 구매 가능 기준 값 (직전 일반 노선의 열차 보유 개수)
    /// </summary>
    private int normalLineCriteria = 100;
    /// <summary>
    /// 경전철 노선 구매 가능 기준 값 (직전 경전철 노선의 열차 보유 개수)
    /// </summary>
    private int lightRailCriteria = 25;

    /// <summary>
    /// 노선 메뉴 오브젝트
    /// </summary>
    public GameObject lineMenu;

    /// <summary>
    /// 구간 그룹 오브젝트
    /// </summary>
    public GameObject sectionGroup;
    /// <summary>
    /// 구간 별 노선도 이미지 오브젝트
    /// </summary>
    public GameObject[] sectionImgs;
    /// <summary>
    /// 열차 부족 안내 오브젝트
    /// </summary>
    public GameObject[] LessTrainImages;
    /// <summary>
    /// 구간 별 가격 텍스트 배열
    /// </summary>
    public Text[] sectionPriceTexts;
    /// <summary>
    /// 구간 별 구매 버튼 배열
    /// </summary>
    public Button[] purchaseButtons;


    public PriceData priceData;
    public LineCollection lineCollection;
    public LineManager lineManager;
    public LineDataManager lineDataManager;
    public SettingManager buttonOption;
    public StationCustomizeManager stationCustomizeManager;
    public BackgroundImageManager backgroundImageManager;
    public UpdateDisplay expandUpdateDisplay;

    /// <summary>
    /// 구매 대상 구간
    /// </summary>
    private int targetSection = -1;

    /// <summary>
    /// 구매 효과음
    /// </summary>
    public AudioSource purchaseSound;

    private void Start()
    {
        expandUpdateDisplay.onEnable += SetSectionPriceTexts;
    }

    /// <summary>
    /// 노선 메뉴 활성화/비활성화
    /// </summary>
    /// <param name="onOff">활성화 여부</param>
    public void SetLineMenu(bool onOff)
    {
        lineMenu.SetActive(onOff);
    }
    /// <summary>
    /// 각 구간별 가격 표시
    /// </summary>
    private void SetSectionPriceTexts()
    {
        string priceLow = "", priceHigh = "";
        for (int i = 0; i < sectionPriceTexts.Length; i++)
        {
            if (sectionPriceTexts[i] != null)
            {
                if (priceData.IsLargeUnit)
                    PlayManager.ArrangeUnit(0, priceData.ExpandPrice[i], ref priceLow, ref priceHigh);
                else
                    PlayManager.ArrangeUnit(priceData.ExpandPrice[i], 0, ref priceLow, ref priceHigh);

                sectionPriceTexts[i].text = "가격: " + priceHigh + priceLow + "$";
            }
        }
    }

    /// <summary>
    /// 특정 구간 구매 확인 메뉴 열기
    /// </summary>
    /// <param name="section">구간 인덱스</param>
    public void OpenSection(int section)
    {
        // 이미 구매한 구간인지 확인 후 구매 메뉴 활성화
        if (!lineCollection.lineData.sectionExpanded[section])
        {
            if(targetSection != -1)
                sectionImgs[targetSection].SetActive(false);
            lineManager.currentLine = lineCollection.line;
            targetSection = section;
            sectionGroup.SetActive(true);
            sectionImgs[section].SetActive(true);
        }
        else
            AlreadyPurchased();
    }

    /// <summary>
    /// 직전 노선의 종류를 파악하여 열차 보유량 기준 값 반환
    /// </summary>
    private int GetTrainAmountCriteria()
    {
        return lineManager.lineCollections[(int)lineCollection.line - 1].expandPurchase.priceData.IsLightRail ? lightRailCriteria : normalLineCriteria;
    }

    /// <summary>
    /// 확장권 구매 자격이 되는지 확인
    /// </summary>
    public void SetQualification()
    {
        // 1호선을 제외하고 구매 자격 확인
        if (!lineCollection.line.Equals(Line.Line1))
        {
            if (lineManager.lineCollections[(int)lineCollection.line - 1].lineData.numOfTrain < GetTrainAmountCriteria())
            {
                SetLessTrainImgs(true);
            }
            else
            {
                SetLessTrainImgs(false);
            }
        }
    }

    /// <summary>
    /// 열차 부족 안내 UI 활성화/비활성화
    /// </summary>
    /// <param name="onOff">활성화 여부</param>
    private void SetLessTrainImgs(bool onOff)
    {
        for (int i = 0; i < LessTrainImages.Length; i++)
        {
            LessTrainImages[i].SetActive(onOff);
            purchaseButtons[i].enabled = !onOff;
        }
    }

    /// <summary>
    /// 구매 확인/취소 처리
    /// </summary>
    /// <param name="accept">구매 여부</param>
    public void CheckConfirm(bool accept)
    {
        if (accept)
            BuyExpand();
        sectionGroup.SetActive(false);
        sectionImgs[targetSection].SetActive(false);
        targetSection = -1;
    }

    /// <summary>
    /// 해당 노선을 처음으로 확장하는 것인지 확인
    /// </summary>
    /// <returns></returns>
    private bool IsFirstExpand()
    {
        for (int i = 0; i < lineCollection.lineData.sectionExpanded.Length; i++)
            if (lineCollection.lineData.sectionExpanded[i])
                return false;

        return true;
    }

    /// <summary>
    /// 확장권 구매 처리
    /// </summary>
    private void BuyExpand()
    {
        // 비용 지불
        bool result;
        if (priceData.IsLargeUnit)
            result = AssetMoneyCalculator.instance.ArithmeticOperation(0, priceData.ExpandPrice[targetSection], false);
        else
            result = AssetMoneyCalculator.instance.ArithmeticOperation(priceData.ExpandPrice[targetSection], 0, false);

        // 비용 지불이 성공하면
        if (result)
        {
            // 처음 확장할 때 백업 추천 UI를 띄운다.
            bool firstExpand = IsFirstExpand();
            if (firstExpand && buttonOption.BackupRecommend)
            {
                messageManager.OpenCommonCheckMenu("클라우드 백업하러 가시겠습니까?", "신규 노선의 확장권을 구입하셨습니다.\n혹시 모를 상황을 대비하여 클라우드 백업을 권장합니다.", Color.cyan, OpenCloud);
            }
            
            // 처음 확장할 때 승강장 배경을 새로운 노선으로 변경
            if(firstExpand)
            {
                try
                {
                    backgroundImageManager.ChangeBackground(0);
                    stationCustomizeManager.SetBackgroundToRecentLine(lineCollection.line);
                    messageManager.ShowMessage("새로운 노선의 배경으로 변경되었습니다!\n배경 노선/역을 변경하려면\n\"설정-노선/역 설정\"을 이용해주세요!", 2f);
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                    messageManager.ShowMessage("일시적인 오류로 인해 노선 배경이 변경되지 않았습니다.\n배경 노선/역을 변경하려면\n\"설정-노선/역 설정\"을 이용해주세요!", 2f);
                }
            }

            // 2호선일 때 모든 구간의 확장권을 구매 처리
            if(lineCollection.line.Equals(Line.Line2))
            {
                messageManager.ShowMessage("2호선 전체 확장권을 구입하였습니다.");
                for(int i = 0; i < lineCollection.lineData.sectionExpanded.Length; i++)
                    lineCollection.lineData.sectionExpanded[i] = true;
            }
            // 아닐땐 각자 구매 처리
            else
            {
                messageManager.ShowMessage(priceData.Sections[targetSection].name + " 노선 확장권을 구입하였습니다.");
                lineCollection.lineData.sectionExpanded[targetSection] = true;
            }
            // 구매한 확장권 버튼의 색상을 바꾸고 저장 및 구매 처리 효과음 출력
            buttonColorManager.SetColorExpand(targetSection);
            DataManager.instance.SaveAll();
            purchaseSound.Play();
        }
        // 돈이 부족하여 비용 지불이 실패할 경우
        else
            LackOfMoney();
    }

    /// <summary>
    /// 클라우드 백업 메뉴 활성화
    /// </summary>
    private void OpenCloud()
    {
        CloudSubManager.instance.SetCloudMenu(true);
    }

    /// <summary>
    /// 이미 구매한 구간이라는 안내 메시지 출력
    /// </summary>
    private void AlreadyPurchased()
    {
        messageManager.ShowMessage("이미 구매하셨습니다.");
    }

    /// <summary>
    /// 돈이 부족하다느 안내 메시지 출력
    /// </summary>
    private void LackOfMoney()
    {
        messageManager.ShowMessage("돈이 부족합니다.");
    }
}
