using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �뼱 ���� �ý��� ���� Ŭ����
/// </summary>
[System.Serializable]
public class LineConnection: MonoBehaviour
{
    /// <summary>
    /// ���� Ȯ�� Ŭ����
    /// </summary>
    public GameObject checkMenu;
    /// <summary>
    /// ���� �׷� ������Ʈ
    /// </summary>
    public GameObject sectionGroup;
    /// <summary>
    /// ���� �� �뼱�� �̹���
    /// </summary>
    public GameObject[] sectionImages;
    /// <summary>
    /// ������ ���� �̸� �ؽ�Ʈ
    /// </summary>
    public Text section_text;
    /// <summary>
    /// ������ ������ ���� �ؽ�Ʈ
    /// </summary>
    public Text price_text;
    /// <summary>
    /// ������ ������ ���� �ؽ�Ʈ (�̸��� passenger����, �ð��� ���� �������� �����)
    /// </summary>
    public Text passenger_text;

    /// <summary>
    /// ������ ���� ���� �ؽ�Ʈ
    /// </summary>
    public Text[] sectionPriceTexts;

    /// <summary>
    /// �뼱 �޴� ������Ʈ
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
    /// ������ ���� �ε���
    /// </summary>
    private int targetSection;
    /// <summary>
    /// ���� ȿ����
    /// </summary>
    public AudioSource purchaseSound;

    private void Start()
    {
        if (connectionUpdateDisplay != null)
            connectionUpdateDisplay.onEnable += SetSectionPriceTexts;
    }

    /// <summary>
    /// �뼱 �޴� Ȱ��ȭ/��Ȱ��ȭ
    /// </summary>
    /// <param name="onOff">Ȱ��ȭ ����</param>
    public void SetLineMenu(bool onOff)
    {
        lineMenu.SetActive(onOff);
    }

    /// <summary>
    /// ��� ���� ���� �ؽ�Ʈ ���� ������Ʈ
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

            sectionPriceTexts[i].text = "����: " + priceHigh + priceLow + "$";
        }
    }

    /// <summary>
    /// �뼱 ���� Ȯ�� �޴��� Ȱ��ȭ�Ѵ�.
    /// </summary>
    /// <param name="sectionIndex">������ ���� �ε���</param>
    public void OpenConnectionCheck(int sectionIndex)
    {
        // ���� �ȵ� �뼱�� ��� ���� ���� �ؽ�Ʈ�� ������Ʈ�Ͽ� �޴��� Ȱ��ȭ�Ѵ�.
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
    /// �� �������� ��� ���� �����ϰ� �ִ��� Ȯ���Ͽ� ���� �� ���� ���� ���θ� ����Ѵ�.
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
    /// �뼱 ���� ó��
    /// </summary>
    public void ConnectLine()
    {
        // ������ ������ �̸��� �����ͻ��� �̸��� ��ġ�ϴ��� Ȯ��
        if (section_text.text.Equals(priceData.Sections[targetSection].name))
        {
            // �ʿ��� ��� ���� �����ϰ� �ִ��� Ȯ��
            CheckHasAllStations();
            if (lineCollection.lineData.hasAllStations[targetSection])
            {
                // ��� ���� ó��
                bool result;
                if (priceData.IsLargeUnit)
                    result = AssetMoneyCalculator.instance.ArithmeticOperation(0, priceData.ConnectPrice[targetSection], false);
                else
                    result = AssetMoneyCalculator.instance.ArithmeticOperation(priceData.ConnectPrice[targetSection], 0, false);
                // ��� ������ �������̸�,
                if (result)
                {
                    // ���� ó�� �� ���� ���� ó�� �� UI ������Ʈ
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
        // ���� �޴� ��Ȱ��ȭ
        CloseCheck();
    }

    /// <summary>
    /// ���� Ȯ�� �޴� ��Ȱ��ȭ
    /// </summary>
    public void CloseCheck()
    {
        sectionGroup.SetActive(false);
        sectionImages[targetSection].SetActive(false);
        checkMenu.SetActive(false);
    }

    /// <summary>
    /// ���� Ȯ�� �޴� �ؽ�Ʈ ������Ʈ
    /// </summary>
    /// <param name="section">���� ��</param>
    /// <param name="price">����</param>
    /// <param name="timeMoney">����</param>
    private void SetCheckMenu(string section, string price, string timeMoney)
    {
        section_text.text = section;
        price_text.text = "���: " + price + "$";
        passenger_text.text = "�ð��� ���� +" + timeMoney + "$";

        checkMenu.SetActive(true);
    }
    /// <summary>
    /// �̹� ������ ������ ���� �޽��� ���
    /// </summary>
    private void Alreadybought()
    {
        messageManager.ShowMessage("�̹� ������ �����Դϴ�.");
    }
    /// <summary>
    /// �ʿ��� ���� �������� �ʾ��� ���� �޽��� ���
    /// </summary>
    private void BuyMoreStations()
    {
        messageManager.ShowMessage("�ش� ������ ���ϴ� ��� ���� �����ϼ���.");
    }
}
