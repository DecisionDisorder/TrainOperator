using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonColor_Controller3 : MonoBehaviour {

	public Image[] facilityImgs;
	//---------------------------------------------
	public Image[] sanitoryPurchaseButtons;
    //==============================================
    public Image[] peacePuechaseButtons;

	public Color Gray = new Vector4(0,0,0,255);

    public GameObject Peace_Menu;
    public GameObject Sanitory_Menu;
    public GameObject Rent_Menu;

    public RentManager rentManager;
    public PeaceManager peaceManager;
    public condition_Sanitory_Controller condition_Sanitory_Controller;
    public PeaceManager button_Peace;
    public SanitoryManager button_Sanitory;

    public void SetPeace()
    {
        for(int i = 0; i < peacePuechaseButtons.Length; i++)
        {
            if (AssetMoneyCalculator.instance.CheckPuchasable(button_Peace.peaceLowPrices[i], button_Peace.peaceHighPrices[i]) && peaceManager.PeaceValue < 100)
            {
                if (i.Equals(peacePuechaseButtons.Length - 1))
                {
                    if (PeaceManager.isCoolTime)
                        peacePuechaseButtons[i].color = Gray;
                    else
                        peacePuechaseButtons[i].color = Color.white;
                }
                else
                    peacePuechaseButtons[i].color = Color.white;
            }
            else
            {
                peacePuechaseButtons[i].color = Gray;
            }
        }
    }
    public void SetSanitory()
    {
        for (int i = 0; i < sanitoryPurchaseButtons.Length; i++)
        {
            if (AssetMoneyCalculator.instance.CheckPuchasable(button_Sanitory.sanitoryLowPrices[i], button_Sanitory.sanitoryHighPrices[i]) && condition_Sanitory_Controller.SanitoryValue < 100)
            {
                if (i.Equals(sanitoryPurchaseButtons.Length - 1))
                {
                    if (SanitoryManager.isCoolTime)
                        peacePuechaseButtons[i].color = Gray;
                    else
                        peacePuechaseButtons[i].color = Color.white;
                }
                else
                    peacePuechaseButtons[i].color = Color.white;
            }
            else
            {
                sanitoryPurchaseButtons[i].color = Gray;
            }
        }
    }
    public void SetRent()
    {
        for(int i = 0; i < facilityImgs.Length; i++)
        {
            if (AssetMoneyCalculator.instance.CheckPuchasable(rentManager.facilityDatas[i].priceLow, rentManager.facilityDatas[i].priceHigh) && rentManager.CheckLimit(i))
            {
                facilityImgs[i].color = Color.white;
            }
            else
            {
                facilityImgs[i].color = Gray;
            }
        }
    }
}
