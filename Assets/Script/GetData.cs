using UnityEngine;
using System.Collections;

/// <summary>
/// 데이터 로드 클래스
/// </summary>
public class GetData : MonoBehaviour {

    public DataConverter dataConverter;
    public DataManager dataManager;
    public BalanceReviser balanceReviser;
    public LineManager lineManager;

	void Awake()
	{
        // 암호화 Playerprefs 키 초기화 (미사용)
        EncryptedPlayerPrefs.keys = new string[5];
        EncryptedPlayerPrefs.keys[0] = "d45a4dww";
        EncryptedPlayerPrefs.keys[1] = "SW5213s4";
        EncryptedPlayerPrefs.keys[2] = "#WEw52da";
        EncryptedPlayerPrefs.keys[3] = "as2DFs5w";
        EncryptedPlayerPrefs.keys[4] = "Wq28t3Sf";

        LoadAllStationNames();

        // 전체 게임 데이터 불러오기
        dataManager.LoadAll();

        // 데이터 변환이 필요 시, 변환 시작
        dataConverter.Convert();
        balanceReviser.Revise();
	}

    /// <summary>
    /// 각 노선의 역 이름 데이터 불러오기
    /// </summary>
    private void LoadAllStationNames()
    {
        for (int i = 0; i < lineManager.lineCollections.Length; i++)
        {
            if(lineManager.lineCollections[i].purchaseStation.stationFileName != "")
                lineManager.lineCollections[i].purchaseStation.LoadStationNames();
        }
    }
}
