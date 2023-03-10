using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 잡상인 민원 처리 미니게임 관리 클래스
/// </summary>
public class PeddlerMiniGameManager : MonoBehaviour {

	/// <summary>
	/// 홈버튼 클릭 효과음
	/// </summary>
	public AudioSource homeButtonAudio;
	/// <summary>
	/// 홈버튼 클릭 오디오 클립
	/// </summary>
	public AudioClip audioclip;

	/// <summary>
	/// 잡상인 미니게임 메뉴 오브젝트
	/// </summary>
	public GameObject PeddlerEvent_Menu;
	/// <summary>
	/// 민원 문자 확인 메뉴 오브젝트
	/// </summary>
	public GameObject G920_Menu;
	/// <summary>
	/// 열차 선택 메뉴 오브젝트
	/// </summary>
	public GameObject Select_Menu;

	/// <summary>
	/// 잡상인이 숨어있는 기차 버튼 배열
	/// </summary>
	public GameObject[] Train_button = new GameObject[3];
	/// <summary>
	/// 기차 뒤의 잡상인 이미지 배열 (셋 중 하나만 활성화)
	/// </summary>
	public GameObject[] peddler_image = new GameObject[3];

	/// <summary>
	/// 열차 선택 결과 정보 텍스트 
	/// </summary>
	public Text Result_text;
	/// <summary>
	/// 미니게임 닫기 버튼 오브젝트
	/// </summary>
	public GameObject close_button;

	/// <summary>
	/// 미니게임 성공 효과음
	/// </summary>
	public AudioSource successAudio;
	/// <summary>
	/// 미니게임 실패 효과음
	/// </summary>
	public AudioSource failedAudio;

	public SettingManager button_Option;

	/// <summary>
	/// 보상 계산 기준 시간
	/// </summary>
	public int clearTime;

	/// <summary>
	/// 테스트 버튼
	/// </summary>
    public void PressKey_test(int nKey)
    {
        switch (nKey)
        {
            case 0:
                InitPeddlerMiniGame();
                PeddlerEvent_Menu.SetActive(true);
                break;
        }
    }
	/// <summary>
	/// 잡상인 민원 접수 버튼 클릭 이벤트 리스너
	/// </summary>
    public void PressKey_homebutton (int nKey) 
	{
        switch (nKey)
        {
            case 0://homebutton
                homeButtonAudio.PlayOneShot(audioclip);
                Select_Menu.SetActive(true);
                G920_Menu.SetActive(false);
                RandomSummon();
                break;
            case 1:
                G920_Menu.SetActive(false);
                PeddlerEvent_Menu.SetActive(false);
                break;
        }
	}
	/// <summary>
	/// 열차 선택 버튼 리스너
	/// </summary>
	public void PressKey_select(int nKey)
	{
		switch (nKey)
		{
			case 0://Close
				PeddlerEvent_Menu.SetActive(false);
				break;
			case 1://1st
				Train_button[0].SetActive(false);
				Train_button[1].SetActive(false);
				Train_button[2].SetActive(false);
				if (peddler_image[0].activeInHierarchy)
				{
					StartCoroutine(Success(1f));

				}
				else
				{
					StartCoroutine(Fail(1f));
				}
				break;
			case 2:
				Train_button[0].SetActive(false);
				Train_button[1].SetActive(false);
				Train_button[2].SetActive(false);
				if (peddler_image[1].activeInHierarchy)
				{
					StartCoroutine(Success(1f));
				}
				else
				{
					StartCoroutine(Fail(1f));
				}
				break;
			case 3:
				Train_button[0].SetActive(false);
				Train_button[1].SetActive(false);
				Train_button[2].SetActive(false);
				if (peddler_image[2].activeInHierarchy)
				{
					StartCoroutine(Success(1f));
				}
				else
				{
					StartCoroutine(Fail(1f));
				}
				break;
		}
	}
	/// <summary>
	/// 성공 대기 후 처리 코루틴
	/// </summary>
	/// <param name="time">대기 시간</param>
	IEnumerator Success(float time)
	{
		yield return new WaitForSeconds(time);

		ulong addedMoneyLow = 0, addedMoneyHigh = 0;
		string money1 = "", money2 = "";
		// 보상 계산 및 UI 업데이트
		PlayManager.SetMoneyReward(ref addedMoneyLow, ref addedMoneyHigh, clearTime);
		AssetMoneyCalculator.instance.ArithmeticOperation(addedMoneyLow, addedMoneyHigh, true);
		PlayManager.ArrangeUnit(addedMoneyLow, addedMoneyHigh, ref money1, ref money2);
		successAudio.Play();

		Result_text.text = "잡상인을 잡으셨습니다! 보상으로 " + money2 + money1 + "$을 지급 받았습니다.";
		close_button.SetActive (true);
	}
	/// <summary>
	/// 실패 대기 후 처리 코루틴
	/// </summary>
	/// <param name="time">대기 시간</param>
	IEnumerator Fail(float time)
	{
		yield return new WaitForSeconds(time);

		failedAudio.Play();
		Result_text.text = "잡상인을 놓쳤습니다... 다음엔 꼭 잡으세요!";
		close_button.SetActive (true);
	}
	/// <summary>
	/// 잡상인 무작위 배치
	/// </summary>
	void RandomSummon()
	{
		int random_peddler = Random.Range (1,91);
		if (random_peddler < 31) {
			peddler_image [0].SetActive (true);
		} else if (random_peddler < 61) {
			peddler_image [1].SetActive (true);
		} else if (random_peddler <= 91) {
			peddler_image [2].SetActive (true);
		}
	}
	/// <summary>
	/// 잡상인 미니게임 초기화 작업
	/// </summary>
	public void InitPeddlerMiniGame()
	{
		Select_Menu.SetActive (false);
		close_button.SetActive (false);
		Result_text.text = "";
		G920_Menu.SetActive (true);
		Train_button [0].SetActive (true);
		Train_button [1].SetActive (true);
		Train_button [2].SetActive (true);
		peddler_image [0].SetActive (false);
		peddler_image [1].SetActive (false);
		peddler_image [2].SetActive (false);
		PeddlerEvent_Menu.SetActive(true);
	}
}
