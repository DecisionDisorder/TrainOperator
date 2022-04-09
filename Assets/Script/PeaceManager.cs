using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PeaceManager : MonoBehaviour {

    public Slider GageSlider;

	public GameObject Crime_Menu;
	public GameObject AfterAccept_Menu;

	public GameObject CrimeImage_1;
	public GameObject CrimeImage_2;
	public GameObject CrimeImage_3;

	public Text Percentage_text;

	public Image Increasing_Gage;

    public MessageManager messageManager;

    public static int Percentage_Crime;
	int Randomed_Percentage;

	public static int Gage = 50;
    public static int GageUP = 8;//6,6,8,12,16

    public ulong[] peaceLowPrices;
    public ulong[] peaceHighPrices;
    public ulong[] peacePriceMultiples;
    public int[] peaceValues;

    public static ulong Bonus_Cal;
	public static int Bonus_Reputation;

    /// <summary>
    /// 보상과 연결되는 예상 클리어 타임
    /// </summary>
    public int clearTime;

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

    public ButtonColor_Controller3 buttonColor_Controller;
    public CompanyReputationManager company_Reputation_Controller;
    public Company_Peace_Controller company_Peace_Controller;
    public GameObject ManagePeace_Menu;

    void Start()
    {
        StartCoroutine(Timer_Gage());
        SetText();
        //LoadTime();
        if (CoolTime < 5)
        {
            isCoolTime = true;
            StartCoroutine(CampaignTimer());
        }
    }

    IEnumerator Timer_Gage()
    {
        yield return new WaitForSeconds(1);

        GageSlider.value = Gage;
        if (AfterAccept_Menu.activeInHierarchy)
        {
            if (Gage > 0)
            {
                float x = Increasing_Gage.transform.position.x;
                float y = Increasing_Gage.transform.position.y;
                Increasing_Gage.GetComponent<RectTransform>().sizeDelta -= new Vector2(4, 0);
                Increasing_Gage.rectTransform.position = new Vector3(x - 1.75f, y, 0);
                Gage -= 4;
            }
            else if (Gage <= 0)
            {
                messageManager.ShowMessage("범죄 해결에 실패하였습니다.", 2f);
                Fail();
            }
            

            if (Gage < 0)
            {
                AfterAccept_Menu.SetActive(false);
                CrimeImage_1.SetActive(false);
                CrimeImage_2.SetActive(false);
                CrimeImage_3.SetActive(false);
                Fail();
                Gage = 99999;
            }
        }
        Percentage_text.text = Percentage_Crime + "%";

        StartCoroutine(Timer_Gage());
    }

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
    public void OnOpenManagePeace()
    {
        SetText();
        company_Peace_Controller.SetInfoText();
    }

	public void PressKey_Close(int nKey)
	{
		switch (nKey) {
		case 0://success
			Success_Menu.SetActive (false);
			Crime_Menu.SetActive (false);
			break;
		case 1://fail
			Fail_Menu.SetActive (false);
			Crime_Menu.SetActive (false);
			break;
		}
	}

	public void PressKey_Choice(int nKey)
	{
        switch (nKey)
        {
            case 0://decline
                Crime_Menu.SetActive(false);
                CrimeImage_1.SetActive(false);
                CrimeImage_2.SetActive(false);
                CrimeImage_3.SetActive(false);
                messageManager.ShowMessage("범죄 해결을 거절하였습니다.(패널티x)", 2f);
                break;
            case 1://accept
                AfterAccept_Menu.SetActive(true);
                break;
        }
	}

    public void PressKey_AfterAccept(int nKey)
    {
        float x = Increasing_Gage.transform.position.x;
        float y = Increasing_Gage.transform.position.y;
        switch (nKey)
        {
            case 1:

                if (Gage < 360)
                {
                    //Increasing_Gage.GetComponent<RectTransform> ().sizeDelta += new Vector2 (GageUP, 0);
                    //Increasing_Gage.rectTransform.position = new Vector3 (x + (float)(GageUP*0.45), y, 0);
                    
                    Gage += GageUP;
                }
                else if (Gage >= 360)
                {
                    AfterAccept_Menu.SetActive(false);
                    CrimeImage_1.SetActive(false);
                    CrimeImage_2.SetActive(false);
                    CrimeImage_3.SetActive(false);
                    GageFull();
                    GageSlider.value = 0;
                    //Increasing_Gage.GetComponent<RectTransform>().localPosition = new Vector3(-125, 0, 0);
                    //Increasing_Gage.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 20);
                }
                break;
        }
    }

	public void PressKey_Manage(int index)
	{
        if (company_Peace_Controller.PeaceValue < 100)
        {
            if (AssetMoneyCalculator.instance.ArithmeticOperation(peaceLowPrices[index], peaceHighPrices[index], false))
            {
                company_Peace_Controller.PeaceValue += peaceValues[index];
                if (company_Peace_Controller.PeaceValue > 100)
                {
                    company_Peace_Controller.PeaceValue = 100;
                }

                if(index.Equals(3))
                {
                    isCoolTime = true;
                    Campaign_button.enabled = false;
                    Campaign_button.image.color = Color.gray;
                    StartCoroutine(CampaignTimer());
                }

                company_Peace_Controller.SetInfoText();
                SetText();
            }
            else
            {
                PlayManager.instance.LackOfMoney();
            }
        }
        else if (company_Peace_Controller.PeaceValue >= 100)
        {
            FullPeace();
        }
	}
	void GageFull()
	{
		if (company_Peace_Controller.peaceCondition == 1) {
			Percentage_Crime = 50;
			Bonus_Reputation = 10;
		}
		if (company_Peace_Controller.peaceCondition == 2) {
			Percentage_Crime = 60;
			Bonus_Reputation = 15;
        }
		if (company_Peace_Controller.peaceCondition == 3) {
			Percentage_Crime = 75;
			Bonus_Reputation = 20;
        }
		if (company_Peace_Controller.peaceCondition == 4) {
			Percentage_Crime = 90;
			Bonus_Reputation = 30;
        }
		if (company_Peace_Controller.peaceCondition == 5) {
			Percentage_Crime = 101;
			Bonus_Reputation = 45;
        }
		CalculatePercentage ();

	}
	void CalculatePercentage()
    {
        Randomed_Percentage = Random.Range (1,101);
		if (Randomed_Percentage <= Percentage_Crime) {
			
			Success_Menu.SetActive (true);
            //ValuePoint_Manager.instance.NumOfSuccess++;
            //ValuePoint_Manager.instance.CheckPoint();

            ulong addedMoneyLow = 0, addedMoneyHigh = 0;
            string money1 = "", money2 = "";
            PlayManager.SetMoneyReward(ref addedMoneyLow, ref addedMoneyHigh, clearTime);
            AssetMoneyCalculator.instance.ArithmeticOperation(addedMoneyLow, addedMoneyHigh, true);
            PlayManager.ArrangeUnit(addedMoneyLow, addedMoneyHigh, ref money1, ref money2);
            if (addedMoneyHigh > 0)
            {
                bonus_Message.text = "범죄 해결을 하여 장려금 <color=green>" + money2 + money1 + "$</color>을 지급받고 고객 만족도가 " + Bonus_Reputation + "P + 가치점수 15vP가 상승하였습니다.";
            }
            else
            {
                bonus_Message.text = "범죄 해결을 하여 장려금 <color=green>" + money1 + "$</color>을 지급받고 고객 만족도가 " + Bonus_Reputation + "P + 가치점수 15vP가 상승하였습니다.";
            }

        } else {
			
			Fail ();
		}
	}

    

    void Fail()
    {
        /*string money1 = "", money2 = "";
        ulong[] deltaPercentages = { 80, 83, 85, 90, 95 };
        int peaceUnit = company_Peace_Controller.peaceCondition - 1;
        ulong deltaMoneyLow = MyAsset.instance.MoneyLow, deltaMoneyHigh = MyAsset.instance.MoneyHigh;
        MoneyUnitTranslator.CalculatePercent(ref deltaMoneyLow, ref deltaMoneyHigh, 100 - deltaPercentages[peaceUnit]);
        AssetMoneyCalculator.instance.PercentOperation(deltaPercentages[peaceUnit]);
        PlayManager.ArrangeUnit(deltaMoneyLow, deltaMoneyHigh, ref money1, ref money2);*/

        ulong deltaMoneyLow = 0, deltaMoneyHigh = 0;
        string money1 = "", money2 = "";
        PlayManager.SetMoneyReward(ref deltaMoneyLow, ref deltaMoneyHigh, clearTime / 3);
        AssetMoneyCalculator.instance.ArithmeticOperation(deltaMoneyLow, deltaMoneyHigh, false);
        PlayManager.ArrangeUnit(deltaMoneyLow, deltaMoneyHigh, ref money1, ref money2);

        if (deltaMoneyLow == 0 && deltaMoneyHigh == 0)
        {
            penalty_Message.text = "범죄 해결에 실패하여 \n손해배상금으로 0$을 지불했습니다.";
        }
        else if (deltaMoneyHigh > 0)
        {
            penalty_Message.text = "범죄 해결에 실패하여 손해배상금으로 <color=red>" + money2 + money1 + "$</color>을 지불했습니다.";
        }
        else
        {
            penalty_Message.text = "범죄 해결에 실패하여 손해배상금으로 <color=red>" + money1 + "$</color>을 지불했습니다.";
        }
        Fail_Menu.SetActive(true);
    }
    /*
	public static void SaveTime()
	{
		PlayerPrefs.SetInt ("CoolTime_Peace",CoolTime);
	}
	public static void LoadTime()
	{
		CoolTime = PlayerPrefs.GetInt ("CoolTime_Peace", 5);
	}*/
    void FullPeace()
	{
        messageManager.ShowMessage("안전도가 최대치입니다.");
	}
}
