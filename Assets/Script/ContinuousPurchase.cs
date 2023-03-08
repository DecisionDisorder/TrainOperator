using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ���� ���� �ý��� ó�� Ŭ����
/// </summary>
public class ContinuousPurchase : MonoBehaviour
{
    /// <summary>
    /// ���� ���� �ð�
    /// </summary>
    private float purchaseInterval;
    /// <summary>
    /// ���� �Լ� ��������Ʈ
    /// </summary>
    public delegate void PurchaseFunction();
    /// <summary>
    /// int �Ķ���� 1���� ������ ���� �Լ� ��������Ʈ
    /// </summary>
    public delegate void PurchaseFunctionWithParam(int param);

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    private int continuousCount = 0;
    /// <summary>
    /// ���� ���� ������ ����
    /// </summary>
    private bool isPurchasing = false;
    /// <summary>
    /// ���� ���� ���Ű� ���� �������� ����
    /// </summary>
    public bool isAllowedEasyPurchase;
    /// <summary>
    /// ���� �ݺ� �ڷ�ƾ
    /// </summary>
    IEnumerator purchasing;

    public MessageManager messageManager;

    /// <summary>
    /// ���� ���� ���� ����
    /// </summary>
    /// <param name="purchaseFunc">���� �Լ�</param>
    public void StartPurchase(PurchaseFunction purchaseFunc)
    {
        // ���� ���� ���Ű� ������ ��ǰ���� Ȯ��
        if(isAllowedEasyPurchase)
        {
            // ���� ���� ���� Ȱ��ȭ �� �ʱ�ȭ
            isPurchasing = true;
            continuousCount = 1;

            if (purchasing != null)
                StopCoroutine(purchasing);

            purchasing = Purchasing(purchaseFunc);
            StartCoroutine(purchasing);
        }
        else
        {
            // 1ȸ�� ����
            purchaseFunc();
        }
    }
    /// <summary>
    /// ���� ���� ���� ����
    /// </summary>
    /// <param name="purchaseFunc">���� �Լ�</param>
    /// <param name="param">int�� �Ķ���� 1</param>
    public void StartPurchase(PurchaseFunctionWithParam purchaseFunc, int param)
    {
        // ���� ���� ���Ű� ������ ��ǰ���� Ȯ��
        if (isAllowedEasyPurchase)
        {
            // ���� ���� ���� Ȱ��ȭ �� �ʱ�ȭ
            isPurchasing = true;
            continuousCount = 1;

            if (purchasing != null)
                StopCoroutine(purchasing);

            purchasing = Purchasing(purchaseFunc, param);
            StartCoroutine(purchasing);
        }
        else
        {
            // 1ȸ�� ����
            purchaseFunc(param);
        }
    }
    /// <summary>
    /// ���� ���� ���� �ߴ�
    /// </summary>
    public void StopPurchase()
    {
        continuousCount = 0;
        isPurchasing = false;
        if (purchasing != null)
            StopCoroutine(purchasing);
    }
    /// <summary>
    /// ���� ������ ����
    /// </summary>
    /// <param name="interval">������ �ð�(��)</param>
    public void SetInterval(float interval)
    {
        purchaseInterval = interval;
    }

    /// <summary>
    /// ���� ���� ���� �ݺ� �ڷ�ƾ
    /// </summary>
    /// <param name="purchaseFunc">���� �Լ�</param>
    IEnumerator Purchasing(PurchaseFunction purchaseFunc)
    {
        yield return new WaitForSeconds(purchaseInterval);

        // ��ũ�� �� �϶��� ���� ó�� ���� �ʵ��� ����
        if (!EventTriggerEx.isScroll)
        {
            messageManager.ShowMessage(continuousCount++ + "ȸ ���� �����ϼ̽��ϴ�.");
            purchaseFunc();
            if (isPurchasing)
            {
                purchasing = Purchasing(purchaseFunc);
                StartCoroutine(purchasing);
            }
        }
    }
    /// <summary>
    /// ���� ���� ���� �ݺ� �ڷ�ƾ
    /// </summary>
    /// <param name="purchaseFunc">���� �Լ�</param>
    /// <param name="param">int �� �Ķ���� 1</param>
    IEnumerator Purchasing(PurchaseFunctionWithParam purchaseFunc, int param)
    {
        yield return new WaitForSeconds(purchaseInterval);

        // ��ũ�� �� �϶��� ���� ó�� ���� �ʵ��� ����
        if (!EventTriggerEx.isScroll)
        {
            messageManager.ShowMessage(continuousCount++ + "ȸ ���� �����ϼ̽��ϴ�.");
            purchaseFunc(param);

            if (isPurchasing)
            {
                purchasing = Purchasing(purchaseFunc, param);
                StartCoroutine(purchasing);
            }
        }
    }
}

/// <summary>
/// ���� ���� ��� �������̽�
/// </summary>
public interface IContinuousPurchase
{
    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public void StartPurchase();
    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public void StopPurchase();
}