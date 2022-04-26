using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataConverter : MonoBehaviour
{
    public LineManager lineManager;
    public LineDataManager lineDataManager;
    public bool IsConverted { get { return playManager.playData.isConverted; } set { playManager.playData.isConverted = value; } }

    private string[] line1StationDataNames = { "Bool_soyosan", "Bool_dongduchun", "Bool_bosan", "Bool_dongduchunjungang", "Bool_jihaeng", "Bool_dukjung", "Bool_dukgye", "Bool_yangju", "Bool_nokyang", "Bool_ganeng"
            , "Bool_uijeongbu", "Bool_hoeryong", "Bool_mangwallsa", "Bool_dobongsan", "Bool_dobong", "Bool_banghak", "Bool_changdong(1)", "Bool_nokchun", "Bool_wallgye", "Bool_gwangundae", "Bool_seokgye", "Bool_sinyeemoon"
            , "Bool_wedaeap", "Bool_hwegi", "FREE", "Bool_jegi", "Bool_sinsul", "Bool_dongmyo", "Bool_dongdaemoon", "Bool_jongro5", "Bool_jongro3(1)", "Bool_jongkak", "Bool_sichung", "FREE", "Bool_namyoung"
            , "Bool_yongsan", "Bool_noryangjin", "Bool_debang", "Bool_singeal", "Bool_yeongdeungpo", "Bool_sindorim", "Bool_guro", "Bool_guil", "Bool_gaebong", "Bool_oryu", "Bool_onsu", "Bool_yeokgok", "Bool_sosa", "Bool_bucheon"
            , "Bool_jungdong", "Bool_songnae", "Bool_bugae", "Bool_bupeong", "Bool_backun", "Bool_dongam", "Bool_ganseok", "Bool_juan", "Bool_dowha", "Bool_jemulpo", "Bool_dowon", "Bool_dongincheon", "Bool_incheon", "Bool_gasan"
            , "Bool_doksan", "Bool_geumcheon", "Bool_seoksu", "Bool_gwuanak", "Bool_anyang", "Bool_myeonghak", "Bool_geumjung", "Bool_gunpo", "Bool_dangjung", "Bool_ewang", "Bool_sungkyunkwan", "Bool_whaseo", "FREE"
            , "Bool_beongjum", "Bool_seodongtan", "Bool_seryu", "Bool_sema", "Bool_osandae", "Bool_osan", "Bool_jinwee", "Bool_songtan", "Bool_sujunglee", "Bool_jije", "Bool_peongtaek", "Bool_sungwhan", "Bool_jiksan", "Bool_dujung"
            , "Bool_chunan", "Bool_bongmyeong", "Bool_ssangyong", "Bool_asan", "Bool_baebang", "Bool_onyang", "Bool_sinchang" };

    private string[] line2StationDataNames = { "Bool_sinsul_2", "Bool_yongdo", "Bool_sindab", "Bool_yongdab", "Bool_sungsu", "Bool_gundae", "Bool_guyee", "Bool_gangbyeon", "Bool_jamsilnaru", "Bool_jamsil", "Bool_sinchun", "Bool_jonghap",
        "Bool_samsung", "Bool_sunleng", "Bool_yuksam", "Bool_gangnam", "Bool_gyodae", "Bool_seocho", "Bool_bangbae", "Bool_sadang", "Bool_naksungdae", "Bool_seouldae", "Bool_bongchun", "Bool_sinleam", "Bool_sindaebang", "Bool_gurodigital",
        "Bool_daelim", "Bool_sindorim_2", "Bool_moonrae", "Bool_youngdengpo_guchung", "Bool_dangsan", "Bool_hapjung", "Bool_hongdae", "Bool_sinchon", "Bool_idae", "Bool_ahyun", "Bool_chungjungro", "Bool_sichung_2", "Bool_eljiroipgu",
        "Bool_eljiro3", "Bool_eljiro4", "Bool_dongdaemoon_yeoksa", "Bool_sindang", "Bool_sangwangsipri", "Bool_wangsipri", "Bool_hanyangdae", "Bool_dduksum", "Bool_ggachisan", "Bool_sinjung4", "Bool_yangchun", "Bool_dorimchun"};

    private string[] line3StationDataNames = { "Bool_daewha", "Bool_juyup", "Bool_jungbalsan", "Bool_madu", "Bool_baksuk", "Bool_daegok", "Bool_whajung", "Bool_wondang", "Bool_wonheng", "Bool_samsong", "Bool_jichuk", "Bool_gupabal",
        "Bool_yunsinnae", "Bool_bulgwang", "Bool_nokbeon", "Bool_hongje", "Bool_muakjae", "Bool_doklipmoon", "Bool_gyeongbokgung", "Bool_angook", "Bool_jongro3", "Bool_eljiro3_3", "Bool_chungmuro", "Bool_dongdae", "Bool_yaksu", 
        "Bool_geumho", "Bool_oksu", "Bool_apgujung", "Bool_sinsa", "Bool_jamwon", "Bool_gosokterminal", "Bool_gyodae_4", "Bool_nambuterminal", "Bool_yangjae", "Bool_maebong", "Bool_dogok", "Bool_daechi", "Bool_hakyeoul", "Bool_daechung",
        "Bool_ilwon", "Bool_suseo", "Bool_garak", "Bool_gyeongchal", "Bool_ogeum" };

    private string[] line4StationDataNames = { "Bool_danggogae", "Bool_sanggye", "Bool_nowon", "Bool_changdong", "Bool_ssangmoon", "Bool_suyu", "Bool_mia", "Bool_mia4", "Bool_gilem", "Bool_sungsinyeodae", "Bool_hansungdae", "Bool_hyewha", 
        "Bool_dongdaemoon_4", "Bool_dongdaemoon_yeoksa_4", "Bool_chungmuro_4", "Bool_myeongdong", "Bool_whehyun", "Bool_seoul_4", "Bool_sukdae", "Bool_samgakji", "Bool_sinyongsan", "Bool_ichon", "Bool_dongjak", "Bool_chongsindae", 
        "Bool_sadang_4", "Bool_namtaeleong", "Bool_sunbawi", "Bool_gyeongma", "Bool_daegongwon", "Bool_gwachun", "Bool_jungbugwachun", "Bool_indukwon", "Bool_pyeongchon", "Bool_bumgye", "Bool_geumjung_4", "Bool_sanbon", "Bool_surisan",
        "Bool_daeyami", "Bool_banwall", "Bool_sangroksu", "Bool_handae", "Bool_jungang", "Bool_gojan", "Bool_choji", "Bool_ansan", "Bool_singealonchun", "Bool_jungwang", "Bool_oido" };

    string[] mp2Codes = { "BD", "SinBD", "SuIn", "IC1", "IC2" };

    #region Scripts
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
    #endregion

    public void Convert()
    {
        if (!IsConverted)
        {
            ConvertTrainData();
            ConvertStationData();
            ConvertExpandData();
            ConvertConnectLineData();
            ConvertScreendoorData();
            ConvertMyAssetData();
            ConvertCompanyData();
            ConvertRentData();
            ConvertItemData();
            ConvertPlayData();
            ConvertEventData();
            ConvertBankData();
            ConvertCompensationData();
            ConvertDriverData();
            ConvertSettingData();
            IsConverted = true;
            DataManager.instance.SaveAll();
#if UNITY_EDITOR
            Debug.Log("Data Converted");
#endif
        }
    }

    private void ConvertTrainData()
    {
        for(int i = 0; i < 9; i++)
        {
            lineManager.lineCollections[i].lineData.numOfTrain = PlayerPrefs.GetInt("NumOfTrain_" + (i + 1));
            if (i.Equals(0))
                lineManager.lineCollections[i].lineData.limitTrain = PlayerPrefs.GetInt("limit_train_" + (i + 1), 10);
            else
                lineManager.lineCollections[i].lineData.limitTrain = PlayerPrefs.GetInt("limit_train_" + (i + 1));
            lineManager.lineCollections[i].lineData.numOfBase = PlayerPrefs.GetInt("numOfBase_" + (i + 1));
            lineManager.lineCollections[i].lineData.numOfBaseEx = PlayerPrefs.GetInt("NumOfBaseEx_" + (i + 1));
            if(i < 4)
            {
                for (int j = 0; j < 4; j++)
                    lineManager.lineCollections[i].lineData.trainExpandStatus[j] = PlayerPrefs.GetInt((i + 1) + "_" + (4 + j * 2));
            }
            else
            {
                for (int j = 0; j < 4; j++)
                    lineManager.lineCollections[i].lineData.trainExpandStatus[j] = PlayerPrefs.GetInt("Length_Line" + (i + 1) + "[" + j + "]");
            }
        }
        for(int i = 9; i < 14; i++)
        {
            int l = i - 9;
            lineManager.lineCollections[i].lineData.numOfTrain = PlayerPrefs.GetInt("numofTrain_B" + (l + 1));
            lineManager.lineCollections[i].lineData.limitTrain = PlayerPrefs.GetInt("limit_train_B" + (l + 1));
            lineManager.lineCollections[i].lineData.numOfBase = PlayerPrefs.GetInt("numofBase_B" + (l + 1));
            lineManager.lineCollections[i].lineData.numOfBaseEx = PlayerPrefs.GetInt("numofBaseEx_B" + (l + 1));

            for (int j = 0; j < 4; j++)
                lineManager.lineCollections[i].lineData.trainExpandStatus[j] = PlayerPrefs.GetInt("Length_B" + (l + 1) + "[" + j + "]");
        }
        for(int i = 15; i < 18; i++)
        {
            int l = i - 15;
            lineManager.lineCollections[i].lineData.numOfTrain = EncryptedPlayerPrefs.GetInt("numofTrain_Dg[" + l + "]");
            lineManager.lineCollections[i].lineData.limitTrain = EncryptedPlayerPrefs.GetInt("limit_train_Dg[" + l + "]");
            lineManager.lineCollections[i].lineData.numOfBase = EncryptedPlayerPrefs.GetInt("numofBase_Dg[" + l + "]");
            lineManager.lineCollections[i].lineData.numOfBaseEx = EncryptedPlayerPrefs.GetInt("numofBaseEx_Dg[" + l + "]");

            for (int j = 0; j < 4; j++)
                lineManager.lineCollections[i].lineData.trainExpandStatus[j] = EncryptedPlayerPrefs.GetInt("Length_Dg" + (l + 1) + "[" + j + "]");
        }
        for(int i = 18; i < 23; i++)
        {
            int l = i - 18;
            lineManager.lineCollections[i].lineData.numOfTrain = EncryptedPlayerPrefs.GetInt("numofTrain_MP2[" + l + "]");
            lineManager.lineCollections[i].lineData.limitTrain = EncryptedPlayerPrefs.GetInt("limit_train_MP2[" + l + "]");
            lineManager.lineCollections[i].lineData.numOfBase = EncryptedPlayerPrefs.GetInt("numofBase_MP2[" + l + "]");
            lineManager.lineCollections[i].lineData.numOfBaseEx = EncryptedPlayerPrefs.GetInt("numofBaseEx_MP2[" + l + "]");
            for (int j = 0; j < 4; j++)
                lineManager.lineCollections[i].lineData.trainExpandStatus[j] = EncryptedPlayerPrefs.GetInt("Length_" + mp2Codes[l] + "[" + j + "]");
        }
    }

    private void ConvertStationData()
    {
        #region MP1
        for (int i = 0; i < line1StationDataNames.Length; i++)
        {
            if (!line1StationDataNames[i].Equals("FREE"))
                lineManager.lineCollections[0].lineData.hasStation[i] = PlayerPrefs.GetInt(line1StationDataNames[i]).Equals(1) ? true : false;
            else
                lineManager.lineCollections[0].lineData.hasStation[i] = true;
        }
        for(int i = 0; i < line2StationDataNames.Length; i++)
            lineManager.lineCollections[1].lineData.hasStation[i] = PlayerPrefs.GetInt(line2StationDataNames[i]).Equals(1) ? true : false;
        for (int i = 0; i < line3StationDataNames.Length; i++)
            lineManager.lineCollections[2].lineData.hasStation[i] = PlayerPrefs.GetInt(line3StationDataNames[i]).Equals(1) ? true : false;
        for (int i = 0; i < line4StationDataNames.Length; i++)
            lineManager.lineCollections[3].lineData.hasStation[i] = PlayerPrefs.GetInt(line4StationDataNames[i]).Equals(1) ? true : false;

        bool[] tmp = new bool[lineManager.lineCollections[4].lineData.hasStation.Length];
        for (int j = 0; j < lineManager.lineCollections[4].lineData.hasStation.Length; j++) // 5È£¼± ¸ÅÄª
             tmp[j] = PlayerPrefs.GetInt("bool_stations5[" + j + "]").Equals(1) ? true : false;
        for (int j = 0; j < 44; j++)
            lineManager.lineCollections[4].lineData.hasStation[j] = tmp[j];
        for(int j = 44; j < 49; j++)
            lineManager.lineCollections[4].lineData.hasStation[j] = tmp[j + 7];
        for (int j = 49; j < 56; j++)
            lineManager.lineCollections[4].lineData.hasStation[j] = tmp[j - 5];

        tmp = new bool[lineManager.lineCollections[5].lineData.hasStation.Length];
        for (int j = 0; j < lineManager.lineCollections[5].lineData.hasStation.Length; j++) // 6È£¼± ¸ÅÄª
            tmp[j] = PlayerPrefs.GetInt("bool_stations6[" + j + "]").Equals(1) ? true : false;
        lineManager.lineCollections[5].lineData.hasStation[0] = tmp[38];
        for (int j = 1; j < lineManager.lineCollections[5].lineData.hasStation.Length; j++) // 6È£¼± ¸ÅÄª
            lineManager.lineCollections[5].lineData.hasStation[j] = tmp[j - 1];

        for (int i = 6; i < 9; i++)
        {
            for (int j = 0; j < lineManager.lineCollections[i].lineData.hasStation.Length; j++)
                lineManager.lineCollections[i].lineData.hasStation[j] = PlayerPrefs.GetInt("bool_stations" + (i + 1) + "[" + j + "]").Equals(1) ? true : false;
        }
        #endregion
        #region Busan
        for (int i = 9; i < 13; i++)
            for(int j = 0; j < lineManager.lineCollections[i].lineData.hasStation.Length; j++)
                lineManager.lineCollections[i].lineData.hasStation[j] = PlayerPrefs.GetInt("bool_stationsB" + (i - 8) + "[" + j + "]").Equals(1) ? true : false;
        for (int j = 0; j < lineManager.lineCollections[13].lineData.hasStation.Length; j++)
            lineManager.lineCollections[13].lineData.hasStation[j] = PlayerPrefs.GetInt("bool_stationsBK[" + j + "]").Equals(1) ? true : false;
        bool[] priorStations = lineManager.lineCollections[14].lineData.hasStation;
        lineManager.lineCollections[14].lineData.hasStation = new bool[23];
        for (int i = 8; i < 23; i++)
            lineManager.lineCollections[14].lineData.hasStation[i] = priorStations[i - 8];
        #endregion
        #region Daegu
        for (int i = 15; i < 18; i++)
            for (int j = 0; j < lineManager.lineCollections[i].lineData.hasStation.Length; j++)
                lineManager.lineCollections[i].lineData.hasStation[j] = PlayerPrefs.GetInt("bool_stationsDg" + (i - 14) + "[" + j + "]").Equals(1) ? true : false;
        #endregion
        #region MP2
        for (int i = 18; i < 23; i++)
        {
            if(i.Equals((int)Line.SuinBundang))
            {
                for (int j = 0; j < lineManager.lineCollections[i].lineData.hasStation.Length; j++)
                {
                    if(j < 14)
                        lineManager.lineCollections[i].lineData.hasStation[j] = EncryptedPlayerPrefs.GetInt("bool_stationsSuIn" + j).Equals(1) ? true : false;
                    else if(j < 26)
                        lineManager.lineCollections[i].lineData.hasStation[j] = EncryptedPlayerPrefs.GetInt("bool_stationsSuInBD" + (j - 14)).Equals(1) ? true : false;
                    else
                        lineManager.lineCollections[i].lineData.hasStation[j] = EncryptedPlayerPrefs.GetInt("bool_stationsBD" + (j - 26)).Equals(1) ? true : false;
                }
            }
            else
                for (int j = 0; j < lineManager.lineCollections[i].lineData.hasStation.Length; j++)
                    lineManager.lineCollections[i].lineData.hasStation[j] = EncryptedPlayerPrefs.GetInt("bool_stations" + mp2Codes[i - 18] + j).Equals(1) ? true : false;
        }
        #endregion
    }

    private void ConvertExpandData()
    {
        #region MP1-Line1~4
        lineManager.lineCollections[(int)Line.Line1].lineData.sectionExpanded[0] = PlayerPrefs.GetInt("Bool_Expand1_3").Equals(1) ? true : false;
        lineManager.lineCollections[(int)Line.Line1].lineData.sectionExpanded[1] = true;
        lineManager.lineCollections[(int)Line.Line1].lineData.sectionExpanded[2] = true;
        lineManager.lineCollections[(int)Line.Line1].lineData.sectionExpanded[3] = PlayerPrefs.GetInt("Bool_Expand1_2").Equals(1) ? true : false;
        lineManager.lineCollections[(int)Line.Line1].lineData.sectionExpanded[4] = true;
        lineManager.lineCollections[(int)Line.Line1].lineData.sectionExpanded[5] = PlayerPrefs.GetInt("Bool_Expand_seodongtan").Equals(1) ? true : false;
        lineManager.lineCollections[(int)Line.Line1].lineData.sectionExpanded[6] = PlayerPrefs.GetInt("Bool_Expand1_4").Equals(1) ? true : false;
        lineManager.lineCollections[(int)Line.Line1].lineData.sectionExpanded[7] = PlayerPrefs.GetInt("Bool_Expand1_4").Equals(1) ? true : false;
        
        bool line2Expand = PlayerPrefs.GetInt("Bool_Expand2").Equals(1) ? true : false;
        for (int i = 0; i < lineManager.lineCollections[1].lineData.sectionExpanded.Length; i++)
            lineManager.lineCollections[1].lineData.sectionExpanded[i] = line2Expand;
        
        for (int i = 2; i < 4; i++)
        {
            for (int j = 0; j < lineManager.lineCollections[i].lineData.sectionExpanded.Length; j++)
                lineManager.lineCollections[i].lineData.sectionExpanded[j] = PlayerPrefs.GetInt("Bool_Expand" + (i + 1) + "_" + (j + 1)).Equals(1) ? true : false;
        }
        #endregion
        #region MP1-Line5~9
        for(int i = 4; i < 9; i++)
        {
            for (int j = 0; j < lineManager.lineCollections[i].lineData.sectionExpanded.Length; j++)
                lineManager.lineCollections[i].lineData.sectionExpanded[j] = GetPlayerPrefIntToBool("bool_expand" + (i + 1) + "[" + (j + 1) + "]");
        }
        #endregion
        #region Busan
        for(int i = 9; i < 14; i++)
        {
            int l = i - 9;
            if(i <= 10)
                for(int j = 0; j < lineManager.lineCollections[i].lineData.sectionExpanded.Length; j++)
                    lineManager.lineCollections[i].lineData.sectionExpanded[j] = GetPlayerPrefIntToBool("Bool_ExpandB" + (l + 1) + "_" + (j + 1));
            else
            {
                lineManager.lineCollections[i].lineData.sectionExpanded[0] = GetPlayerPrefIntToBool("Bool_ExpandB" + (l + 1));
            }
        }
        #endregion
        #region Daegu
        for(int i = 15; i < 18; i++)
        {
            lineManager.lineCollections[i].lineData.sectionExpanded[0] = GetEncPlayerPrefIntToBool("Bool_ExpandDg" + (i - 14));
        }
        #endregion
        #region MP2
        for (int i = 18; i < 23; i++)
        {
            int l = i - 18;
            lineManager.lineCollections[i].lineData.sectionExpanded[0] = GetEncPlayerPrefIntToBool("Bool_Expand" + mp2Codes[l]);
        }
        #endregion
    }

    private void ConvertConnectLineData()
    {
        #region MP1
        for(int line = 0; line < 9; line++)
        {
            for (int i = 0; i < lineManager.lineCollections[line].lineData.connected.Length; i++)
                lineManager.lineCollections[line].lineData.connected[i] = GetPlayerPrefIntToBool("Connect" + (line + 1) + "_Bool_case" + (i + 1));
        }
        #endregion
        #region Busan
        for (int line = 9; line < 14; line++)
        {
            int l = line - 9;
            if(line <= 10)
                for (int i = 0; i < lineManager.lineCollections[line].lineData.connected.Length; i++)
                    lineManager.lineCollections[line].lineData.connected[i] = GetPlayerPrefIntToBool("ConnectB" + (l + 1) + "_bool_" + (i + 1));
            else
                lineManager.lineCollections[line].lineData.connected[0] = GetPlayerPrefIntToBool("ConnectB" + (l + 1) + "_bool");
        }
        #endregion
        #region Daegu
        for (int line = 15; line < 18; line++)
        {
            lineManager.lineCollections[line].lineData.connected[0] = GetEncPlayerPrefIntToBool("ConnectDg" + (line - 14) + "_bool");
        }
        #endregion
        #region MP2
        for(int line = 18; line < 23; line++)
        {
            int l = line - 18;
            if(line.Equals((int)Line.SuinBundang))
            {
                bool suinConnected = GetEncPlayerPrefIntToBool("ConnectSuIn_bool");
                bool suinBDConnected = GetEncPlayerPrefIntToBool("ConnectSuInBD_bool");
                if(suinConnected ^ suinBDConnected)
                {
                    //TouchMoneyManager.ArithmeticOperation(50000000000000, 0, true);
                    lineManager.lineCollections[line].lineData.connected[0] = true;
                }
                else if(suinConnected & suinBDConnected)
                {
                    lineManager.lineCollections[line].lineData.connected[0] = true;
                }
            }
            else
            {
                lineManager.lineCollections[line].lineData.connected[0] = GetEncPlayerPrefIntToBool("Connect" + mp2Codes[l] + "_bool");
            }
        }
        #endregion
    }

    private void ConvertScreendoorData()
    {
        #region MP1
        for (int line = 0; line < 9; line++)
            for (int i = 0; i < lineManager.lineCollections[line].lineData.installed.Length; i++)
                lineManager.lineCollections[line].lineData.installed[i] = GetPlayerPrefIntToBool("Screen" + (line + 1) + "_Bool_case" + (i + 1));
        #endregion
        #region Busan
        for (int line = 9; line < 14; line++)
        {
            int l = line - 9;
            if (line <= 10)
                for (int i = 0; i < lineManager.lineCollections[line].lineData.installed.Length; i++)
                    lineManager.lineCollections[line].lineData.installed[i] = GetPlayerPrefIntToBool("ScreenB" + (l + 1) + "_bool_" + (i + 1));
            else
                lineManager.lineCollections[line].lineData.installed[0] = GetPlayerPrefIntToBool("ScreenB" + (l + 1) + "_bool");
        }
        #endregion
        #region Daegu
        for (int line = 15; line < 18; line++)
        {
            lineManager.lineCollections[line].lineData.installed[0] = GetEncPlayerPrefIntToBool("ScreenDg" + (line - 14) + "_bool");
        }
        #endregion
        #region MP2
        for (int line = 18; line < 23; line++)
        {
            lineManager.lineCollections[line].lineData.installed[0] = GetEncPlayerPrefIntToBool("Screen" + mp2Codes[line - 18] + "_bool");
        }
        #endregion
    }

    private void ConvertMyAssetData()
    {
        myAsset.myAssetData.moneyLow = ulong.Parse(EncryptedPlayerPrefs.GetString("Now_Money_enc", "0"));
        myAsset.myAssetData.timePerEarningLow = ulong.Parse(EncryptedPlayerPrefs.GetString("Time_Per_Money_enc", "0"));
        myAsset.myAssetData.numOfStations = EncryptedPlayerPrefs.GetInt("NumOfStations_enc", 3);
        myAsset.myAssetData.passengersLow = ulong.Parse(EncryptedPlayerPrefs.GetString("Passengers_enc", "1"));
        myAsset.myAssetData.passengersLimitLow= ulong.Parse(EncryptedPlayerPrefs.GetString("Passengers_limit_enc", "100"));
        myAsset.myAssetData.passengersHigh = ulong.Parse(EncryptedPlayerPrefs.GetString("passengers_num2_enc", "0"));
        myAsset.myAssetData.passengersLimitHigh = ulong.Parse(EncryptedPlayerPrefs.GetString("passengers_limit2_enc", "0"));
        myAsset.myAssetData.moneyHigh = ulong.Parse(EncryptedPlayerPrefs.GetString("Now_Money2_enc", "0"));
    }

    private void ConvertCompanyData()
    {
        company_Reputation_Controller.companyData.reputationTotalValue = PlayerPrefs.GetInt("Rep_Totalvalue", 500);
        company_Reputation_Controller.companyData.additionalPercentage = PlayerPrefs.GetInt("Rep_Percentage", 100);
        company_Reputation_Controller.companyData.reputationPercentage = PlayerPrefs.GetInt("Rep_REP_Percentage", 10);
        company_Reputation_Controller.companyData.averageSanitoryCondition = PlayerPrefs.GetInt("ASC", 20);
        company_Reputation_Controller.companyData.peaceValue = PlayerPrefs.GetInt("Peace_Value", 20);
        company_Reputation_Controller.companyData.peaceCoolTime = PlayerPrefs.GetInt("CoolTime_Peace", 5);
        company_Reputation_Controller.companyData.sanitoryCoolTime = PlayerPrefs.GetInt("CoolTime_San", 5);
        company_Reputation_Controller.companyData.valuePoint = EncryptedPlayerPrefs.GetInt("vP_ValuePoint_enc");
        company_Reputation_Controller.companyData.numOfSuccess = EncryptedPlayerPrefs.GetInt("vP_NumofSuccess_enc");

    }

    private void ConvertRentData()
    {
        rent.rentData.numOfFacilities[0] = PlayerPrefs.GetInt("NumOfVm");
        rent.rentData.numOfFacilities[1] = PlayerPrefs.GetInt("NumOfCon");
        rent.rentData.numOfFacilities[2] = PlayerPrefs.GetInt("NumOfAD");
        rent.rentData.numOfFacilities[3] = PlayerPrefs.GetInt("NumOfDps");
        rent.rentData.numOfADWatch = PlayerPrefs.GetInt("NumofADWatch");
        rent.rentData.rentSpaceAmount = PlayerPrefs.GetInt("NumofRent");
        rent.rentData.numOfRented[0] = PlayerPrefs.GetInt("NumofLazer");
        rent.rentData.numOfRented[1] = PlayerPrefs.GetInt("NumofFox");
        rent.rentData.numOfRented[2] = PlayerPrefs.GetInt("NumofCat");
        rent.rentData.numOfRented[3] = PlayerPrefs.GetInt("NumofBox");
        rent.rentData.numOfRented[4] = PlayerPrefs.GetInt("NumofCs25");
        rent.rentData.numOfRented[5] = PlayerPrefs.GetInt("NumofIcu");
        rent.rentData.numOfRented[6] = PlayerPrefs.GetInt("NumofMinitop");
        rent.rentData.totalAccepted = PlayerPrefs.GetInt("NumofAccepted");
        rent.rentData.checkEmpty = PlayerPrefs.GetInt("CheckEmpty");
        rent.rentData.waitingRentTimeMoney[0] = ulong.Parse(PlayerPrefs.GetString("PRICE1", "0"));
        rent.rentData.waitingRentTimeMoney[1] = ulong.Parse(PlayerPrefs.GetString("PRICE2", "0"));
        rent.rentData.waitingRentTimeMoney[2] = ulong.Parse(PlayerPrefs.GetString("PRICE3", "0"));
        rent.rentData.waitingRentTimeMoney[3] = ulong.Parse(PlayerPrefs.GetString("PRICE4", "0"));
        rent.rentData.waitingRentTimeMoney[4] = ulong.Parse(PlayerPrefs.GetString("PRICE5", "0"));
        rent.rentData.waitingRentNames[0] = PlayerPrefs.GetString("Name1");
        rent.rentData.waitingRentTypes[0] = PlayerPrefs.GetString("Type1");
        rent.rentData.waitingRentNames[1] = PlayerPrefs.GetString("Name2");
        rent.rentData.waitingRentTypes[1] = PlayerPrefs.GetString("Type2");
        rent.rentData.waitingRentNames[2] = PlayerPrefs.GetString("Name3");
        rent.rentData.waitingRentTypes[2] = PlayerPrefs.GetString("Type3");
        rent.rentData.waitingRentNames[3] = PlayerPrefs.GetString("Name4");
        rent.rentData.waitingRentTypes[3] = PlayerPrefs.GetString("Type4");
        rent.rentData.waitingRentNames[4] = PlayerPrefs.GetString("Name5");
        rent.rentData.waitingRentTypes[4] = PlayerPrefs.GetString("Type5");
        rent.rentData.quickRentCoolTime = PlayerPrefs.GetInt("Req_Cooltime");
    }

    private void ConvertItemData()
    {
        itemManager.itemData.cardAmounts[0] = PlayerPrefs.GetInt("NumofRedCard");
        itemManager.itemData.cardAmounts[1] = PlayerPrefs.GetInt("NumofOrangeCard");
        itemManager.itemData.cardAmounts[2] = PlayerPrefs.GetInt("NumofYellowCard");
        itemManager.itemData.cardAmounts[3] = PlayerPrefs.GetInt("NumofGreenCard");
        itemManager.itemData.silverCardAmount = PlayerPrefs.GetInt("NumofSilver");
    }

    private void ConvertPlayData()
    {
        playManager.playData.playTimeHour =  PlayerPrefs.GetInt("Playtime_hour");
        playManager.playData.playTimeMin = PlayerPrefs.GetInt("Playtime_minute");
        playManager.playData.playTimeSec = PlayerPrefs.GetInt("Playtime_second");
        playManager.playData.notTutorialPlayed = bool.Parse(EncryptedPlayerPrefs.GetString("notTutorialPlayed", "TRUE"));
        playManager.playData.backgroundType = EncryptedPlayerPrefs.GetInt("backgroundType");
        playManager.playData.patchNoteVersionCode = PlayerPrefs.GetInt("isSaw");
    }

    private void ConvertEventData()
    {
        event_Manager.eventData.isRecommended =  GetPlayerPrefIntToBool("isRecommended");
        event_Manager.eventData.isSurveyed = bool.Parse(EncryptedPlayerPrefs.GetString("isSurveyed", "false"));
    }

    private void ConvertBankData()
    {
        for(int i = 0; i < bankManager.bankData.savedMoneyNormal.Length; i++)
        {
            bankManager.bankData.savedMoneyNormal[i] = ulong.Parse(PlayerPrefs.GetString("SavedMoney[" + i + "]", "0"));
            bankManager.bankData.addedMoneyNormal[i] = ulong.Parse(PlayerPrefs.GetString("AddedMoney[" + i + "]", "0"));
            bankManager.bankData.contractTimesNormal[i] = PlayerPrefs.GetInt("contract_time[" + i + "]");
            bankManager.bankData.isRegisteredNormal[i] = GetPlayerPrefIntToBool("registered[" + i + "]");
        }

        for (int i = 0; i < bankManager.bankData.savedMoneySpecial.Length; i++)
        {
            bankManager.bankData.savedMoneySpecial[i] = ulong.Parse(PlayerPrefs.GetString("S_SavedMoney[" + i + "]", "0"));
            bankManager.bankData.addedMoneySpecialLow[i] = ulong.Parse(PlayerPrefs.GetString("S_AddedMoney[" + i + "]", "0"));
            bankManager.bankData.addedMoneySpecialHigh[i] = ulong.Parse(PlayerPrefs.GetString("S_AddedMoney2[" + i + "]", "0"));
            bankManager.bankData.contractTimesSpecial[i] = PlayerPrefs.GetInt("S_contract_time[" + i + "]");
            bankManager.bankData.isRegisteredSpecial[i] = GetPlayerPrefIntToBool("S_registered[" + i + "]");
        }
        bankManager.bankData.timer = PlayerPrefs.GetInt("S_Timeleft");
    }

    private void ConvertCompensationData()
    {
        compensation_Manager.compensationData.bsCompensationChecked = bool.Parse(EncryptedPlayerPrefs.GetString("bsCompensationChecked", "FALSE"));
        compensation_Manager.compensationData.expandTrainChecked = bool.Parse(EncryptedPlayerPrefs.GetString("expandTrainCompChecked", "FALSE")); 
        compensation_Manager.compensationData.expandTrainPassengerChecked = bool.Parse(EncryptedPlayerPrefs.GetString("expandTrainPassengerChecked", "FALSE"));
        compensation_Manager.compensationData.version3_0_1_checked = bool.Parse(EncryptedPlayerPrefs.GetString("version3_0_1_checked", "FALSE"));
    }

    private void ConvertDriverData()
    {
        drivers_Manager.driverData.salaryTimer = PlayerPrefs.GetInt("Salary_nextTime");
        for (int i = 0; i < drivers_Manager.driverData.numOfDrivers.Length; i++)
            drivers_Manager.driverData.numOfDrivers[i] = PlayerPrefs.GetInt("numofDrivers[" + i + "]");
    }

    private void ConvertSettingData()
    {
        setting.settingData.soundVolume = PlayerPrefs.GetFloat("soundVolume", 1f);
        setting.settingData.playTimeActive = GetPlayerPrefIntToBool("Bool_Playtime_o", 1);
        setting.settingData.updateAlarm = GetPlayerPrefIntToBool("Bool_updateAlarm", 1);
        setting.settingData.peaceEventGameActive = bool.Parse(PlayerPrefs.GetString("Bool_peace", "TRUE"));
        setting.settingData.easyPurchaseType = PlayerPrefs.GetInt("EasyPurchaseType", 1);
        setting.settingData.addedMoneyEffect = bool.Parse(PlayerPrefs.GetString("BoolAddedMoneyEffect", "TRUE"));
    }

    private bool GetPlayerPrefIntToBool(string name, int defaultValue = -99)
    {
        if(defaultValue.Equals(-99))
            return PlayerPrefs.GetInt(name).Equals(1) ? true : false;
        else
            return PlayerPrefs.GetInt(name, defaultValue).Equals(1) ? true : false;
    }
    private bool GetEncPlayerPrefIntToBool(string name)
    {
        return EncryptedPlayerPrefs.GetInt(name).Equals(1) ? true : false;
    }
}
