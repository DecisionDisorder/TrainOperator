using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LineDataManager: MonoBehaviour
{
    public LineManager lineManager;
    public MessageManager messageManager;

    private void InitLine1()
    {
        if (!lineManager.lineCollections[0].lineData.hasStation[24])
            lineManager.lineCollections[0].lineData.hasStation[24] = lineManager.lineCollections[0].lineData.hasStation[33] = lineManager.lineCollections[0].lineData.hasStation[76] = true;
        if (lineManager.lineCollections[0].lineData.limitTrain.Equals(0))
            lineManager.lineCollections[0].lineData.limitTrain = 10;
        if (!lineManager.lineCollections[0].lineData.sectionExpanded[1])
            lineManager.lineCollections[0].lineData.sectionExpanded[1] = lineManager.lineCollections[0].lineData.sectionExpanded[2] = lineManager.lineCollections[0].lineData.sectionExpanded[4] = true;
    }

    public LineData[] AssembleLineData()
    {
        LineData[] lineDatas = new LineData[lineManager.lineCollections.Length];
        for (int i = 0; i < lineDatas.Length; i++)
            lineDatas[i] = lineManager.lineCollections[i].lineData;

        return lineDatas;
    }

    public void SetLineData(LineData[] lineDatas)
    {
        for (int i = 0; i < lineDatas.Length; i++)
            lineManager.lineCollections[i].SetLoadedData(lineDatas[i]);
    }

    public void SaveData()
    {
        LineData[] lineDatas = AssembleLineData();


        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/dataInfo.dat");
        formatter.Serialize(file, lineDatas);
        file.Close();
    }

    public void LoadData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = null;
        LineData[] lineDatas = new LineData[lineManager.lineCollections.Length];
        try
        {
            file = File.Open(Application.persistentDataPath + "/dataInfo.dat", FileMode.Open);
            if (file != null && file.Length > 0)
            {
                lineDatas = (LineData[])formatter.Deserialize(file);

                if(lineDatas.Length.Equals(4))
                {
                    lineManager.lineCollections[14].lineData = lineDatas[0];
                    lineManager.lineCollections[23].lineData = lineDatas[1];
                    lineManager.lineCollections[24].lineData = lineDatas[2];
                    lineManager.lineCollections[25].lineData = lineDatas[3];
                }
                else
                {
                    SetLineData(lineDatas);
                }
                

                file.Close();
            }
            else
            {
                //Debug.Log("Cannot load data file.");
                messageManager.ShowMessage("[ERROR] Cannot load data file.", 5f);
            }

        }
        catch
        {
            /*for(int i = 0; i < lineDatas.Length; i++)
            {
                lineManager.lineCollections[i].InitializeData();
            }*/
            //Debug.Log("Data file does not exist.");
        }
        InitLine1();
    }
}

[System.Serializable]
public class LineData
{
    /// <summary>
    /// 열차 개수 제한
    /// </summary>
    public int limitTrain;
    /// <summary>
    /// 열차 개수
    /// </summary>
    public int numOfTrain;
    /// <summary>
    /// 차량기지 구매 개수
    /// </summary>
    public int numOfBase;
    /// <summary>
    /// 차량기지 확장 횟수
    /// </summary>
    public int numOfBaseEx;
    /// <summary>
    /// 역 구매 여부
    /// </summary>
    public bool[] hasStation;
    /// <summary>
    /// 구간 연결 여부
    /// </summary>
    public bool[] connected;
    /// <summary>
    /// 구간의 역을 모두 구매했는지 여부
    /// </summary>
    public bool[] hasAllStations;
    /// <summary>
    /// 구간 스크린도어 설치 여부
    /// </summary>
    public bool[] installed;
    /// <summary>
    /// 열차 확장
    /// </summary>
    public int[] trainExpandStatus = new int[4];
    /// <summary>
    /// 노선 제어 관리 레벨
    /// </summary>
    public int[] lineControlLevels;
    /// <summary>
    /// 확장권 보유 여부
    /// </summary>
    public bool[] sectionExpanded;


}