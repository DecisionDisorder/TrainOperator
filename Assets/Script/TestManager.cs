using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

/// <summary>
/// 테스트 관리 클래스
/// </summary>
public class TestManager : MonoBehaviour {

    /// <summary>
    /// 비밀번호 확인 메뉴 오브젝트
    /// </summary>
    public GameObject pw_Menu;
    /// <summary>
    /// 데이터 변환기
    /// </summary>
    public DataConverter dataConverter;
    public LineManager lineManager;
    public BankManager bankManager;
    public BankSpecialManager bankSpecialManager;
    public PeaceManager peaceManager;
    public condition_Sanitory_Controller condition_Sanitory_Controller;
    public MiniGameManager miniGameManager;

    public TouchEarning touchEarning;
    /// <summary>
    /// 매크로 테스트 메뉴 오브젝트
    /// </summary>
    public GameObject macroTestMenu;
    /// <summary>
    /// 터치형 수익 조정 입력 필드
    /// </summary>
    public InputField touchAmountInput;

    /// <summary>
    /// 테스트 모드 활성화 여부
    /// </summary>
    public bool TestMode;
    /// <summary>
    /// 테스트 메뉴 진입 버튼 오브젝트
    /// </summary>
    public GameObject testButton;

    public string folderName = "ScreenShots";
    public string fileName = "ScreenShot";
    public string extName = "png";

    private void Start()
    {
        // 테스트 모드 여부에 따라 테스트 메뉴 진입 버튼 활성화
        if (TestMode) 
        {
            testButton.SetActive(true);
            StartCoroutine(TestInput());
        }
        else
            testButton.SetActive(false);
    }

    /// <summary>
    /// 테스트를 위해 키보드 입력으로 특정 미니게임 바로 실행
    /// </summary>
    /// <returns></returns>
    IEnumerator TestInput()
    {
        yield return new WaitForEndOfFrame();

        if (Input.GetKey(KeyCode.F9))
            miniGameManager.InitMiniGame();
        if (Input.GetKey(KeyCode.F10))
            miniGameManager.peaceMiniGameManager.InitPeaceMiniGame();
        else if (Input.GetKey(KeyCode.F11))
            miniGameManager.peddlerMiniGameManager.InitPeddlerMiniGame();
        else if (Input.GetKey(KeyCode.F12))
            miniGameManager.tempMiniGameManager.InitTempControlGame();
        if (Input.GetKey(KeyCode.F8))
            Screenshot();
        StartCoroutine(TestInput());
    }
    private string RootPath
    {
        get
        {
            return Application.dataPath;
        }
    }
    private string FolderPath => $"{RootPath}/{folderName}";
    private string TotalPath => $"{FolderPath}/{fileName}_{DateTime.Now.ToString("MMdd_HHmmss")}.{extName}";


    private void Screenshot()
    {
        string totalPath = TotalPath;
        Texture2D screenTex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        Rect area = new Rect(0f, 0f, Screen.width, Screen.height);
        screenTex.ReadPixels(area, 0, 0);
        try
        {
            if (Directory.Exists(FolderPath) == false)
            {
                Directory.CreateDirectory(FolderPath);
            }

            File.WriteAllBytes(totalPath, screenTex.EncodeToPNG());
            Debug.Log($"Screen Shot Saved : {totalPath}");
        }
        catch(Exception ex)
        {
            Debug.LogWarning($"Screen Shot Save Failed : {totalPath}");
            Debug.LogWarning(ex);
        }
        

    }

    /// <summary>
    /// 특정 테스트 버튼 클릭 리스너
    /// </summary>
    public void Presskey(int nKey)
    {
        switch (nKey)
        {
            case 0:
                MyAsset.instance.myAssetData.passengersLow += 1000000000000000;
                break;
            case 1:
                for (int i = 0; i < lineManager.lineCollections.Length; i++)
                {
                    lineManager.lineCollections[i].lineData.numOfTrain = 100;
                    if(lineManager.lineCollections[i].lineData.trainExpandStatus.Length > 0)
                        lineManager.lineCollections[i].lineData.trainExpandStatus[0] = 100;
                    lineManager.lineCollections[i].lineData.limitTrain = 100;
                    lineManager.lineCollections[i].lineData.numOfBase = 10;
                }
                break;
            case 2:
                bankManager.ContractTime[0] = 0;
                bankManager.ContractTime[1] = 0;
                bankManager.ContractTime[2] = 0;
                bankSpecialManager.ContractTime[0] = 0;
                bankSpecialManager.ContractTime[1] = 0;
                bankSpecialManager.ContractTime[2] = 0;
                break;
            case 3:
                break;
            case 4:
                TouchMoneyManager.AddPassengerLimit(100000000000000000, 0);
                break;
            case 5:
                for (int i = 0; i < 3; i++)
                {
                    bankManager.ContractTime[i] = 0;
                    bankSpecialManager.ContractTime[i] = 0;
                }
                break;
            case 6:
                MyAsset.instance.MoneyHigh += 100000000;
                break;
        }
    }
    public void GetMoney_1(string money)
    {
        AssetMoneyCalculator.instance.ArithmeticOperation(ulong.Parse(money), 0, true);
    }
    public void GetMoney_2(string money)
    {
        AssetMoneyCalculator.instance.ArithmeticOperation(0, ulong.Parse(money), true);
    }
    public void SetPassengerAddLow(string ps)
    {
        TouchMoneyManager.ArithmeticOperation(ulong.Parse(ps), 0,true);
        CompanyReputationManager.instance.RenewPassengerBase();
    }
    public void SetPassengerAddHigh(string ps)
    {
        TouchMoneyManager.ArithmeticOperation(0, ulong.Parse(ps), true);
        CompanyReputationManager.instance.RenewPassengerBase();
    }
    public void SetPassengerLimitLow(string ps)
    {
        TouchMoneyManager.AddPassengerLimit(ulong.Parse(ps), 0);
        CompanyReputationManager.instance.RenewPassengerBase();
    }
    public void SetPassengerLimitHigh(string ps)
    {
        TouchMoneyManager.AddPassengerLimit(0, ulong.Parse(ps));
        CompanyReputationManager.instance.RenewPassengerBase();
    }
    public void GetPeace(string peace)
    {
        peaceManager.PeaceValue = int.Parse(peace);
    }
    public void GetSanitory(string peace)
    {
        condition_Sanitory_Controller.SanitoryValue = int.Parse(peace);
    }

    public void SetPassengerLow(string passenger)
    {
        MyAsset.instance.myAssetData.passengersLow = 0;
        TouchMoneyManager.ArithmeticOperation(ulong.Parse(passenger), 0, true);
        CompanyReputationManager.instance.RenewPassengerBase();
    }
    public void SetPassengerHigh(string passenger)
    {
        MyAsset.instance.myAssetData.passengersHigh = 0;
        TouchMoneyManager.ArithmeticOperation(0, ulong.Parse(passenger), true);
        CompanyReputationManager.instance.RenewPassengerBase();
    }

    public void ResetDataInfo()
    {
        File.Delete(Application.persistentDataPath + "/dataInfo.dat");
        SceneManager.LoadScene(0);
    }

    public void SetAllExpandTrue()
    {
        for(int i = 0; i < lineManager.lineCollections.Length; i++)
        {
            for(int j = 0; j < lineManager.lineCollections[i].lineData.sectionExpanded.Length; j++)
            {
                lineManager.lineCollections[i].lineData.sectionExpanded[j] = true;
            }
        }
    }

    public void GetPW(string ps)
    {
        if(ps.Equals("asd123"))
        {
            pw_Menu.SetActive(false);
        }
    }
    public void SetAllClearAllLines()
    {
        SetAllClearNewLines();
    }

    /// <summary>
    /// 구버전 신규 노선 클리어용
    /// </summary>
    public void SetAllClearNewLines()
    {
        LineData[] lineDatas = new LineData[4];
        lineDatas[0] = lineManager.lineCollections[(int)Line.BusanDH].lineData;
        lineDatas[1] = lineManager.lineCollections[(int)Line.GyeonguiJungang].lineData;
        lineDatas[2] = lineManager.lineCollections[(int)Line.Gyeongchun].lineData;
        lineDatas[3] = lineManager.lineCollections[(int)Line.Gyeonggang].lineData;
        lineDatas[0].hasStation = new bool[15];
        for (int i = 0; i < 4; i++)
        {
            lineDatas[i].numOfTrain = 100 + i;
            lineDatas[i].limitTrain = 110;
            lineDatas[i].numOfBase = 6;
            lineDatas[i].numOfBaseEx = 10;
            for (int j = 0; j < 4; j++)
                lineDatas[i].trainExpandStatus[j] = lineDatas[i].numOfTrain - j;

            for (int j = 0; j < lineDatas[i].installed.Length; j++)
            {
                lineDatas[i].connected[j] = true;
                lineDatas[i].installed[j] = true;
                lineDatas[i].hasAllStations[j] = true;
                lineDatas[i].sectionExpanded[j] = true;
            }

            for (int j = 0; j < lineDatas[i].hasStation.Length; j++)
                lineDatas[i].hasStation[j] = true;
        }
        SaveDataForTest(lineDatas);
    }
    public void TestMeditation()
    {
        //MediationTestSuite.Show();
    }
    public void SaveDataForTest(LineData[] lineDatas)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/dataInfo.dat");
        formatter.Serialize(file, lineDatas);
        file.Close();
    }

    public void MacroTestActive(bool active)
    {
        macroTestMenu.SetActive(active);
    }


    IEnumerator autoTouchMacro = null;

    public void StartTouch()
    {
        StopTouch();
        StartCoroutine(autoTouchMacro = AutoTouchMacro(1 / float.Parse(touchAmountInput.text)));
    }
    public void StopTouch()
    {
        if (autoTouchMacro != null)
            StopCoroutine(autoTouchMacro);
    }

    IEnumerator AutoTouchMacro(float delay)
    {
        yield return new WaitForSeconds(delay);

        touchEarning.TouchIncome();

        StartCoroutine(autoTouchMacro = AutoTouchMacro(1 / float.Parse(touchAmountInput.text)));
    }
}
