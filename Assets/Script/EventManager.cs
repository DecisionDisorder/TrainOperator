using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

/// <summary>
/// 이벤트 시스템 관리 클래스
/// </summary>
public class EventManager : MonoBehaviour {

    /// <summary>
    /// 이벤트 메뉴 오브젝트
    /// </summary>
	public GameObject Event_Menu;

    public MessageManager messageManager;
    public ItemManager itemManager;
    public TutorialManager tutorialManager;

    /// <summary>
    /// 오류제보/건의사항 진입 확인 메뉴 오브젝트
    /// </summary>
    public GameObject reportMenu;

    /// <summary>
    /// 이벤트 데이터 오브젝트
    /// </summary>
    public EventData eventData;

    /// <summary>
    /// 페이스북 이벤트 참여 및 보상 수령 여부
    /// </summary>
    private bool IsRecommended { get { return eventData.isRecommended; } set { eventData.isRecommended = value; } } // int to bool
    /// <summary>
    /// 1차 설문조사 참여 및 보상 수령 여부
    /// </summary>
    private bool IsSurveyed { get { return eventData.isSurveyed; } set { eventData.isSurveyed = value; } }
    /// <summary>
    /// 업데이트 후기 설문조사 참여 및 보상 수령 여부
    /// </summary>
    private bool IsUpdateSurveyed { get { return eventData.isUpdateSurveyed; } set { eventData.isUpdateSurveyed = value; } }
    /// <summary>
    /// 상시 설문조사 참여 및 보상 수령 여부
    /// </summary>
    private bool DidNormalSurvey { get { return eventData.didNormalSurvey; } set { eventData.didNormalSurvey = value; } }

    /// <summary>
    /// 설문조사 URL
    /// </summary>
    public string surveyURL = "https://forms.gle/RRYyvUKMVDqDTNb46";

    void Start () {
        // 기존에 게임을 플레이 했던 유저 중에
        // 설문조사에 참여하지 않은 유저를 대상으로 게임 시작 후 30초 후에 설문조사 메뉴 활성화
        if(!DidNormalSurvey && LineManager.instance.lineCollections[0].lineData.numOfTrain > 0)
        {
            StartCoroutine(WaitOpenEventMenu(30f));
        }
	}

    /// <summary>
    /// 업데이터 설문조사 활성화 대기 코루틴
    /// </summary>
    /// <param name="delay">메뉴 활성화 대기 시간</param>
    IEnumerator WaitOpenEventMenu(float delay)
    {
        yield return new WaitForSeconds(delay);

        if(!tutorialManager.tutorialgroups[1].activeInHierarchy)
        {
            Event_Menu.SetActive(true);
            messageManager.ShowMessage("업데이트가 어땠는지 설문조사 해주시고, 보상으로 카드 포인트를 받아보세요!", 2.0f);
        }
    }

    /// <summary>
    /// 이벤트 메뉴 버튼 리스너
    /// </summary>
	public void PressKey(int nKey)
	{
        switch (nKey)
        {
            case 0://go in Event Menu
                Event_Menu.SetActive(true);
                break;
            case 1://exit Event Menu
                Event_Menu.SetActive(false);
                break;
            case 2://Facebook
                if (!IsRecommended)
                {
                    Application.OpenURL("https://www.facebook.com/DecisionDisorderGame/");
                    itemManager.CardPoint += 350;
                    messageManager.ShowMessage("카드 포인트 350P가 지급되었습니다. 페이스북 좋아요 해주셔서 감사합니다!", 2.0f);
                    IsRecommended = true;
                }
                else
                {
                    Application.OpenURL("https://www.facebook.com/DecisionDisorderGame/");
                    messageManager.ShowMessage("이미 1회 지급이 된 상태 이므로 카드팩은 지급되지않았습니다.", 2.0f);
                }
                break;
            case 4:
                Application.OpenURL(surveyURL);
                if(!DidNormalSurvey)
                {
                    itemManager.CardPoint += 600;
                    messageManager.ShowMessage("카드 포인트 600P가 지급되었습니다.\n소중한 의견 정말 감사드립니다!", 2.0f);
                    DidNormalSurvey = true;
                }
                else
                    messageManager.ShowMessage("소중한 의견 정말 감사드립니다!", 2.0f);
                break;
        }
	}

    /// <summary>
    /// 오류제보 메뉴 관련 버튼 클릭 리스너
    /// </summary>
    public void SetReportMenu(int code)
    {
        switch(code)
        {
            case 0:
                reportMenu.SetActive(true);
                break;
            case 1:
                reportMenu.SetActive(false);
                break;
            case 2:
                Application.OpenURL("https://forms.gle/wmWas8d1eb1KucQb6");
                messageManager.ShowMessage("소중한 의견 감사드립니다!", 2.0f);
                break;

        }
    }
}
