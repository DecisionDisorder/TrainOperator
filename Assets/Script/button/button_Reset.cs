using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class button_Reset : MonoBehaviour {

	public GameObject Yes;
	public GameObject No;
	public GameObject Reset_Menu;
	public GameObject CheckReset_Menu;
	public MessageManager messageManager;

	public static int Alarm_Reseted = 0;

	public void PressKey(int nKey)
	{
		switch (nKey)
		{
			case 0://first yes
				CheckReset_Menu.SetActive(true);
				break;
			case 1://first no
				Reset_Menu.SetActive(false);
				break;
			case 3://second no
				Reset_Menu.SetActive(false);
				CheckReset_Menu.SetActive(false);
				break;
		}
	}
	void ReturnTo0()
	{
		Alarm_Reseted = 0;
	}
}
