using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �°��� ��� �̹��� ���� ���� Ŭ����
/// </summary>
public class StationCustomizeManager : MonoBehaviour
{
    /// <summary>
    /// �̸� ���� �޴� ������Ʈ
    /// </summary>
    public GameObject changeNameMenu;
    /// <summary>
    /// ��� �� ���� ��Ӵٿ�
    /// </summary>
    public Dropdown currentStationDropdown;
    /// <summary>
    /// ���� �� ���� �ؽ�Ʈ
    /// </summary>
    public Text priorStationExampleText;
    /// <summary>
    /// ���� �� ���� �ؽ�Ʈ
    /// </summary>
    public Text nextStationExampleText;
    /// <summary>
    /// �뼱 �̸� �迭
    /// </summary>
    private string[] lineNames;
    /// <summary>
    /// �� �̸� �迭
    /// </summary>
    private string[] stationNames;

    /// <summary>
    /// ���� �� �̸��� ǥ���� �ؽ�Ʈ �迭
    /// </summary>
    public Text[] currentStationTexts;
    /// <summary>
    /// ���� �� �̸��� ǥ���� �ؽ�Ʈ �迭
    /// </summary>
    public Text[] priorStationTexts;
    /// <summary>
    /// ���� �� �̸��� ǥ���� �ؽ�Ʈ �迭
    /// </summary>
    public Text[] nextStationTexts;
    /// <summary>
    /// ��ũ������ ���� ���� ���� ���� ��ũ ������Ʈ �迭
    /// </summary>
    public GameObject[] priorStationMarks;
    /// <summary>
    /// ��ũ������ ���� ���� ���� ���� ��ũ ������Ʈ �迭
    /// </summary>
    public GameObject[] nextStationMarks;
    /// <summary>
    /// ���� �� �̸� ���̿� ���� ũ�� ������ ���� RectTransform
    /// </summary>
    public RectTransform[] currentStationRects;
    /// <summary>
    /// �ؽ�Ʈ ������ ���� ���� ũ��
    /// </summary>
    public float textMargin = 20f;

    /// <summary>
    /// �뼱 ��ǥ ���� �迭
    /// </summary>
    public Color[] lineColors;
    /// <summary>
    /// �뼱 ������ ������ �̹��� �迭
    /// </summary>
    public Image[] lineColorImages;
    /// <summary>
    /// ���� ���� ������ ���� ������Ʈ �迭
    /// </summary>
    public TrainColor[] trainColors;
    /// <summary>
    /// ���� ���Թ� ���� ��� �̹��� �迭
    /// </summary>
    public Image[] trainDoorTops;
    /// <summary>
    /// ���� ���Թ� ���� �ϴ� �̹��� �迭
    /// </summary>
    public Image[] trainDoorBottoms;
    /// <summary>
    /// ���� ���Թ� ���� ����Ʈ ������ ���� �̹��� �迭
    /// </summary>
    public Image[] trainTopBars;

    /// <summary>
    /// ��濡 ������ �뼱 ���� ��Ӵٿ�
    /// </summary>
    public Dropdown currentLineDropdown;

    /// <summary>
    /// ���� ��濡 ���� ���� �뼱 �ε���
    /// </summary>
    public int CurrentLineIndex { get { return settingManager.settingData.currentLineIndex; } set { settingManager.settingData.currentLineIndex = value; } }
    /// <summary>
    /// ���� ��濡 ���� ���� �� �ε���
    /// </summary>
    public int CurrentStationIndex { get { return settingManager.settingData.currentStationIndex; } set { settingManager.settingData.currentStationIndex = value; } }
    /// <summary>
    /// ������/���翪 ���� ���� ����
    /// </summary>
    public bool IsStationReversed { get { return settingManager.settingData.isStationReversed; } set { settingManager.settingData.isStationReversed = value; } }
    /// <summary>
    /// ������/���翪 ���� ���� ���ο� ���� �ӽ� ����
    /// </summary>
    private bool reverseSelected = false;

    public LineManager lineManager;
    public SettingManager settingManager;

    private void Start()
    {
        InitStationInfo();
    }

    /// <summary>
    /// �°��� ��� �ʱ�ȭ
    /// </summary>
    private void InitStationInfo()
    {
        currentLineDropdown.value = CurrentLineIndex;
        SetLineNames();
        SetStationNames(CurrentLineIndex);
        SelectName();
        ApplyToUI();
    }

    /// <summary>
    /// ������ �뼱���� ��� ����
    /// </summary>
    /// <param name="line">������ �뼱</param>
    public void SetBackgroundToRecentLine(Line line)
    {
        CurrentLineIndex = (int)line;
        InitStationInfo();
    }

    /// <summary>
    /// �̿� ������ �뼱 ������ ���´�.
    /// </summary>
    /// <returns>���� �뼱�� true�� bool�� �迭</returns>
    private bool[] GetExpandedLines()
    {
        bool[] expandedLines = new bool[lineManager.lineCollections.Length];
        for(int i = 0; i < lineManager.lineCollections.Length; i++)
        {
            if (lineManager.lineCollections[i].IsExpanded())
                expandedLines[i] = true;
            else
                expandedLines[i] = false;
        }
        return expandedLines;
    }

    /// <summary>
    /// �̿� ������ �뼱�� ������ ���´�.
    /// </summary>
    /// <returns>�̿� ������ �뼱�� ����</returns>
    private int GetAvailableLineLength()
    {
        int len = 0;
        for (int i = 0; i < lineManager.lineCollections.Length; i++)
        {
            if (lineManager.lineCollections[i].IsExpanded())
                len++;
        }
        return len;
    }

    /// <summary>
    /// ��濡 ������ �뼱�� �����ϴ� Dropdown ������Ʈ
    /// </summary>
    private void SetLineNames()
    {
        int lineLength = GetAvailableLineLength();
        lineNames = new string[lineLength];
        currentLineDropdown.ClearOptions();
        bool[] expandedLines = GetExpandedLines();

        for(int i = 0, j = 0; i < lineManager.lineCollections.Length; i++)
        {
            if (expandedLines[i])
            {
                lineNames[j] = lineManager.lineCollections[i].purchaseTrain.lineName;
                currentLineDropdown.options.Add(new Dropdown.OptionData(lineNames[j++]));
            }
        }
        currentLineDropdown.value = GetExpandedIndexByLineIndex(CurrentLineIndex);
        currentLineDropdown.RefreshShownValue();

    }

    /// <summary>
    /// ��ܿ� ������ �� �̸��� �����ϴ� Dropdown ������Ʈ
    /// </summary>
    /// <param name="lineIndex">���õ� �뼱 �ε���</param>
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

    /// <summary>
    /// Dropdown���� ���õ� ��ȣ�� ���� �뼱 ���� �ε����� ��ȯ
    /// </summary>
    /// <param name="dropdownIndex">Dropdown������ �ε���</param>
    /// <returns>�뼱 ���� �ε���</returns>
    private int GetLineIndexByExpandedIndex(int dropdownIndex)
    {
        for(int i = 0; i < lineManager.lineCollections.Length; i++)
        {
            if (lineManager.lineCollections[i].purchaseTrain.lineName.Equals(currentLineDropdown.options[dropdownIndex].text))
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// �뼱 ���� �ε����� Dropdown���� �ε����� ��ȯ
    /// </summary>
    /// <param name="lineIndex">�뼱 ���� �ε���</param>
    /// <returns>Dropdown���� �ε���</returns>
    private int GetExpandedIndexByLineIndex(int lineIndex)
    {
        for (int i = 0; i < currentLineDropdown.options.Count; i++)
        {
            if (lineManager.lineCollections[lineIndex].purchaseTrain.lineName.Equals(currentLineDropdown.options[i].text))
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// �뼱 ����
    /// </summary>
    public void SelectLine()
    {
        int lineIndexTemp = GetLineIndexByExpandedIndex(currentLineDropdown.value);
        SetStationNames(lineIndexTemp);
        SelectName();
    }

    /// <summary>
    /// �� �̸� ����
    /// </summary>
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

    /// <summary>
    /// ������/������ ��ġ ��ȯ
    /// </summary>
    public void ExchangeName()
    {
        string temp = priorStationExampleText.text;
        priorStationExampleText.text = nextStationExampleText.text;
        nextStationExampleText.text = temp;
        reverseSelected = !reverseSelected;
    }

    /// <summary>
    /// ��� ������� ����
    /// </summary>
    public void Apply()
    {
        CurrentStationIndex = currentStationDropdown.value;
        IsStationReversed = reverseSelected;
        CurrentLineIndex = GetLineIndexByExpandedIndex(currentLineDropdown.value);

        ApplyToUI();

        SetChangeNameMenu(false);
    }

    /// <summary>
    /// ��� ��������� ��� �̹���/�ؽ�Ʈ�� ����
    /// </summary>
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

    /// <summary>
    /// �� �̸��� ��� ���� ��� �� �̸� �ؽ�Ʈ�� ����
    /// </summary>
    /// <param name="name"></param>
    /// <param name="texts"></param>
    private void UpdateStationName(string name, Text[] texts)
    {
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].text = name;
        }
    }

    /// <summary>
    /// ������/������ ������ ���� �뼱 ��ũ �̹��� Ȱ��ȭ/��Ȱ��ȭ
    /// </summary>
    /// <param name="name">������/������ �̸�</param>
    /// <param name="stationmarks">�뼱 ��ũ ������Ʈ �迭</param>
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
    /// <summary>
    /// �̸� ���� �޴� Ȱ��ȭ/��Ȱ��ȭ
    /// </summary>
    /// <param name="active">Ȱ��ȭ ����</param>
    public void SetChangeNameMenu(bool active)
    {
        SetLineNames();
        changeNameMenu.SetActive(active);
    }
}

/// <summary>
/// ���� ���� ������ ���� Ŭ����
/// </summary>
[System.Serializable]
public class TrainColor
{
    /// <summary>
    /// ���� ���Թ� ��� ����
    /// </summary>
    public Color doorColorTop;
    /// <summary>
    /// ���� ���Թ� �ϴ� ����
    /// </summary>
    public Color doorColorBottom;
    /// <summary>
    /// ���� ���Թ� ���� �� ����
    /// </summary>
    public Color barColor;
}