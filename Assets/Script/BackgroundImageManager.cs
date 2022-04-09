using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundImageManager : MonoBehaviour
{
    public GameObject changeBackgroundMenu;

    public GameObject[] backgroundBackImage;
    public GameObject[] backgroundFrontImage;
    public Button screenDoorButton;
    public GameObject screenDoorLock;
    public LineManager lineManager;
    public TrainBackgroundManager trainBackgroundManager;

    public int backgroundType { get { return PlayManager.instance.playData.backgroundType; } set { PlayManager.instance.playData.backgroundType = value; } }

    private void Start()
    {
        //LoadData();
        backgroundBackImage[0].SetActive(false);
        backgroundFrontImage[0].SetActive(false);
        ChangeBackground(backgroundType);
    }

    public void SetChangeMenu(bool active)
    {
        SetScreendoorUnlock();
        changeBackgroundMenu.SetActive(active);
    }

    public void ChangeBackground(int type)
    {
        backgroundBackImage[backgroundType].SetActive(false);
        backgroundFrontImage[backgroundType].SetActive(false);

        backgroundType = type;
        backgroundBackImage[backgroundType].SetActive(true);
        backgroundFrontImage[backgroundType].SetActive(true);
        trainBackgroundManager.SetPlatformSettings();
        //SaveData();
    }

    private void SetScreendoorUnlock()
    {
        if(lineManager.lineCollections[3].lineData.installed[0] || lineManager.lineCollections[3].lineData.installed[1] || lineManager.lineCollections[3].lineData.installed[2])
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
    
    /*public void SaveData()
    {
        EncryptedPlayerPrefs.SetInt("backgroundType", backgroundType);
    }
    public void LoadData()
    {
        backgroundType = EncryptedPlayerPrefs.GetInt("backgroundType");
    }*/
}
