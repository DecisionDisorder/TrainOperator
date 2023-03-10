using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 배경 이미지 관리 클래스
/// </summary>
public class BackgroundImageManager : MonoBehaviour
{
    /// <summary>
    /// 배경 변경 메뉴 오브젝트
    /// </summary>
    public GameObject changeBackgroundMenu;

    /// <summary>
    /// 배경 속 후단 이미지 오브젝트 배열
    /// </summary>
    public GameObject[] backgroundBackImage;
    /// <summary>
    /// 배경 속 전단 이미지 오브젝트 배열
    /// </summary>
    public GameObject[] backgroundFrontImage;
    /// <summary>
    /// 스크린도어 배경 선택 버튼
    /// </summary>
    public Button screenDoorButton;
    /// <summary>
    /// 스크린도어 배경 잠금 알림 이미지
    /// </summary>
    public GameObject screenDoorLock;

    public LineManager lineManager;
    public TrainBackgroundManager trainBackgroundManager;

    /// <summary>
    /// 배경 종류 (0: 일반, 1: 스크린도어)
    /// </summary>
    public int backgroundType { get { return PlayManager.instance.playData.backgroundType; } set { PlayManager.instance.playData.backgroundType = value; } }

    private void Start()
    {
        backgroundBackImage[0].SetActive(false);
        backgroundFrontImage[0].SetActive(false);
        ChangeBackground(backgroundType);
    }

    /// <summary>
    /// 배경 변경 메뉴 활성화/비활성화
    /// </summary>
    /// <param name="active">활성화 여부</param>
    public void SetChangeMenu(bool active)
    {
        SetScreendoorUnlock();
        changeBackgroundMenu.SetActive(active);
    }

    /// <summary>
    /// 사용자가 선택한 타입으로 배경 변경
    /// </summary>
    /// <param name="type">배경 타입(0:일반 / 1:스크린도어)</param>
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
    /// 스크린도어 배경 언락 작업
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
