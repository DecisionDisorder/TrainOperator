using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class button_Rent : MonoBehaviour {

	public GameObject Rent_Menu;
	public GameObject AddSpace_Menu;
	public GameObject AddRent_Menu;

    public MessageManager messageManager;
    public ButtonColor_Controller3 buttonColor_Controller;
    public condition_Rent_button condition_Rent_Button;
    public AcceptRent_Controller acceptRent_Controller;
    public UpdateDisplay conditionRentUpdateDisplay;

    public Text[] priceTexts;
    public Text[] timeMoneyTexts;

    public RentData rentData;

	public ulong[] priceFacilities = new ulong[4];

    public int[] NumOfFacilities { get { return rentData.numOfFacilities; } set { NumOfFacilities = value; } }

    public int[] limitFacilities = new int[4];

    public ulong[] standardTimeMoneyFacilities;
    public ulong[] incrementTimeMoneyFacilities;

    private ulong[] paybacks = new ulong[4];

    public int NumofAdWatch { get { return rentData.numOfADWatch; } set { rentData.numOfADWatch = value; } }

    void Start () {
        //LoadNum ();
        conditionRentUpdateDisplay.onEnable += condition_Rent_Button.UpdateText;
    }

    private ulong GetTimeMoneyFacilities(int i)
    {
        return standardTimeMoneyFacilities[i] + (ulong)(NumOfFacilities[i] + 1) * incrementTimeMoneyFacilities[i];
    }

    private ulong GetCumulativeTimeMoney(int i)
    {
        ulong x = ((ulong)NumOfFacilities[i] + 1);
        return x * (2 * standardTimeMoneyFacilities[i] + (x - 1) * incrementTimeMoneyFacilities[i]) / 2;
    }

    void SetText()
    {
        for(int i = 0; i < priceTexts.Length; i++)
        {
            string price = string.Format("{0:#,##0}", priceFacilities[i]);
            priceTexts[i].text = "비용: " + price + "$";

            string touchMoney = string.Format("{0:#,##0}", GetTimeMoneyFacilities(i));
            timeMoneyTexts[i].text = "초당 +" + touchMoney + "$";
        }

        buttonColor_Controller.SetRent();
    }

    void SetPrice()
    {
        paybacks[0] = (ulong)(0.005 * (NumOfFacilities[0] + 1) * (NumOfFacilities[0] + 1) + 0.605 * (NumOfFacilities[0] + 1) + 1.391);
        paybacks[1] = (ulong)(0.059 * (NumOfFacilities[1] + 1) * (NumOfFacilities[1] + 1) + 3.79 * (NumOfFacilities[1] + 1) + 26.15);
        paybacks[2] = (ulong)(0.067 * (NumOfFacilities[2] + 1) * (NumOfFacilities[2] + 1) + 4.814 * (NumOfFacilities[2] + 1) + 45.11);
        paybacks[3] = (ulong)(0.145 * (NumOfFacilities[3] + 1) * (NumOfFacilities[3] + 1) + 9.506 * (NumOfFacilities[3] + 1) + 90.34);

        for(int i = 0; i < priceFacilities.Length; i++)
        {
            if(i.Equals(1))
                priceFacilities[i] = (ulong)(paybacks[i] * GetCumulativeTimeMoney(i) * 0.9) - 1100;
            else
                priceFacilities[i] = paybacks[i] * GetCumulativeTimeMoney(i);
        }

        SetLimit();
        SetText();
    }
    public void SetLimit()
    {
        limitFacilities[0] = MyAsset.instance.NumOfStations * 3;
        limitFacilities[1] = MyAsset.instance.NumOfStations * 1;
        limitFacilities[2] = MyAsset.instance.NumOfStations * 1 + NumofAdWatch;
        limitFacilities[3] = (int)(MyAsset.instance.NumOfStations * 0.2);
    }

    public void PressKey(int nKey)
    {
        switch (nKey)
        {
            case -1://OPenRent
                SetPrice();
                break;
            case 0://back
                Rent_Menu.SetActive(false);
                break;
            case 1://AddSPace
                AddSpace_Menu.SetActive(true);
                break;
            case 2://AddRent
                acceptRent_Controller.CheckEmptyImage();
                acceptRent_Controller.SetWaitingList();
                AddRent_Menu.SetActive(true);
                break;
        }
    }

    public void PurchaseFacility(int index)
    {
        if (NumOfFacilities[index] < limitFacilities[index])
        {
            if (AssetMoneyCalculator.instance.ArithmeticOperation(priceFacilities[index], 0, false))
            {
                MyAsset.instance.TimeEarningOperator(GetTimeMoneyFacilities(index), 0, true);
                NumOfFacilities[index]++;
                SetPrice();
                condition_Rent_Button.UpdateText();
                //ValuePoint_Manager.instance.CheckPoint();
            }
            else
            {
                PlayManager.instance.LackOfMoney();
            }
        }
        else
        {
            FullStation();
        }
    }
	public void PressKey_AddSpace(int nKey)
	{
		switch (nKey) {
		case 0://back
			AddSpace_Menu.SetActive (false);
			break;
		}
	}
	public void PressKey_AddRent(int nKey)
	{
		switch(nKey){
		case 0://back
			AddRent_Menu.SetActive (false);
			break;
		}
	}
    /*
	public static void SaveNum()
	{
		string TM_VM = "" + tm_VM;
        string TM_CON = "" + tm_convienceStore;
        string TM_AD = "" + tm_Ad;
        string TM_DPS = "" + tm_Dps;

        string TTVM = "" + totalTouch_vm;
        string TTCON = "" + totalTouch_con;
        string TTAD = "" + totalTouch_ad;
        string TTDPS = "" + totalTouch_dps;

		PlayerPrefs.SetString ("TT_vm",TTVM);
		PlayerPrefs.SetString ("TT_con",TTCON);
        PlayerPrefs.SetString("TT_ad",TTAD);
        PlayerPrefs.SetString("TT_dps",TTDPS);

		PlayerPrefs.SetInt ("NumOfVm",NumofVm);
		PlayerPrefs.SetInt ("NumOfCon",NumofCon);
        PlayerPrefs.SetInt ("NumOfAD", NumofAd);
        PlayerPrefs.SetInt ("NumOfDps", NumofDps);

        PlayerPrefs.SetInt("NumofADWatch",NumofAdWatch);

		PlayerPrefs.SetString ("tm_vm",TM_VM);
		PlayerPrefs.SetString ("tm_con",TM_CON);
        PlayerPrefs.SetString("tm_ad",TM_AD);
        PlayerPrefs.SetString("tm_dps",TM_DPS);
	}
	public static void LoadNum()
	{
        string TM_VM = PlayerPrefs.GetString ("tm_vm");
        string TM_CON = PlayerPrefs.GetString ("tm_con");
        string TM_AD = PlayerPrefs.GetString("tm_ad");
        string TM_DPS = PlayerPrefs.GetString("tm_dps");


        string TTVM = PlayerPrefs.GetString ("TT_vm");
        string TTCON = PlayerPrefs.GetString ("TT_con");
        string TTAD = PlayerPrefs.GetString("TT_ad");
        string TTDPS = PlayerPrefs.GetString("TT_dps");

        if (TTAD != "")
        {
            totalTouch_vm = ulong.Parse(TTVM);
            totalTouch_con = ulong.Parse(TTCON);
            totalTouch_ad = ulong.Parse(TTAD);
            totalTouch_dps = ulong.Parse(TTDPS);

            tm_VM = ulong.Parse(TM_VM);
            tm_convienceStore = ulong.Parse(TM_CON);
            tm_Ad = ulong.Parse(TM_AD);
            tm_Dps = ulong.Parse(TM_DPS);
        }
		NumofVm = PlayerPrefs.GetInt ("NumOfVm");
		NumofCon =PlayerPrefs.GetInt ("NumOfCon");
        NumofAd = PlayerPrefs.GetInt("NumOfAD");
        NumofDps = PlayerPrefs.GetInt("NumOfDps");

        NumofAdWatch = PlayerPrefs.GetInt("NumofADWatch");
	}
	public static void ResetNum()
	{
		NumofVm = 0;
		NumofCon = 0;
        NumofAd = 0;
        NumofDps = 0;

		tm_VM = reset_tm_vm;
		tm_convienceStore = reset_tm_con;
        tm_Ad = reset_tm_ad;
        tm_Dps = reset_tm_dps;

		totalTouch_vm = 100;
		totalTouch_con = 1000;
        totalTouch_ad = 5000;
        totalTouch_dps = 20000;

        NumofAdWatch = 0;
	}*/
	void FullStation()
	{
        messageManager.ShowMessage("최대치입니다.\n 전철역을 늘리십시오.");
    }

}
