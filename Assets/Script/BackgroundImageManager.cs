using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��� �̹��� ���� Ŭ����
/// </summary>
public class BackgroundImageManager : MonoBehaviour
{
    /// <summary>
    /// ��� ���� �޴� ������Ʈ
    /// </summary>
    public GameObject changeBackgroundMenu;

    /// <summary>
    /// ��� �� �Ĵ� �̹��� ������Ʈ �迭
    /// </summary>
    public GameObject[] backgroundBackImage;
    /// <summary>
    /// ��� �� ���� �̹��� ������Ʈ �迭
    /// </summary>
    public GameObject[] backgroundFrontImage;
    /// <summary>
    /// ��ũ������ ��� ���� ��ư
    /// </summary>
    public Button screenDoorButton;
    /// <summary>
    /// ��ũ������ ��� ��� �˸� �̹���
    /// </summary>
    public GameObject screenDoorLock;

    public LineManager lineManager;
    public TrainBackgroundManager trainBackgroundManager;

    /// <summary>
    /// ��� ���� (0: �Ϲ�, 1: ��ũ������)
    /// </summary>
    public int backgroundType { get { return PlayManager.instance.playData.backgroundType; } set { PlayManager.instance.playData.backgroundType = value; } }

    private void Start()
    {
        backgroundBackImage[0].SetActive(false);
        backgroundFrontImage[0].SetActive(false);
        ChangeBackground(backgroundType);
    }

    /// <summary>
    /// ��� ���� �޴� Ȱ��ȭ/��Ȱ��ȭ
    /// </summary>
    /// <param name="active">Ȱ��ȭ ����</param>
    public void SetChangeMenu(bool active)
    {
        SetScreendoorUnlock();
        changeBackgroundMenu.SetActive(active);
    }

    /// <summary>
    /// ����ڰ� ������ Ÿ������ ��� ����
    /// </summary>
    /// <param name="type">��� Ÿ��(0:�Ϲ� / 1:��ũ������)</param>
    public void ChangeBackground(int type)
    {
        backgroundBackImage[backgroundType].SetActive(false);
        backgroundFrontImage[backgroundType].SetActive(false);

        backgroundType = type;
        backgroundBackImage[backgroundType].SetActive(true);
        backgroundFrontImage[backgroundType].SetActive(true);
        trainBackgroundManager.SetPlatformSettings();
    }

    /// <summary>
    /// ��ũ������ ��� ��� �۾�
    /// </summary>
    private void SetScreendoorUnlock()
    {
        if(lineManager.lineCollections[0].IsScreendoorInstalled())
        {
            screenDoorButton.enabled = true;
            screenDoorLock.SetActive(false);
        }
        else
        {
            screenDoorButton.enabled = false;
            screenDoorLock.SetActive(true);
        }
    }
}
