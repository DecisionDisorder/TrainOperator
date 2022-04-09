using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCollection : MonoBehaviour
{
    public Line line;

    public LineData lineData;
    public PurchaseTrainManager purchaseTrain;
    public PurchaseStation purchaseStation;
    public ExpandPurchase expandPurchase;
    public LineConnection lineConnection;
    public ExpandTrain expandTrain;
    public SetupScreenDoor setupScreenDoor;
    public UpdateManager updateManager;

    public string addedStationFileName;
    private AddedStationData addedStationData;

    public void InitializeData()
    {
        lineData = new LineData();//stationPurchase.stationImgs.Length, lineConnection.sectionImages.Length);
    }

    /// <summary>
    /// 노선 데이터를 적용하기 전 역을 삽입할 부분을 삽입한 후 적용하는 함수
    /// </summary>
    /// <param name="loadedData"></param>
    public void SetLoadedData(LineData loadedData)
    {
        if (addedStationFileName != "")
        {
            InsertAddedStations(loadedData);
        }
        else
            lineData = loadedData;
    }

    private void InsertAddedStations(LineData loadedData)
    {
        LoadAddedStationData();
        if (loadedData.hasStation.Length != lineData.hasStation.Length)
        {
            List<List<AddedStationInfo>> addedList = GetAddedStationList();

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

    private List<List<AddedStationInfo>> GetAddedStationList()
    {
        List<List<AddedStationInfo>> addedList = new List<List<AddedStationInfo>>();
        List<int> addedVersions = new List<int>();

        for(int i = 0; i < addedStationData.stations.Length; i++)
        {
            if (updateManager.RecentViewedVersionCode < addedStationData.stations[i].ver)
            {
                if (addedList.Count == 0)
                {
                    addedList.Add(new List<AddedStationInfo>());
                    addedList[0].Add(addedStationData.stations[i]);
                    addedVersions.Add(addedStationData.stations[i].ver);
                }
                else
                {
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
    public bool isExpanded()
    {
        for (int i = 0; i < lineData.sectionExpanded.Length; i++)
            if (lineData.sectionExpanded[i])
                return true;
        return false;
    }

    public int GetExpandedAmount()
    {
        int amount = 0;
        for (int i = 0; i < lineData.sectionExpanded.Length; i++)
            if (lineData.sectionExpanded[i])
                amount++;
        return amount;
    }

    public int GetConnectionAmount()
    {
        int amount = 0;
        for (int i = 0; i < lineData.connected.Length; i++)
            if (lineData.connected[i])
                amount++;
        return amount;
    }

    public int GetScreendoorAmount()
    {
        int amount = 0;
        for (int i = 0; i < lineData.installed.Length; i++)
            if (lineData.installed[i])
                amount++;
        return amount;
    }

    public int GetTrainExpandAmount()
    {
        return lineData.trainExpandStatus[1] + lineData.trainExpandStatus[2] * 2 + lineData.trainExpandStatus[3] * 3;
    }
}

[System.Serializable]
public class AddedStationData
{
    public AddedStationInfo[] stations;
}

[System.Serializable]
public class AddedStationInfo
{
    public int index;
    public int ver;
}