using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public class SerializeData : MonoBehaviour
{
    private static BinaryFormatter binaryFormatter = new BinaryFormatter();

    public static string SetSerialization(object obj)
    {
        MemoryStream memorystream = new MemoryStream();
        binaryFormatter.Serialize(memorystream, obj);

        return Convert.ToBase64String(memorystream.ToArray());
    }

    public static object GetDeSerialization(string sampleSerializationed)
    {

        MemoryStream memorystream = new MemoryStream(Convert.FromBase64String(sampleSerializationed));
        return binaryFormatter.Deserialize(memorystream);
    }

    public static byte[] StringToByte(string str)
    {
        byte[] strByte = System.Text.Encoding.UTF8.GetBytes(str);
        return strByte;
    }

    public static string ByteToString(byte[] bytes)
    {
        System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
        string str = enc.GetString(bytes);
        return str;
    }
}
