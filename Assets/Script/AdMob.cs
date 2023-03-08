using System;
using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api; // 구글 애드몹 API 네임 스페이스

public class AdMob : MonoBehaviour {

    public MessageManager messageManager;
    public UnityAdsHelper unityAdsHelper;
    /// <summary>
    /// 테스트모드 여부
    /// </summary>
    public bool testMode = false;

    private void Start()
    {
        RequestRewardedVideo();
        RequestBanner();
        RequestRewardedInterstitialAD();
    }

    #region 배너 광고

    /// <summary>
    /// 베너를 보여주기 위한 클래스
    /// </summary>
    private BannerView bannerView = null;
    /// <summary>
    /// 베너 요청
    /// </summary>
    void RequestBanner()
    {
        // 실제 광고 ID
        string adUnitId = "ca-app-pub-8836637760997342/8121798603";
        // 테스트 광고 ID
        string testUnitId = "ca-app-pub-3940256099942544/6300978111";

        // 테스트모드 여부에 따라 광고 표시 초기화
        if (!testMode)
            bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.TopRight);
        else
            bannerView = new BannerView(testUnitId, AdSize.Banner, AdPosition.TopRight);

        // 빈 Ad Request 생성  
        AdRequest request = new AdRequest.Builder().Build();

        // Ad request로 배너 광고 로드 및 표기
        bannerView.LoadAd(request);
        bannerView.Show();
    }
    /// <summary>
    /// 광고 숨기기
    /// </summary>
    public void DisableAd()
    {
        bannerView.Hide();

    }
    #endregion


    #region 동영상광고
    /// <summary>
    /// 동영상 광고 오브젝트
    /// </summary>
    private RewardedAd rewardedAd;

    /// <summary>
    /// 동영상 광고 로딩 요청
    /// </summary>
    void RequestRewardedVideo()
    {
        // 실제 광고 ID
        string adUnitId = "ca-app-pub-8836637760997342/8693478399";
        // 테스트 광고 ID
        string testUnitId = "ca-app-pub-3940256099942544/5224354917";

        // 테스트모드 여부에 따라 광고 표시 초기화
        if (!testMode)
            rewardedAd = new RewardedAd(adUnitId);
        else
            rewardedAd = new RewardedAd(testUnitId);

        // 광고 이벤트 리스너 추가
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // 광고 영상 불러오기 요청
        AdRequest request = new AdRequest.Builder().Build();
        rewardedAd.LoadAd(request);

    }

    /// <summary>
    /// 광고를 닫았을 때의 이벤트
    /// </summary>
    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        RequestRewardedVideo();
    }

    /// <summary>
    /// 광고 시청 보상 이벤트
    /// </summary>
    public void HandleUserEarnedReward(object sender, Reward args)
    {
        unityAdsHelper.Reward();   
    }

    /// <summary>
    /// 애드몹의 보상형 동영상 광고 재생
    /// </summary>
    public void ShowRewardVideo_Admob()
    {
        // 광고가 로딩 되었으면 시작
        if (rewardedAd.IsLoaded())
        {
            unityAdsHelper.StartCoolTime(0);
            rewardedAd.Show();
        }
        // 로딩되지 않았으면 메시지 출력
        else
        {
            messageManager.ShowMessage("광고가 준비되지 않았습니다.", 1.5f);
        }
    }
    #endregion

    #region 보상형 전면광고
    /// <summary>
    /// 보상형 전면 광고 오브젝트
    /// </summary>
    private RewardedInterstitialAd rewardedInterstitialAd;
    /// <summary>
    /// 보상형 전면 광고 요청
    /// </summary>
    private void RequestRewardedInterstitialAD()
    {
        // 실제 광고 ID
        string adUnitID = "ca-app-pub-8836637760997342/2304037048";
        // 테스트 광고 ID
        string testAdUnitID = "ca-app-pub-3940256099942544/5354046379";

        // 광고 표시 시작 (테스트 유무에 따라 ID 다르게 적용)
        AdRequest request = new AdRequest.Builder().Build();
        if (testMode)
            RewardedInterstitialAd.LoadAd(testAdUnitID, request, (req, err) => adLoadCallback(req, err));
        else
            RewardedInterstitialAd.LoadAd(adUnitID, request, adLoadCallback);
    }

    /// <summary>
    /// 보상형 전면 광고 로딩 콜백
    /// </summary>
    private void adLoadCallback(RewardedInterstitialAd ad, AdFailedToLoadEventArgs error)
    {
        if (error == null)
        {
            rewardedInterstitialAd = ad;
        }
    }

    /// <summary>
    /// 보상형 전면광고 재생
    /// </summary>
    public void ShowRewardedInterstitialAd()
    {
        // 불러온 광고가 있으면 재생
        if(rewardedInterstitialAd != null)
        {
            unityAdsHelper.StartCoolTime(0);
            rewardedInterstitialAd.Show(userEarnedRewardCallback);
        }
        // 없으면 메시지 출력
        else
        {
            messageManager.ShowMessage("광고가 준비되지 않았습니다.", 1.5f);
        }
    }

    /// <summary>
    /// 광고 보상 지급 콜백 함수
    /// </summary>
    /// <param name="reward">지급할 보상</param>
    private void userEarnedRewardCallback(Reward reward)
    {
        unityAdsHelper.Reward();
        RequestRewardedInterstitialAD();
    }

    #endregion
}
