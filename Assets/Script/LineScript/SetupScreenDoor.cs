using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스크린도어 설치 관리 클래스
/// </summary>
[System.Serializable]
public class SetupScreenDoor : MonoBehaviour
{
    /// <summary>
    /// 구매 확인 메뉴
    /// </summary>
    public GameObject CheckMenu;

    /// <summary>
    /// 구간 그룹 오브젝트
    /// </summary>
    public GameObject sectionGroup;
    /// <summary>
    /// 구간별 노선도 이미지
    /// </summary>
    public GameObject[] sectionImgs;
    /// <summary>
    /// 선택한 구간 텍스트
    /// </summary>
    public Text sectionText;
    /// <summary>
    /// 선택한 구간의 가격 정보 텍스트
    /// </summary>
    public Text priceText;
    /// <summary>
    /// 선택한 구간의 고객 만족도 증가량 텍스트 
    /// </summary>
    public Text pointText;

    /// <summary>
    /// 구간 별 가격 정보 텍스트
    /// </summary>
    public Text[] sectionPriceTexts;

    public MessageManager messageManager;
    public ButtonColorManager buttonColorManager;
    public PriceData priceData;
    public LineCollection lineCollection;
    public LineManager lineManager;
    public LineDataManager lineDataManager;
    public CompanyReputationManager company_Reputation_Controller;
    public condition_Sanitory_Controller condition_Sanitory_Controller;
    public BackgroundImageManager backgroundImageManager;

    /// <summary>
    /// 노선 메뉴 오브젝트
    /// </summary>
    public GameObject lineMenu;

    /// <summary>
    /// 선택한 구간 인덱스
    /// </summary>
    private int targetSection;

    /// <summary>
    /// 구매 효과음
    /// </summary>
    public AudioSource purchaseSound;

    public UpdateDisplay screenDoorUpdateDisplay;

    private void Start()
    {
        if(screenDoorUpdateDisplay != null)
            screenDoorUpdateDisplay.onEnable += SetSectionPriceTexts;
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
    /// 모든 구간에 대해 가격 정보 텍스트 업데이트
    /// </summary>
    private void SetSectionPriceTexts()
    {
        string priceLow = "", priceHigh = "";
        for (int i = 0; i < sectionPriceTexts.Length; i++)
        {
            if (priceData.IsLargeUnit)
                PlayManager.ArrangeUnit(0, priceData.ScreenDoorPrice[i], ref priceLow, ref priceHigh, true);
            else
                PlayManager.ArrangeUnit(priceData.ScreenDoorPrice[i], 0, ref priceLow, ref priceHigh, true);

            sectionPriceTexts[i].text = "가격: " + priceHigh + priceLow + "$";
        }
    }

    /// <summary>
    /// 구매 확인 메뉴 활성화
    /// </summary>
    /// <param name="section">선택한 구간의 인덱스</param>
    public void OpenCheck(int section)
    {
        // 이미 스크린도어가 설치된 구간인지 확인 후, UI 설정 및 활성화
        if (!lineCollection.lineData.installed[section])
        {
            sectionImgs[targetSection].SetActive(false);
            lineManager.currentLine = lineCollection.line;
            targetSection = section;
            sectionGroup.SetActive(true);
            sectionImgs[section].SetActive(true);
            CheckMenu.SetActive(true);
            sectionText.text = priceData.Sections[section].name;
            string m1 = "", m2 = "";
            if(priceData.IsLargeUnit)
                PlayManager.ArrangeUnit(0, priceData.ScreenDoorPrice[section], ref m1, ref m2, true);
            else
                PlayManager.ArrangeUnit(priceData.ScreenDoorPrice[section], 0, ref m1, ref m2, true);
            priceText.text = "비용 : " + m2 + m1 + "$";
            pointText.text = "고객 만족도 + " + string.Format("{0:#,##0}", priceData.ScreenDoorReputation[section] + "P");
        }
        // 이미 설치한 구간이라는 메시지 출력
        else
            AlreadyPurchased();
    }
    /// <summary>
    /// 스크린도어 설치(구매) 처리
    /// </summary>
    public void ScreenDoorPurchase()
    {
        // 구간 별 역 보유 현황 검사
        lineCollection.lineConnection.CheckHasAllStations();

        // 선택한 구간의 모든 역을 보유하고 있는지 확인
        if (lineCollection.lineData.hasAllStations[targetSection])
        {
            // 비용 지불 처리
            bool result;
            if (priceData.IsLargeUnit)
                result = AssetMoneyCalculator.instance.ArithmeticOperation(0, priceData.ScreenDoorPrice[targetSection], false);
            else
                result = AssetMoneyCalculator.instance.ArithmeticOperation(priceData.ScreenDoorPrice[targetSection], 0, false);

            // 비용 지불 처리 성공 시 
            if (result)
            {
                // 구매 완료, 고객 만족도 점수 지급 및 UI 업데이트
                lineCollection.lineData.installed[targetSection] = true;
                company_Reputation_Controller.ReputationValue += priceData.ScreenDoorReputation[targetSection];
                buttonColorManager.SetColorScreenDoor();
                UpdateData();
                DataManager.instance.SaveAll();
                purchaseSound.Play();

                // 해당 노선에서의 첫 구매일 때 스크린도어 배경으로 변경
                if (IsFirstSectionPurchase())
                    backgroundImageManager.ChangeBackground(1);
            }
            // 비용 부족 알림 메시지
            else
                LackOfMoney();
        }
        // 구간의 역이 부족하다는 메시지 출력
        else
            BuyMoreStations();

        // 스크린도어 구매 메뉴 비활성화
        CloseScreenDoor();
    }

    /// <summary>
    /// 해당 노선에서 처음으로 설치하는 구간인지 확인
    /// </summary>
    /// <returns></returns>
    private bool IsFirstSectionPurchase()
    {
        int installedAmount = 0;
        for(int i = 0; i < lineCollection.lineData.installed.Length; i++)
        {
            if (lineCollection.lineData.installed[i])
                installedAmount++;
        }

        if (installedAmount.Equals(1))
            return true;
        else
            return false;
    }

    /// <summary>
    /// 스크린도어 구매 확인 메뉴 종료
    /// </summary>
    public void CloseScreenDoor()
    {
        CheckMenu.SetActive(false);
        sectionImgs[targetSection].SetActive(false);
        sectionGroup.SetActive(false);
    }

    /// <summary>
    /// 고객 만족도 데이터 업데이트
    /// </summary>
    private void UpdateData()
    {
        company_Reputation_Controller.RenewReputation();
        CompanyReputationManager.instance.RenewPassengerBase();
    }

    /// <summary>
    /// 비용 부족 관련 메시지 출력
    /// </summary>
    private void LackOfMoney()
    {
        messageManager.ShowMessage("돈이 부족합니다.");
    }
    /// <summary>
    /// 이미 설치한 구간에 대한 메시지 출력
    /// </summary>
    private void AlreadyPurchased()
    {
        messageManager.ShowMessage("이미 설치한 구간입니다.");
    }
    /// <summary>
    /// 보유한 역 부족 알림 메시지 출력
    /// </summary>
    private void BuyMoreStations()
    {
        messageManager.ShowMessage("해당 구간에 속하는 모든 역을 구입하세요.");
    }
}
