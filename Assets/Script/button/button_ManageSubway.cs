using UnityEngine;
using System.Collections;

/// <summary>
/// 지하철 관리 메뉴 이벤트 클래스
/// </summary>
public class button_ManageSubway : MonoBehaviour
{
    public ColorUpdater colorUpdater;
    public ButtonColor_Controller3 buttonColor_Controller;
    public SanitoryManager button_Sanitory;
    public PeaceManager button_Peace;
    /// <summary>
    /// 지하철 관리 메뉴 오브젝트
    /// </summary>
    public GameObject ManageSubway_Menu;
    /// <summary>
    /// 위생도 관리 메뉴 오브젝트
    /// </summary>
    public GameObject ManageSanitory_Menu;
    /// <summary>
    /// 치안 관리 메뉴 오브젝트
    /// </summary>
    public GameObject ManagePeace_Menu;

    /// <summary>
    /// 메뉴 버튼 클릭 리스너
    /// </summary>
    public void PressKey(int nKey)
    {
        switch (nKey)
        {
            case 0: // 메뉴 비활성화
                ManageSubway_Menu.SetActive(false);
                break;
            case 1: // 위생도
                colorUpdater.StartUpdateButtonColor(buttonColor_Controller.SetSanitory);
                button_Sanitory.SetPrice();
                button_Sanitory.SetText();
                ManageSanitory_Menu.SetActive(true);
                break;
            case 2: // 치안
                colorUpdater.StartUpdateButtonColor(buttonColor_Controller.SetPeace);
                button_Peace.SetPrice();
                ManagePeace_Menu.SetActive(true);
                break;
        }
    }
    /// <summary>
    /// 메뉴 종료 리스너
    /// </summary>
    public void PressKey_Back(int nKey)
    {
        switch (nKey)
        {
            case 0: // 위생도
                colorUpdater.StopUpdateButtonColor();
                ManageSanitory_Menu.SetActive(false);
                break;
            case 1: // 치안
                colorUpdater.StopUpdateButtonColor();
                ManagePeace_Menu.SetActive(false);
                break;
        }
    }

}
