using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BalanceReviser : MonoBehaviour
{
    /// <summary>
    /// 엑셀로 계산된 노선별 평균 누적 터치형 수익 감소율
    /// </summary>
    private float[] moneyReductionRatio = { 0.512435360748584f, 0.410850350502895f, 0.424487082066869f, 0.592550447580033f, 0.75687048106067f, 0.689090979593419f, 
        0.691036740470427f, 0.756588107685284f, 0.740875915633339f, 0.642735991436623f, 0.457666681973916f, 0.40026418045438f, 0.345166726763936f, 0.345290001094877f,
        0.425092482362337f, 0.516356504874206f, 0.502356739761694f, 0.449114487110048f, 0.391787874668177f, 0.30419273362301f, 0.140802789419222f, 0.0472239595989978f, 
        0.0247064219227298f, 0.0166791836993827f, 0.0138492544537495f, 0.0147889995378792f };

    public bool IsRevised { get { return PlayManager.instance.playData.isRevised; } set { PlayManager.instance.playData.isRevised = value; } }
    public bool IsRevised_3_1_4 { get { return PlayManager.instance.playData.isRevised_3_1_4; } set { PlayManager.instance.playData.isRevised_3_1_4 = value; } }

    public LineManager lineManager;
    public DriversManager driversManager;
    public RentManager rentManager;
    public BankManager bankManager;
    public BankSpecialManager bankSpecialManager;
    public CompanyReputationManager companyReputationManager;
    public LightRailControlManager lightRailControlManager;
    public GameObject reviseMessage;

    public GameObject RevisedMenu;
    public Text revisedLogText;
    private string log = "";

    public void Revise()
    {
        if (!IsRevised && lineManager.lineCollections[0].lineData.numOfTrain > 0)
        {
            DepositAll();
            ReviseMoney();
            RevisePassenger();
            RevisePassengerLimit();
            ReviseTimeMoney();
            ReviseLineConnectReward();
            ReviseCompanyReputation();
            IsRevised = true;
            DataManager.instance.SaveAll();

            RevisedMenu.SetActive(true);
            revisedLogText.text = log;
#if UNITY_EDITOR
            Debug.Log("Balance Related Data Revised");
#endif
        }
        else if (lineManager.lineCollections[0].lineData.numOfTrain.Equals(0))
            IsRevised = true;

        /*if (lineManager.lineCollections[(int)Line.SuinBundang].lineData.connected.Length > 1)
        {
            bool subdConnected = lineManager.lineCollections[(int)Line.SuinBundang].lineData.connected[0];
            lineManager.lineCollections[(int)Line.SuinBundang].lineData.connected = new bool[1];
            lineManager.lineCollections[(int)Line.SuinBundang].lineData.connected[0] = subdConnected;
        }

        if (lineManager.lineCollections[(int)Line.SuinBundang].lineData.sectionExpanded.Length > 1)
        {
            bool subdExpanded = lineManager.lineCollections[(int)Line.SuinBundang].lineData.sectionExpanded[0];
            lineManager.lineCollections[(int)Line.SuinBundang].lineData.sectionExpanded = new bool[1];
            lineManager.lineCollections[(int)Line.SuinBundang].lineData.sectionExpanded[0] = subdExpanded;
        }*/
        if(!IsRevised_3_1_4 && lineManager.lineCollections[0].lineData.numOfTrain > 0)
        {
            ReviseAfter3_1_4();
            IsRevised_3_1_4 = true;
            DataManager.instance.SaveAll();

#if UNITY_EDITOR
            Debug.Log("3.1.4 Data Revised");
#endif
        }
        else if(lineManager.lineCollections[0].lineData.numOfTrain == 0)
        {
            IsRevised_3_1_4 = true;
            DataManager.instance.SaveAll();
        }
    }

    public void CloseRevisedMenu()
    {
        RevisedMenu.SetActive(false);
    }

    private void DepositAll()
    {
        ulong lowWithdraw = 0, highWithdraw = 0;
        string unitLow = "", unitHigh = "";
        for (int i = 0; i < 3; i++)
        {
            lowWithdraw += bankManager.QuickWithdraw(i);

            ulong returnLow = 0, returnHigh = 0;
            bankSpecialManager.QuickWithdraw(i, ref returnLow, ref returnHigh);
            lowWithdraw += returnLow;
            highWithdraw += returnHigh;
            MoneyUnitTranslator.Arrange(ref lowWithdraw, ref highWithdraw);
        }

        PlayManager.ArrangeUnit(lowWithdraw, highWithdraw, ref unitLow, ref unitHigh, true);
        log += "출금된 금액: <color=lime>" + unitHigh + unitLow + "$</color>\n";
    }

    private void ReviseMoney()
    {
        int recentLine = (int)lineManager.GetRecentlyOpenedLine();
        ulong lowMoney = MyAsset.instance.MoneyLow, highMoney = MyAsset.instance.MoneyHigh;
        AssetMoneyCalculator.instance.PercentOperation(moneyReductionRatio[recentLine]);

        string varianceLowUnit = "", varianceHighUnit = "", priorLowUnit = "", priorHighUnit = "", changedLowUnit = "", changedHighUnit = "";
        PlayManager.ArrangeUnit(lowMoney, highMoney, ref priorLowUnit, ref priorHighUnit, true);
        PlayManager.ArrangeUnit(MyAsset.instance.MoneyLow, MyAsset.instance.MoneyHigh, ref changedLowUnit, ref changedHighUnit, true);
        PlayManager.ArrangeUnit(lowMoney - MyAsset.instance.MoneyLow, highMoney - MyAsset.instance.MoneyHigh, ref varianceLowUnit, ref varianceHighUnit, true);
        log += string.Format("보유한 돈: {0}{1}$ → <color=lime>{2}{3}$</color> (<color=red>-{4}{5}$, {6:0.00}% 감소</color>)\n", priorHighUnit, priorLowUnit, changedHighUnit, changedLowUnit, varianceHighUnit, varianceLowUnit, (1 - moneyReductionRatio[recentLine]) * 100);
        log += "<color=grey>(은행에서 출금된 돈이 함께 처리되었습니다.)</color>\n";
    }

    private void RevisePassenger()
    {
        ulong priorLow = MyAsset.instance.PassengersLow, priorHigh = MyAsset.instance.PassengersHigh;
        ulong changedLow = 1, changedHigh = 0;
        MyAsset.instance.PassengersLow = 1;
        MyAsset.instance.PassengersHigh = 0;
        
        for(int i = 0; i < lineManager.lineCollections.Length; i++)
        {
            // 열차 구매 승객 수 재계산
            int amount = lineManager.lineCollections[i].lineData.numOfTrain;
            ulong initialTerm = lineManager.lineCollections[i].purchaseTrain.priceData.TrainPassenger;
            ulong commonDiff = lineManager.lineCollections[i].purchaseTrain.priceData.TrainPassengerAdd;
            ulong result = GetSumOfIncreasing((ulong)amount, initialTerm, commonDiff);
            TouchMoneyManager.ArithmeticOperation(result, 0, true);
            changedLow += result;

            if (!lineManager.lineCollections[i].purchaseTrain.priceData.IsLightRail)
            {
                // 열차확장 승객 수 재계산
                int expandAmount = lineManager.lineCollections[i].lineData.trainExpandStatus[1] + lineManager.lineCollections[i].lineData.trainExpandStatus[2] * 2 + lineManager.lineCollections[i].lineData.trainExpandStatus[3] * 3;
                result = (ulong)expandAmount * lineManager.lineCollections[i].expandTrain.priceData.TrainExapndPassenger;
                TouchMoneyManager.ArithmeticOperation(result, 0, true);
                changedLow += result;
            }

            MoneyUnitTranslator.Arrange(ref changedLow, ref changedHigh);
        }

        string priorUnitLow = "", priorUnitHigh = "", changedUnitLow = "", changedUnitHigh = "";
        PlayManager.ArrangeUnit(priorLow, priorHigh, ref priorUnitLow, ref priorUnitHigh);
        PlayManager.ArrangeUnit(changedLow, changedHigh, ref changedUnitLow, ref changedUnitHigh);
        log += string.Format("승객 수 변화: {0}{1}명 → <color=lime>{2}{3}명</color>\n", priorUnitHigh, priorUnitLow, changedUnitHigh, changedUnitLow);
    }

    /// <summary>
    /// 등차수열의 합을 얻는 함수
    /// </summary>
    /// <param name="amount">개수</param>
    /// <param name="initialTerm">첫째 항</param>
    /// <param name="commonDifference">공차</param>
    private ulong GetSumOfIncreasing(ulong amount, ulong initialTerm, ulong commonDifference)
    {
        return amount * (2 * initialTerm + (amount - 1) * commonDifference) / 2;
    }

    private void RevisePassengerLimit()
    {
        ulong priorLimitLow = MyAsset.instance.PassengersLimitLow, priorLimitHigh = MyAsset.instance.PassengersLimitHigh;
        MyAsset.instance.PassengersLimitLow = 100;
        MyAsset.instance.PassengersLimitHigh = 0;
        driversManager.RenewPrice();

        for (int i = 0; i < driversManager.numOfDrivers.Length; i++)
        {
            ulong lowUnit = GetSumOfIncreasing((ulong)driversManager.numOfDrivers[i], driversManager.standardPassenger[i], driversManager.passenger_UP[i]);
            ulong highUnit = 0;
            MoneyUnitTranslator.Arrange(ref lowUnit, ref highUnit);
            MyAsset.instance.PassengersLimitLow += lowUnit;
            MyAsset.instance.PassengersLimitHigh += highUnit;
        }

        string priorLowUnit = "", priorHighUnit = "", changedLowUnit = "", changedHighUnit = "";
        PlayManager.ArrangeUnit(priorLimitLow, priorLimitHigh, ref priorLowUnit, ref priorHighUnit, true);
        PlayManager.ArrangeUnit(MyAsset.instance.PassengersLimitLow, MyAsset.instance.PassengersLimitHigh, ref changedLowUnit, ref changedHighUnit, true);
        log += string.Format("승객 수 제한: {0}{1}명 → <color=lime>{2}{3}명</color>\n", priorHighUnit, priorLowUnit, changedHighUnit, changedLowUnit);
    }

    private void ReviseTimeMoney()
    {
        MyAsset.instance.TimePerEarningLow = 0;
        MyAsset.instance.TimePerEarningHigh = 0;
        for (int i = 0; i < rentManager.rentData.numOfFacilities.Length; i++)
            rentManager.rentData.numOfFacilities[i] = 0;
        log += "시간형 수익(초당 수익) 및 시설 임대 관련 데이터가 초기화 되었습니다.\n";
    }

    private void ReviseLineConnectReward()
    {
        ulong increasedTMLow = 0, increasedTMHigh = 0;
        for(int i = 0; i < lineManager.lineCollections.Length; i++)
        {
            for(int sec = 0; sec < lineManager.lineCollections[i].lineData.connected.Length; sec++)
            {
                if(lineManager.lineCollections[i].lineData.connected[sec])
                {
                    if (lineManager.lineCollections[i].lineConnection.priceData.ConnectTMLargeUnit)
                    {
                        MyAsset.instance.TimeEarningOperator(0, lineManager.lineCollections[i].lineConnection.priceData.ConnectTimeMoney[sec], true);
                        increasedTMHigh += lineManager.lineCollections[i].lineConnection.priceData.ConnectTimeMoney[sec];
                    }
                    else
                    {
                        bool res = MyAsset.instance.TimeEarningOperator(lineManager.lineCollections[i].lineConnection.priceData.ConnectTimeMoney[sec], 0, true);
                        increasedTMLow += lineManager.lineCollections[i].lineConnection.priceData.ConnectTimeMoney[sec];
                    }
                }
            }
        }
        MoneyUnitTranslator.Arrange(ref increasedTMLow, ref increasedTMHigh);
        string lowUnit = "", highUnit = "";
        PlayManager.ArrangeUnit(increasedTMLow, increasedTMHigh, ref lowUnit, ref highUnit);
        log += "노선 연결의 보상이 승객 수 → 시간형 수익으로 전환된 금액: <color=lime>" + highUnit + lowUnit + "$</color>\n";
    }
    private void ReviseCompanyReputation()
    {
        companyReputationManager.ReputationValue = 0;
        for (int i = 0; i < lineManager.lineCollections.Length; i++)
        {
            for (int section = 0; section < lineManager.lineCollections[i].lineData.installed.Length; section++)
            {
                if (lineManager.lineCollections[i].lineData.installed[section])
                    companyReputationManager.ReputationValue += lineManager.lineCollections[i].setupScreenDoor.priceData.ScreenDoorReputation[section];
            }
        }
        log += string.Format("고객 만족도: {0:#,##0}, 수익 변화율: {1}%\n", companyReputationManager.ReputationValue, "0");

    }

    private void ConvertNormal2LightRailData()
    {
        int[] targetIndex = { 14, 15, 19, 25 };
        for(int i = 0; i < targetIndex.Length; i++)
        {
            int ti = targetIndex[i];
            if (lineManager.lineCollections[ti].lineData.numOfTrain > 100)
                lineManager.lineCollections[ti].lineData.numOfTrain = 25;
            else
                lineManager.lineCollections[ti].lineData.numOfTrain = lineManager.lineCollections[ti].lineData.numOfTrain / 4;

            if (lineManager.lineCollections[ti].lineData.numOfBase > 1)
                lineManager.lineCollections[ti].lineData.numOfBase = 1;

            if (lineManager.lineCollections[ti].lineData.numOfBaseEx > 3 || lineManager.lineCollections[ti].lineData.numOfBase > 2)
                lineManager.lineCollections[ti].lineData.numOfBaseEx = 3;

            lineManager.lineCollections[ti].lineData.limitTrain = lineManager.lineCollections[ti].lineData.numOfBase * 10 + lineManager.lineCollections[ti].lineData.numOfBaseEx * 5;

            int expandAmount = lineManager.lineCollections[ti].lineData.trainExpandStatus[1] + lineManager.lineCollections[ti].lineData.trainExpandStatus[2] * 2 + lineManager.lineCollections[ti].lineData.trainExpandStatus[3] * 3;
            expandAmount /= 12;
            lineManager.lineCollections[ti].lineData.lineControlLevels = new int[5];
            while (expandAmount > 0)
            {
                for (int k = 0; k < lineManager.lineCollections[ti].lineData.lineControlLevels.Length; k++)
                {
                    lineManager.lineCollections[ti].lineData.lineControlLevels[k]++;
                    expandAmount--;
                    if (expandAmount <= 0)
                        break;
                }
            }
            lineManager.lineCollections[ti].lineData.trainExpandStatus = new int[0];
        }    
    }

    private void ApplyLineUpgrade()
    {
        int[] targetIndex = { 14, 15, 19, 25 };
        for (int i = 0; i < targetIndex.Length; i++)
        {
            for(int product = 0; product < 5; product++)
            {
                for(int lev = 0; lev < lightRailControlManager.GetLineControlLevel(product); lev++)
                {
                    LargeVariable passenger = lightRailControlManager.GetPassenger(product, lev);
                    TouchMoneyManager.ArithmeticOperation(passenger.lowUnit, passenger.highUnit, true);
                }
            }
        }
        companyReputationManager.RenewPassengerBase();
    }

    private void ResetRent()
    {
        MyAsset.instance.TimePerEarning = LargeVariable.zero;
        for(int i = 0; i < rentManager.NumOfFacilities.Length; i++)
        {
            rentManager.CumulatedFacilityTimeMoney[i] = 0;
            for (int j = 0; j < rentManager.NumOfFacilities[i]; j++)
            {
                ulong timeMoney = rentManager.GetTimeMoney(i, j);
                rentManager.CumulatedFacilityTimeMoney[i] += timeMoney;
                MyAsset.instance.TimeEarningOperator(timeMoney, 0, true);
            }
        }
    }

    private void ReviseAfter3_1_4()
    {
        ConvertNormal2LightRailData();

        RevisePassenger();
        ApplyLineUpgrade();
        ResetRent();
        ReviseLineConnectReward();

        reviseMessage.SetActive(true);
    }

    public void CloseReviseMessage()
    {
        reviseMessage.SetActive(false);
    }
}
