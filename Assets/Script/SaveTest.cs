using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// 저장시스템 테스트 코드
/// </summary>
public class SaveTest : MonoBehaviour
{
    void Start()
    {
        LoadData();
        SaveData();
    }

    public void SaveData()
    {
        Test test = new Test();
        test.aaa = new int[3];
        test.aaa[0] = 1;
        test.aaa[1] = 2;
        test.aaa[2] = 3;

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/testData.dat");
        formatter.Serialize(file, test);
        file.Close();
    }

    public void LoadData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = null;
        Test test;
        try
        {
            file = File.Open(Application.persistentDataPath + "/testData.dat", FileMode.Open);
            if (file != null && file.Length > 0)
            {
                test = (Test)formatter.Deserialize(file);

                Debug.Log(test.aaa[0]);
                Debug.Log(test.aaa[1]);
                Debug.Log(test.aaa[2]);

                file.Close();
            }
            else
            {
                Debug.Log("Cannot load data file.");
            }

        }
        catch
        {
            Debug.Log("Data file does not exist.");
        }
    }
}

[System.Serializable]
public class Test
{
    public int[] aaa;
}
