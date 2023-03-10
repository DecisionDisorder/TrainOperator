using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임 튜토리얼 시스템 관리 클래스
/// </summary>
public class TutorialManager : MonoBehaviour {

    /// <summary>
    /// 튜토리얼 그룹 오브젝트 배열
    /// </summary>
	public GameObject[] tutorialgroups;
    /// <summary>
    /// 기초 튜토리얼 오브젝트 배열
    /// </summary>
	public GameObject[] basicTutorials;
    /// <summary>
    /// 임대 시스템 튜토리얼 오브젝트 배열
    /// </summary>
    public GameObject[] rentTutorials;
    /// <summary>
    /// 은행 시스템 튜토리얼 오브젝트 배열
    /// </summary>
    public GameObject[] bankTutorials;

    /// <summary>
    /// 기초 튜토리얼 관련 메뉴 오브젝트 배열
    /// </summary>
    public GameObject[] basicTutorialMenus;
    /// <summary>
    /// 메인 메뉴 열기 버튼 오브젝트
    /// </summary>
    public GameObject openMainMenuButton;
    /// <summary>
    /// 열차 가격 예시 텍스트
    /// </summary>
    public Text trainPriceText;
    /// <summary>
    /// 열차 승객 수 예시 텍스트
    /// </summary>
    public Text trainPassengerText;

    /// <summary>
    /// 은행 상품 가입 메뉴 오브젝트
    /// </summary>
    public GameObject bankRegisterMenu;

    public MessageManager messageManager;
    public LineManager lineManager;

    /// <summary>
    /// 기본 튜토리얼을 플레이 하지 않았는지 여부
    /// </summary>
    public bool NotTutorialPlayed { get { return PlayManager.instance.playData.notTutorialPlayed; } set { PlayManager.instance.playData.notTutorialPlayed = value; } }
    /// <summary>
    /// 임대 시스템 튜토리얼을 플레이 했는지 여부
    /// </summary>
    public bool rentTutorialPlayed { get { return PlayManager.instance.playData.rentTutorialPlayed; } set { PlayManager.instance.playData.rentTutorialPlayed = value; } }
    /// <summary>
    /// 은행 시스템 튜토리얼을 플레이 하지 않았는지 여부
    /// </summary>
    public bool NotBankTutorialPlayed { get { return PlayManager.instance.playData.notBankTutorialPlayed; } set { PlayManager.instance.playData.notBankTutorialPlayed = value; } }

    /// <summary>
    /// 진행 중인 튜토리얼 단계
    /// </summary>
    public int tutorialStep = 0;
    /// <summary>
    /// 기초 튜토리얼에서 수익을 거두기 위해 터치한 횟수
    /// </summary>
    private int basicTutorialTouched = 0;
    /// <summary>
    /// 게임 설치 후 처음 튜토리얼 하는지에 대한 여부
    /// </summary>
    private bool isFirstBasicTutorial;

    public UpdateDisplay rentUpdateDisplay;
    public UpdateDisplay bankUpdateDisplay;

    /// <summary>
    /// 튜토리얼 종류 열거형
    /// </summary>
    enum TutorialType { Basic = 1, Rent, Bank };

	void Start () {
        // 기본 튜토리얼 조건 검사
        if(NotTutorialPlayed)
        {
            // 처음인지, 다시 하는 것인지 확인
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

    /// <summary>
    /// 임대시설 메뉴를 열었을 때 튜토리얼 플레이 여부 검사
    /// </summary>
    private void CheckRentTutorial()
    {
        if (!rentTutorialPlayed)
        {
            StartTutorial(TutorialType.Rent);
        }
    }

    /// <summary>
    /// 은행 메뉴를 열었을 때 튜토리얼 플레이 여부 검사
    /// </summary>
    private void CheckBankTutorial()
    {
        if (NotBankTutorialPlayed)
        {
            StartTutorial(TutorialType.Bank);
        }
    }

    /// <summary>
    /// 기본 튜토리얼 다시 할지 묻기
    /// </summary>
    public void ReTutorial()
    {
        messageManager.OpenCommonCheckMenu("튜토리얼을 다시 하시겠습니까?", "기본 튜토리얼을 다시 진행합니다.", Color.white, StartTutorialAgain);
    }

    /// <summary>
    /// 기본 튜토리얼 다시 하기
    /// </summary>
    public void StartTutorialAgain()
    {
        PlayManager.instance.playData.notTutorialPlayed = true;
        DataManager.instance.SaveAll();
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// 선택한 튜토리얼을 시작한다.
    /// </summary>
    /// <param name="type">선택한 튜토리얼</param>
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

    /// <summary>
    /// 기초 튜토리얼의 수익을 거두기 위한 터치 처리
    /// </summary>
    public void BasicTutoralTouch()
    {
        basicTutorialTouched++;
        if(basicTutorialTouched > 10)
            ProceedBasicTutorial();
    }
    /// <summary>
    /// 기초 튜토리얼: 열차 구매 
    /// </summary>
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

    /// <summary>
    /// 기초 튜토리얼: 열차 가격 업데이트
    /// </summary>
    private void UpdateTrainPrice()
    {
        LargeVariable price = LargeVariable.zero;
        lineManager.lineCollections[0].purchaseTrain.priceData.GetTrainPrice(lineManager.lineCollections[0].lineData.numOfTrain, ref price);
        trainPriceText.text = "가격: " + price.lowUnit + "$";
        trainPassengerText.text = "승객수 +" + lineManager.lineCollections[0].purchaseTrain.priceData.GetTrainPassenger(lineManager.lineCollections[0].lineData.numOfTrain) + "명";
    }

    /// <summary>
    /// 기초 튜토리얼 진행
    /// </summary>
    public void ProceedBasicTutorial()
    {
        // 1단계에서의 다음 단계 대기
        if (tutorialStep.Equals(1))
        {
            openMainMenuButton.SetActive(false);
            StartCoroutine(WaitNextStep(0.2f));
        }
        else
        {
            basicTutorials[tutorialStep++].SetActive(false);

            // 6단계에서의 자산 지급 처리
            if (tutorialStep.Equals(6))
            {
                basicTutorials[tutorialStep].SetActive(true);
                basicTutorialMenus[0].SetActive(false);
                basicTutorialMenus[1].SetActive(false);
                AssetMoneyCalculator.instance.ArithmeticOperation(5000, 0, true);
            }
            // 8단계에서의 자산 지급 처리
            else if (tutorialStep.Equals(8))
            {
                basicTutorials[tutorialStep].SetActive(true);
                basicTutorialMenus[2].SetActive(false);
                AssetMoneyCalculator.instance.ArithmeticOperation(10000, 0, true);
            }
            // 마지막 단계에서의 완료 처리
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
    /// <summary>
    /// 다음 단계 진행 대기 코루틴
    /// </summary>
    /// <param name="delay">대기 시간</param>
    IEnumerator WaitNextStep(float delay)
    {
        yield return new WaitForSeconds(delay);

        basicTutorials[tutorialStep++].SetActive(false);
        basicTutorials[tutorialStep].SetActive(true);
        UpdateTrainPrice();
        AssetMoneyCalculator.instance.ArithmeticOperation(1300, 0, true);
    }

    /// <summary>
    /// 임대 시스템 튜토리얼 진행
    /// </summary>
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

    /// <summary>
    /// 은행 시스템 튜토리얼 진행
    /// </summary>
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

    /// <summary>
    /// 튜토리얼 종료
    /// </summary>
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