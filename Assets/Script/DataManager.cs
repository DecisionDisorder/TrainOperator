using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DataManager : MonoBehaviour{

	public static DataManager instance;

	public MyAsset myAsset;
	public CompanyReputationManager company_Reputation_Controller;
	//public button_Rent rent;
	public RentManager rent;
	public ItemManager itemManager;
	public PlayManager playManager;
	public EventManager event_Manager;
	public BankManager bankManager;
	public Compensation_Manager compensation_Manager;
	public DriversManager drivers_Manager;
	public SettingManager setting;
	public LineDataManager lineDataManager;
	public LevelManager levelManager;
	public LotteryTicketManager lotteryTicketManager;
	public AchievementManager achievementManager;

	public GeneralData AssembleGeneralData()
    {
		return new GeneralData(myAsset.myAssetData, company_Reputation_Controller.companyData, rent.rentData, itemManager.itemData, playManager.playData, event_Manager.eventData,
			bankManager.bankData, compensation_Manager.compensationData, drivers_Manager.driverData, setting.settingData, levelManager.levelData, lotteryTicketManager.lotteryData, 
			achievementManager.achievementData);
	}

	public void SetGeneralData(GeneralData generalData)
    {
		myAsset.myAssetData = generalData.myAssetData;
		company_Reputation_Controller.companyData = generalData.companyData;
		rent.rentData = generalData.rentData;
		itemManager.itemData = generalData.itemData;
		playManager.playData = generalData.playData;
		event_Manager.eventData = generalData.eventData;
		bankManager.bankData = generalData.bankData;
		compensation_Manager.compensationData = generalData.compensationData;
		drivers_Manager.driverData = generalData.driverData;
		setting.settingData = generalData.settingData;
		lotteryTicketManager.lotteryData = generalData.lotteryData;
		achievementManager.achievementData = generalData.achievementData;

		if (generalData.levelData != null)
			levelManager.levelData = generalData.levelData;
		else
			levelManager.levelData = new LevelData();
	}

	public void SaveData()
	{
		playManager.playData.recentConnectedTime = System.DateTime.Now;
		GeneralData generalData = AssembleGeneralData();

		BinaryFormatter formatter = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/generalData.dat");
		formatter.Serialize(file, generalData);
		file.Close();
	}

	public void SaveAll()
    {
		SaveData();
		lineDataManager.SaveData();
    }

	public void LoadAll()
    {
		LoadData();
		lineDataManager.LoadData();
    }
	

	public void LoadData()
	{
		instance = this;

		BinaryFormatter formatter = new BinaryFormatter();
		FileStream file = null;
		GeneralData generalData = null;
		try
		{
			file = File.Open(Application.persistentDataPath + "/generalData.dat", FileMode.Open);
			if (file != null && file.Length > 0)
			{
				generalData = (GeneralData)formatter.Deserialize(file);

				file.Close();
			}
			else
			{
				Debug.Log("Cannot load data file.");
				generalData = new GeneralData();
			}

		}
		catch
		{
			Debug.Log("Data file does not exist.");
			generalData = new GeneralData();
		}

		if (generalData != null)
		{
			SetGeneralData(generalData);
		}
	}
}

[System.Serializable]
public class GeneralData
{
	public MyAssetData myAssetData;
	public CompanyData companyData;
	public RentData rentData;
	public ItemData itemData;
	public PlayData playData;
	public EventData eventData;
	public BankData bankData;
	public CompensationData compensationData;
	public DriverData driverData;
	public SettingData settingData;
	public LevelData levelData;
	public LotteryData lotteryData;
	public AchievementData achievementData;

	public GeneralData(MyAssetData myAssetData, CompanyData companyData, RentData rentData, ItemData itemData, PlayData playData,
		EventData eventData, BankData bankData, CompensationData compensationData, DriverData driverData, SettingData settingData, 
		LevelData levelData, LotteryData lotteryData, AchievementData achievementData)
    {
		this.myAssetData = myAssetData;
		this.companyData = companyData;
		this.rentData = rentData;
		this.itemData = itemData;
		this.playData = playData;
		this.eventData = eventData;
		this.bankData = bankData;
		this.compensationData = compensationData;
		this.driverData = driverData;
		this.settingData = settingData;
		this.levelData = levelData;
		this.lotteryData = lotteryData;
		this.achievementData = achievementData;
    }

	public GeneralData()
    {
		myAssetData = new MyAssetData();
		companyData = new CompanyData();
		rentData = new RentData();
		itemData = new ItemData();
		playData = new PlayData();
		eventData = new EventData();
		bankData = new BankData();
		compensationData = new CompensationData();
		driverData = new DriverData();
		settingData = new SettingData();
		levelData = new LevelData();
		lotteryData = new LotteryData();
		achievementData = new AchievementData();
	}
}
