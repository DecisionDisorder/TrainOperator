using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class condition_VehicleBase_button : MonoBehaviour
{

    public GameObject Busan_Menu;
    public GameObject condition_vechicle_Menu;
    public GameObject Daegu_Menu;
    public GameObject MP2_Menu;
    public GameObject GJDJ_Menu;

    public Animation OtherLines_ani;
    public Text other_text;

    public void PressKey(int nKey)
    {
        switch (nKey)
        {
            case 0:
                Busan_Menu.SetActive(true);
                break;
            case 1:
                Busan_Menu.SetActive(false);
                break;
            case 2:
                condition_vechicle_Menu.SetActive(false);
                break;
            case 3:
                Daegu_Menu.SetActive(true);
                break;
            case 4:
                Daegu_Menu.SetActive(false);
                break;
            case 5:
                MP2_Menu.SetActive(true);
                break;
            case 6:
                MP2_Menu.SetActive(false);
                break;
            case 7:
                GJDJ_Menu.SetActive(true);
                break;
            case 8:
                GJDJ_Menu.SetActive(false);
                break;
        }
    }
    bool isdowned = false;
    public void DownMenu()
    {
        if (!isdowned)
        {
            OtherLines_ani["condition_VehicleBase"].speed = 1;
            OtherLines_ani.Play();
            other_text.text = "▲다른노선▲";
            isdowned = true;
        }
        else
        {
            OtherLines_ani["condition_VehicleBase"].time = OtherLines_ani["condition_VehicleBase"].length;
            OtherLines_ani["condition_VehicleBase"].speed = -1;
            OtherLines_ani.Play();
            //OtherLines_ani.CrossFade("back_condition_VehicleBase");
            other_text.text = "▼다른노선▼";
            isdowned = false;
        }
    }
}