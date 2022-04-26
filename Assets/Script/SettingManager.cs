using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour {

    public SettingData settingData;

	public GameObject Main_Menu;
	public GameObject Option_Menu;
	public GameObject Reset_Menu;
	public GameObject Key_Menu;
    public GameObject PW_Menu;

    public Slider soundSlider;
    public float SoundVolume { get { return settingData.soundVolume; } set { settingData.soundVolume = value; } }
    public AudioSource[] effectSounds;

	public Color Disabled_color = new Vector4 (0,0,0,255);

	public bool PlaytimeActive { get { return settingData.playTimeActive; } set { settingData.playTimeActive = value; } } // int to bool
    public GameObject Playtime;
	public Image Playtime_button;
    public Text playTime_text;

    public Text updateAlarm_text;
    public Image updateAlarm_optionbutton;
    public bool UpdateAlarm { get { return settingData.updateAlarm; } set { settingData.updateAlarm = value; } } // int to bool

    public bool MiniGameActive { get { return settingData.peaceEventGameActive; } set { settingData.peaceEventGameActive = value; } }
    public Image Peace_button;
    public Text peace_text;

    public bool AddedMoneyEffect  { get { return settingData.addedMoneyEffect; } set { settingData.addedMoneyEffect = value; } }
    public Image addedMoneyEImg;
    public Text addedMoneyEText;

    public float epFastInterval = 0.15f;
    public float epNormalInterval = 0.25f;
    public ContinuousPurchase continuousPurchase;
    public Image[] easyPurchaseButtons;
    private int EasyPurchaseType { get { return settingData.easyPurchaseType; } set { settingData.easyPurchaseType = value; }
    }
    public bool BackupRecommend { 
        get {
            if (settingData.backupRecommend.Equals(0))
            {
                settingData.backupRecommend = 1;
                return true;
            }
            else if (settingData.backupRecommend.Equals(1))
                return true;
            else
                return false;
        }
        private set {
            if (value.Equals(true))
                settingData.backupRecommend = 1;
            else
                settingData.backupRecommend = 2;
        } 
    }
    public Image backupRecommendImg;
    public Text backupRecommendText;

    void Start () {
        Application.targetFrameRate = 60;
		//LoadOption ();

        soundSlider.value = SoundVolume;
        VolumeSet();
        SetPlayTime(PlaytimeActive);
        SetPatchAlarm(UpdateAlarm);
        SetMiniGame(MiniGameActive);
        SetEasyPurchaseSpeed(EasyPurchaseType);
        SetAddedMoneyEffect(AddedMoneyEffect);
        SetBackupRecommend(BackupRecommend);
    }
	public void PressKey(int nKey)
	{
        switch (nKey)
        {
            case 1://back
                Main_Menu.SetActive(true);
                Option_Menu.SetActive(false);
                DataManager.instance.SaveAll();
                break;
            case 2://reset
                Reset_Menu.SetActive(true);
                break;
            case 3://moneyKey
                Key_Menu.SetActive(true);
                if(Application.platform.Equals(RuntimePlatform.Android))
                {
                    PW_Menu.SetActive(true);
                }
                break;
            case 5://playtime
                SetPlayTime(!PlaytimeActive);
                break;
            case 7://updatealarm
                SetPatchAlarm(!UpdateAlarm);
                break;
            case 9:
                SetMiniGame(!MiniGameActive);
                break;
            case 11:
                SetAddedMoneyEffect(!AddedMoneyEffect);
                break;
            case 12:
                SetBackupRecommend(!BackupRecommend);
                break;
        }
	}

    private void SetPlayTime(bool active)
    {
        if (!active)
        {
            Playtime.SetActive(false);
            playTime_text.text = "플레이시간: OFF";
            Playtime_button.color = Disabled_color;
            PlaytimeActive = false;
        }
        else
        {
            Playtime.SetActive(true);
            playTime_text.text = "플레이시간: ON";
            Playtime_button.color = Color.white;
            PlaytimeActive = true;
        }
    }

    private void SetPatchAlarm(bool active)
    {
        if (!active)
        {
            updateAlarm_text.text = "패치노트 알림: OFF";
            updateAlarm_optionbutton.color = Disabled_color;
            UpdateAlarm = false;
        }
        else
        {
            updateAlarm_text.text = "패치노트 알림: ON";
            updateAlarm_optionbutton.color = Color.white;
            UpdateAlarm = true;
        }
    }

    private void SetMiniGame(bool active)
    {
        if (!active)
        {
            Peace_button.color = Disabled_color;
            peace_text.text = "미니게임 OFF";
            MiniGameActive = false;
        }
        else
        {
            Peace_button.color = Color.white;
            peace_text.text = "미니게임 ON";
            MiniGameActive = true;
        }
    }

	public void PressKey_Key(int nKey)
	{
        switch (nKey)
        {
            case 1://back
                Key_Menu.SetActive(false);
                break;
        }
    }

    private void SetAddedMoneyEffect(bool onOff)
    {
        if (!onOff)
        {
            addedMoneyEImg.color = Disabled_color;
            addedMoneyEText.text = "수익 효과 OFF";
            AddedMoneyEffect = onOff;
        }
        else
        {
            addedMoneyEImg.color = Color.white;
            addedMoneyEText.text = "수익 효과 ON";
            AddedMoneyEffect = onOff;
        }
    }

    private void SetBackupRecommend(bool active)
    {
        if(active)
        {
            backupRecommendImg.color = Color.white;
            backupRecommendText.text = "백업 권유: ON";
        }
        else
        {
            backupRecommendImg.color = Disabled_color;
            backupRecommendText.text = "백업 권유: OFF";
        }
        BackupRecommend = active;
    }

    public void SetEasyPurchaseSpeed(int type)
    {
        EasyPurchaseType = type;
        switch(type)
        {
            case 0: // Fast
                continuousPurchase.isAllowedEasyPurchase = true;
                continuousPurchase.SetInterval(epFastInterval);
                easyPurchaseButtons[0].color = Color.gray;
                easyPurchaseButtons[1].color = easyPurchaseButtons[2].color = Color.white;
                break;
            case 1: // Normal
                continuousPurchase.isAllowedEasyPurchase = true;
                continuousPurchase.SetInterval(epNormalInterval);
                easyPurchaseButtons[1].color = Color.gray;
                easyPurchaseButtons[0].color = easyPurchaseButtons[2].color = Color.white;
                break;
            case 2: // OFF
                continuousPurchase.isAllowedEasyPurchase = false;
                easyPurchaseButtons[2].color = Color.gray;
                easyPurchaseButtons[0].color = easyPurchaseButtons[1].color = Color.white;
                break;
        }
    }

    public void VolumeSet()
    {
        SoundVolume = soundSlider.value;
        for (int i = 0; i < effectSounds.Length; i++)
            effectSounds[i].volume = SoundVolume;
    }

    /*
	public static void SaveOption()
	{
        PlayerPrefs.SetFloat("soundVolume", soundVolume);
		PlayerPrefs.SetInt ("Bool_Playtime_o",bool_playtime);
        PlayerPrefs.SetInt ("Bool_updateAlarm",bool_updateAlarm);
        PlayerPrefs.SetString("Bool_peace",""+bool_peace);
        PlayerPrefs.SetString("Bool_peddler", "" + bool_peddler);
        PlayerPrefs.SetInt("EasyPurchaseType", easyPurchaseType);
        PlayerPrefs.SetString("BoolAddedMoneyEffect", "" + addedMoneyEffect);
    }
	public static void LoadOption()
	{
		soundVolume = PlayerPrefs.GetFloat ("soundVolume", 1f);
		bool_playtime = PlayerPrefs.GetInt ("Bool_Playtime_o");
        bool_updateAlarm = PlayerPrefs.GetInt("Bool_updateAlarm");
        bool_peace = bool.Parse(PlayerPrefs.GetString("Bool_peace", "true"));
        bool_peddler = bool.Parse(PlayerPrefs.GetString("Bool_peddler", "true"));
        easyPurchaseType = PlayerPrefs.GetInt("EasyPurchaseType", 1);
        addedMoneyEffect = bool.Parse(PlayerPrefs.GetString("BoolAddedMoneyEffect", "true"));

    }*/
}
