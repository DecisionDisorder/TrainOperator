using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Ŭ���� ���� ���� Ŭ����
/// </summary>
public class CloudSubManager : MonoBehaviour
{
    /// <summary>
    /// Ŭ���� ���� �Ŵ��� �̱��� �ν��Ͻ�
    /// </summary>
    public static CloudSubManager instance;

    /// <summary>
    /// �뼱 ������ ������ ������Ʈ
    /// </summary>
    public LineDataManager lineDataManager;

    /// <summary>
    /// Ŭ���� ���� �α� ���¸� ǥ���ϴ� �ؽ�Ʈ
    /// </summary>
    public Text cloudStateText;

    /// <summary>
    /// Ŭ���� ���� ��ư ������Ʈ
    /// </summary>
    public GameObject saveButton;
    /// <summary>
    /// Ŭ���� �ҷ����� ��ư ������Ʈ
    /// </summary>
    public GameObject loadButton;
    /// <summary>
    /// �޴� �ݱ� ��ư ������Ʈ
    /// </summary>
    public GameObject closeButton;

    /// <summary>
    /// Ŭ���� �޴� ������Ʈ
    /// </summary>
    public GameObject cloudMenu;
    /// <summary>
    /// Ŭ���� �۾� Ȯ�� �޴� ������Ʈ
    /// </summary>
    public GameObject checkMenu;
    /// <summary>
    /// ȭ�� �ٽ� �ҷ����� Ȯ�� ������Ʈ
    /// </summary>
    public GameObject reloadCheckMenu;
    /// <summary>
    /// ������ �۾��� �����ִ� �ؽ�Ʈ
    /// </summary>
    public Text taskText;
    /// <summary>
    /// ����ڰ� ������ �۾� ����
    /// </summary>
    private int taskType = -1;

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Ŭ���� ���� �޴� Ȱ��ȭ/��Ȱ��ȭ
    /// </summary>
    /// <param name="active">Ȱ��ȭ ����</param>
    public void SetCloudMenu(bool active)
    {
        cloudMenu.SetActive(active);
    }

    /// <summary>
    /// Ŭ���� ���� �α� �߰�
    /// </summary>
    /// <param name="msg">�߰��� �޽���</param>
    /// <param name="reset">������ �α� �ؽ�Ʈ �ʱ�ȭ ����</param>
    public void AddCloudState(string msg, bool reset = false)
    {
        if (reset)
            cloudStateText.text = msg + "\n";
        else
            cloudStateText.text += msg + "\n";
    }

    /// <summary>
    /// Ŭ���� �۾� ����
    /// </summary>
    /// <param name="type">������ �۾�(0: ����, 1: �ҷ�����)</param>
    public void SelectCloudProcess(int type)
    {
        taskType = type;
        if(type.Equals(0))
            taskText.text = "�۾�: <color=lime>�����ϱ�</color>";
        else if(type.Equals(1))
            taskText.text = "�۾�: <color=orange>�ҷ�����</color>";
        checkMenu.SetActive(true);
    }

    /// <summary>
    /// �۾� ���� ���� Ȯ�� ���� ó��
    /// </summary>
    /// <param name="confirm"></param>
    public void CheckConfirm(bool confirm)
    {
        if(confirm)
            StartCloudProcess();

        checkMenu.SetActive(false);
    }

    /// <summary>
    /// Ŭ���� �۾� ����
    /// </summary>
    public void StartCloudProcess()
    {
        // �߰��� �ٸ� �۾��� �������� ���ϵ��� ��ư ��Ȱ��ȭ
        saveButton.SetActive(false);
        loadButton.SetActive(false);
        closeButton.SetActive(false);
        
        // �۾��� �����̸�, ��ü �����͸� ����ȭ�Ͽ� ���� �۾��� ��û�Ѵ�.
        if(taskType.Equals(0))
        {
            EntireData entireData = new EntireData(DataManager.instance.AssembleGeneralData(), lineDataManager.AssembleLineData());
            byte[] data = SerializeData.StringToByte(SerializeData.SetSerialization(entireData));
            CloudManager.SaveToCloud(data);
        }
        // �۾��� �ҷ������, �ҷ����⸦ �����Ѵ�.
        else if (taskType.Equals(1))
        {
            CloudManager.LoadFromCloud();
        }
    }

    /// <summary>
    /// Ŭ����κ��� �ҷ��� �����͸� ������ȭ �Ͽ� ���� �����Ϳ� �����Ѵ�.
    /// </summary>
    /// <param name="bytes"></param>
    public void LoadCloudCallback(byte[] bytes)
    {
        string serialized = SerializeData.ByteToString(bytes);
        EntireData entireData = (EntireData)SerializeData.GetDeserialization(serialized);
        entireData.generalData.playData.isConverted = true;
        DataManager.instance.SetGeneralData(entireData.generalData);
        lineDataManager.SetLineData(entireData.lineDatas);
    }

    /// <summary>
    /// Ŭ���� �۾��� �����Ѵ�.
    /// </summary>
    /// <param name="isLoadSuccess">�۾� ���� ����</param>
    public void EndCloudProcess(bool isLoadSuccess = false)
    {
        // ���� �������̽� ����
        saveButton.SetActive(true);
        loadButton.SetActive(true);
        closeButton.SetActive(true);

        // ������ ���� �� ���� �ʱ�ȭ
        DataManager.instance.SaveAll();
        if (taskType.Equals(0))
            taskType = -1;
        // �ҷ����⿡ �����ߴٸ�, ȭ�� ���ΰ�ħ�� ���� �ҷ����� ���� �޴��� ����.
        else if (taskType.Equals(1) && isLoadSuccess)
            OpenReloadCheck();
    }

    /// <summary>
    /// ���ΰ�ħ Ȯ�� �޴� Ȱ��ȭ
    /// </summary>
    public void OpenReloadCheck()
    {
        reloadCheckMenu.SetActive(true);
        taskType = -1;
    }

    /// <summary>
    /// ���� ���� ��, ȭ�� ���ΰ�ħ ����
    /// </summary>
    public void ApplyReload()
    {
        DataManager.instance.SaveAll();

        SceneManager.LoadScene(0);
    }
}

/// <summary>
/// �Ϲ� �����Ϳ� �뼱 ���� �����͸� Ŭ���� ������ ���� ���� Ŭ����
/// </summary>
[System.Serializable]
public class EntireData
{
    public GeneralData generalData;
    public LineData[] lineDatas;

    public EntireData(GeneralData generalData, LineData[] lineDatas)
    {
        this.generalData = generalData;
        this.lineDatas = lineDatas;
    }
}