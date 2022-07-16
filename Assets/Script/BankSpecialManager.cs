using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BankSpecialManager : MonoBehaviour
{
    public GameObject Bank_Menu;
    public GameObject Deposit_Menu;
    public GameObject Withdraw_Menu;
    public GameObject register_Menu;
    public GameObject registerCheck_Menu;

    public GameObject[] CheckAccount_Menu = new GameObject[3];

    public static ulong MaxMoney = 18000000000000000000;

    public Text[] SA_text = new Text[3];
    public Text[] SS_text = new Text[3];
    public Text[] SSP_text = new Text[3];

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

    public GameObject notRegisteredBlockImg;

    public MessageManager messageManager;
    public BankManager bankManager;

    public Text timeleft_text;

    public static ulong depositMoney;

    private ulong withdrawMoneyLow;
    private ulong withdrawMoneyHigh;

    public bool[] IsRegistered { get { return bankManager.bankData.isRegisteredSpecial; } set { bankManager.bankData.isRegisteredSpecial = value; } }

    public ulong[] AddedMoneyLow { get { return bankManager.bankData.addedMoneySpecialLow; } set { bankManager.bankData.addedMoneySpecialLow = value; } }
    public ulong[] AddedMoneyHigh { get { return bankManager.bankData.addedMoneySpecialHigh; } set { bankManager.bankData.addedMoneySpecialHigh = value; } }
    public ulong[] SavedMoneyHigh { get { return bankManager.bankData.savedMoneySpecial; } set { bankManager.bankData.savedMoneySpecial = value; } }
    public int[] ContractTime { get { return bankManager.bankData.contractTimesSpecial; } set { bankManager.bankData.contractTimesSpecial = value; } }
    public float[] rate;
    public ulong[] limitDeposit = new ulong[3] { 100 , 10000 , 1000000};
    public int[] standardContractTimes;
    private string[] productNames = { "스페셜A", "스페셜S", "스페셜S+" };
    private bool[] isAlarmed = new bool[3];


    public static ulong totalSavedMoney;
    public static ulong totalMoney;
    public static ulong totalAddedMoney;

    public int Timer { get { return bankManager.bankData.timer; } set { bankManager.bankData.timer = value; } }

    string money1;
    string money2;

    private ulong first_unit;
    private ulong first_up;

    public static bool cal_success;

    public static ulong second_unit;
    public static ulong second_down;

    public static ulong standard_maximum = 10000000000000000;

    public AchievementManager achievementManager;

    void Start()
    {
        StartCoroutine(CalTimer());
        //LoadBank();
    }
    IEnumerator CalTimer()
    {
        yield return new WaitForSeconds(1f);

        Timer--;

        if (Timer <= 0)
        {
            CalculateMoney();
            Timer = 60;
        }


        for (int i = 0; i < ContractTime.Length; ++i)
        {
            if (ContractTime[i] > 0)
            {
                ContractTime[i]--;
            }
            else
            {
                if (!isAlarmed[i] && IsRegistered[i])
                {
                    isAlarmed[i] = true;
                    messageManager.ShowPopupMessage("상품 " + productNames[i] + "가 만기 되어 출금이 가능합니다.");
                }
            }
        }
        Texts();

        timeleft_text.text = "남은 이자 입금 시간: " + Timer + "초";

        StartCoroutine(CalTimer());
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
            case 4:
                CheckAccount_Menu[3].SetActive(true);
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
            case 0://스페셜A
                if (!IsRegistered[0])
                {
                    targetAccount = AccountType.SpecialA;
                    registerCheck_Menu.SetActive(true);
                    register_merchandise_text.text = "상품: 스페셜A";
                    contractTime_text.text = "계약기간: " + (standardContractTimes[0] / 60) + "분";
                    benefit_text.text = string.Format("이율: 분당 {0:0.0}%", rate[0]);
                }
                else
                {
                    AlreadyRegistered();
                }
                break;
            case 1://스페셜S
                if (!IsRegistered[1])
                {
                    targetAccount = AccountType.SpecialS;
                    registerCheck_Menu.SetActive(true);
                    register_merchandise_text.text = "상품: 스페셜S";
                    contractTime_text.text = "계약기간: " + (standardContractTimes[1] / 60) + "분";
                    benefit_text.text = string.Format("이율: 분당 {0:0.0}%", rate[1]);
                }
                else
                {
                    AlreadyRegistered();
                }
                break;
            case 2://스페셜S+
                if (!IsRegistered[2])
                {
                    targetAccount = AccountType.SpecialSPlus;
                    registerCheck_Menu.SetActive(true);
                    register_merchandise_text.text = "상품: 스페셜S+";
                    contractTime_text.text = "계약기간: " + (standardContractTimes[2] / 60) + "분";
                    benefit_text.text = string.Format("이율: 분당 {0:0.0}%", rate[2]);
                }
                else
                {
                    AlreadyRegistered();
                }
                break;
            case 4://Accept
                if (targetAccount.Equals(AccountType.SpecialA))
                {
                    if (MyAsset.instance.MoneyHigh >= 1)
                    {
                        ContractTime[0] = standardContractTimes[0];
                        IsRegistered[0] = true;
                        isAlarmed[0] = false;
                        Alarm_registered();
                        DataManager.instance.SaveData();
                        PressKey_checkAccount(0);
                    }
                    else
                    {
                        messageManager.ShowMessage("보유 금액이 1경$ 이상이여야 가입할 수 있습니다.", 1.0f);
                    }

                }
                else if (targetAccount.Equals(AccountType.SpecialS))
                {
                    if (MyAsset.instance.MoneyHigh >= 100)
                    {
                        ContractTime[1] = standardContractTimes[1];
                        IsRegistered[1] = true;
                        isAlarmed[1] = false;
                        Alarm_registered();
                        DataManager.instance.SaveData();
                        PressKey_checkAccount(2);
                    }
                    else
                    {
                        messageManager.ShowMessage("보유 금액이 100경$ 이상이여야 가입할 수 있습니다.", 1.0f);
                    }

                }
                else if (targetAccount.Equals(AccountType.SpecialSPlus))
                {
                    if (MyAsset.instance.MoneyHigh >= 10000)
                    {
                        ContractTime[2] = standardContractTimes[2];
                        IsRegistered[2] = true;
                        isAlarmed[2] = false;
                        Alarm_registered();
                        DataManager.instance.SaveData();
                        PressKey_checkAccount(4);
                    }
                    else
                    {
                        messageManager.ShowMessage("보유 금액이 1해$ 이상이여야 가입할 수 있습니다.", 1.0f);
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
                    targetAccount = AccountType.SpecialA;
                    Deposit_Menu.SetActive(true);
                    depositMoney = 0;
                    merchandise_text.text = "상품: 스페셜A";
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
                        merchandiseWD_text.text = "상품: 스페셜A";
                        withdrawMoneyLow = (ulong)(AddedMoneyLow[0] * 1.1);
                        withdrawMoneyHigh = SavedMoneyHigh[0] + (ulong)(AddedMoneyHigh[0] * 1.1);
                        ArrangeUnit(withdrawMoneyLow, withdrawMoneyHigh);
                        if (withdrawMoneyHigh > 0)
                        {
                            withdraw_text.text = "출금 할 금액:" + money2 + money1 + "$";
                        }
                        else
                        {
                            withdraw_text.text = "출금 할 금액:" + money1 + "$";
                        }

                        ArrangeUnit((ulong)(AddedMoneyLow[0] * 1.1), (ulong)(AddedMoneyHigh[0] * 1.1));
                        result_text.text = "이윤: " + money2 + money1 + "$";
                    }
                    else
                    {
                        merchandiseWD_text.text = "상품: 스페셜A";
                        withdrawMoneyLow = 0;
                        withdrawMoneyHigh = SavedMoneyHigh[0];
                        ArrangeUnit(withdrawMoneyLow, withdrawMoneyHigh);
                        if (withdrawMoneyHigh > 0)
                        {
                            withdraw_text.text = "출금 할 금액:" + money2 + money1 + "$";
                        }
                        else
                        {
                            withdraw_text.text = "출금 할 금액:" + money1 + "$";
                        }

                        ArrangeUnit(AddedMoneyLow[0], AddedMoneyHigh[0]);
                        if (AddedMoneyHigh[0] > 0)
                        {
                            result_text.text = "사라지는 이윤: " + money2 + money1 + "$";
                        }
                        else
                        {
                            result_text.text = "사라지는 이윤: " + money1 + "$";
                        }
                    }
                }
                else
                    RegisterFirst();

                break;
            case 2:
                if (IsRegistered[1])
                {
                    targetAccount = AccountType.SpecialS;
                    Deposit_Menu.SetActive(true);
                    depositMoney = 0;
                    merchandise_text.text = "상품: 스페셜S";
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
                        merchandiseWD_text.text = "상품: 스페셜S";
                        withdrawMoneyLow = (ulong)(AddedMoneyLow[1] * 1.1);
                        withdrawMoneyHigh = SavedMoneyHigh[1] + (ulong)(AddedMoneyHigh[1] * 1.1);
                        ArrangeUnit(withdrawMoneyLow, withdrawMoneyHigh);
                        if (withdrawMoneyHigh > 0)
                        {
                            withdraw_text.text = "출금 할 금액:" + money2 + money1 + "$";
                        }
                        else
                        {
                            withdraw_text.text = "출금 할 금액:" + money1 + "$";
                        }

                        ArrangeUnit((ulong)(AddedMoneyLow[1] * 1.1), (ulong)(AddedMoneyHigh[1] * 1.1));
                        result_text.text = "이윤: " + money2 + money1 + "$";
                    }
                    else
                    {
                        merchandiseWD_text.text = "상품: 스페셜S";
                        withdrawMoneyLow = 0;
                        withdrawMoneyHigh = SavedMoneyHigh[1];
                        ArrangeUnit(withdrawMoneyLow, withdrawMoneyHigh);
                        if (withdrawMoneyHigh > 0)
                        {
                            withdraw_text.text = "출금 할 금액:" + money2 + money1 + "$";
                        }
                        else
                        {
                            withdraw_text.text = "출금 할 금액:" + money1 + "$";
                        }

                        ArrangeUnit(AddedMoneyLow[1], AddedMoneyHigh[1]);
                        if (AddedMoneyHigh[1] > 0)
                        {
                            result_text.text = "사라지는 이윤: " + money2 + money1 + "$";
                        }
                        else
                        {
                            result_text.text = "사라지는 이윤: " + money1 + "$";
                        }
                    }
                }
                else
                    RegisterFirst();
                break;
            case 4:
                if (IsRegistered[2])
                {
                    targetAccount = AccountType.SpecialSPlus;
                    Deposit_Menu.SetActive(true);
                    depositMoney = 0;
                    merchandise_text.text = "상품: 스페셜S+";
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
                        merchandiseWD_text.text = "상품: 스페셜S+";
                        withdrawMoneyLow = (ulong)(AddedMoneyLow[2] * 1.1);
                        withdrawMoneyHigh = SavedMoneyHigh[2] + (ulong)(AddedMoneyHigh[2] * 1.1);
                        ArrangeUnit(withdrawMoneyLow, withdrawMoneyHigh);
                        if (withdrawMoneyHigh > 0)
                        {
                            withdraw_text.text = "출금 할 금액:" + money2 + money1 + "$";
                        }
                        else
                        {
                            withdraw_text.text = "출금 할 금액:" + money1 + "$";
                        }

                        ArrangeUnit((ulong)(AddedMoneyLow[2] * 1.1), (ulong)(AddedMoneyHigh[2] * 1.1));
                        result_text.text = "이윤: " + money2 + money1 + "$";
                    }
                    else
                    {
                        merchandiseWD_text.text = "상품: 스페셜S+";
                        withdrawMoneyLow = 0;
                        withdrawMoneyHigh = SavedMoneyHigh[2];
                        ArrangeUnit(withdrawMoneyLow, withdrawMoneyHigh);
                        if (withdrawMoneyHigh > 0)
                        {
                            withdraw_text.text = "출금 할 금액:" + money2 + money1 + "$";
                        }
                        else
                        {
                            withdraw_text.text = "출금 할 금액:" + money1 + "$";
                        }

                        ArrangeUnit(AddedMoneyLow[2], AddedMoneyHigh[2]);
                        if (AddedMoneyHigh[2] > 0)
                        {
                            result_text.text = "사라지는 이윤: " + money2 + money1 + "$";
                        }
                        else
                        {
                            result_text.text = "사라지는 이윤: " + money1 + "$";
                        }
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
                depositMoney = MyAsset.instance.MoneyHigh;

                if (depositMoney + SavedMoneyHigh[(int)targetAccount - 3] > limitDeposit[(int)targetAccount - 3])
                {
                    depositMoney = limitDeposit[(int)targetAccount - 3] - SavedMoneyHigh[(int)targetAccount - 3];
                }

                string DM;
                if (depositMoney < 10000)
                {
                    DM = string.Format("{0:###0경}", depositMoney);
                }
                else
                {
                    DM = string.Format("{0:####해####경}", depositMoney);
                }
                depositing_text.text = "입금할 금액: " + DM + "$";
                deposit_inputField.text = depositMoney.ToString();
                break;
        }
    }
    public void DepositMoneyInput()
    {
        depositMoney = ulong.Parse(deposit_inputField.text);
        string ud = "";
        if (depositMoney + SavedMoneyHigh[(int)targetAccount - 3] > limitDeposit[(int)targetAccount - 3])
        {
            depositMoney = limitDeposit[(int)targetAccount - 3] - SavedMoneyHigh[(int)targetAccount - 3];
            if (depositMoney < 10000)
            {
                ud = string.Format("{0:###0경}", depositMoney);
            }
            else
            {
                ud = string.Format("{0:####해####경}", depositMoney);
            }
            messageManager.ShowMessage("희망 예금 금액이 예금 제한금액보다 높아 " + ud + "$으로 수정되었습니다.", 1.5f);
            deposit_inputField.text = depositMoney.ToString();
        }
        if (depositMoney > MyAsset.instance.MoneyHigh)
        {
            depositMoney = MyAsset.instance.MoneyHigh;
            deposit_inputField.text = depositMoney.ToString();
        }

        if (ud.Equals(""))
        {
            if (depositMoney < 10000)
            {
                ud = string.Format("{0:###0경}", depositMoney);
            }
            else
            {
                ud = string.Format("{0:####해####경}", depositMoney);
            }
        }
        depositing_text.text = "입금할 금액: " + ud + "$";
    }

    public ulong QuickWithdraw(int i, ref ulong lowReturn, ref ulong highReturn)
    {
        if (IsRegistered[i])
        {
            withdrawMoneyLow = (ulong)(AddedMoneyLow[i] * 1.1);
            withdrawMoneyHigh = SavedMoneyHigh[i] + (ulong)(AddedMoneyHigh[i] * 1.1);
            lowReturn = withdrawMoneyLow;
            highReturn = withdrawMoneyHigh;

            AssetMoneyCalculator.instance.ArithmeticOperation(withdrawMoneyLow, withdrawMoneyHigh, true);
            SavedMoneyHigh[i] = 0;
            AddedMoneyHigh[i] = 0;
            AddedMoneyLow[i] = 0;
            ContractTime[i] = 0;
            IsRegistered[i] = false;
            return withdrawMoneyLow;
        }
        else
            return 0;
    }

    public void PressKey_withdraw(int nKey)
    {
        switch (nKey)
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
        PlayManager.ArrangeUnit(withdrawMoneyLow, withdrawMoneyHigh, ref money1, ref money2, true);
        messageManager.ShowMessage("<color=green>" + money2 + money1 + "$</color>가 정상적으로 출금 되었습니다." , 3f);
    }
    private void PrintDepositMessage()
    {
        string money1 = "", money2 = "";
        PlayManager.ArrangeUnit(0, depositMoney, ref money1, ref money2, true);
        messageManager.ShowMessage("<color=orange>" + money2 + "$</color>가 정상적으로 입금 되었습니다.", 3f);
    }
    void Cal_Merchandise()
    {
        if (targetAccount.Equals(AccountType.SpecialA))
        {
            Cal_deposit(0);
        }
        else if (targetAccount.Equals(AccountType.SpecialS))
        {
            Cal_deposit(1);
        }
        else if (targetAccount.Equals(AccountType.SpecialSPlus))
        {
            Cal_deposit(2);
        }
    }
    void Cal_deposit(int i)
    {
        if (AssetMoneyCalculator.instance.ArithmeticOperation(0, depositMoney, false))
        {
            SavedMoneyHigh[i] += depositMoney;
        }
        else
        {
            messageManager.ShowMessage("입금액이 보유금액 보다 커 입금이 진행되지 않았습니다.\n모든 금액을 입금하고 싶으시면 '전체' 버튼을 이용해 주시길 바랍니다.",1.5f);
        }
    }

    void Cal_Merchandise_withdraw()
    {
        LargeVariable interest = LargeVariable.zero;
        if (merchandiseWD_text.text == "상품: 스페셜A")
        {
            AssetMoneyCalculator.instance.ArithmeticOperation(withdrawMoneyLow, withdrawMoneyHigh, true);
            interest = new LargeVariable(AddedMoneyLow[0], AddedMoneyHigh[0]);
            AddedMoneyLow[0] = 0;
            SavedMoneyHigh[0] = 0;
            AddedMoneyHigh[0] = 0;
            ContractTime[0] = 0;
            IsRegistered[0] = false;
        }
        else if (merchandiseWD_text.text == "상품: 스페셜S")
        {
            AssetMoneyCalculator.instance.ArithmeticOperation(withdrawMoneyLow, withdrawMoneyHigh, true);
            interest = new LargeVariable(AddedMoneyLow[1], AddedMoneyHigh[1]);
            AddedMoneyLow[1] = 0;
            SavedMoneyHigh[1] = 0;
            AddedMoneyHigh[1] = 0;
            ContractTime[1] = 0;
            IsRegistered[1] = false;
        }
        else if (merchandiseWD_text.text == "상품: 스페셜S+")
        {
            AssetMoneyCalculator.instance.ArithmeticOperation(withdrawMoneyLow, withdrawMoneyHigh, true);
            interest = new LargeVariable(AddedMoneyLow[2], AddedMoneyHigh[2]);
            AddedMoneyLow[2] = 0;
            SavedMoneyHigh[2] = 0;
            AddedMoneyHigh[2] = 0;
            ContractTime[2] = 0;
            IsRegistered[2] = false;
        }
        achievementManager.cumulativeInterest += interest * 1.1f;
    }
    private void CalculateMoney()
    {
        for (int i = 0; i < SavedMoneyHigh.Length; ++i)
        {
            if(ContractTime[i] > 0)
            {
                Percent_Cal_Money(rate[i], 0, SavedMoneyHigh[i]);
                AddedMoneyLow[i] += first_unit;
                AddedMoneyHigh[i] += second_unit;
                Synchronization_Money(AddedMoneyLow[i], AddedMoneyHigh[i]);
                AddedMoneyLow[i] = first_unit;
                AddedMoneyHigh[i] = second_unit;
            }
            if (AddedMoneyHigh[i] > MaxMoney)
            {
                AddedMoneyHigh[i] = MaxMoney;
            }
        }
    }

    void Texts()
    {
        if (SavedMoneyHigh[0] + AddedMoneyLow[0] + AddedMoneyHigh[0] == 0)
        {
            SA_text[0].text = "총 잔액: 0$";
        }
        else
        {
            ArrangeUnit(AddedMoneyLow[0],SavedMoneyHigh[0]+AddedMoneyHigh[0]);
            SA_text[0].text = "총 잔액: " + money2 + money1 + "$";
        }
        if (AddedMoneyLow[0] == 0 && AddedMoneyHigh[0] == 0)
        {
            SA_text[1].text = "이윤: 0$";
        }
        else
        {
            ArrangeUnit(AddedMoneyLow[0], AddedMoneyHigh[0]);
            if (AddedMoneyHigh[0] > 0)
            {
                SA_text[1].text = "이윤: " + money2 + money1 + "$";
            }
            else
            {
                SA_text[1].text = "이윤: " + money1 + "$";
            }
        }
        //-----------------------------------------------------------------------------------------------------------
        if (SavedMoneyHigh[1] + AddedMoneyLow[1] + AddedMoneyHigh[1] == 0)
        {
            SS_text[0].text = "총 잔액: 0$";
        }
        else
        {
            ArrangeUnit(AddedMoneyLow[1], SavedMoneyHigh[1] + AddedMoneyHigh[1]);
            SS_text[0].text = "총 잔액: " + money2 + money1 + "$";
        }
        if (AddedMoneyLow[1] == 0 && AddedMoneyHigh[1] == 0)
        {
            SS_text[1].text = "이윤: 0$";
        }
        else
        {
            ArrangeUnit(AddedMoneyLow[1], AddedMoneyHigh[1]);
            if (AddedMoneyHigh[1] > 0)
            {
                SS_text[1].text = "이윤: " + money2 + money1 + "$";
            }
            else
            {
                SS_text[1].text = "이윤: " + money1 + "$";
            }
        }
        //-----------------------------------------------------------------------------------------------------------
        if (SavedMoneyHigh[2] + AddedMoneyLow[2] + AddedMoneyHigh[2] == 0)
        {
            SSP_text[0].text = "총 잔액: 0$";
        }
        else
        {
            ArrangeUnit(AddedMoneyLow[2], SavedMoneyHigh[2] + AddedMoneyHigh[2]);
            SSP_text[0].text = "총 잔액: " + money2 + money1 + "$";
        }
        if (AddedMoneyLow[2] == 0 && AddedMoneyHigh[2] == 0)
        {
            SSP_text[1].text = "이윤: 0$";
        }
        else
        {
            ArrangeUnit(AddedMoneyLow[2], AddedMoneyHigh[2]);
            if (AddedMoneyHigh[2] > 0)
            {
                SSP_text[1].text = "이윤: " + money2 + money1 + "$";
            }
            else
            {
                SSP_text[1].text = "이윤: " + money1 + "$";
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        SA_text[2].text = (int)(ContractTime[0] / 60) + "분" + ContractTime[0] % 60 + "초";
        SS_text[2].text = (int)(ContractTime[1] / 60) + "분" + ContractTime[1] % 60 + "초";
        SSP_text[2].text = (int)(ContractTime[2] / 60) + "분" + ContractTime[2] % 60 + "초";
    }
    void AlreadyRegistered()
    {
        messageManager.ShowMessage("이미 가입된 상품입니다.", 1.0f);
    }
    void Alarm_registered()
    {
        messageManager.ShowMessage("가입이 완료 되었습니다.", 1.0f);
    }
    void RegisterFirst()
    {
        messageManager.ShowMessage("해당 상품 가입을 먼저 해주세요.", 1.0f);
    }
    void FullMoney()
    {
        messageManager.ShowMessage("입금 가능한 최대 금액입니다.", 1.0f);
    }
    /*
    public static void SaveBank()
    {
        string[] SM = new string[3];
        string[] AM = new string[3];
        string[] AM2 = new string[3];
        for (int i = 0; i < SavedMoney_2.Length; ++i)
        {
            SM[i] = "" + SavedMoney_2[i];
            AM[i] = "" + AddedMoney_1[i];
            AM2[i] = "" + AddedMoney_2[i];

            PlayerPrefs.SetString("S_SavedMoney[" + i + "]", SM[i]);
            PlayerPrefs.SetString("S_AddedMoney[" + i + "]", AM[i]);
            PlayerPrefs.SetString("S_AddedMoney2["+i+"]",AM2[i]);

            PlayerPrefs.SetInt("S_contract_time[" + i + "]", contract_time[i]);
            PlayerPrefs.SetInt("S_registered[" + i + "]", isRegistered[i]);

            PlayerPrefs.SetInt("S_Timeleft",i_nextTime);
        }
    }
    public static void LoadBank()
    {
        string[] SM = new string[3];
        string[] AM = new string[3];
        string[] AM2 = new string[3];
        for (int i = 0; i < SavedMoney_2.Length; ++i)
        {
            SM[i] = PlayerPrefs.GetString("S_SavedMoney[" + i + "]");
            AM[i] = PlayerPrefs.GetString("S_AddedMoney[" + i + "]");
            AM2[i] = PlayerPrefs.GetString("S_AddedMoney2["+i+"]");

            if (SM[i] != "")
            {
                SavedMoney_2[i] = ulong.Parse(SM[i]);
                AddedMoney_1[i] = ulong.Parse(AM[i]);
                AddedMoney_2[i] = ulong.Parse(AM2[i]);
            }
            contract_time[i] = PlayerPrefs.GetInt("S_contract_time[" + i + "]");
            isRegistered[i] = PlayerPrefs.GetInt("S_registered[" + i + "]");

            i_nextTime = PlayerPrefs.GetInt("S_Timeleft");
        }
    }
    public static void ResetBank()
    {
        for (int i = 0; i < SavedMoney_2.Length; ++i)
        {
            SavedMoney_2[i] = 0;
            AddedMoney_1[i] = 0;
            AddedMoney_2[i] = 0;

            contract_time[i] = 0;
            isRegistered[i] = 0;

            i_nextTime = 60;
        }
    }*/
    void ArrangeUnit(ulong insertvar, ulong insertvar2)
    {
        if (insertvar == 0)
        {
            money1 = "";
        }
        else if (insertvar < 100000000)
        {
            money1 = string.Format("{0:####만####}", insertvar);
        }
        else if (insertvar < 1000000000000)
        {
            ulong downmoney = (ulong)(insertvar * 0.00000001);
            money1 = string.Format("{0:####억####만}", downmoney);
        }
        else
        {
            ulong downmoney = (ulong)(insertvar * 0.000000000001);
            money1 = string.Format("{0:####조}", downmoney);
        }
        if (insertvar2 < 10000)
        {
            money2 = string.Format("{0:####경}", insertvar2);
        }
        else
        {
            money2 = string.Format("{0:####해####경}", insertvar2);
        }

    }
    private void Percent_Cal_Money(float percentage , ulong fu , ulong su)
    {
        MoneyUnitTranslator.Multiply(ref fu, ref su, percentage / 100f);
        first_unit = fu;
        second_unit = su;
    }
    private void Synchronization_Money(ulong fu, ulong su)
    {
        first_unit = fu;
        second_unit = su;
        if (first_unit >= standard_maximum)
        {
            first_up = (ulong)(first_unit / standard_maximum);
            first_unit -= first_up * standard_maximum;
            second_unit += first_up;
        }
    }
}
