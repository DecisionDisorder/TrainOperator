using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {

    public Animation MainMenu_ani;
    public Animation AssetMenu_ani;
    public RectTransform MainMenu;
    public RectTransform AssetMenu;

	public GameObject Exit;
	public GameObject Option_Menu;
	public GameObject Help_Menu;
    //-----------------------------------------------------------------------------
    public MessageManager messageManager;
    //public OpeningCardPack openingCardPack;
    public ButtonColor_Controller buttonColor_Controller;
    public ButtonColor_Controller3 buttonColor_Controller3;
    //-----------------------------------------------------------------------------
    public AudioSource buttonsound_source;
    public AudioClip butonsound_clip;
	
	public Text Exit_text;

    public ColorUpdater colorUpdater;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;//화면꺼짐방지
        StartCoroutine(BackKey());
    }

    IEnumerator BackKey()
    {
        yield return new WaitForEndOfFrame();
        if (Input.GetKey(KeyCode.Escape))
        {
            button_Exit.RandomText();
            Exit_text.text = button_Exit.exitMessage;
            Exit.SetActive(true);
            Option_Menu.SetActive(false);
            StartCoroutine(BackKeyDelay(0.3f));
        }
        else
            StartCoroutine(BackKey());
    }
    IEnumerator BackKeyDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        StartCoroutine(BackKey());
    }
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
    IEnumerator CloseMainMenu()
    {
        yield return new WaitForSeconds(0.4f);

        MainMenu.anchoredPosition = new Vector2(-270, -90);
    }
    IEnumerator CloseAssetMenu()
    {
        yield return new WaitForSeconds(0.4f);

        AssetMenu.anchoredPosition = new Vector2(270, -90);
    }
}
