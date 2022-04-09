using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PeddlerEvent_Manager : MonoBehaviour {

	public AudioSource homeButtonAudio;
	public AudioClip audioclip;

	public GameObject PeddlerEvent_Menu;
	public GameObject G920_Menu;
	public GameObject Select_Menu;

	public GameObject[] Train_button = new GameObject[3];
	public GameObject[] peddler_image = new GameObject[3];

	public Text Result_text;
	public GameObject close_button;

	public SettingManager button_Option;

	private int random_peddler;
	public static ulong bonusMoney;
    public static ulong bonusMoney2;

	public int clearTime;

	public static int eventOccurTimer;

	void Start ()
	{
		StartCoroutine(Timer(Random.Range(90, 300)));
    }
	
    IEnumerator Timer(float wTime)
    {
        yield return new WaitForSeconds(wTime);
        if (button_Option.PeddlerEventGameActive)
        {
            Initialize_Peddler();
            PeddlerEvent_Menu.SetActive(true);
        }
        wTime = Random.Range(90, 300);
        StartCoroutine(Timer(wTime));
    }
    public void PressKey_test(int nKey)
    {
        switch (nKey)
        {
            case 0:
                Initialize_Peddler();
                PeddlerEvent_Menu.SetActive(true);
                break;
        }
    }
    public void PressKey_homebutton (int nKey) 
	{
        switch (nKey)
        {
            case 0://homebutton
                homeButtonAudio.PlayOneShot(audioclip);
                Select_Menu.SetActive(true);
                G920_Menu.SetActive(false);
                RandomSummon();
                break;
            case 1:
                G920_Menu.SetActive(false);
                PeddlerEvent_Menu.SetActive(false);
                break;
        }
	}
	public void PressKey_select(int nKey)
	{
		switch (nKey)
		{
			case 0://Close
				Initialize_Peddler();
				break;
			case 1://1st
				Train_button[0].SetActive(false);
				Train_button[1].SetActive(false);
				Train_button[2].SetActive(false);
				if (peddler_image[0].activeInHierarchy)
				{
					StartCoroutine(Success(1f));

				}
				else
				{
					StartCoroutine(Fail(1f));
				}
				break;
			case 2:
				Train_button[0].SetActive(false);
				Train_button[1].SetActive(false);
				Train_button[2].SetActive(false);
				if (peddler_image[1].activeInHierarchy)
				{
					StartCoroutine(Success(1f));
				}
				else
				{
					StartCoroutine(Fail(1f));
				}
				break;
			case 3:
				Train_button[0].SetActive(false);
				Train_button[1].SetActive(false);
				Train_button[2].SetActive(false);
				if (peddler_image[2].activeInHierarchy)
				{
					StartCoroutine(Success(1f));
				}
				else
				{
					StartCoroutine(Fail(1f));
				}
				break;
		}
	}
	IEnumerator Success(float time)
	{
		yield return new WaitForSeconds(time);

		ulong addedMoneyLow = 0, addedMoneyHigh = 0;
		string money1 = "", money2 = "";
		PlayManager.SetMoneyReward(ref addedMoneyLow, ref addedMoneyHigh, clearTime);
		AssetMoneyCalculator.instance.ArithmeticOperation(addedMoneyLow, addedMoneyHigh, true);
		PlayManager.ArrangeUnit(addedMoneyLow, addedMoneyHigh, ref money1, ref money2);

		Result_text.text = "잡상인을 잡으셨습니다! 보상으로 " + money2 + money1 + "$을 지급 받았습니다.";
		close_button.SetActive (true);
	}
	IEnumerator Fail(float time)
	{
		yield return new WaitForSeconds(time);

		Result_text.text = "잡상인을 놓쳤습니다... 다음엔 꼭 잡으세요!";
		close_button.SetActive (true);
	}
	void RandomSummon()
	{
		random_peddler = Random.Range (1,91);
		if (random_peddler < 31) {
			peddler_image [0].SetActive (true);
		} else if (random_peddler < 61) {
			peddler_image [1].SetActive (true);
		} else if (random_peddler <= 91) {
			peddler_image [2].SetActive (true);
		}
	}
	void Initialize_Peddler()
	{
		PeddlerEvent_Menu.SetActive (false);
		Select_Menu.SetActive (false);
		close_button.SetActive (false);
		Result_text.text = "";
		G920_Menu.SetActive (true);
		Train_button [0].SetActive (true);
		Train_button [1].SetActive (true);
		Train_button [2].SetActive (true);
		peddler_image [0].SetActive (false);
		peddler_image [1].SetActive (false);
		peddler_image [2].SetActive (false);
	}
}
