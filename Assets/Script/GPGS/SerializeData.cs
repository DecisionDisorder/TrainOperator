using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// 데이터를 직렬화하는 클래스
/// </summary>
public class SerializeData : MonoBehaviour
{
    /// <summary>
    /// 2진수로 변환해주는 Formatter
    /// </summary>
    private static BinaryFormatter binaryFormatter = new BinaryFormatter();

    /// <summary>
    /// 오브젝트를 바이트 배열로 직렬화
    /// </summary>
    /// <param name="obj">직렬화 대상 오브젝트</param>
    /// <returns></returns>
    public static string SetSerialization(object obj)
    {
        MemoryStream memorystream = new MemoryStream();
        binaryFormatter.Serialize(memorystream, obj);

        return Convert.ToBase64String(memorystream.ToArray());
    }

    /// <summary>
    /// 역직렬화 하여 오브젝트를 만드는 함수
    /// </summary>
    /// <param name="sampleSerializationed">역직렬화 대상 데이터</param>
    /// <returns>복구된 오브젝트 데이터</returns>
    public static object GetDeserialization(string sampleSerializationed)
    {
        MemoryStream memorystream = new MemoryStream(Convert.FromBase64String(sampleSerializationed));
        return binaryFormatter.Deserialize(memorystream);
    }

    /// <summary>
    /// string에서 byte 배열로 변환
    /// </summary>
    /// <param name="str">변환 대상 문자열</param>
    /// <returns>변환된 byte 배열</returns>
    public static byte[] StringToByte(string str)
    {
        byte[] strByte = System.Text.Encoding.UTF8.GetBytes(str);
        return strByte;
    }

    /// <summary>
    /// byte 배열에서 string으로 변환
    /// </summary>
    /// <param name="bytes">변환 대상 byte 배열</param>
    /// <returns>변환된 문자열</returns>
    public static string ByteToString(byte[] bytes)
    {
        System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
        string str = enc.GetString(bytes);
        return str;
    }
}
