using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 파라미터 없는 버튼 색상 업데이트 함수
/// </summary>
public delegate void ButtonColorUpdate();
/// <summary>
/// 노선 파라이터를 포함한 버튼 색상 업데이트 함수
/// </summary>
/// <param name="line">대상 노선</param>
public delegate void ButtonColorUpdateLine(int line);

/// <summary>
/// 주기적으로 색상 업데이트 매니저 클래스
/// </summary>
public class ColorUpdater: MonoBehaviour
{
    IEnumerator updateButtonColor;

    /// <summary>
    /// 구매 버튼 색상 업데이트 코루틴 시작
    /// </summary>
    /// <param name="colorUpdateLine">색상 정보 업데이트 함수</param>
    /// <param name="line">대상 노선</param>
    public void StartUpdateButtonColor(ButtonColorUpdateLine colorUpdateLine, int line)
    {
        updateButtonColor = UpdateButtonColor(colorUpdateLine, line);
        StartCoroutine(updateButtonColor);
    }
    /// <summary>
    /// 구매 버튼 색상 업데이트 코루틴 시작
    /// </summary>
    /// <param name="colorUpdate">색상 정보 업데이트 함수</param>
    public void StartUpdateButtonColor(ButtonColorUpdate colorUpdate)
    {
        updateButtonColor = UpdateButtonColor(colorUpdate);
        StartCoroutine(updateButtonColor);
    }
    /// <summary>
    /// 색상 업데이트 중단
    /// </summary>
    public void StopUpdateButtonColor()
    {
        if (updateButtonColor != null)
            StopCoroutine(updateButtonColor);
    }

    /// <summary>
    /// 구매 버튼 색상 업데이트 코루틴
    /// </summary>
    /// <param name="colorUpdate">색상 정보 업데이트 함수</param>
    IEnumerator UpdateButtonColor(ButtonColorUpdate colorUpdate)
    {
        yield return new WaitForSeconds(0.1f);

        colorUpdate();

        updateButtonColor = UpdateButtonColor(colorUpdate);
        StartCoroutine(updateButtonColor);
    }
    /// <summary>
    /// 구매 버튼 색상 업데이트 코루틴
    /// </summary>
    /// <param name="colorUpdate">색상 정보 업데이트 함수</param>
    /// <param name="line">대상 노선</param>
    IEnumerator UpdateButtonColor(ButtonColorUpdateLine colorUpdateLine, int line)
    {
        yield return new WaitForSeconds(0.1f);

        colorUpdateLine(line);

        updateButtonColor = UpdateButtonColor(colorUpdateLine, line);
        StartCoroutine(updateButtonColor);
    }
}
