using UnityEngine;
using System.Collections;

public class AutoSaveManager : MonoBehaviour {

    public LineDataManager lineDataManager;
    private void Start()
    {
        StartCoroutine(SaveTimer());
    }

    IEnumerator SaveTimer()
    {
        yield return new WaitForSeconds(10);

        DataManager.instance.SaveAll();

        StartCoroutine(SaveTimer());
    }
}
