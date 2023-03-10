using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudienceNetwork;

/// <summary>
/// Meta(Facebook) Audience ���� ǥ�� ���� Ŭ����
/// </summary>
public class MetaAdManager : MonoBehaviour
{
    public MessageManager messageManager;
    public UnityAdsHelper unityAdsHelper;
    /// <summary>
    /// Meta Audience ������ ����
    /// </summary>
    private RewardedVideoAd rewardedVideoAd;
    /// <summary>
    /// ���� �ҷ��������� ����
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
    /// ������ ���� ���� �ҷ�����
    /// </summary>
    public void LoadRewardedVideo()
    {
        rewardedVideoAd = new RewardedVideoAd("296651322573406_301727702065768");
        rewardedVideoAd.Register(gameObject);

        // ���� �ε��� �Ǿ��� ���� �ݹ�
        rewardedVideoAd.RewardedVideoAdDidLoad = (delegate ()
        {
            Debug.Log("RewardedVideo ad loaded.");
            isLoaded = true;
        });

        // ���� ����� �Ϸ��� ���� �ݹ�
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
        // ���� ���� �����
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
    /// ������ ������ ���� ���
    /// </summary>
    public void ShowRewardedVideo()
    {
        // �ε��� �Ϸ�Ǿ����� ���
        if (isLoaded)
        {
            rewardedVideoAd.Show();
            isLoaded = false;
            unityAdsHelper.StartCoolTime(2);
        }
        // �ε����� ���� �Ϳ� ���� �޽��� ��� �� �ٽ� ���� �ε� �õ�
        else
        {
            messageManager.ShowMessage("���� �غ���� �ʾҽ��ϴ�.", 1.5f);
            LoadRewardedVideo();
            //Debug.Log("Ad not loaded. Click load to request an ad.");
        }
    }
}
