using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Ķ���� ���� ��ư ���� ������Ʈ �Լ�
/// </summary>
public delegate void ButtonColorUpdate();
/// <summary>
/// �뼱 �Ķ����͸� ������ ��ư ���� ������Ʈ �Լ�
/// </summary>
/// <param name="line">��� �뼱</param>
public delegate void ButtonColorUpdateLine(int line);

/// <summary>
/// �ֱ������� ���� ������Ʈ �Ŵ��� Ŭ����
/// </summary>
public class ColorUpdater: MonoBehaviour
{
    IEnumerator updateButtonColor;

    /// <summary>
    /// ���� ��ư ���� ������Ʈ �ڷ�ƾ ����
    /// </summary>
    /// <param name="colorUpdateLine">���� ���� ������Ʈ �Լ�</param>
    /// <param name="line">��� �뼱</param>
    public void StartUpdateButtonColor(ButtonColorUpdateLine colorUpdateLine, int line)
    {
        updateButtonColor = UpdateButtonColor(colorUpdateLine, line);
        StartCoroutine(updateButtonColor);
    }
    /// <summary>
    /// ���� ��ư ���� ������Ʈ �ڷ�ƾ ����
    /// </summary>
    /// <param name="colorUpdate">���� ���� ������Ʈ �Լ�</param>
    public void StartUpdateButtonColor(ButtonColorUpdate colorUpdate)
    {
        updateButtonColor = UpdateButtonColor(colorUpdate);
        StartCoroutine(updateButtonColor);
    }
    /// <summary>
    /// ���� ������Ʈ �ߴ�
    /// </summary>
    public void StopUpdateButtonColor()
    {
        if (updateButtonColor != null)
            StopCoroutine(updateButtonColor);
    }

    /// <summary>
    /// ���� ��ư ���� ������Ʈ �ڷ�ƾ
    /// </summary>
    /// <param name="colorUpdate">���� ���� ������Ʈ �Լ�</param>
    IEnumerator UpdateButtonColor(ButtonColorUpdate colorUpdate)
    {
        yield return new WaitForSeconds(0.1f);

        colorUpdate();

        updateButtonColor = UpdateButtonColor(colorUpdate);
        StartCoroutine(updateButtonColor);
    }
    /// <summary>
    /// ���� ��ư ���� ������Ʈ �ڷ�ƾ
    /// </summary>
    /// <param name="colorUpdate">���� ���� ������Ʈ �Լ�</param>
    /// <param name="line">��� �뼱</param>
    IEnumerator UpdateButtonColor(ButtonColorUpdateLine colorUpdateLine, int line)
    {
        yield return new WaitForSeconds(0.1f);

        colorUpdateLine(line);

        updateButtonColor = UpdateButtonColor(colorUpdateLine, line);
        StartCoroutine(updateButtonColor);
    }
}
