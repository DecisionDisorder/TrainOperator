using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DriversElevateManager : MonoBehaviour {

    public MessageManager messageManager;
    public DriversManager drivers_Manager;
    public SalaryPay_Controller salaryPay_Controller;

    public GameObject ElevateDriversCheck_Menu;

    public Text driverRank_text;
    public Text numOfElevate_text;
    public Text elevatePrice_text;
    public Text addPay_text;
    public Text addPS_text;

    public static int Rank;

    public static int NumofDriver;
    public static ulong TotalPrice;
    public static ulong AddedPay;
    public static ulong AddedPs;

    public Button[] elevateButtons;


    public void PressKey(int driverCode)
    {
        Rank = driverCode;
        driverRank_text.text = drivers_Manager.driverNames[Rank];
        ElevateDriversCheck_Menu.SetActive(true);
        SetButtonActive(false);
    }

    private void SetButtonActive(bool active)
    {
        for (int i = 0; i < elevateButtons.Length; i++)
            elevateButtons[i].enabled = active;
    }

    public void PressKey_CheckMenu(int nKey)
    {
        switch (nKey)
        {
            case 0://cancel
                ElevateDriversCheck_Menu.SetActive(false);
                NumofDriver = 0;
                TotalPrice = 0;
                numOfElevate_text.text = "0명";
                elevatePrice_text.text = "승진 비용: 0$";
                addPay_text.text = "추가 월급: 0$";
                break;
            case 1://apply
                if (AssetMoneyCalculator.instance.ArithmeticOperation(TotalPrice, 0, false))
                {
                    ArrangeDrivers();
                }
                else
                {
                    messageManager.ShowMessage("돈이 부족합니다.",1.0f);
                }
                NumofDriver = 0;
                TotalPrice = 0;
                numOfElevate_text.text = "0명";
                elevatePrice_text.text = "승진 비용: 0$";
                addPay_text.text = "추가 월급: 0$";
                ElevateDriversCheck_Menu.SetActive(false);
                DataManager.instance.SaveAll();
                break;
        }
        SetButtonActive(true);
    }
    public void PressKey_Num(int nKey)
    {
        switch (nKey)
        {
            case 0://up
                if (NumofDriver < drivers_Manager.numOfDrivers[Rank])
                {
                    NumofDriver++;
                }
                TotalPrices();
                string totalprice = string.Format("{0:#,###}", TotalPrice);
                elevatePrice_text.text = "승진 비용: " + totalprice + "$";
                string addpay = string.Format("{0:#,###}", AddedPay);
                addPay_text.text = "추가 월급: " + addpay + "$";
                numOfElevate_text.text = NumofDriver + "명";
                string APS = string.Format("{0:#,###}", AddedPs);
                addPS_text.text = "추가 최대 승객 수: " + APS + "명";
                if (NumofDriver == 0)
                {
                    elevatePrice_text.text = "승진 비용: 0$";
                    addPay_text.text = "추가 월급: 0$";
                    addPS_text.text = "추가 최대 승객 수: 0명";
                }
               
                break;
            case 1://down
                if (NumofDriver > 0)
                {
                    NumofDriver--;
                }
                TotalPrices();
                string totalprice2 = string.Format("{0:#,###}", TotalPrice);
                elevatePrice_text.text = "승진 비용: " + totalprice2 + "$";
                string addpay1 = string.Format("{0:#,###}", AddedPay);
                addPay_text.text = "추가 월급: " + addpay1 + "$";
                numOfElevate_text.text = NumofDriver + "명";
                string APS1 = string.Format("{0:#,###}", AddedPs);
                addPS_text.text = "추가 최대 승객 수: " + APS1 + "명";
                if (NumofDriver == 0)
                {
                    elevatePrice_text.text = "승진 비용: 0$";
                    addPay_text.text = "추가 월급: 0$";
                    addPS_text.text = "추가 최대 승객 수: 0명";
                }
                break;
            case 2://all
                NumofDriver = drivers_Manager.numOfDrivers[Rank];
                TotalPrices();
                string totalprice3 = string.Format("{0:#,###}", TotalPrice);
                elevatePrice_text.text = "승진 비용: " + totalprice3 + "$";
                string addpay2 = string.Format("{0:#,###}", AddedPay);
                addPay_text.text = "추가 월급: " + addpay2 + "$";
                numOfElevate_text.text = NumofDriver + "명";
                string APS2 = string.Format("{0:#,###}", AddedPs);
                addPS_text.text = "추가 최대 승객 수: " + APS2 + "명";
                if (NumofDriver == 0)
                {
                    elevatePrice_text.text = "승진 비용: 0$";
                    addPay_text.text = "추가 월급: 0$";
                    addPS_text.text = "추가 최대 승객 수: 0명";
                }
                break;
        }
    }
    void TotalPrices()
    {
        TotalPrice = DriversManager.pay_Drivers[Rank] * 1000 * (ulong)NumofDriver;
        AddedPay = (DriversManager.pay_Drivers[Rank + 1] - DriversManager.pay_Drivers[Rank]) * (ulong)NumofDriver;
        AddedPs = DriversManager.passenger_Drivers[Rank] * (ulong)NumofDriver;
    }
    void ArrangeDrivers()
    {
        drivers_Manager.numOfDrivers[Rank] -= NumofDriver;
        drivers_Manager.numOfDrivers[Rank + 1] += NumofDriver;
        MyAsset.instance.PassengersLimitLow += AddedPs;
        salaryPay_Controller.CalculateSalary();
    }
}