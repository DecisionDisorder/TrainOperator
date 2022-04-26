using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    public SettingManager settingManager;
    public PeddlerMiniGameManager peddlerMiniGameManager;
    public PeaceManager peaceMiniGameManager;
    public TempMiniGameManager tempMiniGameManager;

    public int[] possibilities;
    public int timeLeft;

    public int timerMin;
    public int timerMax;

    private void Start()
    {
        timeLeft = Random.Range(timerMin, timerMax);
        StartCoroutine(GameStartTimer());
    }

    IEnumerator GameStartTimer()
    {
        yield return new WaitForSeconds(1);

        timeLeft--;

        if (timeLeft <= 0)
        {
            InitMiniGame();
            timeLeft = Random.Range(timerMin, timerMax);
        }
        StartCoroutine(GameStartTimer());
    }
    
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
