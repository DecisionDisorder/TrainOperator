using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ��Ȳ�� �ý��� ���� Ŭ����
/// </summary>
public class StatusBoardManager : MonoBehaviour
{
    /// <summary>
    /// ��Ȳ�� Ȱ��ȭ �ִϸ��̼� ȿ��
    /// </summary>
    public Animation statusBoardAni;
    /// <summary>
    /// ��Ȳ�� Ȱ��ȭ ��ư
    /// </summary>
    public Button statusBoardButton;
    /// <summary>
    /// ��Ȳ�� Ȱ��ȭ ��ư �ؽ�Ʈ
    /// </summary>
    public Text statusBoardButtonText;

    /// <summary>
    /// ��Ȳ�� ��ȯ ���̵� �ִϸ��̼� ȿ��
    /// </summary>
    public Animation stageBoardFadeAni;
    /// <summary>
    /// ��Ȳ�� �׷� �� ������Ʈ �迭
    /// </summary>
    public GameObject[] statusBoards;
    /// <summary>
    /// ��Ȳ�� �׷� ��ȯ ��ư �迭
    /// </summary>
    public Button[] switchBoardButtons;

    /// <summary>
    /// ��ȯ ���� ���� �ε���
    /// </summary>
    private int waitingBoard;
    /// <summary>
    /// ���� ���� �ε���
    /// </summary>
    private int currentBoard;

    private delegate void AnimationCallback();

    /// <summary>
    /// ��Ȳ�� Ȱ��ȭ/��Ȱ��ȭ ó��
    /// </summary>
    public void SetStatusBoard()
    {
        statusBoardButton.enabled = false;
        if (statusBoardAni.gameObject.activeInHierarchy)
        {
            statusBoardButtonText.text = "��Ȳ�� ����";
            statusBoardAni["StateBoard"].time = statusBoardAni["StateBoard"].clip.length;
            statusBoardAni["StateBoard"].speed = -1;
            statusBoardAni.Play();
            StartCoroutine(WaitStateboardAnimation(statusBoardAni, CloseStatusBoard));
        }
        else
        {
            statusBoardButtonText.text = "��Ȳ�� �ݱ�";
            statusBoardAni.gameObject.SetActive(true);
            statusBoardAni["StateBoard"].time = 0;
            statusBoardAni["StateBoard"].speed = 1;
            statusBoardAni.Play();
            StartCoroutine(WaitStateboardAnimation(statusBoardAni, AfterStatusBoardAni));
        }
    }
    /// <summary>
    /// ������ ����� ��Ȳ�� ��ȯ ȿ�� ����
    /// </summary>
    /// <param name="board">���� �ε���</param>
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

    /// <summary>
    /// ������ ����� ��Ȳ�� ��ȯ ����
    /// </summary>
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

    /// <summary>
    /// ��Ȳ�� ��ȯ �ִϸ��̼� ��� �ڷ�ƾ
    /// </summary>
    /// <param name="ani">������� �ִϸ��̼�</param>
    /// <param name="afterAni">�ִϸ��̼� ���� �� ȣ���� �Լ�</param>
    IEnumerator WaitStateboardAnimation(Animation ani, AnimationCallback afterAni)
    {
        while (ani.isPlaying)
            yield return new WaitForEndOfFrame();

        afterAni();
    }

    /// <summary>
    /// ��Ȳ�� ��Ȱ��ȭ
    /// </summary>
    private void CloseStatusBoard()
    {
        statusBoardAni.gameObject.SetActive(false);
        AfterStatusBoardAni();
    }
    /// <summary>
    /// ��Ȳ�� Ȱ��ȭ/��Ȱ��ȭ �ִϸ��̼� ���� �� �ݹ� �Լ�
    /// </summary>
    private void AfterStatusBoardAni()
    {
        statusBoardButton.enabled = true;
    }
}
