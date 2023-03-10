using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 클라우드 보조 관리 클래스
/// </summary>
public class CloudSubManager : MonoBehaviour
{
    /// <summary>
    /// 클라우드 보조 매니저 싱글톤 인스턴스
    /// </summary>
    public static CloudSubManager instance;

    /// <summary>
    /// 노선 데이터 관리자 오브젝트
    /// </summary>
    public LineDataManager lineDataManager;

    /// <summary>
    /// 클라우드 저장 로그 상태를 표시하는 텍스트
    /// </summary>
    public Text cloudStateText;

    /// <summary>
    /// 클라우드 저장 버튼 오브젝트
    /// </summary>
    public GameObject saveButton;
    /// <summary>
    /// 클라우드 불러오기 버튼 오브젝트
    /// </summary>
    public GameObject loadButton;
    /// <summary>
    /// 메뉴 닫기 버튼 오브젝트
    /// </summary>
    public GameObject closeButton;

    /// <summary>
    /// 클라우드 메뉴 오브젝트
    /// </summary>
    public GameObject cloudMenu;
    /// <summary>
    /// 클라우드 작업 확인 메뉴 오브젝트
    /// </summary>
    public GameObject checkMenu;
    /// <summary>
    /// 화면 다시 불러오기 확인 오브젝트
    /// </summary>
    public GameObject reloadCheckMenu;
    /// <summary>
    /// 선택한 작업을 보여주는 텍스트
    /// </summary>
    public Text taskText;
    /// <summary>
    /// 사용자가 선택한 작업 종류
    /// </summary>
    private int taskType = -1;

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 클라우드 저장 메뉴 활성화/비활성화
    /// </summary>
    /// <param name="active">활성화 여부</param>
    public void SetCloudMenu(bool active)
    {
        cloudMenu.SetActive(active);
    }

    /// <summary>
    /// 클라우드 상태 로그 추가
    /// </summary>
    /// <param name="msg">추가할 메시지</param>
    /// <param name="reset">누적된 로그 텍스트 초기화 여부</param>
    public void AddCloudState(string msg, bool reset = false)
    {
        if (reset)
            cloudStateText.text = msg + "\n";
        else
            cloudStateText.text += msg + "\n";
    }

    /// <summary>
    /// 클라우드 작업 선택
    /// </summary>
    /// <param name="type">선택할 작업(0: 저장, 1: 불러오기)</param>
    public void SelectCloudProcess(int type)
    {
        taskType = type;
        if(type.Equals(0))
            taskText.text = "작업: <color=lime>저장하기</color>";
        else if(type.Equals(1))
            taskText.text = "작업: <color=orange>불러오기</color>";
        checkMenu.SetActive(true);
    }

    /// <summary>
    /// 작업 진행 최종 확인 여부 처리
    /// </summary>
    /// <param name="confirm"></param>
    public void CheckConfirm(bool confirm)
    {
        if(confirm)
            StartCloudProcess();

        checkMenu.SetActive(false);
    }

    /// <summary>
    /// 클라우드 작업 시작
    /// </summary>
    public void StartCloudProcess()
    {
        // 중간에 다른 작업을 진행하지 못하도록 버튼 비활성화
        saveButton.SetActive(false);
        loadButton.SetActive(false);
        closeButton.SetActive(false);
        
        // 작업이 저장이면, 전체 데이터를 직렬화하여 저장 작업을 요청한다.
        if(taskType.Equals(0))
        {
            EntireData entireData = new EntireData(DataManager.instance.AssembleGeneralData(), lineDataManager.AssembleLineData());
            byte[] data = SerializeData.StringToByte(SerializeData.SetSerialization(entireData));
            CloudManager.SaveToCloud(data);
        }
        // 작업이 불러오기면, 불러오기를 시작한다.
        else if (taskType.Equals(1))
        {
            CloudManager.LoadFromCloud();
        }
    }

    /// <summary>
    /// 클라우드로부터 불러온 데이터를 역직렬화 하여 게임 데이터에 적용한다.
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
    /// 클라우드 작업을 종료한다.
    /// </summary>
    /// <param name="isLoadSuccess">작업 성공 여부</param>
    public void EndCloudProcess(bool isLoadSuccess = false)
    {
        // 유저 인터페이스 복구
        saveButton.SetActive(true);
        loadButton.SetActive(true);
        closeButton.SetActive(true);

        // 데이터 저장 및 상태 초기화
        DataManager.instance.SaveAll();
        if (taskType.Equals(0))
            taskType = -1;
        // 불러오기에 성공했다면, 화면 새로고침을 위한 불러오기 성공 메뉴를 띄운다.
        else if (taskType.Equals(1) && isLoadSuccess)
            OpenReloadCheck();
    }

    /// <summary>
    /// 새로고침 확인 메뉴 활성화
    /// </summary>
    public void OpenReloadCheck()
    {
        reloadCheckMenu.SetActive(true);
        taskType = -1;
    }

    /// <summary>
    /// 게임 저장 후, 화면 새로고침 시작
    /// </summary>
    public void ApplyReload()
    {
        DataManager.instance.SaveAll();

        SceneManager.LoadScene(0);
    }
}

/// <summary>
/// 일반 데이터와 노선 관련 데이터를 클라우드 저장을 위해 묶은 클래스
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