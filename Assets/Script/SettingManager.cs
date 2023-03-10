using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 게임 설정 관리 클래스
/// </summary>
public class SettingManager : MonoBehaviour {

    /// <summary>
    /// 설정 데이터 오브젝트
    /// </summary>
    public SettingData settingData;

    /// <summary>
    /// 좌측 메인 메뉴 오브젝트
    /// </summary>
	public GameObject Main_Menu;
    /// <summary>
    /// 설정 메뉴 오브젝트
    /// </summary>
	public GameObject Option_Menu;
    /// <summary>
    /// 게임 데이터 초기화 메뉴 오브젝트
    /// </summary>
	public GameObject Reset_Menu;
    /// <summary>
    /// 테스트 메뉴 오브젝트
    /// </summary>
	public GameObject Key_Menu;
    /// <summary>
    /// 테스트 비밀번호 확인 메뉴 오브젝트
    /// </summary>
    public GameObject PW_Menu;

    /// <summary>
    /// 효과음 볼륨 조절 슬라이더
    /// </summary>
    public Slider soundSlider;
    /// <summary>
    /// 효과음 볼륨 수치
    /// </summary>
    public float SoundVolume { get { return settingData.soundVolume; } set { settingData.soundVolume = value; } }
    /// <summary>
    /// 게임에서 재생되는 효과음 오디오 소스
    /// </summary>
    public AudioSource[] effectSounds;
    /// <summary>
    /// 비활성화 색상
    /// </summary>
	public Color Disabled_color = new Vector4 (0,0,0,255);

    /// <summary>
    /// 플레이타임 표기 선택 여부
    /// </summary>
	public bool PlaytimeActive { get { return settingData.playTimeActive; } set { settingData.playTimeActive = value; } } // int to bool
    /// <summary>
    /// 플레이타임 표기 중인 오브젝트
    /// </summary>
    public GameObject Playtime;
    /// <summary>
    /// 플레이 타임 설정 버튼 이미지
    /// </summary>
	public Image Playtime_button;
    /// <summary>
    /// 플레이 타임 설정 버튼 텍스트
    /// </summary>
    public Text playTime_text;

    /// <summary>
    /// 패치노트 알림 설정 버튼 텍스트
    /// </summary>
    public Text updateAlarm_text;
    /// <summary>
    /// 패치노트 알림 설정 버튼 이미지
    /// </summary>
    public Image updateAlarm_optionbutton;
    /// <summary>
    /// 패치노트 알림 필요 여부
    /// </summary>
    public bool UpdateAlarm { get { return settingData.updateAlarm; } set { settingData.updateAlarm = value; } } // int to bool

    /// <summary>
    /// 미니게임 활성화 여부
    /// </summary>
    public bool MiniGameActive { get { return settingData.peaceEventGameActive; } set { settingData.peaceEventGameActive = value; } }
    /// <summary>
    /// 미니게임 설정 버튼 이미지
    /// </summary>
    public Image Peace_button;
    /// <summary>
    /// 미니게임 설정 버튼 텍스트
    /// </summary>
    public Text peace_text;

    /// <summary>
    /// 수익 획득 효과 활성화 여부
    /// </summary>
    public bool AddedMoneyEffect  { get { return settingData.addedMoneyEffect; } set { settingData.addedMoneyEffect = value; } }
    /// <summary>
    /// 수익 획득 효과 설정 버튼 이미지
    /// </summary>
    public Image addedMoneyEImg;
    /// <summary>
    /// 수익 획득 효과 설정 버튼 텍스트
    /// </summary>
    public Text addedMoneyEText;

    /// <summary>
    /// 간편 연속 구매 빠름 시간 간격
    /// </summary>
    public float epFastInterval = 0.15f;
    /// <summary>
    /// 간편 연속 구매 보통 시간 간격
    /// </summary>
    public float epNormalInterval = 0.25f;
    /// <summary>
    /// 간편 연속 구매 시스템 관리 인스턴스
    /// </summary>
    public ContinuousPurchase continuousPurchase;
    /// <summary>
    /// 간편 연속 구매 설정 버튼 이미지 배열
    /// </summary>
    public Image[] easyPurchaseButtons;
    /// <summary>
    /// 설정된 간편 연속 구매 종류
    /// </summary>
    private int EasyPurchaseType { get { return settingData.easyPurchaseType; } set { settingData.easyPurchaseType = value; } }
    /// <summary>
    /// 클라우드 백업 권장 안내 메시지 활성화 여부
    /// </summary>
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
    /// <summary>
    /// 클라우드 백업 권장 안내 설정 버튼 이미지
    /// </summary>
    public Image backupRecommendImg;
    /// <summary>
    /// 클라우드 백업 권장 안내 설정 버튼 텍스트
    /// </summary>
    public Text backupRecommendText;

    void Start () {
        // 최대 프레임 설정
        Application.targetFrameRate = 60;

        // 설정 데이터 적용
        soundSlider.value = SoundVolume;
        VolumeSet();
        SetPlayTime(PlaytimeActive);
        SetPatchAlarm(UpdateAlarm);
        SetMiniGame(MiniGameActive);
        SetEasyPurchaseSpeed(EasyPurchaseType);
        SetAddedMoneyEffect(AddedMoneyEffect);
        SetBackupRecommend(BackupRecommend);
    }
    /// <summary>
    /// 설정 버튼 클릭 리스너
    /// </summary>
	public void PressKey(int nKey)
	{
        switch (nKey)
        {
            case 1: // back
                Main_Menu.SetActive(true);
                Option_Menu.SetActive(false);
                DataManager.instance.SaveAll();
                break;
            case 2: // reset
                Reset_Menu.SetActive(true);
                break;
            case 3: // 테스트 메뉴 활성화
                Key_Menu.SetActive(true);
                if(Application.platform.Equals(RuntimePlatform.Android))
                {
                    PW_Menu.SetActive(true);
                }
                break;
            case 5: //playtime
                SetPlayTime(!PlaytimeActive);
                break;
            case 7: //updatealarm
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

    /// <summary>
    /// 플레이 타임 활성화 여부 설정
    /// </summary>
    /// <param name="active">활성화 여부</param>
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
    /// <summary>
    /// 패치노트 알림 활성화 여부 설정
    /// </summary>
    /// <param name="active">활성화 여부</param>
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

    /// <summary>
    /// 미니게임 활성화 여부 설정
    /// </summary>
    /// <param name="active">활성화 여부</param>
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

    /// <summary>
    /// 테스트 메뉴 비활성화
    /// </summary>
	public void PressKey_Key(int nKey)
	{
        switch (nKey)
        {
            case 1://back
                Key_Menu.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// 수익 효과 활성화 여부 설정
    /// </summary>
    /// <param name="onOff">활성화 여부</param>
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

    /// <summary>
    /// 백업 권유 활성화 여부 설정
    /// </summary>
    /// <param name="active">활성화 여부</param>
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

    /// <summary>
    /// 간편 구매 속도 설정
    /// </summary>
    /// <param name="type">설정값 인덱스</param>
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

    /// <summary>
    /// 효과음 볼륨 조절
    /// </summary>
    public void VolumeSet()
    {
        SoundVolume = soundSlider.value;
        for (int i = 0; i < effectSounds.Length; i++)
            effectSounds[i].volume = SoundVolume;
    }
}
