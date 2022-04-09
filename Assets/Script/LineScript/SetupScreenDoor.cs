using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SetupScreenDoor : MonoBehaviour
{
    public GameObject CheckMenu;

    public GameObject sectionGroup;
    public GameObject[] sectionImgs;
    public Text sectionText;
    public Text priceText;
    public Text pointText;

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

    public GameObject lineMenu;

    private int targetSection;

    public AudioSource purchaseSound;
    public UpdateDisplay screenDoorUpdateDisplay;

    private void Start()
    {
        if(screenDoorUpdateDisplay != null)
            screenDoorUpdateDisplay.onEnable += SetSectionPriceTexts;
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
                PlayManager.ArrangeUnit(0, priceData.ScreenDoorPrice[i], ref priceLow, ref priceHigh, true);
            else
                PlayManager.ArrangeUnit(priceData.ScreenDoorPrice[i], 0, ref priceLow, ref priceHigh, true);

            sectionPriceTexts[i].text = "가격: " + priceHigh + priceLow + "$";
        }
    }

    public void OpenCheck(int section)
    {
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
        else
            AlreadyPurchased();
    }

    public void ScreenDoorPurchase()
    {
        lineCollection.lineConnection.CheckHasAllStations();

        if (lineCollection.lineData.hasAllStations[targetSection])
        {
            bool result;
            if (priceData.IsLargeUnit)
                result = AssetMoneyCalculator.instance.ArithmeticOperation(0, priceData.ScreenDoorPrice[targetSection], false);
            else
                result = AssetMoneyCalculator.instance.ArithmeticOperation(priceData.ScreenDoorPrice[targetSection], 0, false);

            if (result)
            {
                lineCollection.lineData.installed[targetSection] = true;
                company_Reputation_Controller.ReputationValue += priceData.ScreenDoorReputation[targetSection];
                buttonColorManager.SetColorScreenDoor();
                UpdateData();
                DataManager.instance.SaveAll();
                purchaseSound.Play();

                if (lineCollection.line.Equals(Line.Line4) && IsFirstSectionPurchase())
                    backgroundImageManager.ChangeBackground(1);
            }
            else
                LackOfMoney();
        }
        else
            BuyMoreStations();
        CloseScreenDoor();
    }

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

    public void CloseScreenDoor()
    {
        CheckMenu.SetActive(false);
        sectionImgs[targetSection].SetActive(false);
        sectionGroup.SetActive(false);
    }

    private void UpdateData()
    {
        company_Reputation_Controller.RenewReputation();
        //condition_Sanitory_Controller.CalReputation();
        CompanyReputationManager.instance.RenewPassengerBase();
    }

    private void LackOfMoney()
    {
        messageManager.ShowMessage("돈이 부족합니다.");
    }
    private void AlreadyPurchased()
    {
        messageManager.ShowMessage("이미 설치한 구간입니다.");
    }
    private void BuyMoreStations()
    {
        messageManager.ShowMessage("해당 구간에 속하는 모든 역을 구입하세요.");
    }
}
