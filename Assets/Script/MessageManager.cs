using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    public GameObject _Message;
    public Text Normal_Message;

    public GameObject popUpMessage;
    public Text popUpMessageText;
    public Animation popUpMessageAni;
    public Button popUpCloseButton;

    public Image commonCheckMenu;
    public Text checkTitleText;
    public Text checkMessageText;
    public delegate void CheckCallBack();
    private CheckCallBack checkCallBack;

    IEnumerator erase = null;

    public void ShowMessage(string msg, float time = 1.0f)
    {
        _Message.SetActive(true);
        Normal_Message.text = msg;

        if (erase != null)
            StopCoroutine(erase);
        erase = Erase(time);
        StartCoroutine(erase);
    }

    public void CloseMessage()
    {
        StopCoroutine(erase);
        _Message.SetActive(false);
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

    public void OpenCommonCheckMenu(string title, string message, Color backgroundColor, CheckCallBack checkCallBackFunc)
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

    IEnumerator DisablePopupMessage()
    {
        while (popUpMessageAni.isPlaying)
            yield return new WaitForEndOfFrame();

        popUpMessage.SetActive(false);
    }

    IEnumerator Erase(float time)
    {
        yield return new WaitForSeconds(time);

        _Message.SetActive(false);
        Normal_Message.text = "";
    }
}
