using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��� ���� ������ �ִϸ��̼� ȿ�� ���� Ŭ����
/// </summary>
public class TrainBackgroundManager : MonoBehaviour
{
    /// <summary>
    /// ���� �̵� �ִϸ��̼�
    /// </summary>
    public Animation trainAnimation;
    /// <summary>
    /// ���� ���Թ� ���� �ִϸ��̼�
    /// </summary>
    public Animation[] trainDoorAnimations;
    /// <summary>
    /// �Ϲ� �°��忡���� ���� �ִϸ��̼� Ŭ�� �迭
    /// </summary>
    public AnimationClip[] normalDoorClips;
    /// <summary>
    /// ��ũ������ �°��忡���� ���� �ִϸ��̼� Ŭ�� �迭
    /// </summary>
    public AnimationClip[] screendoorClips;
    /// <summary>
    /// ���� �̵� �ִϸ��̼� Ŭ�� �迭
    /// </summary>
    public AnimationClip[] trainClips;

    /// <summary>
    /// �ִϸ��̼ǿ� ���Թ� �׷� ������Ʈ
    /// </summary>
    public GameObject doors;

    /// <summary>
    /// �Ϲ� �°��忡���� ���� ��ġ Y
    /// </summary>
    public float normalPlatformTrainY;
    /// <summary>
    /// ��ũ������ �°��忡���� ���� ��ġ Y
    /// </summary>
    public float screendoorTrainY;

    public BackgroundImageManager backgroundImageManager;
    public TouchMoneyImageControl touchMoneyImageControl;

    /// <summary>
    /// ������ Ÿ�� �ִ� ���� ������� ǥ���ϴ� ������Ʈ
    /// </summary>
    public GameObject massPeople;
    /// <summary>
    /// ������ Ÿ�� �ִ� �°� �̹��� �迭
    /// </summary>
    public Image[] peopleInTrainImgs;
    /// <summary>
    /// �°� ���� ������ �̹��� ��������Ʈ ���ҽ� �迭
    /// </summary>
    public Sprite[] massSprites;
    /// <summary>
    /// ���Թ��� �����ִ��� ����
    /// </summary>
    private bool isDoorOpened;

    /// <summary>
    /// ��� �׽�Ʈ ��� ����
    /// </summary>
    public bool isTestMode = false;


    private void Start()
    {
        StartWaitTrainEntrance();
    }

    /// <summary>
    /// ���� ���� �ִϸ��̼� ��� ����
    /// </summary>
    private void StartWaitTrainEntrance()
    {
        if(!isTestMode)
            StartCoroutine(TrainEntranceTimer(Random.Range(30f, 50f)));
        else
            StartCoroutine(TrainEntranceTimer(Random.Range(3f, 5f)));
    }

    /// <summary>
    /// �Ϲ�/��ũ������ �°��忡 ���� ��� ���� ����
    /// </summary>
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
            SyncScreendoorAnimation();
            doors.transform.localPosition = new Vector3(doors.transform.localPosition.x, screendoorTrainY);
            trainAnimation.transform.localPosition = new Vector3(trainAnimation.transform.localPosition.x, screendoorTrainY);
        }
    }

    /// <summary>
    /// ��� ���� ���Թ� ���� �ִϸ��̼� �ε����� ���Ѵ�
    /// </summary>
    private int GetPlayingDoorIndex()
    {
        if (trainDoorAnimations[0].clip.Equals(normalDoorClips[0]))
            return 0;
        else
            return 1;
    }

    /// <summary>
    /// ��ũ������ �ִϸ��̼� Ÿ�̹� ����ȭ
    /// </summary>
    private void SyncScreendoorAnimation()
    {
        if (trainDoorAnimations[0].isPlaying)
        {
            int playingIndex = GetPlayingDoorIndex();
            trainDoorAnimations[1].clip = screendoorClips[playingIndex];
            trainDoorAnimations[1].Play();
            trainDoorAnimations[1][screendoorClips[playingIndex].name].time = trainDoorAnimations[0][normalDoorClips[playingIndex].name].time;
        }
    }

    /// <summary>
    /// ���� ���� �ִϸ��̼� ��� �ڷ�ƾ
    /// </summary>
    /// <param name="delay">��� �ð�</param>
    IEnumerator TrainEntranceTimer(float delay)
    {
        yield return new WaitForSeconds(delay);
        // ���� ���� �غ� ����
        SetPlatformSettings();

        // ���� ���� �ִϸ��̼� ����
        trainAnimation.gameObject.SetActive(true);
        trainAnimation.clip = trainClips[0];
        trainAnimation.Play();
        // ���� �� ���Թ� ���� �ִϸ��̼� ���
        PlayManager.instance.WaitAnimation(trainAnimation, StartWaitDoorOpen);
    }

    /// <summary>
    /// ���Թ� ���� �ִϸ��̼� ���
    /// </summary>
    private void StartWaitDoorOpen()
    {
        StartCoroutine(DoorOpen(Random.Range(1f,3f)));
    }

    /// <summary>
    /// ���� �ð� ��� �� ���Թ� ���� �ִϸ��̼� �������ִ� �ڷ�ƾ
    /// </summary>
    /// <param name="delay">���� ��� �ð�</param>
    IEnumerator DoorOpen(float delay)
    {
        yield return new WaitForSeconds(delay);

        // ���Թ� ���� ����
        doors.SetActive(true);
        isDoorOpened = true;

        // ��ũ�������� �� ��ũ������ �ִϸ��̼ǵ� ����
        if(backgroundImageManager.backgroundType.Equals(1))
        {
            trainDoorAnimations[1].clip = screendoorClips[0];
            trainDoorAnimations[1].Play();
        }

        // �°� ���� ���� ���� ȿ�� ����
        if(TouchEarning.passengerRandomFactor >= 100)
        {
            massPeople.SetActive(true);
            StartCoroutine(MassPeopleEffect(0.7f));
        }

        // ���Թ� ���� �ִϸ��̼� ȿ�� ����
        trainDoorAnimations[0].clip = normalDoorClips[0];
        trainDoorAnimations[0].Play();
        // ���Թ� �ݱ� �ִϸ��̼� ���
        PlayManager.instance.WaitAnimation(trainDoorAnimations[0], StartWaitDoorClose);
    }

    /// <summary>
    /// ���Թ� �ݱ� �ִϸ��̼� ȿ�� ���
    /// </summary>
    private void StartWaitDoorClose()
    {
        StartCoroutine(DoorClose(Random.Range(4f,8f)));
    }

    /// <summary>
    /// ���Թ� �ݱ� ��� �� ������ִ� �ڷ�ƾ
    /// </summary>
    /// <param name="delay">��� �ð�</param>
    IEnumerator DoorClose(float delay)
    {
        yield return new WaitForSeconds(delay);

        // ��ũ������ �� �� Ŭ�� ����
        if (backgroundImageManager.backgroundType.Equals(1))
        {
            trainDoorAnimations[1].clip = screendoorClips[1];
            trainDoorAnimations[1].Play();
        }
        // �ݱ� �ִϿ��̼� ����
        trainDoorAnimations[0].clip = normalDoorClips[1];
        trainDoorAnimations[0].Play();
        // ���� ��� �ִϸ��̼� ���
        PlayManager.instance.WaitAnimation(trainDoorAnimations[0], StartWaitTrainExit);
    }

    /// <summary>
    /// ���� ��� �ִϸ��̼� ���
    /// </summary>
    private void StartWaitTrainExit()
    {
        isDoorOpened = false;
        doors.SetActive(false);
        StartCoroutine(TrainExitTimer(Random.Range(1f,3f)));
    }

    /// <summary>
    /// ���� �ð� ��� �� ���� ��� �ִϸ��̼��� �����ϴ� �ڷ�ƾ
    /// </summary>
    /// <param name="delay">��� �ð�</param>
    IEnumerator TrainExitTimer(float delay)
    {
        yield return new WaitForSeconds(delay);

        trainAnimation.clip = trainClips[1];
        trainAnimation.Play();
        PlayManager.instance.WaitAnimation(trainAnimation, DisableTrain);
    }

    /// <summary>
    /// ���� ��� ���Ŀ� ���� ������Ʈ �׷� ��Ȱ��ȭ �� ���� ���� �ִϸ��̼� ȿ�� ��� ����
    /// </summary>
    private void DisableTrain()
    {
        trainAnimation.gameObject.SetActive(false);
        StartWaitTrainEntrance();
    }

    /// <summary>
    /// ����� ���� ���� �̹��� ��ȯ ȿ�� �ڷ�ƾ
    /// </summary>
    /// <param name="delay">��ȯ ��� �ð�</param>
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
