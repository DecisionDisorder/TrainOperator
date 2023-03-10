using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// 데이터 저장 및 불러오기 시스템 관리 클래스
/// </summary>
public class DataManager : MonoBehaviour{

	/// <summary>
	/// 데이터 관리 클래스의 싱글톤 인스턴스
	/// </summary>
	public static DataManager instance;

	public MyAsset myAsset;
	public CompanyReputationManager company_Reputation_Controller;
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

	/// <summary>
	/// 여러 오브젝트에 퍼져있는 노선 외 데이터를 하나의 데이터 클래스로 모은다.
	/// </summary>
	/// <returns>하나로 모아진 노선 외 데이터 오브젝트</returns>
	public GeneralData AssembleGeneralData()
    {
		return new GeneralData(myAsset.myAssetData, company_Reputation_Controller.companyData, rent.rentData, itemManager.itemData, playManager.playData, event_Manager.eventData,
			bankManager.bankData, compensation_Manager.compensationData, drivers_Manager.driverData, setting.settingData, levelManager.levelData, lotteryTicketManager.lotteryData, 
			achievementManager.achievementData);
	}

	/// <summary>
	/// 불러온 노선 외 데이터를 각 오브젝트에 적용한다.
	/// </summary>
	/// <param name="generalData">불러온 노선 외 데이터</param>
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
		if (generalData.lotteryData != null)
			lotteryTicketManager.lotteryData = generalData.lotteryData;
		else
			lotteryTicketManager.lotteryData = new LotteryData();
		if (generalData.achievementData != null)
			achievementManager.achievementData = generalData.achievementData;
		else
			achievementManager.achievementData = new AchievementData();

		if (generalData.levelData != null)
			levelManager.levelData = generalData.levelData;
		else
			levelManager.levelData = new LevelData();
	}

	/// <summary>
	/// 데이터 저장 함수
	/// </summary>
	public void SaveData()
	{
		// 최근 접속 시간을 기기의 현재 시간으로 설정 후 데이터 합치기
		playManager.playData.recentConnectedTime = System.DateTime.Now;
		GeneralData generalData = AssembleGeneralData();

		// 직려로하하여 파일에 저장
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/generalData.dat");
		formatter.Serialize(file, generalData);
		file.Close();
	}

	/// <summary>
	/// 노선 및 노선 외 전체 데이터 저장
	/// </summary>
	public void SaveAll()
    {
		SaveData();
		lineDataManager.SaveData();
    }

	/// <summary>
	/// 노선 및 노선 외 전체 데이터 불러오기
	/// </summary>
	public void LoadAll()
    {
		LoadData();
		lineDataManager.LoadData();
    }
	
	/// <summary>
	/// 파일로부터 노선 외 데이터 불러오기
	/// </summary>
	public void LoadData()
	{
		instance = this;

		// 직렬화된 파일 데이터 불러오기
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream file = null;
		GeneralData generalData = null;
		try
		{
			file = File.Open(Application.persistentDataPath + "/generalData.dat", FileMode.Open);
			if (file != null && file.Length > 0)
			{
				// 불러와진 데이터가 있으면 역직렬화 후 파일 닫기
				generalData = (GeneralData)formatter.Deserialize(file);

				file.Close();
			}
			else
			{
				// 불러올 데이터가 없으면 초기화 작업
				Debug.Log("Cannot load data file.");
				generalData = new GeneralData();
			}

		}
		catch
        {
            // 불러올 데이터가 없으면 초기화 작업
            Debug.Log("Data file does not exist.");
			generalData = new GeneralData();
		}

		// 불러오거나 초기화된 데이터 적용하기
		if (generalData != null)
		{
			SetGeneralData(generalData);
		}
	}
}

/// <summary>
/// 노선 외 데이터 모음 데이터 클래스
/// </summary>
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
