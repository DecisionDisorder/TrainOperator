using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TouchEarning : MonoBehaviour {

    public Text touchpersecond_text;
    public MessageManager messageManager;
    public SettingManager button_Option;
    public LevelManager levelManager;
    public MacroDetector macroDetector;
    public ItemManager itemManager;
    //-----------------------------------------------------------------------------
    public GameObject TouchMoney_Menu;
	//-----------------------------------------------------------------------------
	public static ulong passengerRandomFactor;
	//-----------------------------------------------------------------------------
	
	public static int randomSetTime = 0;

    public int touchPerSecond;

	public AudioSource touchAudio;
	public AudioClip audioclip;

    public Animation[] addedMoneyEffectAni;
    public Text[] addedMoneyTexts;
    private int currentIndex = 0;
    private int numOfChanged = 0;

    void Start()
    {
        StartCoroutine(Timer(0));
    }
    IEnumerator Timer(float delay = 1)
    {
        yield return new WaitForSeconds(delay);

        if(!itemManager.itemActived)
            macroDetector.DetectTouchAmount(touchPerSecond);

        randomSetTime--;
        touchPerSecond = 0;
        if (randomSetTime < 1 && MyAsset.instance.myAssetData.passengersLow >= 10)
        {
            passengerRandomFactor = (ulong)Random.Range(70, 131);
            CompanyReputationManager.instance.RenewPassengerBase();
            randomSetTime = 60;
            if(!numOfChanged.Equals(0))
                messageManager.ShowMessage("승객 수에 따라 터치당 수익이 변경되었습니다.", 1f);
            numOfChanged++;
        }
        else if (MyAsset.instance.myAssetData.passengersLow.Equals(0))
        {
            MyAsset.instance.myAssetData.passengersLow = 1;
            passengerRandomFactor = 100;
            TouchMoneyManager.PassengersRandomLow = MyAsset.instance.myAssetData.passengersLow;
        }
        StartCoroutine(Timer());
    }

	public void TouchIncome()
	{
        touchAudio.PlayOneShot(audioclip);
        touchPerSecond++;
        AssetMoneyCalculator.instance.ArithmeticOperation(TouchMoneyManager.TouchMoneyLow, TouchMoneyManager.TouchMoneyHigh, true);
        levelManager.AddExp();
        if(button_Option.AddedMoneyEffect)
            AddedMoneyEffect();

        if (touchPerSecond != 0)
            touchpersecond_text.text = "초당 " + touchPerSecond + "회 터치";

        if (!itemManager.itemActived)
            macroDetector.DetectInterval();
    }

    private void AddedMoneyEffect()
    {
        string m2 = "", m1 = "";
        if (TouchMoneyManager.TouchMoneyHigh > 0)
        {
            PlayManager.ArrangeUnit(TouchMoneyManager.TouchMoneyLow, TouchMoneyManager.TouchMoneyHigh, ref m1, ref m2);
            addedMoneyTexts[currentIndex].text = "+" + m2 + m1 + "$";
        }
        else
        {
            if (TouchMoneyManager.TouchMoneyLow >= 1000000000000)
                PlayManager.ArrangeUnit(TouchMoneyManager.TouchMoneyLow, 0, ref m1, ref m2, true);
            else
                m1 = string.Format("{0:#,##0}", TouchMoneyManager.TouchMoneyLow);
            addedMoneyTexts[currentIndex].text = "+" + m1 + "$";
        }

        addedMoneyEffectAni[currentIndex].Play();

        currentIndex++;
        if (currentIndex >= addedMoneyEffectAni.Length)
            currentIndex = 0;
    }
}
