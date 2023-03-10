using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 역 구매 관리 클래스
/// </summary>
[System.Serializable]
public class PurchaseStation : MonoBehaviour
{
    public MessageManager messageManager;
    /// <summary>
    /// 구매 확인 메뉴
    /// </summary>
    public GameObject checkPurchase;
    /// <summary>
    /// 구매할 역 이름(구간) 텍스트
    /// </summary>
    public Text stationName_text;
    /// <summary>
    /// 구매할 역의 개별 가격 텍스트
    /// </summary>
    public Text stationPrice_text;

    /// <summary>
    /// 개별 구매 확인 메뉴
    /// </summary>
    public GameObject idvCheckPurchase;
    /// <summary>
    /// 개별 구매 역 이름 텍스트
    /// </summary>
    public Text idvStationNameText;
    /// <summary>
    /// 개별 구매 역 가격 텍스트
    /// </summary>
    public Text idvStationPriceText;
    /// <summary>
    /// 개별 구매 - 역 간편 구매 버튼
    /// </summary>
    public Image[] idvEasyPurchaseButtons;
    /// <summary>
    /// 개별 구매 - 간편 구매 버튼 속 역 이름 텍스트
    /// </summary>
    public Text[] idvEasyPurchaseNameTexts;
    /// <summary>
    /// 개별 구매 스크롤 콘텐츠의 크기 조정을 위한 RectTransform
    /// </summary>
    public RectTransform idvContents;

    public ButtonColorManager buttonColorManager;
    /// <summary>
    /// 노선도 상의 역 아이콘 이미지
    /// </summary>
    public Image[] stationImgs;
    /// <summary>
    /// 역 정보에 대한 파일 이름
    /// </summary>
    public string stationFileName;
    /// <summary>
    /// 역 이름 데이터
    /// </summary>
    public StationNameData stationNameData;

    /// <summary>
    /// 노선 메뉴
    /// </summary>
    public GameObject lineMenu;

    public PriceData priceData;
    public LineConnection lineConnection;
    public LineCollection lineCollection;
    public LineManager lineManager;
    public LineDataManager lineDataManager;
    public AchievementManager achievementManager;

    /// <summary>
    /// 구매 효과음
    /// </summary>
    public AudioSource purchaseSound;

    /// <summary>
    /// 선택한 구간 인덱스
    /// </summary>
    private int targetSection = -1;
    /// <summary>
    /// 선택한 역 인덱스
    /// </summary>
    private int targetIndex = -1;

    private void Awake()
    {
        if (stationFileName != "")
            LoadStationNames();
    }

    /// <summary>
    /// 노선 메뉴 활성화/비활성화
    /// </summary>
    /// <param name="onOff">활성화 여부</param>
    public void SetLineMenu(bool onOff)
    {
        lineMenu.SetActive(onOff);
        if(onOff)
        {
            if (idvContents != null)
                InitializeEasyButton();
            LoadColor();
        }
    }

    /// <summary>
    /// 구매 확인 메뉴 활성화
    /// </summary>
    /// <param name="section">선택한 구간</param>
    public void OpenCheck(int section)
    {
        // 구매하고자 하는 노선 설정 및 UI 정보 업데이트
        lineManager.currentLine = lineCollection.line;
        targetSection = section;
        stationName_text.text = priceData.Sections[targetSection].name;
        string m1 = "", m2 = "";
        if(priceData.IsLargeUnit)
            PlayManager.ArrangeUnit(0, priceData.StationPrice, ref m1, ref m2, true);
        else
            PlayManager.ArrangeUnit(priceData.StationPrice, 0, ref m1, ref m2, true);
        stationPrice_text.text = "가격: 각 " +  m2 + m1 + "$ 씩";
        checkPurchase.SetActive(true);
    }

    /// <summary>
    /// 역 구매 처리
    /// </summary>
    public void BuyStation()
    {
        // 해당 구간의 확장권을 보유했는지 확인
        if (lineCollection.lineData.sectionExpanded[targetSection])
        {
            // 해당 구간에 대해서 개별 구매 처리
            int purchaseCount = 0;
            for(int i = priceData.Sections[targetSection].from; i <= priceData.Sections[targetSection].to; i++)
            {
                // 보유하지 않은 역이면
                if(!lineCollection.lineData.hasStation[i])
                {
                    // 비용 지불 처리
                    bool result;
                    if (priceData.IsLargeUnit)
                        result = AssetMoneyCalculator.instance.ArithmeticOperation(0, priceData.StationPrice, false);
                    else
                        result = AssetMoneyCalculator.instance.ArithmeticOperation(priceData.StationPrice, 0, false);

                    // 비용 처리 후 구매 완료 처리 및 기타 데이터 설정
                    if (result)
                    {
                        lineCollection.lineData.hasStation[i] = true;
                        stationImgs[i].color = buttonColorManager.lineColor;
                        MyAsset.instance.NumOfStations++;
                        achievementManager.TotalStationAmount++;
                        purchaseCount++;
                        DataManager.instance.SaveAll();
                        purchaseSound.Play();
                    }
                    else
                        LackOfMoney();
                }
            }
            if (purchaseCount > 0)
                PurchasedMessage(purchaseCount);
            else
                messageManager.ShowMessage("해당 구간 내에 구매 가능한 역이 없습니다.");
        }
        else
            ExpandFirst();

        // 구매 색상 정보 업데이트
        LoadColor();
        // 메뉴 비활성화 및 구간 별 구매 완료 확인 후 저장
        checkPurchase.SetActive(false);
        lineConnection.CheckHasAllStations();
        DataManager.instance.SaveAll();
    }
    
    /// <summary>
    /// 구매 취소
    /// </summary>
    public void CancelPurchase()
    {
        checkPurchase.SetActive(false);
    }

    /// <summary>
    /// 개별 구매 확인 메뉴
    /// </summary>
    /// <param name="getIndexByName">게임 오브젝트의 이름으로 노선 인덱스 구한 결과</param>
    public void IDVOpenCheck(GetIndexByName getIndexByName)
    {
        IDVOpenCheck(getIndexByName.GetIndex("Button"));
    }

    /// <summary>
    /// 개별 구매 확인 메뉴
    /// </summary>
    /// <param name="index">개별 구매 할 인덱스</param>
    public void IDVOpenCheck(int index)
    {
        lineManager.currentLine = lineCollection.line;
        targetIndex = index;
        targetSection = GetSectionByStationIndex(index);
        idvStationNameText.text = stationNameData.stations[index].name + "역";
        string m1 = "", m2 = "";
        if (priceData.IsLargeUnit)
            PlayManager.ArrangeUnit(0, priceData.StationPrice, ref m1, ref m2, true);
        else
            PlayManager.ArrangeUnit(priceData.StationPrice, 0, ref m1, ref m2, true);
        idvStationPriceText.text = "가격: " + m2 + m1 + "$";
        idvCheckPurchase.SetActive(true);
    }

    /// <summary>
    /// 역의 인덱스로 구간 찾기
    /// </summary>
    /// <param name="index">역의 인덱스</param>
    /// <returns></returns>
    private int GetSectionByStationIndex(int index)
    {
        for(int i = 0; i < priceData.Sections.Length; i++)
        {
            if(index <=  priceData.Sections[i].to)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 역 개별 구매 처리
    /// </summary>
    public void IDVBuyStation()
    {
        // 해당 확장권 보유 여부 확인
        if (lineCollection.lineData.sectionExpanded[targetSection])
        {
            // 보유한 역이 아니라면
            if (!lineCollection.lineData.hasStation[targetIndex])
            {
                // 비용 지불 처리
                bool result;
                if (priceData.IsLargeUnit)
                    result = AssetMoneyCalculator.instance.ArithmeticOperation(0, priceData.StationPrice, false);
                else
                    result = AssetMoneyCalculator.instance.ArithmeticOperation(priceData.StationPrice, 0, false);

                // 비용 지불이 완료되면 구매 처리 및 기타 데이터 업데이트
                if (result)
                {
                    lineCollection.lineData.hasStation[targetIndex] = true;
                    stationImgs[targetIndex].color = buttonColorManager.lineColor;
                    if (idvEasyPurchaseButtons.Length != 0)
                        idvEasyPurchaseButtons[targetIndex].color = buttonColorManager.lineColor;
                    MyAsset.instance.NumOfStations++;
                    achievementManager.TotalStationAmount++;
                    IDVPurchasedMessage(stationNameData.stations[targetIndex].name);
                    DataManager.instance.SaveAll();
                }
                else
                    LackOfMoney();
            }
            else
                AlreadyBought();
        }
        else
            ExpandFirst();

        // 구매 색상 정보 업데이트
        LoadColor();
        // 메뉴 비활성화 및 구간 별 구매 완료 확인 후 저장
        idvCheckPurchase.SetActive(false);
        lineConnection.CheckHasAllStations();
        DataManager.instance.SaveAll();
    }

    /// <summary>
    /// 개별 역 구매 취소
    /// </summary>
    public void IDVCancelPurchase()
    {
        idvCheckPurchase.SetActive(false);
    }
    /// <summary>
    /// 간편 구매 버튼 초기화 작업
    /// </summary>
    private void InitializeEasyButton()
    {
        bool isFirstLoad = false;
        for(int i = 0; i < idvEasyPurchaseButtons.Length; i++)
        {
            if (idvEasyPurchaseNameTexts[i].text.Equals(""))
            {
                isFirstLoad = true;
                if (i != 0)
                    idvEasyPurchaseButtons[i].transform.localPosition = idvEasyPurchaseButtons[i - 1].transform.localPosition - new Vector3(0, 70, 0);
                idvEasyPurchaseNameTexts[i].text = stationNameData.stations[i].name;
            }
            if(lineCollection.lineData.hasStation[i])
                idvEasyPurchaseButtons[i].color = buttonColorManager.lineColor;
        }
        if (isFirstLoad)
        {
            idvContents.sizeDelta = new Vector2(idvContents.sizeDelta.x, idvEasyPurchaseButtons.Length * 70 + 10);
            idvContents.anchoredPosition = new Vector2(idvContents.anchoredPosition.x, -idvContents.sizeDelta.y / 2);
        }
    }

    /// <summary>
    /// 구매/비구매 역을 구분하기 위한 색상 정보 업데이트
    /// </summary>
    public void LoadColor()
    {
        for (int i = 0; i < stationImgs.Length; i++)
        {
            if (lineCollection.lineData.hasStation[i])
                stationImgs[i].color = buttonColorManager.lineColor;
            else
                stationImgs[i].color = Color.white;
        }
    }
    /// <summary>
    /// 구매 완료 메시지 출력
    /// </summary>
    /// <param name="count">구매한 역 개수</param>
    private void PurchasedMessage(int count)
    {
        messageManager.ShowMessage("구매 가능한 " + count + "개의 역을 구매하였습니다.");
    }
    /// <summary>
    /// 개별 역 구매 완료 메시지
    /// </summary>
    /// <param name="name">구매한 역 이름</param>
    private void IDVPurchasedMessage(string name)
    {
        messageManager.ShowMessage(name + "역을 구매하였습니다.");
    }
    /// <summary>
    /// 노선 확장권 구매 안내 메시지
    /// </summary>
    private void ExpandFirst()
    {
        messageManager.ShowMessage("해당 노선확장권을 먼저 구매하여주십시오.");
    }
    /// <summary>
    /// 자산 부족 안내 메시지
    /// </summary>
    private void LackOfMoney()
    {
        messageManager.ShowMessage("돈이 부족하여 일부 구간 구매가 누락되었습니다.");
    }
    /// <summary>
    /// 이미 보유한 역에 대한ㅁ ㅔ시지
    /// </summary>
    private void AlreadyBought()
    {
        messageManager.ShowMessage("이미 보유한 역입니다.");
    }
    /// <summary>
    /// 역 이름 데이터 로딩
    /// </summary>
    public void LoadStationNames()
    {
        TextAsset mytxtData = Resources.Load<TextAsset>("Json/" + stationFileName);
        string txt = mytxtData.text;
        if(txt != "" && txt != null)
        {
            string dataAsJson = txt;
            stationNameData = JsonUtility.FromJson<StationNameData>(dataAsJson);
        }
    }
}

/// <summary>
/// 역 이름 데이터 클래스
/// </summary>
[System.Serializable]
public class StationNameData
{
    public StationNames[] stations;
}

/// <summary>
/// 개별 역 이름 클래스
/// </summary>
[System.Serializable]
public class StationNames
{
    public string name;
}