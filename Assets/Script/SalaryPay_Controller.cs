using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 기관사 월급 지급 관리 클래스
/// </summary>
public class SalaryPay_Controller : MonoBehaviour {
    /// <summary>
    /// 월급 지급 대기 시간
    /// </summary>
	public int salaryTimer { get { return driversManager.driverData.salaryTimer; } set { driversManager.driverData.salaryTimer = value; } }

    /// <summary>
    /// 1경 미만 단위의 총 월급량
    /// </summary>
	public static ulong totalPayLow;
    /// <summary>
    /// 1경 이상 단위의 총 월급량
    /// </summary>
	public static ulong totalPayHigh;

	public MessageManager messageManager;
	public DriversManager driversManager;
    /// <summary>
    /// 월급 지급까지 남은 시간 정보 텍스트
    /// </summary>
	public Text timeleft_Text;

	void Start ()
    {
        CalculateSalary();
        StartCoroutine(Timer());
    }
    /// <summary>
    /// 월급 대기 시간 차감 코루틴
    /// </summary>
    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1);

        salaryTimer--;
        if (salaryTimer < 1)
        {
            salaryTimer = 300;
            PaySalary();
        }
        timeleft_Text.text = "남은 월급 지급시간: " + salaryTimer + "초";

        StartCoroutine(Timer());
    }
    
    /// <summary>
    /// 총 월급 계산
    /// </summary>
    public void CalculateSalary()
    {
        totalPayLow = 0;
		totalPayHigh = 0;
        for (int i = 0; i < DriversManager.pay_Drivers.Length; i++)
        {
            totalPayLow += (ulong)driversManager.numOfDrivers[i] * DriversManager.pay_Drivers[i];
        }
    }
    /// <summary>
    /// 월급 지불
    /// </summary>
	void PaySalary()
	{
        CalculateSalary();
        // 돈이 부족하더라도 강제 지불 처리 (자산 부족시 자산 0원 처리)
        if (!AssetMoneyCalculator.instance.ArithmeticOperation(totalPayLow, totalPayHigh, false))
		{
			MyAsset.instance.MoneyHigh = 0;
			MyAsset.instance.MoneyLow = 0;
		}
        // 지출 안내 메시지 출력
		string m1 = "", m2 = "";
		PlayManager.ArrangeUnit(totalPayLow, totalPayHigh, ref m1, ref m2);
		if (totalPayLow.Equals(0) && totalPayHigh.Equals(0)) 
		{
			messageManager.ShowMessage("고용된 기관사가 없어, 이번 월급지출은 0$입니다.", 2.0f);
		} 
		else 
		{
			messageManager.ShowMessage("기관사들의 월급으로 <color=red>" + m2 + m1 +"$</color>을 지불 하였습니다.", 2f);
		}

	}
}
