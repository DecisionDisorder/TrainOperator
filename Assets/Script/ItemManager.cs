using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 아이템 시스템 관리 클래스
/// </summary>
public class ItemManager : MonoBehaviour
{
    /// <summary>
    /// 아이템 데이터 오브젝트
    /// </summary>
    public ItemData itemData;

    /// <summary>
    /// 컬러 카드의 종류 별 스펙 데이터 배열
    /// </summary>
    public CardAbility[] colorCardAbilities;
    /// <summary>
    /// 레어 카드의 종류 별 스펙 데이터 배열
    /// </summary>
    public CardAbility[] rareCardAbilities;

    /// <summary>
    /// 컬러 카드 사용 버튼 배열
    /// </summary>
    public Button[] colorCardButtons;
    /// <summary>
    /// 레어 카드 사용 버튼 배열
    /// </summary>
    public Button[] rareCardButtons;
    /// <summary>
    /// 컬러 카드 종류 마다의 남은 수량 텍스트 배열
    /// </summary>
    public Text[] colorCardAmountTexts;
    /// <summary>
    /// 레어 카드 종류 마다의 남은 수량 텍스트 배열
    /// </summary>
    public Text[] rareCardAmountTexts;
    /// <summary>
    /// 컬러 카드팩 남은 수량 텍스트 배열
    /// </summary>
    public Text[] colorPackAmountText;
    /// <summary>
    /// 레어 카드팩 남은 수량 텍스트 배열
    /// </summary>
    public Text[] rarePackAmountText;

    /// <summary>
    /// 카드 포인트 정보 텍스트
    /// </summary>
    public Text cardPointText;
    /// <summary>
    /// 피버 리필 쿠폰 수량 텍스트
    /// </summary>
    public Text feverRefillText;

    /// <summary>
    /// 카드 타이머 배경 이미지
    /// </summary>
    public Image cardTimer;
    /// <summary>
    /// 카드 타이머 fill 이미지
    /// </summary>
    public Image cardTimerImg;

    /// <summary>
    /// 카드팩 선택 메뉴 오브젝트
    /// </summary>
    public GameObject cardPackMenu;
    /// <summary>
    /// 카드팩 구매 메뉴 오브젝트
    /// </summary>
    public GameObject purchaseMenu;
    /// <summary>
    /// 카드팩 구매 메뉴에 표시할 카드팩 오브젝트 배열
    /// </summary>
    public GameObject[] purchaseTypeObjs;
    /// <summary>
    /// 카드팩 개봉 메뉴 오브젝트 배열
    /// </summary>
    public GameObject[] openCardPackMenus;
    /// <summary>
    /// 카드팩 도움말 메뉴 오브젝트
    /// </summary>
    public GameObject cardHelpMenu;

    /// <summary>
    /// 컬러 카드팩 개봉 전/후 이미지 배열
    /// </summary>
    public GameObject[] colorCardPackOpenImgs;
    /// <summary>
    /// 레어 카드팩 개봉 전/후 이미지 배열
    /// </summary>
    public GameObject[] rareCardPackOpenImgs;
    /// <summary>
    /// 컬러 카드팩 개봉 애니메이션 효과 배열
    /// </summary>
    public Animation[] colorPackOpenAni;
    /// <summary>
    /// 레어 카드팩 개봉 애니메이션 효과 배열
    /// </summary>
    public Animation[] rarePackOpenAni;
    /// <summary>
    /// 컬러 카드팩 개봉 버튼 배열
    /// </summary>
    public Button[] colorOpenButtons;
    /// <summary>
    /// 레어 카드팩 개봉 버튼 배열
    /// </summary>
    public Button[] rareOpenButtons;
    /// <summary>
    /// 컬러 카드팩 개봉 결과 카드 이미지
    /// </summary>
    public Image colorResultCardImg;
    /// <summary>
    /// 컬러 카드팩 개봉 결과 설명 텍스트
    /// </summary>
    public Text colorResultDescText;
    /// <summary>
    /// 레어 카드팩 개봉 결과 카드 이미지
    /// </summary>
    public Image rareResultCardImg;
    /// <summary>
    /// 레어 카드팩 개봉 결과 설명 텍스트
    /// </summary>
    public Text rareResultDescText;
    
    /// <summary>
    /// 카드팩 개봉 오디오 효과음
    /// </summary>
    public AudioSource cardPackAudio;
    /// <summary>
    /// 카드팩 개봉 효과음 클립
    /// </summary>
    public AudioClip[] cardAudioClips;

    private delegate void AfterAnimation(int card);
    public TimeMoneyManager timeMoneyManager;

    /// <summary>
    /// 컬러 카드 종류별 남은 수량
    /// </summary>
    public int[] ColorCardAmounts
    {
        get
        {
            ReallocateColorCardAmount();
            return itemData.cardAmounts;
        }
        set { itemData.cardAmounts = value; UpdateColorCardInfo(); }
    }
    /// <summary>
    /// 레어 카드 종류별 남은 수량
    /// </summary>
    public int[] RareCardAmounts
    {
        get
        {
            if (itemData.rareCardAmounts == null) itemData.rareCardAmounts = new int[6];
            return itemData.rareCardAmounts;
        }
        set { itemData.rareCardAmounts = value; UpdateRareCardInfo(); }
    }
    /// <summary>
    /// 컬러 카드팩 남은 수량
    /// </summary>
    public int ColorPackAmount { get { return itemData.silverCardAmount; } set { itemData.silverCardAmount = value; SetPackAmountText(); } }
    /// <summary>
    /// 레어 카드팩 남은 수량
    /// </summary>
    public int RarePackAmount { get { return itemData.rareCardPackAmount; } set { itemData.rareCardPackAmount = value; SetPackAmountText(); } }

    /// <summary>
    /// 카드 포인트 수치
    /// </summary>
    public int CardPoint { get { return itemData.cardPoint; } set { itemData.cardPoint = value; SetCardPointText(); } }
    
    /// <summary>
    /// 피버 리필 쿠폰 남은 수량 
    /// </summary>
    public int FeverRefillAmount { get { return itemData.feverRefillAmount; } set { if (value >= 0) itemData.feverRefillAmount = value; UpdateFeverRefillText(); } }

    /// <summary>
    /// 아이템 효과로 적용 중인 터치형 수익 증가율
    /// </summary>
    private ulong activedTouchAbility;
    /// <summary>
    /// 아이템 효과로 적용 중인 시간형 수익 증가율
    /// </summary>
    public ulong activedTimeAbility;

    /// <summary>
    /// 카드팩의 1회 구매 수량 배열 (1/5/10)
    /// </summary>
    public int[] purchaseAmounts;
    /// <summary>
    /// 1회 구매 수량 상품 별 컬러 카드팩 구매 가격
    /// </summary>
    public int[] colorPackPrices;
    /// <summary>
    /// 1회 구매 수량 상품 별 레어 카드팩 구매 가격
    /// </summary>
    public int[] rarePackPrices;
    /// <summary>
    /// 구매하는 카드팩 종류 인덱스
    /// </summary>
    private int purchasePackType;
    /// <summary>
    /// 구매하는 1회 구매 수량 인덱스
    /// </summary>
    private int purchaseAmountType;

    /// <summary>
    /// 구매 배경 색상
    /// </summary>
    public Color purchaseBackgroundColor;
    /// <summary>
    /// 피버 배경 색상
    /// </summary>
    public Color feverBackgroundColor;

    /// <summary>
    /// 아이템 효과가 적용 중인지 여부
    /// </summary>
    public bool itemActived;

    /// <summary>
    /// 구매 효과음
    /// </summary>
    public AudioSource purchaseAudio;

    public AssetInfoUpdater assetInfoUpdater;
    public TouchEarning touchEarning;
    public MessageManager messageManager;
    public MiniGameManager miniGameManager;
    public FeverManager feverManager;

    private void Start()
    {
        UpdateColorCardInfo();
        UpdateRareCardInfo();
        SetCardPointText();
        SetPackAmountText();
        UpdateFeverRefillText();
    }
    
    #region 메뉴 관리
    public void SetMenuOn(int menu)
    {
        switch(menu)
        {
            case 0://카드팩 메뉴
                cardPackMenu.SetActive(true);
                break;
            case 1://컬러 카드팩 구매
                purchaseMenu.SetActive(true);
                purchaseTypeObjs[0].SetActive(true);
                break;
            case 2://레어 카드팩 구매
                purchaseMenu.SetActive(true);
                purchaseTypeObjs[1].SetActive(true);
                break;
            case 3://컬러 카드팩 오픈
                openCardPackMenus[0].SetActive(true);
                break;
            case 4://레어 카드팩 오픈
                openCardPackMenus[1].SetActive(true);
                break;
            case 5://카드 도움말
                cardHelpMenu.SetActive(true);
                break;
        }
    }
    public void SetMenuOff(int menu)
    {
        switch (menu)
        {
            case 0://카드팩 메뉴
                cardPackMenu.SetActive(false);
                break;
            case 1://컬러 카드팩 구매
                purchaseMenu.SetActive(false);
                purchaseTypeObjs[0].SetActive(false);
                purchaseTypeObjs[1].SetActive(false);
                break;
            case 3://컬러 카드팩 오픈
                openCardPackMenus[0].SetActive(false);
                colorResultCardImg.gameObject.SetActive(false);
                break;
            case 4://레어 카드팩 오픈
                openCardPackMenus[1].SetActive(false);
                rareResultCardImg.gameObject.SetActive(false);
                break;
            case 5://카드 도움말
                cardHelpMenu.SetActive(false);
                break;
        }
    }
    #endregion
    #region 아이템 사용
    /// <summary>
    /// 컬러 카드팩 수량 데이터 재할당
    /// </summary>
    private void ReallocateColorCardAmount()
    {
        if (itemData.cardAmounts.Length.Equals(4))
        {
            int[] amounts;
            amounts = itemData.cardAmounts;
            itemData.cardAmounts = new int[5];
            for (int i = 0; i < amounts.Length; i++)
                itemData.cardAmounts[i] = amounts[i];
        }
    }

    /// <summary>
    /// 카드 포인트 정보 업데이트
    /// </summary>
    private void SetCardPointText()
    {
        cardPointText.text = string.Format("{0:#,##0}P", CardPoint);
    }

    /// <summary>
    /// 컬러 카드 아이템 사용 처리
    /// </summary>
    /// <param name="card">카드 종류 인덱스</param>
    public void UseColorCard(int card)
    {
        // 남은 카드 수량 확인
        if (ColorCardAmounts[card] > 0)
        {
            // 카드 소모 처리 및 UI 업데이트
            ColorCardAmounts[card]--;
            UpdateColorCardInfo();
            // 아이템 활성화 처리 및 타이머 시각적 효과 설정
            itemActived = true;
            cardTimer.gameObject.SetActive(true);

            int duration = colorCardAbilities[card].duration;
            cardTimerImg.sprite = colorCardAbilities[card].cardSprite;
            cardTimer.sprite = colorCardAbilities[card].cardSprite;

            // 터치형 혹은 시간형 수익 지급량 증대 효과 적용
            SetMassiveIncomeEnable(duration, (ulong)colorCardAbilities[card].ability[0], (ulong)colorCardAbilities[card].ability[1], ref activedTouchAbility, ref activedTimeAbility);

            // 활성화 도중, 다른 아이템 사용 불가 처리
            ActiveCards(false);
            StartCoroutine(ColorCardTimer(duration, duration));
        }
        else
            messageManager.ShowMessage(colorCardAbilities[card].name + "를 보유하고 있지 않습니다.");
    }

    /// <summary>
    /// 터치형/시간형 수익 지급량 증대 효과 적용
    /// </summary>
    /// <param name="duration">지속시간</param>
    /// <param name="touchMoneyAbility">터치형 수익 증가율</param>
    /// <param name="timeMoneyAbility">시간형 수익 증가율</param>
    /// <param name="activedTouchAbility">아이템 효과로 적용 될 터치형 수익 증가율</param>
    /// <param name="activedTimeAbility">아이템 효과로 적용 될 시간형 수익 증가율</param>
    public void SetMassiveIncomeEnable(int duration, ulong touchMoneyAbility, ulong timeMoneyAbility, ref ulong activedTouchAbility, ref ulong activedTimeAbility)
    {
        TouchEarning.externalCoefficient += touchMoneyAbility;
        activedTouchAbility = touchMoneyAbility;

        activedTimeAbility = timeMoneyAbility;
        timeMoneyManager.externalCoefficient += timeMoneyAbility;

        // 랜덤 승객 수 재지정 및 미니게임 대기시간 딜레이
        TouchEarning.randomSetTime += duration; 
        miniGameManager.timeLeft += duration;

        CompanyReputationManager.instance.RenewPassengerRandom();
        assetInfoUpdater.UpdateTimeMoneyText();
    }

    /// <summary>
    /// 터치형/시간형 수익 지급량 복원
    /// </summary>
    /// <param name="activedTouchAbility">아이템 효과로 적용되었던 터치형 수익 증가율</param>
    /// <param name="activedTimeAbility">아이템 효과로 적용되었던 시간형 수익 증가율</param>
    public void SetMassiveIncomeDisable(ulong activedTouchAbility, ulong activedTimeAbility)
    {
        TouchEarning.externalCoefficient -= activedTouchAbility;
        timeMoneyManager.externalCoefficient -= activedTimeAbility;
        assetInfoUpdater.UpdateTimeMoneyText();
        CompanyReputationManager.instance.RenewPassengerRandom();
    }

    /// <summary>
    /// 컬러 카드 적용 효과 비활성화 처리
    /// </summary>
    private void DisableColorCardEffect()
    {
        DisableCardEffect();
        SetMassiveIncomeDisable(activedTouchAbility, activedTimeAbility);
    }

    /// <summary>
    /// 컬러 카드 지속 시간 타이머
    /// </summary>
    /// <param name="remainTime">남은 지속 시간</param>
    /// <param name="fullTime">총 지속 시간</param>
    /// <param name="interval">업데이트 간격 시간(초)</param>
    /// <returns></returns>
    IEnumerator ColorCardTimer(float remainTime, float fullTime, float interval = 0.1f)
    {
        yield return new WaitForSeconds(interval);

        remainTime -= interval;
        cardTimerImg.fillAmount = (fullTime - remainTime) / fullTime;

        if (remainTime > 0)
            StartCoroutine(ColorCardTimer(remainTime, fullTime, interval));
        else
            DisableColorCardEffect();
    }

    /// <summary>
    /// 레어 카드 사용 처리
    /// </summary>
    /// <param name="card">카드 종류 인덱스</param>
    public void UseRareCard(int card)
    {
        // 카드 남은 수량 확인
        if(RareCardAmounts[card] > 0)
        {
            // 카드 소모 처리 및 UI 업데이트
            RareCardAmounts[card]--;
            UpdateRareCardInfo();

            // 아이템 활성화 처리 및 타이머 시각적 효과 설정
            itemActived = true;
            cardTimer.gameObject.SetActive(true);

            int duration = rareCardAbilities[card].duration;
            cardTimerImg.sprite = rareCardAbilities[card].cardSprite;
            cardTimer.sprite = rareCardAbilities[card].cardSprite;

            // 랜덤 승객 수 재지정 및 미니게임 대기시간 딜레이
            TouchEarning.randomSetTime += duration; 
            miniGameManager.timeLeft += duration;

            // 활성화 도중, 다른 아이템 사용 불가 처리
            ActiveCards(false);
            // 자동 터치 효과 시작
            StartCoroutine(AutoClicker(rareCardAbilities[card].ability[0], duration, duration));
        }
        else
            messageManager.ShowMessage(rareCardAbilities[card].name + "를 보유하고 있지 않습니다.");
    }

    /// <summary>
    /// 카드 아이템 사용 효과 비활성화 처리
    /// </summary>
    private void DisableCardEffect()
    {
        itemActived = false;
        ActiveCards(true);
        cardTimer.gameObject.SetActive(false);
    }

    /// <summary>
    /// 레어 카드 효과: 자동 터치 반복 코루틴
    /// </summary>
    /// <param name="touchPerSecond">초당 자동 터치 횟수</param>
    /// <param name="remainTime">남은 시간</param>
    /// <param name="fullTime">총 지속 시간</param>
    IEnumerator AutoClicker(int touchPerSecond, float remainTime, int fullTime)
    {
        float interval = 1f / touchPerSecond;
        yield return new WaitForSeconds(interval);
        
        // 터치 처리 및 타이머 업데이트
        touchEarning.TouchIncome();
        cardTimerImg.fillAmount = (fullTime - remainTime) / fullTime;

        remainTime -= interval;
        if (remainTime > 0)
            StartCoroutine(AutoClicker(touchPerSecond, remainTime, fullTime));
        else
            DisableCardEffect();
    }

    /// <summary>
    /// 컬러 카드 수량 정보 업데이트
    /// </summary>
    private void UpdateColorCardInfo()
    {
        for (int i = 0; i < colorCardAmountTexts.Length; i++)
            colorCardAmountTexts[i].text = ColorCardAmounts[i] + "개";
    }

    /// <summary>
    /// 레어 카드 수량 정보 업데이트
    /// </summary>
    private void UpdateRareCardInfo()
    {
        for (int i = 0; i < rareCardAmountTexts.Length; i++)
            rareCardAmountTexts[i].text = RareCardAmounts[i] + "개";
    }

    /// <summary>
    /// 카드 아이템 사용 활성화/비활성화 처리
    /// </summary>
    /// <param name="active">사용 가능 여부</param>
    private void ActiveCards(bool active)
    {
        SetActiveColorCard(active);
        SetActiveRareCard(active);
    }

    /// <summary>
    /// 컬러 카드 사용 활성화/비활성화 처리
    /// </summary>
    /// <param name="active">사용 가능 여부</param>
    public void SetActiveColorCard(bool active)
    {
        for (int i = 0; i < colorCardButtons.Length; i++)
            colorCardButtons[i].interactable = active;
    }
    /// <summary>
    /// 레어 카드 사용 활성화/비활성화 처리
    /// </summary>
    /// <param name="active">사용 가능 여부</param>
    public void SetActiveRareCard(bool active)
    {
        for (int i = 0; i < rareCardButtons.Length; i++)
            rareCardButtons[i].interactable = active;
    }

    /// <summary>
    /// 피버 게이지 리필 쿠폰 사용 확인
    /// </summary>
    public void UseFeverRefill()
    {
        if (FeverRefillAmount > 0)
        {
            messageManager.OpenCommonCheckMenu("피버 충전 쿠폰 사용 확인", "피버 충전 쿠폰을 사용하여\n피버 게이지를 완전히 충전하시겠습니까?\n<size=25>(게이지만 충전되며 원하는 타이밍에\n직접 피버 타임을 시작할 수 있습니다.)</size>", feverBackgroundColor, RefillFever);
        }
        else
            messageManager.ShowMessage("피버 충전 쿠폰이 없습니다.");
    }

    /// <summary>
    /// 피버 게이지 리필 쿠폰 사용 처리
    /// </summary>
    private void RefillFever()
    {
        // 이미 피버 스택이 최대인 상태에서는 사용되지 않도록 방지
        if (feverManager.FeverStack < feverManager.targetFeverStack)
        {
            // 피버 스택 충전 후 쿠폰 소모 처리
            feverManager.FeverStack = feverManager.targetFeverStack;
            FeverRefillAmount--;
            messageManager.ShowMessage("피버 게이지를 완전히 충전했습니다!");
        }
        else
            messageManager.ShowMessage("피버 게이지가 이미 완전히 충전되어있습니다.");
    }
    /// <summary>
    /// 피버 리필 쿠폰 수량 정보 업데이트
    /// </summary>
    private void UpdateFeverRefillText()
    {
        feverRefillText.text = "피버 충전 쿠폰\n" + FeverRefillAmount + "개";
    }
    #endregion
    #region 카드팩 구매 기능
    /// <summary>
    /// 컬러 카드팩 구매 확인 메뉴 활성화
    /// </summary>
    /// <param name="type">구매할 수량 상품 인덱스</param>
    public void OpenColorPackPurchaseCheck(int type)
    {
        purchasePackType = 0;
        purchaseAmountType = type;
        string title = "카드팩 구매 확인";
        string product = "제품: 컬러 카드팩";
        string amount = "수량: " + purchaseAmounts[type] + "개";
        string price = string.Format("가격: {0:#,##0}P", colorPackPrices[type]);

        messageManager.SetPurchaseCheckMenu(title, product, amount, price, purchaseBackgroundColor, PurchasePack, Cancel);
    }
    /// <summary>
    /// 레어 카드팩 구매 확인 메뉴 활성화
    /// </summary>
    /// <param name="type">구매할 수량 상품 인덱스</param>
    public void OpenRarePackPurchaseCheck(int type)
    {
        purchasePackType = 1;
        purchaseAmountType = type;
        string title = "카드팩 구매 확인";
        string product = "제품: 레어 카드팩";
        string amount = "수량: " + purchaseAmounts[type] + "개";
        string price = string.Format("가격: {0:#,##0}P", rarePackPrices[type]);

        messageManager.SetPurchaseCheckMenu(title, product, amount, price, purchaseBackgroundColor, PurchasePack, Cancel);
    }

    /// <summary>
    /// 이벤트 콜백 등록용 구매 취소 함수
    /// </summary>
    private void Cancel() { }

    /// <summary>
    /// 카드팩 구매 처리
    /// </summary>
    public void PurchasePack()
    {
        // 컬러 카드팩 구매 처리
        if (purchasePackType.Equals(0))
        {
            // 카드 포인트 확인
            if (CardPoint >= colorPackPrices[purchaseAmountType])
            {
                // 카드 포인트 차감 및 구매 처리
                CardPoint -= colorPackPrices[purchaseAmountType];
                ColorPackAmount += purchaseAmounts[purchaseAmountType];
                messageManager.ShowMessage("컬러 카드팩 " + purchaseAmounts[purchaseAmountType] + "개를 구매하였습니다.");
                purchaseAudio.Play();
            }
            else
                messageManager.ShowMessage("카드 포인트가 부족합니다.");
        }
        // 레어 카드팩 구매 처리
        else
        {
            // 카드 포인트 확인
            if (CardPoint >= rarePackPrices[purchaseAmountType])
            {
                // 카드 포인트 차감 및 구매 처리
                CardPoint -= rarePackPrices[purchaseAmountType];
                RarePackAmount += purchaseAmounts[purchaseAmountType];
                messageManager.ShowMessage("레어 카드팩 " + purchaseAmounts[purchaseAmountType] + "개를 구매하였습니다.");
                purchaseAudio.Play();
            }
            else
                messageManager.ShowMessage("카드 포인트가 부족합니다.");
        }
    }

    /// <summary>
    /// 카드팩 수량 정보 업데이트
    /// </summary>
    private void SetPackAmountText()
    {
        colorPackAmountText[0].text = string.Format("{0:#,##0}개", ColorPackAmount);
        rarePackAmountText[0].text = string.Format("{0:#,##0}개", RarePackAmount);

        colorPackAmountText[1].text = string.Format("현재 {0:#,##0}개 보유 중", ColorPackAmount);
        rarePackAmountText[1].text = string.Format("현재 {0:#,##0}개 보유 중", RarePackAmount);

        colorPackAmountText[2].text = string.Format("현재 {0:#,##0}개 보유 중", ColorPackAmount);
        rarePackAmountText[2].text = string.Format("현재 {0:#,##0}개 보유 중", RarePackAmount);
    }
    #endregion
    #region 카드팩 열기

    /// <summary>
    /// 컬러 카드팩 개봉
    /// </summary>
    public void OpenColorPack()
    {
        // 컬러 카드팩 남은 수량 확인
        if (ColorPackAmount > 0)
        {
            // 개봉 효과 재생
            colorResultCardImg.gameObject.SetActive(false);
            colorCardPackOpenImgs[0].SetActive(false);
            colorCardPackOpenImgs[1].SetActive(true);
            colorOpenButtons[0].enabled = colorOpenButtons[1].enabled = false;
            colorPackOpenAni[0].Play();
            cardPackAudio.clip = cardAudioClips[0];
            cardPackAudio.Play();

            // 정해진 확률에 따라 카드 선정
            int r = Random.Range(0, 100), possibility = 0;
            int resultCard = 0;
            for (int i = 0; i < colorCardAbilities.Length; i++)
            {
                possibility += colorCardAbilities[i].probability;
                if (r < possibility)
                {
                    resultCard = i;
                    break;
                }
            }
            // 재생중인 애니메이션 종료 후에 카드 선정 결과 표시
            StartCoroutine(WaitOpenAnimation(colorPackOpenAni[0], AfterOpenColorPack, resultCard));
        }
        else
            messageManager.ShowMessage("컬러 카드팩을 보유하고 있지 않습니다.");
    }

    /// <summary>
    /// 컬러 카드팩 결과 표시
    /// </summary>
    /// <param name="card">선정된 카드</param>
    private void AfterOpenColorPack(int card)
    {
        // 컬러 카드팩 개수 차감
        ColorPackAmount--;
        // 선정된 카드 획득 처리
        ColorCardAmounts[card]++;
        cardPackAudio.clip = cardAudioClips[1];
        cardPackAudio.Play();

        // 카드 설명 정보 업데이트
        colorResultDescText.text = colorCardAbilities[card].duration + "초 동안 ";
        if (colorCardAbilities[card].ability[0] > 1 && colorCardAbilities[card].ability[1] > 1)
            colorResultDescText.text += "터치형 수익 " + colorCardAbilities[card].ability[0] + "배 및\n시간형 수익" + colorCardAbilities[card].ability[1] + "배 증가";
        else if (colorCardAbilities[card].ability[0] > 1)
            colorResultDescText.text += "터치형 수익 " + colorCardAbilities[card].ability[0] + "배 증가";
        else
            colorResultDescText.text += "시간형 수익 " + colorCardAbilities[card].ability[1] + "배 증가";

        // 카드 이미지 정보 업데이트
        colorResultCardImg.sprite = colorCardAbilities[card].cardSprite;
        colorResultCardImg.gameObject.SetActive(true);
        colorPackOpenAni[1].Play();
        colorCardPackOpenImgs[0].SetActive(true);
        colorCardPackOpenImgs[1].SetActive(false);
        colorOpenButtons[0].enabled = colorOpenButtons[1].enabled = true;
        UpdateColorCardInfo();
    }

    /// <summary>
    /// 레어 카드팩 개봉
    /// </summary>
    public void OpenRarePack()
    {
        // 레어 카드팩 수량 확인
        if (RarePackAmount > 0)
        {
            // 개봉 효과 재생
            rareResultCardImg.gameObject.SetActive(false);
            rareCardPackOpenImgs[0].SetActive(false);
            rareCardPackOpenImgs[1].SetActive(true);
            rareOpenButtons[0].enabled = rareOpenButtons[1].enabled = false;
            rarePackOpenAni[0].Play();
            cardPackAudio.clip = cardAudioClips[0];
            cardPackAudio.Play();

            // 정해진 확률에 따라 카드 선정
            int r = Random.Range(0, 100), possibility = 0;
            int resultCard = 0;
            for (int i = 0; i < rareCardAbilities.Length; i++)
            {
                possibility += rareCardAbilities[i].probability;
                if (r < possibility)
                {
                    resultCard = i;
                    break;
                }
            }
            // 재생중인 애니메이션 종료 후에 카드 선정 결과 표시
            StartCoroutine(WaitOpenAnimation(rarePackOpenAni[0], AfterOpenRarePack, resultCard));
        }
        else
            messageManager.ShowMessage("레어 카드팩을 보유하고 있지 않습니다.");
    }
    /// <summary>
    /// 레어 카드팩 결과 표시
    /// </summary>
    /// <param name="card">선정된 카드</param>
    private void AfterOpenRarePack(int card)
    {
        // 레어 카드팩 개수 차감
        RarePackAmount--;
        // 선정된 카드 획득 처리
        RareCardAmounts[card]++;
        cardPackAudio.clip = cardAudioClips[1];
        cardPackAudio.Play();

        // 카드 설명 정보 업데이트
        rareResultDescText.text = rareCardAbilities[card].duration + "초 동안 1초 당 " + rareCardAbilities[card].ability[0] + "회 자동 터치";

        // 카드 이미지 정보 업데이트
        rareResultCardImg.sprite = rareCardAbilities[card].cardSprite;
        rareResultCardImg.gameObject.SetActive(true);
        rarePackOpenAni[1].Play();
        rareCardPackOpenImgs[0].SetActive(true);
        rareCardPackOpenImgs[1].SetActive(false);
        rareOpenButtons[0].enabled = rareOpenButtons[1].enabled = true;
        UpdateRareCardInfo();
    }

    /// <summary>
    /// 애니메이션 재생 종료 대기 코루틴
    /// </summary>
    /// <param name="openingAni">재생중인 애니메이션</param>
    /// <param name="afterAnimation">종료 이후 호출할 함수</param>
    /// <param name="card">파라미터(카드 인덱스)</param>
    IEnumerator WaitOpenAnimation(Animation openingAni, AfterAnimation afterAnimation, int card)
    {
        while (openingAni.isPlaying)
            yield return new WaitForEndOfFrame();

        afterAnimation(card);
    }
    #endregion
}

/// <summary>
/// 카드 스펙 데이터
/// </summary>
[System.Serializable]
public struct CardAbility
{
    /// <summary>
    /// 카드 이름
    /// </summary>
    public string name;
    /// <summary>
    /// 지속 시간
    /// </summary>
    public int duration;
    /// <summary>
    /// 능력치
    /// </summary>
    public int[] ability;
    /// <summary>
    /// 등장 확률
    /// </summary>
    public int probability;
    /// <summary>
    /// 카드 스프라이트 이미지 리소스
    /// </summary>
    public Sprite cardSprite;
}
