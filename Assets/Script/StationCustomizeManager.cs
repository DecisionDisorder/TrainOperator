using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StationCustomizeManager : MonoBehaviour
{
    public GameObject changeNameMenu;
    public Dropdown currentStationDropdown;
    public Text priorStationExampleText;
    public Text nextStationExampleText;
    private string[] lineNames;
    private string[] stationNames;

    public Text[] currentStationTexts;
    public Text[] priorStationTexts;
    public Text[] nextStationTexts;
    public GameObject[] priorStationMarks;
    public GameObject[] nextStationMarks;
    public RectTransform[] currentStationRects;
    public float textMargin = 20f;

    public Color[] lineColors;
    public Image[] lineColorImages;
    public TrainColor[] trainColors;
    public Image[] trainDoorTops;
    public Image[] trainDoorBottoms;
    public Image[] trainTopBars;

    public Dropdown currentLineDropdown;

    public int CurrentLineIndex { get { return settingManager.settingData.currentLineIndex; } set { settingManager.settingData.currentLineIndex = value; } }
    public int CurrentStationIndex { get { return settingManager.settingData.currentStationIndex; } set { settingManager.settingData.currentStationIndex = value; } }
    public bool IsStationReversed { get { return settingManager.settingData.isStationReversed; } set { settingManager.settingData.isStationReversed = value; } }
    private bool reverseSelected = false;

    public LineManager lineManager;
    public SettingManager settingManager;

    private void Start()
    {
        InitStationInfo();
    }

    private void InitStationInfo()
    {
        currentLineDropdown.value = CurrentLineIndex;
        SetStationNames(CurrentLineIndex);
        SelectName();
        ApplyToUI();
    }

    private int GetAvailableLineLength()
    {
        int count = 0;
        for(int i = 0; i < lineManager.lineCollections.Length; i++)
        {
            if (lineManager.lineCollections[i].isExpanded())
                count++;
            else
                break;
        }
        return count;
    }

    private void SetLineNames()
    {
        int lineLength = GetAvailableLineLength();
        lineNames = new string[lineLength];
        currentLineDropdown.ClearOptions();
        for(int i = 0; i < lineLength; i++)
        {
            lineNames[i] = lineManager.lineCollections[i].purchaseTrain.lineName;
            currentLineDropdown.options.Add(new Dropdown.OptionData(lineNames[i]));
        }
        currentLineDropdown.value = CurrentLineIndex;
        currentLineDropdown.RefreshShownValue();

    }

    private void SetStationNames(int lineIndex)
    {
        StationNames[] names = lineManager.lineCollections[lineIndex].purchaseStation.stationNameData.stations;
        stationNames = new string[names.Length];
        currentStationDropdown.ClearOptions();
        for (int i = 0; i < stationNames.Length; i++)
        {
            stationNames[i] = names[i].name;
            currentStationDropdown.options.Add(new Dropdown.OptionData(stationNames[i]));
        }
        currentStationDropdown.value = CurrentStationIndex;
        reverseSelected = IsStationReversed;
        currentStationDropdown.RefreshShownValue();
    }

    public void SelectLine()
    {
        int lineIndexTemp = currentLineDropdown.value;
        SetStationNames(lineIndexTemp);
        SelectName();
    }

    public void SelectName()
    {
        int index = currentStationDropdown.value;
        if(0 < index)
        {
            if(!IsStationReversed)
                priorStationExampleText.text = stationNames[index - 1];
            else
                nextStationExampleText.text = stationNames[index - 1];
        }
        else
        {
            if (!IsStationReversed)
                priorStationExampleText.text = "";
            else
                nextStationExampleText.text = "";
        }

        if (index < stationNames.Length - 1)
        {
            if (!IsStationReversed)
                nextStationExampleText.text = stationNames[index + 1];
            else
                priorStationExampleText.text = stationNames[index + 1];
        }
        else
        {
            if (!IsStationReversed)
                nextStationExampleText.text = "";
            else
                priorStationExampleText.text = "";
        }
    }

    public void ExchangeName()
    {
        string temp = priorStationExampleText.text;
        priorStationExampleText.text = nextStationExampleText.text;
        nextStationExampleText.text = temp;
        reverseSelected = !reverseSelected;
    }

    public void Apply()
    {
        CurrentStationIndex = currentStationDropdown.value;
        IsStationReversed = reverseSelected;
        CurrentLineIndex = currentLineDropdown.value;

        ApplyToUI();

        SetChangeNameMenu(false);
    }

    private void ApplyToUI()
    {
        UpdateStationName(stationNames[CurrentStationIndex], currentStationTexts);
        for (int i = 0; i < currentStationRects.Length; i++)
        {
            var rectSize = currentStationRects[i].sizeDelta;
            rectSize.x = currentStationTexts[i].preferredWidth + textMargin;
            currentStationRects[i].sizeDelta = rectSize;
        }

        UpdateStationName(priorStationExampleText.text, priorStationTexts);
        UpdateStationMark(priorStationExampleText.text, priorStationMarks);

        UpdateStationName(nextStationExampleText.text, nextStationTexts);
        UpdateStationMark(nextStationExampleText.text, nextStationMarks);

        for (int i = 0; i < lineColorImages.Length; i++)
            lineColorImages[i].color = lineColors[CurrentLineIndex];
        for (int i = 0; i < trainDoorTops.Length; i++)
            trainDoorTops[i].color = trainColors[CurrentLineIndex].doorColorTop;
        for (int i = 0; i < trainDoorBottoms.Length; i++)
            trainDoorBottoms[i].color = trainColors[CurrentLineIndex].doorColorBottom;
        for (int i = 0; i < trainTopBars.Length; i++)
            trainTopBars[i].color = trainColors[CurrentLineIndex].barColor;
    }

    private void UpdateStationName(string name, Text[] texts)
    {
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].text = name;
        }
    }

    private void UpdateStationMark(string name, GameObject[] stationmarks)
    {
        for (int i = 0; i < stationmarks.Length; i++)
        {
            if (name.Equals(""))
                stationmarks[i].SetActive(false);
            else
                stationmarks[i].SetActive(true);
        }
    }

    public void SetChangeNameMenu(bool active)
    {
        SetLineNames();
        changeNameMenu.SetActive(active);
    }
}

[System.Serializable]
public class TrainColor
{
    public Color doorColorTop;
    public Color doorColorBottom;
    public Color barColor;
}