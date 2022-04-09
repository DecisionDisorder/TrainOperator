using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Company_Peace_Controller : MonoBehaviour {

	public Text Peace_text;
	public Text Peace_Conditon_Text;

	public GameObject Crime_Menu;
    public GameObject Fail_Menu;
	public GameObject CrimeImage_1;
	public GameObject CrimeImage_2;
	public GameObject CrimeImage_3;

	public SettingManager button_Option;
	public CompanyReputationManager company_Reputation_Controller;

	public int PeaceValue { get { return company_Reputation_Controller.companyData.peaceValue ; } set { company_Reputation_Controller.companyData.peaceValue = value; } }
	public int peaceCondition;

	public static int peace_updated = 0;

	public static int eventOccurTimer;

	public UpdateDisplay stationConditionUpdateDisplay;

	void Start ()
    {
        StartCoroutine(Timer());

        //LoadData();
        
		eventOccurTimer = Random.Range (60,301);
		CheckPeace ();
		SetCrimePercentage();
		SetInfoText();
		stationConditionUpdateDisplay.onEnableUpdate += SetInfoText;
	}

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1);

		if (button_Option.PeaceEventGameActive)
        {
            eventOccurTimer--;
            if (eventOccurTimer < 1)
            {
                executeCrime();
                eventOccurTimer = Random.Range(60, 301);
            }
		}

		CheckPeace();
		SetCrimePercentage();
        
        StartCoroutine(Timer());
    }

	public void SetInfoText()
	{
		Peace_text.text = "안전도: " + PeaceValue + "점";
		CheckPeace();
	}

	private void SetCrimePercentage()
    {
		if (peaceCondition.Equals(1))
		{
			PeaceManager.Percentage_Crime = 50;
		}
		else if (peaceCondition.Equals(2))
		{
			PeaceManager.Percentage_Crime = 60;
		}
		else if (peaceCondition.Equals(3))
		{
			PeaceManager.Percentage_Crime = 75;
		}
		else if (peaceCondition.Equals(4))
		{
			PeaceManager.Percentage_Crime = 90;
		}
		else if (peaceCondition.Equals(5))
		{
			PeaceManager.Percentage_Crime = 100;
		}
	}
    
    public void PressKey_test(int nKey)
    {
        switch (nKey)
        {
            case 0:
                executeCrime();
                break;
        }
    }

	void CheckPeace()
	{
		if(PeaceValue < -40)
		{
			Peace_Conditon_Text.text = "상태: " + "매우 위험함";
			peaceCondition = 1;
            PeaceManager.GageUP = 8;
        }
		if(-40 <= PeaceValue && PeaceValue < 0)
		{
			Peace_Conditon_Text.text = "상태: " + "위험함";
			peaceCondition = 2;
            PeaceManager.GageUP = 8;

        }
		if(0 <= PeaceValue && PeaceValue <= 50)
		{
			Peace_Conditon_Text.text = "상태: " + "보통";
			peaceCondition = 3;
            PeaceManager.GageUP = 10;

        }
		if(50 < PeaceValue && PeaceValue <= 80)
		{
			Peace_Conditon_Text.text = "상태: " + "안전함";
			peaceCondition = 4;
            PeaceManager.GageUP = 16;

        }
		if(80 < PeaceValue && PeaceValue <= 100)
		{
			Peace_Conditon_Text.text = "상태: " + "매우 안전함";
			peaceCondition = 5;
            PeaceManager.GageUP = 24;

        }
	}

	void executeCrime()
	{
		int crimeR = Random.Range (0,101);
		if(0 <= crimeR && crimeR < 101)
		{
			CrimeImage_1.SetActive (true);
            Fail_Menu.SetActive(false);
			Crime_Menu.SetActive (true);
			PeaceManager.Gage = 50;
		}
		else if(30 <= crimeR && crimeR < 60)
		{
			CrimeImage_2.SetActive (true);
            Fail_Menu.SetActive(false);
            Crime_Menu.SetActive (true);
			PeaceManager.Gage = 50;
		}
		else if(60 <= crimeR && crimeR < 101)
		{
			CrimeImage_3.SetActive (true);
            Fail_Menu.SetActive(false);
            Crime_Menu.SetActive(true);
			PeaceManager.Gage = 50;
		}
	}/*
	public static void SaveData()
	{
		PlayerPrefs.SetInt ("Crime_Percentage",button_Peace.Percentage_Crime);
		PlayerPrefs.SetInt ("Peace_Value",Peace_value);
		PlayerPrefs.SetInt ("Peace_Condition",Peace_condition);
	}
	public static void LoadData()
	{
		button_Peace.Percentage_Crime = PlayerPrefs.GetInt ("Crime_Percentage");
		Peace_value = PlayerPrefs.GetInt ("Peace_Value",20);
		Peace_condition = PlayerPrefs.GetInt ("Peace_Condition");
	}
	public static void ResetData()
	{
		button_Peace.Percentage_Crime = 60;
		Peace_value = 20;
	}*/
}
