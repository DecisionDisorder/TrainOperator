using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 미니게임 관리 클래스
/// </summary>
public class MiniGameManager : MonoBehaviour
{
    public SettingManager settingManager;
    public PeddlerMiniGameManager peddlerMiniGameManager;
    public PeaceManager peaceMiniGameManager;
    public TempMiniGameManager tempMiniGameManager;
    public TutorialManager tutorialManager;

    /// <summary>
    /// 미니게임 종류 별 등장 확률
    /// </summary>
    public int[] possibilities;
    /// <summary>
    /// 다음 미니게임  시작 대기 시간
    /// </summary>
    public int timeLeft;

    /// <summary>
    /// 미니게임 대기 시간 최소치
    /// </summary>
    public int timerMin;
    /// <summary>
    /// 미니게임 대기 시간 최대치
    /// </summary>
    public int timerMax;

    private void Start()
    {
        // 미니게임 대기 시간 초기화
        timeLeft = Random.Range(timerMin, timerMax);
        StartCoroutine(GameStartTimer());
    }

    /// <summary>
    /// 미니게임 대기 타이머 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator GameStartTimer()
    {
        yield return new WaitForSeconds(1);

        if(!tutorialManager.tutorialgroups[1].activeInHierarchy)
            timeLeft--;

        if (timeLeft <= 0)
        {
            InitMiniGame();
            timeLeft = Random.Range(timerMin, timerMax);
        }
        StartCoroutine(GameStartTimer());
    }
    
    /// <summary>
    /// 미니게임 시작 전 초기화
    /// </summary>
    public void InitMiniGame()
    {
        if (settingManager.MiniGameActive)
        {
            int gameP = Random.Range(0, 100);
            if (gameP < possibilities[0])
            {
                peddlerMiniGameManager.InitPeddlerMiniGame();
            }
            else if (gameP < possibilities[1])
            {
                peaceMiniGameManager.InitPeaceMiniGame();
            }
            else
            {
                tempMiniGameManager.InitTempControlGame();
            }
        }
    }
}
