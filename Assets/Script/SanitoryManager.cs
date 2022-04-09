using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SanitoryManager : MonoBehaviour {

	public GameObject ManageMain_Menu;
    public MessageManager messageManager;

    public Text[] priceTexts;
	public Text[] sanitoryTexts;

    public ulong[] sanitoryLowPrices;
    public ulong[] sanitoryHighPrices;
    public ulong[] sanitoryPriceMultiples;
    public int[] sanitoryValues;

    public int CoolTime { get { return company_Reputation_Controller.companyData.sanitoryCoolTime; } set { company_Reputation_Controller.companyData.sanitoryCoolTime = value; } }
    public static bool isCoolTime;
	public Button Campaign_button;
	public Text CoolTime_text;

    public GameObject ManageSanitory_Menu;


    public CompanyReputationManager company_Reputation_Controller;
    public ButtonColor_Controller3 buttonColor_Controller;
    public condition_Sanitory_Controller con_sanitory;

    void Start()
    {
        //LoadTime();
        if (CoolTime < 5)
        {
            isCoolTime = true;
            StartCoroutine(Timer());
        }
        SetText();
    }

    public void SetText()
    {
        string moneyLow = "", moneyHigh = "";
        for (int i = 0; i < priceTexts.Length; i++)
        {
            PlayManager.ArrangeUnit(sanitoryLowPrices[i], sanitoryHighPrices[i], ref moneyLow, ref moneyHigh);
            priceTexts[i].text = "비용: " + moneyHigh + moneyLow + "$";

            sanitoryTexts[i].text = "위생도 +" + sanitoryValues[i];
        }

        CoolTime_text.text = "쿨타임: " + CoolTime + "초";
        buttonColor_Controller.SetSanitory();
    }
    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1);

        CoolTime--;
        if (CoolTime < 1)
        {
            CoolTime = 5;
            isCoolTime = false;
        }

        CoolTime_text.text = "쿨타임: " + CoolTime + "초";

        if (isCoolTime)
        {
            StartCoroutine(Timer());
        }
        else
        {
            Campaign_button.enabled = true;
            Campaign_button.image.color = Color.white;
        }
    }

    public void SetPrice()
    {
        ulong lowRevenue = 0, highRevenue = 0;
        MyAsset.instance.GetTotalRevenue(ref lowRevenue, ref highRevenue);
        for(int i = 0; i < sanitoryLowPrices.Length; i++)
        {
            sanitoryLowPrices[i] = lowRevenue * (ulong)PlayManager.instance.averageTouch * sanitoryPriceMultiples[i];
            sanitoryHighPrices[i] = highRevenue * (ulong)PlayManager.instance.averageTouch * sanitoryPriceMultiples[i];
            MoneyUnitTranslator.Arrange(ref sanitoryLowPrices[i], ref sanitoryHighPrices[i]);
        }
    }

    public void SetSanitoryMenu(bool active)
    {
        ManageMain_Menu.SetActive(active);
    }

    public void PurchaseSanitoryItem(int index)
	{
        if (con_sanitory.SanitoryValue < 100)
        {
            if (AssetMoneyCalculator.instance.ArithmeticOperation(sanitoryLowPrices[index], sanitoryHighPrices[index], false))
            {
                con_sanitory.SanitoryValue += sanitoryValues[index];
                if (con_sanitory.SanitoryValue > 100)
                {
                    con_sanitory.SanitoryValue = 100;
                }

                if (index.Equals(2))
                {
                    isCoolTime = true;
                    Campaign_button.enabled = false;
                    Campaign_button.image.color = Color.gray;
                    StartCoroutine(Timer());
                }

                con_sanitory.UpdateText();
                //con_sanitory.CalReputation();
                SetText();
            }
            else
            {
                PlayManager.instance.LackOfMoney();
            }
        }
        else if (con_sanitory.SanitoryValue >= 100)
        {
            FullSanitory();
        }
    }
    /*
	public static void SaveTime()
	{
		PlayerPrefs.SetInt ("CoolTime_San",CoolTime);
	}
	public static void LoadTime()
	{
		CoolTime = PlayerPrefs.GetInt ("CoolTime_San", 5);
	}*/
	void FullSanitory()
	{
        messageManager.ShowMessage("더 이상 깨끗해질 수 없습니다!");
	}
}
