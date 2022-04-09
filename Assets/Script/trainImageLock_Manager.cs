using UnityEngine;
using System.Collections;

public class trainImageLock_Manager : MonoBehaviour {

    public GameObject[] lockImage_train;

    public GameObject buyTrain_Menu;

    private void Start()
    {
        StartCoroutine(ItdUpdate());
    }

    IEnumerator ItdUpdate()
    {
        yield return new WaitForSeconds(1.0f);

        LineCollection[] lineCollections = LineManager.instance.lineCollections;

        if(buyTrain_Menu.activeInHierarchy)
        {
            for (int i = 0; i < 8; i++)
                if (lineCollections[i + 1].isExpanded())
                    lockImage_train[i].SetActive(false);
        }

        StartCoroutine(ItdUpdate());
    }
}