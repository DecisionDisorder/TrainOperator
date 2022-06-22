using System;
using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api; // 구글 애드몹 API 네임 스페이스

public class AdMob : MonoBehaviour {

    public MessageManager messageManager;
    public UnityAdsHelper unityAdsHelper;
    public bool testMode = false;

    private void Start()
    {
        RequestRewardedVideo();
        RequestBanner();
        RequestRewardedInterstitialAD();
    }

    #region 배너 광고

    private BannerView bannerView = null;
    void RequestBanner()
    {
        string adUnitId = "ca-app-pub-8836637760997342/8121798603";
        string testUnitId = "ca-app-pub-3940256099942544/6300978111";

        if (!testMode)
            bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.TopRight);
        else
            bannerView = new BannerView(testUnitId, AdSize.Banner, AdPosition.TopRight);

        // Create an empty ad request.        

        // Load the banner with the request.
        AdRequest request = new AdRequest.Builder().Build();

        bannerView.LoadAd(request);
        bannerView.Show();
    }
    public void DisableAd()
    {
        bannerView.Hide();

    }
    #endregion
    #region 동영상광고
    private RewardedAd rewardedAd;

    void RequestRewardedVideo()
    {
        string adUnitId = "ca-app-pub-8836637760997342/8693478399";
        string testUnitId = "ca-app-pub-3940256099942544/5224354917";

        if (!testMode)
            rewardedAd = new RewardedAd(adUnitId);
        else
            rewardedAd = new RewardedAd(testUnitId);

        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        AdRequest request = new AdRequest.Builder().Build();
        rewardedAd.LoadAd(request);

    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        RequestRewardedVideo();
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        unityAdsHelper.Reward();   
    }

    public void ShowRewardVideo_Admob()
    {
        if (rewardedAd.IsLoaded())
        {
            unityAdsHelper.StartCoolTime(0);
            rewardedAd.Show();
        }
        else
        {
            messageManager.ShowMessage("광고가 준비되지 않았습니다.", 1.5f);
        }
    }
    #endregion

    #region 보상형 전면광고
    private RewardedInterstitialAd rewardedInterstitialAd;
    private void RequestRewardedInterstitialAD()
    {
        string adUnitID = "ca-app-pub-8836637760997342/2304037048";
        string testAdUnitID = "ca-app-pub-3940256099942544/5354046379";

        AdRequest request = new AdRequest.Builder().Build();
        if (testMode)
            RewardedInterstitialAd.LoadAd(testAdUnitID, request, (req, err) => adLoadCallback(req, err));
        else
            RewardedInterstitialAd.LoadAd(adUnitID, request, adLoadCallback);
    }

    private void adLoadCallback(RewardedInterstitialAd ad, AdFailedToLoadEventArgs error)
    {
        if (error == null)
        {
            rewardedInterstitialAd = ad;
        }
    }

    public void ShowRewardedInterstitialAd()
    {
        if(rewardedInterstitialAd != null)
        {
            unityAdsHelper.StartCoolTime(0);
            rewardedInterstitialAd.Show(userEarnedRewardCallback);
        }
        else
        {
            messageManager.ShowMessage("광고가 준비되지 않았습니다.", 1.5f);
        }
    }

    private void userEarnedRewardCallback(Reward reward)
    {
        unityAdsHelper.Reward();
        RequestRewardedInterstitialAD();
    }

    #endregion
}
