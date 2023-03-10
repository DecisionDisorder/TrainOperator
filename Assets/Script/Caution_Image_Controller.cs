using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 주의/경고 이미지 제어 클래스
/// </summary>
public class Caution_Image_Controller : MonoBehaviour
{
    public condition_Sanitory_Controller condition_Sanitory_Controller;
    public PeaceManager peaceManager;
    /// <summary>
    /// 주의 아이콘 이미지 오브젝트
    /// </summary>
    public GameObject caution_image;
    /// <summary>
    /// 경고 아이콘 이미지 오브젝트
    /// </summary>
	public GameObject warning_image;

    private void Start()
    {
        StartCoroutine(ItdUpdate());
    }

    /// <summary>
    /// 치안 단계 혹은 위생도 단계에 따라 주의/경고 이미지 활성화
    /// </summary>
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
