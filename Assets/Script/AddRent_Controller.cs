using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AddRent_Controller : MonoBehaviour {

	public GameObject AddRent_Menu;
	public Text Num_text;
	public Text numOfAdd_text;
	public Text price;

	public button_Rent rent;
	public MessageManager messageManager;
	public condition_Rent_button condition_Rent_Button;

	public AudioSource purchaseSound;

	public int NumOfRentSpace { get { return rent.rentData.rentSpaceAmount; } set { rent.rentData.rentSpaceAmount = value; } }
	private int spaceExpandAmount = 0;
	private ulong totalPrice = 0;

	public ulong rentSpacePrice;
	public static int RentSpaceLimit { get { return MyAsset.instance.myAssetData.numOfStations * 2; } }

	private int standardValue;
	/*void Start ()
    {
        LoadData ();
    }*/

	public void PressKey(int nKey)
	{
		switch (nKey) {

		case 1:
			AddRent_Menu.SetActive (true);
			Num_text.text = "현재 공간: " + NumOfRentSpace + "개 ("+ (RentSpaceLimit - NumOfRentSpace) +"개 추가 가능)";
			break;
		}
	}
	public void PressKey_CheckMenu(int nKey)
	{
        switch (nKey)
        {
            case 0://cancel
                AddRent_Menu.SetActive(false);
                spaceExpandAmount = 0;
                totalPrice = 0;
                numOfAdd_text.text = spaceExpandAmount + "개";
                price.text = "비용: 0$";
                break;
            case 1://apply
                if (AssetMoneyCalculator.instance.ArithmeticOperation(totalPrice, 0, false))
                {
                    NumOfRentSpace += spaceExpandAmount;
                    //SaveData();
					messageManager.ShowMessage("임대공간 " + spaceExpandAmount + "개를 추가하였습니다.", 1.0f);
					condition_Rent_Button.UpdateText();
					purchaseSound.Play();
				}
                else
                {
					messageManager.ShowMessage("돈이 부족합니다.");
                }
                spaceExpandAmount = 0;
                totalPrice = 0;
                numOfAdd_text.text = spaceExpandAmount + "개";
                price.text = "비용: 0$";
				DataManager.instance.SaveAll();
				AddRent_Menu.SetActive(false);
                break;
        }

	}
	public void PressKey_Num(int nKey)
	{
		switch (nKey) {
		case 0://up
			standardValue = RentSpaceLimit - NumOfRentSpace;
			if (spaceExpandAmount < standardValue && standardValue > 0) {
				spaceExpandAmount++;
				CheckNum ();
				totalPrice += rentSpacePrice;
			}
			string totalprice = string.Format("{0:#,###}",totalPrice);
			price.text = "비용: " + totalprice + "$";
			string numofadd = string.Format("{0:#,###}",spaceExpandAmount);//
			numOfAdd_text.text = numofadd + "개";
			if (spaceExpandAmount == 0)
			{
				price.text = "비용: 0$";
				numOfAdd_text.text = "0개";
			}
			break;
		case 1://down
			standardValue = RentSpaceLimit - NumOfRentSpace;
			if(spaceExpandAmount > 0 && standardValue > 0)
			{
				spaceExpandAmount--;
				totalPrice -= rentSpacePrice;
				CheckNum ();
			}

			string totalprice2 = string.Format("{0:#,###}",totalPrice);
			price.text = "비용: " + totalprice2 + "$";
			string numofadd2 = string.Format("{0:#,###}",spaceExpandAmount);//
			numOfAdd_text.text = numofadd2 + "개";
			if (spaceExpandAmount == 0)
			{
				price.text = "비용: 0$";
				numOfAdd_text.text = "0개";
			}

			break;
		case 2://all
			standardValue = RentSpaceLimit - NumOfRentSpace;
			if(standardValue > 0)
			{
				
				for(int i = spaceExpandAmount+1; i <= standardValue;i++)
				{
					spaceExpandAmount++;
					CheckNum ();
					totalPrice += rentSpacePrice;
				}
				string totalprice3 = string.Format("{0:#,###}",totalPrice);
				price.text = "비용: " + totalprice3 + "$";
				string numofadd3 = string.Format("{0:#,###}",spaceExpandAmount);//
				numOfAdd_text.text = numofadd3 + "개";
				if (spaceExpandAmount == 0)
				{
					price.text = "비용: 0$";
					numOfAdd_text.text = "0개";
				}
			}

			break;
		}
	}
	void CheckNum()
	{
        if (NumOfRentSpace + spaceExpandAmount < 31)
        {
            rentSpacePrice = 100000;
        }
        else if (NumOfRentSpace + spaceExpandAmount < 51)
        {
            rentSpacePrice = 500000;
        }
        else if (NumOfRentSpace + spaceExpandAmount < 101)
        {
            rentSpacePrice = 1000000;
        }
        else if (NumOfRentSpace + spaceExpandAmount < 151)
        {
            rentSpacePrice = 2000000;
        }
        else if (NumOfRentSpace + spaceExpandAmount < 201)
        {
            rentSpacePrice = 8000000;
        }
        else if (NumOfRentSpace + spaceExpandAmount < 301)
        {
            rentSpacePrice = 25000000;
        }
        else if (NumOfRentSpace + spaceExpandAmount < 400)
        {
            rentSpacePrice = 60000000;
        }
        else if (NumOfRentSpace + spaceExpandAmount < 500)
        {
            rentSpacePrice = 500000000;
        }
        else if (NumOfRentSpace + spaceExpandAmount < 600)
        {
            rentSpacePrice = 5000000000;
        }
        else if (NumOfRentSpace + spaceExpandAmount >= 600)
        {
            rentSpacePrice = 50000000000;
        }
	}
		/*
	public static void SaveData()
	{
		PlayerPrefs.SetInt ("NumofRent",NumOfRent);
	}
	public static void LoadData()
	{
		NumOfRent = PlayerPrefs.GetInt ("NumofRent");
	}
	public static void ResetData()
	{
		NumOfRent = 0;
	}*/
}
