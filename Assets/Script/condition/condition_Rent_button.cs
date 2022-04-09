using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class condition_Rent_button : MonoBehaviour {

	public Text[] facilityConditionTexts;

    public Text Max_val_text;

	public Text Total_text;

	public Text[] rentConditionTexts;

    public GameObject condition_RentMenu;
    public button_Rent rent;
    public AcceptRent_Controller acceptRent_Controller;
    public AddRent_Controller addRent_Controller;
	
    public void UpdateText()
    {
        Total_text.text = "총: " + acceptRent_Controller.NumofAccepted + "개 / " + addRent_Controller.NumOfRentSpace + "개";

        rent.SetLimit();
        for(int i = 0; i < facilityConditionTexts.Length; i++)
        {
            facilityConditionTexts[i].text = rent.NumOfFacilities[i] + "개 / " + rent.limitFacilities[i] + "개";
        }

        Max_val_text.text = addRent_Controller.NumOfRentSpace + "개 / " + AddRent_Controller.RentSpaceLimit + "개";

        for (int i = 0; i < acceptRent_Controller.numOfRented.Length; i++)
            rentConditionTexts[i].text = acceptRent_Controller.numOfRented[i] + "개";
    }
}
