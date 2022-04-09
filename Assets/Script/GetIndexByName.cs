using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetIndexByName : MonoBehaviour
{
    public int GetIndex(string keyword)
    {
        string name = gameObject.name;
        return int.Parse(name.Substring(name.IndexOf(keyword) + keyword.Length));

    }
}
