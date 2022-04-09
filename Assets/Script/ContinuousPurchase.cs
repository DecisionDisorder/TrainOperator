using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousPurchase : MonoBehaviour
{
    private float purchaseInterval;
    public delegate void PurchaseFunction();
    public delegate void PurchaseFunctionWithParam(int param);

    private int continuousCount = 0;
    private bool isPurchasing = false;
    public bool isAllowedEasyPurchase;
    IEnumerator purchasing;

    public MessageManager messageManager;

    public void StartPurchase(PurchaseFunction purchaseFunc)
    {
        if(isAllowedEasyPurchase)
        {
            isPurchasing = true;
            continuousCount = 1;

            if (purchasing != null)
                StopCoroutine(purchasing);

            purchasing = Purchasing(purchaseFunc);
            StartCoroutine(purchasing);
        }
        else
        {
            purchaseFunc();
        }
    }
    public void StartPurchase(PurchaseFunctionWithParam purchaseFunc, int param)
    {
        if (isAllowedEasyPurchase)
        {
            isPurchasing = true;
            continuousCount = 1;

            if (purchasing != null)
                StopCoroutine(purchasing);

            purchasing = Purchasing(purchaseFunc, param);
            StartCoroutine(purchasing);
        }
        else
        {
            purchaseFunc(param);
        }
    }
    public void StopPurchase()
    {
        continuousCount = 0;
        isPurchasing = false;
        if (purchasing != null)
            StopCoroutine(purchasing);
    }
    public void SetInterval(float interval)
    {
        purchaseInterval = interval;
    }

    IEnumerator Purchasing(PurchaseFunction purchaseFunc)
    {
        yield return new WaitForSeconds(purchaseInterval);

        if (!EventTriggerEx.isScroll)
        {
            messageManager.ShowMessage(continuousCount++ + "회 연속 구매하셨습니다.");
            purchaseFunc();
            if (isPurchasing)
            {
                purchasing = Purchasing(purchaseFunc);
                StartCoroutine(purchasing);
            }
        }
    }
    IEnumerator Purchasing(PurchaseFunctionWithParam purchaseFunc, int param)
    {
        yield return new WaitForSeconds(purchaseInterval);

        if (!EventTriggerEx.isScroll)
        {
            messageManager.ShowMessage(continuousCount++ + "회 연속 구매하셨습니다.");
            purchaseFunc(param);

            if (isPurchasing)
            {
                purchasing = Purchasing(purchaseFunc, param);
                StartCoroutine(purchasing);
            }
        }
    }
}

public interface IContinuousPurchase
{
    public void StartPurchase();
    public void StopPurchase();
}