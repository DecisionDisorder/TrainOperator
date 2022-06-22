using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudienceNetwork;

public class MetaAdManager : MonoBehaviour
{
    public MessageManager messageManager;
    public UnityAdsHelper unityAdsHelper;
    private RewardedVideoAd rewardedVideoAd;
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

    public void LoadRewardedVideo()
    {
        rewardedVideoAd = new RewardedVideoAd("296651322573406_301727702065768");
        rewardedVideoAd.Register(gameObject);

        rewardedVideoAd.RewardedVideoAdDidLoad = (delegate ()
        {
            Debug.Log("RewardedVideo ad loaded.");
            isLoaded = true;
        });

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

    public void ShowRewardedVideo()
    {
        if (isLoaded)
        {
            rewardedVideoAd.Show();
            isLoaded = false;
            unityAdsHelper.StartCoolTime(2);
        }
        else
        {
            messageManager.ShowMessage("광고가 준비되지 않았습니다.", 1.5f);
            LoadRewardedVideo();
            //Debug.Log("Ad not loaded. Click load to request an ad.");
        }
    }
}
