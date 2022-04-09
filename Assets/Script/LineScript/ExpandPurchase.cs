using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ExpandPurchase : MonoBehaviour
{
    public MessageManager messageManager;
    public ButtonColorManager buttonColorManager;

    public GameObject lineMenu;

    public GameObject sectionGroup;
    public GameObject[] sectionImgs;
    public GameObject[] LessTrainImages;
    public Text[] sectionPriceTexts;
    public Button[] purchaseButtons;
    public PriceData priceData;
    public LineCollection lineCollection;
    public LineManager lineManager;
    public LineDataManager lineDataManager;
    public SettingManager buttonOption;

    public UpdateDisplay expandUpdateDisplay;

    private int targetSection = -1;

    public AudioSource purchaseSound;

    /*private void Start()
    {
        float max = priceData.Sections[priceData.Sections.Length - 1].to;
        string msg = lineCollection.line + "\n";
        for(int i = 0; i < priceData.Sections.Length; i++)
        {
            if(i.Equals(priceData.Sections.Length - 1))
                msg += priceData.Sections[i].name + ": " + (priceData.Sections[i].to - priceData.Sections[i].from) / max * 100 + "%\n";
            else
                msg += priceData.Sections[i].name + ": " + (priceData.Sections[i].to - priceData.Sections[i].from + 1) / max * 100 + "%\n";
        }
        Debug.Log(msg);
    }*/

    private void Start()
    {
        expandUpdateDisplay.onEnable += SetSectionPriceTexts;
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

    public void OpenSection(int section)
    {
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

    public void SetQualification()
    {
        if (!lineCollection.line.Equals(Line.Line1))
        {
            if (lineManager.lineCollections[(int)lineCollection.line - 1].lineData.numOfTrain < 100)
            {
                SetLessTrainImgs(true);
            }
            else
            {
                SetLessTrainImgs(false);
            }
        }
    }

    private void SetLessTrainImgs(bool onOff)
    {
        for (int i = 0; i < LessTrainImages.Length; i++)
        {
            LessTrainImages[i].SetActive(onOff);
            purchaseButtons[i].enabled = !onOff;
        }
    }

    public void CheckConfirm(bool accept)
    {
        if (accept)
            BuyExpand();
        sectionGroup.SetActive(false);
        sectionImgs[targetSection].SetActive(false);
        targetSection = -1;
    }

    private bool IsFirstExpand()
    {
        for (int i = 0; i < lineCollection.lineData.sectionExpanded.Length; i++)
            if (lineCollection.lineData.sectionExpanded[i])
                return false;

        return true;
    }

    private void BuyExpand()
    {
        bool result;
        if (priceData.IsLargeUnit)
            result = AssetMoneyCalculator.instance.ArithmeticOperation(0, priceData.ExpandPrice[targetSection], false);
        else
            result = AssetMoneyCalculator.instance.ArithmeticOperation(priceData.ExpandPrice[targetSection], 0, false);

        if (result)
        {
            if(IsFirstExpand() && buttonOption.BackupRecommend)
            {
                messageManager.OpenCommonCheckMenu("클라우드 백업하러 가시겠습니까?", "신규 노선의 확장권을 구입하셨습니다.\n혹시 모를 상황을 대비하여 클라우드 백업을 권장합니다.", Color.cyan, OpenCloud);
            }

            if(lineCollection.line.Equals(Line.Line2))
            {
                messageManager.ShowMessage("2호선 전체 확장권을 구입하였습니다.");
                for(int i = 0; i < lineCollection.lineData.sectionExpanded.Length; i++)
                    lineCollection.lineData.sectionExpanded[i] = true;
            }
            else
            {
                messageManager.ShowMessage(priceData.Sections[targetSection].name + " 노선 확장권을 구입하였습니다.");
                lineCollection.lineData.sectionExpanded[targetSection] = true;
            }
            buttonColorManager.SetColorExpand(targetSection);
            DataManager.instance.SaveAll();
            purchaseSound.Play();
        }
        else
            LackOfMoney();
    }

    private void OpenCloud()
    {
        CloudSubManager.instance.SetCloudMenu(true);
    }

    private void AlreadyPurchased()
    {
        messageManager.ShowMessage("이미 구매하셨습니다.");
    }

    private void LackOfMoney()
    {
        messageManager.ShowMessage("돈이 부족합니다.");
    }
}
