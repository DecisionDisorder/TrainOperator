using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 현황판 시스템 관리 클래스
/// </summary>
public class StatusBoardManager : MonoBehaviour
{
    /// <summary>
    /// 현황판 활성화 애니메이션 효과
    /// </summary>
    public Animation statusBoardAni;
    /// <summary>
    /// 현황판 활성화 버튼
    /// </summary>
    public Button statusBoardButton;
    /// <summary>
    /// 현황판 활성화 버튼 텍스트
    /// </summary>
    public Text statusBoardButtonText;

    /// <summary>
    /// 현황판 전환 페이드 애니메이션 효과
    /// </summary>
    public Animation stageBoardFadeAni;
    /// <summary>
    /// 현황판 그룹 별 오브젝트 배열
    /// </summary>
    public GameObject[] statusBoards;
    /// <summary>
    /// 현황판 그룹 전환 버튼 배열
    /// </summary>
    public Button[] switchBoardButtons;

    /// <summary>
    /// 전환 예정 보드 인덱스
    /// </summary>
    private int waitingBoard;
    /// <summary>
    /// 현재 보드 인덱스
    /// </summary>
    private int currentBoard;

    private delegate void AnimationCallback();

    /// <summary>
    /// 현황판 활성화/비활성화 처리
    /// </summary>
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
    /// <summary>
    /// 선택한 보드로 현황판 전환 효과 시작
    /// </summary>
    /// <param name="board">보드 인덱스</param>
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
    /// 선택한 보드로 현황판 전환 적용
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
    /// 현황판 전환 애니메이션 대기 코루틴
    /// </summary>
    /// <param name="ani">재생중인 애니메이션</param>
    /// <param name="afterAni">애니메이션 종료 후 호출할 함수</param>
    IEnumerator WaitStateboardAnimation(Animation ani, AnimationCallback afterAni)
    {
        while (ani.isPlaying)
            yield return new WaitForEndOfFrame();

        afterAni();
    }

    /// <summary>
    /// 현황판 비활성화
    /// </summary>
    private void CloseStatusBoard()
    {
        statusBoardAni.gameObject.SetActive(false);
        AfterStatusBoardAni();
    }
    /// <summary>
    /// 현황판 활성화/비활성화 애니메이션 종료 후 콜백 함수
    /// </summary>
    private void AfterStatusBoardAni()
    {
        statusBoardButton.enabled = true;
    }
}
