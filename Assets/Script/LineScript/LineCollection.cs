using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �� �뼱�� ���� �뼱 ���� Ŭ���� ����
/// </summary>
public class LineCollection : MonoBehaviour
{
    /// <summary>
    /// ��� �뼱
    /// </summary>
    public Line line;

    public LineData lineData;
    /// <summary>
    /// �뼱 ���� ����(����ö ���׷��̵�)
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
    /// �߰��� �߰��� �뼱 ���� �̸�
    /// </summary>
    public string addedStationFileName;
    [SerializeField]
    private AddedStationData addedStationData;

    /// <summary>
    /// �뼱 ������ �ʱ�ȭ
    /// </summary>
    public void InitializeData()
    {
        lineData = new LineData();//stationPurchase.stationImgs.Length, lineConnection.sectionImages.Length);
    }

    /// <summary>
    /// �뼱 �����͸� �����ϱ� �� ���� ������ �κ��� ������ �� �����ϴ� �Լ�
    /// </summary>
    /// <param name="loadedData">�ε�Ǿ� �ִ� ���� ������</param>
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
    /// �߰��� �뼱 �߰��� ����
    /// </summary>
    /// <param name="loadedData">���� ������</param>
    private void InsertAddedStations(LineData loadedData)
    {
        // �߰��� �뼱 �ε�
        LoadAddedStationData();
        // ���� �����Ϳ� �ű� �������� �� ������ �ٸ��� �߰� �۾� ����
        if (loadedData.hasStation.Length != lineData.hasStation.Length)
        {
            // �߰��� ���� ����Ʈ�� �޾ƿ´�.
            List<List<AddedStationInfo>> addedList = GetAddedStationList();

            // ����Ʈ ���·� ��ȯ ��, �߰��� �ε����� ������ �ϰ� �ٽ� �迭 ���·� ��ȯ�Ѵ�.
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
    /// �߰��� �� �����͸� �ҷ��´�.
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
    /// �߰��� �� ����Ʈ�� �޾ƿ´�.
    /// </summary>
    private List<List<AddedStationInfo>> GetAddedStationList()
    {
        List<List<AddedStationInfo>> addedList = new List<List<AddedStationInfo>>();
        List<int> addedVersions = new List<int>();

        // �߰��� ��� ���� ���� �˻�
        for(int i = 0; i < addedStationData.stations.Length; i++)
        {
            // �߰��� ���� Ȯ��
            if (updateManager.RecentViewedVersionCode < addedStationData.stations[i].ver)
            {
                // ����Ʈ�� ��������� �ʱ� ������ ����
                if (addedList.Count == 0)
                {
                    addedList.Add(new List<AddedStationInfo>());
                    addedList[0].Add(addedStationData.stations[i]);
                    addedVersions.Add(addedStationData.stations[i].ver);
                }
                else
                {
                    // ���� �߰��� �������� ������ ����Ʈ�� �߰�
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
    /// �ش� �뼱�� Ȯ����� �ϳ��� �����ߴ��� Ȯ���ϴ� �Լ�
    /// </summary>
    public bool IsExpanded()
    {
        for (int i = 0; i < lineData.sectionExpanded.Length; i++)
            if (lineData.sectionExpanded[i])
                return true;
        return false;
    }

    /// <summary>
    /// �ش� �뼱���� �ϳ��� ��ũ����� ��ġ�Ǿ����� Ȯ���ϴ� �Լ�
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
    /// Ȯ���� ������ ���� ���ϴ� �Լ�
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
    /// �뼱 ����� ������ ������ ���ϴ� �Լ�
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
    /// ��ũ����� ��ġ�� ������ ������ ���ϴ� �Լ�
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
    /// Ȯ��� ������ ĭ ���� ���ϴ� �Լ�
    /// </summary>
    /// <returns></returns>
    public int GetTrainExpandAmount()
    {
        return lineData.trainExpandStatus[1] + lineData.trainExpandStatus[2] * 2 + lineData.trainExpandStatus[3] * 3;
    }
}

/// <summary>
/// �߰��� ������ ������
/// </summary>
[System.Serializable]
public class AddedStationData
{
    public AddedStationInfo[] stations;
}

/// <summary>
/// �߰��� ���� �ε����� ����
/// </summary>
[System.Serializable]
public class AddedStationInfo
{
    public int index;
    public int ver;
}