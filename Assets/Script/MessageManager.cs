using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����ڿ��� ������ �޽��� ���� �ý���
/// </summary>
public class MessageManager : MonoBehaviour
{
    /// <summary>
    /// �⺻ �޽��� ��� �̹���
    /// </summary>
    public Image messageBackgroundImg;
    /// <summary>
    /// �⺻ �޽��� ���� �ؽ�Ʈ
    /// </summary>
    public Text messageText;

    /// <summary>
    /// �˾� �޽��� ������Ʈ
    /// </summary>
    public GameObject popUpMessage;
    /// <summary>
    /// �˾� �޽��� ���� �ؽ�Ʈ
    /// </summary>
    public Text popUpMessageText;
    /// <summary>
    /// �˾� �޽��� �ִϸ��̼� ȿ��
    /// </summary>
    public Animation popUpMessageAni;
    /// <summary>
    /// �˾� ���� ��ư
    /// </summary>
    public Button popUpCloseButton;

    /// <summary>
    /// (����) Ȯ�� �޴�(Dialog) ������Ʈ
    /// </summary>
    public Image commonCheckMenu;
    /// <summary>
    /// Ȯ�� �޴�(Dialog) ���� �ؽ�Ʈ
    /// </summary>
    public Text checkTitleText;
    /// <summary>
    /// Ȯ�� �޴�(Dialog) ���� �ؽ�Ʈ
    /// </summary>
    public Text checkMessageText;
    /// <summary>
    /// Ȯ�� �޴� �ݹ� �Լ� ��������Ʈ ����
    /// </summary>
    public delegate void CallBackFunc();
    /// <summary>
    /// Ȯ�� �޴�(Dialog) ��ư �ݹ� �Լ� ��������Ʈ
    /// </summary>
    private CallBackFunc checkCallBack;

    /// <summary>
    /// ���� ���� Ȯ�� �޴�
    /// </summary>
    public GameObject purchaseCheckMenu;
    /// <summary>
    /// ���� Ȯ�� ��� �̹���
    /// </summary>
    public Image backgroundImg;
    /// <summary>
    /// ���� Ȯ�� ���� �ؽ�Ʈ
    /// </summary>
    public Text titleText;
    /// <summary>
    /// ���� Ȯ���� ��ǰ �� �ؽ�Ʈ
    /// </summary>
    public Text productText;
    /// <summary>
    /// ������ ������ ���� �ؽ�Ʈ
    /// </summary>
    public Text amountText;
    /// <summary>
    /// ������ ������ ���� �ؽ�Ʈ
    /// </summary>
    public Text priceText;
    /// <summary>
    /// ���� �Ϸ� ó�� �ݹ� �Լ�
    /// </summary>
    private CallBackFunc purchaseCallback;
    /// <summary>
    /// ���� ��� ó�� �ݹ� �Լ�
    /// </summary>
    private CallBackFunc cancelCallback;

    /// <summary>
    /// ���� �߻� ���� �޴� ������Ʈ
    /// </summary>
    public GameObject revenueAlarmMenu;
    /// <summary>
    /// ���� �߻� ���� �޴� ���� �ؽ�Ʈ
    /// </summary>
    public Text revenueTitleText;
    /// <summary>
    /// ���� �߻� ���� ���� �ؽ�Ʈ
    /// </summary>
    public Text revenueMessageText;

    /// <summary>
    /// �޽��� ��Ȱ��ȭ ��� �ڷ�ƾ ����
    /// </summary>
    IEnumerator erase = null;

    /// <summary>
    /// ���� ��� �������� �⺻ �޽��� ���
    /// </summary>
    /// <param name="msg">�޽��� ����</param>
    /// <param name="time">���� �ð�(�⺻ 1��)</param>
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
    /// Ư�� ��� �������� �⺻ �޽��� ���
    /// </summary>
    /// <param name="msg">�޽��� ����</param>
    /// <param name="backgroundColor">��� ����</param>
    /// <param name="time">���� �ð�(�⺻ 1��)</param>
    public void ShowMessage(string msg, Color backgroundColor, float time = 1.0f)
    {
        messageBackgroundImg.color = backgroundColor;
        ShowMessage(msg, time);
    }

    /// <summary>
    /// �޽��� ��� ��Ȱ��ȭ
    /// </summary>
    public void CloseMessage()
    {
        StopCoroutine(erase);
        messageBackgroundImg.gameObject.SetActive(false);
    }

    /// <summary>
    /// �˾� �޽��� ���
    /// </summary>
    /// <param name="msg">�޽��� ����</param>
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
    /// �˾� �޽��� ��Ȱ��ȭ
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
    /// ���� Ȯ�� �޴� Ȱ��ȭ
    /// </summary>
    /// <param name="title">����</param>
    /// <param name="message">����</param>
    /// <param name="backgroundColor">��� ����</param>
    /// <param name="checkCallBackFunc">Ȯ�� �ݹ� �Լ�</param>
    public void OpenCommonCheckMenu(string title, string message, Color backgroundColor, CallBackFunc checkCallBackFunc)
    {
        checkTitleText.text = title;
        checkMessageText.text = message;
        commonCheckMenu.color = backgroundColor;
        checkCallBack = checkCallBackFunc;
        commonCheckMenu.gameObject.SetActive(true);
    }

    /// <summary>
    /// ���� Ȯ�� �޴� Ȯ�� ó��
    /// </summary>
    /// <param name="confirm">Ȯ�� ����</param>
    public void CommonCheckResult(bool confirm)
    {
        if(confirm)
        {
            checkCallBack();
        }
        commonCheckMenu.gameObject.SetActive(false);
    }

    /// <summary>
    /// ���� Ȯ�� �޴� Ȱ��ȭ
    /// </summary>
    /// <param name="title">����</param>
    /// <param name="productName">��ǰ ��</param>
    /// <param name="amount">���� ����</param>
    /// <param name="price">����</param>
    /// <param name="backgroundColor">��� ����</param>
    /// <param name="purchaseCallback">���� ó�� �ݹ� �Լ�</param>
    /// <param name="cancelCallback">���� ��� �ݹ� �Լ�</param>
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
    /// ���� Ȯ��: ���� ó��
    /// </summary>
    /// <param name="confirm">���� Ȯ�� ����</param>
    public void Purchase(bool confirm)
    {
        if (confirm)
            purchaseCallback();
        else
            cancelCallback();
        purchaseCheckMenu.SetActive(false);
    }

    /// <summary>
    /// ���� ���� ���
    /// </summary>
    /// <param name="title">����</param>
    /// <param name="msg">����</param>
    public void ShowRevenueReport(string title, string msg)
    {
        revenueTitleText.text = title;
        revenueMessageText.text = msg;
        revenueAlarmMenu.SetActive(true);
    }

    /// <summary>
    /// ���� ���� ��Ȱ��ȭ
    /// </summary>
    public void CloseRevenueReport()
    {
        revenueAlarmMenu.SetActive(false);
    }

    /// <summary>
    /// �˾� �޽��� ��Ȱ��ȭ �ִϸ��̼� ��� �ڷ�ƾ
    /// </summary>
    IEnumerator DisablePopupMessage()
    {
        while (popUpMessageAni.isPlaying)
            yield return new WaitForEndOfFrame();

        popUpMessage.SetActive(false);
    }

    /// <summary>
    /// �⺻ �޽��� ��Ȱ��ȭ ��� �ڷ�ƾ
    /// </summary>
    /// <param name="time">�޽��� ���� �ð�</param>
    IEnumerator Erase(float time)
    {
        yield return new WaitForSeconds(time);

        messageBackgroundImg.gameObject.SetActive(false);
        messageText.text = "";
    }
}
