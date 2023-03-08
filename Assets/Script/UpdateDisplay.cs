using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 위생 조회 메뉴 활성화 이벤트 리스너 오브젝트
/// </summary>
public class UpdateDisplay : MonoBehaviour
{
    public delegate void OnEnableUpdate();
    public OnEnableUpdate onEnable;
    public OnEnableUpdate onEnableUpdate;

    IEnumerator updateContents;

    private void OnEnable()
    {
        if (onEnable != null)
        {
            onEnable();
        }
        if(onEnableUpdate != null)
        {
            StartCoroutine(updateContents = UpdateContents());
        }
    }

    private void OnDisable()
    {
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
