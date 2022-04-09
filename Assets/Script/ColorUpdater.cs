using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ButtonColorUpdate();
public delegate void ButtonColorUpdateLine(int line);
public class ColorUpdater: MonoBehaviour
{
    IEnumerator updateButtonColor;

    public void StartUpdateButtonColor(ButtonColorUpdateLine colorUpdateLine, int line)
    {
        updateButtonColor = UpdateButtonColor(colorUpdateLine, line);
        StartCoroutine(updateButtonColor);
    }
    public void StartUpdateButtonColor(ButtonColorUpdate colorUpdate)
    {
        updateButtonColor = UpdateButtonColor(colorUpdate);
        StartCoroutine(updateButtonColor);
    }
    public void StopUpdateButtonColor()
    {
        if (updateButtonColor != null)
            StopCoroutine(updateButtonColor);
    }

    IEnumerator UpdateButtonColor(ButtonColorUpdate colorUpdate)
    {
        yield return new WaitForSeconds(0.1f);

        colorUpdate();

        updateButtonColor = UpdateButtonColor(colorUpdate);
        StartCoroutine(updateButtonColor);
    }
    IEnumerator UpdateButtonColor(ButtonColorUpdateLine colorUpdateLine, int line)
    {
        yield return new WaitForSeconds(0.1f);

        colorUpdateLine(line);

        updateButtonColor = UpdateButtonColor(colorUpdateLine, line);
        StartCoroutine(updateButtonColor);
    }
}
