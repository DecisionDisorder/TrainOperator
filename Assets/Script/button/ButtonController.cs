using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 최상위 메뉴 컨트롤러 클래스
/// </summary>
public class ButtonController : MonoBehaviour {

    /// <summary>
    /// 좌측 메인 메뉴 On/Off 애니메이션
    /// </summary>
    public Animation MainMenu_ani;
    /// <summary>
    /// 우측 조회 메뉴 On/Off 애니메이션
    /// </summary>
    public Animation AssetMenu_ani;
    /// <summary>
    /// 메인 메뉴의 RectTransform
    /// </summary>
    public RectTransform MainMenu;
    /// <summary>
    /// 조회 메뉴의 RectTransform
    /// </summary>
    public RectTransform AssetMenu;

    /// <summary>
    /// 게임 종료 안내 창 오브젝트
    /// </summary>
	public GameObject Exit;
    /// <summary>
    /// 설정 메뉴 오브젝트
    /// </summary>
	public GameObject Option_Menu;
    /// <summary>
    /// 도움말 메뉴 오브젝트
    /// </summary>
	public GameObject Help_Menu;
    
    /// <summary>
    /// 게임 안내 메시지 관련 매니저
    /// </summary>
    public MessageManager messageManager;
    
    public ButtonColor_Controller buttonColor_Controller;
    public ButtonColor_Controller3 buttonColor_Controller3;
    
    /// <summary>
    /// 버튼 소리를 재생하는 오디오 소스
    /// </summary>
    public AudioSource buttonsound_source;
    /// <summary>
    /// 버튼 소리 리소스 오디오 클립
    /// </summary>
    public AudioClip butonsound_clip;
	
    /// <summary>
    /// 게임 종료 메시지 텍스트
    /// </summary>
	public Text Exit_text;

    public ColorUpdater colorUpdater;

    void Start()
    {
        // 화면 꺼짐 방지
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // 뒤로가기 키 감지 시작
        StartCoroutine(BackKey());
    }

    /// <summary>
    /// 뒤로가기 키 입력 감지 시, 게임 종료 안내 메시지 출력 코루틴
    /// </summary>
    IEnumerator BackKey()
    {
        yield return new WaitForEndOfFrame();
        if (Input.GetKey(KeyCode.Escape))
        {
            Exit_text.text = button_Exit.RandomText();
            Exit.SetActive(true);
            Option_Menu.SetActive(false);
            StartCoroutine(BackKeyDelay(0.3f));
        }
        else
            StartCoroutine(BackKey());
    }

    /// <summary>
    /// 뒤로가기 키가 감지되었을 때 다시 감지하기까지의 딜레이 코루틴
    /// </summary>
    /// <param name="delay">감지를 지연할 시간</param>
    IEnumerator BackKeyDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        StartCoroutine(BackKey());
    }

    /// <summary>
    /// 메인메뉴 및 조회메뉴를 활성화/비활성화 하는 버튼 리스너
    /// </summary>
    /// <param name="nkey"></param>
    public void OpenMenu(int nkey)
    {
        switch (nkey)
        {
            case 0:
                MainMenu_ani.Play();
                break;
            case 1:
                MainMenu_ani.CrossFade("MainMenuClose_ani");
                StartCoroutine(CloseMainMenu());
                break;
            case 2:
                AssetMenu_ani.Play();
                break;
            case 3:
                AssetMenu_ani.CrossFade("AssetMenuClose_ani");
                StartCoroutine(CloseAssetMenu());
                break;
        }

    }

    /// <summary>
    /// 우측의 기타 메뉴 버튼에 대한 이벤트 리스너
    /// </summary>
    /// <param name="nKey"></param>
	public void PressKey(int nKey)
	{
        switch (nKey)
        {
            case 2:
                DataManager.instance.SaveAll();
                messageManager.ShowMessage("게임이 저장되었습니다.");
                break;
            case 3:
                Option_Menu.SetActive(true);
                buttonsound_source.PlayOneShot(butonsound_clip);
                break;
            case 14://Help
                Help_Menu.SetActive(true);
                buttonsound_source.PlayOneShot(butonsound_clip);
                break;

        }
	}

    /// <summary>
    /// 메인 메뉴 위치 보정
    /// </summary>
    IEnumerator CloseMainMenu()
    {
        yield return new WaitForSeconds(0.4f);

        MainMenu.anchoredPosition = new Vector2(-270, -90);
    }

    /// <summary>
    /// 조회 메뉴 위치 보정
    /// </summary>
    IEnumerator CloseAssetMenu()
    {
        yield return new WaitForSeconds(0.4f);

        AssetMenu.anchoredPosition = new Vector2(270, -90);
    }
}
