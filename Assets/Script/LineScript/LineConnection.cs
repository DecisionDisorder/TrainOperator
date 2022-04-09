using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LineConnection: MonoBehaviour
{
    public GameObject checkMenu;
    public GameObject sectionGroup;
    public GameObject[] sectionImages;
    public Text section_text;
    public Text price_text;
    public Text passenger_text;

    public Text[] sectionPriceTexts;

    public GameObject lineMenu;

    public ButtonColorManager buttonColorManager;
    public MessageManager messageManager;

    public PriceData priceData;

    public LineCollection lineCollection;
    public LineManager lineManager;
    public LineDataManager lineDataManager;
    public UpdateDisplay connectionUpdateDisplay;

    private int targetSection;
    public AudioSource purchaseSound;

    private void Start()
    {
        if (connectionUpdateDisplay != null)
            connectionUpdateDisplay.onEnable += SetSectionPriceTexts;
    }

    public void SetLineMenu(bool onOff)
    {
        lineMenu.SetActive(onOff);
    }

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

    public void OpenConnectionCheck(int sectionIndex)
    {
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

    public void ConnectLine()
    {
        if (section_text.text.Equals(priceData.Sections[targetSection].name))
        {
            CheckHasAllStations();
            if (lineCollection.lineData.hasAllStations[targetSection])
            {
                bool result;
                if (priceData.IsLargeUnit)
                    result = AssetMoneyCalculator.instance.ArithmeticOperation(0, priceData.ConnectPrice[targetSection], false);
                else
                    result = AssetMoneyCalculator.instance.ArithmeticOperation(priceData.ConnectPrice[targetSection], 0, false);

                if (result)
                {
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
        sectionImages[targetSection].SetActive(false);
        sectionGroup.SetActive(false);
        checkMenu.SetActive(false);
    }

    public void CloseCheck()
    {
        sectionGroup.SetActive(false);
        sectionImages[targetSection].SetActive(false);
        checkMenu.SetActive(false);
    }

    private void SetCheckMenu(string section, string price, string timeMoney)
    {
        section_text.text = section;
        price_text.text = "비용: " + price + "$";
        passenger_text.text = "시간형 수익 +" + timeMoney + "$";

        checkMenu.SetActive(true);
    }
    private void Alreadybought()
    {
        messageManager.ShowMessage("이미 구매한 구간입니다.");
    }
    private void BuyMoreStations()
    {
        messageManager.ShowMessage("해당 구간에 속하는 모든 역을 구입하세요.");
    }
}
