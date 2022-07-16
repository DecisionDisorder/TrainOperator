using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour
{
    public static PlayManager instance;

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
    public static void SetMoneyReward(ref ulong rewardLow, ref ulong rewardHigh, int clearTime)
    {
        TouchMoneyManager.Multiply(instance.averageTouch * clearTime, ref rewardLow, ref rewardHigh);
    }

    public static string GetSimpleUnit(ulong lowValue, ulong highValue)
    {
        string result = "";
        if (lowValue == 0 && highValue == 0)
            result = "0";
        else
        {
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

    public void WaitAnimation(Animation animation, AfterAnimationPlay afterAnimation)
    {
        StartCoroutine(WaitforAnimation(animation, afterAnimation));
    }
    IEnumerator WaitforAnimation(Animation animation, AfterAnimationPlay afterAnimation)
    {
        while (animation.isPlaying)
            yield return new WaitForEndOfFrame();

        afterAnimation();
    }

    public void LackOfMoney()
    {
        messageManager.ShowMessage("돈이 부족합니다.\n터치를 통해 돈을 더 모아주세요!");
    }

    public void LackOfPassengerLimit()
    {
        messageManager.ShowMessage("감당할수 있는 승객을 초과했습니다.\n기관사를 고용해서 제한을 늘려주세요!", 1.5f);
    }

    public void LackOfVehicleBase()
    {
        messageManager.ShowMessage("차량기지에 보관할 수 있는 공간이 부족합니다.\n차량기지를 추가해주세요!");
    }
}
