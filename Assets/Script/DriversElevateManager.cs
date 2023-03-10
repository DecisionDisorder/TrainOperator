using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 기관사 승진 시스템 관리 클래스
/// </summary>
public class DriversElevateManager : MonoBehaviour {

    public MessageManager messageManager;
    public DriversManager drivers_Manager;
    public SalaryPay_Controller salaryPay_Controller;

    /// <summary>
    /// 승진 확인 메뉴 오브젝트
    /// </summary>
    public GameObject ElevateDriversCheck_Menu;

    /// <summary>
    /// 기관사 등급 텍스트
    /// </summary>
    public Text driverRank_text;
    /// <summary>
    /// 승진 대상 기관사 인원 수 텍스트
    /// </summary>
    public Text numOfElevate_text;
    /// <summary>
    /// 승진 비용 텍스트
    /// </summary>
    public Text elevatePrice_text;
    /// <summary>
    /// 추가되는 임금 텍스트
    /// </summary>
    public Text addPay_text;
    /// <summary>
    /// 추가되는 승객 수 제한량 텍스트
    /// </summary>
    public Text addPS_text;

    /// <summary>
    /// 승진 대상 기관사 등급
    /// </summary>
    private int rank;

    /// <summary>
    /// 승진 대상 기관시 인원 수
    /// </summary>
    private int numofDriver;
    /// <summary>
    /// 승진 비용
    /// </summary>
    private ulong totalPrice;
    /// <summary>
    /// 추가되는 임금
    /// </summary>
    private ulong addedPay;
    /// <summary>
    /// 추가되는 승객 수 제한량
    /// </summary>
    private ulong addedPs;

    /// <summary>
    /// 각 기관사 마다의 승진 버튼
    /// </summary>
    public Button[] elevateButtons;

    /// <summary>
    /// 승진 버튼 클릭 리스너
    /// </summary>
    /// <param name="driverCode">기관사 등급 인덱스</param>
    public void PressKey(int driverCode)
    {
        rank = driverCode;
        driverRank_text.text = drivers_Manager.driverNames[rank];
        ElevateDriversCheck_Menu.SetActive(true);
        SetButtonActive(false);
    }

    /// <summary>
    /// 승진 버튼 활성화/비활성화
    /// </summary>
    /// <param name="active">활성화 여부</param>
    private void SetButtonActive(bool active)
    {
        for (int i = 0; i < elevateButtons.Length; i++)
            elevateButtons[i].enabled = active;
    }

    /// <summary>
    /// 승진 확인 메뉴 버튼 리스너
    /// </summary>
    public void PressKey_CheckMenu(int nKey)
    {
        switch (nKey)
        {
            case 0://cancel
                ResetCheckMenu();
                break;
            case 1://apply
                if (AssetMoneyCalculator.instance.ArithmeticOperation(totalPrice, 0, false))
                {
                    ArrangeDrivers();
                }
                else
                {
                    messageManager.ShowMessage("돈이 부족합니다.",1.0f);
                }
                ResetCheckMenu();
                DataManager.instance.SaveAll();
                break;
        }
        SetButtonActive(true);
    }

    /// <summary>
    /// 승진 확인 메뉴 초기화
    /// </summary>
    private void ResetCheckMenu()
    {
        ElevateDriversCheck_Menu.SetActive(false);
        numofDriver = 0;
        totalPrice = 0;
        numOfElevate_text.text = "0명";
        elevatePrice_text.text = "승진 비용: 0$";
        addPay_text.text = "추가 월급: 0$";
    }

    /// <summary>
    /// 승진 대상 인원 수 조절 버튼 리스너
    /// </summary>
    public void PressKey_Num(int nKey)
    {
        switch (nKey)
        {
            case 0://up
                if (numofDriver < drivers_Manager.numOfDrivers[rank])
                {
                    numofDriver++;
                }
                UpdateValues();
                UpdateCheckMenuTexts();
                break;
            case 1://down
                if (numofDriver > 0)
                {
                    numofDriver--;
                }
                UpdateValues();
                UpdateCheckMenuTexts();
                break;
            case 2://all
                numofDriver = drivers_Manager.numOfDrivers[rank];
                UpdateValues();
                UpdateCheckMenuTexts();
                break;
        }
    }

    /// <summary>
    /// 승진 확인 메뉴 텍스트 정보 업데이트
    /// </summary>
    private void UpdateCheckMenuTexts()
    {
        string totalprice = string.Format("{0:#,##0}", totalPrice);
        elevatePrice_text.text = "승진 비용: " + totalprice + "$";
        string addpay = string.Format("{0:#,##0}", addedPay);
        addPay_text.text = "추가 월급: " + addpay + "$";
        numOfElevate_text.text = numofDriver + "명";
        string addedPassengerLimit = string.Format("{0:#,##0}", addedPs);
        addPS_text.text = "추가 최대 승객 수: " + addedPassengerLimit + "명";
    }

    /// <summary>
    /// 승진에 필요한 수치 데이터 업데이트 계산 
    /// </summary>
    private void UpdateValues()
    {
        totalPrice = DriversManager.pay_Drivers[rank] * 1000 * (ulong)numofDriver;
        addedPay = (DriversManager.pay_Drivers[rank + 1] - DriversManager.pay_Drivers[rank]) * (ulong)numofDriver;
        addedPs = DriversManager.passenger_Drivers[rank] * (ulong)numofDriver;
    }

    /// <summary>
    /// 승진 프로세스 적용
    /// </summary>
    private void ArrangeDrivers()
    {
        drivers_Manager.numOfDrivers[rank] -= numofDriver;
        drivers_Manager.numOfDrivers[rank + 1] += numofDriver;
        MyAsset.instance.PassengersLimitLow += addedPs;
        salaryPay_Controller.CalculateSalary();
    }
}