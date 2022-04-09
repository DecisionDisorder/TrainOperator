using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonColor_Controller : MonoBehaviour {
    
    public Image[] drivers = new Image[6];
	
	public Color Gray = new Vector4(0,0,0,255);

    public GameObject Driver_Menu;

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
