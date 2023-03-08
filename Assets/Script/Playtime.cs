using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 플레이 시간 계산 관리 클래스
/// </summary>
public class Playtime : MonoBehaviour {

    /// <summary>
    /// 플레이 시간 정보 텍스트
    /// </summary>
	public Text Playtime_text;

    /// <summary>
    /// 초 단위의 플레이 타임
    /// </summary>
	public int PlayTime_second { get { return PlayManager.instance.playData.playTimeSec; } set { PlayManager.instance.playData.playTimeSec = value; } }
    /// <summary>
    /// 분 단위의 플레이 타임
    /// </summary>
	public int PlayTime_minute { get { return PlayManager.instance.playData.playTimeMin; } set { PlayManager.instance.playData.playTimeMin = value; } }
    /// <summary>
    /// 시간 단위의 플레이 타임
    /// </summary>
	public int PlayTime_hour { get { return PlayManager.instance.playData.playTimeHour; } set { PlayManager.instance.playData.playTimeHour = value; } }

	void Start () {
        StartCoroutine(Timer());
    }
	
    /// <summary>
    /// 매 초마다 플레이 타임을 증가시키며 계산
    /// </summary>
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
}
