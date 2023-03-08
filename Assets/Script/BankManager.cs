using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 계좌 종류
/// </summary>
public enum AccountType { A, B, C, SpecialA, SpecialS, SpecialSPlus };
/// <summary>
/// 은행 시스템(일반 상품) 관리 클래스
/// </summary>
public class BankManager : MonoBehaviour {
    /// <summary>
    /// 은행 메뉴 오브젝트
    /// </summary>
    public GameObject Bank_Menu;
    /// <summary>
    /// 예금 메뉴 오브젝트
    /// </summary>
    public GameObject Deposit_Menu;
    /// <summary>
    /// 출금 메뉴 오브젝트
    /// </summary>
    public GameObject Withdraw_Menu;
    /// <summary>
    /// 가입 메뉴 오브젝트
    /// </summary>
    public GameObject register_Menu;
    /// <summary>
    /// 가입 확인 메뉴 오브젝트
    /// </summary>
    public GameObject registerCheck_Menu;
    
    /// <summary>
    /// 계좌 내역 메뉴 오브젝트
    /// </summary>
    public GameObject[] CheckAccount_Menu = new GameObject[4];
    
    /// <summary>
    /// 보유 가능 최대 금액 (1800경)
    /// </summary>
    public static ulong MaxMoney = 18000000000000000000;

    /// <summary>
    /// A 상품의 총 입금액, 이자, 만기 시간정보 텍스트
    /// </summary>
    public Text[] A_text = new Text[3];
    /// <summary>
    /// B 상품의 총 입금액, 이자, 만기 시간정보 텍스트
    /// </summary>
    public Text[] B_text = new Text[3];
    /// <summary>
    /// C 상품의 총 입금액, 이자, 만기 시간정보 텍스트
    /// </summary>
    public Text[] C_text = new Text[3];

    /// <summary>
    /// 예금할 금액 텍스트
    /// </summary>
    public Text depositing_text;
    /// <summary>
    /// 입금 상품 이름 텍스트
    /// </summary>
    public Text merchandise_text;
    /// <summary>
    /// 출금할 금액 텍스트
    /// </summary>
    public Text withdraw_text;
    /// <summary>
    /// 출금할 상품 이름 텍스트
    /// </summary>
    public Text merchandiseWD_text;
    /// <summary>
    /// 결과 텍스트
    /// </summary>
    public Text result_text;

    /// <summary>
    /// 가입할 상품 정보 텍스트
    /// </summary>
    public Text register_merchandise_text;
    /// <summary>
    /// 계약 시간 정보 텍스트
    /// </summary>
    public Text contractTime_text;
    /// <summary>
    /// 이자 텍스트
    /// </summary>
    public Text benefit_text;

    /// <summary>
    /// 작업 대상 계좌 타입
    /// </summary>
    private AccountType? targetAccount;
    /// <summary>
    /// 입금액 입력 필드
    /// </summary>
    public InputField deposit_inputField;

    public MessageManager messageManager;

    /// <summary>
    /// 가입되지 않음에 대한 알림 오브젝트
    /// </summary>
    public GameObject notRegisteredBlockImg;

    /// <summary>
    /// 예금할 금액
    /// </summary>
    private ulong depositMoney;
    /// <summary>
    /// 출금할 금액
    /// </summary>
    private ulong withdrawMoney;


    public BankData bankData;

    /// <summary>
    /// 상품 가입 여부 데이터
    /// </summary>
    public bool[] IsRegistered { get { return bankData.isRegisteredNormal; } set { bankData.isRegisteredNormal = value; } }
    /// <summary>
    /// 예치금 데이터
    /// </summary>
    public ulong[] SavedMoney { get { return bankData.savedMoneyNormal; } set { bankData.savedMoneyNormal = value; } }
    /// <summary>
    /// 이자 금액 데이터
    /// </summary>
    public ulong[] AddedMoney { get { return bankData.addedMoneyNormal; } set { bankData.addedMoneyNormal = value; } }
    /// <summary>
    /// 남은 계약 시간 데이터
    /// </summary>
    public int[] ContractTime { get { return bankData.contractTimesNormal; } set { bankData.contractTimesNormal = value; } }
    /// <summary>
    /// 상품별 이율
    /// </summary>
    public float[] rate = new float[3] { 0.001f, 0.0006f, 0.0005f};
    /// <summary>
    /// 입금 제한 금액
    /// </summary>
    public ulong[] limitDeposit = new ulong[3] { 100000000,100000000000,100000000000000};
    /// <summary>
    /// 각 상품의 이름
    /// </summary>
    private string[] productNames = { "A", "B", "C" };
    /// <summary>
    /// 만기 알림 전송 여부
    /// </summary>
    private bool[] isAlarmed = new bool[3];

    /// <summary>
    /// 은행 상품 메뉴
    /// </summary>
    public GameObject bankProductMenu;
    /// <summary>
    /// 복권 구매 메뉴
    /// </summary>
    public GameObject lotteryTicketMenu;

    public AchievementManager achievementManager;

    void Start()
    {
        StartCoroutine(CalTimer());
    }
    /// <summary>
    /// 계약 시간 카운터 코루틴
    /// </summary>
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
                // 만기 시, 만기 알림 표시
                if(!isAlarmed[i] && IsRegistered[i])
                {
                    isAlarmed[i] = true;
                    messageManager.ShowPopupMessage("상품 " + productNames[i] + "가 만기 되어 출금이 가능합니다.");
                }
            }
        }
        UpdateStateTexts();

        StartCoroutine(CalTimer());
    }
    /// <summary>
    /// 은행 메뉴 활성화/비활성화
    /// </summary>
    /// <param name="active">활성화 여부</param>
    public void SetBankProductMenu(bool active)
    {
        bankProductMenu.SetActive(active);
    }    
    /// <summary>
    /// 복권 메뉴 활성화/비활성화
    /// </summary>
    /// <param name="active">활성화 여부</param>
    public void SetLotteryTicketMenu(bool active)
    {
        lotteryTicketMenu.SetActive(active);
    }

    /// <summary>
    /// 각 상품 조회 및 가입 메뉴 버튼 리스너
    /// </summary>
    /// <param name="nKey"></param>
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
    /// <summary>
    /// 각 상품별 가입 및 가입 확인 처리 버튼 리스너 
    /// </summary>
    /// <param name="nKey"></param>
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
    /// <summary>
    /// 각 상품별 상태 조회 버튼 리스너
    /// </summary>
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
    /// <summary>
    /// 입금 기능 버튼 리스너
    /// </summary>
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
            case 13://Max
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

    /// <summary>
    /// 예금 입력 처리
    /// </summary>
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

    /// <summary>
    /// 빠른 출금
    /// </summary>
    /// <param name="i">상품 인덱스</param>
    /// <returns>출금액</returns>
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

    /// <summary>
    /// 출금 확인 버튼 리스너
    /// </summary>
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

    /// <summary>
    /// 출금 메시지 출력
    /// </summary>
    private void PrintWithdrawMessage()
    {
        string money1 = "", money2 = "";
        PlayManager.ArrangeUnit(withdrawMoney, 0, ref money1, ref money2, true);
        messageManager.ShowMessage("<color=green>" + money1 + "$</color>가 정상적으로 출금 되었습니다.", 3f);
    }
    /// <summary>
    /// 입금 메시지출력
    /// </summary>
    private void PrintDepositMessage()
    {
        string money1 = "", money2 = "";
        PlayManager.ArrangeUnit(depositMoney, 0, ref money1, ref money2, true);
        messageManager.ShowMessage("<color=blue>" + money2 + money1 + "$</color>가 정상적으로 입금 되었습니다.", 3f);
    }

    /// <summary>
    /// 각 상품별 입금 처리
    /// </summary>
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
    /// <summary>
    /// 특정 상품의 입금 처리
    /// </summary>
    /// <param name="i">상품 인덱스</param>
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

    /// <summary>
    /// 특정 상품 출금 처리
    /// </summary>
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
        achievementManager.CumulativeInterest += interest * 1.1f;
    }
    /// <summary>
    /// 예치금 이자 지급
    /// </summary>
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
    }

    /// <summary>
    /// 각 상품 별 텍스트 정보 업데이트
    /// </summary>
    private void UpdateStateTexts()
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
        A_text[2].text = (int)(ContractTime[0] / 60) + "분" + ContractTime[0]%60 + "초";
        B_text[2].text = (int)(ContractTime[1] / 60) + "분" + ContractTime[1]%60 + "초";
        C_text[2].text = (int)(ContractTime[2] / 60) + "분" + ContractTime[2]%60 + "초";
    }
    /// <summary>
    /// 이미 가입된 상품에 대한 알림 메시지
    /// </summary>
    void AlreadyRegistered()
    {
        messageManager.ShowMessage("이미 가입된 상품입니다.");
    }
    /// <summary>
    /// 가입 완료 알림 메시지
    /// </summary>
    void Alarm_registered()
    {
        messageManager.ShowMessage("가입이 완료 되었습니다.");
    }
    /// <summary>
    /// 가입 요구 안내 메시지 출력
    /// </summary>
    void RegisterFirst()
    {
        messageManager.ShowMessage("해당 상품 가입을 먼저 해주세요.");
    }
    /// <summary>
    /// 입금 최대 금액 메시지 출력
    /// </summary>
    void FullMoney()
    {
        messageManager.ShowMessage("입금 가능한 최대 금액입니다.");
    }
}
