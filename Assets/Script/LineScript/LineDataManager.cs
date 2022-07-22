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