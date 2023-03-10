using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 터치형 수익 정산 관리 클래스
/// </summary>
public class TouchEarning : MonoBehaviour {

    /// <summary>
    /// 터치형 수익 정보 텍스트
    /// </summary>
    public Text touchpersecond_text;

    public MessageManager messageManager;
    public SettingManager button_Option;
    public LevelManager levelManager;
    public MacroDetector macroDetector;
    public ItemManager itemManager;
    public FeverManager feverManager;
    public AchievementManager achievementManager;
    
    /// <summary>
    /// 터치 대상 오브젝트
    /// </summary>
    public GameObject TouchMoney_Menu;
	
    /// <summary>
    /// 승객 수 랜덤 계수
    /// </summary>
	public static ulong passengerRandomFactor;
    /// <summary>
    /// 승객 수 랜덤 계수 (아이템 계수 포함)
    /// </summary>
    public static ulong PassengerRandomFactor { get { return passengerRandomFactor * ExternalCoefficient; } }
    /// <summary>
    /// 외부 계수 (아이템 효과)
    /// </summary>
    public static ulong externalCoefficient = 0;
    /// <summary>
    /// 외부 계수 (아이템 효과)
    /// </summary>
    private static ulong ExternalCoefficient {
        get
        {
            if (externalCoefficient < 1)
                return 1;
            else
                return externalCoefficient;
        }
    }
	
	/// <summary>
    /// 승객 수 변화 시간
    /// </summary>
	public static int randomSetTime = 0;

    /// <summary>
    /// 초당 터치 횟수
    /// </summary>
    public int touchPerSecond;

    /// <summary>
    /// 터치 효과음
    /// </summary>
	public AudioSource touchAudio;
    /// <summary>
    /// 터치 효과음 클립
    /// </summary>
	public AudioClip audioclip;

    /// <summary>
    /// 수익 발생 애니메이션 효과
    /// </summary>
    public Animation[] addedMoneyEffectAni;
    /// <summary>
    /// 발생한 수익 정보 텍스트
    /// </summary>
    public Text[] addedMoneyTexts;
    /// <summary>
    /// 가장 최근에 표시된 텍스트 인덱스
    /// </summary>
    private int currentIndex = 0;
    /// <summary>
    /// 승객 수가 변경된 횟수
    /// </summary>
    private int numOfChanged = 0;

    void Start()
    {
        StartCoroutine(Timer(0));
    }
    /// <summary>
    /// 승객 수 랜덤 변경 타이머 코루틴
    /// </summary>
    /// <param name="delay">타이머 업데이트 간격</param>
    IEnumerator Timer(float delay = 1)
    {
        yield return new WaitForSeconds(delay);

        // 매크로 감지
        if(!itemManager.itemActived)
            macroDetector.DetectTouchAmount(touchPerSecond);

        // 승객 수 변경 대기 시간 차감
        randomSetTime--;
        // 초당 터치 횟수 초기화
        touchPerSecond = 0;

        // 기준 승객 수 70~130% 범위 내에서 랜덤 적용
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

    /// <summary>
    /// 터치 수익 지급 처리
    /// </summary>
	public void TouchIncome()
	{
        touchAudio.PlayOneShot(audioclip);  // 효과음 출력
        touchPerSecond++;                   // 초당 터치 횟수 업데이트
        achievementManager.TouchCount++;    // 업적: 터치 횟수 업데이트
        // 수익 지급
        AssetMoneyCalculator.instance.ArithmeticOperation(TouchMoneyManager.TouchMoneyLow, TouchMoneyManager.TouchMoneyHigh, true);
        levelManager.AddExp();              // 경험치 지급
        if (!feverManager.feverActived)     // 피버 모드가 아니면 피버 스택 추가
            feverManager.AddFeverStack();
        if (button_Option.AddedMoneyEffect) // 소득 효과 출력
            AddedMoneyEffect();

        if (touchPerSecond != 0)
            touchpersecond_text.text = "초당 " + touchPerSecond + "회 터치";

        if (!itemManager.itemActived && !feverManager.feverActived)
            macroDetector.DetectInterval();
    }

    /// <summary>
    /// 소득 효과 추가
    /// </summary>
    private void AddedMoneyEffect()
    {
        string m2 = "", m1 = "";
        // 터치형 수익이 1경 이상일 때
        if (TouchMoneyManager.TouchMoneyHigh > 0)
        {
            PlayManager.ArrangeUnit(TouchMoneyManager.TouchMoneyLow, TouchMoneyManager.TouchMoneyHigh, ref m1, ref m2);
            addedMoneyTexts[currentIndex].text = "+" + m2 + m1 + "$";
        }
        // 터치형 수익이 1경 미만일 때
        else
        {
            if (TouchMoneyManager.TouchMoneyLow >= 1000000000000)
                PlayManager.ArrangeUnit(TouchMoneyManager.TouchMoneyLow, 0, ref m1, ref m2, true);
            else
                m1 = string.Format("{0:#,##0}", TouchMoneyManager.TouchMoneyLow);
            addedMoneyTexts[currentIndex].text = "+" + m1 + "$";
        }
        // 애니메이션 효과 재생
        addedMoneyEffectAni[currentIndex].Play();

        // 인덱스 업데이트
        currentIndex++;
        if (currentIndex >= addedMoneyEffectAni.Length)
            currentIndex = 0;
    }
}
