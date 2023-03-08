using UnityEngine;
using System.Collections;

public class UpdateManager : MonoBehaviour {

	public GameObject connectToblog_Menu;

	public int RecentViewedVersionCode { get { return PlayManager.instance.playData.patchNoteVersionCode; } set { PlayManager.instance.playData.patchNoteVersionCode = value; } }
	public int versionCode;
	public string URL;
	public SettingManager settings;

    void Start () {
		//Loaddata ();
		if (RecentViewedVersionCode != versionCode && settings.UpdateAlarm) {
			connectToblog_Menu.SetActive (true);
			RecentViewedVersionCode = versionCode;
		}
	}

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
