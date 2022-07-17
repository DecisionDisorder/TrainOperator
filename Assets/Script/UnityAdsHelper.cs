using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class UnityAdsHelper : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public MessageManager messageManager;
    //public OpeningCardPack openingCardPack;
    public ItemManager itemManager;
    public RentManager rent;
    string gameID = "1039615";
    string myPlacementId = "rewardedVideo";
    bool testMode = false;

    public Text[] coolTimeTexts;
    public Button[] adButtons;
    public GameObject[] coolTimeObjs;
    public int coolTime;

    private void Start()
    {
		Advertisement.Initialize(gameID, testMode);
        LoadAd();
    }

    public void StartCoolTime(int index)
    {
        coolTimeObjs[index].SetActive(true);
        adButtons[index].enabled = false;
        coolTimeTexts[index].text = coolTime + "초";
        StartCoroutine(CoolTimer(coolTime, index));
    }

    public void FinishCoolTime(int index)
    {
        adButtons[index].enabled = true;
        coolTimeObjs[index].SetActive(false);
    }

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

    public void Reward()
    {
        //openingCardPack.SilverCardAmount++;
        rent.rentData.numOfADWatch++;
        int add = 100;
        if(rent.rentData.numOfADWatch % 5 == 0)
            add += 50;
        itemManager.CardPoint += add;
        messageManager.ShowMessage("카드 포인트 " + add + "P가 지급되었습니다.", 1.5f);
        DataManager.instance.SaveAll();
    }

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
    // Implement a method to execute when the user clicks the button.
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

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.LogFormat("OnUnityAdsShowComplete: {0}, {1}", placementId, showCompletionState.ToString());

        if (myPlacementId.Equals(placementId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            //Debug.Log("Unity Ads Rewarded Ad Completed");
            // Grant a reward.
            //Debug.Log("보상을 받았습니다.");
            Reward();

            // Load another ad:
            Advertisement.Load(myPlacementId, this);
        }
    }
    #endregion
}