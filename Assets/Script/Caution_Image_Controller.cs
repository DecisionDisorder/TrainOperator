using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Caution_Image_Controller : MonoBehaviour
{
    public condition_Sanitory_Controller condition_Sanitory_Controller;
    public PeaceManager peaceManager;
    public GameObject caution_image;
	public GameObject warning_image;

    private void Start()
    {
        StartCoroutine(ItdUpdate());
    }

    IEnumerator ItdUpdate()
    {
        yield return new WaitForEndOfFrame();
        if (peaceManager.peaceStage == 1 || condition_Sanitory_Controller.Condition == 2)
        {
            warning_image.SetActive(false);
            caution_image.SetActive(true);
        }
        else if (peaceManager.peaceStage == 0 || condition_Sanitory_Controller.Condition == 1)
        {
            caution_image.SetActive(false);
            warning_image.SetActive(true);
        }
        else if (peaceManager.peaceStage > 1 && condition_Sanitory_Controller.Condition > 2)
        {
            warning_image.SetActive(false);
            caution_image.SetActive(false);
        }
        StartCoroutine(ItdUpdate());
    }
}
