using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LineDataManager: MonoBehaviour
{
    public LineManager lineManager;
    public MessageManager messageManager;
    public PlayManager playManager;

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
        {
            int stationSize = lineManager.lineCollections[i].lineData.hasStation.Length;
            lineManager.lineCollections[i].SetLoadedData(lineDatas[i]);

            FixAll(i, stationSize);
        }
    }

    private void FixAll(int line, int stationSize)
    {
        int sectionSize = lineManager.lineCollections[line].purchaseStation.priceData.Sections.Length;
        FixData(ref lineManager.lineCollections[line].lineData.connected, sectionSize);
        FixData(ref lineManager.lineCollections[line].lineData.installed, sectionSize);
        FixData(ref lineManager.lineCollections[line].lineData.sectionExpanded, sectionSize);
        FixData(ref lineManager.lineCollections[line].lineData.hasAllStations, sectionSize);

        FixData(ref lineManager.lineCollections[line].lineData.hasStation, stationSize);

        if (!lineManager.lineCollections[line].purchaseStation.priceData.IsLightRail)
            FixData(ref lineManager.lineCollections[line].lineData.trainExpandStatus, 4);
        else
            FixData(ref lineManager.lineCollections[line].lineData.lineControlLevels, 5);
    }

    private void FixData(ref bool[] data, int sectionSize)
    {
        if (data != null)
        {
            if (sectionSize != data.Length)
                data = new bool[sectionSize];
        }
        else
            data = new bool[sectionSize];
    }

    private void FixData(ref int[] data, int size)
    {
        if (data != null)
        {
            if (size != data.Length)
                data = new int[size];
        }
        else
            data = new int[size];
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

                if (lineDatas[0].numOfTrain == 0)
                    playManager.playData.didLineAdd314 = true;

                if (lineDatas.Length.Equals(4))
                {
                    lineManager.lineCollections[14].lineData = lineDatas[0];
                    lineManager.lineCollections[23].lineData = lineDatas[1];
                    lineManager.lineCollections[24].lineData = lineDatas[2];
                    lineManager.lineCollections[25].lineData = lineDatas[3];

                    if (!playManager.playData.didLineAdd314)
                        InsertNewLines(lineDatas);
                }
                else
                {
                    if (!playManager.playData.didLineAdd314)
                        InsertNewLines(lineDatas);
                    else
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
        catch (System.Exception e)
        {
            /*for(int i = 0; i < lineDatas.Length; i++)
            {
                lineManager.lineCollections[i].InitializeData();
            }*/
            Debug.Log(e.Message);
        }
        InitLine1();
    }

    private void InsertNewLines(LineData[] lineDatas)
    {
        int[] newLines = { 4, 10, 20, 23 };
        int k = 0, n = 0;
        for (int i = 0; i < 29; i++)
        {
            if (i == newLines[n])
            {
                if (n < newLines.Length - 1)
                    n++;
            }
            else if (k == 18)
            {
                lineDatas[20] = ConvertBD2SuinBD(lineDatas[18], lineDatas[20]);
                k++;
                i--;
            }
            else
            {
                lineManager.lineCollections[i].SetLoadedData(lineDatas[k]);
                k++;
            }
        }
        playManager.playData.didLineAdd314 = true;
    }

    private LineData ConvertBD2SuinBD(LineData bdLineData, LineData suinBdLineData)
    {
        LineData convertedData = new LineData();
        convertedData.limitTrain = suinBdLineData.limitTrain;
        convertedData.numOfTrain = suinBdLineData.numOfTrain;
        convertedData.numOfBase = suinBdLineData.numOfBase;
        convertedData.numOfBaseEx = suinBdLineData.numOfBaseEx;
        convertedData.hasStation = suinBdLineData.hasStation;
        convertedData.trainExpandStatus = suinBdLineData.trainExpandStatus;
        
        convertedData.installed = new bool[3];
        convertedData.connected = new bool[3];
        convertedData.sectionExpanded = new bool[3];
        convertedData.hasAllStations = new bool[3];

        convertedData.installed[0] = suinBdLineData.installed[0];
        convertedData.installed[1] = suinBdLineData.installed[0];
        convertedData.installed[2] = bdLineData.installed[0];

        convertedData.connected[0] = suinBdLineData.connected[0];
        convertedData.connected[1] = suinBdLineData.connected[0];
        convertedData.connected[2] = bdLineData.connected[0];

        convertedData.sectionExpanded[0] = suinBdLineData.sectionExpanded[0];
        convertedData.sectionExpanded[1] = suinBdLineData.sectionExpanded[0];
        convertedData.sectionExpanded[2] = bdLineData.sectionExpanded[0];
        return convertedData;
    }
}

[System.Serializable]
public class LineData
{
    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public int limitTrain;
    /// <summary>
    /// ���� ����
    /// </summary>
    public int numOfTrain;
    /// <summary>
    /// �������� ���� ����
    /// </summary>
    public int numOfBase;
    /// <summary>
    /// �������� Ȯ�� Ƚ��
    /// </summary>
    public int numOfBaseEx;
    /// <summary>
    /// �� ���� ����
    /// </summary>
    public bool[] hasStation;
    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public bool[] connected;
    /// <summary>
    /// ������ ���� ��� �����ߴ��� ����
    /// </summary>
    public bool[] hasAllStations;
    /// <summary>
    /// ���� ��ũ������ ��ġ ����
    /// </summary>
    public bool[] installed;
    /// <summary>
    /// ���� Ȯ��
    /// </summary>
    public int[] trainExpandStatus = new int[4];
    /// <summary>
    /// �뼱 ���� ���� ����
    /// </summary>
    public int[] lineControlLevels;
    /// <summary>
    /// Ȯ��� ���� ����
    /// </summary>
    public bool[] sectionExpanded;


}