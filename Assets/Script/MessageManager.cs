using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    public Image messageBackgroundImg;
    public Text messageText;

    public GameObject popUpMessage;
    public Text popUpMessageText;
    public Animation popUpMessageAni;
    public Button popUpCloseButton;

    public Image commonCheckMenu;
    public Text checkTitleText;
    public Text checkMessageText;
    public delegate void CallBackFunc();
    private CallBackFunc checkCallBack;

    public GameObject purchaseCheckMenu;
    public Image backgroundImg;
    public Text titleText;
    public Text productText;
    public Text amountText;
    public Text priceText;
    private CallBackFunc purchaseCallback;
    private CallBackFunc cancelCallback;

    public GameObject revenueAlarmMenu;
    public Text revenueTitleText;
    public Text revenueMessageText;

    IEnumerator erase = null;

    public void ShowMessage(string msg, float time = 1.0f)
    {
        messageBackgroundImg.gameObject.SetActive(true);
        messageText.text = msg;

        if (erase != null)
            StopCoroutine(erase);
        erase = Erase(time);
        StartCoroutine(erase);
    }

    public void ShowMessage(string msg, Color backgroundColor, float time = 1.0f)
    {
        messageBackgroundImg.color = backgroundColor;
        ShowMessage(msg, time);
    }

    public void CloseMessage()
    {
        StopCoroutine(erase);
        messageBackgroundImg.gameObject.SetActive(false);
    }

    public void ShowPopupMessage(string msg)
    {
        popUpMessage.SetActive(true);
        popUpCloseButton.enabled = true;
        popUpMessageAni["PopUpMessage"].time = 0;
        popUpMessageAni["PopUpMessage"].speed = 1;
        popUpMessageAni.Play();
        popUpMessageText.text = msg;
    }

    public void ClosePopupMessage()
    {
        popUpCloseButton.enabled = false;
        popUpMessageAni["PopUpMessage"].time = 1;
        popUpMessageAni["PopUpMessage"].speed = -2;
        popUpMessageAni.Play();
        StartCoroutine(DisablePopupMessage());
    }

    public void OpenCommonCheckMenu(string title, string message, Color backgroundColor, CallBackFunc checkCallBackFunc)
    {
        checkTitleText.text = title;
        checkMessageText.text = message;
        commonCheckMenu.color = backgroundColor;
        checkCallBack = checkCallBackFunc;
        commonCheckMenu.gameObject.SetActive(true);
    }

    public void CommonCheckResult(bool confirm)
    {
        if(confirm)
        {
            checkCallBack();
        }
        commonCheckMenu.gameObject.SetActive(false);
    }

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

    public void Purchase(bool confirm)
    {
        if (confirm)
            purchaseCallback();
        else
            cancelCallback();
        purchaseCheckMenu.SetActive(false);
    }

    public void ShowRevenueReport(string title, string msg)
    {
        revenueTitleText.text = title;
        revenueMessageText.text = msg;
        revenueAlarmMenu.SetActive(true);
    }

    public void CloseRevenueReport()
    {
        revenueAlarmMenu.SetActive(false);
    }

    IEnumerator DisablePopupMessage()
    {
        while (popUpMessageAni.isPlaying)
            yield return new WaitForEndOfFrame();

        popUpMessage.SetActive(false);
    }

    IEnumerator Erase(float time)
    {
        yield return new WaitForSeconds(time);

        messageBackgroundImg.gameObject.SetActive(false);
        messageText.text = "";
    }
}
