using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

/// <summary>
/// 유니티Ads 시스템 관리 클래스
/// </summary>
public class UnityAdsHelper : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsInitializationListener
{
    public MessageManager messageManager;
    public ItemManager itemManager;
    public RentManager rent;
    /// <summary>
    /// 게임 광고 ID
    /// </summary>
    string gameID = "1039615";
    /// <summary>
    /// Placement ID
    /// </summary>
    string myPlacementId = "rewardedVideo";
    /// <summary>
    /// 테스트모드 여부
    /// </summary>
    bool testMode = false;

    /// <summary>
    /// 광고 재생 쿨타임 텍스트 배열
    /// </summary>
    public Text[] coolTimeTexts;
    /// <summary>
    /// 광고 재생 버튼 배열
    /// </summary>
    public Button[] adButtons;
    /// <summary>
    /// 쿨타임 표시 관련 오브젝트 배열
    /// </summary>
    public GameObject[] coolTimeObjs;
    /// <summary>
    /// 유니티 Ads 광고 재생 쿨타임
    /// </summary>
    public int coolTime;

    private void Start()
    {
        // 광고 초기화 및 불러오기 시작
		Advertisement.Initialize(gameID, testMode, this);
        LoadAd();
    }

    /// <summary>
    /// 해당 광고 인덱스에 대해 쿨타임 적용
    /// </summary>
    /// <param name="index">광고 종류 인덱스</param>
    public void StartCoolTime(int index)
    {
        coolTimeObjs[index].SetActive(true);
        adButtons[index].enabled = false;
        coolTimeTexts[index].text = coolTime + "초";
        StartCoroutine(CoolTimer(coolTime, index));
    }

    /// <summary>
    /// 쿨타임 진행중인 광고에 대해 쿨타임 종료 적용
    /// </summary>
    /// <param name="index">광고 종류 인덱스</param>
    public void FinishCoolTime(int index)
    {
        adButtons[index].enabled = true;
        coolTimeObjs[index].SetActive(false);
    }

    /// <summary>
    /// 쿨타임 타이머 코루틴
    /// </summary>
    /// <param name="remainTime">남은 시간</param>
    /// <param name="index">광고 종류 인덱스</param>
    IEnumerator CoolTimer(int remainTime, int index)
    {
        yield return new WaitForSeconds(1);
        remainTime--;
        coolTimeTexts[index].text = remainTime + "초";
        if (remainTime > 0)
        {
            StartCoroutine(CoolTimer(remainTime, index));
        }
        else
        {
            FinishCoolTime(index);
        }
    }

    /// <summary>
    /// 광고 시청 보상 지급
    /// </summary>
    public void Reward()
    {
        rent.rentData.numOfADWatch++;
        int add = 100;
        if(rent.rentData.numOfADWatch % 5 == 0)
            add += 50;
        itemManager.CardPoint += add;
        messageManager.ShowMessage("카드 포인트 " + add + "P가 지급되었습니다.", 1.5f);
        DataManager.instance.SaveAll();
    }

    /// <summary>
    /// 광고 불러오기
    /// </summary>
    public void LoadAd()
    {
        Advertisement.Load(myPlacementId, this);
    }

    #region UnityAdsLoadListener Interface
    public void OnUnityAdsAdLoaded(string placementId)
    {
        //Debug.LogFormat("OnUnityAdsAdLoaded: {0}", placementId);
    }
    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        //Debug.LogFormat("OnUnityAdsFailedToLoad: {0}, {1}, {2}", placementId, error, message);
    }
    #endregion

    #region UnityAdsShowListener interfaces
    /// <summary>
    /// Unity Ads 보상형 동영상 광고 재생
    /// </summary>
    public void ShowAd()
    {
        // Then show the ad:
        StartCoolTime(1);
        Advertisement.Show(myPlacementId, this);
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        //Debug.LogFormat("OnUnityAdsShowFailure: {0}, {1}, {2}", placementId, error, message);
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        //Debug.LogFormat("OnUnityAdsShowStart: {0}", placementId);
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        //Debug.LogFormat("OnUnityAdsShowClick: {0}", placementId);
    }
    /// <summary>
    /// Unity Ads 보상형 광고 시청 완료 콜백
    /// </summary>
    /// <param name="placementId">광고 ID</param>
    /// <param name="showCompletionState">재생 결과</param>
    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.LogFormat("OnUnityAdsShowComplete: {0}, {1}", placementId, showCompletionState.ToString());

        if (myPlacementId.Equals(placementId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            //Debug.Log("보상을 받았습니다.");
            Reward();

            // Load another ad:
            Advertisement.Load(myPlacementId, this);
        }
    }
    #endregion

    #region IUnityAdsInitializationListener
    void IUnityAdsInitializationListener.OnInitializationComplete()
    {

    }

    void IUnityAdsInitializationListener.OnInitializationFailed(UnityAdsInitializationError error, string message)
    {

    }
    #endregion

}