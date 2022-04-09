using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainBackgroundManager : MonoBehaviour
{
    public Animation trainAnimation;
    public Animation[] trainDoorAnimations;
    public AnimationClip[] normalDoorClips;
    public AnimationClip[] screendoorClips;
    public AnimationClip[] trainClips;

    public GameObject doors;

    public float normalPlatformTrainY;
    public float screendoorTrainY;

    public BackgroundImageManager backgroundImageManager;
    public TouchMoneyImageControl touchMoneyImageControl;

    public GameObject massPeople;
    public Image[] peopleInTrainImgs;
    public Sprite[] massSprites;
    private bool isDoorOpened;

    public bool isTestMode = false;


    private void Start()
    {
        StartWaitTrainEntrance();
    }

    private void StartWaitTrainEntrance()
    {
        if(!isTestMode)
            StartCoroutine(TrainEntranceTimer(Random.Range(30f, 50f)));
        else
            StartCoroutine(TrainEntranceTimer(Random.Range(3f, 5f)));
    }

    public void SetPlatformSettings()
    {
        if (backgroundImageManager.backgroundType.Equals(0))
        {
            trainDoorAnimations[1].gameObject.SetActive(false);
            doors.transform.localPosition = new Vector3(doors.transform.localPosition.x, normalPlatformTrainY);
            trainAnimation.transform.localPosition = new Vector3(trainAnimation.transform.localPosition.x, normalPlatformTrainY);
        }
        else
        {
            trainDoorAnimations[1].gameObject.SetActive(true);
            doors.transform.localPosition = new Vector3(doors.transform.localPosition.x, screendoorTrainY);
            trainAnimation.transform.localPosition = new Vector3(trainAnimation.transform.localPosition.x, screendoorTrainY);
        }
    }

    IEnumerator TrainEntranceTimer(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetPlatformSettings();

        trainAnimation.gameObject.SetActive(true);
        trainAnimation.clip = trainClips[0];
        trainAnimation.Play();
        PlayManager.instance.WaitAnimation(trainAnimation, StartWaitDoorOpen);
    }

    private void StartWaitDoorOpen()
    {
        StartCoroutine(DoorOpen(Random.Range(1f,3f)));
    }

    IEnumerator DoorOpen(float delay)
    {
        yield return new WaitForSeconds(delay);

        doors.SetActive(true);
        isDoorOpened = true;

        if(backgroundImageManager.backgroundType.Equals(1))
        {
            trainDoorAnimations[1].clip = screendoorClips[0];
            trainDoorAnimations[1].Play();
        }

        if(TouchEarning.passengerRandomFactor >= 100)
        {
            massPeople.SetActive(true);
            StartCoroutine(MassPeopleEffect(0.7f));
        }


        trainDoorAnimations[0].clip = normalDoorClips[0];
        trainDoorAnimations[0].Play();
        PlayManager.instance.WaitAnimation(trainDoorAnimations[0], StartWaitDoorClose);
    }

    private void StartWaitDoorClose()
    {
        StartCoroutine(DoorClose(Random.Range(4f,8f)));
    }

    IEnumerator DoorClose(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (backgroundImageManager.backgroundType.Equals(1))
        {
            trainDoorAnimations[1].clip = screendoorClips[1];
            trainDoorAnimations[1].Play();
        }

        trainDoorAnimations[0].clip = normalDoorClips[1];
        trainDoorAnimations[0].Play();
        PlayManager.instance.WaitAnimation(trainDoorAnimations[0], StartWaitTrainExit);
    }

    private void StartWaitTrainExit()
    {
        isDoorOpened = false;
        doors.SetActive(false);
        StartCoroutine(TrainExitTimer(Random.Range(1f,3f)));
    }

    IEnumerator TrainExitTimer(float delay)
    {
        yield return new WaitForSeconds(delay);

        trainAnimation.clip = trainClips[1];
        trainAnimation.Play();
        PlayManager.instance.WaitAnimation(trainAnimation, DisableTrain);
    }

    private void DisableTrain()
    {
        trainAnimation.gameObject.SetActive(false);
        StartWaitTrainEntrance();
    }

    IEnumerator MassPeopleEffect(float delay)
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < peopleInTrainImgs.Length; i++)
            peopleInTrainImgs[i].sprite = massSprites[touchMoneyImageControl.currentIndex];

        if(isDoorOpened)
            StartCoroutine(MassPeopleEffect(delay));
        else
            massPeople.SetActive(false);
    }
}
