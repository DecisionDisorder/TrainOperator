using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 메뉴 활성화시 업데이트 도우미 클래스
/// </summary>
public class UpdateDisplay : MonoBehaviour
{
    public delegate void OnEnableUpdate();
    /// <summary>
    /// 활성화 당시 1회 업데이트 콜백 델리게이트
    /// </summary>
    public OnEnableUpdate onEnable;
    /// <summary>
    /// 활성화 되어 있는 동안 지속적 업데이트 콜백 델리게이트
    /// </summary>
    public OnEnableUpdate onEnableUpdate;

    /// <summary>
    /// 업데이트 코루틴
    /// </summary>
    IEnumerator updateContents;

    private void OnEnable()
    {
        // 등록된 1회 업데이트 함수가 있으면 실행
        if (onEnable != null)
        {
            onEnable();
        }
        // 등록된 지속적 업데이트 함수가 있으면 업데이트 시작
        if(onEnableUpdate != null)
        {
            StartCoroutine(updateContents = UpdateContents());
        }
    }

    private void OnDisable()
    {
        // 해당 오브젝트가 비활성화 되면 업데이트 중단
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
