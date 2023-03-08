using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 치안 시스템 매니저 클래스
/// </summary>
public class PeaceManager : MonoBehaviour {

    /// <summary>
    /// 범죄 해결 미니 게임 게이지 슬라이더
    /// </summary>
    public Slider GageSlider;

    /// <summary>
    /// 범죄 해결 메뉴 오브젝트
    /// </summary>
	public GameObject Crime_Menu;
    /// <summary>
    /// 사건 접수 이후 메뉴 오브젝트
    /// </summary>
	public GameObject AfterAccept_Menu;

    /// <summary>
    /// 범죄 해결 확률 텍스트 정보
    /// </summary>
	public Text Percentage_text;

    public MessageManager messageManager;

    /// <summary>
    /// 치안 단계별 미니게임 성공 확률
    /// </summary>
    public int[] successPossibility;
    /// <summary>
    /// 치안 단계 기준치
    /// </summary>
    public int[] peaceCriterias;
    /// <summary>
    /// 치안 단계별 타이틀 
    /// </summary>
    public string[] peaceStageTitles;

    /// <summary>
    /// 범죄해결 미니게임 게이지 값
    /// </summary>
	public int gage = 50;
    /// <summary>
    /// 치안 단계별 범죄해결 미니게임 터치당 증가치
    /// </summary>
    public int[] gageIncrements;

    /// <summary>
    /// 치안 관리 상품 별 가격 (1경 미만 단위)
    /// </summary>
    public ulong[] peaceLowPrices;
    /// <summary>
    /// 치안 관리 상품 별 가격 (1경 이상 단위)
    /// </summary>
    public ulong[] peaceHighPrices;
    /// <summary>
    /// 치안 상품별 가격 배율
    /// </summary>
    public ulong[] peacePriceMultiples;
    /// <summary>
    /// 치안 관리 상품 별 치안 상태 향상 수치
    /// </summary>
    public int[] peaceValues;

    /// <summary>
    /// 보상과 연결되는 예상 클리어 타임
    /// </summary>
    public int clearTime;

    /// <summary>
    /// 치안 상태 수치
    /// </summary>
    public int PeaceValue { get { return company_Reputation_Controller.companyData.peaceValue; } set { company_Reputation_Controller.companyData.peaceValue = value; } }
    /// <summary>
    /// 치안 단계
    /// </summary>
    public int peaceStage;
    /// <summary>
    /// 범죄예방 캠페인 개최 상품 구매 대기 쿨타임
    /// </summary>
    public int CoolTime { get { return company_Reputation_Controller.companyData.peaceCoolTime; } set { company_Reputation_Controller.companyData.peaceCoolTime = value; } }
    /// <summary>
    /// 범죄예방 캠페인 개최 상품 쿨타임 여부
    /// </summary>
    public static bool isCoolTime = false;
    /// <summary>
    /// 범죄예방 캠페인 개최 상품 구매 버튼
    /// </summary>
	public Button Campaign_button;
    /// <summary>
    /// 범죄예방 캠페인 개최 상품 쿨타임 남은 시간
    /// </summary>
	public Text CoolTime_text;

    /// <summary>
    /// 범죄 해결 성공 메뉴 오브젝트
    /// </summary>
	public GameObject Success_Menu;
    /// <summary>
    /// 범죄 해결 보상 정보 텍스트
    /// </summary>
	public Text bonus_Message;
    /// <summary>
    /// 범죄 해결 실패 메뉴 오브젝트
    /// </summary>
	public GameObject Fail_Menu;
    /// <summary>
    /// 범죄 해결 실패 패널티 정보 텍스트
    /// </summary>
	public Text penalty_Message;

    /// <summary>
    /// 치안 관리 상품 가격 정보 텍스트
    /// </summary>
    public Text[] priceTexts;
    /// <summary>
    /// 치안 관리 상품 치안 상태 향상 수치 정보 텍스트
    /// </summary>
    public Text[] peaceValueTexts;

    /// <summary>
    /// 현재 치안 수치 정보 텍스트
    /// </summary>
    public Text Peace_text;
    /// <summary>
    /// 현재 치안 단계 정보 텍스트
    /// </summary>
    public Text peaceConditonText;

    /// <summary>
    /// 범죄 해결 성공 효과음
    /// </summary>
    public AudioSource successAudio;
    /// <summary>
    /// 버모지 해결 실패 효과아ㅡㅁ
    /// </summary>
    public AudioSource failedAudio;

    public ButtonColor_Controller3 buttonColor_Controller;
    public CompanyReputationManager company_Reputation_Controller;

    /// <summary>
    /// 치안 관리 상품 구매 메뉴 오브젝트
    /// </summary>
    public GameObject ManagePeace_Menu;

    public UpdateDisplay stationConditionUpdateDisplay;

    void Start()
    {
        SetPeaceInfoText();
        SetText();
        if (CoolTime < 5)
        {
            isCoolTime = true;
            StartCoroutine(CampaignTimer());
        }
        StartCoroutine(ValueDropTimer(SetTime()));
        stationConditionUpdateDisplay.onEnableUpdate += SetPeaceInfoText;
    }

    #region Peace MiniGame
    /// <summary>
    /// 범죄해결 미니게임 초기화
    /// </summary>
    public void InitPeaceMiniGame()
    {
        Fail_Menu.SetActive(false);
        Crime_Menu.SetActive(true);
        Percentage_text.text = successPossibility[peaceStage] + "%";
        gage = 50;
        GageSlider.value = gage;
    }

    /// <summary>
    /// 범죄 해결 게이지 감소 코루틴
    /// </summary>
    IEnumerator GageDecrement()
    {
        yield return new WaitForSeconds(0.3f);

        if (gage > 0)
        {
            gage -= gageIncrements[peaceStage] / 2;
            GageSlider.value = gage;
            StartCoroutine(GageDecrement());
        }
        else
        {
            messageManager.ShowMessage("범죄 해결에 실패하였습니다.", 2f);
            AfterAccept_Menu.SetActive(false);
            OnFail();
            gage = 99999;
        }
    }

    /// <summary>
    /// 범죄 해결 결과 메뉴 비활성화 버튼 클릭 이벤트 리스너
    /// </summary>
    public void PressKey_Close(int nKey)
    {
        switch (nKey)
        {
            case 0://success
                Success_Menu.SetActive(false);
                Crime_Menu.SetActive(false);
                break;
            case 1://fail
                Fail_Menu.SetActive(false);
                Crime_Menu.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// 범죄 해결 접수/거절 버튼 클릭 이벤트 리스너
    /// </summary>
    public void PressKey_Choice(int nKey)
    {
        switch (nKey)
        {
            case 0://decline
                Crime_Menu.SetActive(false);
                messageManager.ShowMessage("범죄 해결을 거절하였습니다.(패널티x)", 2f);
                break;
            case 1://accept
                AfterAccept_Menu.SetActive(true);
                StartCoroutine(GageDecrement());
                break;
        }
    }

    /// <summary>
    /// 범죄자 상대하기
    /// </summary>
    public void Fighting()
    {
        if (gage < 360)
        {
            gage += gageIncrements[peaceStage];
            GageSlider.value = gage;
        }
        else if (gage >= 360)
        {
            AfterAccept_Menu.SetActive(false);
            OnSuccess();
        }
    }

    /// <summary>
    /// 범죄 해결 성공 여부 처리
    /// </summary>
    private void OnSuccess()
    {
        // 성공 확률에 따라서 성공/실패 여부 결정
        int randomP = Random.Range(0, 100);
        if (randomP < successPossibility[peaceStage])
        {
            Success_Menu.SetActive(true);

            ulong addedMoneyLow = 0, addedMoneyHigh = 0;
            string money1 = "", money2 = "";
            // 보상 계산 및 표기
            PlayManager.SetMoneyReward(ref addedMoneyLow, ref addedMoneyHigh, clearTime);
            AssetMoneyCalculator.instance.ArithmeticOperation(addedMoneyLow, addedMoneyHigh, true);
            PlayManager.ArrangeUnit(addedMoneyLow, addedMoneyHigh, ref money1, ref money2);
            successAudio.Play();

            bonus_Message.text = "범죄 해결을 하여 장려금 <color=green>" + money2 + money1 + "$</color>을 지급받았습니다.";

        }
        else
            OnFail();
    }

    /// <summary>
    /// 범죄 해결 실패처리 (패널티 없음)
    /// </summary>
    private void OnFail()
    {
        penalty_Message.text = "범죄 해결에 실패하였습니다.";
        failedAudio.Play();
        Fail_Menu.SetActive(true);
    }
    #endregion
    #region Peace Item store
    /// <summary>
    /// 범죄예방 캠페인 개최 상품 쿨타임 타이머 코루틴
    /// </summary>
    IEnumerator CampaignTimer()
    {
        yield return new WaitForSeconds(1);

        // 쿨타임 감소
        CoolTime--;
        if (CoolTime < 1)
        {
            isCoolTime = false;
            CoolTime = 5;
        }
        // 쿨타임 업데이트
        CoolTime_text.text = "쿨타임: " + CoolTime + "초";

        if (isCoolTime)
            StartCoroutine(CampaignTimer());
        else
        {
            // 구매 가능 상태로 전환
            Campaign_button.enabled = true;
            Campaign_button.image.color = Color.white;
        }
    }

    /// <summary>
    /// 치안 관리 상품 정보 업데이트
    /// </summary>
    private void SetText()
    {
        string moneyLow = "", moneyHigh = "";
        for (int i = 0; i < priceTexts.Length; i++)
        {
            PlayManager.ArrangeUnit(peaceLowPrices[i], peaceHighPrices[i], ref moneyLow, ref moneyHigh);
            priceTexts[i].text = "비용: " + moneyHigh + moneyLow + "$";

            peaceValueTexts[i].text = "안전도 +" + peaceValues[i];
        }

        CoolTime_text.text = "쿨타임: " + CoolTime + "초";
        buttonColor_Controller.SetPeace();
    }
    /// <summary>
    /// 치안 관리 상품 가격 계산
    /// </summary>
    public void SetPrice()
    {
        ulong lowRevenue = 0, highRevenue = 0;
        MyAsset.instance.GetTotalRevenue(ref lowRevenue, ref highRevenue);
        for (int i = 0; i < peaceLowPrices.Length; i++)
        {
            peaceLowPrices[i] = lowRevenue * (ulong)PlayManager.instance.averageTouch * peacePriceMultiples[i];
            peaceHighPrices[i] = highRevenue * (ulong)PlayManager.instance.averageTouch * peacePriceMultiples[i];
            MoneyUnitTranslator.Arrange(ref peaceLowPrices[i], ref peaceHighPrices[i]);
        }
    }
    /// <summary>
    /// 선택한 치안 관리 상품 구매 처리
    /// </summary>
    /// <param name="index">상품 인덱스</param>
    public void PressKey_Manage(int index)
    {
        // 치안 수치 최대치 검사
        if (PeaceValue < 100)
        {
            // 비용 지불 처리
            if (AssetMoneyCalculator.instance.ArithmeticOperation(peaceLowPrices[index], peaceHighPrices[index], false))
            {
                // 치안 수치 상승 처리
                PeaceValue += peaceValues[index];
                if (PeaceValue > 100)
                {
                    PeaceValue = 100;
                }

                // 범죄예방 캠페인 개최 상품 쿨타임 적용
                if (index.Equals(3))
                {
                    isCoolTime = true;
                    Campaign_button.enabled = false;
                    Campaign_button.image.color = Color.gray;
                    StartCoroutine(CampaignTimer());
                }
                // UI 업데이트
                SetPeaceInfoText();
                SetText();
            }
            else
            {
                PlayManager.instance.LackOfMoney();
            }
        }
        else 
        {
            FullOfPeace();
        }
    }

    /// <summary>
    /// 치안 수치 최대치 알림
    /// </summary>
    private void FullOfPeace()
    {
        messageManager.ShowMessage("안전도가 최대치입니다.");
    }
    #endregion
    #region PeaceValue Dropping
    /// <summary>
    /// 치안 수치 차감 코루틴
    /// </summary>
    /// <param name="waittime">차감 대기 시간</param>
    IEnumerator ValueDropTimer(float waittime)
    {
        yield return new WaitForSeconds(waittime);

        // 치안 단계에 따라 치안 수치 차감
        PeaceValue -= GetDropAmount();
        if (PeaceValue < -100)
        {
            PeaceValue = -100;
        }
        SetPeaceInfoText();
        StartCoroutine(ValueDropTimer(SetTime()));
    }
    /// <summary>
    /// 치안 수치 차감 대기 시간 계산
    /// </summary>
    /// <returns>차감 대기 시간</returns>
    private int SetTime()
    {
        int Timeset;
        if (!LineManager.instance.lineCollections[4].IsExpanded())
        {
            Timeset = Random.Range(50, 71);
        }
        else if (!LineManager.instance.lineCollections[9].IsExpanded())
        {
            Timeset = Random.Range(45, 61);
        }
        else
        {
            Timeset = Random.Range(40, 51);
        }
        return Timeset;
    }
    /// <summary>
    /// 치안 수치 차감량 단계별 계산
    /// </summary>
    /// <returns></returns>
    private int GetDropAmount()
    {
        if (!LineManager.instance.lineCollections[4].IsExpanded())
            return Random.Range(0, 5);
        else if (!LineManager.instance.lineCollections[9].IsExpanded())
            return Random.Range(2, 7);
        else
            return Random.Range(4, 9);
    }
    #endregion
    #region Information/State updater
    /// <summary>
    /// 치안 관리 메뉴 활성화 시, 업데이트 함수
    /// </summary>
    public void OnOpenManagePeace()
    {
        SetText();
        SetPeaceInfoText();
    }

    /// <summary>
    /// 치안 수치 UI 업데이트
    /// </summary>
    public void SetPeaceInfoText()
    {
        Peace_text.text = "안전도: " + PeaceValue + "점";
        CheckPeaceStage();
    }

    /// <summary>
    /// 치안 단계 업데이트
    /// </summary>
    public void CheckPeaceStage()
    {
        for (int i = 0; i < peaceCriterias.Length; i++)
        {
            if (PeaceValue <= peaceCriterias[i])
            {
                peaceStage = i;
                break;
            }
        }
        peaceConditonText.text = "상태: " + peaceStageTitles[peaceStage];

    }
    #endregion
}
