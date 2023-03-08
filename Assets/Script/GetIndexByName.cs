using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 오브젝트의 이름을 기반으로 노선의 인덱스를 추출하는 클래스
/// </summary>
public class GetIndexByName : MonoBehaviour
{
    // 게임 오브젝트의 이름을 기반으로 노선의 인덱스를 추출하는 함수
    public int GetIndex(string keyword)
    {
        string name = gameObject.name;
        return int.Parse(name.Substring(name.IndexOf(keyword) + keyword.Length));

    }
}
