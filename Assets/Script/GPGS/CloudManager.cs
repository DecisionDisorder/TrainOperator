using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;

public static class CloudManager
{
    private static byte[] gameData;

    public static void Init()
    {
        //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        
        
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);

        //PlayGamesPlatform.DebugLogEnabled = false;
        //Activate the Google Play gaems platform
        //PlayGamesPlatform.Activate();

    }
    //인증여부 확인

    public static bool CheckLogin()
    {
        return Social.localUser.authenticated;
    }

    //--------------------------------------------------------------------

    //게임 저장은 다음과 같이 합니다.

    public static void SaveToCloud(byte[] data)
    {
        if (!CheckLogin()) //로그인되지 않았으면
        {
            //로그인루틴을 진행하던지 합니다.

            CloudSubManager.instance.AddCloudState("[Error] 구글 플레이 서비스 로그인에 실패했습니다.", true);
            CloudSubManager.instance.EndCloudProcess();
            return;
        }
        gameData = data;
        //파일이름에 적당히 사용하실 파일이름을 지정해줍니다.
        CloudSubManager.instance.AddCloudState("게임 데이터 저장이 시작되었습니다. 게임이 종료되지 않게 주의해주세요.", true);
        OpenSavedGame("TrainOperator_GameSave", true);

    }


    static void OpenSavedGame(string filename, bool bSave)
    {

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (bSave)
            savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedToSave); //저장루틴진행

        else
            savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedToRead); //로딩루틴 진행

    }

    //savedGameClient.OpenWithAutomaticConflictResolution호출시 아래 함수를 콜백으로 지정했습니다. 준비된경우 자동으로 호출될겁니다.

    static void OnSavedGameOpenedToSave(SavedGameRequestStatus status, ISavedGameMetadata game)
    {

        if (status == SavedGameRequestStatus.Success)
        {
            // handle reading or writing of saved game.

            //파일이 준비되었습니다. 실제 게임 저장을 수행합니다.

            CloudSubManager.instance.AddCloudState("현재 클라우드에 저장을 하고있습니다. 시간이 좀 걸릴 수 있으니 조금만 기다려주세요.");

            //저장할데이터바이트배열에 저장하실 데이터의 바이트 배열을 지정합니다.
            SaveGame(game, gameData, DateTime.Now.TimeOfDay);//바이트배열을 데이터직렬화후 넣음

        }
        else
        {
            CloudSubManager.instance.AddCloudState("[Error] 게임 데이터 열기에 실패했습니다.");
            CloudSubManager.instance.EndCloudProcess();
            //파일열기에 실패 했습니다. 오류메시지를 출력하든지 합니다.

        }
    }

    static void SaveGame(ISavedGameMetadata game, byte[] savedData, TimeSpan totalPlaytime)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
        builder = builder.WithUpdatedPlayedTime(totalPlaytime).WithUpdatedDescription("Saved game at " + DateTime.Now);
        /*
        if (savedImage != null)
        {
            // This assumes that savedImage is an instance of Texture2D
            // and that you have already called a function equivalent to
            // getScreenshot() to set savedImage
            // NOTE: see sample definition of getScreenshot() method below
            byte[] pngData = savedImage.EncodeToPNG();
            builder = builder.WithUpdatedPngCoverImage(pngData);
        }*/

        SavedGameMetadataUpdate updatedMetadata = builder.Build();

        savedGameClient.CommitUpdate(game, updatedMetadata, savedData, OnSavedGameWritten);
    }

    static void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            CloudSubManager.instance.AddCloudState("게임 데이터 저장을 완료하였습니다.");
        }
        else
        {
            CloudSubManager.instance.AddCloudState("게임 데이터 저장에 실패했습니다.");
            //데이터 저장에 실패 했습니다.
        }
        CloudSubManager.instance.EndCloudProcess();
    }


    //----------------------------------------------------------------------------------------------------------------

    //클라우드로 부터 파일읽기

    public static void LoadFromCloud()
    {
        if (!CheckLogin())
        {
            //로그인되지 않았으니 로그인 루틴을 진행하던지 합니다.
            CloudSubManager.instance.AddCloudState("[Error] 구글 플레이 서비스 로그인에 실패했습니다.", true);
            CloudSubManager.instance.EndCloudProcess();
            return;
        }
        CloudSubManager.instance.AddCloudState("게임 데이터를 불러오기 시작되었습니다. 게임이 종료되지 않게 주의해주세요.", true);
        //내가 사용할 파일이름을 지정해줍니다. 그냥 컴퓨터상의 파일과 똑같다 생각하시면됩니다.

        OpenSavedGame("TrainOperator_GameSave", false);
    }



    static void OnSavedGameOpenedToRead(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // handle reading or writing of saved game.
            CloudSubManager.instance.AddCloudState("게임 데이터 파일을 불러왔습니다.");
            LoadGameData(game);
        }
        else
        {
            CloudSubManager.instance.AddCloudState("게임 데이터 파일을 불러오는데에 실패했습니다.");
            CloudSubManager.instance.EndCloudProcess();
            //파일열기에 실패 한경우, 오류메시지를 출력하던지 합니다.
        }
    }


    //데이터 읽기를 시도합니다.

    static void LoadGameData(ISavedGameMetadata game)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
    }

    static void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // handle processing the byte array data
            //데이터 읽기에 성공했습니다.
            CloudSubManager.instance.AddCloudState("게임 데이터를 성공적으로 불러왔습니다.");
            CloudSubManager.instance.LoadCloudCallback(data);
            CloudSubManager.instance.EndCloudProcess(true);
            //data 배열을 복구해서 적절하게 사용하시면됩니다.
        }

        else
        {
            //읽기에 실패 했습니다. 오류메시지를 출력하던지 합니다.
            CloudSubManager.instance.AddCloudState("게임 데이터를 불러오는데에 실패했습니다.");
            CloudSubManager.instance.EndCloudProcess();
        }
    }
}