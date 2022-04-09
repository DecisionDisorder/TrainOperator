using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {

	public GameObject[] tutorialgroups;
	public GameObject[] basicTutorials;
    public GameObject[] rentTutorials;
    public GameObject[] bankTutorials;

    public GameObject[] basicTutorialMenus;
    public GameObject openMainMenuButton;
    public Text trainPriceText;
    public Text trainPassengerText;

    public GameObject bankRegisterMenu;

	public GameObject wantHelp_menu;
    public MessageManager messageManager;
    public LineManager lineManager;

    public bool NotTutorialPlayed { get { return PlayManager.instance.playData.notTutorialPlayed; } set { PlayManager.instance.playData.notTutorialPlayed = value; } }
    public bool rentTutorialPlayed { get { return PlayManager.instance.playData.rentTutorialPlayed; } set { PlayManager.instance.playData.rentTutorialPlayed = value; } }
    public bool NotBankTutorialPlayed { get { return PlayManager.instance.playData.notBankTutorialPlayed; } set { PlayManager.instance.playData.notBankTutorialPlayed = value; } }

    public int tutorialStep = 0;
    private int basicTutorialTouched = 0;
    private bool isFirstBasicTutorial;

    public UpdateDisplay rentUpdateDisplay;
    public UpdateDisplay bankUpdateDisplay;


    enum TutorialType { Basic = 1, Rent, Bank };

	void Start () {
        if(NotTutorialPlayed)
        {
            if (lineManager.lineCollections[0].lineData.numOfTrain.Equals(0))
                isFirstBasicTutorial = true;
            else
                isFirstBasicTutorial = false;

            StartTutorial(TutorialType.Basic);
        }
        if (!rentTutorialPlayed)
            rentUpdateDisplay.onEnable += CheckRentTutorial;
        if (NotBankTutorialPlayed)
            bankUpdateDisplay.onEnable += CheckBankTutorial;
	}

    private void CheckRentTutorial()
    {
        if (!rentTutorialPlayed)
        {
            StartTutorial(TutorialType.Rent);
        }
    }

    private void CheckBankTutorial()
    {
        if (NotBankTutorialPlayed)
        {
            StartTutorial(TutorialType.Bank);
        }
    }

    public void ReTutorial()
    {
        messageManager.OpenCommonCheckMenu("튜토리얼을 다시 하시겠습니까?", "기본 튜토리얼을 다시 진행합니다.", Color.white, StartTutorialAgain);
    }

    public void StartTutorialAgain()
    {
        PlayManager.instance.playData.notTutorialPlayed = true;
        DataManager.instance.SaveAll();
        SceneManager.LoadScene(0);
    }

    private void StartTutorial(TutorialType type)
    {
        tutorialgroups[0].SetActive(true);
        tutorialgroups[(int)type].SetActive(true);
        tutorialStep = 0;

        switch(type)
        {
            case TutorialType.Basic:
                basicTutorials[0].SetActive(true);
                break;
            case TutorialType.Rent:
                rentTutorials[0].SetActive(true);
                break;
            case TutorialType.Bank:
                bankTutorials[0].SetActive(true);
                break;
        }
    }

    public void BasicTutoralTouch()
    {
        basicTutorialTouched++;
        if(basicTutorialTouched > 10)
            ProceedBasicTutorial();
    }
    public void BuyTrainTutorial()
    {
        UpdateTrainPrice();
        if (isFirstBasicTutorial)
        {
            if (!TouchMoneyManager.CheckLimitValid(lineManager.lineCollections[0].purchaseTrain.priceData.GetTrainPassenger(lineManager.lineCollections[0].lineData.numOfTrain), 0))
            {
                ProceedBasicTutorial();
            }
        }
        else
            ProceedBasicTutorial();
    }

    private void UpdateTrainPrice()
    {
        LargeVariable price = LargeVariable.zero;
        lineManager.lineCollections[0].purchaseTrain.priceData.GetTrainPrice(lineManager.lineCollections[0].lineData.numOfTrain, ref price);
        trainPriceText.text = "가격: " + price.lowUnit + "$";
        trainPassengerText.text = "승객수 +" + lineManager.lineCollections[0].purchaseTrain.priceData.GetTrainPassenger(lineManager.lineCollections[0].lineData.numOfTrain) + "명";
    }

    public void ProceedBasicTutorial()
    {
        if (tutorialStep.Equals(1))
        {
            openMainMenuButton.SetActive(false);
            StartCoroutine(WaitNextStep(0.2f));
        }
        else
        {
            basicTutorials[tutorialStep++].SetActive(false);

            if (tutorialStep.Equals(6))
            {
                basicTutorials[tutorialStep].SetActive(true);
                basicTutorialMenus[0].SetActive(false);
                basicTutorialMenus[1].SetActive(false);
                AssetMoneyCalculator.instance.ArithmeticOperation(5000, 0, true);
            }
            else if (tutorialStep.Equals(8))
            {
                basicTutorials[tutorialStep].SetActive(true);
                basicTutorialMenus[2].SetActive(false);
                AssetMoneyCalculator.instance.ArithmeticOperation(10000, 0, true);
            }
            else if (tutorialStep.Equals(basicTutorials.Length))
            {
                tutorialgroups[0].SetActive(false);
                tutorialgroups[1].SetActive(false);
                NotTutorialPlayed = false;
                DataManager.instance.SaveAll();
            }
            else
                basicTutorials[tutorialStep].SetActive(true);
        }
    }
    IEnumerator WaitNextStep(float delay)
    {
        yield return new WaitForSeconds(delay);

        basicTutorials[tutorialStep++].SetActive(false);
        basicTutorials[tutorialStep].SetActive(true);
        UpdateTrainPrice();
        AssetMoneyCalculator.instance.ArithmeticOperation(1300, 0, true);
    }

    public void ProceedRentTutorial()
    {
        rentTutorials[tutorialStep++].SetActive(false);
        if (tutorialStep.Equals(rentTutorials.Length))
        {
            tutorialgroups[0].SetActive(false);
            tutorialgroups[2].SetActive(false);
            rentTutorialPlayed = true;
            DataManager.instance.SaveAll();
        }
        else
            rentTutorials[tutorialStep].SetActive(true);
    }

    public void ProceedBankTutorial()
    {
        bankTutorials[tutorialStep++].SetActive(false);

        if (tutorialStep.Equals(4))
        {
            bankTutorials[tutorialStep].SetActive(true);
            bankRegisterMenu.SetActive(false);
        }
        else if (tutorialStep.Equals(bankTutorials.Length))
        {
            tutorialgroups[0].SetActive(false);
            tutorialgroups[3].SetActive(false);
            NotBankTutorialPlayed = false;
            DataManager.instance.SaveAll();
        }
        else
            bankTutorials[tutorialStep].SetActive(true);
    }

    public void TutorialExit()
    {
        if (tutorialgroups[1].activeInHierarchy)
            NotTutorialPlayed = false;
        else if (tutorialgroups[2].activeInHierarchy)
            rentTutorialPlayed = true;
        else if (tutorialgroups[3].activeInHierarchy)
            NotBankTutorialPlayed = false;

        for (int i = 0; i < tutorialgroups.Length; i++)
            tutorialgroups[i].SetActive(false);

        DataManager.instance.SaveAll();
    }
}
/*
	public static void SaveTutorialed()
	{
        EncryptedPlayerPrefs.SetString("notTutorialPlayed", ""+notTutorialPlayed);
	}
	public static void LoadTutorialed()
	{
        notTutorialPlayed = bool.Parse(EncryptedPlayerPrefs.GetString("notTutorialPlayed","true"));
    }*/