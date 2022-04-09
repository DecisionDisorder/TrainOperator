using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Playtime : MonoBehaviour {

	public Text Playtime_text;

	public int PlayTime_second { get { return PlayManager.instance.playData.playTimeSec; } set { PlayManager.instance.playData.playTimeSec = value; } }
	public int PlayTime_minute { get { return PlayManager.instance.playData.playTimeMin; } set { PlayManager.instance.playData.playTimeMin = value; } }
	public int PlayTime_hour { get { return PlayManager.instance.playData.playTimeHour; } set { PlayManager.instance.playData.playTimeHour = value; } }

	void Start () {
		//LoadTime ();
        StartCoroutine(Timer());
    }
	
    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1);

        PlayTime_second++;
        if (PlayTime_second >= 60)
        {
            PlayTime_minute++;
            PlayTime_second = 0;
        }
        if (PlayTime_minute >= 60)
        {
            PlayTime_hour++;
            PlayTime_minute = 0;
        }

		if(PlayTime_hour > 0)
			Playtime_text.text = "플레이 시간: " + PlayTime_hour + "시간 " + PlayTime_minute + "분 " + PlayTime_second + "초";
		else
			Playtime_text.text = "플레이 시간: " + PlayTime_minute + "분 " + PlayTime_second + "초";

		StartCoroutine(Timer());
    }
	/*
	public static void SaveTime()
	{
		PlayerPrefs.SetInt ("Playtime_second",PlayTime_second);
		PlayerPrefs.SetInt ("Playtime_minute",PlayTime_minute);
		PlayerPrefs.SetInt ("Playtime_hour",PlayTime_hour);
	}
	public static void LoadTime()
	{
		PlayTime_second = PlayerPrefs.GetInt ("Playtime_second");
		PlayTime_minute = PlayerPrefs.GetInt ("Playtime_minute");
		PlayTime_hour = PlayerPrefs.GetInt ("Playtime_hour");
	}
	public static void ResetTime()
	{
		PlayTime_hour = 0;
		PlayTime_minute = 0;
		PlayTime_second = 0;
	}*/
}
