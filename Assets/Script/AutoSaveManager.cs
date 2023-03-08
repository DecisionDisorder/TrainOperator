using UnityEngine;
using System.Collections;

/// <summary>
/// 자동 저장 관리 클래스
/// </summary>
public class AutoSaveManager : MonoBehaviour {

    public LineDataManager lineDataManager;
    private void Start()
    {
        StartCoroutine(SaveTimer());
    }

    /// <summary>
    /// 10초마다 저장을 수행한다
    /// </summary>
    IEnumerator SaveTimer()
    {
        yield return new WaitForSeconds(10);

        DataManager.instance.SaveAll();

        StartCoroutine(SaveTimer());
    }
}
