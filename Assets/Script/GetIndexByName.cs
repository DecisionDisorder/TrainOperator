using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������Ʈ�� �̸��� ������� �뼱�� �ε����� �����ϴ� Ŭ����
/// </summary>
public class GetIndexByName : MonoBehaviour
{
    // ���� ������Ʈ�� �̸��� ������� �뼱�� �ε����� �����ϴ� �Լ�
    public int GetIndex(string keyword)
    {
        string name = gameObject.name;
        return int.Parse(name.Substring(name.IndexOf(keyword) + keyword.Length));

    }
}
