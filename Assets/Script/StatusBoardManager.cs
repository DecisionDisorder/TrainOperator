using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBoardManager : MonoBehaviour
{
    public Animation statusBoardAni;
    public Button statusBoardButton;
    public Text statusBoardButtonText;

    public Animation stageBoardFadeAni;
    public GameObject[] statusBoards;
    public Button[] switchBoardButtons;

    private int waitingBoard;
    private int currentBoard;

    private delegate void AnimationCallback();

    public void SetStatusBoard()
    {
        statusBoardButton.enabled = false;
        if (statusBoardAni.gameObject.activeInHierarchy)
        {
            statusBoardButtonText.text = "현황판 열기";
            statusBoardAni["StateBoard"].time = statusBoardAni["StateBoard"].clip.length;
            statusBoardAni["StateBoard"].speed = -1;
            statusBoardAni.Play();
            StartCoroutine(WaitStateboardAnimation(statusBoardAni, CloseStatusBoard));
        }
        else
        {
            statusBoardButtonText.text = "현황판 닫기";
            statusBoardAni.gameObject.SetActive(true);
            statusBoardAni["StateBoard"].time = 0;
            statusBoardAni["StateBoard"].speed = 1;
            statusBoardAni.Play();
            StartCoroutine(WaitStateboardAnimation(statusBoardAni, AfterStatusBoardAni));
        }
    }
    public void SetSwitchBoard(int board)
    {
        if (board != currentBoard)
        {
            stageBoardFadeAni["StateBoardFade"].time = 0;
            stageBoardFadeAni["StateBoardFade"].speed = 1;
            stageBoardFadeAni.Play();
            waitingBoard = board;
            for (int i = 0; i < switchBoardButtons.Length; i++)
                switchBoardButtons[i].enabled = false;
            StartCoroutine(WaitStateboardAnimation(stageBoardFadeAni, SwichBoard));
        }
    }

    private void SwichBoard()
    {
        statusBoards[currentBoard].SetActive(false);
        statusBoards[waitingBoard].SetActive(true);
        currentBoard = waitingBoard;

        stageBoardFadeAni["StateBoardFade"].time = stageBoardFadeAni["StateBoardFade"].length;
        stageBoardFadeAni["StateBoardFade"].speed = -1;
        stageBoardFadeAni.Play();

        for (int i = 0; i < switchBoardButtons.Length; i++)
            switchBoardButtons[i].enabled = true;
    }

    IEnumerator WaitStateboardAnimation(Animation ani, AnimationCallback afterAni)
    {
        while (ani.isPlaying)
            yield return new WaitForEndOfFrame();

        afterAni();
    }

    private void CloseStatusBoard()
    {
        statusBoardAni.gameObject.SetActive(false);
        AfterStatusBoardAni();
    }
    private void AfterStatusBoardAni()
    {
        statusBoardButton.enabled = true;
    }
}
