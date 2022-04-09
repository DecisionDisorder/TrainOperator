using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class EventManager : MonoBehaviour {

	public GameObject Event_Menu;
    public MessageManager messageManager;
    //public OpeningCardPack openingCardPack;
    public ItemManager itemManager;

    public GameObject reportMenu;

    public EventData eventData;

    private bool IsRecommended { get { return eventData.isRecommended; } set { eventData.isRecommended = value; } } // int to bool
    private bool IsSurveyed { get { return eventData.isSurveyed; } set { eventData.isSurveyed = value; } }
    private bool IsUpdateSurveyed { get { return eventData.isUpdateSurveyed; } set { eventData.isUpdateSurveyed = value; } }

    void Start () {
        if(!IsUpdateSurveyed && LineManager.instance.lineCollections[0].lineData.numOfTrain > 0)
        {
            StartCoroutine(WaitOpenEventMenu(600f));
        }
	}

    IEnumerator WaitOpenEventMenu(float delay)
    {
        yield return new WaitForSeconds(delay);

        Event_Menu.SetActive(true);
        messageManager.ShowMessage("업데이트가 어땠는지 설문조사 해주시고, 보상으로 카드 포인트를 받아보세요!", 2.0f);
    }

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
            case 3:
                Application.OpenURL("https://forms.gle/MvFncvwM3D9TwsEy8");
                if (!IsSurveyed)
                {
                    itemManager.CardPoint += 600;
                    messageManager.ShowMessage("카드 포인트 600P가 지급되었습니다.\n소중한 의견 정말 감사드립니다!", 2.0f);
                    IsSurveyed = true;
                    //SaveEvent();
                }
                else
                    messageManager.ShowMessage("소중한 의견 정말 감사드립니다!", 2.0f);
                break;
            case 4:
                Application.OpenURL("https://forms.gle/5GJ6vpSb2eH6KGrp9");
                if(!IsUpdateSurveyed)
                {
                    itemManager.CardPoint += 600;
                    messageManager.ShowMessage("카드 포인트 600P가 지급되었습니다.\n소중한 의견 정말 감사드립니다!", 2.0f);
                    IsUpdateSurveyed = true;
                }
                else
                    messageManager.ShowMessage("소중한 의견 정말 감사드립니다!", 2.0f);
                break;
        }
	}

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
    /*
	public static void SaveEvent()
	{
		PlayerPrefs.SetInt ("isRecommended",isrecommended);
        EncryptedPlayerPrefs.SetString("isSurveyed", isSurveyed.ToString());
	}
	public static void LoadEvent ()
	{
		isrecommended = PlayerPrefs.GetInt ("isRecommended");
        isSurveyed = bool.Parse(EncryptedPlayerPrefs.GetString("isSurveyed", "false"));
    }
	public static void ResetEvent()
	{
		isrecommended = 0;
        isSurveyed = false;
	}*/
}
