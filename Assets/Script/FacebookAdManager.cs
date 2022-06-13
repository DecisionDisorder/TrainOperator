using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudienceNetwork;

public class FacebookAdManager : MonoBehaviour
{
    public MessageManager messageManager;
    public UnityAdsHelper unityAdsHelper;
    private RewardedVideoAd rewardedVideoAd;
    [SerializeField]
    private bool isLoaded = false;

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
            //Debug.Log("RewardedVideo ad loaded.");
            isLoaded = true;
        });

        rewardedVideoAd.RewardedVideoAdDidClose = (delegate () {
            //Debug.Log("Rewarded video ad did close.");
            if (rewardedVideoAd != null)
            {
                rewardedVideoAd.Dispose();
            }
        });

        rewardedVideoAd.RewardedVideoAdDidSucceed = (delegate () {
            //Debug.Log("Rewarded video ad validated by server");
            unityAdsHelper.Reward();
        });
        rewardedVideoAd.RewardedVideoAdDidFail = (delegate () {
            messageManager.ShowMessage("광고가 정상적으로 종료되지 않아 보상이 지급되지 않았습니다.", 1.5f);
            //Debug.Log("Rewarded video ad not validated, or no response from server");
        });

        rewardedVideoAd.LoadAd();
    }

    public void ShowRewardedVideo()
    {
        if(isLoaded)
        {
            rewardedVideoAd.Show();
            isLoaded = false;
            LoadRewardedVideo();
            unityAdsHelper.StartCoolTime(2);
        }
        else
        {
            messageManager.ShowMessage("광고가 준비되지 않았습니다.", 1.5f);
            //Debug.Log("Ad not loaded. Click load to request an ad.");
        }
    }
}
