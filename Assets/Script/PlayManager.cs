using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 공통적으로 관리가 필요한 코드를 모아놓은 클래스
/// </summary>
public class PlayManager : MonoBehaviour
{
    /// <summary>
    /// 공통 관리 싱글톤 인스턴스
    /// </summary>
    public static PlayManager instance;

    /// <summary>
    /// 게임 플레이 데이터 오브젝트
    /// </summary>
    public PlayData playData;

    public MessageManager messageManager;

    /// <summary>
    /// 1초당 평균 터치 횟수
    /// </summary>
    public int averageTouch;

    public delegate void AfterAnimationPlay();

    private void Awake()
    {
        instance = this;
    }
    /// <summary>
    /// 클리어 시간 기준 보상 처리
    /// </summary>
    /// <param name="rewardLow">1경 미만 단위의 보상금</param>
    /// <param name="rewardHigh">1경 이상 단위의 보상금</param>
    /// <param name="clearTime">클리어 기준 시간</param>
    public static void SetMoneyReward(ref ulong rewardLow, ref ulong rewardHigh, int clearTime)
    {
        TouchMoneyManager.Multiply(instance.averageTouch * clearTime, ref rewardLow, ref rewardHigh);
    }

    /// <summary>
    /// 입력한 값의 가장 큰 단위에서의 문자화 정보 리턴
    /// </summary>
    /// <param name="lowValue">1경 미만 단위의 수치</param>
    /// <param name="highValue">1경 이상 단위의 수치</param>
    /// <returns></returns>
    public static string GetSimpleUnit(ulong lowValue, ulong highValue)
    {
        string result = "";
        if (lowValue == 0 && highValue == 0)
            result = "0";
        else
        {
            // 1경 이상 단위가 없을 때
            if(highValue == 0)
            {
                if (lowValue < 10000)
                    result = lowValue.ToString();
                else if (lowValue < 100000000)
                    result = (lowValue / 10000) + "만";
                else if (lowValue < 1000000000000)
                    result = (lowValue / 100000000) + "억";
                else
                    result = (lowValue / 1000000000000) + "조";
            }
            // 1경 이상 단위가 있을 때
            else
            {
                if (highValue < 10000)
                    result = highValue + "경";
                else if (highValue < 100000000)
                    result = (highValue / 10000) + "해";
                else if (highValue < 1000000000000)
                    result = (highValue / 100000000) + "자";
                else
                    result = (highValue / 1000000000000) + "양";
            }
        }

        return result;
    }

    /// <summary>
    /// 입력한 값의 문자화 정보 리턴
    /// </summary>
    /// <param name="lowValue">1경 미만 단위의 수치</param>
    /// <param name="highValue">1경 이상 단위의 수치</param>
    /// <param name="resultLow">문자화 된 1경 미만의 단위</param>
    /// <param name="resultHigh">문자화 된 1경 이상의 단위</param>
    /// <param name="detailed">자세히 표기할지 여부</param>
    public static void ArrangeUnit(ulong lowValue, ulong highValue, ref string resultLow, ref string resultHigh, bool detailed = false)
    {
        if (lowValue == 0)
        {
            if (highValue.Equals(0))
                resultLow = "0";
            else
                resultLow = "";
        }
        else if (lowValue < 10000)
        {
            resultLow = string.Format("{0:#,##0}", lowValue);
        }
        else if (lowValue < 100000000)
        {
            if ((lowValue % 10000).Equals(0))
            {
                resultLow = string.Format("{0:####만}", lowValue / 10000);
            }
            else
            {
                resultLow = string.Format("{0:####만###0}", lowValue);
            }
        }
        else if (lowValue < 1000000000000)
        {
            ulong downmoney = (ulong)(lowValue * 0.0001);
            if ((downmoney % 10000).Equals(0))
            {
                resultLow = string.Format("{0:####억}", downmoney / 10000);
            }
            else
            {
                resultLow = string.Format("{0:####억####만}", downmoney);
            }

        }
        else
        {
            if (!detailed)
            {
                ulong downmoney = (ulong)(lowValue / 1000000000000);
                if (downmoney < 10000)
                {
                    resultLow = string.Format("{0:####조}", downmoney);
                }
                else
                {
                    highValue += (ulong)(downmoney / 10000);
                    downmoney %= 10000;
                    if (downmoney.Equals(0))
                    {
                        resultLow = "";
                    }
                    else
                    {
                        resultLow = string.Format("{0:####조}", downmoney);
                    }
                }
            }
            else
            {
                ulong downmoney = (ulong)(lowValue / 100000000);
                if (downmoney < 100000000)
                {
                    if (downmoney % 10000 == 0)
                        resultLow = string.Format("{0:####조}", downmoney / 10000);
                    else
                        resultLow = string.Format("{0:####조####억}", downmoney);
                }
                else
                {
                    highValue += (ulong)(downmoney / 100000000);
                    downmoney %= 100000000;
                    if (downmoney.Equals(0))
                    {
                        resultLow = "";
                    }
                    else
                    {
                        if(downmoney % 10000 == 0)
                            resultLow = string.Format("{0:####조}", downmoney / 10000);
                        else
                            resultLow = string.Format("{0:####조####억}", downmoney);
                    }
                }
            }
        }
        if (highValue.Equals(0))
        {
            resultHigh = "";
        }
        else if (highValue < 10000)
        {
            resultHigh = string.Format("{0:####경}", highValue);
        }
        else if(highValue < 100000000)
        {
            if((highValue % 10000).Equals(0))
                resultHigh = string.Format("{0:####해}", highValue / 10000);
            else 
                resultHigh = string.Format("{0:####해####경}", highValue);
        }
        else
        {
            if((highValue % 100000000).Equals(0))
                resultHigh = string.Format("{0:####자}", highValue / 100000000);
            else if((highValue % 10000).Equals(0))
                resultHigh = string.Format("{0:####자####해}", highValue / 10000);
            else
                resultHigh = string.Format("{0:####자####해####경}", highValue);
        }
    }

    /// <summary>
    /// 10경을 넘는지 확인하는 함수
    /// </summary>
    public static bool CheckLowUnitRange(ulong n1, ulong n2)
    {
        int n1Count = -1, n2Count = -1;

        for (; n1 > 0; n1 /= 10, n1Count++) { }
        for (; n2 > 0; n2 /= 10, n2Count++) { }


        if (n1Count + n2Count > 16)
            return false;
        else
            return true;
    }

    /// <summary>
    /// 애니메이션 대기 후 콜백 함수 호출
    /// </summary>
    /// <param name="animation">재생 중인 애니메이션</param>
    /// <param name="afterAnimation">콜백함수</param>
    public void WaitAnimation(Animation animation, AfterAnimationPlay afterAnimation)
    {
        StartCoroutine(WaitforAnimation(animation, afterAnimation));
    }
    /// <summary>
    /// 애니메이션 대기 후 콜백 함수 호출하는 코루틴
    /// </summary>
    /// <param name="animation">재생중인 애니메이션</param>
    /// <param name="afterAnimation">콜백 함수</param>
    IEnumerator WaitforAnimation(Animation animation, AfterAnimationPlay afterAnimation)
    {
        while (animation.isPlaying)
            yield return new WaitForEndOfFrame();

        afterAnimation();
    }

    /// <summary>
    /// 보유 중인 돈이 부족함을 알림
    /// </summary>
    public void LackOfMoney()
    {
        messageManager.ShowMessage("돈이 부족합니다.\n터치를 통해 돈을 더 모아주세요!");
    }

    /// <summary>
    /// 승객 수 제한이 부족함을 알림
    /// </summary>
    public void LackOfPassengerLimit()
    {
        messageManager.ShowMessage("감당할수 있는 승객을 초과했습니다.\n기관사를 고용해서 제한을 늘려주세요!", 1.5f);
    }

    /// <summary>
    /// 차량기지가 부족함을 알림
    /// </summary>
    public void LackOfVehicleBase()
    {
        messageManager.ShowMessage("차량기지에 보관할 수 있는 공간이 부족합니다.\n차량기지를 추가해주세요!");
    }
}
