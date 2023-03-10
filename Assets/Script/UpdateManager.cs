using UnityEngine;
using System.Collections;

/// <summary>
/// 패치노트 알림 시스템 관리 클래스
/// </summary>
public class UpdateManager : MonoBehaviour {

    /// <summary>
    /// 블로그로 이동할지를 묻는 메뉴 오브젝트
    /// </summary>
	public GameObject connectToblog_Menu;

    /// <summary>
    /// 최근에 확인한 패치노트의 버전 코드
    /// </summary>
	public int RecentViewedVersionCode { get { return PlayManager.instance.playData.patchNoteVersionCode; } set { PlayManager.instance.playData.patchNoteVersionCode = value; } }
    /// <summary>
    /// 최신 패치노트의 버전 코드
    /// </summary>
	public int versionCode;
    /// <summary>
    /// 패치노트 URL
    /// </summary>
	public string URL;

	public SettingManager settings;

    void Start () {
		// 최신 패치노트가 업데이트 되고, 알림을 받는 설정이면 메뉴 활성화
		if (RecentViewedVersionCode != versionCode && settings.UpdateAlarm) {
			connectToblog_Menu.SetActive (true);
			RecentViewedVersionCode = versionCode;
		}
	}

    /// <summary>
    /// 블로그 진입 여부 관련 버튼 클릭 이벤트 리스너
    /// </summary>
	public void PressKey(int nKey){
        switch (nKey)
        {
            case 0://yes
				DataManager.instance.SaveAll();
                Application.OpenURL(URL);
                connectToblog_Menu.SetActive(false);
                break;
            case 1://no
                connectToblog_Menu.SetActive(false);
                break;
            case 2://other button
                connectToblog_Menu.SetActive(true);
                break;
        }
	}
}
