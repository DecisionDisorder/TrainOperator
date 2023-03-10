using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 기관사 버튼 색상 컨트롤러 클래스
/// </summary>
public class ButtonColor_Controller : MonoBehaviour {
    
    /// <summary>
    /// 기관사 구매 버튼 이미지
    /// </summary>
    public Image[] drivers = new Image[6];
	
    /// <summary>
    /// 구매 불가 안내 색상
    /// </summary>
	public Color Gray = new Vector4(0,0,0,255);

    /// <summary>
    /// 기관사 구매 메뉴 오브젝트
    /// </summary>
    public GameObject Driver_Menu;

    /// <summary>
    /// 각 기관사가 구매 가능한 상태인지 여부를 색상으로 나타내는 함수
    /// </summary>
    public void SetDriver()
    {
        for (int i = 0; i < drivers.Length; ++i)
        {
            if (MyAsset.instance.MoneyLow >= DriversManager.price_Drivers[i])
            {
                if(i >= 7)
                {
                    if(MyAsset.instance.MoneyHigh >= DriversManager.price_Drivers[i])
                        drivers[i].color = Color.white;
                    else
                        drivers[i].color = Gray;
                }
                else
                    drivers[i].color = Color.white;
            }
            else
            {
                drivers[i].color = Gray;
            }
        }
    }
}
