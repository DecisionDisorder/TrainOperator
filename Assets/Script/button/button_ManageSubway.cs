using UnityEngine;
using System.Collections;

public class button_ManageSubway : MonoBehaviour
{
    public ColorUpdater colorUpdater;
    public ButtonColor_Controller3 buttonColor_Controller;
    public SanitoryManager button_Sanitory;
    public PeaceManager button_Peace;
    public GameObject ManageSubway_Menu;
    public GameObject ManageSanitory_Menu;
    public GameObject ManagePeace_Menu;

    public void PressKey(int nKey)
    {
        switch (nKey)
        {
            case 0://back
                ManageSubway_Menu.SetActive(false);
                break;
            case 1://Sanitory
                colorUpdater.StartUpdateButtonColor(buttonColor_Controller.SetSanitory);
                button_Sanitory.SetPrice();
                button_Sanitory.SetText();
                ManageSanitory_Menu.SetActive(true);
                break;
            case 2://Peace
                colorUpdater.StartUpdateButtonColor(buttonColor_Controller.SetPeace);
                button_Peace.SetPrice();
                ManagePeace_Menu.SetActive(true);
                break;
        }
    }
    public void PressKey_Back(int nKey)
    {
        switch (nKey)
        {
            case 0://Sanitory
                colorUpdater.StopUpdateButtonColor();
                ManageSanitory_Menu.SetActive(false);
                break;
            case 1://Peace
                colorUpdater.StopUpdateButtonColor();
                ManagePeace_Menu.SetActive(false);
                break;
        }
    }

}
