using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 위생 관리 상태 컨트롤러 클래스
/// </summary>
public class condition_Sanitory_Controller : MonoBehaviour {
    public CompanyReputationManager companyReputationController;

    /// <summary>
    /// 위생도 수치
    /// </summary>
	public int SanitoryValue { get { return companyReputationController.companyData.averageSanitoryCondition; } set { companyReputationController.companyData.averageSanitoryCondition = value; } }

    /// <summary>
    /// 위생도 상태 등급 정보
    /// </summary>
    private int condition = 0;

    /// <summary>
    /// 위생도 상태 등급 정보
    /// </summary>
    public int Condition
    {
        get { return condition; }
        set
        {
            condition = value;
            SetSanitoryImage();
        }
    }

    /// <summary>
    /// 위생도 수치를 나타내는 텍스트
    /// </summary>
	public Text TotalText;
    /// <summary>
    /// 위생도 등급을 나타내는 텍스트
    /// </summary>
	public Text ConditionText;

    /// <summary>
    /// 위생 상태가 나쁨을 나타내는 이미지 셋
    /// </summary>
    public GameObject[] dirtyImgs;
    /// <summary>
    /// 위생 상태가 좋음을 나타내는 이미지 셋
    /// </summary>
    public GameObject[] cleanImgs;

    /// <summary>
    /// 현재 표시 중인 이미지 인덱스
    /// </summary>
    private int currentIndex = 0;
    /// <summary>
    /// 위생 상태 이미지를 업데이트하는 코루틴
    /// </summary>
    IEnumerator cSetSanitoryImage;

    public UpdateDisplay conditionSanitoryUpdateDisplay;

    void Start()
    {
        // 위생도 조회 메뉴가 활성화 되었을 때 텍스트 정보 업데이트
        conditionSanitoryUpdateDisplay.onEnableUpdate += UpdateText;
        // 위생도 하락 타이머 코루틴
        StartCoroutine(Timer(GetInterval()));

        CheckSanitory();
    }

    /// <summary>
    /// 위생도 하락을 계산하는 코루틴
    /// </summary>
    /// <param name="time">위생도 하락 주기</param>
    IEnumerator Timer(float time)
    {
        yield return new WaitForSeconds(time);

        // 위생도 값이 최저 -100 한도로 주기적으로 위생도 수치를 깎는다.
        if (SanitoryValue > -100)
        {
            SanitoryValue -= GetSanitoryDecrement();
            if (SanitoryValue < -100)
            {
                SanitoryValue = -100;
            }
        }
        // 텍스트 정보 업데이트
        UpdateText();

        StartCoroutine(Timer(GetInterval()));
    }

    /// <summary>
    /// 텍스트 정보 업데이트 함수
    /// </summary>
    public void UpdateText()
    {
        // 위생도 등급 계산
        CheckSanitory();

        // 위생도 점수 업데이트
        if (SanitoryValue == 0)
        {
            TotalText.text = "0점";
        }
        else
        {
            TotalText.text = SanitoryValue + "점";
        }
    }

    /// <summary>
    /// 노선 진행 상황에 따라 반복 주기 랜덤으로 계산하여 리턴
    /// </summary>
    /// <returns>계산된 위생도 하락 주기</returns>
    private int GetInterval()
    {
        int Timeset;
        if (!LineManager.instance.lineCollections[4].IsExpanded())
        {
            Timeset = Random.Range(60, 86);
        }
        else if (!LineManager.instance.lineCollections[9].IsExpanded())
        {
            Timeset = Random.Range(55, 71);
        }
        else
        {
            Timeset = Random.Range(45, 61);
        }
        return Timeset;
    }

    /// <summary>
    /// 노선 진행 상황에 따라 랜덤으로 위생도 감소량 계산
    /// </summary>
    /// <returns></returns>
    private int GetSanitoryDecrement()
    {
        int Score;
        if (!LineManager.instance.lineCollections[4].IsExpanded())
        {
            Score = Random.Range(0, 4);
        }
        else if (!LineManager.instance.lineCollections[9].IsExpanded())
        {
            Score = Random.Range(2, 5);
        }
        else
        {
            Score = Random.Range(3, 7);
        }
        return Score;
    }

    /// <summary>
    /// 위생도 단계 계산 함수
    /// </summary>
	public void CheckSanitory()
	{
		if(-100 <= SanitoryValue && SanitoryValue < -40)
		{
			ConditionText.text = "상태: " + "역겨움";
			Condition = 1; 
		}
		else if(-40 <= SanitoryValue && SanitoryValue < 0)
		{
			ConditionText.text = "상태: " + "더러움";
			Condition = 2;
		}
		else if(0 <= SanitoryValue && SanitoryValue <= 50)
		{
			ConditionText.text = "상태: " + "평범함";
			Condition = 3;
		}
		else if(50 < SanitoryValue && SanitoryValue <= 80)
		{
			ConditionText.text = "상태: " + "깨끗함";
			Condition = 4;
		}
		else if(80 < SanitoryValue && SanitoryValue <= 100)
		{
			ConditionText.text = "상태: " + "아주 깔끔함";
			Condition = 5;
        }
        // 위생도 단계 계산 후 고객 만족도에 반영
        companyReputationController.RenewReputation();
    }

    /// <summary>
    /// 위생 단계에 따라 플랫폼 배경에 표시되는 이미지 설정 (2가지 이미지 교차 표기)
    /// </summary>
    /// <param name="delay">교차 주기</param>
    /// <returns></returns>
    IEnumerator CSetSanitoryImage(float delay)
    {
        if (Condition < 3)
        {
            cleanImgs[0].SetActive(false);
            cleanImgs[1].SetActive(false);

            dirtyImgs[currentIndex].SetActive(false);
            dirtyImgs[GetNextIndex()].SetActive(true);
        }
        else if (Condition == 3)
        {
            cleanImgs[0].SetActive(false);
            cleanImgs[1].SetActive(false);

            dirtyImgs[0].SetActive(false);
            dirtyImgs[1].SetActive(false);
        }
        else if (Condition > 3)
        {
            cleanImgs[currentIndex].SetActive(false);
            cleanImgs[GetNextIndex()].SetActive(true);

            dirtyImgs[0].SetActive(false);
            dirtyImgs[1].SetActive(false);
        }
        yield return new WaitForSeconds(delay);

        if(Condition != 3)
            StartCoroutine(cSetSanitoryImage = CSetSanitoryImage(delay));
    }

    /// <summary>
    /// 특정 배열 오브젝트가 모두 활성화 중인지 확인하는 함수
    /// </summary>
    /// <param name="objects">확인할 게임 오브젝트</param>
    /// <returns>활성화 여부</returns>
    private bool IsArrayObjectActive(GameObject[] objects)
    {
        for(int i = 0; i < objects.Length; i++)
        {
            if (objects[i].activeInHierarchy)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 위생 단계 이미지 변경이 필요한지 확인하는 함수
    /// </summary>
    /// <returns>이미지 변경 필요 여부</returns>
    private bool NeedSanitoryImageChange()
    {
        if (Condition < 3 && !IsArrayObjectActive(dirtyImgs))
            return true;
        else if (Condition > 3 && !IsArrayObjectActive(cleanImgs))
            return true;
        else if (Condition == 3 && (IsArrayObjectActive(cleanImgs) || IsArrayObjectActive(dirtyImgs)))
            return true;
        else
            return false;
    }

    /// <summary>
    /// 위생 단계 이미지 설정
    /// </summary>
    private void SetSanitoryImage()
    {
        if (NeedSanitoryImageChange())
        {
            if (cSetSanitoryImage != null)
                StopCoroutine(cSetSanitoryImage);

            cSetSanitoryImage = CSetSanitoryImage(0.8f);
            StartCoroutine(cSetSanitoryImage);
        }
    }
    /// <summary>
    /// 교차할 다음 이미지 인덱스 구하는 함수
    /// </summary>
    /// <returns>활성화 할 이미지 인덱스</returns>
    private int GetNextIndex()
    {
        currentIndex = currentIndex == 0 ? 1 : 0;
        return currentIndex;
    }
}
