using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AcceptRent_Controller : MonoBehaviour {
	enum StoreType { Lazer, Fox, Cat, box, Cs25, Icu, Minitop }

    public Text[] nameTexts;
	public Text[] typeTexts;
	public Text[] timeMoneyTexts;

	public Text Check_Name;
	public Text Check_Type;
	public Text Check_Price;
	public Text Check_TM;

	public GameObject[] emptyObjs;

	public Button[] acceptButtons;

	public GameObject CheckAccept_Menu;

	public Text cooltime_text;
	public int QuickRentCoolTimefast { get { return rent.rentData.quickRentCoolTime; } set { rent.rentData.quickRentCoolTime = value; } }
	private bool quickRentCoolDown = false;

	public Button quickRentButton;
	public Color Gray;

	public button_Rent rent;
	public AddRent_Controller addRent_Controller;
	public MessageManager messageManager;
	public condition_Rent_button condition_Rent_Button;

	public int[] numOfRented { get { return rent.rentData.numOfRented; } set { rent.rentData.numOfRented = value; } }

	public ulong[] price_Store = new ulong[7] {10000,8000,7000,6000,4000,3500,3000};
    public ulong[] price_Store_std = new ulong[7] { 10000, 8000, 7000, 6000, 4000, 3500, 3000 };
	public ulong[] addPrice = new ulong[7] { 1000, 500, 500, 500, 100, 100, 100 };
    public ulong price_percentage = 1;
    public ulong costMultiple = 36;

	public ulong price;
	public ulong[] WaitingRentTimeMoney { get { return rent.rentData.waitingRentTimeMoney; } set { rent.rentData.waitingRentTimeMoney = value; } }
	public string[] WaitingRentNames { get { return rent.rentData.waitingRentNames; } set { rent.rentData.waitingRentNames = value; } }
	public string[] WaitingRentTypes { get { return rent.rentData.waitingRentTypes; } set { rent.rentData.waitingRentTypes = value; } }

	public ulong check_timemoney;
	public int storeNum;
	public int percentage;
	public int CheckEmpty { get { return rent.rentData.checkEmpty ; } set { rent.rentData.checkEmpty = value; } }
	private string newStoreName;
	private ulong newStoreTimeMoney;
	private string type;

	private int turn;

	private int autoFillIndex = 0;

	public int NumofAccepted { get { return rent.rentData.totalAccepted; } set { rent.rentData.totalAccepted = value; } }

	void Start ()
    {
        StartCoroutine(WaitRentCall(Random.Range(150f, 270f)));
		if (QuickRentCoolTimefast < 120)
		{
			SetQuickRentButton(true);
			StartCoroutine(CoolTimeTimer());
		}
	}
	public void SetWaitingList()
    {
		for (int i = 0; i < nameTexts.Length; i++)
			SetWaitingListText(i);
		CheckEmptyImage();
	}

    IEnumerator WaitRentCall(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (CheckEmpty < 5)
        {
            RequestRent();
            WriteOnButton();
			messageManager.ShowMessage("새로운 입점 신청이 들어왔습니다.", 1);
            //SaveStore();
            CheckEmptyImage();
        }
        
        StartCoroutine(WaitRentCall(Random.Range(150f,270f)));
    }
    public void CheckEmptyImage()
    {
		for (int i = 0; i < acceptButtons.Length; i++)
		{
			if (nameTexts[i].text.Equals(""))
			{
				emptyObjs[i].SetActive(true);
				acceptButtons[i].enabled = false;
			}
			else
			{
				emptyObjs[i].SetActive(false);
				acceptButtons[i].enabled = true;
			}
		}

    }
    public void QuickRent()
	{
		messageManager.ShowMessage("입점 신청을 받았습니다.", 1.5f);
		autoFillIndex = 0;
		CheckEmpty = 0;
		SetQuickRentButton(true);
		StartCoroutine(AddRent_Auto());
		StartCoroutine(CoolTimeTimer());
	}

	private void SetQuickRentButton(bool isCoolDown)
    {
		if(isCoolDown)
        {
			quickRentCoolDown = true;
			quickRentButton.enabled = false;
			quickRentButton.image.color = Gray;
		}
		else
		{
			quickRentCoolDown = false;
			quickRentButton.enabled = true;
			quickRentButton.image.color = Color.white;
		}
    }

	IEnumerator CoolTimeTimer()
	{
		yield return new WaitForSeconds(1);

		if (QuickRentCoolTimefast > 0)
		{
			QuickRentCoolTimefast--;
		}
		else
		{
			QuickRentCoolTimefast = 120;
			SetQuickRentButton(false);
		}
		cooltime_text.text = QuickRentCoolTimefast + "초";

		if(quickRentCoolDown)
			StartCoroutine(CoolTimeTimer());
	}
	IEnumerator AddRent_Auto()
    {
        yield return new WaitForSeconds(0.1f);
        RequestRent();
        WriteOnButton();
        //SaveStore();
        CheckEmptyImage();
        autoFillIndex++;
        if(autoFillIndex < 5)
        {
            StartCoroutine(AddRent_Auto());
        }
    }
   
	public void PressKey_Check(int nKey)
	{
		switch (nKey) {
		case 1://yes
			CheckAccept_Menu.SetActive (false);
			AfterAccept ();
			break;
		case 2://no
			CheckAccept_Menu.SetActive (false);
			PushTexts ();
			//SaveStore ();
			break;
		case 3://보류
			CheckAccept_Menu.SetActive(false);
			break;
		}
	}

	public void OpenAcceptCheck(int index)
	{
		Cal_Price();
		CheckAccept_Menu.SetActive(true);
		turn = index;
		AfterClick(index);
	}

	void RequestRent()
	{
		percentage = Random.Range (1,101);
        Cal_Price();
        if (percentage <= 7) {
			newStoreName = "레이저";
            newStoreTimeMoney = price_Store[0] * price_percentage;
			type = "화장품";
        } else if (percentage <= 18) {
			newStoreName = "불낸여우";
			newStoreTimeMoney = price_Store[1] * price_percentage;
			type = "악세서리";
        } else if (percentage <= 29) {
			newStoreName = "착한고양이";
			newStoreTimeMoney = price_Store[2] * price_percentage;
			type = "악세서리";
        } else if (percentage <= 40) {
			newStoreName = "비트박스";
			newStoreTimeMoney = price_Store[3] * price_percentage;
			type = "악세서리";
        } else if (percentage <= 60) {
			newStoreName = "CS25개";
			newStoreTimeMoney = price_Store[4] * price_percentage;
			type = "편의점";
        } else if (percentage <= 80) {
			newStoreName = "ICU";
			newStoreTimeMoney = price_Store[5] * price_percentage;
			type = "편의점";
        } else if (percentage <= 100) {
			newStoreName = "작은톱";
			newStoreTimeMoney = price_Store[6] * price_percentage;
			type = "편의점";
        }
    }
	private void WriteOnButton()
    {
		WaitingRentNames[CheckEmpty] = newStoreName;
		WaitingRentTimeMoney[CheckEmpty] = newStoreTimeMoney;
		WaitingRentTypes[CheckEmpty] = type;
		WaitingRentTimeMoney[CheckEmpty] = newStoreTimeMoney;
		SetWaitingListText(CheckEmpty);
		CheckEmpty++;
    }
	private void PushTexts()
	{
		for(int i = turn; i < 4; i++)
        {
			WaitingRentNames[i] = WaitingRentNames[i + 1];
			WaitingRentTypes[i] = WaitingRentTypes[i + 1];
			WaitingRentTimeMoney[i] = WaitingRentTimeMoney[i + 1];
			SetWaitingListText(i);
        }
		WaitingRentNames[4] = "";
		WaitingRentTypes[4] = "";
		WaitingRentTimeMoney[4] = 0;
		SetWaitingListText(4);
		CheckEmpty--;
	}

	private void SetWaitingListText(int index)
	{
		if (!string.IsNullOrEmpty(WaitingRentNames[index]))
		{
			nameTexts[index].text = "가게이름: " + WaitingRentNames[index];
			string timemoney = string.Format("{0:#,###}", WaitingRentTimeMoney[index]);
			timeMoneyTexts[index].text = "초당 + " + timemoney + "$";
			typeTexts[index].text = "가게종류: " + WaitingRentTypes[index];
		}
		else
        {
			nameTexts[index].text = "";
			timeMoneyTexts[index].text = "";
			typeTexts[index].text = "";
		}
	}

	void AfterClick(int index)
	{
		price = WaitingRentTimeMoney[index] * costMultiple;
		Check_Name.text = nameTexts[index].text;
		string priceStr = string.Format("{0:#,###}", price);
		Check_Price.text = "중계 비용: " + priceStr + "$";
		Check_TM.text = timeMoneyTexts[index].text;
		Check_Type.text = typeTexts[index].text;
		check_timemoney = WaitingRentTimeMoney[index];
	}
	void AfterAccept()
	{
		if (addRent_Controller.NumOfRentSpace > NumofAccepted) {
            if (AssetMoneyCalculator.instance.ArithmeticOperation(price, 0, false))
            {
				MyAsset.instance.TimeEarningOperator(check_timemoney, 0, true);
                NumofAccepted++;
                CheckStore();
                PushTexts();
				CheckEmptyImage();
				//SaveStore();
				DataManager.instance.SaveAll();
				//ValuePoint_Manager.instance.CheckPoint();
				condition_Rent_Button.UpdateText();
            }
            else
            {
				messageManager.ShowMessage("돈이 부족하여 보류처리가 되었습니다.", 1.0f);
            }
        }
		else
        {
			messageManager.ShowMessage("임대시설이 부족하여 보류처리가 되었습니다.",1.0f);
		}
	}
	void CheckStore()
	{
		if (Check_Name.text == "가게이름: 레이저") {
			numOfRented[0]++;
		} else if (Check_Name.text == "가게이름: 불낸여우") {
			numOfRented[1]++;
		} else if (Check_Name.text == "가게이름: 착한고양이") {
			numOfRented[2]++;
		} else if (Check_Name.text == "가게이름: 비트박스") {
			numOfRented[3]++;
		} else if (Check_Name.text == "가게이름: CS25개") {
			numOfRented[4]++;
		} else if (Check_Name.text == "가게이름: ICU") {
			numOfRented[5]++;
		} else if (Check_Name.text == "가게이름: 작은톱") {
			numOfRented[6]++;
		}
	}
    private void Cal_Price()
    {
		for(int i = 0; i <price_Store.Length; i++)
        {
			price_Store[i] = (ulong)numOfRented[i] * addPrice[i] + price_Store_std[i];
		}
    }
	/*
	public static void SaveData()
	{
		PlayerPrefs.SetInt ("updated_empty",updated_empty);

		PlayerPrefs.SetInt ("NumofLazer",Numoflazer);
		PlayerPrefs.SetInt ("NumofFox",Numoffox);
		PlayerPrefs.SetInt ("NumofCat",Numofcat);
		PlayerPrefs.SetInt ("NumofBox",Numofbox);
		PlayerPrefs.SetInt ("NumofCs25",Numofcs25);
		PlayerPrefs.SetInt ("NumofIcu",Numoficu);
		PlayerPrefs.SetInt ("NumofMinitop",Numofminitop);
		PlayerPrefs.SetInt ("NumofAccepted",NumofAccepted);
	}

	public static void LoadData()
	{
		Numoflazer = PlayerPrefs.GetInt ("NumofLazer");
		Numoffox = PlayerPrefs.GetInt ("NumofFox");
		Numofcat = PlayerPrefs.GetInt ("NumofCat");
		Numofbox = PlayerPrefs.GetInt ("NumofBox");
		Numofcs25 = PlayerPrefs.GetInt ("NumofCs25");
		Numoficu = PlayerPrefs.GetInt ("NumofIcu");
		Numofminitop = PlayerPrefs.GetInt ("NumofMinitop");
		NumofAccepted = PlayerPrefs.GetInt ("NumofAccepted");
	}
	public static void ResetData()
	{
		Numoflazer = 0;
		Numoffox = 0;
		Numofcat = 0;
		Numofbox = 0;
		Numofcs25 = 0;
		Numoficu = 0;
		Numofminitop = 0;
		NumofAccepted = 0;
	}
	void SaveStore()
	{
		PlayerPrefs.SetInt ("CheckEmpty",CheckEmpty);

		PRICE1 = "" + price1;
		PRICE2 = "" + price2;
		PRICE3 = "" + price3;
		PRICE4 = "" + price4;
		PRICE5 = "" + price5;

		PlayerPrefs.SetString ("PRICE1",PRICE1);
		PlayerPrefs.SetString ("PRICE2",PRICE2);
		PlayerPrefs.SetString ("PRICE3",PRICE3);
		PlayerPrefs.SetString ("PRICE4",PRICE4);
		PlayerPrefs.SetString ("PRICE5",PRICE5);

		PlayerPrefs.SetString ("Name1",Name1.text);
		PlayerPrefs.SetString ("Type1",type1.text);
		PlayerPrefs.SetString ("tm1",tm1.text);

		PlayerPrefs.SetString ("Name2",Name2.text);
		PlayerPrefs.SetString ("Type2",type2.text);
		PlayerPrefs.SetString ("tm2",tm2.text);

		PlayerPrefs.SetString ("Name3",Name3.text);
		PlayerPrefs.SetString ("Type3",type3.text);
		PlayerPrefs.SetString ("tm3",tm3.text);

		PlayerPrefs.SetString ("Name4",Name4.text);
		PlayerPrefs.SetString ("Type4",type4.text);
		PlayerPrefs.SetString ("tm4",tm4.text);

		PlayerPrefs.SetString ("Name5",Name5.text);
		PlayerPrefs.SetString ("Type5",type5.text);
		PlayerPrefs.SetString ("tm5",tm5.text);
	}
	void LoadStore()
	{
		CheckEmpty = PlayerPrefs.GetInt ("CheckEmpty");

		PRICE1 = PlayerPrefs.GetString ("PRICE1");
		PRICE2 = PlayerPrefs.GetString ("PRICE2");
		PRICE3 = PlayerPrefs.GetString ("PRICE3");
		PRICE4 = PlayerPrefs.GetString ("PRICE4");
		PRICE5 = PlayerPrefs.GetString ("PRICE5");

		price1 = ulong.Parse (PRICE1);
		price2 = ulong.Parse (PRICE2);
		price3 = ulong.Parse (PRICE3);
		price4 = ulong.Parse (PRICE4);
		price5 = ulong.Parse (PRICE5);

		Name1.text = PlayerPrefs.GetString ("Name1");
		type1.text = PlayerPrefs.GetString ("Type1");
		tm1.text = PlayerPrefs.GetString ("tm1");

		Name2.text = PlayerPrefs.GetString ("Name2");
		type2.text = PlayerPrefs.GetString ("Type2");
		tm2.text = PlayerPrefs.GetString ("tm2");

		Name3.text = PlayerPrefs.GetString ("Name3");
		type3.text = PlayerPrefs.GetString ("Type3");
		tm3.text = PlayerPrefs.GetString ("tm3");

		Name4.text = PlayerPrefs.GetString ("Name4");
		type4.text = PlayerPrefs.GetString ("Type4");
		tm4.text = PlayerPrefs.GetString ("tm4");

		Name5.text = PlayerPrefs.GetString ("Name5");
		type5.text = PlayerPrefs.GetString ("Type5");
		tm5.text = PlayerPrefs.GetString ("tm5");
	}*/
}
