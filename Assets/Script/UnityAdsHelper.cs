using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class UnityAdsHelper : MonoBehaviour, IUnityAdsListener
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
		Advertisement.AddListener(this);
		Advertisement.Initialize(gameID, testMode);
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
            StartCoroutine(CoolTimer(remainTime, index));
        else
            FinishCoolTime(index);
    }

    public void ShowRewardedAd()
	{
        StartCoroutine(ShowAdWhenReady());
	}

    IEnumerator ShowAdWhenReady()
    {
        while(!Advertisement.IsReady(myPlacementId))
        {
            yield return null;
        }
        StartCoolTime(1);
        Advertisement.Show(myPlacementId);
    }

    private void OnDisable()
    {
        Advertisement.RemoveListener(this);
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

    #region UnityAdListener interfaces
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (showResult == ShowResult.Finished)
        {
            Reward();
        }
        else if (showResult == ShowResult.Skipped)
        {

        }
        else if (showResult == ShowResult.Failed)
        {
            messageManager.ShowMessage("광고가 준비되지 않았습니다.", 1.5f);
        }
    }

    public void OnUnityAdsReady(string placementId)
    {
        if (placementId == myPlacementId)
        {
            //Advertisement.Show();
        }
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.LogError("OnUnityAdsDiderror : " + message);
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        if (placementId == myPlacementId)
        {
            //Advertisement.Show();
        }
    }
    #endregion
}