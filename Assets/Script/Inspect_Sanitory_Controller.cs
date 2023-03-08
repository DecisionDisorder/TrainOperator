using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 위생 감사 시스템 관리 클래스
/// </summary>
public class Inspect_Sanitory_Controller : MonoBehaviour {
    public MessageManager messageManager;
    public condition_Sanitory_Controller condition_Sanitory_Controller;

    /// <summary>
    /// 위생 감사 남은 시간
    /// </summary>
    public int InspectSanitoryTimer { get { return companyReputationManager.companyData.inspectSanitoryTime; } set { companyReputationManager.companyData.inspectSanitoryTime = value; } }

    /// <summary>
    /// 보상 계산 기준 소요 시간
    /// </summary>
    public int clearTime;
    public CompanyReputationManager companyReputationManager;

    /// <summary>
    /// 위생 감사 테스트 버튼
    /// </summary>
    public void PressKey_test(int nKey)
    {
        switch (nKey)
        {
            case 0:
                Inspect();
                break;
        }
    }

    void Start () {
        if(InspectSanitoryTimer == 0)
            InspectSanitoryTimer = Random.Range(240, 300);

        StartCoroutine(Timer());
	}
    /// <summary>
    /// 위생 감사 남은 시간 차감 코루틴
    /// </summary>
    /// <param name="delay">차감 간격</param>
	IEnumerator Timer(int delay = 1)
    {
        yield return new WaitForSeconds(delay);

        InspectSanitoryTimer -= delay;

        // 차감된 시간이 0이 되면 남은 시간 초기화 후 위생 감사 시작
        if(InspectSanitoryTimer <= 0)
        {
            InspectSanitoryTimer = Random.Range(240, 300);
            Inspect();
        }

        StartCoroutine(Timer(delay));
    }
    /// <summary>
    /// 위생 감사 시작
    /// </summary>
	void Inspect()
    {
        messageManager.ShowMessage("위생 감시원이 위생검사를 나왔습니다!");

        // 위생도 단계 별 패널티/보상 차등 지급 
        if (condition_Sanitory_Controller.Condition == 1)
        {
            StartCoroutine(Penalty(3.0f, 1.5f));
        }
        else if (condition_Sanitory_Controller.Condition == 2)
        {
            StartCoroutine(Penalty(3.0f, 1.0f));
        }
        else if(condition_Sanitory_Controller.Condition == 3)
        {
            StartCoroutine(Bonus(3.0f, 0.5f));
        }
        else if (condition_Sanitory_Controller.Condition == 4)
        {
            StartCoroutine(Bonus(3.0f, 1.0f));
        }
        else if (condition_Sanitory_Controller.Condition == 5)
        {
            StartCoroutine(Bonus(3.0f, 1.5f));
        }
	}

    /// <summary>
    /// 일정 시간이 지난 후 정해진 수준 만큼 자산 차감 패널티 계산 코루틴
    /// </summary>
    /// <param name="time">대기 시간</param>
    /// <param name="penalty">패널티 배율</param>
    IEnumerator Penalty(float time, float penalty)
	{
        yield return new WaitForSeconds(time);

        ulong subtractedMoneyLow = 0, subtractedMoneyHigh = 0;
        string money1 = "", money2 = "";
        PlayManager.SetMoneyReward(ref subtractedMoneyLow, ref subtractedMoneyHigh, (int)(clearTime / 3 * penalty));
        AssetMoneyCalculator.instance.ForceSubtract(subtractedMoneyLow, subtractedMoneyHigh);
        PlayManager.ArrangeUnit(subtractedMoneyLow, subtractedMoneyHigh, ref money1, ref money2);

        if(penalty <= 1.0f)
            messageManager.ShowMessage("위생검사 결과가 좋지않아 벌금으로 <color=red>" + money2 + money1 + "$</color>을 지불했습니다.", 3.0f);
        else
            messageManager.ShowMessage("위생검사 결과가 매우 좋지않아 벌금으로 <color=red>" + money2 + money1 + "$</color>을 지불했습니다.", 3.0f);
    }
    /// <summary>
    /// 일정 시간이 지난 후 정해진 수준 만큼의 보상 지급 코루틴
    /// </summary>
    /// <param name="time">대기 시간</param>
    /// <param name="bonus">보상 배율</param>
    /// <returns></returns>
    IEnumerator Bonus(float time, float bonus)
    {
        yield return new WaitForSeconds(time);

        ulong addedMoneyLow = 0, addedMoneyHigh = 0;
        string money1 = "", money2 = "";
        PlayManager.SetMoneyReward(ref addedMoneyLow, ref addedMoneyHigh, (int)(clearTime * bonus));
        AssetMoneyCalculator.instance.ArithmeticOperation(addedMoneyLow, addedMoneyHigh, true);
        PlayManager.ArrangeUnit(addedMoneyLow, addedMoneyHigh, ref money1, ref money2);

        if(bonus < 1f)
            messageManager.ShowMessage("위생검사 결과가 기준에 부합하여 장려금 <color=green>" + money2 + money1 + "$</color>을 지급받았습니다.", 3.0f);
        else if(bonus == 1f)
            messageManager.ShowMessage("위생검사 결과가 양호하여 장려금 <color=green>" + money2 + money1 + "$</color>을 지급받았습니다.", 3.0f);
        else
            messageManager.ShowMessage("위생검사 결과가 매우 양호하여 장려금 <color=green>" + money2 + money1 + "$</color>을 지급받았습니다.", 3.0f);
    }
}
