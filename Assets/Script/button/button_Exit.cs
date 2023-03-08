using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 게임 종료 메뉴 매니저 클래스
/// </summary>
public class button_Exit : MonoBehaviour {
	/// <summary>
	/// 종료 안내창 오브젝트
	/// </summary>
	public GameObject _Exit;


	/// <summary>
	/// 종료 안내창 버튼 이벤트 처리하는 함수
	/// </summary>
	/// <param name="nKey"></param>
	public void PressKey(int nKey)
	{
		switch (nKey)
		{
			case 1: // 게임 저장 후 종료
				DataManager.instance.SaveAll();
				GPGSManager.instance.QuitGame();
				break;

			case 2: // 게임 종료 취소
				_Exit.SetActive(false);
				break;
		}
	}

	/// <summary>
	/// 랜덤한 텍스트 선정
	/// </summary>
	public static string RandomText()
	{
		string exitMessage = "";
		int Random_exit = Random.Range (1,101);
		if (Random_exit < 21) {
			exitMessage = "게임을 종료하실 겁니까? 정말로요?";
		} else if (Random_exit < 41) {
			exitMessage = "진짜로 게임 끄실거에요?";
		} else if (Random_exit < 61) {
			exitMessage = "당신은 이게임을 끌지 말지 고민을 하게됩니다...";
		} else if (Random_exit < 81) {
			exitMessage = "이 게임을 끄는 것을 후회하지 않으실 겁니까?";
		} else if (Random_exit <= 101) {
			exitMessage = "게임을 빨리 깰겁니까, 끄실겁니까";
		}

		return exitMessage;
	}
}
