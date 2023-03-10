using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �� ���� ���� Ŭ����
/// </summary>
[System.Serializable]
public class PurchaseStation : MonoBehaviour
{
    public MessageManager messageManager;
    /// <summary>
    /// ���� Ȯ�� �޴�
    /// </summary>
    public GameObject checkPurchase;
    /// <summary>
    /// ������ �� �̸�(����) �ؽ�Ʈ
    /// </summary>
    public Text stationName_text;
    /// <summary>
    /// ������ ���� ���� ���� �ؽ�Ʈ
    /// </summary>
    public Text stationPrice_text;

    /// <summary>
    /// ���� ���� Ȯ�� �޴�
    /// </summary>
    public GameObject idvCheckPurchase;
    /// <summary>
    /// ���� ���� �� �̸� �ؽ�Ʈ
    /// </summary>
    public Text idvStationNameText;
    /// <summary>
    /// ���� ���� �� ���� �ؽ�Ʈ
    /// </summary>
    public Text idvStationPriceText;
    /// <summary>
    /// ���� ���� - �� ���� ���� ��ư
    /// </summary>
    public Image[] idvEasyPurchaseButtons;
    /// <summary>
    /// ���� ���� - ���� ���� ��ư �� �� �̸� �ؽ�Ʈ
    /// </summary>
    public Text[] idvEasyPurchaseNameTexts;
    /// <summary>
    /// ���� ���� ��ũ�� �������� ũ�� ������ ���� RectTransform
    /// </summary>
    public RectTransform idvContents;

    public ButtonColorManager buttonColorManager;
    /// <summary>
    /// �뼱�� ���� �� ������ �̹���
    /// </summary>
    public Image[] stationImgs;
    /// <summary>
    /// �� ������ ���� ���� �̸�
    /// </summary>
    public string stationFileName;
    /// <summary>
    /// �� �̸� ������
    /// </summary>
    public StationNameData stationNameData;

    /// <summary>
    /// �뼱 �޴�
    /// </summary>
    public GameObject lineMenu;

    public PriceData priceData;
    public LineConnection lineConnection;
    public LineCollection lineCollection;
    public LineManager lineManager;
    public LineDataManager lineDataManager;
    public AchievementManager achievementManager;

    /// <summary>
    /// ���� ȿ����
    /// </summary>
    public AudioSource purchaseSound;

    /// <summary>
    /// ������ ���� �ε���
    /// </summary>
    private int targetSection = -1;
    /// <summary>
    /// ������ �� �ε���
    /// </summary>
    private int targetIndex = -1;

    private void Awake()
    {
        if (stationFileName != "")
            LoadStationNames();
    }

    /// <summary>
    /// �뼱 �޴� Ȱ��ȭ/��Ȱ��ȭ
    /// </summary>
    /// <param name="onOff">Ȱ��ȭ ����</param>
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
    /// ���� Ȯ�� �޴� Ȱ��ȭ
    /// </summary>
    /// <param name="section">������ ����</param>
    public void OpenCheck(int section)
    {
        // �����ϰ��� �ϴ� �뼱 ���� �� UI ���� ������Ʈ
        lineManager.currentLine = lineCollection.line;
        targetSection = section;
        stationName_text.text = priceData.Sections[targetSection].name;
        string m1 = "", m2 = "";
        if(priceData.IsLargeUnit)
            PlayManager.ArrangeUnit(0, priceData.StationPrice, ref m1, ref m2, true);
        else
            PlayManager.ArrangeUnit(priceData.StationPrice, 0, ref m1, ref m2, true);
        stationPrice_text.text = "����: �� " +  m2 + m1 + "$ ��";
        checkPurchase.SetActive(true);
    }

    /// <summary>
    /// �� ���� ó��
    /// </summary>
    public void BuyStation()
    {
        // �ش� ������ Ȯ����� �����ߴ��� Ȯ��
        if (lineCollection.lineData.sectionExpanded[targetSection])
        {
            // �ش� ������ ���ؼ� ���� ���� ó��
            int purchaseCount = 0;
            for(int i = priceData.Sections[targetSection].from; i <= priceData.Sections[targetSection].to; i++)
            {
                // �������� ���� ���̸�
                if(!lineCollection.lineData.hasStation[i])
                {
                    // ��� ���� ó��
                    bool result;
                    if (priceData.IsLargeUnit)
                        result = AssetMoneyCalculator.instance.ArithmeticOperation(0, priceData.StationPrice, false);
                    else
                        result = AssetMoneyCalculator.instance.ArithmeticOperation(priceData.StationPrice, 0, false);

                    // ��� ó�� �� ���� �Ϸ� ó�� �� ��Ÿ ������ ����
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
                messageManager.ShowMessage("�ش� ���� ���� ���� ������ ���� �����ϴ�.");
        }
        else
            ExpandFirst();

        // ���� ���� ���� ������Ʈ
        LoadColor();
        // �޴� ��Ȱ��ȭ �� ���� �� ���� �Ϸ� Ȯ�� �� ����
        checkPurchase.SetActive(false);
        lineConnection.CheckHasAllStations();
        DataManager.instance.SaveAll();
    }
    
    /// <summary>
    /// ���� ���
    /// </summary>
    public void CancelPurchase()
    {
        checkPurchase.SetActive(false);
    }

    /// <summary>
    /// ���� ���� Ȯ�� �޴�
    /// </summary>
    /// <param name="getIndexByName">���� ������Ʈ�� �̸����� �뼱 �ε��� ���� ���</param>
    public void IDVOpenCheck(GetIndexByName getIndexByName)
    {
        IDVOpenCheck(getIndexByName.GetIndex("Button"));
    }

    /// <summary>
    /// ���� ���� Ȯ�� �޴�
    /// </summary>
    /// <param name="index">���� ���� �� �ε���</param>
    public void IDVOpenCheck(int index)
    {
        lineManager.currentLine = lineCollection.line;
        targetIndex = index;
        targetSection = GetSectionByStationIndex(index);
        idvStationNameText.text = stationNameData.stations[index].name + "��";
        string m1 = "", m2 = "";
        if (priceData.IsLargeUnit)
            PlayManager.ArrangeUnit(0, priceData.StationPrice, ref m1, ref m2, true);
        else
            PlayManager.ArrangeUnit(priceData.StationPrice, 0, ref m1, ref m2, true);
        idvStationPriceText.text = "����: " + m2 + m1 + "$";
        idvCheckPurchase.SetActive(true);
    }

    /// <summary>
    /// ���� �ε����� ���� ã��
    /// </summary>
    /// <param name="index">���� �ε���</param>
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
    /// �� ���� ���� ó��
    /// </summary>
    public void IDVBuyStation()
    {
        // �ش� Ȯ��� ���� ���� Ȯ��
        if (lineCollection.lineData.sectionExpanded[targetSection])
        {
            // ������ ���� �ƴ϶��
            if (!lineCollection.lineData.hasStation[targetIndex])
            {
                // ��� ���� ó��
                bool result;
                if (priceData.IsLargeUnit)
                    result = AssetMoneyCalculator.instance.ArithmeticOperation(0, priceData.StationPrice, false);
                else
                    result = AssetMoneyCalculator.instance.ArithmeticOperation(priceData.StationPrice, 0, false);

                // ��� ������ �Ϸ�Ǹ� ���� ó�� �� ��Ÿ ������ ������Ʈ
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

        // ���� ���� ���� ������Ʈ
        LoadColor();
        // �޴� ��Ȱ��ȭ �� ���� �� ���� �Ϸ� Ȯ�� �� ����
        idvCheckPurchase.SetActive(false);
        lineConnection.CheckHasAllStations();
        DataManager.instance.SaveAll();
    }

    /// <summary>
    /// ���� �� ���� ���
    /// </summary>
    public void IDVCancelPurchase()
    {
        idvCheckPurchase.SetActive(false);
    }
    /// <summary>
    /// ���� ���� ��ư �ʱ�ȭ �۾�
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
    /// ����/�񱸸� ���� �����ϱ� ���� ���� ���� ������Ʈ
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
    /// ���� �Ϸ� �޽��� ���
    /// </summary>
    /// <param name="count">������ �� ����</param>
    private void PurchasedMessage(int count)
    {
        messageManager.ShowMessage("���� ������ " + count + "���� ���� �����Ͽ����ϴ�.");
    }
    /// <summary>
    /// ���� �� ���� �Ϸ� �޽���
    /// </summary>
    /// <param name="name">������ �� �̸�</param>
    private void IDVPurchasedMessage(string name)
    {
        messageManager.ShowMessage(name + "���� �����Ͽ����ϴ�.");
    }
    /// <summary>
    /// �뼱 Ȯ��� ���� �ȳ� �޽���
    /// </summary>
    private void ExpandFirst()
    {
        messageManager.ShowMessage("�ش� �뼱Ȯ����� ���� �����Ͽ��ֽʽÿ�.");
    }
    /// <summary>
    /// �ڻ� ���� �ȳ� �޽���
    /// </summary>
    private void LackOfMoney()
    {
        messageManager.ShowMessage("���� �����Ͽ� �Ϻ� ���� ���Ű� �����Ǿ����ϴ�.");
    }
    /// <summary>
    /// �̹� ������ ���� ���Ѥ� �Ľ���
    /// </summary>
    private void AlreadyBought()
    {
        messageManager.ShowMessage("�̹� ������ ���Դϴ�.");
    }
    /// <summary>
    /// �� �̸� ������ �ε�
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
/// �� �̸� ������ Ŭ����
/// </summary>
[System.Serializable]
public class StationNameData
{
    public StationNames[] stations;
}

/// <summary>
/// ���� �� �̸� Ŭ����
/// </summary>
[System.Serializable]
public class StationNames
{
    public string name;
}