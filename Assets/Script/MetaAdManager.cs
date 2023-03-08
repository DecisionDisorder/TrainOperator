using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudienceNetwork;

/// <summary>
/// Meta(Facebook) Audience 광고 표시 관리 클래스
/// </summary>
public class MetaAdManager : MonoBehaviour
{
    public MessageManager messageManager;
    public UnityAdsHelper unityAdsHelper;
    /// <summary>
    /// Meta Audience 보상형 광고
    /// </summary>
    private RewardedVideoAd rewardedVideoAd;
    /// <summary>
    /// 광고가 불러와졌는지 여부
    /// </summary>
    [SerializeField]
    private bool isLoaded = false;

    private void Awake()
    {
        AudienceNetworkAds.Initialize();
    }
    private void Start()
    {
        LoadRewardedVideo();
    }

    /// <summary>
    /// 보상형 광고 영상 불러오기
    /// </summary>
    public void LoadRewardedVideo()
    {
        rewardedVideoAd = new RewardedVideoAd("296651322573406_301727702065768");
        rewardedVideoAd.Register(gameObject);

        // 광고 로딩이 되었을 때의 콜백
        rewardedVideoAd.RewardedVideoAdDidLoad = (delegate ()
        {
            Debug.Log("RewardedVideo ad loaded.");
            isLoaded = true;
        });

        // 광고 재생을 완료한 후의 콜백
        rewardedVideoAd.RewardedVideoAdDidClose = (delegate () {
            Debug.Log("Rewarded video ad did close.");
            if (rewardedVideoAd != null)
            {
                rewardedVideoAd.Dispose();
                unityAdsHelper.Reward();
                LoadRewardedVideo();
            }
        }); 
        rewardedVideoAd.RewardedVideoAdDidClick = (delegate () {
            Debug.Log("RewardedVideo ad clicked.");
        });
        // 광고 오류 디버깅
        rewardedVideoAd.RewardedVideoAdDidFailWithError = (delegate (string error) {
            Debug.Log("RewardedVideo ad failed to load with error: " + error);
        });
        rewardedVideoAd.RewardedVideoAdWillLogImpression = (delegate () {
            Debug.Log("RewardedVideo ad logged impression.");
        });
        rewardedVideoAd.RewardedVideoAdDidSucceed = (delegate () {
            Debug.Log("Rewarded video ad validated by server");
        });
        rewardedVideoAd.RewardedVideoAdDidFail = (delegate () {
            Debug.Log("Rewarded video ad not validated, or no response from server");
        });


        rewardedVideoAd.LoadAd();
    }

    /// <summary>
    /// 보상형 동영상 광고 재생
    /// </summary>
    public void ShowRewardedVideo()
    {
        // 로딩이 완료되었으면 재생
        if (isLoaded)
        {
            rewardedVideoAd.Show();
            isLoaded = false;
            unityAdsHelper.StartCoolTime(2);
        }
        // 로딩되지 않은 것에 대한 메시지 출력 및 다시 광고 로딩 시도
        else
        {
            messageManager.ShowMessage("광고가 준비되지 않았습니다.", 1.5f);
            LoadRewardedVideo();
            //Debug.Log("Ad not loaded. Click load to request an ad.");
        }
    }
}
