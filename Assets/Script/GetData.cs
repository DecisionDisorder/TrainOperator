using UnityEngine;
using System.Collections;

public class GetData : MonoBehaviour {

    public DataConverter dataConverter;
    public DataManager dataManager;
    public BalanceReviser balanceReviser;

    public LineManager lineManager;

	void Awake()
	{
        EncryptedPlayerPrefs.keys = new string[5];
        EncryptedPlayerPrefs.keys[0] = "d45a4dww";
        EncryptedPlayerPrefs.keys[1] = "SW5213s4";
        EncryptedPlayerPrefs.keys[2] = "#WEw52da";
        EncryptedPlayerPrefs.keys[3] = "as2DFs5w";
        EncryptedPlayerPrefs.keys[4] = "Wq28t3Sf";

        LoadAllStationNames();

        dataManager.LoadAll();

        dataConverter.Convert();
        balanceReviser.Revise();
	}

    private void LoadAllStationNames()
    {
        for (int i = 0; i < lineManager.lineCollections.Length; i++)
        {
            if(lineManager.lineCollections[i].purchaseStation.stationFileName != "")
                lineManager.lineCollections[i].purchaseStation.LoadStationNames();
        }
    }
}
