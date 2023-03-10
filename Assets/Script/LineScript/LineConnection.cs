using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 노선 연결 시스템 관리 클래스
/// </summary>
[System.Serializable]
public class LineConnection: MonoBehaviour
{
    /// <summary>
    /// 구매 확인 클래스
    /// </summary>
    public GameObject checkMenu;
    /// <summary>
    /// 구간 그룹 오브젝트
    /// </summary>
    public GameObject sectionGroup;
    /// <summary>
    /// 구간 별 노선도 이미지
    /// </summary>
    public GameObject[] sectionImages;
    /// <summary>
    /// 선택한 구간 이름 텍스트
    /// </summary>
    public Text section_text;
    /// <summary>
    /// 선택한 구간의 가격 텍스트
    /// </summary>
    public Text price_text;
    /// <summary>
    /// 선택한 구간의 보상 텍스트 (이름은 passenger지만, 시간형 수익 보상으로 변경됨)
    /// </summary>
    public Text passenger_text;

    /// <summary>
    /// 구간별 가격 정보 텍스트
    /// </summary>
    public Text[] sectionPriceTexts;

    /// <summary>
    /// 노선 메뉴 오브젝트
    /// </summary>
    public GameObject lineMenu;

    public ButtonColorManager buttonColorManager;
    public MessageManager messageManager;
    public PriceData priceData;
    public LineCollection lineCollection;
    public LineManager lineManager;
    public LineDataManager lineDataManager;
    public UpdateDisplay connectionUpdateDisplay;

    /// <summary>
    /// 선택한 구간 인덱스
    /// </summary>
    private int targetSection;
    /// <summary>
    /// 구매 효과음
    /// </summary>
    public AudioSource purchaseSound;

    private void Start()
    {
        if (connectionUpdateDisplay != null)
            connectionUpdateDisplay.onEnable += SetSectionPriceTexts;
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
    /// 모든 구간 가격 텍스트 정보 업데이트
    /// </summary>
    private void SetSectionPriceTexts()
    {
        string priceLow = "", priceHigh = "";
        for (int i = 0; i < sectionPriceTexts.Length; i++)
        {
            if (priceData.IsLargeUnit)
                PlayManager.ArrangeUnit(0, priceData.ConnectPrice[i], ref priceLow, ref priceHigh);
            else
                PlayManager.ArrangeUnit(priceData.ConnectPrice[i], 0, ref priceLow, ref priceHigh);

            sectionPriceTexts[i].text = "가격: " + priceHigh + priceLow + "$";
        }
    }

    /// <summary>
    /// 노선 연결 확인 메뉴를 활성화한다.
    /// </summary>
    /// <param name="sectionIndex">선택한 구간 인덱스</param>
    public void OpenConnectionCheck(int sectionIndex)
    {
        // 연결 안된 노선인 경우 구매 정보 텍스트를 업데이트하여 메뉴를 활성화한다.
        if (!lineCollection.lineData.connected[sectionIndex])
        {
            sectionImages[targetSection].SetActive(false);
            targetSection = sectionIndex;
            lineManager.currentLine = lineCollection.line;

            string priceLow = "", priceHigh = "", timeMoneyLow = "", timeMoneyHigh = "";
            if(priceData.IsLargeUnit)
                PlayManager.ArrangeUnit(0, priceData.ConnectPrice[sectionIndex], ref priceLow, ref priceHigh, true);
            else
                PlayManager.ArrangeUnit(priceData.ConnectPrice[sectionIndex], 0, ref priceLow, ref priceHigh, true);

            if(priceData.ConnectTMLargeUnit)
                PlayManager.ArrangeUnit(0, priceData.ConnectTimeMoney[sectionIndex], ref timeMoneyLow, ref timeMoneyHigh, true);
            else
                PlayManager.ArrangeUnit(priceData.ConnectTimeMoney[sectionIndex], 0, ref timeMoneyLow, ref timeMoneyHigh, true);

            sectionGroup.SetActive(true);
            sectionImages[sectionIndex].SetActive(true);
            SetCheckMenu(priceData.Sections[sectionIndex].name, priceHigh + priceLow, timeMoneyHigh + timeMoneyLow);
        }
        else
        {
            Alreadybought();
        }
    }

    /// <summary>
    /// 각 구간별로 모든 역을 소유하고 있는지 확인하여 구간 별 연결 가능 여부를 계산한다.
    /// </summary>
    public void CheckHasAllStations()
    {
        for(int i = 0; i < priceData.Sections.Length; i++)
        {
            for(int k = priceData.Sections[i].from; k <= priceData.Sections[i].to; k++)
            {
                if (lineCollection.lineData.hasStation[k])
                    lineCollection.lineData.hasAllStations[i] = true;
                else
                {
                    lineCollection.lineData.hasAllStations[i] = false;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 노선 연결 처리
    /// </summary>
    public void ConnectLine()
    {
        // 선택한 구간의 이름과 데이터상의 이름이 일치하는지 확인
        if (section_text.text.Equals(priceData.Sections[targetSection].name))
        {
            // 필요한 모든 역을 보유하고 있는지 확인
            CheckHasAllStations();
            if (lineCollection.lineData.hasAllStations[targetSection])
            {
                // 비용 지불 처리
                bool result;
                if (priceData.IsLargeUnit)
                    result = AssetMoneyCalculator.instance.ArithmeticOperation(0, priceData.ConnectPrice[targetSection], false);
                else
                    result = AssetMoneyCalculator.instance.ArithmeticOperation(priceData.ConnectPrice[targetSection], 0, false);
                // 비용 지불이 성공적이면,
                if (result)
                {
                    // 연결 처리 후 보상 지급 처리 및 UI 업데이트
                    lineCollection.lineData.connected[targetSection] = true;
                    if (priceData.ConnectTMLargeUnit)
                        MyAsset.instance.TimeEarningOperator(0, priceData.ConnectTimeMoney[targetSection], true);
                    else
                        MyAsset.instance.TimeEarningOperator(priceData.ConnectTimeMoney[targetSection], 0, true);

                    CompanyReputationManager.instance.RenewPassengerBase();
                    buttonColorManager.SetColorConnection(targetSection);
                    DataManager.instance.SaveAll();
                    purchaseSound.Play();
                }
                else
                    PlayManager.instance.LackOfMoney();
            }
            else
                BuyMoreStations();
        }
        // 구매 메뉴 비활성화
        CloseCheck();
    }

    /// <summary>
    /// 구매 확인 메뉴 비활성화
    /// </summary>
    public void CloseCheck()
    {
        sectionGroup.SetActive(false);
        sectionImages[targetSection].SetActive(false);
        checkMenu.SetActive(false);
    }

    /// <summary>
    /// 구매 확인 메뉴 텍스트 업데이트
    /// </summary>
    /// <param name="section">구간 명</param>
    /// <param name="price">가격</param>
    /// <param name="timeMoney">보상</param>
    private void SetCheckMenu(string section, string price, string timeMoney)
    {
        section_text.text = section;
        price_text.text = "비용: " + price + "$";
        passenger_text.text = "시간형 수익 +" + timeMoney + "$";

        checkMenu.SetActive(true);
    }
    /// <summary>
    /// 이미 구매한 구간에 대한 메시지 출력
    /// </summary>
    private void Alreadybought()
    {
        messageManager.ShowMessage("이미 구매한 구간입니다.");
    }
    /// <summary>
    /// 필요한 역을 보유하지 않았을 때의 메시지 출력
    /// </summary>
    private void BuyMoreStations()
    {
        messageManager.ShowMessage("해당 구간에 속하는 모든 역을 구입하세요.");
    }
}
