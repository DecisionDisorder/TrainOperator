using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CloudSubManager : MonoBehaviour
{
    public static CloudSubManager instance;

    public LineDataManager lineDataManager;

    public Text cloudStateText;

    public GameObject saveButton;
    public GameObject loadButton;
    public GameObject closeButton;

    public GameObject cloudMenu;
    public GameObject checkMenu;
    public GameObject reloadCheckMenu;
    public Text taskText;
    private int taskType = -1;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public void SetCloudMenu(bool active)
    {
        cloudMenu.SetActive(active);
    }

    public void AddCloudState(string msg, bool reset = false)
    {
        if (reset)
            cloudStateText.text = msg + "\n";
        else
            cloudStateText.text += msg + "\n";
    }

    public void SelectCloudProcess(int type)
    {
        taskType = type;
        if(type.Equals(0))
            taskText.text = "작업: <color=lime>저장하기</color>";
        else if(type.Equals(1))
            taskText.text = "작업: <color=orange>불러오기</color>";
        checkMenu.SetActive(true);
    }

    public void CheckConfirm(bool confirm)
    {
        if(confirm)
            StartCloudProcess();

        checkMenu.SetActive(false);
    }

    public void StartCloudProcess()
    {
        saveButton.SetActive(false);
        loadButton.SetActive(false);
        closeButton.SetActive(false);
        if(taskType.Equals(0))
        {
            EntireData entireData = new EntireData(DataManager.instance.AssembleGeneralData(), lineDataManager.AssembleLineData());
            byte[] data = SerializeData.StringToByte(SerializeData.SetSerialization(entireData));
            CloudManager.SaveToCloud(data);
        }
        else if (taskType.Equals(1))
        {
            CloudManager.LoadFromCloud();
        }
    }

    public void LoadCloudCallback(byte[] bytes)
    {
        string serialized = SerializeData.ByteToString(bytes);
        EntireData entireData = (EntireData)SerializeData.GetDeSerialization(serialized);
        entireData.generalData.playData.isConverted = true;
        DataManager.instance.SetGeneralData(entireData.generalData);
        lineDataManager.SetLineData(entireData.lineDatas);
    }

    public void EndCloudProcess(bool isLoadSuccess = false)
    {
        saveButton.SetActive(true);
        loadButton.SetActive(true);
        closeButton.SetActive(true);
        DataManager.instance.SaveAll();
        if (taskType.Equals(0))
            taskType = -1;
        else if (taskType.Equals(1) && isLoadSuccess)
            OpenReloadCheck();
    }

    public void OpenReloadCheck()
    {
        reloadCheckMenu.SetActive(true);
        taskType = -1;
    }

    public void ApplyReload()
    {
        DataManager.instance.SaveAll();

        SceneManager.LoadScene(0);
    }
}


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