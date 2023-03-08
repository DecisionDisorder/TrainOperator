using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 위생도 시스템 관리 클래스
/// </summary>
public class SanitoryManager : MonoBehaviour {

    /// <summary>
    /// 위생 관리 메뉴 오브젝트
    /// </summary>
	public GameObject ManageMain_Menu;
    public MessageManager messageManager;

    /// <summary>
    /// 위생 관리 상품 별 가격 정보 텍스트 배열
    /// </summary>
    public Text[] priceTexts;
    /// <summary>
    /// 위생 관리 상품 별 위생도 향상 수치 정보 텍스트 배열
    /// </summary>
	public Text[] sanitoryTexts;

    /// <summary>
    /// 1경 미만 단위의 위생 관리 상품 별 가격 배열
    /// </summary>
    public ulong[] sanitoryLowPrices;
    /// <summary>
    /// 1경 이상 단위의 위생 관리 상품 별 가격 배열
    /// </summary>
    public ulong[] sanitoryHighPrices;
    /// <summary>
    /// 위생 관리 상품 별 가격 배율 배열
    /// </summary>
    public ulong[] sanitoryPriceMultiples;
    /// <summary>
    /// 위생 관리 상품 별 위새도 향상 수치 배열
    /// </summary>
    public int[] sanitoryValues;

    /// <summary>
    /// 환경 캠페인 개최 상품 구매 쿨타임
    /// </summary>
    public int CoolTime { get { return company_Reputation_Controller.companyData.sanitoryCoolTime; } set { company_Reputation_Controller.companyData.sanitoryCoolTime = value; } }
    /// <summary>
    /// 환경 캠페인 개최 상품 쿨타임 진행 중인지 여부
    /// </summary>
    public static bool isCoolTime;
    /// <summary>
    /// 환경 캠페인 개최 상품 구매 버튼
    /// </summary>
	public Button Campaign_button;
    /// <summary>
    /// 환경 캠페인 개최 상품 구매 쿨타임 텍스트
    /// </summary>
	public Text CoolTime_text;

    public CompanyReputationManager company_Reputation_Controller;
    public ButtonColor_Controller3 buttonColor_Controller;
    public condition_Sanitory_Controller con_sanitory;

    void Start()
    {
        if (CoolTime < 5)
        {
            isCoolTime = true;
            StartCoroutine(Timer());
        }
        SetText();
    }

    /// <summary>
    /// 위생도 상품 정보 텍스트 업데이트
    /// </summary>
    public void SetText()
    {
        string moneyLow = "", moneyHigh = "";
        for (int i = 0; i < priceTexts.Length; i++)
        {
            PlayManager.ArrangeUnit(sanitoryLowPrices[i], sanitoryHighPrices[i], ref moneyLow, ref moneyHigh);
            priceTexts[i].text = "비용: " + moneyHigh + moneyLow + "$";

            sanitoryTexts[i].text = "위생도 +" + sanitoryValues[i];
        }

        CoolTime_text.text = "쿨타임: " + CoolTime + "초";
        buttonColor_Controller.SetSanitory();
    }
    /// <summary>
    /// 환경 캠페인 개최 상품 쿨타임 차감 코루틴
    /// </summary>
    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1);

        // 쿨타임 차감 및 쿨다운 완료 검사
        CoolTime--;
        if (CoolTime < 1)
        {
            CoolTime = 5;
            isCoolTime = false;
        }

        CoolTime_text.text = "쿨타임: " + CoolTime + "초";

        if (isCoolTime)
        {
            StartCoroutine(Timer());
        }
        else
        {
            // 환경 캠페인 개최 상품 구매 가능 처리
            Campaign_button.enabled = true;
            Campaign_button.image.color = Color.white;
        }
    }
    /// <summary>
    /// 위생 관리 상품 가격 업데이트
    /// </summary>
    public void SetPrice()
    {
        ulong lowRevenue = 0, highRevenue = 0;
        MyAsset.instance.GetTotalRevenue(ref lowRevenue, ref highRevenue);
        for(int i = 0; i < sanitoryLowPrices.Length; i++)
        {
            sanitoryLowPrices[i] = lowRevenue * (ulong)PlayManager.instance.averageTouch * sanitoryPriceMultiples[i];
            sanitoryHighPrices[i] = highRevenue * (ulong)PlayManager.instance.averageTouch * sanitoryPriceMultiples[i];
            MoneyUnitTranslator.Arrange(ref sanitoryLowPrices[i], ref sanitoryHighPrices[i]);
        }
    }

    /// <summary>
    /// 위생 관리 메뉴 활성화/비활성화 처리
    /// </summary>
    /// <param name="active">활성화 여부</param>
    public void SetSanitoryMenu(bool active)
    {
        ManageMain_Menu.SetActive(active);
    }

    /// <summary>
    /// 위생 관리 상품 구매 처리
    /// </summary>
    /// <param name="index">위생 상품 인덱스</param>
    public void PurchaseSanitoryItem(int index)
	{
        // 위생도가 최대치 미만인지 확인
        if (con_sanitory.SanitoryValue < 100)
        {
            // 비용 지불 처리
            if (AssetMoneyCalculator.instance.ArithmeticOperation(sanitoryLowPrices[index], sanitoryHighPrices[index], false))
            {
                // 위생 수치 향상 
                con_sanitory.SanitoryValue += sanitoryValues[index];
                if (con_sanitory.SanitoryValue > 100)
                {
                    con_sanitory.SanitoryValue = 100;
                }

                // 환경 캠페인 개최 상품에 대해 쿨타임 적용
                if (index.Equals(2))
                {
                    isCoolTime = true;
                    Campaign_button.enabled = false;
                    Campaign_button.image.color = Color.gray;
                    StartCoroutine(Timer());
                }

                // 관련 UI 업데이트
                con_sanitory.UpdateText();
                SetText();
            }
            else
            {
                PlayManager.instance.LackOfMoney();
            }
        }
        else 
        {
            FullSanitory();
        }
    }
    /// <summary>
    /// 위생 수치 최대 안내 메시지 출력
    /// </summary>
	void FullSanitory()
	{
        messageManager.ShowMessage("더 이상 깨끗해질 수 없습니다!");
	}
}
