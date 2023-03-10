using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 버튼 색상 조건 컨트롤러 클래스
/// </summary>
public class ButtonColor_Controller3 : MonoBehaviour {

    /// <summary>
    /// 임대 시설 구매 버튼 이미지
    /// </summary>
	public Image[] facilityImgs;
	
    /// <summary>
    /// 위생 관리 아이템 구매 버튼 이미지
    /// </summary>
	public Image[] sanitoryPurchaseButtons;
    
    /// <summary>
    /// 치안 관리 아이템 구매 버튼 이미지
    /// </summary>
    public Image[] peacePurchaseButtons;

    /// <summary>
    /// 구매 불가 안내 색상
    /// </summary>
	public Color Gray = new Vector4(0,0,0,255);

    /// <summary>
    /// 치안 관리 메뉴 오브젝트
    /// </summary>
    public GameObject Peace_Menu;
    /// <summary>
    /// 위생 관리 메뉴 오브젝트
    /// </summary>
    public GameObject Sanitory_Menu;
    /// <summary>
    /// 임대 관리 메뉴 오브젝트
    /// </summary>
    public GameObject Rent_Menu;

    /// <summary>
    /// 임대 시스템 매니저
    /// </summary>
    public RentManager rentManager;
    /// <summary>
    /// 치안 시스템 매니저
    /// </summary>
    public PeaceManager peaceManager;
    /// <summary>
    /// 위생 상태 컨트롤러
    /// </summary>
    public condition_Sanitory_Controller condition_Sanitory_Controller;
    /// <summary>
    /// 위생 시스템 매니저
    /// </summary>
    public SanitoryManager sanitoryManager;

    /// <summary>
    /// 각 치안 관리 상품이 구매 가능한 상태인지 여부를 색상으로 나타내는 함수
    /// </summary>
    public void SetPeace()
    {
        for(int i = 0; i < peacePurchaseButtons.Length; i++)
        {
            if (AssetMoneyCalculator.instance.CheckPuchasable(peaceManager.peaceLowPrices[i], peaceManager.peaceHighPrices[i]) && peaceManager.PeaceValue < 100)
            {
                if (i.Equals(peacePurchaseButtons.Length - 1))
                {
                    if (PeaceManager.isCoolTime)
                        peacePurchaseButtons[i].color = Gray;
                    else
                        peacePurchaseButtons[i].color = Color.white;
                }
                else
                    peacePurchaseButtons[i].color = Color.white;
            }
            else
            {
                peacePurchaseButtons[i].color = Gray;
            }
        }
    }
    /// <summary>
    /// 각 위생 관리 상품이 구매 가능한 상태인지 여부를 색상으로 나타내는 함수
    /// </summary>
    public void SetSanitory()
    {
        for (int i = 0; i < sanitoryPurchaseButtons.Length; i++)
        {
            if (AssetMoneyCalculator.instance.CheckPuchasable(sanitoryManager.sanitoryLowPrices[i], sanitoryManager.sanitoryHighPrices[i]) && condition_Sanitory_Controller.SanitoryValue < 100)
            {
                if (i.Equals(sanitoryPurchaseButtons.Length - 1))
                {
                    if (SanitoryManager.isCoolTime)
                        sanitoryPurchaseButtons[i].color = Gray;
                    else
                        sanitoryPurchaseButtons[i].color = Color.white;
                }
                else
                    sanitoryPurchaseButtons[i].color = Color.white;
            }
            else
            {
                sanitoryPurchaseButtons[i].color = Gray;
            }
        }
    }
    /// <summary>
    /// 각 임대 상품이 구매 가능한 상태인지 여부를 색상으로 나타내는 함수
    /// </summary>
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
