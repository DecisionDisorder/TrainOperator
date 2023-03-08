using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 각 노선에 대한 노선 관리 클래스 모음
/// </summary>
public class LineCollection : MonoBehaviour
{
    /// <summary>
    /// 대상 노선
    /// </summary>
    public Line line;

    public LineData lineData;
    /// <summary>
    /// 노선 제어 레벨(경전철 업그레이드)
    /// </summary>
    public int[] LineControlLevels { set { lineData.lineControlLevels = value; } 
        get { 
            if (lineData.lineControlLevels == null && purchaseStation.priceData.IsLightRail)
                lineData.lineControlLevels = new int[5]; 
            return lineData.lineControlLevels; 
        } 
    }

    public PurchaseTrainManager purchaseTrain;
    public PurchaseStation purchaseStation;
    public ExpandPurchase expandPurchase;
    public LineConnection lineConnection;
    public ExpandTrain expandTrain;
    public SetupScreenDoor setupScreenDoor;
    public UpdateManager updateManager;

    /// <summary>
    /// 중간에 추가된 노선 파일 이름
    /// </summary>
    public string addedStationFileName;
    [SerializeField]
    private AddedStationData addedStationData;

    /// <summary>
    /// 노선 데이터 초기화
    /// </summary>
    public void InitializeData()
    {
        lineData = new LineData();//stationPurchase.stationImgs.Length, lineConnection.sectionImages.Length);
    }

    /// <summary>
    /// 노선 데이터를 적용하기 전 역을 삽입할 부분을 삽입한 후 적용하는 함수
    /// </summary>
    /// <param name="loadedData">로드되어 있는 기존 데이터</param>
    public void SetLoadedData(LineData loadedData)
    {
        if (addedStationFileName != "")
        {
            InsertAddedStations(loadedData);
        }
        else
            lineData = loadedData;
    }

    /// <summary>
    /// 추가된 노선 중간에 삽입
    /// </summary>
    /// <param name="loadedData">기존 데이터</param>
    private void InsertAddedStations(LineData loadedData)
    {
        // 추가할 노선 로드
        LoadAddedStationData();
        // 기존 데이터와 신규 데이터의 역 개수가 다르면 추가 작업 시작
        if (loadedData.hasStation.Length != lineData.hasStation.Length)
        {
            // 추가할 역의 리스트를 받아온다.
            List<List<AddedStationInfo>> addedList = GetAddedStationList();

            // 리스트 형태로 변환 후, 추가될 인덱스에 삽입을 하고 다시 배열 형태로 변환한다.
            for (int v = 0; v < addedList.Count; v++)
            {
                List<bool> hasStations = new List<bool>(loadedData.hasStation);
                for (int i = 0; i < addedList[v].Count; i++)
                    hasStations.Insert(addedList[v][i].index, false);
                loadedData.hasStation = hasStations.ToArray();
            }
            lineData = loadedData;
        }
        else
            lineData = loadedData;
    }

    /// <summary>
    /// 추가할 역 데이터를 불러온다.
    /// </summary>
    private void LoadAddedStationData()
    {
        TextAsset mytxtData = Resources.Load<TextAsset>("Json/AddedStations/" + addedStationFileName);
        string txt = mytxtData.text;
        if (txt != "" && txt != null)
        {
            string dataAsJson = txt;
            addedStationData = JsonUtility.FromJson<AddedStationData>(dataAsJson);
        }
    }

    /// <summary>
    /// 추가할 역 리스트를 받아온다.
    /// </summary>
    private List<List<AddedStationInfo>> GetAddedStationList()
    {
        List<List<AddedStationInfo>> addedList = new List<List<AddedStationInfo>>();
        List<int> addedVersions = new List<int>();

        // 추가할 모든 역에 대해 검사
        for(int i = 0; i < addedStationData.stations.Length; i++)
        {
            // 추가된 버전 확인
            if (updateManager.RecentViewedVersionCode < addedStationData.stations[i].ver)
            {
                // 리스트가 비어있으면 초기 데이터 세팅
                if (addedList.Count == 0)
                {
                    addedList.Add(new List<AddedStationInfo>());
                    addedList[0].Add(addedStationData.stations[i]);
                    addedVersions.Add(addedStationData.stations[i].ver);
                }
                else
                {
                    // 역이 추가된 버전별로 묶으며 리스트에 추가
                    if (addedVersions[addedVersions.Count - 1] >= addedStationData.stations[i].ver)
                    {
                        for (int j = 0; j < addedVersions.Count; j++)
                        {
                            if (addedVersions[j].Equals(addedStationData.stations[i].ver))
                            {
                                addedList[j].Add(addedStationData.stations[i]);
                            }
                        }
                    }
                    else
                    {
                        addedList.Add(new List<AddedStationInfo>());
                        addedList[addedVersions.Count].Add(addedStationData.stations[i]);
                        addedVersions.Add(addedStationData.stations[i].ver);
                    }
                }
            }
        }

        return addedList;
    }


    /// <summary>
    /// 해당 노선의 확장권을 하나라도 보유했는지 확인하는 함수
    /// </summary>
    public bool IsExpanded()
    {
        for (int i = 0; i < lineData.sectionExpanded.Length; i++)
            if (lineData.sectionExpanded[i])
                return true;
        return false;
    }

    /// <summary>
    /// 해당 노선에서 하나라도 스크린도어가 설치되었는지 확인하는 함수
    /// </summary>
    public bool IsScreendoorInstalled()
    {
        for (int i = 0; i < lineData.installed.Length; i++)
        {
            if (lineData.installed[i])
                return true;
        }

        return false;
    }

    /// <summary>
    /// 확장한 구간의 개수 구하는 함수
    /// </summary>
    public int GetExpandedAmount()
    {
        int amount = 0;
        for (int i = 0; i < lineData.sectionExpanded.Length; i++)
            if (lineData.sectionExpanded[i])
                amount++;
        return amount;
    }

    /// <summary>
    /// 노선 연결된 구간의 개수를 구하는 함수
    /// </summary>
    /// <returns></returns>
    public int GetConnectionAmount()
    {
        int amount = 0;
        for (int i = 0; i < lineData.connected.Length; i++)
            if (lineData.connected[i])
                amount++;
        return amount;
    }

    /// <summary>
    /// 스크린도어가 설치된 구간의 개수를 구하는 함수
    /// </summary>
    /// <returns></returns>
    public int GetScreendoorAmount()
    {
        int amount = 0;
        for (int i = 0; i < lineData.installed.Length; i++)
            if (lineData.installed[i])
                amount++;
        return amount;
    }

    /// <summary>
    /// 확장된 열차의 칸 수를 구하는 함수
    /// </summary>
    /// <returns></returns>
    public int GetTrainExpandAmount()
    {
        return lineData.trainExpandStatus[1] + lineData.trainExpandStatus[2] * 2 + lineData.trainExpandStatus[3] * 3;
    }
}

/// <summary>
/// 추가된 역들의 데이터
/// </summary>
[System.Serializable]
public class AddedStationData
{
    public AddedStationInfo[] stations;
}

/// <summary>
/// 추가된 역의 인덱스와 버전
/// </summary>
[System.Serializable]
public class AddedStationInfo
{
    public int index;
    public int ver;
}