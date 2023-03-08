using System.Collections;
using UnityEngine;
using GooglePlayGames;

/// <summary>
/// 구글 플레이 게임 서비스(Google Play Game Service) 관리 클래스
/// </summary>
public class GPGSManager : MonoBehaviour
{
    /// <summary>
    /// 첫 로그인 시도인지 여부
    /// </summary>
    public static bool isFirstLoginAccess = true;
    /// <summary>
    /// GPGS 매니저 싱글톤 인스턴스
    /// </summary>
    public static GPGSManager instance;

    /// <summary>
    /// 로그인 여부
    /// </summary>
    public bool bLogin;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 처음 로그인 시도일 때, 로그인 작업 시작
        if (isFirstLoginAccess)
        {
            Login();
        }
    }

    /// <summary>
    /// GPGS를 초기화하고 로그인 작업 시작
    /// </summary>
    public void Login()
    {
        InitializeGPGS();
        LoginGPGS();
        isFirstLoginAccess = false;
    }

    /// <summary>
    /// GPGS 로그아웃 후 게임 종료
    /// </summary>
    public void QuitGame()
    {
        LogoutGPGS();

        StartCoroutine(WaitLogoutGoogle());
    }
    /// <summary>
    /// 로그아웃이 되었을 때 게임 종료
    /// </summary>
    IEnumerator WaitLogoutGoogle()
    {
        yield return new WaitForEndOfFrame();

        if (!Social.localUser.authenticated)
            Application.Quit();

        StartCoroutine(WaitLogoutGoogle());
    }


   /// <summary>
   /// GPGS 초기화
   /// </summary>
    public void InitializeGPGS()
    {
        bLogin = false;

        CloudManager.Init();
        PlayGamesPlatform.Activate();
    }
    
    /// <summary>
    /// GPGS 로그인 시도
    /// </summary>
    public void LoginGPGS()
    {
        // 로그인이 안되어 있으면 로그인 시도
        if (!Social.localUser.authenticated)
            Social.localUser.Authenticate(LoginCallBackGPGS);

    }

    /// <summary>
    /// 로그인 시도에 대한 결과 콜백
    /// </summary>
    /// <param name="result">로그인 시도 결과</param>
    public void LoginCallBackGPGS(bool result)
    {
        bLogin = result;
    }

    /// <summary>
    /// GPGS 로그아웃
    /// </summary>
    public void LogoutGPGS()
    {
        // 로그인이 되어 있으면
        if (Social.localUser.authenticated)
        {
            ((PlayGamesPlatform)Social.Active).SignOut();
            bLogin = false;
        }
    }
}
