using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class button_Exit : MonoBehaviour {
	public GameObject _Main_Menu;
	public GameObject _Exit;

	public static string exit_string;
	public static int Random_exit;


	public void PressKey(int nKey)
	{
		switch (nKey)
		{
			case 1://Yes
				DataManager.instance.SaveAll();
				//Application.Quit();
				GPGSManager.instance.QuitGame();
				break;

			case 2://No
				_Main_Menu.SetActive(true);
				_Exit.SetActive(false);
				break;
		}
	}

	public static void Random_Text()
	{
		Random_exit = Random.Range (1,101);
		if (Random_exit < 21) {
			exit_string = "게임을 종료하실 겁니까? 정말로요?";
		} else if (Random_exit < 41) {
			exit_string = "진짜로 게임 끄실거에요?";
		} else if (Random_exit < 61) {
			exit_string = "당신은 이게임을 끌지 말지 고민을 하게됩니다...";
		} else if (Random_exit < 81) {
			exit_string = "이 게임을 끄는 것을 후회하지 않으실 겁니까?";
		} else if (Random_exit <= 101) {
			exit_string = "게임을 빨리 깰겁니까, 끄실겁니까";
		}
	}
}
