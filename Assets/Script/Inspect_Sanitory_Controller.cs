using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Inspect_Sanitory_Controller : MonoBehaviour {
    public MessageManager messageManager;
    public condition_Sanitory_Controller condition_Sanitory_Controller;

    public static ulong Bonus_Percentage;
	public static ulong Bonus_money;

    public int clearTime;

    public void PressKey_test(int nKey)
    {
        switch (nKey)
        {
            case 0:
                AppearGuard();
                break;
        }
    }

    void Start () {
		StartCoroutine(Timer(Random.Range (180,361)));
	}
	IEnumerator Timer(float wTime)
    {
        yield return new WaitForSeconds(wTime);
        
        AppearGuard();
        wTime = Random.Range(180, 361);

        StartCoroutine(Timer(wTime));
    }
	void AppearGuard()
	{
        condition_Sanitory_Controller.Condition = 5;
		if (condition_Sanitory_Controller.Condition == 2) {
            messageManager.ShowMessage("위생 감시원이 위생검사를 나왔습니다!");
			StartCoroutine(Penalty(4.0f));
		} else if (condition_Sanitory_Controller.Condition == 1) {
            messageManager.ShowMessage("위생 감시원이 위생검사를 나왔습니다!");
            StartCoroutine(Penalty(4.0f));
        } else if (condition_Sanitory_Controller.Condition == 4) {
			Bonus_money = 120000;
			Bonus_Percentage = 3;
            messageManager.ShowMessage("위생 감시원이 위생검사를 나왔습니다!");
            StartCoroutine(Bonus(4.0f));
        } else if (condition_Sanitory_Controller.Condition == 5) {
			Bonus_money = 250000;
			Bonus_Percentage = 5;
            messageManager.ShowMessage("위생 감시원이 위생검사를 나왔습니다!");
            StartCoroutine(Bonus(4.0f));
        }
	}

    IEnumerator Penalty(float time)
	{
        yield return new WaitForSeconds(time);

        ulong subtractedMoneyLow = 0, subtractedMoneyHigh = 0;
        string money1 = "", money2 = "";
        PlayManager.SetMoneyReward(ref subtractedMoneyLow, ref subtractedMoneyHigh, clearTime / 2);
        AssetMoneyCalculator.instance.ForceSubtract(subtractedMoneyLow, subtractedMoneyHigh);
        PlayManager.ArrangeUnit(subtractedMoneyLow, subtractedMoneyHigh, ref money1, ref money2);

        messageManager.ShowMessage("위생검사 결과가 좋지않아 벌금으로 <color=red>" + money2 + money1 + "$</color>을 지불했습니다.", 3.0f);
    }
    IEnumerator Bonus(float time)
    {
        yield return new WaitForSeconds(time);

        ulong addedMoneyLow = 0, addedMoneyHigh = 0;
        string money1 = "", money2 = "";
        PlayManager.SetMoneyReward(ref addedMoneyLow, ref addedMoneyHigh, clearTime);
        AssetMoneyCalculator.instance.ArithmeticOperation(addedMoneyLow, addedMoneyHigh, true);
        PlayManager.ArrangeUnit(addedMoneyLow, addedMoneyHigh, ref money1, ref money2);

        messageManager.ShowMessage("위생검사 결과가 양호하여 장려금 <color=green>" + money2 + money1 + "$</color>을 지급받았습니다.", 3.0f);
    }
}
