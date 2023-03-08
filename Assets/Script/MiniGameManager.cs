using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �̴ϰ��� ���� Ŭ����
/// </summary>
public class MiniGameManager : MonoBehaviour
{
    public SettingManager settingManager;
    public PeddlerMiniGameManager peddlerMiniGameManager;
    public PeaceManager peaceMiniGameManager;
    public TempMiniGameManager tempMiniGameManager;
    public TutorialManager tutorialManager;

    /// <summary>
    /// �̴ϰ��� ���� �� ���� Ȯ��
    /// </summary>
    public int[] possibilities;
    /// <summary>
    /// ���� �̴ϰ���  ���� ��� �ð�
    /// </summary>
    public int timeLeft;

    /// <summary>
    /// �̴ϰ��� ��� �ð� �ּ�ġ
    /// </summary>
    public int timerMin;
    /// <summary>
    /// �̴ϰ��� ��� �ð� �ִ�ġ
    /// </summary>
    public int timerMax;

    private void Start()
    {
        // �̴ϰ��� ��� �ð� �ʱ�ȭ
        timeLeft = Random.Range(timerMin, timerMax);
        StartCoroutine(GameStartTimer());
    }

    /// <summary>
    /// �̴ϰ��� ��� Ÿ�̸� �ڷ�ƾ
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
    /// �̴ϰ��� ���� �� �ʱ�ȭ
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
