using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 사용자에게 전달할 메시지 관리 시스템
/// </summary>
public class MessageManager : MonoBehaviour
{
    /// <summary>
    /// 기본 메시지 배경 이미지
    /// </summary>
    public Image messageBackgroundImg;
    /// <summary>
    /// 기본 메시지 내용 텍스트
    /// </summary>
    public Text messageText;

    /// <summary>
    /// 팝업 메시지 오브젝트
    /// </summary>
    public GameObject popUpMessage;
    /// <summary>
    /// 팝업 메시지 내용 텍스트
    /// </summary>
    public Text popUpMessageText;
    /// <summary>
    /// 팝업 메시지 애니메이션 효과
    /// </summary>
    public Animation popUpMessageAni;
    /// <summary>
    /// 팝업 종료 버튼
    /// </summary>
    public Button popUpCloseButton;

    /// <summary>
    /// (공용) 확인 메뉴(Dialog) 오브젝트
    /// </summary>
    public Image commonCheckMenu;
    /// <summary>
    /// 확인 메뉴(Dialog) 제목 텍스트
    /// </summary>
    public Text checkTitleText;
    /// <summary>
    /// 확인 메뉴(Dialog) 내용 텍스트
    /// </summary>
    public Text checkMessageText;
    /// <summary>
    /// 확인 메뉴 콜백 함수 델리게이트 원형
    /// </summary>
    public delegate void CallBackFunc();
    /// <summary>
    /// 확인 메뉴(Dialog) 버튼 콜백 함수 델리데이트
    /// </summary>
    private CallBackFunc checkCallBack;

    /// <summary>
    /// 공용 구매 확인 메뉴
    /// </summary>
    public GameObject purchaseCheckMenu;
    /// <summary>
    /// 구매 확인 배경 이미지
    /// </summary>
    public Image backgroundImg;
    /// <summary>
    /// 구매 확인 제목 텍스트
    /// </summary>
    public Text titleText;
    /// <summary>
    /// 구매 확인할 제품 명 텍스트
    /// </summary>
    public Text productText;
    /// <summary>
    /// 구매할 아이템 수량 텍스트
    /// </summary>
    public Text amountText;
    /// <summary>
    /// 구매할 아이템 가격 텍스트
    /// </summary>
    public Text priceText;
    /// <summary>
    /// 구매 완료 처리 콜백 함수
    /// </summary>
    private CallBackFunc purchaseCallback;
    /// <summary>
    /// 구매 취소 처리 콜백 함수
    /// </summary>
    private CallBackFunc cancelCallback;

    /// <summary>
    /// 수익 발생 보고 메뉴 오브젝트
    /// </summary>
    public GameObject revenueAlarmMenu;
    /// <summary>
    /// 수익 발생 보고 메뉴 제목 텍스트
    /// </summary>
    public Text revenueTitleText;
    /// <summary>
    /// 수익 발생 보고 내용 텍스트
    /// </summary>
    public Text revenueMessageText;

    /// <summary>
    /// 메시지 비활성화 대기 코루틴 변수
    /// </summary>
    IEnumerator erase = null;

    /// <summary>
    /// 기존 배경 색상으로 기본 메시지 출력
    /// </summary>
    /// <param name="msg">메시지 내용</param>
    /// <param name="time">지속 시간(기본 1초)</param>
    public void ShowMessage(string msg, float time = 1.0f)
    {
        messageBackgroundImg.gameObject.SetActive(true);
        messageText.text = msg;

        if (erase != null)
            StopCoroutine(erase);
        erase = Erase(time);
        StartCoroutine(erase);
    }

    /// <summary>
    /// 특정 배경 색상으로 기본 메시지 출력
    /// </summary>
    /// <param name="msg">메시지 내용</param>
    /// <param name="backgroundColor">배경 색상</param>
    /// <param name="time">지속 시간(기본 1초)</param>
    public void ShowMessage(string msg, Color backgroundColor, float time = 1.0f)
    {
        messageBackgroundImg.color = backgroundColor;
        ShowMessage(msg, time);
    }

    /// <summary>
    /// 메시지 즉시 비활성화
    /// </summary>
    public void CloseMessage()
    {
        StopCoroutine(erase);
        messageBackgroundImg.gameObject.SetActive(false);
    }

    /// <summary>
    /// 팝업 메시지 출력
    /// </summary>
    /// <param name="msg">메시지 내용</param>
    public void ShowPopupMessage(string msg)
    {
        popUpMessage.SetActive(true);
        popUpCloseButton.enabled = true;
        popUpMessageAni["PopUpMessage"].time = 0;
        popUpMessageAni["PopUpMessage"].speed = 1;
        popUpMessageAni.Play();
        popUpMessageText.text = msg;
    }

    /// <summary>
    /// 팝업 메시지 비활성화
    /// </summary>
    public void ClosePopupMessage()
    {
        popUpCloseButton.enabled = false;
        popUpMessageAni["PopUpMessage"].time = 1;
        popUpMessageAni["PopUpMessage"].speed = -2;
        popUpMessageAni.Play();
        StartCoroutine(DisablePopupMessage());
    }

    /// <summary>
    /// 공용 확인 메뉴 활성화
    /// </summary>
    /// <param name="title">제목</param>
    /// <param name="message">내용</param>
    /// <param name="backgroundColor">배경 색상</param>
    /// <param name="checkCallBackFunc">확인 콜백 함수</param>
    public void OpenCommonCheckMenu(string title, string message, Color backgroundColor, CallBackFunc checkCallBackFunc)
    {
        checkTitleText.text = title;
        checkMessageText.text = message;
        commonCheckMenu.color = backgroundColor;
        checkCallBack = checkCallBackFunc;
        commonCheckMenu.gameObject.SetActive(true);
    }

    /// <summary>
    /// 공용 확인 메뉴 확인 처리
    /// </summary>
    /// <param name="confirm">확인 여부</param>
    public void CommonCheckResult(bool confirm)
    {
        if(confirm)
        {
            checkCallBack();
        }
        commonCheckMenu.gameObject.SetActive(false);
    }

    /// <summary>
    /// 구매 확인 메뉴 활성화
    /// </summary>
    /// <param name="title">제목</param>
    /// <param name="productName">제품 명</param>
    /// <param name="amount">구매 수량</param>
    /// <param name="price">가격</param>
    /// <param name="backgroundColor">배경 색상</param>
    /// <param name="purchaseCallback">구매 처리 콜백 함수</param>
    /// <param name="cancelCallback">구매 취소 콜백 함수</param>
    public void SetPurchaseCheckMenu(string title, string productName, string amount, string price, Color backgroundColor, CallBackFunc purchaseCallback, CallBackFunc cancelCallback)
    {
        titleText.text = title;
        productText.text = productName;
        amountText.text = amount;
        priceText.text = price;
        backgroundImg.color = backgroundColor;
        this.purchaseCallback = purchaseCallback;
        this.cancelCallback = cancelCallback;
        purchaseCheckMenu.SetActive(true);
    }

    /// <summary>
    /// 구매 확인: 구매 처리
    /// </summary>
    /// <param name="confirm">구매 확인 여부</param>
    public void Purchase(bool confirm)
    {
        if (confirm)
            purchaseCallback();
        else
            cancelCallback();
        purchaseCheckMenu.SetActive(false);
    }

    /// <summary>
    /// 수익 보고서 출력
    /// </summary>
    /// <param name="title">제목</param>
    /// <param name="msg">내용</param>
    public void ShowRevenueReport(string title, string msg)
    {
        revenueTitleText.text = title;
        revenueMessageText.text = msg;
        revenueAlarmMenu.SetActive(true);
    }

    /// <summary>
    /// 수익 보고서 비활성화
    /// </summary>
    public void CloseRevenueReport()
    {
        revenueAlarmMenu.SetActive(false);
    }

    /// <summary>
    /// 팝업 메시지 비활성화 애니메이션 대기 코루틴
    /// </summary>
    IEnumerator DisablePopupMessage()
    {
        while (popUpMessageAni.isPlaying)
            yield return new WaitForEndOfFrame();

        popUpMessage.SetActive(false);
    }

    /// <summary>
    /// 기본 메시지 비활성화 대기 코루틴
    /// </summary>
    /// <param name="time">메시지 지속 시간</param>
    IEnumerator Erase(float time)
    {
        yield return new WaitForSeconds(time);

        messageBackgroundImg.gameObject.SetActive(false);
        messageText.text = "";
    }
}
