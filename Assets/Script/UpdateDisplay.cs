using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
