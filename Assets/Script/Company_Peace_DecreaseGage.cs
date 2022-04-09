using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Company_Peace_DecreaseGage : MonoBehaviour {

	public GameObject AfterAccept_Menu;
	public Image Increasing_Gage;

    private void Start()
    {
        StartCoroutine(ItdUpdate());
    }

    IEnumerator ItdUpdate()
    {
        yield return new WaitForSeconds(0.5f);

        if (AfterAccept_Menu.activeInHierarchy)
        {

            if (PeaceManager.Gage > 0)
            {
                float x = Increasing_Gage.transform.position.x;
                float y = Increasing_Gage.transform.position.y;
                Increasing_Gage.GetComponent<RectTransform>().sizeDelta -= new Vector2(4, 0);
                Increasing_Gage.transform.position = new Vector3(x - 2.4929f, y, 0);
                PeaceManager.Gage -= 4;
            }
            else if (PeaceManager.Gage <= 0)
            {

            }
        }

        StartCoroutine(ItdUpdate());
    }
}
