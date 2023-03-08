using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;

/// <summary>
/// 클라우드 백업 시스템 관리 클래스
/// </summary>
public static class CloudManager
{
    /// <summary>
    /// 직렬화된 게임 데이터
    /// </summary>
    private static byte[] gameData;

    /// <summary>
    /// 구글 플레이 게임 서비스 초기화
    /// </summary>
    public static void Init()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);
    }
    /// <summary>
    /// 인증여부 확인
    /// </summary>
    public static bool CheckLogin()
    {
        return Social.localUser.authenticated;
    }

    /// <summary>
    /// 클라우드 저장 시작 
    /// </summary>
    /// <param name="data">저장할 serialized 데이터</param>
    public static void SaveToCloud(byte[] data)
    {
        // 로그인이 안돼있으면 클라우드 저장 프로세스를 종료한다.
        if (!CheckLogin()) 
        {
            CloudSubManager.instance.AddCloudState("[Error] 구글 플레이 서비스 로그인에 실패했습니다.", true);
            CloudSubManager.instance.EndCloudProcess();
            return;
        }

        // 저장될 파일의 이름을 지정하면서 클라우드 저장을 시작한다.
        gameData = data;
        CloudSubManager.instance.AddCloudState("게임 데이터 저장이 시작되었습니다. 게임이 종료되지 않게 주의해주세요.", true);
        OpenSavedGame("TrainOperator_GameSave", true);

    }

    /// <summary>
    /// 저장된 게임 세이브 파일을 불러온다.
    /// </summary>
    /// <param name="filename">파일 이름</param>
    /// <param name="bSave">저장(true)/불러오기(false) 선택</param>
    static void OpenSavedGame(string filename, bool bSave)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (bSave)
            savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedToSave); //저장루틴진행
        else
            savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedToRead); //로딩루틴 진행
    }

    /// <summary>
    /// 저장하기 위해 저장된 게임 세이브 파일을 불러온다.
    /// (Callback from savedGameClient.OpenWithAutomaticConflictResolution) 
    /// </summary>
    static void OnSavedGameOpenedToSave(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        // 파일이 준비되어, 실제 게임 저장을 수행한다.
        if (status == SavedGameRequestStatus.Success)
        {
            CloudSubManager.instance.AddCloudState("현재 클라우드에 저장을 하고있습니다. 시간이 좀 걸릴 수 있으니 조금만 기다려주세요.");
            // 저장할 데이터 바이트 배열에 저장할 데이터의 바이트 배열을 지정한다.
            SaveGame(game, gameData, DateTime.Now.TimeOfDay); // 바이트 배열을 데이터 직렬화 후 넣음
        }
        else
        {
            // 파일 열기에 실패하여 로그를 출력하고 저장 과정을 종료한다.
            CloudSubManager.instance.AddCloudState("[Error] 게임 데이터 열기에 실패했습니다.");
            CloudSubManager.instance.EndCloudProcess();
        }
    }

    /// <summary>
    /// 구글 클라우드에 게임 세이브 파일을 저장한다.
    /// </summary>
    static void SaveGame(ISavedGameMetadata game, byte[] savedData, TimeSpan totalPlaytime)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
        builder = builder.WithUpdatedPlayedTime(totalPlaytime).WithUpdatedDescription("Saved game at " + DateTime.Now);

        SavedGameMetadataUpdate updatedMetadata = builder.Build();

        savedGameClient.CommitUpdate(game, updatedMetadata, savedData, OnSavedGameWritten);
    }

    /// <summary>
    /// 게임 저장 완료 여부 콜백 함수
    /// </summary>
    static void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // 데이터 저장 성공
            CloudSubManager.instance.AddCloudState("게임 데이터 저장을 완료하였습니다.");
        }
        else
        {
            // 데이터 저장 실패
            CloudSubManager.instance.AddCloudState("게임 데이터 저장에 실패했습니다.");
        }
        CloudSubManager.instance.EndCloudProcess();
    }

    /// <summary>
    /// 클라우드로 부터 파일 읽기 시작
    /// </summary>
    public static void LoadFromCloud()
    {
        // 로그인이 안돼있으면 클라우드 불러오기 프로세스를 종료한다.
        if (!CheckLogin())
        {
            CloudSubManager.instance.AddCloudState("[Error] 구글 플레이 서비스 로그인에 실패했습니다.", true);
            CloudSubManager.instance.EndCloudProcess();
            return;
        }

        // 불러올 파일의 이름을 지정하면서 클라우드 로드를 시작한다.
        CloudSubManager.instance.AddCloudState("게임 데이터를 불러오기 시작되었습니다. 게임이 종료되지 않게 주의해주세요.", true);
        OpenSavedGame("TrainOperator_GameSave", false);
    }

    /// <summary>
    /// 불러오기 위해 저장된 게임 세이브 파일을 불러온다.
    /// (Callback from savedGameClient.OpenWithAutomaticConflictResolution) 
    /// </summary>
    static void OnSavedGameOpenedToRead(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        // 파일이 준비되어, 실제 게임 저장을 수행한다.
        if (status == SavedGameRequestStatus.Success)
        {
            CloudSubManager.instance.AddCloudState("게임 데이터 파일을 불러왔습니다.");
            LoadGameData(game);
        }
        else
        {
            // 파일 열기에 실패하여 로그를 출력하고 저장 과정을 종료한다.
            CloudSubManager.instance.AddCloudState("게임 데이터 파일을 불러오는데에 실패했습니다.");
            CloudSubManager.instance.EndCloudProcess();
        }
    }

    /// <summary>
    /// 클라우드에서 데이터 읽기를 시도한다.
    /// </summary>
    static void LoadGameData(ISavedGameMetadata game)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
    }

    /// <summary>
    /// 게임 로드 완료 여부 콜백 함수
    /// </summary>
    static void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // 데이터 로드 성공
            CloudSubManager.instance.AddCloudState("게임 데이터를 성공적으로 불러왔습니다.");
            CloudSubManager.instance.LoadCloudCallback(data);
            CloudSubManager.instance.EndCloudProcess(true);
        }

        else
        {
            // 데이터 로드 실패
            CloudSubManager.instance.AddCloudState("게임 데이터를 불러오는데에 실패했습니다.");
            CloudSubManager.instance.EndCloudProcess();
        }
    }
}