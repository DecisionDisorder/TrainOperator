using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public ItemData itemData;

    public CardAbility[] colorCardAbilities;
    public CardAbility[] rareCardAbilities;

    public Button[] colorCardButtons;
    public Button[] rareCardButtons;
    public Text[] colorCardAmountTexts;
    public Text[] rareCardAmountTexts;
    public Text[] colorPackAmountText;
    public Text[] rarePackAmountText;

    public Text cardPointText;
    public Text feverRefillText;

    public Image cardTimer;
    public Image cardTimerImg;

    public GameObject cardPackMenu;
    public GameObject purchaseMenu;
    public GameObject[] purchaseTypeObjs;
    public GameObject[] openCardPackMenus;
    public GameObject cardHelpMenu;

    public GameObject[] colorCardPackOpenImgs;
    public GameObject[] rareCardPackOpenImgs;
    public Animation[] colorPackOpenAni;
    public Animation[] rarePackOpenAni;
    public Button[] colorOpenButtons;
    public Button[] rareOpenButtons;
    public Image colorResultCardImg;
    public Text colorResultDescText;
    public Image rareResultCardImg;
    public Text rareResultDescText;

    public AudioSource cardPackAudio;
    public AudioClip[] cardAudioClips;

    private delegate void AfterAnimation(int card);
    public TimeMoneyManager timeMoneyManager;

    public int[] ColorCardAmounts
    {
        get
        {
            ReallocateColorCardAmount();
            return itemData.cardAmounts;
        }
        set { itemData.cardAmounts = value; UpdateColorCardInfo(); }
    }
    public int[] RareCardAmounts
    {
        get
        {
            if (itemData.rareCardAmounts == null) itemData.rareCardAmounts = new int[6];
            return itemData.rareCardAmounts;
        }
        set { itemData.rareCardAmounts = value; UpdateRareCardInfo(); }
    }
    public int ColorPackAmount { get { return itemData.silverCardAmount; } set { itemData.silverCardAmount = value; SetPackAmountText(); } }
    public int RarePackAmount { get { return itemData.rareCardPackAmount; } set { itemData.rareCardPackAmount = value; SetPackAmountText(); } }

    public int CardPoint { get { return itemData.cardPoint; } set { itemData.cardPoint = value; SetCardPointText(); } }
    
    public int FeverRefillAmount { get { return itemData.feverRefillAmount; } set { if (value >= 0) itemData.feverRefillAmount = value; UpdateFeverRefillText(); } }

    private ulong activedTouchAbility;
    public ulong activedTimeAbility;

    public int[] purchaseAmounts;
    public int[] colorPackPrices;
    public int[] rarePackPrices;
    private int purchasePackType;
    private int purchaseAmountType;

    public Color purchaseBackgroundColor;
    public Color feverBackgroundColor;

    public bool itemActived;

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

    private void SetCardPointText()
    {
        cardPointText.text = string.Format("{0:#,##0}P", CardPoint);
    }

    public void UseColorCard(int card)
    {
        if (ColorCardAmounts[card] > 0)
        {
            ColorCardAmounts[card]--;
            UpdateColorCardInfo();
            itemActived = true;
            cardTimer.gameObject.SetActive(true);

            int duration = colorCardAbilities[card].duration;
            cardTimerImg.sprite = colorCardAbilities[card].cardSprite;
            cardTimer.sprite = colorCardAbilities[card].cardSprite;

            SetMassiveIncomeEnable(duration, (ulong)colorCardAbilities[card].ability[0], (ulong)colorCardAbilities[card].ability[1], ref activedTouchAbility, ref activedTimeAbility);

            ActiveCards(false);
            StartCoroutine(ColorCardTimer(duration, duration));
        }
        else
            messageManager.ShowMessage(colorCardAbilities[card].name + "를 보유하고 있지 않습니다.");
    }

    public void SetMassiveIncomeEnable(int duration, ulong touchMoneyAbility, ulong timeMoneyAbility, ref ulong activedTouchAbility, ref ulong activedTimeAbility)
    {
        TouchEarning.externalCoefficient += touchMoneyAbility;
        activedTouchAbility = touchMoneyAbility;

        activedTimeAbility = timeMoneyAbility;
        timeMoneyManager.externalCoefficient += timeMoneyAbility;

        TouchEarning.randomSetTime += duration; // 이벤트 시간 미룸
        miniGameManager.timeLeft += duration;

        CompanyReputationManager.instance.RenewPassengerRandom();
        assetInfoUpdater.UpdateTimeMoneyText();
    }

    public void SetMassiveIncomeDisable(ulong activedTouchAbility, ulong activedTimeAbility)
    {
        TouchEarning.externalCoefficient -= activedTouchAbility;
        timeMoneyManager.externalCoefficient -= activedTimeAbility;
        assetInfoUpdater.UpdateTimeMoneyText();
        CompanyReputationManager.instance.RenewPassengerRandom();
    }

    private void DisableColorCardEffect()
    {
        DisableCardEffect();
        SetMassiveIncomeDisable(activedTouchAbility, activedTimeAbility);
    }

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

    public void UseRareCard(int card)
    {
        if(RareCardAmounts[card] > 0)
        {
            RareCardAmounts[card]--;
            UpdateRareCardInfo();
            itemActived = true;
            cardTimer.gameObject.SetActive(true);

            int duration = rareCardAbilities[card].duration;
            cardTimerImg.sprite = rareCardAbilities[card].cardSprite;
            cardTimer.sprite = rareCardAbilities[card].cardSprite;

            TouchEarning.randomSetTime += duration; // 이벤트 시간 미룸
            miniGameManager.timeLeft += duration;

            ActiveCards(false);
            StartCoroutine(AutoClicker(rareCardAbilities[card].ability[0], duration, duration));
        }
        else
            messageManager.ShowMessage(rareCardAbilities[card].name + "를 보유하고 있지 않습니다.");
    }

    private void DisableCardEffect()
    {
        itemActived = false;
        ActiveCards(true);
        cardTimer.gameObject.SetActive(false);
    }

    IEnumerator AutoClicker(int touchPerSecond, float remainTime, int fullTime)
    {
        float interval = 1f / touchPerSecond;
        yield return new WaitForSeconds(interval);
        
        touchEarning.TouchIncome();
        cardTimerImg.fillAmount = (fullTime - remainTime) / fullTime;

        remainTime -= interval;
        if (remainTime > 0)
            StartCoroutine(AutoClicker(touchPerSecond, remainTime, fullTime));
        else
            DisableCardEffect();
    }

    private void UpdateColorCardInfo()
    {
        for (int i = 0; i < colorCardAmountTexts.Length; i++)
            colorCardAmountTexts[i].text = ColorCardAmounts[i] + "개";
    }

    private void UpdateRareCardInfo()
    {
        for (int i = 0; i < rareCardAmountTexts.Length; i++)
            rareCardAmountTexts[i].text = RareCardAmounts[i] + "개";
    }

    private void ActiveCards(bool active)
    {
        SetActiveColorCard(active);
        SetActiveRareCard(active);
    }

    public void SetActiveColorCard(bool active)
    {
        for (int i = 0; i < colorCardButtons.Length; i++)
            colorCardButtons[i].interactable = active;
    }
    public void SetActiveRareCard(bool active)
    {
        for (int i = 0; i < rareCardButtons.Length; i++)
            rareCardButtons[i].interactable = active;
    }

    public void UseFeverRefill()
    {
        if (FeverRefillAmount > 0)
        {
            messageManager.OpenCommonCheckMenu("피버 충전 쿠폰 사용 확인", "피버 충전 쿠폰을 사용하여\n피버 게이지를 완전히 충전하시겠습니까?", feverBackgroundColor, RefillFever);
        }
        else
            messageManager.ShowMessage("피버 충전 쿠폰이 없습니다.");
    }

    private void RefillFever()
    {
        if (feverManager.FeverStack < feverManager.targetFeverStack)
        {
            feverManager.FeverStack = feverManager.targetFeverStack;
            FeverRefillAmount--;
            messageManager.ShowMessage("피버 게이지를 완전히 충전했습니다!");
        }
        else
            messageManager.ShowMessage("피버 게이지가 이미 완전히 충전되어있습니다.");
    }
    private void UpdateFeverRefillText()
    {
        feverRefillText.text = "피버 충전 쿠폰\n" + FeverRefillAmount + "개";
    }
    #endregion
    #region 카드팩 구매 기능
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

    private void Cancel() { }

    public void PurchasePack()
    {
        if (purchasePackType.Equals(0))
        {
            if (CardPoint >= colorPackPrices[purchaseAmountType])
            {
                CardPoint -= colorPackPrices[purchaseAmountType];
                ColorPackAmount += purchaseAmounts[purchaseAmountType];
                messageManager.ShowMessage("컬러 카드팩 " + purchaseAmounts[purchaseAmountType] + "개를 구매하였습니다.");
                purchaseAudio.Play();
            }
            else
                messageManager.ShowMessage("카드 포인트가 부족합니다.");
        }
        else
        {
            if (CardPoint >= rarePackPrices[purchaseAmountType])
            {
                CardPoint -= rarePackPrices[purchaseAmountType];
                RarePackAmount += purchaseAmounts[purchaseAmountType];
                messageManager.ShowMessage("레어 카드팩 " + purchaseAmounts[purchaseAmountType] + "개를 구매하였습니다.");
                purchaseAudio.Play();
            }
            else
                messageManager.ShowMessage("카드 포인트가 부족합니다.");
        }
    }

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
    public void OpenColorPack()
    {
        if (ColorPackAmount > 0)
        {
            colorResultCardImg.gameObject.SetActive(false);
            colorCardPackOpenImgs[0].SetActive(false);
            colorCardPackOpenImgs[1].SetActive(true);
            colorOpenButtons[0].enabled = colorOpenButtons[1].enabled = false;
            colorPackOpenAni[0].Play();
            cardPackAudio.clip = cardAudioClips[0];
            cardPackAudio.Play();

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
            StartCoroutine(WaitOpenAnimation(colorPackOpenAni[0], AfterOpenColorPack, resultCard));
        }
        else
            messageManager.ShowMessage("컬러 카드팩을 보유하고 있지 않습니다.");
    }

    private void AfterOpenColorPack(int card)
    {
        ColorPackAmount--;
        ColorCardAmounts[card]++;
        cardPackAudio.clip = cardAudioClips[1];
        cardPackAudio.Play();

        colorResultDescText.text = colorCardAbilities[card].duration + "초 동안 ";
        if (colorCardAbilities[card].ability[0] > 1 && colorCardAbilities[card].ability[1] > 1)
            colorResultDescText.text += "터치형 수익 " + colorCardAbilities[card].ability[0] + "배 및\n시간형 수익" + colorCardAbilities[card].ability[1] + "배 증가";
        else if (colorCardAbilities[card].ability[0] > 1)
            colorResultDescText.text += "터치형 수익 " + colorCardAbilities[card].ability[0] + "배 증가";
        else
            colorResultDescText.text += "시간형 수익 " + colorCardAbilities[card].ability[1] + "배 증가";


        colorResultCardImg.sprite = colorCardAbilities[card].cardSprite;
        colorResultCardImg.gameObject.SetActive(true);
        colorPackOpenAni[1].Play();
        colorCardPackOpenImgs[0].SetActive(true);
        colorCardPackOpenImgs[1].SetActive(false);
        colorOpenButtons[0].enabled = colorOpenButtons[1].enabled = true;
        UpdateColorCardInfo();
    }

    public void OpenRarePack()
    {
        if (RarePackAmount > 0)
        {
            rareResultCardImg.gameObject.SetActive(false);
            rareCardPackOpenImgs[0].SetActive(false);
            rareCardPackOpenImgs[1].SetActive(true);
            rareOpenButtons[0].enabled = rareOpenButtons[1].enabled = false;
            rarePackOpenAni[0].Play();
            cardPackAudio.clip = cardAudioClips[0];
            cardPackAudio.Play();

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
            StartCoroutine(WaitOpenAnimation(rarePackOpenAni[0], AfterOpenRarePack, resultCard));
        }
        else
            messageManager.ShowMessage("레어 카드팩을 보유하고 있지 않습니다.");
    }
    private void AfterOpenRarePack(int card)
    {
        RarePackAmount--;
        RareCardAmounts[card]++;
        cardPackAudio.clip = cardAudioClips[1];
        cardPackAudio.Play();

        rareResultDescText.text = rareCardAbilities[card].duration + "초 동안 1초 당 " + rareCardAbilities[card].ability[0] + "회 자동 터치";

        rareResultCardImg.sprite = rareCardAbilities[card].cardSprite;
        rareResultCardImg.gameObject.SetActive(true);
        rarePackOpenAni[1].Play();
        rareCardPackOpenImgs[0].SetActive(true);
        rareCardPackOpenImgs[1].SetActive(false);
        rareOpenButtons[0].enabled = rareOpenButtons[1].enabled = true;
        UpdateRareCardInfo();
    }

    IEnumerator WaitOpenAnimation(Animation openingAni, AfterAnimation afterAnimation, int card)
    {
        while (openingAni.isPlaying)
            yield return new WaitForEndOfFrame();

        afterAnimation(card);
    }
    #endregion
}

[System.Serializable]
public struct CardAbility
{
    public string name;
    public int duration;
    public int[] ability;
    public int probability;
    public Sprite cardSprite;
}
