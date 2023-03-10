using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 승강장 배경 이미지 설정 관리 클래스
/// </summary>
public class StationCustomizeManager : MonoBehaviour
{
    /// <summary>
    /// 이름 변경 메뉴 오브젝트
    /// </summary>
    public GameObject changeNameMenu;
    /// <summary>
    /// 배경 역 선택 드롭다운
    /// </summary>
    public Dropdown currentStationDropdown;
    /// <summary>
    /// 이전 역 예시 텍스트
    /// </summary>
    public Text priorStationExampleText;
    /// <summary>
    /// 다음 역 예시 텍스트
    /// </summary>
    public Text nextStationExampleText;
    /// <summary>
    /// 노선 이름 배열
    /// </summary>
    private string[] lineNames;
    /// <summary>
    /// 역 이름 배열
    /// </summary>
    private string[] stationNames;

    /// <summary>
    /// 현재 역 이름을 표시할 텍스트 배열
    /// </summary>
    public Text[] currentStationTexts;
    /// <summary>
    /// 이전 역 이름을 표시할 텍스트 배열
    /// </summary>
    public Text[] priorStationTexts;
    /// <summary>
    /// 다음 역 이름을 표시할 텍스트 배열
    /// </summary>
    public Text[] nextStationTexts;
    /// <summary>
    /// 스크린도어 위에 이전 역의 원형 마크 오브젝트 배열
    /// </summary>
    public GameObject[] priorStationMarks;
    /// <summary>
    /// 스크린도어 위애 다음 역의 원형 마크 오브젝트 배열
    /// </summary>
    public GameObject[] nextStationMarks;
    /// <summary>
    /// 현재 역 이름 길이에 따른 크기 조절을 위한 RectTransform
    /// </summary>
    public RectTransform[] currentStationRects;
    /// <summary>
    /// 텍스트 내부의 여유 공간 크기
    /// </summary>
    public float textMargin = 20f;

    /// <summary>
    /// 노선 대표 색상 배열
    /// </summary>
    public Color[] lineColors;
    /// <summary>
    /// 노선 색상을 적용할 이미지 배열
    /// </summary>
    public Image[] lineColorImages;
    /// <summary>
    /// 열차 색상 데이터 모음 오브젝트 배열
    /// </summary>
    public TrainColor[] trainColors;
    /// <summary>
    /// 열차 출입문 색상 상단 이미지 배열
    /// </summary>
    public Image[] trainDoorTops;
    /// <summary>
    /// 열차 출입문 색상 하단 이미지 배열
    /// </summary>
    public Image[] trainDoorBottoms;
    /// <summary>
    /// 열차 출입문 위의 포인트 색상을 위한 이미지 배열
    /// </summary>
    public Image[] trainTopBars;

    /// <summary>
    /// 배경에 적용할 노선 선택 드롭다운
    /// </summary>
    public Dropdown currentLineDropdown;

    /// <summary>
    /// 현재 배경에 적용 중인 노선 인덱스
    /// </summary>
    public int CurrentLineIndex { get { return settingManager.settingData.currentLineIndex; } set { settingManager.settingData.currentLineIndex = value; } }
    /// <summary>
    /// 현재 배경에 적용 중인 역 인덱스
    /// </summary>
    public int CurrentStationIndex { get { return settingManager.settingData.currentStationIndex; } set { settingManager.settingData.currentStationIndex = value; } }
    /// <summary>
    /// 이전역/현재역 순서 반전 여부
    /// </summary>
    public bool IsStationReversed { get { return settingManager.settingData.isStationReversed; } set { settingManager.settingData.isStationReversed = value; } }
    /// <summary>
    /// 이전역/현재역 순서 반전 여부에 대한 임시 변수
    /// </summary>
    private bool reverseSelected = false;

    public LineManager lineManager;
    public SettingManager settingManager;

    private void Start()
    {
        InitStationInfo();
    }

    /// <summary>
    /// 승강장 배경 초기화
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
    /// 선택한 노선으로 배경 변경
    /// </summary>
    /// <param name="line">선택한 노선</param>
    public void SetBackgroundToRecentLine(Line line)
    {
        CurrentLineIndex = (int)line;
        InitStationInfo();
    }

    /// <summary>
    /// 이용 가능한 노선 정보를 얻어온다.
    /// </summary>
    /// <returns>열린 노선이 true인 bool형 배열</returns>
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
    /// 이용 가능한 노선의 개수를 얻어온다.
    /// </summary>
    /// <returns>이용 가능한 노선의 개수</returns>
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
    /// 배경에 적용할 노선을 선택하는 Dropdown 업데이트
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
    /// 배겨에 적용할 역 이름을 선택하는 Dropdown 업데이트
    /// </summary>
    /// <param name="lineIndex">선택된 노선 인덱스</param>
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
    /// Dropdown에서 선택된 번호로 실제 노선 상의 인덱스로 변환
    /// </summary>
    /// <param name="dropdownIndex">Dropdown에서의 인덱스</param>
    /// <returns>노선 상의 인덱스</returns>
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
    /// 노선 상의 인덱스를 Dropdown상의 인덱스로 변환
    /// </summary>
    /// <param name="lineIndex">노선 상의 인덱스</param>
    /// <returns>Dropdown상의 인덱스</returns>
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
    /// 노선 선택
    /// </summary>
    public void SelectLine()
    {
        int lineIndexTemp = GetLineIndexByExpandedIndex(currentLineDropdown.value);
        SetStationNames(lineIndexTemp);
        SelectName();
    }

    /// <summary>
    /// 역 이름 선택
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
    /// 이전역/다음역 위치 교환
    /// </summary>
    public void ExchangeName()
    {
        string temp = priorStationExampleText.text;
        priorStationExampleText.text = nextStationExampleText.text;
        nextStationExampleText.text = temp;
        reverseSelected = !reverseSelected;
    }

    /// <summary>
    /// 배경 변경사항 적용
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
    /// 배경 변경사항을 배경 이미지/텍스트에 적용
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
    /// 역 이름을 배경 상의 모든 역 이름 텍스트에 적용
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
    /// 이전역/다음역 유무에 따라 노선 마크 이미지 활성화/비활성화
    /// </summary>
    /// <param name="name">이전역/다음역 이름</param>
    /// <param name="stationmarks">노선 마크 오브젝트 배열</param>
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
    /// 이름 변경 메뉴 활성화/비활성화
    /// </summary>
    /// <param name="active">활성화 여부</param>
    public void SetChangeNameMenu(bool active)
    {
        SetLineNames();
        changeNameMenu.SetActive(active);
    }
}

/// <summary>
/// 열차 색상 데이터 모음 클래스
/// </summary>
[System.Serializable]
public class TrainColor
{
    /// <summary>
    /// 열차 출입문 상단 색상
    /// </summary>
    public Color doorColorTop;
    /// <summary>
    /// 열차 출입문 하단 색상
    /// </summary>
    public Color doorColorBottom;
    /// <summary>
    /// 열차 출입문 위의 바 색상
    /// </summary>
    public Color barColor;
}