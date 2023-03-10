using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������ �ý��� ���� Ŭ����
/// </summary>
public class ItemManager : MonoBehaviour
{
    /// <summary>
    /// ������ ������ ������Ʈ
    /// </summary>
    public ItemData itemData;

    /// <summary>
    /// �÷� ī���� ���� �� ���� ������ �迭
    /// </summary>
    public CardAbility[] colorCardAbilities;
    /// <summary>
    /// ���� ī���� ���� �� ���� ������ �迭
    /// </summary>
    public CardAbility[] rareCardAbilities;

    /// <summary>
    /// �÷� ī�� ��� ��ư �迭
    /// </summary>
    public Button[] colorCardButtons;
    /// <summary>
    /// ���� ī�� ��� ��ư �迭
    /// </summary>
    public Button[] rareCardButtons;
    /// <summary>
    /// �÷� ī�� ���� ������ ���� ���� �ؽ�Ʈ �迭
    /// </summary>
    public Text[] colorCardAmountTexts;
    /// <summary>
    /// ���� ī�� ���� ������ ���� ���� �ؽ�Ʈ �迭
    /// </summary>
    public Text[] rareCardAmountTexts;
    /// <summary>
    /// �÷� ī���� ���� ���� �ؽ�Ʈ �迭
    /// </summary>
    public Text[] colorPackAmountText;
    /// <summary>
    /// ���� ī���� ���� ���� �ؽ�Ʈ �迭
    /// </summary>
    public Text[] rarePackAmountText;

    /// <summary>
    /// ī�� ����Ʈ ���� �ؽ�Ʈ
    /// </summary>
    public Text cardPointText;
    /// <summary>
    /// �ǹ� ���� ���� ���� �ؽ�Ʈ
    /// </summary>
    public Text feverRefillText;

    /// <summary>
    /// ī�� Ÿ�̸� ��� �̹���
    /// </summary>
    public Image cardTimer;
    /// <summary>
    /// ī�� Ÿ�̸� fill �̹���
    /// </summary>
    public Image cardTimerImg;

    /// <summary>
    /// ī���� ���� �޴� ������Ʈ
    /// </summary>
    public GameObject cardPackMenu;
    /// <summary>
    /// ī���� ���� �޴� ������Ʈ
    /// </summary>
    public GameObject purchaseMenu;
    /// <summary>
    /// ī���� ���� �޴��� ǥ���� ī���� ������Ʈ �迭
    /// </summary>
    public GameObject[] purchaseTypeObjs;
    /// <summary>
    /// ī���� ���� �޴� ������Ʈ �迭
    /// </summary>
    public GameObject[] openCardPackMenus;
    /// <summary>
    /// ī���� ���� �޴� ������Ʈ
    /// </summary>
    public GameObject cardHelpMenu;

    /// <summary>
    /// �÷� ī���� ���� ��/�� �̹��� �迭
    /// </summary>
    public GameObject[] colorCardPackOpenImgs;
    /// <summary>
    /// ���� ī���� ���� ��/�� �̹��� �迭
    /// </summary>
    public GameObject[] rareCardPackOpenImgs;
    /// <summary>
    /// �÷� ī���� ���� �ִϸ��̼� ȿ�� �迭
    /// </summary>
    public Animation[] colorPackOpenAni;
    /// <summary>
    /// ���� ī���� ���� �ִϸ��̼� ȿ�� �迭
    /// </summary>
    public Animation[] rarePackOpenAni;
    /// <summary>
    /// �÷� ī���� ���� ��ư �迭
    /// </summary>
    public Button[] colorOpenButtons;
    /// <summary>
    /// ���� ī���� ���� ��ư �迭
    /// </summary>
    public Button[] rareOpenButtons;
    /// <summary>
    /// �÷� ī���� ���� ��� ī�� �̹���
    /// </summary>
    public Image colorResultCardImg;
    /// <summary>
    /// �÷� ī���� ���� ��� ���� �ؽ�Ʈ
    /// </summary>
    public Text colorResultDescText;
    /// <summary>
    /// ���� ī���� ���� ��� ī�� �̹���
    /// </summary>
    public Image rareResultCardImg;
    /// <summary>
    /// ���� ī���� ���� ��� ���� �ؽ�Ʈ
    /// </summary>
    public Text rareResultDescText;
    
    /// <summary>
    /// ī���� ���� ����� ȿ����
    /// </summary>
    public AudioSource cardPackAudio;
    /// <summary>
    /// ī���� ���� ȿ���� Ŭ��
    /// </summary>
    public AudioClip[] cardAudioClips;

    private delegate void AfterAnimation(int card);
    public TimeMoneyManager timeMoneyManager;

    /// <summary>
    /// �÷� ī�� ������ ���� ����
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
    /// ���� ī�� ������ ���� ����
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
    /// �÷� ī���� ���� ����
    /// </summary>
    public int ColorPackAmount { get { return itemData.silverCardAmount; } set { itemData.silverCardAmount = value; SetPackAmountText(); } }
    /// <summary>
    /// ���� ī���� ���� ����
    /// </summary>
    public int RarePackAmount { get { return itemData.rareCardPackAmount; } set { itemData.rareCardPackAmount = value; SetPackAmountText(); } }

    /// <summary>
    /// ī�� ����Ʈ ��ġ
    /// </summary>
    public int CardPoint { get { return itemData.cardPoint; } set { itemData.cardPoint = value; SetCardPointText(); } }
    
    /// <summary>
    /// �ǹ� ���� ���� ���� ���� 
    /// </summary>
    public int FeverRefillAmount { get { return itemData.feverRefillAmount; } set { if (value >= 0) itemData.feverRefillAmount = value; UpdateFeverRefillText(); } }

    /// <summary>
    /// ������ ȿ���� ���� ���� ��ġ�� ���� ������
    /// </summary>
    private ulong activedTouchAbility;
    /// <summary>
    /// ������ ȿ���� ���� ���� �ð��� ���� ������
    /// </summary>
    public ulong activedTimeAbility;

    /// <summary>
    /// ī������ 1ȸ ���� ���� �迭 (1/5/10)
    /// </summary>
    public int[] purchaseAmounts;
    /// <summary>
    /// 1ȸ ���� ���� ��ǰ �� �÷� ī���� ���� ����
    /// </summary>
    public int[] colorPackPrices;
    /// <summary>
    /// 1ȸ ���� ���� ��ǰ �� ���� ī���� ���� ����
    /// </summary>
    public int[] rarePackPrices;
    /// <summary>
    /// �����ϴ� ī���� ���� �ε���
    /// </summary>
    private int purchasePackType;
    /// <summary>
    /// �����ϴ� 1ȸ ���� ���� �ε���
    /// </summary>
    private int purchaseAmountType;

    /// <summary>
    /// ���� ��� ����
    /// </summary>
    public Color purchaseBackgroundColor;
    /// <summary>
    /// �ǹ� ��� ����
    /// </summary>
    public Color feverBackgroundColor;

    /// <summary>
    /// ������ ȿ���� ���� ������ ����
    /// </summary>
    public bool itemActived;

    /// <summary>
    /// ���� ȿ����
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
    
    #region �޴� ����
    public void SetMenuOn(int menu)
    {
        switch(menu)
        {
            case 0://ī���� �޴�
                cardPackMenu.SetActive(true);
                break;
            case 1://�÷� ī���� ����
                purchaseMenu.SetActive(true);
                purchaseTypeObjs[0].SetActive(true);
                break;
            case 2://���� ī���� ����
                purchaseMenu.SetActive(true);
                purchaseTypeObjs[1].SetActive(true);
                break;
            case 3://�÷� ī���� ����
                openCardPackMenus[0].SetActive(true);
                break;
            case 4://���� ī���� ����
                openCardPackMenus[1].SetActive(true);
                break;
            case 5://ī�� ����
                cardHelpMenu.SetActive(true);
                break;
        }
    }
    public void SetMenuOff(int menu)
    {
        switch (menu)
        {
            case 0://ī���� �޴�
                cardPackMenu.SetActive(false);
                break;
            case 1://�÷� ī���� ����
                purchaseMenu.SetActive(false);
                purchaseTypeObjs[0].SetActive(false);
                purchaseTypeObjs[1].SetActive(false);
                break;
            case 3://�÷� ī���� ����
                openCardPackMenus[0].SetActive(false);
                colorResultCardImg.gameObject.SetActive(false);
                break;
            case 4://���� ī���� ����
                openCardPackMenus[1].SetActive(false);
                rareResultCardImg.gameObject.SetActive(false);
                break;
            case 5://ī�� ����
                cardHelpMenu.SetActive(false);
                break;
        }
    }
    #endregion
    #region ������ ���
    /// <summary>
    /// �÷� ī���� ���� ������ ���Ҵ�
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
    /// ī�� ����Ʈ ���� ������Ʈ
    /// </summary>
    private void SetCardPointText()
    {
        cardPointText.text = string.Format("{0:#,##0}P", CardPoint);
    }

    /// <summary>
    /// �÷� ī�� ������ ��� ó��
    /// </summary>
    /// <param name="card">ī�� ���� �ε���</param>
    public void UseColorCard(int card)
    {
        // ���� ī�� ���� Ȯ��
        if (ColorCardAmounts[card] > 0)
        {
            // ī�� �Ҹ� ó�� �� UI ������Ʈ
            ColorCardAmounts[card]--;
            UpdateColorCardInfo();
            // ������ Ȱ��ȭ ó�� �� Ÿ�̸� �ð��� ȿ�� ����
            itemActived = true;
            cardTimer.gameObject.SetActive(true);

            int duration = colorCardAbilities[card].duration;
            cardTimerImg.sprite = colorCardAbilities[card].cardSprite;
            cardTimer.sprite = colorCardAbilities[card].cardSprite;

            // ��ġ�� Ȥ�� �ð��� ���� ���޷� ���� ȿ�� ����
            SetMassiveIncomeEnable(duration, (ulong)colorCardAbilities[card].ability[0], (ulong)colorCardAbilities[card].ability[1], ref activedTouchAbility, ref activedTimeAbility);

            // Ȱ��ȭ ����, �ٸ� ������ ��� �Ұ� ó��
            ActiveCards(false);
            StartCoroutine(ColorCardTimer(duration, duration));
        }
        else
            messageManager.ShowMessage(colorCardAbilities[card].name + "�� �����ϰ� ���� �ʽ��ϴ�.");
    }

    /// <summary>
    /// ��ġ��/�ð��� ���� ���޷� ���� ȿ�� ����
    /// </summary>
    /// <param name="duration">���ӽð�</param>
    /// <param name="touchMoneyAbility">��ġ�� ���� ������</param>
    /// <param name="timeMoneyAbility">�ð��� ���� ������</param>
    /// <param name="activedTouchAbility">������ ȿ���� ���� �� ��ġ�� ���� ������</param>
    /// <param name="activedTimeAbility">������ ȿ���� ���� �� �ð��� ���� ������</param>
    public void SetMassiveIncomeEnable(int duration, ulong touchMoneyAbility, ulong timeMoneyAbility, ref ulong activedTouchAbility, ref ulong activedTimeAbility)
    {
        TouchEarning.externalCoefficient += touchMoneyAbility;
        activedTouchAbility = touchMoneyAbility;

        activedTimeAbility = timeMoneyAbility;
        timeMoneyManager.externalCoefficient += timeMoneyAbility;

        // ���� �°� �� ������ �� �̴ϰ��� ���ð� ������
        TouchEarning.randomSetTime += duration; 
        miniGameManager.timeLeft += duration;

        CompanyReputationManager.instance.RenewPassengerRandom();
        assetInfoUpdater.UpdateTimeMoneyText();
    }

    /// <summary>
    /// ��ġ��/�ð��� ���� ���޷� ����
    /// </summary>
    /// <param name="activedTouchAbility">������ ȿ���� ����Ǿ��� ��ġ�� ���� ������</param>
    /// <param name="activedTimeAbility">������ ȿ���� ����Ǿ��� �ð��� ���� ������</param>
    public void SetMassiveIncomeDisable(ulong activedTouchAbility, ulong activedTimeAbility)
    {
        TouchEarning.externalCoefficient -= activedTouchAbility;
        timeMoneyManager.externalCoefficient -= activedTimeAbility;
        assetInfoUpdater.UpdateTimeMoneyText();
        CompanyReputationManager.instance.RenewPassengerRandom();
    }

    /// <summary>
    /// �÷� ī�� ���� ȿ�� ��Ȱ��ȭ ó��
    /// </summary>
    private void DisableColorCardEffect()
    {
        DisableCardEffect();
        SetMassiveIncomeDisable(activedTouchAbility, activedTimeAbility);
    }

    /// <summary>
    /// �÷� ī�� ���� �ð� Ÿ�̸�
    /// </summary>
    /// <param name="remainTime">���� ���� �ð�</param>
    /// <param name="fullTime">�� ���� �ð�</param>
    /// <param name="interval">������Ʈ ���� �ð�(��)</param>
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
    /// ���� ī�� ��� ó��
    /// </summary>
    /// <param name="card">ī�� ���� �ε���</param>
    public void UseRareCard(int card)
    {
        // ī�� ���� ���� Ȯ��
        if(RareCardAmounts[card] > 0)
        {
            // ī�� �Ҹ� ó�� �� UI ������Ʈ
            RareCardAmounts[card]--;
            UpdateRareCardInfo();

            // ������ Ȱ��ȭ ó�� �� Ÿ�̸� �ð��� ȿ�� ����
            itemActived = true;
            cardTimer.gameObject.SetActive(true);

            int duration = rareCardAbilities[card].duration;
            cardTimerImg.sprite = rareCardAbilities[card].cardSprite;
            cardTimer.sprite = rareCardAbilities[card].cardSprite;

            // ���� �°� �� ������ �� �̴ϰ��� ���ð� ������
            TouchEarning.randomSetTime += duration; 
            miniGameManager.timeLeft += duration;

            // Ȱ��ȭ ����, �ٸ� ������ ��� �Ұ� ó��
            ActiveCards(false);
            // �ڵ� ��ġ ȿ�� ����
            StartCoroutine(AutoClicker(rareCardAbilities[card].ability[0], duration, duration));
        }
        else
            messageManager.ShowMessage(rareCardAbilities[card].name + "�� �����ϰ� ���� �ʽ��ϴ�.");
    }

    /// <summary>
    /// ī�� ������ ��� ȿ�� ��Ȱ��ȭ ó��
    /// </summary>
    private void DisableCardEffect()
    {
        itemActived = false;
        ActiveCards(true);
        cardTimer.gameObject.SetActive(false);
    }

    /// <summary>
    /// ���� ī�� ȿ��: �ڵ� ��ġ �ݺ� �ڷ�ƾ
    /// </summary>
    /// <param name="touchPerSecond">�ʴ� �ڵ� ��ġ Ƚ��</param>
    /// <param name="remainTime">���� �ð�</param>
    /// <param name="fullTime">�� ���� �ð�</param>
    IEnumerator AutoClicker(int touchPerSecond, float remainTime, int fullTime)
    {
        float interval = 1f / touchPerSecond;
        yield return new WaitForSeconds(interval);
        
        // ��ġ ó�� �� Ÿ�̸� ������Ʈ
        touchEarning.TouchIncome();
        cardTimerImg.fillAmount = (fullTime - remainTime) / fullTime;

        remainTime -= interval;
        if (remainTime > 0)
            StartCoroutine(AutoClicker(touchPerSecond, remainTime, fullTime));
        else
            DisableCardEffect();
    }

    /// <summary>
    /// �÷� ī�� ���� ���� ������Ʈ
    /// </summary>
    private void UpdateColorCardInfo()
    {
        for (int i = 0; i < colorCardAmountTexts.Length; i++)
            colorCardAmountTexts[i].text = ColorCardAmounts[i] + "��";
    }

    /// <summary>
    /// ���� ī�� ���� ���� ������Ʈ
    /// </summary>
    private void UpdateRareCardInfo()
    {
        for (int i = 0; i < rareCardAmountTexts.Length; i++)
            rareCardAmountTexts[i].text = RareCardAmounts[i] + "��";
    }

    /// <summary>
    /// ī�� ������ ��� Ȱ��ȭ/��Ȱ��ȭ ó��
    /// </summary>
    /// <param name="active">��� ���� ����</param>
    private void ActiveCards(bool active)
    {
        SetActiveColorCard(active);
        SetActiveRareCard(active);
    }

    /// <summary>
    /// �÷� ī�� ��� Ȱ��ȭ/��Ȱ��ȭ ó��
    /// </summary>
    /// <param name="active">��� ���� ����</param>
    public void SetActiveColorCard(bool active)
    {
        for (int i = 0; i < colorCardButtons.Length; i++)
            colorCardButtons[i].interactable = active;
    }
    /// <summary>
    /// ���� ī�� ��� Ȱ��ȭ/��Ȱ��ȭ ó��
    /// </summary>
    /// <param name="active">��� ���� ����</param>
    public void SetActiveRareCard(bool active)
    {
        for (int i = 0; i < rareCardButtons.Length; i++)
            rareCardButtons[i].interactable = active;
    }

    /// <summary>
    /// �ǹ� ������ ���� ���� ��� Ȯ��
    /// </summary>
    public void UseFeverRefill()
    {
        if (FeverRefillAmount > 0)
        {
            messageManager.OpenCommonCheckMenu("�ǹ� ���� ���� ��� Ȯ��", "�ǹ� ���� ������ ����Ͽ�\n�ǹ� �������� ������ �����Ͻðڽ��ϱ�?\n<size=25>(�������� �����Ǹ� ���ϴ� Ÿ�ֿ̹�\n���� �ǹ� Ÿ���� ������ �� �ֽ��ϴ�.)</size>", feverBackgroundColor, RefillFever);
        }
        else
            messageManager.ShowMessage("�ǹ� ���� ������ �����ϴ�.");
    }

    /// <summary>
    /// �ǹ� ������ ���� ���� ��� ó��
    /// </summary>
    private void RefillFever()
    {
        // �̹� �ǹ� ������ �ִ��� ���¿����� ������ �ʵ��� ����
        if (feverManager.FeverStack < feverManager.targetFeverStack)
        {
            // �ǹ� ���� ���� �� ���� �Ҹ� ó��
            feverManager.FeverStack = feverManager.targetFeverStack;
            FeverRefillAmount--;
            messageManager.ShowMessage("�ǹ� �������� ������ �����߽��ϴ�!");
        }
        else
            messageManager.ShowMessage("�ǹ� �������� �̹� ������ �����Ǿ��ֽ��ϴ�.");
    }
    /// <summary>
    /// �ǹ� ���� ���� ���� ���� ������Ʈ
    /// </summary>
    private void UpdateFeverRefillText()
    {
        feverRefillText.text = "�ǹ� ���� ����\n" + FeverRefillAmount + "��";
    }
    #endregion
    #region ī���� ���� ���
    /// <summary>
    /// �÷� ī���� ���� Ȯ�� �޴� Ȱ��ȭ
    /// </summary>
    /// <param name="type">������ ���� ��ǰ �ε���</param>
    public void OpenColorPackPurchaseCheck(int type)
    {
        purchasePackType = 0;
        purchaseAmountType = type;
        string title = "ī���� ���� Ȯ��";
        string product = "��ǰ: �÷� ī����";
        string amount = "����: " + purchaseAmounts[type] + "��";
        string price = string.Format("����: {0:#,##0}P", colorPackPrices[type]);

        messageManager.SetPurchaseCheckMenu(title, product, amount, price, purchaseBackgroundColor, PurchasePack, Cancel);
    }
    /// <summary>
    /// ���� ī���� ���� Ȯ�� �޴� Ȱ��ȭ
    /// </summary>
    /// <param name="type">������ ���� ��ǰ �ε���</param>
    public void OpenRarePackPurchaseCheck(int type)
    {
        purchasePackType = 1;
        purchaseAmountType = type;
        string title = "ī���� ���� Ȯ��";
        string product = "��ǰ: ���� ī����";
        string amount = "����: " + purchaseAmounts[type] + "��";
        string price = string.Format("����: {0:#,##0}P", rarePackPrices[type]);

        messageManager.SetPurchaseCheckMenu(title, product, amount, price, purchaseBackgroundColor, PurchasePack, Cancel);
    }

    /// <summary>
    /// �̺�Ʈ �ݹ� ��Ͽ� ���� ��� �Լ�
    /// </summary>
    private void Cancel() { }

    /// <summary>
    /// ī���� ���� ó��
    /// </summary>
    public void PurchasePack()
    {
        // �÷� ī���� ���� ó��
        if (purchasePackType.Equals(0))
        {
            // ī�� ����Ʈ Ȯ��
            if (CardPoint >= colorPackPrices[purchaseAmountType])
            {
                // ī�� ����Ʈ ���� �� ���� ó��
                CardPoint -= colorPackPrices[purchaseAmountType];
                ColorPackAmount += purchaseAmounts[purchaseAmountType];
                messageManager.ShowMessage("�÷� ī���� " + purchaseAmounts[purchaseAmountType] + "���� �����Ͽ����ϴ�.");
                purchaseAudio.Play();
            }
            else
                messageManager.ShowMessage("ī�� ����Ʈ�� �����մϴ�.");
        }
        // ���� ī���� ���� ó��
        else
        {
            // ī�� ����Ʈ Ȯ��
            if (CardPoint >= rarePackPrices[purchaseAmountType])
            {
                // ī�� ����Ʈ ���� �� ���� ó��
                CardPoint -= rarePackPrices[purchaseAmountType];
                RarePackAmount += purchaseAmounts[purchaseAmountType];
                messageManager.ShowMessage("���� ī���� " + purchaseAmounts[purchaseAmountType] + "���� �����Ͽ����ϴ�.");
                purchaseAudio.Play();
            }
            else
                messageManager.ShowMessage("ī�� ����Ʈ�� �����մϴ�.");
        }
    }

    /// <summary>
    /// ī���� ���� ���� ������Ʈ
    /// </summary>
    private void SetPackAmountText()
    {
        colorPackAmountText[0].text = string.Format("{0:#,##0}��", ColorPackAmount);
        rarePackAmountText[0].text = string.Format("{0:#,##0}��", RarePackAmount);

        colorPackAmountText[1].text = string.Format("���� {0:#,##0}�� ���� ��", ColorPackAmount);
        rarePackAmountText[1].text = string.Format("���� {0:#,##0}�� ���� ��", RarePackAmount);

        colorPackAmountText[2].text = string.Format("���� {0:#,##0}�� ���� ��", ColorPackAmount);
        rarePackAmountText[2].text = string.Format("���� {0:#,##0}�� ���� ��", RarePackAmount);
    }
    #endregion
    #region ī���� ����

    /// <summary>
    /// �÷� ī���� ����
    /// </summary>
    public void OpenColorPack()
    {
        // �÷� ī���� ���� ���� Ȯ��
        if (ColorPackAmount > 0)
        {
            // ���� ȿ�� ���
            colorResultCardImg.gameObject.SetActive(false);
            colorCardPackOpenImgs[0].SetActive(false);
            colorCardPackOpenImgs[1].SetActive(true);
            colorOpenButtons[0].enabled = colorOpenButtons[1].enabled = false;
            colorPackOpenAni[0].Play();
            cardPackAudio.clip = cardAudioClips[0];
            cardPackAudio.Play();

            // ������ Ȯ���� ���� ī�� ����
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
            // ������� �ִϸ��̼� ���� �Ŀ� ī�� ���� ��� ǥ��
            StartCoroutine(WaitOpenAnimation(colorPackOpenAni[0], AfterOpenColorPack, resultCard));
        }
        else
            messageManager.ShowMessage("�÷� ī������ �����ϰ� ���� �ʽ��ϴ�.");
    }

    /// <summary>
    /// �÷� ī���� ��� ǥ��
    /// </summary>
    /// <param name="card">������ ī��</param>
    private void AfterOpenColorPack(int card)
    {
        // �÷� ī���� ���� ����
        ColorPackAmount--;
        // ������ ī�� ȹ�� ó��
        ColorCardAmounts[card]++;
        cardPackAudio.clip = cardAudioClips[1];
        cardPackAudio.Play();

        // ī�� ���� ���� ������Ʈ
        colorResultDescText.text = colorCardAbilities[card].duration + "�� ���� ";
        if (colorCardAbilities[card].ability[0] > 1 && colorCardAbilities[card].ability[1] > 1)
            colorResultDescText.text += "��ġ�� ���� " + colorCardAbilities[card].ability[0] + "�� ��\n�ð��� ����" + colorCardAbilities[card].ability[1] + "�� ����";
        else if (colorCardAbilities[card].ability[0] > 1)
            colorResultDescText.text += "��ġ�� ���� " + colorCardAbilities[card].ability[0] + "�� ����";
        else
            colorResultDescText.text += "�ð��� ���� " + colorCardAbilities[card].ability[1] + "�� ����";

        // ī�� �̹��� ���� ������Ʈ
        colorResultCardImg.sprite = colorCardAbilities[card].cardSprite;
        colorResultCardImg.gameObject.SetActive(true);
        colorPackOpenAni[1].Play();
        colorCardPackOpenImgs[0].SetActive(true);
        colorCardPackOpenImgs[1].SetActive(false);
        colorOpenButtons[0].enabled = colorOpenButtons[1].enabled = true;
        UpdateColorCardInfo();
    }

    /// <summary>
    /// ���� ī���� ����
    /// </summary>
    public void OpenRarePack()
    {
        // ���� ī���� ���� Ȯ��
        if (RarePackAmount > 0)
        {
            // ���� ȿ�� ���
            rareResultCardImg.gameObject.SetActive(false);
            rareCardPackOpenImgs[0].SetActive(false);
            rareCardPackOpenImgs[1].SetActive(true);
            rareOpenButtons[0].enabled = rareOpenButtons[1].enabled = false;
            rarePackOpenAni[0].Play();
            cardPackAudio.clip = cardAudioClips[0];
            cardPackAudio.Play();

            // ������ Ȯ���� ���� ī�� ����
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
            // ������� �ִϸ��̼� ���� �Ŀ� ī�� ���� ��� ǥ��
            StartCoroutine(WaitOpenAnimation(rarePackOpenAni[0], AfterOpenRarePack, resultCard));
        }
        else
            messageManager.ShowMessage("���� ī������ �����ϰ� ���� �ʽ��ϴ�.");
    }
    /// <summary>
    /// ���� ī���� ��� ǥ��
    /// </summary>
    /// <param name="card">������ ī��</param>
    private void AfterOpenRarePack(int card)
    {
        // ���� ī���� ���� ����
        RarePackAmount--;
        // ������ ī�� ȹ�� ó��
        RareCardAmounts[card]++;
        cardPackAudio.clip = cardAudioClips[1];
        cardPackAudio.Play();

        // ī�� ���� ���� ������Ʈ
        rareResultDescText.text = rareCardAbilities[card].duration + "�� ���� 1�� �� " + rareCardAbilities[card].ability[0] + "ȸ �ڵ� ��ġ";

        // ī�� �̹��� ���� ������Ʈ
        rareResultCardImg.sprite = rareCardAbilities[card].cardSprite;
        rareResultCardImg.gameObject.SetActive(true);
        rarePackOpenAni[1].Play();
        rareCardPackOpenImgs[0].SetActive(true);
        rareCardPackOpenImgs[1].SetActive(false);
        rareOpenButtons[0].enabled = rareOpenButtons[1].enabled = true;
        UpdateRareCardInfo();
    }

    /// <summary>
    /// �ִϸ��̼� ��� ���� ��� �ڷ�ƾ
    /// </summary>
    /// <param name="openingAni">������� �ִϸ��̼�</param>
    /// <param name="afterAnimation">���� ���� ȣ���� �Լ�</param>
    /// <param name="card">�Ķ����(ī�� �ε���)</param>
    IEnumerator WaitOpenAnimation(Animation openingAni, AfterAnimation afterAnimation, int card)
    {
        while (openingAni.isPlaying)
            yield return new WaitForEndOfFrame();

        afterAnimation(card);
    }
    #endregion
}

/// <summary>
/// ī�� ���� ������
/// </summary>
[System.Serializable]
public struct CardAbility
{
    /// <summary>
    /// ī�� �̸�
    /// </summary>
    public string name;
    /// <summary>
    /// ���� �ð�
    /// </summary>
    public int duration;
    /// <summary>
    /// �ɷ�ġ
    /// </summary>
    public int[] ability;
    /// <summary>
    /// ���� Ȯ��
    /// </summary>
    public int probability;
    /// <summary>
    /// ī�� ��������Ʈ �̹��� ���ҽ�
    /// </summary>
    public Sprite cardSprite;
}
