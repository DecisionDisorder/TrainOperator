using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �޴� Ȱ��ȭ�� ������Ʈ ����� Ŭ����
/// </summary>
public class UpdateDisplay : MonoBehaviour
{
    public delegate void OnEnableUpdate();
    /// <summary>
    /// Ȱ��ȭ ��� 1ȸ ������Ʈ �ݹ� ��������Ʈ
    /// </summary>
    public OnEnableUpdate onEnable;
    /// <summary>
    /// Ȱ��ȭ �Ǿ� �ִ� ���� ������ ������Ʈ �ݹ� ��������Ʈ
    /// </summary>
    public OnEnableUpdate onEnableUpdate;

    /// <summary>
    /// ������Ʈ �ڷ�ƾ
    /// </summary>
    IEnumerator updateContents;

    private void OnEnable()
    {
        // ��ϵ� 1ȸ ������Ʈ �Լ��� ������ ����
        if (onEnable != null)
        {
            onEnable();
        }
        // ��ϵ� ������ ������Ʈ �Լ��� ������ ������Ʈ ����
        if(onEnableUpdate != null)
        {
            StartCoroutine(updateContents = UpdateContents());
        }
    }

    private void OnDisable()
    {
        // �ش� ������Ʈ�� ��Ȱ��ȭ �Ǹ� ������Ʈ �ߴ�
        if (updateContents != null)
            StopCoroutine(updateContents);
    }

    IEnumerator UpdateContents()
    {
        yield return new WaitForSeconds(0.1f);

        onEnableUpdate();

        StartCoroutine(updateContents = UpdateContents());
    }
}
