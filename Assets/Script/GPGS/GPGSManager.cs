using System.Collections;
using UnityEngine;
using GooglePlayGames;

/// <summary>
/// ���� �÷��� ���� ����(Google Play Game Service) ���� Ŭ����
/// </summary>
public class GPGSManager : MonoBehaviour
{
    /// <summary>
    /// ù �α��� �õ����� ����
    /// </summary>
    public static bool isFirstLoginAccess = true;
    /// <summary>
    /// GPGS �Ŵ��� �̱��� �ν��Ͻ�
    /// </summary>
    public static GPGSManager instance;

    /// <summary>
    /// �α��� ����
    /// </summary>
    public bool bLogin;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // ó�� �α��� �õ��� ��, �α��� �۾� ����
        if (isFirstLoginAccess)
        {
            Login();
        }
    }

    /// <summary>
    /// GPGS�� �ʱ�ȭ�ϰ� �α��� �۾� ����
    /// </summary>
    public void Login()
    {
        InitializeGPGS();
        LoginGPGS();
        isFirstLoginAccess = false;
    }

    /// <summary>
    /// GPGS �α׾ƿ� �� ���� ����
    /// </summary>
    public void QuitGame()
    {
        LogoutGPGS();

        StartCoroutine(WaitLogoutGoogle());
    }
    /// <summary>
    /// �α׾ƿ��� �Ǿ��� �� ���� ����
    /// </summary>
    IEnumerator WaitLogoutGoogle()
    {
        yield return new WaitForEndOfFrame();

        if (!Social.localUser.authenticated)
            Application.Quit();

        StartCoroutine(WaitLogoutGoogle());
    }


   /// <summary>
   /// GPGS �ʱ�ȭ
   /// </summary>
    public void InitializeGPGS()
    {
        bLogin = false;

        CloudManager.Init();
        PlayGamesPlatform.Activate();
    }
    
    /// <summary>
    /// GPGS �α��� �õ�
    /// </summary>
    public void LoginGPGS()
    {
        // �α����� �ȵǾ� ������ �α��� �õ�
        if (!Social.localUser.authenticated)
            Social.localUser.Authenticate(LoginCallBackGPGS);

    }

    /// <summary>
    /// �α��� �õ��� ���� ��� �ݹ�
    /// </summary>
    /// <param name="result">�α��� �õ� ���</param>
    public void LoginCallBackGPGS(bool result)
    {
        bLogin = result;
    }

    /// <summary>
    /// GPGS �α׾ƿ�
    /// </summary>
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
