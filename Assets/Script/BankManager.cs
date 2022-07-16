using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum AccountType { A, B, C, SpecialA, SpecialS, SpecialSPlus };
public class BankManager : MonoBehaviour {

    public GameObject Bank_Menu;
    public GameObject Deposit_Menu;
    public GameObject Withdraw_Menu;
    public GameObject register_Menu;
    public GameObject registerCheck_Menu;

    public GameObject[] CheckAccount_Menu = new GameObject[4];

    public static ulong MaxMoney = 18000000000000000000;

    public Text[] A_text = new Text[3];
    public Text[] B_text = new Text[3];
    public Text[] C_text = new Text[3];

    public Text depositing_text;
    public Text merchandise_text;
    public Text withdraw_text;
    public Text merchandiseWD_text;
    public Text result_text;

    public Text register_merchandise_text;
    public Text contractTime_text;
    public Text benefit_text;

    private AccountType? targetAccount;
    public InputField deposit_inputField;

    public MessageManager messageManager;

    public GameObject notRegisteredBlockImg;

    private ulong depositMoney;
    private ulong withdrawMoney;

    public BankData bankData;

    public bool[] IsRegistered { get { return bankData.isRegisteredNormal; } set { bankData.isRegisteredNormal = value; } }
    public ulong[] SavedMoney { get { return bankData.savedMoneyNormal; } set { bankData.savedMoneyNormal = value; } }
    public ulong[] AddedMoney { get { return bankData.addedMoneyNormal; } set { bankData.addedMoneyNormal = value; } }
    public int[] ContractTime { get { return bankData.contractTimesNormal; } set { bankData.contractTimesNormal = value; } }
    public float[] rate = new float[3] { 0.001f, 0.0006f, 0.0005f};
    public ulong[] limitDeposit = new ulong[3] { 100000000,100000000000,100000000000000};
    private string[] productNames = { "A", "B", "C" };
    private bool[] isAlarmed = new bool[3];


    private ulong totalSavedMoney;
    private ulong totalMoney;
    private ulong totalAddedMoney;

    public GameObject bankProductMenu;
    public GameObject lotteryTicketMenu;

    public AchievementManager achievementManager;

    void Start()
    {
        StartCoroutine(CalTimer());
        //LoadBank();
    }
    IEnumerator CalTimer()
    {
        yield return new WaitForSeconds(1);

        CalculateMoney();
        for (int i = 0; i < ContractTime.Length; ++i)
        {
            if (ContractTime[i] > 0)
            {
                ContractTime[i]--;
            }
            else
            {
                if(!isAlarmed[i] && IsRegistered[i])
                {
                    isAlarmed[i] = true;
                    messageManager.ShowPopupMessage("상품 " + productNames[i] + "가 만기 되어 출금이 가능합니다.");
                }
            }
        }
        Texts();

        StartCoroutine(CalTimer());
    }

    public void SetBankProductMenu(bool active)
    {
        bankProductMenu.SetActive(active);
    }
    public void SetLotteryTicketMenu(bool active)
    {
        lotteryTicketMenu.SetActive(active);
    }

    public void PressKey(int nKey)
    {
        switch (nKey)
        {
            case 0://close bank
                Bank_Menu.SetActive(false);
                break;
            case 1:
                CheckAccount_Menu[0].SetActive(true);
                if (!IsRegistered[nKey - 1])
                    notRegisteredBlockImg.SetActive(true);
                break;
            case 2:
                CheckAccount_Menu[1].SetActive(true);
                if (!IsRegistered[nKey - 1])
                    notRegisteredBlockImg.SetActive(true);
                break;
            case 3:
                CheckAccount_Menu[2].SetActive(true);
                if (!IsRegistered[nKey - 1])
                    notRegisteredBlockImg.SetActive(true);
                break;
            case 5:
                register_Menu.SetActive(true);
                break;
        }
    }
    public void PressKey_Register(int nKey)
    {
        switch (nKey)
        {
            case 0://A
                if (!IsRegistered[0])
                {
                    registerCheck_Menu.SetActive(true);
                    register_merchandise_text.text = "상품: A";
                    contractTime_text.text = "계약기간: 10분";
                    benefit_text.text = "이율: 분당 6.0%";
                    targetAccount = AccountType.A;
                }
                else
                {
                    AlreadyRegistered();
                }
                break;
            case 1://B
                if (!IsRegistered[1])
                {
                    registerCheck_Menu.SetActive(true);
                    register_merchandise_text.text = "상품: B";
                    contractTime_text.text = "계약기간: 20분";
                    benefit_text.text = "이율: 분당 3.6%";
                    targetAccount = AccountType.B;
                }
                else
                {
                    AlreadyRegistered();
                }
                break;
            case 2://C
                if (!IsRegistered[2])
                {
                    registerCheck_Menu.SetActive(true);
                    register_merchandise_text.text = "상품: C";
                    contractTime_text.text = "계약기간: 30분";
                    benefit_text.text = "이율: 분당 3.0%";
                    targetAccount = AccountType.C;
                }
                else
                {
                    AlreadyRegistered();
                }
                break;
                /*
            case 3://D
                if (isRegistered[3] == 0)
                {
                    registerCheck_Menu.SetActive(true);
                    register_merchandise_text.text = "상품: D";
                    contractTime_text.text = "계약기간: 30개월(60분)";
                    benefit_text.text = "이율: 초당 0.1%";
                }
                else
                {
                    AlreadyRegistered();
                }
                break;*/
            case 4://Accept
                if (targetAccount.Equals(AccountType.A))
                {
                    if (MyAsset.instance.MoneyLow >= 100000 || MyAsset.instance.MoneyHigh > 0)
                    {
                        ContractTime[0] = 600;
                        IsRegistered[0] = true;
                        isAlarmed[0] = false;
                        Alarm_registered();
                        DataManager.instance.SaveData();
                        PressKey_checkAccount(0);
                    }
                    else
                    {
                        messageManager.ShowMessage("보유 금액이 10만$ 이상이여야 가입할 수 있습니다.");
                    }

                }
                else if (targetAccount.Equals(AccountType.B))
                {
                    if (MyAsset.instance.MoneyLow >= 100000000 || MyAsset.instance.MoneyHigh > 0)
                    {
                        ContractTime[1] = 1200;
                        IsRegistered[1] = true;
                        isAlarmed[1] = false;
                        Alarm_registered();
                        DataManager.instance.SaveData();
                        PressKey_checkAccount(2);
                    }
                    else
                    {
                        messageManager.ShowMessage("보유 금액이 1억$ 이상이여야 가입할 수 있습니다.");
                    }

                }
                else if (targetAccount.Equals(AccountType.C))
                {
                    if (MyAsset.instance.MoneyLow >= 100000000000 || MyAsset.instance.MoneyHigh > 0)
                    {
                        ContractTime[2] = 1800;
                        IsRegistered[2] = true;
                        isAlarmed[2] = false;
                        Alarm_registered();
                        DataManager.instance.SaveData();
                        PressKey_checkAccount(4);
                    }
                    else
                    {
                        messageManager.ShowMessage("보유 금액이 1000억$ 이상이여야 가입할 수 있습니다.");
                    }

                }
                registerCheck_Menu.SetActive(false);
                break;
            case 5://Cancel
                targetAccount = null;
                registerCheck_Menu.SetActive(false);
                break;
            case 6://back
                register_Menu.SetActive(false);
                break;

        }
    }
    public void PressKey_checkAccount(int nKey)
    {
        switch (nKey)
        {
            case 0://입금
                if (IsRegistered[0])
                {
                    targetAccount = AccountType.A;
                    Deposit_Menu.SetActive(true);
                    depositMoney = 0;
                    merchandise_text.text = "상품: A";
                }
                else
                {
                    RegisterFirst();
                }
                break;
            case 1://출금
                if (IsRegistered[0])
                {
                    Withdraw_Menu.SetActive(true);
                    if (ContractTime[0] <= 0)
                    {
                        merchandiseWD_text.text = "상품: A";
                        withdrawMoney = SavedMoney[0] + (ulong)(AddedMoney[0] * 1.1);
                        string WM = string.Format("{0:#,###}", withdrawMoney);
                        withdraw_text.text = "출금 할 금액:" + WM + "$";
                        string IM = string.Format("{0:#,###}", (ulong)(AddedMoney[0] * 1.1));
                        result_text.text = "이윤: " + IM + "원";
                    }
                    else
                    {
                        merchandiseWD_text.text = "상품: A";
                        //U_withdrawMoney = (ulong)(AddedMoney[0] * 0.3) + SavedMoney[0];
                        withdrawMoney = SavedMoney[0];
                        string WM = string.Format("{0:#,###}", withdrawMoney);
                        withdraw_text.text = "출금 할 금액: " + WM + "$";
                        string DM = string.Format("{0:#,###}", AddedMoney[0]);
                        result_text.text = "사라지는 이윤: " + DM + "$";
                    }
                }
                else
                    RegisterFirst();
                break;
            case 2:
                if (IsRegistered[1])
                {
                    targetAccount = AccountType.B;
                    Deposit_Menu.SetActive(true);
                    depositMoney = 0;
                    merchandise_text.text = "상품: B";
                }
                else
                {
                    RegisterFirst();
                }
                break;
            case 3:
                if (IsRegistered[1])
                {
                    Withdraw_Menu.SetActive(true);
                    if (ContractTime[1] <= 0)
                    {
                        merchandiseWD_text.text = "상품: B";
                        withdrawMoney = SavedMoney[1] + (ulong)(AddedMoney[1] * 1.1);
                        string WM = string.Format("{0:#,###}", withdrawMoney);
                        withdraw_text.text = "출금 할 금액:" + WM + "$";
                        string IM = string.Format("{0:#,###}", (ulong)(AddedMoney[1] * 1.1));
                        result_text.text = "이윤: " + IM + "$";
                    }
                    else
                    {
                        merchandiseWD_text.text = "상품: B";
                        withdrawMoney = SavedMoney[1];
                        string WM = string.Format("{0:#,###}", withdrawMoney);
                        withdraw_text.text = "출금 할 금액: " + WM + "$";
                        string DM = string.Format("{0:#,###}", AddedMoney[1]);
                        result_text.text = "사라지는 이윤: " + DM + "$";
                    }
                }
                else
                    RegisterFirst();
                break;
            case 4:
                if (IsRegistered[2])
                {
                    targetAccount = AccountType.C;
                    Deposit_Menu.SetActive(true);
                    depositMoney = 0;
                    merchandise_text.text = "상품: C";
                }
                else
                {
                    RegisterFirst();
                }
                break;
            case 5:
                if (IsRegistered[2])
                {
                    Withdraw_Menu.SetActive(true);
                    if (ContractTime[2] <= 0)
                    {
                        merchandiseWD_text.text = "상품: C";
                        withdrawMoney = SavedMoney[2] + (ulong)(AddedMoney[2] * 1.1);
                        string WM = string.Format("{0:#,###}", withdrawMoney);
                        withdraw_text.text = "출금 할 금액:" + WM + "$";
                        string IM = string.Format("{0:#,###}", +(ulong)(AddedMoney[2] * 1.1));
                        result_text.text = "이윤: " + IM + "$";
                    }
                    else
                    {
                        merchandiseWD_text.text = "상품: C";
                        withdrawMoney = SavedMoney[2];
                        string WM = string.Format("{0:#,###}", withdrawMoney);
                        withdraw_text.text = "출금 할 금액: " + WM + "$";
                        string DM = string.Format("{0:#,###}", AddedMoney[2]);
                        result_text.text = "사라지는 이윤: " + DM + "$";
                    }
                }
                else
                    RegisterFirst();
                break;
            case 8:
                CheckAccount_Menu[0].SetActive(false);
                notRegisteredBlockImg.SetActive(false);
                break;
            case 9:
                CheckAccount_Menu[1].SetActive(false);
                notRegisteredBlockImg.SetActive(false);
                break;
            case 10:
                CheckAccount_Menu[2].SetActive(false);
                notRegisteredBlockImg.SetActive(false);
                break;
            case 11:
                CheckAccount_Menu[3].SetActive(false);
                notRegisteredBlockImg.SetActive(false);
                break;
        }
    }
    public void PressKey_Addmoney(int nKey)
    {
        switch (nKey)
        {
            case 10://Accept
                Cal_Merchandise();
                Deposit_Menu.SetActive(false);
                PrintDepositMessage();
                depositMoney = 0;
                targetAccount = null;
                deposit_inputField.text = "";
                depositing_text.text = "입금할 금액: ";
                DataManager.instance.SaveData();
                break;
            case 11://Cancel
                Deposit_Menu.SetActive(false);
                depositMoney = 0;
                targetAccount = null;
                deposit_inputField.text = "";
                depositing_text.text = "입금할 금액: ";
                break;
            case 13:
                depositMoney = MyAsset.instance.MoneyLow;

                if (depositMoney + SavedMoney[(int)targetAccount] > limitDeposit[(int)targetAccount])
                {
                    depositMoney = limitDeposit[(int)targetAccount] - SavedMoney[(int)targetAccount];
                }
                string DM = string.Format("{0:#,##0}", depositMoney);
                depositing_text.text = "입금할 금액: " + DM + "$";
                deposit_inputField.text = depositMoney.ToString();
                break;
        }
    }

    public void DepositMoneyInput()
    {
        depositMoney = ulong.Parse(deposit_inputField.text);
        if(depositMoney + SavedMoney[(int)targetAccount] > limitDeposit[(int)targetAccount])
        {
            depositMoney = limitDeposit[(int)targetAccount] - SavedMoney[(int)targetAccount];
            string ud = string.Format("{0:#,##0}", depositMoney);
            messageManager.ShowMessage("희망 예금 금액이 예금 제한금액보다 높아 " + ud + "$으로 수정되었습니다.", 1.5f);
            deposit_inputField.text = depositMoney.ToString();
        }
        if (depositMoney > MyAsset.instance.MoneyLow)
        {
            depositMoney = MyAsset.instance.MoneyLow;
            deposit_inputField.text = depositMoney.ToString();
        }

        string DM = string.Format("{0:#,##0}", depositMoney);
        depositing_text.text = "입금할 금액: " + DM + "$";
    }

    public ulong QuickWithdraw(int i)
    {
        if (IsRegistered[i])
        {
            withdrawMoney = SavedMoney[i] + (ulong)(AddedMoney[i] * 1.1f);

            AssetMoneyCalculator.instance.ArithmeticOperation(withdrawMoney, 0, true);
            SavedMoney[i] = 0;
            AddedMoney[i] = 0;
            ContractTime[i] = 0;
            IsRegistered[i] = false;
            return withdrawMoney;
        }
        else
            return 0;
    }

    public void PressKey_withdraw(int nKey)
    { 
        switch(nKey)
        {
            case 0://accept
                Cal_Merchandise_withdraw();
                Withdraw_Menu.SetActive(false);
                PrintWithdrawMessage();
                DataManager.instance.SaveData();
                break;
            case 1:
                Withdraw_Menu.SetActive(false);
                break;
        }
    }

    private void PrintWithdrawMessage()
    {
        string money1 = "", money2 = "";
        PlayManager.ArrangeUnit(withdrawMoney, 0, ref money1, ref money2, true);
        messageManager.ShowMessage("<color=green>" + money1 + "$</color>가 정상적으로 출금 되었습니다.", 3f);
    }
    private void PrintDepositMessage()
    {
        string money1 = "", money2 = "";
        PlayManager.ArrangeUnit(depositMoney, 0, ref money1, ref money2, true);
        messageManager.ShowMessage("<color=orange>" + money2 + money1 + "$</color>가 정상적으로 입금 되었습니다.", 3f);
    }

    void Cal_Merchandise()
    {
        if (targetAccount.Equals(AccountType.A))
        {
            Cal_deposit(0);
        }
        else if (targetAccount.Equals(AccountType.B))
        {
            Cal_deposit(1);
        }
        else if (targetAccount.Equals(AccountType.C))
        {
            Cal_deposit(2);
        }
    }
    void Cal_deposit(int i)
    {
        if (AssetMoneyCalculator.instance.ArithmeticOperation(depositMoney, 0, false))
        {
            SavedMoney[i] += depositMoney;
        }
        else
        {
            messageManager.ShowMessage("입금액이 보유금액 보다 커 입금이 진행되지 않았습니다.\n모든 금액을 입금하고 싶으시면 '전체' 버튼을 이용해 주시길 바랍니다.", 1.5f);
        }
    }

    void Cal_Merchandise_withdraw()
    {
        LargeVariable interest = LargeVariable.zero;
        if (merchandiseWD_text.text == "상품: A")
        {
            AssetMoneyCalculator.instance.ArithmeticOperation(withdrawMoney, 0, true);
            interest = new LargeVariable(AddedMoney[0], 0);
            
            SavedMoney[0] = 0;
            AddedMoney[0] = 0;
            ContractTime[0] = 0;
            IsRegistered[0] = false;
        }
        else if (merchandiseWD_text.text == "상품: B")
        {
            AssetMoneyCalculator.instance.ArithmeticOperation(withdrawMoney, 0, true);
            interest = new LargeVariable(AddedMoney[1], 0);

            SavedMoney[1] = 0;
            AddedMoney[1] = 0;
            ContractTime[1] = 0;
            IsRegistered[1] = false;
        }
        else if (merchandiseWD_text.text == "상품: C")
        {
            AssetMoneyCalculator.instance.ArithmeticOperation(withdrawMoney, 0, true);
            interest = new LargeVariable(AddedMoney[2], 0);

            SavedMoney[2] = 0;
            AddedMoney[2] = 0;
            ContractTime[2] = 0;
            IsRegistered[2] = false;
        }
        achievementManager.cumulativeInterest += interest * 1.1f;
    }
    private void CalculateMoney()
    {
        for (int i = 0; i < SavedMoney.Length; ++i)
        {
            if (ContractTime[i] > 0)
            {
                AddedMoney[i] += (ulong)(SavedMoney[i] * rate[i]);
                if (AddedMoney[i] > MaxMoney)
                {
                    AddedMoney[i] = MaxMoney;
                }
            }
        }
        totalAddedMoney = AddedMoney[0] + AddedMoney[1] + AddedMoney[2]/* + AddedMoney[3]*/;
        totalSavedMoney = SavedMoney[0] + SavedMoney[1] + SavedMoney[2]/* + SavedMoney[3]*/;
        totalMoney = totalAddedMoney + totalSavedMoney;
        if (totalAddedMoney > MaxMoney)
        {
            totalAddedMoney = MaxMoney;
        }
        if (totalSavedMoney > MaxMoney)
        {
            totalSavedMoney = MaxMoney;
        }
        if (totalMoney > MaxMoney)
        {
            totalMoney = MaxMoney;
        }
    }

    void Texts()
    {
        if (SavedMoney[0] + AddedMoney[0] == 0)
        {
            A_text[0].text = "총 잔액: 0$";
        }
        else
        {
            string TM_A = string.Format("{0:#,###}", SavedMoney[0] + AddedMoney[0]);
            A_text[0].text = "총 잔액: " + TM_A + "$";
        }
        if (AddedMoney[0] == 0)
        {
            A_text[1].text = "이윤: 0$";
        }
        else
        {
            string AM_A = string.Format("{0:#,###}", AddedMoney[0]);
            A_text[1].text = "이윤: " + AM_A + "$";
        }
        //-----------------------------------------------------------------------------------------------------------
        if (SavedMoney[1] + AddedMoney[1] == 0)
        {
            B_text[0].text = "총 잔액: 0$";
        }
        else
        {
            string TM_A = string.Format("{0:#,###}", SavedMoney[1] + AddedMoney[1]);
            B_text[0].text = "총 잔액: " + TM_A + "$";
        }
        if (AddedMoney[1] == 0)
        {
            B_text[1].text = "이윤: 0$";
        }
        else
        {
            string AM_A = string.Format("{0:#,###}", AddedMoney[1]);
            B_text[1].text = "이윤: " + AM_A + "$";
        }
        //-----------------------------------------------------------------------------------------------------------
        if (SavedMoney[2] + AddedMoney[2] == 0)
        {
            C_text[0].text = "총 잔액: 0$";
        }
        else
        {
            string TM_A = string.Format("{0:#,###}", SavedMoney[2] + AddedMoney[2]);
            C_text[0].text = "총 잔액: " + TM_A + "$";
        }
        if (AddedMoney[2] == 0)
        {
            C_text[1].text = "이윤: 0$";
        }
        else
        {
            string AM_A = string.Format("{0:#,###}", AddedMoney[2]);
            C_text[1].text = "이윤: " + AM_A + "$";
        }
        //-----------------------------------------------------------------------------------------------------------
        /*
        if (SavedMoney[3] + AddedMoney[3] == 0)
        {
            D_text[0].text = "총 잔액: 0$";
        }
        else
        {
            string TM_A = string.Format("{0:#,###}", SavedMoney[3] + AddedMoney[3]);
            D_text[0].text = "총 잔액: " + TM_A + "$";
        }
        if (AddedMoney[3] == 0)
        {
            D_text[1].text = "이윤: 0$";
        }
        else
        {
            string AM_A = string.Format("{0:#,###}", AddedMoney[3]);
            D_text[1].text = "이윤: " + AM_A + "$";
        }*/
        //-----------------------------------------------------------------------------------------------------------
        A_text[2].text = (int)(ContractTime[0] / 60) + "분" + ContractTime[0]%60 + "초";
        B_text[2].text = (int)(ContractTime[1] / 60) + "분" + ContractTime[1]%60 + "초";
        C_text[2].text = (int)(ContractTime[2] / 60) + "분" + ContractTime[2]%60 + "초";
        //D_text[2].text = (int)(contract_time[3] / 60) + "분" + contract_time[3]%60 + "초";
    }
    void AlreadyRegistered()
    {
        messageManager.ShowMessage("이미 가입된 상품입니다.");
    }
    void Alarm_registered()
    {
        messageManager.ShowMessage("가입이 완료 되었습니다.");
    }
    void RegisterFirst()
    {
        messageManager.ShowMessage("해당 상품 가입을 먼저 해주세요.");
    }
    void FullMoney()
    {
        messageManager.ShowMessage("입금 가능한 최대 금액입니다.");
    }
    /*
    public static void SaveBank()
    {
        string[] SM = new string[3];
        string[] AM = new string[3];
        for (int i = 0; i < SavedMoney.Length; ++i)
        {
            SM[i] = "" + SavedMoney[i];
            AM[i] = "" + AddedMoney[i];

            PlayerPrefs.SetString("SavedMoney["+i+"]",SM[i]);
            PlayerPrefs.SetString("AddedMoney["+i+"]", AM[i]);

            PlayerPrefs.SetInt("contract_time["+i + "]", contract_time[i]);
            PlayerPrefs.SetInt("registered["+i+"]",isRegistered[i]);
        }
    }
    public static void LoadBank()
    {
        string[] SM = new string[3];
        string[] AM = new string[3];
        for (int i = 0; i < SavedMoney.Length; ++i)
        {
            SM[i] = PlayerPrefs.GetString("SavedMoney["+i+"]");
            AM[i] = PlayerPrefs.GetString("AddedMoney["+i+"]");

            if (SM[i] != "")
            {
                SavedMoney[i] = ulong.Parse(SM[i]);
                AddedMoney[i] = ulong.Parse(AM[i]);
            }
            contract_time[i] = PlayerPrefs.GetInt("contract_time["+i+"]");
            isRegistered[i] = PlayerPrefs.GetInt("registered["+i+"]");
        }
    }
    public static void ResetBank()
    {
        for (int i = 0; i < SavedMoney.Length; ++i)
        {
            SavedMoney[i] = 0;
            AddedMoney[i] = 0;

            contract_time[i] = 0;
            isRegistered[i] = 0;
        }
    }*/
}
