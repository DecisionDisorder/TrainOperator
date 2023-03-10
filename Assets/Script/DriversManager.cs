using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 기관사 구매 시스템 관리 클래스
/// </summary>
public class DriversManager : MonoBehaviour
{
    /// <summary>
    /// 기관사 데이터 오브젝트
    /// </summary>
    public DriverData driverData;

    /// <summary>
    /// 고용 현황 메뉴 오브젝트
    /// </summary>
    public GameObject condition_Driver_Menu;

    /// <summary>
    /// 총 급여 텍스트
    /// </summary>
    public Text Salary_text;
    /// <summary>
    /// 기관사 등급별 인원 수 텍스트
    /// </summary>
    public Text[] numofDrivers_text = new Text[8];

    public MessageManager messageManager;
    public ButtonColor_Controller buttonColor_Controller;
    public ColorUpdater colorUpdater;
    public AssetInfoUpdater AddMoney;
    public SalaryPay_Controller salaryPay_Controller;

    /// <summary>
    /// 기관사 구매 메뉴 오브젝트
    /// </summary>
    public GameObject Driver_Menu;

    /// <summary>
    /// 등급 별 가격 텍스트
    /// </summary>
    public Text[] price_text = new Text[8];
    /// <summary>
    /// 등급 별 월급 텍스트
    /// </summary>
    public Text[] pay_text = new Text[8];
    /// <summary>
    /// 등급 별 추가 승객 수 제한량 텍스트
    /// </summary>
    public Text[] passenger_text = new Text[8];

    /// <summary>
    /// 등급 별 기관사 인원수 
    /// </summary>
    public int[] numOfDrivers { get { return driverData.numOfDrivers; } set { driverData.numOfDrivers = value; } }
    /// <summary>
    /// 등급 별 기관사 가격
    /// </summary>
    public static ulong[] price_Drivers = new ulong[11] { 5000, 1000000, 1300000000, 150000000000, 3500000000000, 80000000000000, 3400000000000000, 7, 50, 1500, 12000};
    /// <summary>
    /// 7번 등급 기관사 가격 (LargeVariable)
    /// </summary>
    public LargeVariable priceDriver7;
    /// <summary>
    /// S+급 기관사의 High Unit(1경 이상) 가격
    /// </summary>
    public static ulong price_Driver_Sp_2;
    /// <summary>
    /// 등급별 기관사 월급
    /// </summary>
    public static ulong[] pay_Drivers = new ulong[11] { 1000, 100000, 5000000, 50000000, 1500000000, 50000000000, 1000000000000, 50000000000000, 100000000000000, 500000000000000, 1000000000000000 };
    /// <summary>
    /// 등급별 최대 승객 수 확장 수치
    /// </summary>
    public static ulong[] passenger_Drivers = new ulong[11] { 500, 50000, 1000000, 15000000, 150000000, 5000000000, 50000000000, 500000000000, 5000000000000, 50000000000000, 100000000000000 };
    /// <summary>
    /// 등급별 구매 당 가격 인상 폭
    /// </summary>
    public ulong[] price_UP = new ulong[11] { 500, 100000, 130000000, 15000000000, 350000000000, 8000000000000, 350000000000000, 7000000000000000, 5, 120, 1200 };
    /// <summary>
    /// 등급별 구매 당 추가 승객 수 제한량 인상 폭
    /// </summary>
    public ulong[] passenger_UP = new ulong[11] { 100, 5000, 250000, 1750000, 37500000, 1000000000, 12500000000, 50000000000, 500000000000, 5000000000000, 12500000000000 };

    /// <summary>
    /// 등급별 기관사 기준 가격
    /// </summary>
    public ulong[] stanardPrice = new ulong[11] { 5000, 1000000, 1300000000, 150000000000, 3500000000000, 80000000000000, 3400000000000000, 7, 50, 1500, 12000 };
    /// <summary>
    /// 등급별 기관사 기준 추가 승객 수 확장 수치
    /// </summary>
    public ulong[] standardPassenger = new ulong[11] { 500, 50000, 1000000, 15000000, 150000000, 5000000000, 50000000000, 500000000000, 5000000000000, 50000000000000, 100000000000000 };
    /// <summary>
    /// 등급별 기관사 수식어 명칭
    /// </summary>
    public string[] driverNames = {  "신입", "D급", "C급", "B급", "A급", "S급", "S+급", "대단한", "훌륭한", "경험 많은"};

    /// <summary>
    /// 문자열로 정리된 Low Unit (1경 미만)
    /// </summary>
    private string money1;
    /// <summary>
    /// 문자열로 정리된 High (1경 이상)
    /// </summary>
    private string money2;
    /// <summary>
    /// 간편 연속 구매 중 인지 여부
    /// </summary>
    private bool isEasyPurchase = false;

    /// <summary>
    /// 간편 연속 구매 처리 관리 오브젝트
    /// </summary>
    public ContinuousPurchase continuousPurchase;

    void Start()
    {
        StartCoroutine(Timer());
        RenewPrice();
        UpdateTextInfo();
    }

    /// <summary>
    /// 기관사 구매 정보 주기적 업데이트를 위한 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator Timer()
    {
        yield return new WaitForSeconds(2.0f);

        UpdateTextInfo();

        StartCoroutine(Timer());
    }

    /// <summary>
    /// 등급별 고용 버튼
    /// </summary>
    /// <param name="nKey">기관사 등급</param>
    public void PressKey_Hire(int nKey)
    {
        HireDriver(nKey);
    }
    /// <summary>
    /// 기관사 구매 메뉴 관련 버튼 리스너
    /// </summary>
    public void PressKey(int nKey)
    {
        switch (nKey)
        {
            case 0:
                colorUpdater.StopUpdateButtonColor();
                Driver_Menu.SetActive(false);
                break;
            case 1:
                condition_Driver_Menu.SetActive(false);
                break;
            case 2:
                colorUpdater.StartUpdateButtonColor(buttonColor_Controller.SetDriver);
                break;
        }
    }

    /// <summary>
    /// 기관사 가격 정보 갱신
    /// </summary>
    public void RenewPrice()
    {
        for (int i = 0; i < numOfDrivers.Length; ++i)
        {
            if(!numOfDrivers[i].Equals(0))
            {
                if (i == 7)
                {
                    priceDriver7 = new LargeVariable(0, stanardPrice[7],  true);
                    priceDriver7 += new LargeVariable(price_UP[7] * (ulong)numOfDrivers[i], 0);
                }
                else
                {
                    price_Drivers[i] = stanardPrice[i] + price_UP[i] * (ulong)numOfDrivers[i];
                    passenger_Drivers[i] = standardPassenger[i] + passenger_UP[i] * (ulong)numOfDrivers[i];
                }
            }
            else
            {
                if(i == 7)
                    priceDriver7 = new LargeVariable(0, stanardPrice[7], true);
            }
        }
        if (price_Drivers[6] > 10000000000000000)
        {
            price_Drivers[6] = 9990000000000000;
        }
    }

    /// <summary>
    /// 기관사 간편 연속 구매 시작
    /// </summary>
    /// <param name="i"></param>
    public void StartPurchase(int i)
    {
        isEasyPurchase = true;
        continuousPurchase.StartPurchase(HireDriver, i);
    }
    /// <summary>
    /// 기관사 간편 연속 구매 중단
    /// </summary>
    public void StopPurchase()
    {
        isEasyPurchase = false;
        continuousPurchase.StopPurchase();
    }
    /// <summary>
    /// 선택한 등급의 기관사 고용 처리
    /// </summary>
    /// <param name="i"></param>
    private void HireDriver(int i)
    {
        // 5번 인덱스까지의 고용(구매) 처리
        if (i < 6)
        {
            // 가격 지불 처리
            if (AssetMoneyCalculator.instance.ArithmeticOperation(price_Drivers[i], 0, false))
            {
                ApplyHire(i);
            }
            else
            {
                messageManager.ShowMessage("돈이 부족합니다.");
            }
        }
        else if(i == 7) // 대단한 기관사
        {
            if (AssetMoneyCalculator.instance.ArithmeticOperation(priceDriver7, false))
            {
                ApplyHire(i);
            }
        }
        else if(i > 7)// 훌륭한 부터 적용
        {
            if(AssetMoneyCalculator.instance.ArithmeticOperation(0, price_Drivers[i], false))
            {
                ApplyHire(i);
            }
            else
            {
                messageManager.ShowMessage("돈이 부족합니다.");
            }
        }
        else//S+ 등급 기관사 까지 적용
        {
            if (price_Drivers[i] < 17000000000000000000)
            {
                if (AssetMoneyCalculator.instance.ArithmeticOperation(price_Drivers[i], 0, false))
                {
                    ApplyHire(i);
                }
                else
                {
                    messageManager.ShowMessage("돈이 부족합니다.");
                }
            }
            else
            {
                messageManager.ShowMessage("더 이상 고용이 불가능합니다.\n다음 등급의 기관사를 고용해주세요.", 1.5f);
            }
        }
        AddMoney.UpdatePassengerText();
    }

    /// <summary>
    /// 기관사 고용(구매) 성공 후 데이터 적용
    /// </summary>
    /// <param name="i">기관사 등급</param>
    private void ApplyHire(int i)
    {
        // 승객 수 제한 확장 및 인원 수 추가 작업
        TouchMoneyManager.AddPassengerLimit(passenger_Drivers[i], 0);
        numOfDrivers[i]++;
        RenewPrice();
        // 월급 갱신 및 UI 업데이트
        salaryPay_Controller.CalculateSalary();
        if (!isEasyPurchase || messageManager.messageText.text.Equals(""))
            MessageBuy(i);
        UpdateTextInfo();
    }

    /// <summary>
    /// 구매 완료 메시지 출력
    /// </summary>
    /// <param name="i">기관사 등급</param>
    private void MessageBuy(int i)
    {
        messageManager.ShowMessage(driverNames[i] + " 기관사를 고용하였습니다.");
    }

    /// <summary>
    /// 기관사 고용(구매) 정보 업데이트
    /// </summary>
    private void UpdateTextInfo()
    {
        for (int i = 0; i < numOfDrivers.Length; ++i)
        {
            if (i < 6)
            {
                PlayManager.ArrangeUnit(price_Drivers[i], 0, ref money1, ref money2, true);
                price_text[i].text = "가격: " + money2 + money1 + "$";
            }
            else if(i == 7)
            {
                price_text[i].text = "가격: " + priceDriver7.ToString() + "$";
            }
            else if(i > 7)
            {
                ArrangeUnit(0, price_Drivers[i]);
                price_text[i].text = "가격: " + money2 + "$";
            }
            else
            {
                ArrangeUnit(price_Drivers[i], price_Driver_Sp_2);
                price_text[i].text = "가격: " + money2 + money1 + "$";
            }

            PlayManager.ArrangeUnit(pay_Drivers[i], 0, ref money1, ref money2, true);
            pay_text[i].text = "월급: " + money1 + "$";

            PlayManager.ArrangeUnit(passenger_Drivers[i], 0, ref money1, ref money2, true);
            passenger_text[i].text = "최대 승객 수: +" + money2 + money1 + "명";

            //현황---------------------------------------------------------------------------------
            numofDrivers_text[i].text = numOfDrivers[i] + "명";
        }

        string m1 = "", m2 = "";
        PlayManager.ArrangeUnit(SalaryPay_Controller.totalPayLow, SalaryPay_Controller.totalPayHigh, ref m1, ref m2);

        Salary_text.text = m2 + m1 + "$";
        if (SalaryPay_Controller.totalPayLow == 0 && SalaryPay_Controller.totalPayHigh == 0)
        {
            Salary_text.text = "0$";
        }
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
        else if (insertvar < 10000)
        {
            money1 = string.Format("{0:#,###}", insertvar);
        }
        else if (insertvar < 100000000)
        {
            if ((insertvar % 10000).Equals(0))
            {
                money1 = string.Format("{0:####만}", insertvar * 0.0001);
            }
            else
            {
                money1 = string.Format("{0:####만####}", insertvar);
            }
        }
        else if (insertvar < 1000000000000)
        {
            ulong downmoney = (ulong)(insertvar * 0.0001);
            if ((downmoney % 10000).Equals(0))
            {
                money1 = string.Format("{0:####억}", downmoney * 0.0001);
            }
            else
            {
                money1 = string.Format("{0:####억####만}", downmoney);
            }

        }
        else
        {
            ulong downmoney = (ulong)(insertvar * 0.000000000001);

            if (downmoney < 10000)
            {
                money1 = string.Format("{0:####조}", downmoney);
            }
            else
            {
                insertvar2 += (ulong)(downmoney * 0.0001f);
                downmoney %= 10000;
                if (downmoney.Equals(0))
                {
                    money1 = "";
                }
                else
                {
                    money1 = string.Format("{0:####조}", downmoney);
                }
            }
        }
        if (insertvar2.Equals(0))
        {
            money2 = "";
        }
        else if (insertvar2 < 10000)
        {
            money2 = string.Format("{0:####경}", insertvar2);
        }
        else
        {
            money2 = string.Format("{0:####해####경}", insertvar2);
        }
    }

}
