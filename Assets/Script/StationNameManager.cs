using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StationNameManager : MonoBehaviour
{
    public GameObject changeNameMenu;
    public Dropdown currentStationDropdown;
    public Text priorStationExampleText;
    public Text nextStationExampleText;
    private string[] stationNames;

    public Text[] currentStationTexts;
    public Text[] priorStationTexts;
    public Text[] nextStationTexts;
    public GameObject[] priorStationMarks;
    public GameObject[] nextStationMarks;
    public RectTransform[] currentStationRects;
    public float textMargin = 20f;

    public int CurrentStationIndex { get { return settingManager.settingData.currentStationIndex; } set { settingManager.settingData.currentStationIndex = value; } }
    public bool IsStationReversed { get { return settingManager.settingData.isStationReversed; } set { settingManager.settingData.isStationReversed = value; } }
    private bool reverseSelected = false;

    public SettingManager settingManager;
    public PurchaseStation purchaseStation;

    private void Start()
    {
        SetStationNames();
    }

    private void SetStationNames()
    {
        stationNames = new string[purchaseStation.stationNameData.stations.Length];
        for (int i = 0; i < stationNames.Length; i++)
        {
            stationNames[i] = purchaseStation.stationNameData.stations[i].name;
            currentStationDropdown.options.Add(new Dropdown.OptionData(stationNames[i]));
        }
        currentStationDropdown.value = CurrentStationIndex;
        reverseSelected = IsStationReversed;
        SelectName();

        Apply();
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

        UpdateStationName(stationNames[CurrentStationIndex], currentStationTexts);
        for(int i = 0; i < currentStationRects.Length; i++)
        {
            var rectSize = currentStationRects[i].sizeDelta;
            rectSize.x = currentStationTexts[i].preferredWidth + textMargin;
            currentStationRects[i].sizeDelta = rectSize;
        }

        UpdateStationName(priorStationExampleText.text, priorStationTexts);
        UpdateStationMark(priorStationExampleText.text, priorStationMarks);

        UpdateStationName(nextStationExampleText.text, nextStationTexts);
        UpdateStationMark(nextStationExampleText.text, nextStationMarks);
        SetChangeNameMenu(false);
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
        changeNameMenu.SetActive(active);
    }
}
