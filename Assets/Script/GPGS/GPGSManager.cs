using System.Collections;
using UnityEngine;
using GooglePlayGames;

public class GPGSManager : MonoBehaviour
{
    public static bool isFirstLoginAccess = true;
    public static GPGSManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (isFirstLoginAccess)
        {
            InitializeGPGS();
            LoginGPGS();
            //Debug.Log("Login Tried");
            isFirstLoginAccess = false;
        }
    }

    public void Login()
    {
        InitializeGPGS();
        LoginGPGS();
        //Debug.Log("Login Tried");
        isFirstLoginAccess = false;
    }
    /// 
    /// ���� �α��� ������ üũ
    /// 

    public void QuitGame()
    {
        LogoutGPGS();
        //LogoutMessage.SetActive(true);

        StartCoroutine(WaitLogoutGoogle());
    }
    IEnumerator WaitLogoutGoogle()
    {
        yield return new WaitForEndOfFrame();

        if (!Social.localUser.authenticated)
            Application.Quit();

        StartCoroutine(WaitLogoutGoogle());
    }

    public bool bLogin
    {
        get;
        set;
    }

    /// 
    /// GPGS�� �ʱ�ȭ �մϴ�
    /// 

    public void InitializeGPGS()
    {
        bLogin = false;

        CloudManager.Init();
        PlayGamesPlatform.Activate();
    }
    /// GPGS�� �α��� �մϴ�.

    public void LoginGPGS()
    {
        // �α����� �ȵǾ� ������

        if (!Social.localUser.authenticated)
            Social.localUser.Authenticate(LoginCallBackGPGS);

    }

    /// GPGS Login Callback
    /// 
    ///  ��� 


    public void LoginCallBackGPGS(bool result)
    {
        bLogin = result;
    }

    /// 
    /// GPGS�� �α׾ƿ� �մϴ�.

    public void LogoutGPGS()
    {
        // �α����� �Ǿ� ������
        if (Social.localUser.authenticated)
        {
            ((PlayGamesPlatform)Social.Active).SignOut();
            bLogin = false;
        }
    }
}
