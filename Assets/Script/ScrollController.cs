using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// (Deprecated) 스크롤중인 UI의 스크롤을 멈춰주는 클래스
/// </summary>
public class ScrollController : MonoBehaviour {

	public Text Onoff_text;
	public GameObject[] Scrolls_main;
	public GameObject[] Scrolls_assets;

	public static int isScoll = 0;


	public void PressKey(int nKey)
	{
		switch (nKey) {
		case 0:
			if (isScoll == 0) {
				isScoll = 1;
				Onoff_text.text = "스크롤\n켜기";
				for (int i = 0; i < Scrolls_main.Length; ++i) {
					Scrolls_main [i].GetComponent<ScrollRect>().vertical = false;
				}
				for (int i = 0; i < Scrolls_assets.Length; ++i) {
					Scrolls_assets [i].GetComponent<ScrollRect> ().vertical = false;
				}
			} else if(isScoll == 1){
				isScoll = 0;
				Onoff_text.text = "스크롤\n끄기";
				for (int i = 0; i < Scrolls_main.Length; ++i) {
					Scrolls_main [i].GetComponent<ScrollRect>().vertical = true;
				}
				for (int i = 0; i < Scrolls_assets.Length; ++i) {
					Scrolls_assets [i].GetComponent<ScrollRect> ().vertical = true;
				}
			}
			break;
		}
	}
}
