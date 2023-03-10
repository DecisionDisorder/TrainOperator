using UnityEngine;
using System.Collections;

/// <summary>
/// 언락되지 않은 노선의 열차 이미지 잠금 시스템 관리 클래스
/// </summary>
public class trainImageLock_Manager : MonoBehaviour {

    /// <summary>
    /// 열차 이미지 잠금 오브젝트
    /// </summary>
    public GameObject[] lockImage_train;

    /// <summary>
    /// 열차 구매 메뉴 오브젝트
    /// </summary>
    public GameObject buyTrain_Menu;

    private void Start()
    {
        StartCoroutine(StateUpdate());
    }

    /// <summary>
    /// 1초마다 확장권을 구매하지 않은 노선에 대해 열차 이미지 잠금 처리
    /// </summary>
    IEnumerator StateUpdate()
    {
        yield return new WaitForSeconds(1.0f);

        LineCollection[] lineCollections = LineManager.instance.lineCollections;

        if(buyTrain_Menu.activeInHierarchy)
        {
            for (int i = 0; i < 8; i++)
                if (lineCollections[i + 1].IsExpanded())
                    lockImage_train[i].SetActive(false);
        }

        StartCoroutine(StateUpdate());
    }
}