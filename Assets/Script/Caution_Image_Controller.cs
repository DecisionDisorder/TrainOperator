using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Caution_Image_Controller : MonoBehaviour
{
    public condition_Sanitory_Controller condition_Sanitory_Controller;
    public Company_Peace_Controller peace_Controller;
    public GameObject caution_image;
	public GameObject warning_image;

    private void Start()
    {
        StartCoroutine(ItdUpdate());
    }

    IEnumerator ItdUpdate()
    {
        yield return new WaitForEndOfFrame();
        if (peace_Controller.peaceCondition == 2 || condition_Sanitory_Controller.Condition == 2)
        {
            warning_image.SetActive(false);
            caution_image.SetActive(true);
        }
        else if (peace_Controller.peaceCondition == 1 || condition_Sanitory_Controller.Condition == 1)
        {
            caution_image.SetActive(false);
            warning_image.SetActive(true);
        }
        else if (peace_Controller.peaceCondition > 2 && condition_Sanitory_Controller.Condition > 2)
        {
            warning_image.SetActive(false);
            caution_image.SetActive(false);
        }
        StartCoroutine(ItdUpdate());
    }
}
