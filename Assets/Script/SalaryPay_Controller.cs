using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SalaryPay_Controller : MonoBehaviour {
	public int salaryTimer { get { return driversManager.driverData.salaryTimer; } set { driversManager.driverData.salaryTimer = value; } }

	public static ulong totalPayLow;
	public static ulong totalPayHigh;
	public MessageManager messageManager;
	public DriversManager driversManager;
	public Text timeleft_Text;

	void Start ()
    {
        CalculateSalary();
        //LoadTime();
        StartCoroutine(Timer());
    }
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
    
    public void CalculateSalary()
    {
		//int lowCount = 7;
        totalPayLow = 0;
		totalPayHigh = 0;
        for (int i = 0; i < DriversManager.pay_Drivers.Length; i++)
        {
            totalPayLow += (ulong)driversManager.numOfDrivers[i] * DriversManager.pay_Drivers[i];
        }
		/*for(int i = lowCount; i < Drivers_Manager.pay_Drivers.Length; i++)
        {
			totalPayHigh += (ulong)driversManager.numOfDrivers[i] * Drivers_Manager.pay_Drivers[i];
        }*/
    }
	void PaySalary()
	{
        CalculateSalary();
        if (!AssetMoneyCalculator.instance.ArithmeticOperation(totalPayLow, totalPayHigh, false))
		{
			MyAsset.instance.MoneyHigh = 0;
			MyAsset.instance.MoneyLow = 0;
		}
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
	/*
	public static void SaveTime()
	{
		PlayerPrefs.SetInt ("Salary_nextTime",save_nextTime);
	}

	void LoadTime()
	{
		save_nextTime = PlayerPrefs.GetInt("Salary_nextTime");
	}

	public static void ResetTime()
	{
		save_nextTime = 123;
	}*/
}
