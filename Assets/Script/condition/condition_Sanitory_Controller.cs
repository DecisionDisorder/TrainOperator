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

    public GameObject[] dirtyImgs;
    public GameObject[] cleanImgs;
    private int currentIndex = 0;
    IEnumerator cSetSanitoryImage;

    public UpdateDisplay conditionSanitoryUpdateDisplay;

    void Start()
    {
        conditionSanitoryUpdateDisplay.onEnableUpdate += UpdateText;
        StartCoroutine(Timer(SetTime()));

        CheckSanitory();
    }

    IEnumerator Timer(float time)
    {
        yield return new WaitForSeconds(time);

        if (SanitoryValue > -100)
        {
            SanitoryValue -= MinusSanitory();
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

        if (SanitoryValue == 0)
        {
            Train_text.text = "0점";
        }
        else
        {
            Average_text.text = SanitoryValue + "점";
        }
    }
    int SetTime()
    {
        int Timeset;
        if (!LineManager.instance.lineCollections[4].IsExpanded())
        {
            Timeset = Random.Range(60, 86);
        }
        else if (!LineManager.instance.lineCollections[9].IsExpanded())
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
        if (!LineManager.instance.lineCollections[4].IsExpanded())
        {
            Score = Random.Range(0, 4);
        }
        else if (!LineManager.instance.lineCollections[9].IsExpanded())
        {
            Score = Random.Range(2, 5);
        }
        else
        {
            Score = Random.Range(3, 7);
        }
        return Score;
    }

	public void CheckSanitory()
	{
		if(-100 <= SanitoryValue && SanitoryValue < -40)
		{
			Condition_text.text = "상태: " + "역겨움";
			Condition = 1; 
            company_Reputation_Controller.RenewReputation();
		}
		if(-40 <= SanitoryValue && SanitoryValue < 0)
		{
			Condition_text.text = "상태: " + "더러움";
			Condition = 2;
            company_Reputation_Controller.RenewReputation();
		}
		if(0 <= SanitoryValue && SanitoryValue <= 50)
		{
			Condition_text.text = "상태: " + "평범함";
			Condition = 3;
            company_Reputation_Controller.RenewReputation();
		}
		if(50 < SanitoryValue && SanitoryValue <= 80)
		{
			Condition_text.text = "상태: " + "깨끗함";
			Condition = 4;
            company_Reputation_Controller.RenewReputation();
		}
		if(80 < SanitoryValue && SanitoryValue <= 100)
		{
			Condition_text.text = "상태: " + "아주 깔끔함";
			Condition = 5;
            company_Reputation_Controller.RenewReputation();
		}
	}

    IEnumerator CSetSanitoryImage(float delay)
    {
        if (Condition < 3)
        {
            cleanImgs[0].SetActive(false);
            cleanImgs[1].SetActive(false);

            dirtyImgs[currentIndex].SetActive(false);
            dirtyImgs[GetNextIndex()].SetActive(true);
        }
        else if (Condition == 3)
        {
            cleanImgs[0].SetActive(false);
            cleanImgs[1].SetActive(false);

            dirtyImgs[0].SetActive(false);
            dirtyImgs[1].SetActive(false);
        }
        else if (Condition > 3)
        {
            cleanImgs[currentIndex].SetActive(false);
            cleanImgs[GetNextIndex()].SetActive(true);

            dirtyImgs[0].SetActive(false);
            dirtyImgs[1].SetActive(false);
        }
        yield return new WaitForSeconds(delay);

        if(Condition != 3)
            StartCoroutine(cSetSanitoryImage = CSetSanitoryImage(delay));
    }

    private bool IsArrayObjectActive(GameObject[] objects)
    {
        for(int i = 0; i < objects.Length; i++)
        {
            if (objects[i].activeInHierarchy)
                return true;
        }
        return false;
    }

    private bool NeedSanitoryImageChange()
    {
        if (Condition < 3 && !IsArrayObjectActive(dirtyImgs))
            return true;
        else if (Condition > 3 && !IsArrayObjectActive(cleanImgs))
            return true;
        else if (Condition == 3 && (IsArrayObjectActive(cleanImgs) || IsArrayObjectActive(dirtyImgs)))
            return true;
        else
            return false;
    }

    private void SetSanitoryImage()
    {
        if (NeedSanitoryImageChange())
        {
            if (cSetSanitoryImage != null)
                StopCoroutine(cSetSanitoryImage);

            cSetSanitoryImage = CSetSanitoryImage(0.8f);
            StartCoroutine(cSetSanitoryImage);
        }
    }
    private int GetNextIndex()
    {
        currentIndex = currentIndex == 0 ? 1 : 0;
        return currentIndex;
    }
}
