using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class condition_Sanitory_Controller : MonoBehaviour {
    public CompanyReputationManager company_Reputation_Controller;
	public int SanitoryValue { get { return company_Reputation_Controller.companyData.averageSanitoryCondition; } set { company_Reputation_Controller.companyData.averageSanitoryCondition = value; } }

    public static int ChangedReputation = 0;

    private int condition = 0;
	public int Condition
    {
        get { return condition; }
        set
        {
            condition = value;
            SetSanitoryImage();
        }
    }

	public Text Train_text;
	public Text Station_text;
	public Text Average_text;
	public Text Condition_text;
	public Text ChangedRep_text;

    public GameObject[] dirtyImgs;
    public GameObject[] cleanImgs;
    private int currentIndex = 0;
    IEnumerator cSetSanitoryImage;

    public UpdateDisplay conditionSanitoryUpdateDisplay;

    void Start()
    {
        conditionSanitoryUpdateDisplay.onEnableUpdate += UpdateText;
        StartCoroutine(Timer(SetTime()));

        //LoadSanitory();
        CheckSanitory();
        //CalReputation();
    }

    IEnumerator Timer(float time)
    {
        yield return new WaitForSeconds(time);

        if (SanitoryValue > -100)
        {
            SanitoryValue -= MinusSanitory();
            //Debug.Log("-" + MinusSanitory());
            if (SanitoryValue < -100)
            {
                SanitoryValue = -100;
            }
        }
        UpdateText();

        StartCoroutine(Timer(SetTime()));
    }

    public void UpdateText()
    {
        CheckSanitory();
        //CalReputation();

        if (SanitoryValue == 0)
        {
            Train_text.text = "0점";
        }
        else
        {
            Average_text.text = SanitoryValue + "점";
        }

        if (ChangedReputation == 0)
        {
            ChangedRep_text.text = "변경된 고객 만족도: 0P";
        }
        else
        {
            ChangedRep_text.text = "변경된 고객 만족도: " + ChangedReputation + "P";
        }
    }
    int SetTime()
    {
        int Timeset;
        if (!LineManager.instance.lineCollections[4].isExpanded())
        {
            Timeset = Random.Range(60, 86);
        }
        else if (!LineManager.instance.lineCollections[9].isExpanded())
        {
            Timeset = Random.Range(55, 71);
        }
        else
        {
            Timeset = Random.Range(45, 61);
        }
        return Timeset;
    }
    int MinusSanitory()
    {
        int Score;
        if (!LineManager.instance.lineCollections[4].isExpanded())
        {
            Score = Random.Range(0, 4);
        }
        else if (!LineManager.instance.lineCollections[9].isExpanded())
        {
            Score = Random.Range(2, 5);
        }
        else
        {
            Score = Random.Range(3, 7);
        }
        return Score;
    }
	/*public void CalReputation()
	{
		ChangedReputation = CompanyReputationManager.reputationValueCalculated - company_Reputation_Controller.ReputationValue;
	}*/

	public void CheckSanitory()
	{
		if(-100 <= SanitoryValue && SanitoryValue < -40)
		{
			Condition_text.text = "상태: " + "역겨움";
			Condition = 1; 
            //ValuePoint_Manager.Sanitory_Percentage = 80;
            //company_Reputation_Controller.Rep_Percentage = 7;
            company_Reputation_Controller.RenewReputation();
		}
		if(-40 <= SanitoryValue && SanitoryValue < 0)
		{
			Condition_text.text = "상태: " + "더러움";
			Condition = 2;
            //ValuePoint_Manager.Sanitory_Percentage = 90;
            //company_Reputation_Controller.Rep_Percentage = 9;
            company_Reputation_Controller.RenewReputation();
		}
		if(0 <= SanitoryValue && SanitoryValue <= 50)
		{
			Condition_text.text = "상태: " + "평범함";
			Condition = 3;
            //ValuePoint_Manager.Sanitory_Percentage = 100;
            //company_Reputation_Controller.Rep_Percentage = 10;
            company_Reputation_Controller.RenewReputation();
		}
		if(50 < SanitoryValue && SanitoryValue <= 80)
		{
			Condition_text.text = "상태: " + "깨끗함";
			Condition = 4;
            //ValuePoint_Manager.Sanitory_Percentage = 120;
            //company_Reputation_Controller.Rep_Percentage = 11;
            company_Reputation_Controller.RenewReputation();
		}
		if(80 < SanitoryValue && SanitoryValue <= 100)
		{
			Condition_text.text = "상태: " + "아주 깔끔함";
			Condition = 5;
            //ValuePoint_Manager.Sanitory_Percentage = 140;
            //company_Reputation_Controller.Rep_Percentage = 12;
            company_Reputation_Controller.RenewReputation();
		}
	}

    IEnumerator CSetSanitoryImage(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (Condition < 3)
        {
            cleanImgs[0].SetActive(false);
            cleanImgs[1].SetActive(false);

            dirtyImgs[currentIndex].SetActive(false);
            dirtyImgs[GetNextIndex()].SetActive(true);
            StartCoroutine(cSetSanitoryImage = CSetSanitoryImage(delay));
        }
        else if (Condition == 3)
        {
            cleanImgs[0].SetActive(false);
            cleanImgs[1].SetActive(false);

            dirtyImgs[0].SetActive(false);
            dirtyImgs[0].SetActive(false);
        }
        else if (Condition > 3)
        {
            cleanImgs[currentIndex].SetActive(false);
            cleanImgs[GetNextIndex()].SetActive(true);

            dirtyImgs[0].SetActive(false);
            dirtyImgs[1].SetActive(false);
            StartCoroutine(cSetSanitoryImage = CSetSanitoryImage(delay));
        }
    }

    private void SetSanitoryImage()
    {
        if (cSetSanitoryImage != null)
            StopCoroutine(cSetSanitoryImage);

        cSetSanitoryImage = CSetSanitoryImage(0.8f);
        StartCoroutine(cSetSanitoryImage);
    }
    private int GetNextIndex()
    {
        currentIndex = currentIndex == 0 ? 1 : 0;
        return currentIndex;
    }
    /*
    public static void SaveSanitory()
	{
		PlayerPrefs.SetInt ("ASC",Average_Sanitory_Condition);
	}
	public static void LoadSanitory()
	{
		Average_Sanitory_Condition = PlayerPrefs.GetInt ("ASC",20);
	}
	public static void ResetSanitory()
	{
		Average_Sanitory_Condition = 20;
	}*/
}
