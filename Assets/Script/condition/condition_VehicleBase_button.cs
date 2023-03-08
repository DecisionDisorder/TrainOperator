using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 차량기지 현황 컨트롤러 클래스
/// </summary>
public class condition_VehicleBase_button : MonoBehaviour
{
    /// <summary>
    /// 차량기지 현황: 부산 메뉴 오브젝트
    /// </summary>
    public GameObject Busan_Menu;
    /// <summary>
    /// 차량기지 현황: 차량기지 현황 메뉴
    /// </summary>
    public GameObject condition_vechicle_Menu;
    /// <summary>
    /// 차량기지 현황: 대구 메뉴 오브젝트
    /// </summary>
    public GameObject Daegu_Menu;
    /// <summary>
    /// 차량기지 현황: 수도권 2구간 메뉴 오브젝트
    /// </summary>
    public GameObject MP2_Menu;
    /// <summary>
    /// 차량기지 현황: 광주/대전 메뉴 오브젝트
    /// </summary>
    public GameObject GJDJ_Menu;

    /// <summary>
    /// 기타 노선 표기 활성화 애니메이션
    /// </summary>
    public Animation OtherLines_ani;
    /// <summary>
    /// 기타 노선 on/off 상태를 나타내는 텍스트
    /// </summary>
    public Text other_text;
    /// <summary>
    /// 기타 노선이 활성화 되어있는지 여부
    /// </summary>
    private bool isOtherLinesActive = false;

    /// <summary>
    /// 다른 노선들의 현황을 활성화/비활성화 하는 버튼 리스너
    /// </summary>
    /// <param name="nKey"></param>
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

    /// <summary>
    /// 기타 노선 메뉴를 내리거나(활성화) 올리는(비활성화) 함수
    /// </summary>
    public void DownMenu()
    {
        if (!isOtherLinesActive)
        {
            OtherLines_ani["condition_VehicleBase"].speed = 1;
            OtherLines_ani.Play();
            other_text.text = "▲다른노선▲";
            isOtherLinesActive = true;
        }
        else
        {
            OtherLines_ani["condition_VehicleBase"].time = OtherLines_ani["condition_VehicleBase"].length;
            OtherLines_ani["condition_VehicleBase"].speed = -1;
            OtherLines_ani.Play();
            other_text.text = "▼다른노선▼";
            isOtherLinesActive = false;
        }
    }
}