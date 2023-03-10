using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// 은행 시스템(특별 상품) 관리 클래스
/// </summary>
public class BankSpecialManager : MonoBehaviour
{
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
    public GameObject[] CheckAccount_Menu = new GameObject[3];

    /// <summary>
    /// 보유 가능 최대 금액 (1800경)
    /// </summary>
    public static ulong MaxMoney = 18000000000000000000;

    /// <summary>
    /// 스페셜 A 상품의 총 입금액, 이자, 만기 시간정보 텍스트
    /// </summary>
    public Text[] SA_text = new Text[3];
    /// <summary>
    /// 스페셜 S 상품의 총 입금액, 이자, 만기 시간정보 텍스트
    /// </summary>
    public Text[] SS_text = new Text[3];
    /// <summary>
    /// 스페셜 S Plus 상품의 총 입금액, 이자, 만기 시간정보 텍스트
    /// </summary>
    public Text[] SSP_text = new Text[3];

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

    /// <summary>
    /// 가입되지 않음에 대한 알림 오브젝트
    /// </summary>
    public GameObject notRegisteredBlockImg;

    public MessageManager messageManager;
    public BankManager bankManager;

    /// <summary>
    /// 이자 입금 대기 시간 알림 텍스트
    /// </summary>
    public Text timeleft_text;

    /// <summary>
    /// 예금할 금액
    /// </summary>
    public static ulong depositMoney;

    /// <summary>
    /// 출금할 금액 (Low Unit - 1경 미만)
    /// </summary>
    private ulong withdrawMoneyLow;
    /// <summary>
    /// 출금할 금액 (High Unit - 1경 이상)
    /// </summary>
    private ulong withdrawMoneyHigh;

    /// <summary>
    /// 상품 가입 여부 데이터
    /// </summary>
    public bool[] IsRegistered { get { return bankManager.bankData.isRegisteredSpecial; } set { bankManager.bankData.isRegisteredSpecial = value; } }

    /// <summary>
    /// 이자 금액 데이터 (Low Unit - 1경 미만)
    /// </summary>
    public ulong[] AddedMoneyLow { get { return bankManager.bankData.addedMoneySpecialLow; } set { bankManager.bankData.addedMoneySpecialLow = value; } }
    /// <summary>
    /// 이자 금액 데이터 (High Unit - 1경 이상)
    /// </summary>
    public ulong[] AddedMoneyHigh { get { return bankManager.bankData.addedMoneySpecialHigh; } set { bankManager.bankData.addedMoneySpecialHigh = value; } }
    /// <summary>
    /// 예치금 데이터 (High Unit - 1경 이상)
    /// </summary>
    public ulong[] SavedMoneyHigh { get { return bankManager.bankData.savedMoneySpecial; } set { bankManager.bankData.savedMoneySpecial = value; } }
    /// <summary>
    /// 남은 계약 시간 데이터
    /// </summary>
    public int[] ContractTime { get { return bankManager.bankData.contractTimesSpecial; } set { bankManager.bankData.contractTimesSpecial = value; } }
    /// <summary>
    /// 상품별 이율
    /// </summary>
    public float[] rate;
    /// <summary>
    /// 입금 제한 금액
    /// </summary>
    public ulong[] limitDeposit = new ulong[3] { 100 , 10000 , 1000000};
    /// <summary>
    /// 상품별 기준 계약 시간
    /// </summary>
    public int[] standardContractTimes;
    /// <summary>
    /// 각 상품의 이름
    /// </summary>
    private string[] productNames = { "스페셜A", "스페셜S", "스페셜S+" };
    /// <summary>
    /// 만기 알림 전송 여부
    /// </summary>
    private bool[] isAlarmed = new bool[3];

    /// <summary>
    /// 스페셜 상품 이자 지급 대기 시간
    /// </summary>
    public int Timer { get { return bankManager.bankData.timer; } set { bankManager.bankData.timer = value; } }

    /// <summary>
    /// 문자열로 정리된 Low Unit (1경 미만)
    /// </summary>
    private string money1;
    /// <summary>
    /// 문자열로 정리된 High (1경 이상)
    /// </summary>
    private string money2;

    /// <summary>
    /// 단위 계산 임시 변수 (low unit)
    /// </summary>
    private ulong tempLowUnit;
    /// <summary>
    /// 단위 계산 임시 변수 (high unit으로 합산 대기)
    /// </summary>
    private ulong tempLow2High;
    /// <summary>
    /// 단위 계산 임시 변수 (high unit)
    /// </summary>
    private ulong tempHighUnit;

    /// <summary>
    /// 단위 변환 기준 수치 (1경)
    /// </summary>
    public static ulong standard_maximum = 10000000000000000;

    public AchievementManager achievementManager;

    void Start()
    {
        StartCoroutine(CalTimer());
    }

    /// <summary>
    /// 이자 계산 대기 타이머 코루틴
    /// </summary>
    IEnumerator CalTimer()
    {
        yield return new WaitForSeconds(1f);

        // 대기 시간 1초 삭감
        Timer--;

        // 대기 시간이 0이 되면 이자 지급 후 대기 시간 초기화
        if (Timer <= 0)
        {
            CalculateMoney();
            Timer = 60;
        }

        // 각 상품 별 남은 계약 기간 삭감
        for (int i = 0; i < ContractTime.Length; ++i)
        {
            if (ContractTime[i] > 0)
            {
                ContractTime[i]--;
            }
            else
            {
                // 계약 기간이 만료 되면 팝업 메시지 출력
                if (!isAlarmed[i] && IsRegistered[i])
                {
                    isAlarmed[i] = true;
                    messageManager.ShowPopupMessage("상품 " + productNames[i] + "가 만기 되어 출금이 가능합니다.");
                }
            }
        }
        UpdateStateTexts();

        timeleft_text.text = "남은 이자 입금 시간: " + Timer + "초";

        StartCoroutine(CalTimer());
    }
    /// <summary>
    /// 스페셜 상품 관련 버튼 클릭 이벤트 리스너
    /// </summary>
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
    /// <summary>
    /// 스페셜 상품 가입 관련 버튼 이벤트 리스너
    /// </summary>
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
            case 4: // 스페셜 A 가입 확인
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
                // 스페셜 S 가입 확인
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
                // 스페셜 S+ 가입 확인
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
                        LargeVariable interest = new LargeVariable(AddedMoneyLow[0], AddedMoneyHigh[0]);
                        interest *= 1.1f;
                        withdrawMoneyLow = interest.lowUnit;
                        withdrawMoneyHigh = SavedMoneyHigh[0] + interest.highUnit;
                        ArrangeUnit(withdrawMoneyLow, withdrawMoneyHigh);
                        if (withdrawMoneyHigh > 0)
                        {
                            withdraw_text.text = "출금 할 금액:" + money2 + money1 + "$";
                        }
                        else
                        {
                            withdraw_text.text = "출금 할 금액:" + money1 + "$";
                        }

                        ArrangeUnit(interest.lowUnit, interest.highUnit);
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
                        LargeVariable interest = new LargeVariable(AddedMoneyLow[1], AddedMoneyHigh[1]);
                        interest *= 1.1f;
                        withdrawMoneyLow = interest.lowUnit;
                        withdrawMoneyHigh = SavedMoneyHigh[1] + interest.highUnit;
                        ArrangeUnit(withdrawMoneyLow, withdrawMoneyHigh);
                        if (withdrawMoneyHigh > 0)
                        {
                            withdraw_text.text = "출금 할 금액:" + money2 + money1 + "$";
                        }
                        else
                        {
                            withdraw_text.text = "출금 할 금액:" + money1 + "$";
                        }

                        ArrangeUnit(interest.lowUnit, interest.highUnit);
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
                        LargeVariable interest = new LargeVariable(AddedMoneyLow[2], AddedMoneyHigh[2]);
                        interest *= 1.1f;
                        withdrawMoneyLow = interest.lowUnit;
                        withdrawMoneyHigh = SavedMoneyHigh[2] + interest.highUnit;
                        ArrangeUnit(withdrawMoneyLow, withdrawMoneyHigh);
                        if (withdrawMoneyHigh > 0)
                        {
                            withdraw_text.text = "출금 할 금액:" + money2 + money1 + "$";
                        }
                        else
                        {
                            withdraw_text.text = "출금 할 금액:" + money1 + "$";
                        }

                        ArrangeUnit(interest.lowUnit, interest.highUnit);
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
    /// <summary>
    /// 예금 입력 처리
    /// </summary>
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

    /// <summary>
    /// 빠른 출금
    /// </summary>
    /// <param name="i">상품 인덱스</param>
    /// <param name="lowReturn">출금할 low unit</param>
    /// <param name="highReturn">출금할 high unit</param>
    public void QuickWithdraw(int i, ref ulong lowReturn, ref ulong highReturn)
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
        }
    }

    /// <summary>
    /// 출금 확인 버튼 리스너
    /// </summary>
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

    /// <summary>
    /// 출금 메시지 출력
    /// </summary>
    private void PrintWithdrawMessage()
    {
        string money1 = "", money2 = "";
        PlayManager.ArrangeUnit(withdrawMoneyLow, withdrawMoneyHigh, ref money1, ref money2, true);
        messageManager.ShowMessage("<color=green>" + money2 + money1 + "$</color>가 정상적으로 출금 되었습니다." , 3f);
    }
    /// <summary>
    /// 입금 메시지출력
    /// </summary>
    private void PrintDepositMessage()
    {
        string money1 = "", money2 = "";
        PlayManager.ArrangeUnit(0, depositMoney, ref money1, ref money2, true);
        messageManager.ShowMessage("<color=blue>" + money2 + "$</color>가 정상적으로 입금 되었습니다.", 3f);
    }
    /// <summary>
    /// 각 상품별 입금 처리
    /// </summary>
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
    /// <summary>
    /// 특정 상품의 입금 처리
    /// </summary>
    /// <param name="i">상품 인덱스</param>
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

    /// <summary>
    /// 특정 상품 출금 처리
    /// </summary>
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
        achievementManager.CumulativeInterest += interest * 1.1f;
    }
    /// <summary>
    /// 예치금 이자 지급
    /// </summary>
    private void CalculateMoney()
    {
        for (int i = 0; i < SavedMoneyHigh.Length; ++i)
        {
            if(ContractTime[i] > 0)
            {
                Percent_Cal_Money(rate[i], 0, SavedMoneyHigh[i]);
                AddedMoneyLow[i] += tempLowUnit;
                AddedMoneyHigh[i] += tempHighUnit;
                Synchronization_Money(AddedMoneyLow[i], AddedMoneyHigh[i]);
                AddedMoneyLow[i] = tempLowUnit;
                AddedMoneyHigh[i] = tempHighUnit;
            }
            if (AddedMoneyHigh[i] > MaxMoney)
            {
                AddedMoneyHigh[i] = MaxMoney;
            }
        }
    }

    /// <summary>
    /// 각 상품 별 텍스트 정보 업데이트
    /// </summary>
    private void UpdateStateTexts()
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
    /// <summary>
    /// 이미 가입된 상품에 대한 알림 메시지
    /// </summary>
    void AlreadyRegistered()
    {
        messageManager.ShowMessage("이미 가입된 상품입니다.", 1.0f);
    }
    /// <summary>
    /// 가입 완료 알림 메시지
    /// </summary>
    void Alarm_registered()
    {
        messageManager.ShowMessage("가입이 완료 되었습니다.", 1.0f);
    }
    /// <summary>
    /// 가입 요구 안내 메시지 출력
    /// </summary>
    void RegisterFirst()
    {
        messageManager.ShowMessage("해당 상품 가입을 먼저 해주세요.", 1.0f);
    }
    /// <summary>
    /// 입금 최대 금액 메시지 출력
    /// </summary>
    void FullMoney()
    {
        messageManager.ShowMessage("입금 가능한 최대 금액입니다.", 1.0f);
    }
    /// <summary>
    /// 큰 숫자를 '만/억/조/경' 등의 단위의 문자열로 정리
    /// </summary>
    /// <param name="insertvar">low unit (1경 미만)</param>
    /// <param name="insertvar2">high unit (1경 이상)</param>
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
    /// <summary>
    /// 이자 지급을 위한 퍼센트 계산
    /// </summary>
    /// <param name="percentage">백분율</param>
    /// <param name="fu">low unit(1경 미만)</param>
    /// <param name="su">high unit(1경 이상)</param>
    private void Percent_Cal_Money(float percentage , ulong fu , ulong su)
    {
        MoneyUnitTranslator.Multiply(ref fu, ref su, percentage / 100f);
        tempLowUnit = fu;
        tempHighUnit = su;
    }
    /// <summary>
    /// 1경 미만/이상 수치의 단위 정리
    /// </summary>
    /// <param name="fu">low unit(1경 미만)</param>
    /// <param name="su">high unit(1경 이상)</param>
    private void Synchronization_Money(ulong fu, ulong su)
    {
        tempLowUnit = fu;
        tempHighUnit = su;
        if (tempLowUnit >= standard_maximum)
        {
            tempLow2High = (ulong)(tempLowUnit / standard_maximum);
            tempLowUnit -= tempLow2High * standard_maximum;
            tempHighUnit += tempLow2High;
        }
    }
}
