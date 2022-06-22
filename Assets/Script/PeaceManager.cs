using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PeaceManager : MonoBehaviour {

    public Slider GageSlider;

	public GameObject Crime_Menu;
	public GameObject AfterAccept_Menu;

	public Text Percentage_text;

    public MessageManager messageManager;

    public int[] successPossibility;
    public int[] peaceCriterias;
    public string[] peaceStageTitles;

	public int gage = 50;
    public int[] gageIncrements;

    public ulong[] peaceLowPrices;
    public ulong[] peaceHighPrices;
    public ulong[] peacePriceMultiples;
    public int[] peaceValues;

    /// <summary>
    /// 보상과 연결되는 예상 클리어 타임
    /// </summary>
    public int clearTime;

    public int PeaceValue { get { return company_Reputation_Controller.companyData.peaceValue; } set { company_Reputation_Controller.companyData.peaceValue = value; } }
    public int peaceStage;
    public int CoolTime { get { return company_Reputation_Controller.companyData.peaceCoolTime; } set { company_Reputation_Controller.companyData.peaceCoolTime = value; } }
    public static bool isCoolTime = false;
	public Button Campaign_button;
	public Text CoolTime_text;

	public GameObject Success_Menu;
	public Text bonus_Message;
	public GameObject Fail_Menu;
	public Text penalty_Message;

    public Text[] priceTexts;
    public Text[] peaceValueTexts;

    public Text Peace_text;
    public Text peaceConditonText;

    public AudioSource successAudio;
    public AudioSource failedAudio;

    public ButtonColor_Controller3 buttonColor_Controller;
    public CompanyReputationManager company_Reputation_Controller;
    public GameObject ManagePeace_Menu;

    public UpdateDisplay stationConditionUpdateDisplay;

    void Start()
    {
        SetPeaceInfoText();
        SetText();
        if (CoolTime < 5)
        {
            isCoolTime = true;
            StartCoroutine(CampaignTimer());
        }
        StartCoroutine(ValueDropTimer(SetTime()));
        stationConditionUpdateDisplay.onEnableUpdate += SetPeaceInfoText;
    }

    #region Peace MiniGame
    public void InitPeaceMiniGame()
    {
        Fail_Menu.SetActive(false);
        Crime_Menu.SetActive(true);
        Percentage_text.text = successPossibility[peaceStage] + "%";
        gage = 50;
        GageSlider.value = gage;
    }

    IEnumerator GageDecrement()
    {
        yield return new WaitForSeconds(0.3f);

        if (gage > 0)
        {
            gage -= gageIncrements[peaceStage] / 2;
            GageSlider.value = gage;
            StartCoroutine(GageDecrement());
        }
        else
        {
            messageManager.ShowMessage("범죄 해결에 실패하였습니다.", 2f);
            AfterAccept_Menu.SetActive(false);
            OnFail();
            gage = 99999;
        }
    }

    public void PressKey_Close(int nKey)
    {
        switch (nKey)
        {
            case 0://success
                Success_Menu.SetActive(false);
                Crime_Menu.SetActive(false);
                break;
            case 1://fail
                Fail_Menu.SetActive(false);
                Crime_Menu.SetActive(false);
                break;
        }
    }

    public void PressKey_Choice(int nKey)
    {
        switch (nKey)
        {
            case 0://decline
                Crime_Menu.SetActive(false);
                messageManager.ShowMessage("범죄 해결을 거절하였습니다.(패널티x)", 2f);
                break;
            case 1://accept
                AfterAccept_Menu.SetActive(true);
                StartCoroutine(GageDecrement());
                break;
        }
    }

    public void Fighting()
    {
        if (gage < 360)
        {
            gage += gageIncrements[peaceStage];
            GageSlider.value = gage;
        }
        else if (gage >= 360)
        {
            AfterAccept_Menu.SetActive(false);
            OnSuccess();
        }
    }

    private void OnSuccess()
    {
        int randomP = Random.Range(0, 100);
        if (randomP < successPossibility[peaceStage])
        {

            Success_Menu.SetActive(true);

            ulong addedMoneyLow = 0, addedMoneyHigh = 0;
            string money1 = "", money2 = "";
            PlayManager.SetMoneyReward(ref addedMoneyLow, ref addedMoneyHigh, clearTime);
            AssetMoneyCalculator.instance.ArithmeticOperation(addedMoneyLow, addedMoneyHigh, true);
            PlayManager.ArrangeUnit(addedMoneyLow, addedMoneyHigh, ref money1, ref money2);
            successAudio.Play();

            bonus_Message.text = "범죄 해결을 하여 장려금 <color=green>" + money2 + money1 + "$</color>을 지급받았습니다.";

        }
        else
            OnFail();
    }

    private void OnFail()
    {
        penalty_Message.text = "범죄 해결에 실패하였습니다.";
        failedAudio.Play();
        Fail_Menu.SetActive(true);
    }
    #endregion
    #region Peace Item store
    IEnumerator CampaignTimer()
    {
        yield return new WaitForSeconds(1);

        CoolTime--;
        if (CoolTime < 1)
        {
            isCoolTime = false;
            CoolTime = 5;
        }
        CoolTime_text.text = "쿨타임: " + CoolTime + "초";

        if (isCoolTime)
            StartCoroutine(CampaignTimer());
        else
        {
            Campaign_button.enabled = true;
            Campaign_button.image.color = Color.white;
        }
    }

    private void SetText()
    {
        string moneyLow = "", moneyHigh = "";
        for (int i = 0; i < priceTexts.Length; i++)
        {
            PlayManager.ArrangeUnit(peaceLowPrices[i], peaceHighPrices[i], ref moneyLow, ref moneyHigh);
            priceTexts[i].text = "비용: " + moneyHigh + moneyLow + "$";

            peaceValueTexts[i].text = "안전도 +" + peaceValues[i];
        }

        CoolTime_text.text = "쿨타임: " + CoolTime + "초";
        buttonColor_Controller.SetPeace();
    }

    public void SetPrice()
    {
        ulong lowRevenue = 0, highRevenue = 0;
        MyAsset.instance.GetTotalRevenue(ref lowRevenue, ref highRevenue);
        for (int i = 0; i < peaceLowPrices.Length; i++)
        {
            peaceLowPrices[i] = lowRevenue * (ulong)PlayManager.instance.averageTouch * peacePriceMultiples[i];
            peaceHighPrices[i] = highRevenue * (ulong)PlayManager.instance.averageTouch * peacePriceMultiples[i];
            MoneyUnitTranslator.Arrange(ref peaceLowPrices[i], ref peaceHighPrices[i]);
        }
    }
    public void PressKey_Manage(int index)
    {
        if (PeaceValue < 100)
        {
            if (AssetMoneyCalculator.instance.ArithmeticOperation(peaceLowPrices[index], peaceHighPrices[index], false))
            {
                PeaceValue += peaceValues[index];
                if (PeaceValue > 100)
                {
                    PeaceValue = 100;
                }

                if (index.Equals(3))
                {
                    isCoolTime = true;
                    Campaign_button.enabled = false;
                    Campaign_button.image.color = Color.gray;
                    StartCoroutine(CampaignTimer());
                }

                SetPeaceInfoText();
                SetText();
            }
            else
            {
                PlayManager.instance.LackOfMoney();
            }
        }
        else if (PeaceValue >= 100)
        {
            FullOfPeace();
        }
    }

    private void FullOfPeace()
    {
        messageManager.ShowMessage("안전도가 최대치입니다.");
    }
    #endregion
    #region PeaceValue Dropping
    IEnumerator ValueDropTimer(float waittime)
    {
        yield return new WaitForSeconds(waittime);

        PeaceValue -= GetDropAmount();
        if (PeaceValue < -100)
        {
            PeaceValue = -100;
        }
        SetPeaceInfoText();
        StartCoroutine(ValueDropTimer(SetTime()));
    }
    private int SetTime()
    {
        int Timeset;
        if (!LineManager.instance.lineCollections[4].isExpanded())
        {
            Timeset = Random.Range(50, 71);
        }
        else if (!LineManager.instance.lineCollections[9].isExpanded())
        {
            Timeset = Random.Range(45, 61);
        }
        else
        {
            Timeset = Random.Range(40, 51);
        }
        return Timeset;
    }
    private int GetDropAmount()
    {
        if (!LineManager.instance.lineCollections[4].isExpanded())
            return Random.Range(0, 5);
        else if (!LineManager.instance.lineCollections[9].isExpanded())
            return Random.Range(2, 7);
        else
            return Random.Range(4, 9);
    }
    #endregion
    #region Information/State updater
    public void OnOpenManagePeace()
    {
        SetText();
        SetPeaceInfoText();
    }

    public void SetPeaceInfoText()
    {
        Peace_text.text = "안전도: " + PeaceValue + "점";
        CheckPeaceStage();
    }

    public void CheckPeaceStage()
    {
        for (int i = 0; i < peaceCriterias.Length; i++)
        {
            if (PeaceValue <= peaceCriterias[i])
            {
                peaceStage = i;
                break;
            }
        }
        peaceConditonText.text = "상태: " + peaceStageTitles[peaceStage];

    }
    #endregion
}
