using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;

/// <summary>
/// Ŭ���� ��� �ý��� ���� Ŭ����
/// </summary>
public static class CloudManager
{
    /// <summary>
    /// ����ȭ�� ���� ������
    /// </summary>
    private static byte[] gameData;

    /// <summary>
    /// ���� �÷��� ���� ���� �ʱ�ȭ
    /// </summary>
    public static void Init()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);
    }
    /// <summary>
    /// �������� Ȯ��
    /// </summary>
    public static bool CheckLogin()
    {
        return Social.localUser.authenticated;
    }

    /// <summary>
    /// Ŭ���� ���� ���� 
    /// </summary>
    /// <param name="data">������ serialized ������</param>
    public static void SaveToCloud(byte[] data)
    {
        // �α����� �ȵ������� Ŭ���� ���� ���μ����� �����Ѵ�.
        if (!CheckLogin()) 
        {
            CloudSubManager.instance.AddCloudState("[Error] ���� �÷��� ���� �α��ο� �����߽��ϴ�.", true);
            CloudSubManager.instance.EndCloudProcess();
            return;
        }

        // ����� ������ �̸��� �����ϸ鼭 Ŭ���� ������ �����Ѵ�.
        gameData = data;
        CloudSubManager.instance.AddCloudState("���� ������ ������ ���۵Ǿ����ϴ�. ������ ������� �ʰ� �������ּ���.", true);
        OpenSavedGame("TrainOperator_GameSave", true);

    }

    /// <summary>
    /// ����� ���� ���̺� ������ �ҷ��´�.
    /// </summary>
    /// <param name="filename">���� �̸�</param>
    /// <param name="bSave">����(true)/�ҷ�����(false) ����</param>
    static void OpenSavedGame(string filename, bool bSave)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (bSave)
            savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedToSave); //�����ƾ����
        else
            savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedToRead); //�ε���ƾ ����
    }

    /// <summary>
    /// �����ϱ� ���� ����� ���� ���̺� ������ �ҷ��´�.
    /// (Callback from savedGameClient.OpenWithAutomaticConflictResolution) 
    /// </summary>
    static void OnSavedGameOpenedToSave(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        // ������ �غ�Ǿ�, ���� ���� ������ �����Ѵ�.
        if (status == SavedGameRequestStatus.Success)
        {
            CloudSubManager.instance.AddCloudState("���� Ŭ���忡 ������ �ϰ��ֽ��ϴ�. �ð��� �� �ɸ� �� ������ ���ݸ� ��ٷ��ּ���.");
            // ������ ������ ����Ʈ �迭�� ������ �������� ����Ʈ �迭�� �����Ѵ�.
            SaveGame(game, gameData, DateTime.Now.TimeOfDay); // ����Ʈ �迭�� ������ ����ȭ �� ����
        }
        else
        {
            // ���� ���⿡ �����Ͽ� �α׸� ����ϰ� ���� ������ �����Ѵ�.
            CloudSubManager.instance.AddCloudState("[Error] ���� ������ ���⿡ �����߽��ϴ�.");
            CloudSubManager.instance.EndCloudProcess();
        }
    }

    /// <summary>
    /// ���� Ŭ���忡 ���� ���̺� ������ �����Ѵ�.
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
    /// ���� ���� �Ϸ� ���� �ݹ� �Լ�
    /// </summary>
    static void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // ������ ���� ����
            CloudSubManager.instance.AddCloudState("���� ������ ������ �Ϸ��Ͽ����ϴ�.");
        }
        else
        {
            // ������ ���� ����
            CloudSubManager.instance.AddCloudState("���� ������ ���忡 �����߽��ϴ�.");
        }
        CloudSubManager.instance.EndCloudProcess();
    }

    /// <summary>
    /// Ŭ����� ���� ���� �б� ����
    /// </summary>
    public static void LoadFromCloud()
    {
        // �α����� �ȵ������� Ŭ���� �ҷ����� ���μ����� �����Ѵ�.
        if (!CheckLogin())
        {
            CloudSubManager.instance.AddCloudState("[Error] ���� �÷��� ���� �α��ο� �����߽��ϴ�.", true);
            CloudSubManager.instance.EndCloudProcess();
            return;
        }

        // �ҷ��� ������ �̸��� �����ϸ鼭 Ŭ���� �ε带 �����Ѵ�.
        CloudSubManager.instance.AddCloudState("���� �����͸� �ҷ����� ���۵Ǿ����ϴ�. ������ ������� �ʰ� �������ּ���.", true);
        OpenSavedGame("TrainOperator_GameSave", false);
    }

    /// <summary>
    /// �ҷ����� ���� ����� ���� ���̺� ������ �ҷ��´�.
    /// (Callback from savedGameClient.OpenWithAutomaticConflictResolution) 
    /// </summary>
    static void OnSavedGameOpenedToRead(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        // ������ �غ�Ǿ�, ���� ���� ������ �����Ѵ�.
        if (status == SavedGameRequestStatus.Success)
        {
            CloudSubManager.instance.AddCloudState("���� ������ ������ �ҷ��Խ��ϴ�.");
            LoadGameData(game);
        }
        else
        {
            // ���� ���⿡ �����Ͽ� �α׸� ����ϰ� ���� ������ �����Ѵ�.
            CloudSubManager.instance.AddCloudState("���� ������ ������ �ҷ����µ��� �����߽��ϴ�.");
            CloudSubManager.instance.EndCloudProcess();
        }
    }

    /// <summary>
    /// Ŭ���忡�� ������ �б⸦ �õ��Ѵ�.
    /// </summary>
    static void LoadGameData(ISavedGameMetadata game)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
    }

    /// <summary>
    /// ���� �ε� �Ϸ� ���� �ݹ� �Լ�
    /// </summary>
    static void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // ������ �ε� ����
            CloudSubManager.instance.AddCloudState("���� �����͸� ���������� �ҷ��Խ��ϴ�.");
            CloudSubManager.instance.LoadCloudCallback(data);
            CloudSubManager.instance.EndCloudProcess(true);
        }

        else
        {
            // ������ �ε� ����
            CloudSubManager.instance.AddCloudState("���� �����͸� �ҷ����µ��� �����߽��ϴ�.");
            CloudSubManager.instance.EndCloudProcess();
        }
    }
}