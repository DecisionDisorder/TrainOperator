using UnityEngine;
using System.Collections;

public class LineSectionHelpController : MonoBehaviour {

	public GameObject LinesectionHelp_Menu;
	public GameObject Line1_Menu;
	public GameObject Line2_Menu;
	public GameObject Line3_Menu;
	public GameObject Line4_Menu;
    public GameObject[] Line_Menu;


    public void PressKey(int nKey)
    {
        switch (nKey)
        {
            case 0://back
                LinesectionHelp_Menu.SetActive(false);
                break;
            case 1://line1
                Line1_Menu.SetActive(true);
                break;
            case 2://line2
                Line2_Menu.SetActive(true);
                break;
            case 3://line3
                Line3_Menu.SetActive(true);
                break;
            case 4://line4
                Line4_Menu.SetActive(true);
                break;
            case 5:
                Line_Menu[0].SetActive(true);
                break;
            case 6:
                Line_Menu[1].SetActive(true);
                break;
            case 7:
                Line_Menu[2].SetActive(true);
                break;
            case 8:
                Line_Menu[3].SetActive(true);
                break;
            case 9:
                Line_Menu[4].SetActive(true);
                break;
        }
    }
	public void PressKey_Back(int nKey)
	{
        switch (nKey)
        {
            case 1://line1
                Line1_Menu.SetActive(false);
                break;
            case 2://line2
                Line2_Menu.SetActive(false);
                break;
            case 3://line3
                Line3_Menu.SetActive(false);
                break;
            case 4://line4
                Line4_Menu.SetActive(false);
                break;
            case 5:
                Line_Menu[0].SetActive(false);
                break;
            case 6:
                Line_Menu[1].SetActive(false);
                break;
            case 7:
                Line_Menu[2].SetActive(false);
                break;
            case 8:
                Line_Menu[3].SetActive(false);
                break;
            case 9:
                Line_Menu[4].SetActive(false);
                break;
        }
	}
		
}
