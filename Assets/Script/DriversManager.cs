using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DriversManager : MonoBehaviour
{
    public DriverData driverData;

    public GameObject condition_Driver_Menu;

    public Text Salary_text;
    public Text[] numofDrivers_text = new Text[8];

    public MessageManager messageManager;
    public ButtonColor_Controller buttonColor_Controller;
    public ColorUpdater colorUpdater;
    public AssetInfoUpdater AddMoney;
    public SalaryPay_Controller salaryPay_Controller;

    public GameObject Driver_Menu;

    public Text[] price_text = new Text[8];
    public Text[] pay_text = new Text[8];
    public Text[] passenger_text = new Text[8];

    public int[] numOfDrivers { get { return driverData.numOfDrivers; } set { driverData.numOfDrivers = value; } }
    public static ulong[] price_Drivers = new ulong[11] { 5000, 1000000, 1300000000, 150000000000, 3500000000000, 80000000000000, 3400000000000000, 7, 50, 1500, 12000};
    public LargeVariable priceDriver7;
    public static ulong price_Driver_Sp_2;
    public static ulong[] pay_Drivers = new ulong[11] { 1000, 100000, 5000000, 50000000, 1500000000, 50000000000, 1000000000000, 50000000000000, 100000000000000, 500000000000000, 1000000000000000 };
    public static ulong[] passenger_Drivers = new ulong[11] { 500, 50000, 1000000, 15000000, 150000000, 5000000000, 50000000000, 500000000000, 5000000000000, 50000000000000, 100000000000000 };
    public ulong[] price_UP = new ulong[11] { 500, 100000, 130000000, 15000000000, 350000000000, 8000000000000, 350000000000000, 7000000000000000, 5, 120, 1200 };
    public ulong[] passenger_UP = new ulong[11] { 100, 5000, 250000, 1750000, 37500000, 1000000000, 12500000000, 50000000000, 500000000000, 5000000000000, 12500000000000 };

    public ulong[] stanardPrice = new ulong[11] { 5000, 1000000, 1300000000, 150000000000, 3500000000000, 80000000000000, 3400000000000000, 7, 50, 1500, 12000 };
    public ulong[] standardPassenger = new ulong[11] { 500, 50000, 1000000, 15000000, 150000000, 5000000000, 50000000000, 500000000000, 5000000000000, 50000000000000, 100000000000000 };
    public string[] driverNames = {  "신입", "D급", "C급", "B급", "A급", "S급", "S+급", "대단한", "훌륭한", "경험 많은"};

    string money1;
    string money2;
    private bool isEasyPurchase = false;

    public ContinuousPurchase continuousPurchase;

    void Start() //1회에 한하여 기존의 기관사들과 동기화 시키기
    {
        StartCoroutine(Timer());
        //LoadDrivers();
        RenewPrice();
        TextInfo();
    }
    IEnumerator Timer()
    {
        yield return new WaitForSeconds(2.0f);

        TextInfo();

        StartCoroutine(Timer());
    }

    public void PressKey_Hire(int nKey)
    {
        IncreaseData(nKey);
    }
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

    public void StartPurchase(int i)
    {
        isEasyPurchase = true;
        continuousPurchase.StartPurchase(IncreaseData, i);
    }
    public void StopPurchase()
    {
        isEasyPurchase = false;
        continuousPurchase.StopPurchase();
    }
    void IncreaseData(int i)
    {
        if (i < 6)
        {
            if (AssetMoneyCalculator.instance.ArithmeticOperation(price_Drivers[i], 0, false))
            {
                TouchMoneyManager.AddPassengerLimit(passenger_Drivers[i], 0);
                numOfDrivers[i]++;
                RenewPrice();
                salaryPay_Controller.CalculateSalary();
                //SaveDrivers();
                //buttonColor_Controller.SetDriver();
                if(!isEasyPurchase || messageManager.messageText.text.Equals(""))
                    MessageBuy(i);
                TextInfo();
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
                TouchMoneyManager.AddPassengerLimit(passenger_Drivers[i], 0);
                numOfDrivers[i]++;
                RenewPrice();
                salaryPay_Controller.CalculateSalary();
                if (!isEasyPurchase || messageManager.messageText.text.Equals(""))
                    MessageBuy(i);
                TextInfo();
            }
        }
        else if(i > 7)// 훌륭한 부터 적용
        {
            if(AssetMoneyCalculator.instance.ArithmeticOperation(0, price_Drivers[i], false))
            {
                TouchMoneyManager.AddPassengerLimit(passenger_Drivers[i], 0);
                numOfDrivers[i]++;
                RenewPrice();
                salaryPay_Controller.CalculateSalary();
                //SaveDrivers();
                //buttonColor_Controller.SetDriver();
                if (!isEasyPurchase || messageManager.messageText.text.Equals(""))
                    MessageBuy(i);
                TextInfo();
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
                    TouchMoneyManager.AddPassengerLimit(passenger_Drivers[i], 0);
                    numOfDrivers[i]++;
                    RenewPrice();
                    salaryPay_Controller.CalculateSalary();
                    //SaveDrivers();
                    //buttonColor_Controller.SetDriver();
                    if (!isEasyPurchase || messageManager.messageText.text.Equals(""))
                        MessageBuy(i);
                    TextInfo();
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

    private void MessageBuy(int i)
    {
        messageManager.ShowMessage(driverNames[i] + " 기관사를 고용하였습니다.");
    }

    private void TextInfo()
    {
        for (int i = 0; i < numOfDrivers.Length; ++i)
        {
            //string price = string.Format("{0:#,###}",price_Drivers[i]);
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

            //string pay = string.Format("{0:#,###}",pay_Drivers[i]);

            PlayManager.ArrangeUnit(pay_Drivers[i], 0, ref money1, ref money2, true);
            pay_text[i].text = "월급: " + money1 + "$";

            //string passenger = string.Format("{0:#,###}",passenger_Drivers[i]);
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
    /*
    public static void SaveDrivers()//이전 작업 완료시, DataManager 스크립트에 적용시키기
    {
        for (int i = 0; i < numOfDrivers.Length; ++i)
        {
            PlayerPrefs.SetInt("numofDrivers[" + i + "]", numOfDrivers[i]);
        }
    }
    public static void LoadDrivers()
    {
        for (int i = 0; i < numOfDrivers.Length; ++i)
        {
            numOfDrivers[i] = PlayerPrefs.GetInt("numofDrivers[" + i + "]", 0);
        }
    }
    public static void ResetDrivers()
    {
        for (int i = 0; i < numOfDrivers.Length; ++i)
        {
            numOfDrivers[i] = 0;
        }
    }*/
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
    void SetDriverPriceUnit()
    {
        ulong insertVar = price_Drivers[6];
        ulong temp = price_Drivers[6];
        temp = (ulong)(temp * 0.0001);
        temp = (ulong)(temp * 0.0001);
        temp = (ulong)(temp * 0.0001);
        temp = (ulong)(temp * 0.0001);
        price_Driver_Sp_2 = temp;
        insertVar %= 10000000000000000;
        price_Drivers[6] = insertVar;

    }

}
