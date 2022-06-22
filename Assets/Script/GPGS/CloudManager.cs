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
    //�������� Ȯ��

    public static bool CheckLogin()
    {
        return Social.localUser.authenticated;
    }

    //--------------------------------------------------------------------

    //���� ������ ������ ���� �մϴ�.

    public static void SaveToCloud(byte[] data)
    {
        if (!CheckLogin()) //�α��ε��� �ʾ�����
        {
            //�α��η�ƾ�� �����ϴ��� �մϴ�.

            CloudSubManager.instance.AddCloudState("[Error] ���� �÷��� ���� �α��ο� �����߽��ϴ�.", true);
            CloudSubManager.instance.EndCloudProcess();
            return;
        }
        gameData = data;
        //�����̸��� ������ ����Ͻ� �����̸��� �������ݴϴ�.
        CloudSubManager.instance.AddCloudState("���� ������ ������ ���۵Ǿ����ϴ�. ������ ������� �ʰ� �������ּ���.", true);
        OpenSavedGame("TrainOperator_GameSave", true);

    }


    static void OpenSavedGame(string filename, bool bSave)
    {

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (bSave)
            savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedToSave); //�����ƾ����

        else
            savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedToRead); //�ε���ƾ ����

    }

    //savedGameClient.OpenWithAutomaticConflictResolutionȣ��� �Ʒ� �Լ��� �ݹ����� �����߽��ϴ�. �غ�Ȱ�� �ڵ����� ȣ��ɰ̴ϴ�.

    static void OnSavedGameOpenedToSave(SavedGameRequestStatus status, ISavedGameMetadata game)
    {

        if (status == SavedGameRequestStatus.Success)
        {
            // handle reading or writing of saved game.

            //������ �غ�Ǿ����ϴ�. ���� ���� ������ �����մϴ�.

            CloudSubManager.instance.AddCloudState("���� Ŭ���忡 ������ �ϰ��ֽ��ϴ�. �ð��� �� �ɸ� �� ������ ���ݸ� ��ٷ��ּ���.");

            //�����ҵ����͹���Ʈ�迭�� �����Ͻ� �������� ����Ʈ �迭�� �����մϴ�.
            SaveGame(game, gameData, DateTime.Now.TimeOfDay);//����Ʈ�迭�� ����������ȭ�� ����

        }
        else
        {
            CloudSubManager.instance.AddCloudState("[Error] ���� ������ ���⿡ �����߽��ϴ�.");
            CloudSubManager.instance.EndCloudProcess();
            //���Ͽ��⿡ ���� �߽��ϴ�. �����޽����� ����ϵ��� �մϴ�.

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
            CloudSubManager.instance.AddCloudState("���� ������ ������ �Ϸ��Ͽ����ϴ�.");
        }
        else
        {
            CloudSubManager.instance.AddCloudState("���� ������ ���忡 �����߽��ϴ�.");
            //������ ���忡 ���� �߽��ϴ�.
        }
        CloudSubManager.instance.EndCloudProcess();
    }


    //----------------------------------------------------------------------------------------------------------------

    //Ŭ����� ���� �����б�

    public static void LoadFromCloud()
    {
        if (!CheckLogin())
        {
            //�α��ε��� �ʾ����� �α��� ��ƾ�� �����ϴ��� �մϴ�.
            CloudSubManager.instance.AddCloudState("[Error] ���� �÷��� ���� �α��ο� �����߽��ϴ�.", true);
            CloudSubManager.instance.EndCloudProcess();
            return;
        }
        CloudSubManager.instance.AddCloudState("���� �����͸� �ҷ����� ���۵Ǿ����ϴ�. ������ ������� �ʰ� �������ּ���.", true);
        //���� ����� �����̸��� �������ݴϴ�. �׳� ��ǻ�ͻ��� ���ϰ� �Ȱ��� �����Ͻø�˴ϴ�.

        OpenSavedGame("TrainOperator_GameSave", false);
    }



    static void OnSavedGameOpenedToRead(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // handle reading or writing of saved game.
            CloudSubManager.instance.AddCloudState("���� ������ ������ �ҷ��Խ��ϴ�.");
            LoadGameData(game);
        }
        else
        {
            CloudSubManager.instance.AddCloudState("���� ������ ������ �ҷ����µ��� �����߽��ϴ�.");
            CloudSubManager.instance.EndCloudProcess();
            //���Ͽ��⿡ ���� �Ѱ��, �����޽����� ����ϴ��� �մϴ�.
        }
    }


    //������ �б⸦ �õ��մϴ�.

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
            //������ �б⿡ �����߽��ϴ�.
            CloudSubManager.instance.AddCloudState("���� �����͸� ���������� �ҷ��Խ��ϴ�.");
            CloudSubManager.instance.LoadCloudCallback(data);
            CloudSubManager.instance.EndCloudProcess(true);
            //data �迭�� �����ؼ� �����ϰ� ����Ͻø�˴ϴ�.
        }

        else
        {
            //�б⿡ ���� �߽��ϴ�. �����޽����� ����ϴ��� �մϴ�.
            CloudSubManager.instance.AddCloudState("���� �����͸� �ҷ����µ��� �����߽��ϴ�.");
            CloudSubManager.instance.EndCloudProcess();
        }
    }
}