using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 임대 시스템 관리 클래스
/// </summary>
public class RentManager : MonoBehaviour
{
    /// <summary>
    /// 임대 시설 프리셋 데이터
    /// </summary>
    public FacilityData[] facilityDatas;

    /// <summary>
    /// 임대 시설 관련 저장 데이터 오브젝트
    /// </summary>
    public RentData rentData;

    /// <summary>
    /// 시설 별로 보유 중인 임대 시설 개수
    /// </summary>
    public int[] NumOfFacilities
    {
        get { return rentData.numOfFacilities; }
        set { rentData.numOfFacilities = value; }
    }

    /// <summary>
    /// 시설 별로 획득한 누적 시간형 수익
    /// </summary>
    public ulong[] CumulatedFacilityTimeMoney
    {
        get
        {
            if (rentData.cumulatedFacilityTimeMoney == null) rentData.cumulatedFacilityTimeMoney = new ulong[4];
            return rentData.cumulatedFacilityTimeMoney;
        }
        set
        {
            rentData.cumulatedFacilityTimeMoney = value;
        }
    }
    /// <summary>
    /// 시설 별 최대 보유량
    /// </summary>
    public int[] maxFacilityAmount;

    /// <summary>
    /// 시설 별 가격 정보 텍스트 배열
    /// </summary>
    public Text[] facilityPriceTexts;
    /// <summary>
    /// 시설 별 시간형 수익 보상 정보 텍스트 배열
    /// </summary>
    public Text[] facilityTimeMoneyTexts;
    /// <summary>
    /// 보유 중인 시설 정보 텍스트 배열
    /// </summary>
    public Text[] conditionTexts;

    public LineManager lineManager;
    public MessageManager messageManager;
    public UpdateDisplay rentUpdateDisplay;
    public ButtonColor_Controller3 buttonColor_Controller3;

    /// <summary>
    /// 일반 노선, 경전철 노선이 열린 개수 (0번: 일반, 1번: 경전철)
    /// </summary>
    private int[] openedAmount = new int[2];
    /// <summary>
    /// 일반 노선, 경전철 노선 총 개수 (0번: 일반, 1번: 경전철)
    /// </summary>
    private int[] lineAmount = new int[2];

    private void Start()
    {
        SetTexts();
        rentUpdateDisplay.onEnable += SetLineAmount;
        rentUpdateDisplay.onEnable += SetTexts;
        rentUpdateDisplay.onEnableUpdate += buttonColor_Controller3.SetRent;
    }

    /// <summary>
    /// 임대 시설 구매 처리
    /// </summary>
    /// <param name="item">시설 인덱스</param>
    public void PurchaseFacility(int item)
    {
        // 시설별 개수 최대치 확인
        if (NumOfFacilities[item] < GetMaxLimit(item))
        {
            // 총 개수 제한 수량 확인
            if (NumOfFacilities[item] < GetTotalLimit(item))
            {
                // 비용 지불 처리
                if (AssetMoneyCalculator.instance.ArithmeticOperation(facilityDatas[item].priceLow, facilityDatas[item].priceHigh, false))
                {
                    // 시간형 수익 보상 지급 등 수치 적용
                    if (facilityDatas[item].isTMLargeUnit)
                        MyAsset.instance.TimeEarningOperator(0, GetTimeMoney(item), true);
                    else
                        MyAsset.instance.TimeEarningOperator(GetTimeMoney(item), 0, true);
                    CumulatedFacilityTimeMoney[item] += GetTimeMoney(item);
                    NumOfFacilities[item]++;
                    SetTexts();
                }
                else
                    PlayManager.instance.LackOfMoney();
            }
            else
                LackOfSpace();
        }
        else
            FacilityMax();
    }

    /// <summary>
    /// 각 임대 시설 별 텍스트 정보 업데이트
    /// </summary>
    private void SetTexts()
    {
        string unitLow = "", unitHigh = "";
        for(int i = 0; i < facilityPriceTexts.Length; i++)
        {
            if (NumOfFacilities[i] < GetMaxLimit(i))
            {
                SetPrice(i);
                PlayManager.ArrangeUnit(facilityDatas[i].priceLow, facilityDatas[i].priceHigh, ref unitLow, ref unitHigh, true);
                facilityPriceTexts[i].text = "비용: " + unitHigh + unitLow + "$";

                if (facilityDatas[i].isTMLargeUnit)
                    PlayManager.ArrangeUnit(0, GetTimeMoney(i), ref unitLow, ref unitHigh, true);
                else
                    PlayManager.ArrangeUnit(GetTimeMoney(i), 0, ref unitLow, ref unitHigh, true);
                facilityTimeMoneyTexts[i].alignment = TextAnchor.MiddleRight;
                facilityTimeMoneyTexts[i].text = "초당 +" + unitHigh + unitLow + "$";
            }
            else
            {
                facilityPriceTexts[i].text = "";
                facilityTimeMoneyTexts[i].alignment = TextAnchor.MiddleCenter;
                facilityTimeMoneyTexts[i].text = "설치 완료";
            }
        }
        conditionTexts[0].text = facilityDatas[0].name + ": " + NumOfFacilities[0] + "/" + GetTotalLimit(0) + "\n" + facilityDatas[2].name + ": " + NumOfFacilities[2] + "/" + GetTotalLimit(2);
        conditionTexts[1].text = facilityDatas[1].name + ": " + NumOfFacilities[1] + "/" + GetTotalLimit(1) + "\n" + facilityDatas[3].name + ": " + NumOfFacilities[3] + "/" + GetTotalLimit(3);
        
        buttonColor_Controller3.SetRent();
    }

    /// <summary>
    /// 임대 시설의 구매 비용 업데이트
    /// </summary>
    /// <param name="index">임대 시설 인덱스</param>
    private void SetPrice(int index)
    {
        PriceData priceData = lineManager.lineCollections[GetLine(index)].purchaseTrain.priceData;
        int x = GetAmountPerLine(index) * 100 / facilityDatas[index].limit;
        ulong difficulty = (ulong)(priceData.TrainPriceFactors[0] * x * x + priceData.TrainPriceFactors[1] * x + priceData.TrainPriceFactors[2]);
        ulong cumulTM = CumulatedFacilityTimeMoney[index] + GetTimeMoney(index);

        // 난이도 계수와 누적 시간형 수익의 곱이 1경이 넘지 않을 때는 단순 곱 계산, 넘으면 high 단위만 고려하여 누적 시간형 수익을 1경으로 나누어 소수점으로 만들고 high 기준 가격 계산
        if (PlayManager.CheckLowUnitRange(difficulty, cumulTM))
        {
            facilityDatas[index].priceLow = difficulty * cumulTM;
            facilityDatas[index].priceHigh = 0;
        }
        else
        {
            facilityDatas[index].priceLow = 0;
            facilityDatas[index].priceHigh = (ulong)(difficulty * ((float)cumulTM / AssetMoneyCalculator.standard_maximum));
        }
    }

    /// <summary>
    /// 노선 개수 정보 업데이트
    /// </summary>
    private void SetLineAmount()
    {
        openedAmount[0] = lineManager.GetOpenedNormalLineAmount();
        openedAmount[1] = lineManager.GetOpenedLightRailAmount();

        lineAmount[0] = lineManager.GetNormalLineAmount();
        lineAmount[1] = lineManager.GetLightRailAmount();
    }

    /// <summary>
    /// 해당 시설의 시간형 수익 보상을 구함
    /// </summary>
    private ulong GetTimeMoney(int index)
    {
        int line = GetLine(index);
        return facilityDatas[index].standardTimeMoney[line] + facilityDatas[index].incrementTimeMoney[line] * ((ulong)GetAmountPerLine(index) - 1);
    }
    /// <summary>
    /// 해당 시설의 시간형 수익 보상을 구함
    /// </summary>
    public ulong GetTimeMoney(int index, int num)
    {
        int line = GetLine(index, num);
        return facilityDatas[index].standardTimeMoney[line] + facilityDatas[index].incrementTimeMoney[line] * ((ulong)GetAmountPerLine(index, num) - 1);
    }

    /// <summary>
    /// 해당 인덱스의 시설이 어느 노선에 해당되는지 리턴함
    /// </summary>
    private int GetLine(int index)
    {
        return GetLine(index, NumOfFacilities[index]);
    }
    /// <summary>
    /// 해당 인덱스의 시설이 어느 노선에 해당되는지 리턴함
    /// </summary>
    private int GetLine(int index, int amount)
    {
        int numOfFacility = amount, line;
        for (line = 0; line < lineManager.lineCollections.Length; line++)
        {
            if (lineManager.lineCollections[line].lineConnection.priceData.IsLightRail)
            {
                if (numOfFacility < facilityDatas[index].lightRailLimit)
                    break;
                else
                    numOfFacility -= facilityDatas[index].lightRailLimit;
            }
            else
            {
                if (numOfFacility < facilityDatas[index].limit)
                    break;
                else
                    numOfFacility -= facilityDatas[index].limit;
            }
        }
        return line;
    }

    /// <summary>
    /// 제한 수량 확인
    /// </summary>
    /// <param name="index">시설 인덱스</param>
    /// <returns>구매 가능 여부</returns>
    public bool CheckLimit(int index)
    {
        if (NumOfFacilities[index] < GetTotalLimit(index) && NumOfFacilities[index] < GetMaxLimit(index))
            return true;
        else
            return false;
    }

    /// <summary>
    /// 현재까지 노선 확장권을 보유한 것을 고려한 제한 수치를 구함
    /// </summary>
    public int GetTotalLimit(int index)
    {
        return facilityDatas[index].limit * openedAmount[0] + facilityDatas[index].lightRailLimit * openedAmount[1];
    }

    /// <summary>
    /// 모든 노선을 확장했을 때의 최대 제한 수치
    /// </summary>
    public int GetMaxLimit(int index)
    {
        return facilityDatas[index].limit * lineAmount[0] + facilityDatas[index].lightRailLimit * lineAmount[1];
    }

    /// <summary>
    /// 노선마다의 구매 횟수를 구함
    /// </summary>
    private int GetAmountPerLine(int index)
    {
        return GetAmountPerLine(index, NumOfFacilities[index]);
    }
    /// <summary>
    /// 노선마다의 구매 횟수를 구함
    /// </summary>
    private int GetAmountPerLine(int index, int num)
    {
        int amountLeft = num, line;
        for (line = 0; line < lineManager.lineCollections.Length; line++)
        {
            if (lineManager.lineCollections[line].lineConnection.priceData.IsLightRail)
            {
                if (amountLeft < facilityDatas[index].lightRailLimit)
                    break;
                else
                    amountLeft -= facilityDatas[index].lightRailLimit;
            }
            else
            {
                if (amountLeft < facilityDatas[index].limit)
                    break;
                else
                    amountLeft -= facilityDatas[index].limit;
            }
        }

        return amountLeft + 1;
    }


    /// <summary>
    /// 임대 시설 공간 부족 안내 메시지
    /// </summary>
    private void LackOfSpace()
    {
        messageManager.ShowMessage("해당 시설을 위한 공간이 가득찼습니다.\n새로운 노선의 확장권을 구입해주세요.");
    }

    /// <summary>
    /// 임대 시설 구매 최대치 안내
    /// </summary>
    private void FacilityMax()
    {
        messageManager.ShowMessage("해당 시설의 설치를 모두 완료하였습니다.");
    }
}

/// <summary>
/// 임대시설 스펙 프리셋 데이터 클래스
/// </summary>
[System.Serializable]
public class FacilityData
{
    /// <summary>
    /// 시설 이름
    /// </summary>
    public string name;
    /// <summary>
    /// 일반 노선 1개당 제한 수량
    /// </summary>
    public int limit;
    /// <summary>
    /// 경전철 노선 1개당 제한 수량
    /// </summary>
    public int lightRailLimit;
    /// <summary>
    /// 1경 미만 단위의 가격
    /// </summary>
    public ulong priceLow;
    /// <summary>
    /// 1경 이상 단위의 가격
    /// </summary>
    public ulong priceHigh;
    /// <summary>
    /// 노선 마다의 시간형 수익 보상 기준
    /// </summary>
    public ulong[] standardTimeMoney;
    /// <summary>
    /// 노선 마다의 시간형 수익 보상 증가량
    /// </summary>
    public ulong[] incrementTimeMoney;
    /// <summary>
    /// 시간형 수익 보상이 1경 이상의 단위인지 여부
    /// </summary>
    public bool isTMLargeUnit;
}
