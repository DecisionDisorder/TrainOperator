using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Help_Controller : MonoBehaviour {

	public GameObject Help_Menu;

	public GameObject Main_Help_group;
	public GameObject condition_Help_group;
	public GameObject recommend_Help_group;

	public GameObject main1_image;
	public GameObject main2_image;
	public GameObject main3_image;

	public GameObject condition1_image;
	public GameObject condition2_image;

	public GameObject QnA_image;
	public void PressKey(int nKey)
	{
		switch (nKey) {
		case 0:
			Help_Menu.SetActive (false);
			break;
		case 1:
			Main_Help_group.SetActive (true);
			break;
		case 2:
			condition_Help_group.SetActive (true);
			break;
		case 3:
			recommend_Help_group.SetActive (true);
			break;
		case 4:
			QnA_image.SetActive (true);
			break;
		}
	}
	public void PressKey_Main(int nKey)
	{
		switch (nKey) {
		case 1:
			main1_image.SetActive (false);
			break;
		case 2:
			main2_image.SetActive (false);
			break;
		case 3:
			main1_image.SetActive (true);
			main2_image.SetActive (true);
			Main_Help_group.SetActive (false);
			break;
		}
	}
	public void PressKey_condition(int nKey)
	{
		switch(nKey){
		case 1:
			condition1_image.SetActive (false);
			break;
		case 2:
			condition1_image.SetActive (true);
			condition_Help_group.SetActive (false);
			break;
		}
	}
	public void PressKey_recommend(int nKey){
		switch(nKey){
		case 1:
			recommend_Help_group.SetActive (false);
			break;
		}
	}
	public void PressKey_QnA(int nKey){
		switch (nKey) {
		case 0:
			QnA_image.SetActive (false);
			break;
		}
	}
}
