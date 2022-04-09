using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PurchaseStation : MonoBehaviour
{
    public MessageManager messageManager;
    public GameObject checkPurchase;
    public Text stationName_text;
    public Text stationPrice_text;

    public GameObject idvCheckPurchase;
    public Text idvStationNameText;
    public Text idvStationPriceText;
    public Image[] idvEasyPurchaseButtons;
    public Text[] idvEasyPurchaseNameTexts;
    public RectTransform idvContents;

    public ButtonColorManager buttonColorManager;
    public Image[] stationImgs;
    public string stationFileName;
    public StationNameData stationNameData;

    public GameObject lineMenu;

    public PriceData priceData;
    public LineConnection lineConnection;
    public LineCollection lineCollection;
    public LineManager lineManager;
    public LineDataManager lineDataManager;

    public AudioSource purchaseSound;

    private int targetSection = -1;
    private int targetIndex = -1;

    private void Awake()
    {
        if (stationFileName != "")
            LoadStationNames();
    }

    private void Start()
    {
        //LoadColor();
    }

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

    public void OpenCheck(int section)
    {
        lineManager.currentLine = lineCollection.line;
        targetSection = section;
        stationName_text.text = priceData.Sections[targetSection].name;
        string m1 = "", m2 = "";
        if(priceData.IsLargeUnit)
            PlayManager.ArrangeUnit(0, priceData.StationPrice, ref m1, ref m2);
        else
            PlayManager.ArrangeUnit(priceData.StationPrice, 0, ref m1, ref m2);
        stationPrice_text.text = "가격: 각 " +  m2 + m1 + "$ 씩";
        checkPurchase.SetActive(true);
    }


    public void BuyStation()
    {
        if (lineCollection.lineData.sectionExpanded[targetSection])
        {
            int purchaseCount = 0;
            for(int i = priceData.Sections[targetSection].from; i <= priceData.Sections[targetSection].to; i++)
            {
                if(!lineCollection.lineData.hasStation[i])
                {
                    bool result;
                    if (priceData.IsLargeUnit)
                        result = AssetMoneyCalculator.instance.ArithmeticOperation(0, priceData.StationPrice, false);
                    else
                        result = AssetMoneyCalculator.instance.ArithmeticOperation(priceData.StationPrice, 0, false);

                    if (result)
                    {
                        lineCollection.lineData.hasStation[i] = true;
                        if (lineCollection.line.Equals(Line.Bundang))
                            lineManager.lineCollections[(int)Line.SuinBundang].lineData.hasStation[i + 26] = true;
                        stationImgs[i].color = buttonColorManager.lineColor;
                        MyAsset.instance.NumOfStations++;
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

        LoadColor();
        checkPurchase.SetActive(false);
        lineConnection.CheckHasAllStations();
        //ValuePoint_Manager.instance.CheckPoint();
        DataManager.instance.SaveAll();
    }
    
    public void CancelPurchase()
    {
        checkPurchase.SetActive(false);
    }

    public void IDVOpenCheck(GetIndexByName getIndexByName)
    {
        IDVOpenCheck(getIndexByName.GetIndex("Button"));
    }

    public void IDVOpenCheck(int index)
    {
        lineManager.currentLine = lineCollection.line;
        targetIndex = index;
        targetSection = GetSectionByStationIndex(index);
        idvStationNameText.text = stationNameData.stations[index].name + "역";
        string m1 = "", m2 = "";
        if (priceData.IsLargeUnit)
            PlayManager.ArrangeUnit(0, priceData.StationPrice, ref m1, ref m2);
        else
            PlayManager.ArrangeUnit(priceData.StationPrice, 0, ref m1, ref m2);
        idvStationPriceText.text = "가격: " + m2 + m1 + "$";
        idvCheckPurchase.SetActive(true);
    }

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

    public void IDVBuyStation()
    {
        if (lineCollection.lineData.sectionExpanded[targetSection])
        {
            if (!lineCollection.lineData.hasStation[targetIndex])
            {
                bool result;
                if (priceData.IsLargeUnit)
                    result = AssetMoneyCalculator.instance.ArithmeticOperation(0, priceData.StationPrice, false);
                else
                    result = AssetMoneyCalculator.instance.ArithmeticOperation(priceData.StationPrice, 0, false);

                if (result)
                {
                    lineCollection.lineData.hasStation[targetIndex] = true;
                    stationImgs[targetIndex].color = buttonColorManager.lineColor;
                    if (idvEasyPurchaseButtons.Length != 0)
                        idvEasyPurchaseButtons[targetIndex].color = buttonColorManager.lineColor;
                    MyAsset.instance.NumOfStations++;
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

        LoadColor();
        idvCheckPurchase.SetActive(false);
        lineConnection.CheckHasAllStations();
        //ValuePoint_Manager.instance.CheckPoint();
        DataManager.instance.SaveAll();
    }

    public void IDVCancelPurchase()
    {
        idvCheckPurchase.SetActive(false);
    }
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
    private void PurchasedMessage(int count)
    {
        messageManager.ShowMessage("구매 가능한 " + count + "개의 역을 구매하였습니다.");
    }
    private void IDVPurchasedMessage(string name)
    {
        messageManager.ShowMessage(name + "역을 구매하였습니다.");
    }
    private void ExpandFirst()
    {
        messageManager.ShowMessage("해당 노선확장권을 먼저 구매하여주십시오.");
    }
    private void LackOfMoney()
    {
        messageManager.ShowMessage("돈이 부족하여 일부 구간 구매가 누락되었습니다.");
    }
    private void AlreadyBought()
    {
        messageManager.ShowMessage("이미 보유한 역입니다.");
    }
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

[System.Serializable]
public class StationNameData
{
    public StationNames[] stations;
}

[System.Serializable]
public class StationNames
{
    public string name;
}