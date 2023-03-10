using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 간편 연속 구매 시스템 처리 클래스
/// </summary>
public class ContinuousPurchase : MonoBehaviour
{
    /// <summary>
    /// 구매 간격 시간
    /// </summary>
    private float purchaseInterval;
    /// <summary>
    /// 구매 함수 델리게이트
    /// </summary>
    public delegate void PurchaseFunction();
    /// <summary>
    /// int 파라미터 1개를 포함한 구매 함수 델리게이트
    /// </summary>
    public delegate void PurchaseFunctionWithParam(int param);

    /// <summary>
    /// 연속 구매 수량
    /// </summary>
    private int continuousCount = 0;
    /// <summary>
    /// 연속 구매 중인지 여부
    /// </summary>
    private bool isPurchasing = false;
    /// <summary>
    /// 간편 연속 구매가 적용 가능한지 여부
    /// </summary>
    public bool isAllowedEasyPurchase;
    /// <summary>
    /// 구매 반복 코루틴
    /// </summary>
    IEnumerator purchasing;

    public MessageManager messageManager;

    /// <summary>
    /// 간편 연속 구매 시작
    /// </summary>
    /// <param name="purchaseFunc">구매 함수</param>
    public void StartPurchase(PurchaseFunction purchaseFunc)
    {
        // 간편 연속 구매가 가능한 상품인지 확인
        if(isAllowedEasyPurchase)
        {
            // 간편 연속 구매 활성화 및 초기화
            isPurchasing = true;
            continuousCount = 1;

            if (purchasing != null)
                StopCoroutine(purchasing);

            purchasing = Purchasing(purchaseFunc);
            StartCoroutine(purchasing);
        }
        else
        {
            // 1회성 구매
            purchaseFunc();
        }
    }
    /// <summary>
    /// 간편 연속 구매 시작
    /// </summary>
    /// <param name="purchaseFunc">구매 함수</param>
    /// <param name="param">int형 파라미터 1</param>
    public void StartPurchase(PurchaseFunctionWithParam purchaseFunc, int param)
    {
        // 간편 연속 구매가 가능한 상품인지 확인
        if (isAllowedEasyPurchase)
        {
            // 간편 연속 구매 활성화 및 초기화
            isPurchasing = true;
            continuousCount = 1;

            if (purchasing != null)
                StopCoroutine(purchasing);

            purchasing = Purchasing(purchaseFunc, param);
            StartCoroutine(purchasing);
        }
        else
        {
            // 1회성 구매
            purchaseFunc(param);
        }
    }
    /// <summary>
    /// 간편 연속 구매 중단
    /// </summary>
    public void StopPurchase()
    {
        continuousCount = 0;
        isPurchasing = false;
        if (purchasing != null)
            StopCoroutine(purchasing);
    }
    /// <summary>
    /// 구매 딜레이 조정
    /// </summary>
    /// <param name="interval">조정할 시간(초)</param>
    public void SetInterval(float interval)
    {
        purchaseInterval = interval;
    }

    /// <summary>
    /// 간편 연속 구매 반복 코루틴
    /// </summary>
    /// <param name="purchaseFunc">구매 함수</param>
    IEnumerator Purchasing(PurchaseFunction purchaseFunc)
    {
        yield return new WaitForSeconds(purchaseInterval);

        // 스크롤 중 일때는 구매 처리 하지 않도록 방지
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
    /// <summary>
    /// 간편 연속 구매 반복 코루틴
    /// </summary>
    /// <param name="purchaseFunc">구매 함수</param>
    /// <param name="param">int 형 파라미터 1</param>
    IEnumerator Purchasing(PurchaseFunctionWithParam purchaseFunc, int param)
    {
        yield return new WaitForSeconds(purchaseInterval);

        // 스크롤 중 일때는 구매 처리 하지 않도록 방지
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

/// <summary>
/// 간편 구매 기능 인터페이스
/// </summary>
public interface IContinuousPurchase
{
    /// <summary>
    /// 간편 구매 시작
    /// </summary>
    public void StartPurchase();
    /// <summary>
    /// 간편 구매 종료
    /// </summary>
    public void StopPurchase();
}