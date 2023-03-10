using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// (Deprecated) 업데이트로 인한 데이터 초기화 확인 클래스
/// </summary>
public class Update_Reset : MonoBehaviour {

	public GameObject Update_Reset_Menu;
	public GameObject Check_Reset_Menu;

	public MessageManager messageManager;

	public static int balace_updated = 0;

	void Start () {
		balace_updated = PlayerPrefs.GetInt ("Balance_updated");
		if (balace_updated == 2) {
			
		} else {
			Update_Reset_Menu.SetActive (true);
			balace_updated = 2;
		}

	}
	


	public void PressKey(int nKey)
	{
		switch (nKey)
		{
			case 0://first yes
				Check_Reset_Menu.SetActive(true);
				break;
			case 1://first no
				Update_Reset_Menu.SetActive(false);
				break;
			case 3://second no
				Update_Reset_Menu.SetActive(false);
				Check_Reset_Menu.SetActive(false);
				break;
		}
	}
		
	public static void SaveUpdated()
	{
		PlayerPrefs.SetInt ("Balance_updated",balace_updated);
	}

}
