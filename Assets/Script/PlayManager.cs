using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���������� ������ �ʿ��� �ڵ带 ��Ƴ��� Ŭ����
/// </summary>
public class PlayManager : MonoBehaviour
{
    /// <summary>
    /// ���� ���� �̱��� �ν��Ͻ�
    /// </summary>
    public static PlayManager instance;

    /// <summary>
    /// ���� �÷��� ������ ������Ʈ
    /// </summary>
    public PlayData playData;

    public MessageManager messageManager;

    /// <summary>
    /// 1�ʴ� ��� ��ġ Ƚ��
    /// </summary>
    public int averageTouch;

    public delegate void AfterAnimationPlay();

    private void Awake()
    {
        instance = this;
    }
    /// <summary>
    /// Ŭ���� �ð� ���� ���� ó��
    /// </summary>
    /// <param name="rewardLow">1�� �̸� ������ �����</param>
    /// <param name="rewardHigh">1�� �̻� ������ �����</param>
    /// <param name="clearTime">Ŭ���� ���� �ð�</param>
    public static void SetMoneyReward(ref ulong rewardLow, ref ulong rewardHigh, int clearTime)
    {
        TouchMoneyManager.Multiply(instance.averageTouch * clearTime, ref rewardLow, ref rewardHigh);
    }

    /// <summary>
    /// �Է��� ���� ���� ū ���������� ����ȭ ���� ����
    /// </summary>
    /// <param name="lowValue">1�� �̸� ������ ��ġ</param>
    /// <param name="highValue">1�� �̻� ������ ��ġ</param>
    /// <returns></returns>
    public static string GetSimpleUnit(ulong lowValue, ulong highValue)
    {
        string result = "";
        if (lowValue == 0 && highValue == 0)
            result = "0";
        else
        {
            // 1�� �̻� ������ ���� ��
            if(highValue == 0)
            {
                if (lowValue < 10000)
                    result = lowValue.ToString();
                else if (lowValue < 100000000)
                    result = (lowValue / 10000) + "��";
                else if (lowValue < 1000000000000)
                    result = (lowValue / 100000000) + "��";
                else
                    result = (lowValue / 1000000000000) + "��";
            }
            // 1�� �̻� ������ ���� ��
            else
            {
                if (highValue < 10000)
                    result = highValue + "��";
                else if (highValue < 100000000)
                    result = (highValue / 10000) + "��";
                else if (highValue < 1000000000000)
                    result = (highValue / 100000000) + "��";
                else
                    result = (highValue / 1000000000000) + "��";
            }
        }

        return result;
    }

    /// <summary>
    /// �Է��� ���� ����ȭ ���� ����
    /// </summary>
    /// <param name="lowValue">1�� �̸� ������ ��ġ</param>
    /// <param name="highValue">1�� �̻� ������ ��ġ</param>
    /// <param name="resultLow">����ȭ �� 1�� �̸��� ����</param>
    /// <param name="resultHigh">����ȭ �� 1�� �̻��� ����</param>
    /// <param name="detailed">�ڼ��� ǥ������ ����</param>
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
                resultLow = string.Format("{0:####��}", lowValue / 10000);
            }
            else
            {
                resultLow = string.Format("{0:####��###0}", lowValue);
            }
        }
        else if (lowValue < 1000000000000)
        {
            ulong downmoney = (ulong)(lowValue * 0.0001);
            if ((downmoney % 10000).Equals(0))
            {
                resultLow = string.Format("{0:####��}", downmoney / 10000);
            }
            else
            {
                resultLow = string.Format("{0:####��####��}", downmoney);
            }

        }
        else
        {
            if (!detailed)
            {
                ulong downmoney = (ulong)(lowValue / 1000000000000);
                if (downmoney < 10000)
                {
                    resultLow = string.Format("{0:####��}", downmoney);
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
                        resultLow = string.Format("{0:####��}", downmoney);
                    }
                }
            }
            else
            {
                ulong downmoney = (ulong)(lowValue / 100000000);
                if (downmoney < 100000000)
                {
                    if (downmoney % 10000 == 0)
                        resultLow = string.Format("{0:####��}", downmoney / 10000);
                    else
                        resultLow = string.Format("{0:####��####��}", downmoney);
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
                            resultLow = string.Format("{0:####��}", downmoney / 10000);
                        else
                            resultLow = string.Format("{0:####��####��}", downmoney);
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
            resultHigh = string.Format("{0:####��}", highValue);
        }
        else if(highValue < 100000000)
        {
            if((highValue % 10000).Equals(0))
                resultHigh = string.Format("{0:####��}", highValue / 10000);
            else 
                resultHigh = string.Format("{0:####��####��}", highValue);
        }
        else
        {
            if((highValue % 100000000).Equals(0))
                resultHigh = string.Format("{0:####��}", highValue / 100000000);
            else if((highValue % 10000).Equals(0))
                resultHigh = string.Format("{0:####��####��}", highValue / 10000);
            else
                resultHigh = string.Format("{0:####��####��####��}", highValue);
        }
    }

    /// <summary>
    /// 10���� �Ѵ��� Ȯ���ϴ� �Լ�
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
    /// �ִϸ��̼� ��� �� �ݹ� �Լ� ȣ��
    /// </summary>
    /// <param name="animation">��� ���� �ִϸ��̼�</param>
    /// <param name="afterAnimation">�ݹ��Լ�</param>
    public void WaitAnimation(Animation animation, AfterAnimationPlay afterAnimation)
    {
        StartCoroutine(WaitforAnimation(animation, afterAnimation));
    }
    /// <summary>
    /// �ִϸ��̼� ��� �� �ݹ� �Լ� ȣ���ϴ� �ڷ�ƾ
    /// </summary>
    /// <param name="animation">������� �ִϸ��̼�</param>
    /// <param name="afterAnimation">�ݹ� �Լ�</param>
    IEnumerator WaitforAnimation(Animation animation, AfterAnimationPlay afterAnimation)
    {
        while (animation.isPlaying)
            yield return new WaitForEndOfFrame();

        afterAnimation();
    }

    /// <summary>
    /// ���� ���� ���� �������� �˸�
    /// </summary>
    public void LackOfMoney()
    {
        messageManager.ShowMessage("���� �����մϴ�.\n��ġ�� ���� ���� �� ����ּ���!");
    }

    /// <summary>
    /// �°� �� ������ �������� �˸�
    /// </summary>
    public void LackOfPassengerLimit()
    {
        messageManager.ShowMessage("�����Ҽ� �ִ� �°��� �ʰ��߽��ϴ�.\n����縦 ����ؼ� ������ �÷��ּ���!", 1.5f);
    }

    /// <summary>
    /// ���������� �������� �˸�
    /// </summary>
    public void LackOfVehicleBase()
    {
        messageManager.ShowMessage("���������� ������ �� �ִ� ������ �����մϴ�.\n���������� �߰����ּ���!");
    }
}
