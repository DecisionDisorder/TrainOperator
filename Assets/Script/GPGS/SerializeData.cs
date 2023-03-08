using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// �����͸� ����ȭ�ϴ� Ŭ����
/// </summary>
public class SerializeData : MonoBehaviour
{
    /// <summary>
    /// 2������ ��ȯ���ִ� Formatter
    /// </summary>
    private static BinaryFormatter binaryFormatter = new BinaryFormatter();

    /// <summary>
    /// ������Ʈ�� ����Ʈ �迭�� ����ȭ
    /// </summary>
    /// <param name="obj">����ȭ ��� ������Ʈ</param>
    /// <returns></returns>
    public static string SetSerialization(object obj)
    {
        MemoryStream memorystream = new MemoryStream();
        binaryFormatter.Serialize(memorystream, obj);

        return Convert.ToBase64String(memorystream.ToArray());
    }

    /// <summary>
    /// ������ȭ �Ͽ� ������Ʈ�� ����� �Լ�
    /// </summary>
    /// <param name="sampleSerializationed">������ȭ ��� ������</param>
    /// <returns>������ ������Ʈ ������</returns>
    public static object GetDeserialization(string sampleSerializationed)
    {
        MemoryStream memorystream = new MemoryStream(Convert.FromBase64String(sampleSerializationed));
        return binaryFormatter.Deserialize(memorystream);
    }

    /// <summary>
    /// string���� byte �迭�� ��ȯ
    /// </summary>
    /// <param name="str">��ȯ ��� ���ڿ�</param>
    /// <returns>��ȯ�� byte �迭</returns>
    public static byte[] StringToByte(string str)
    {
        byte[] strByte = System.Text.Encoding.UTF8.GetBytes(str);
        return strByte;
    }

    /// <summary>
    /// byte �迭���� string���� ��ȯ
    /// </summary>
    /// <param name="bytes">��ȯ ��� byte �迭</param>
    /// <returns>��ȯ�� ���ڿ�</returns>
    public static string ByteToString(byte[] bytes)
    {
        System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
        string str = enc.GetString(bytes);
        return str;
    }
}
