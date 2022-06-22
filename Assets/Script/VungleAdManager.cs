using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VungleAdManager : MonoBehaviour
{
    public UnityAdsHelper unityAdsHelper;
    public MessageManager messageManager;

    string appID = "56d2fc26f3bdd9c306000015";
    string placementID = "REWARD-1714112";
    bool playable = false;
    /*
    private void Start()
    {
        Vungle.init(appID);
        Vungle.loadAd(placementID);
        InitEventHandlers();
    }

    public void CheckPlayable()
    {
        Vungle.adPlayableEvent += (placementID, adPlayable) =>
        {
            if (placementID == this.placementID)
            {
                Debug.Log("Not playable");
            }
        };
    }

    public void PlayAD()
    {
        if(playable)
            Vungle.playAd(placementID);
        else
            messageManager.ShowMessage("광고가 준비되지 않았습니다.", 1.5f);
    }

    private void InitEventHandlers()
    {
        Vungle.onAdRewardedEvent += (placementID) =>
        {
            Debug.Log("Ad Reward");
            unityAdsHelper.Reward();
        };

        Vungle.onAdEndEvent += (placementID) =>
        {
            Debug.Log("Ad ended");
            Vungle.loadAd(placementID);
        };

        Vungle.adPlayableEvent += (placementID, adPlayable) =>
        {
            playable = true;
        };
    }*/
}
