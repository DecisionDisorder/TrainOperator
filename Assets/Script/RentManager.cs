using UnityEngine;
using UnityEngine.UI;

public class RentManager : MonoBehaviour
{
    public FacilityData[] facilityDatas;

    public RentData rentData;

    public int[] NumOfFacilities
    {
        get { return rentData.numOfFacilities; }
        set { rentData.numOfFacilities = value; }
    }

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

    public Text[] facilityPriceTexts;
    public Text[] facilityTimeMoneyTexts;
    public Text[] conditionTexts;

    public LineManager lineManager;
    public MessageManager messageManager;
    public UpdateDisplay rentUpdateDisplay;
    public ButtonColor_Controller3 buttonColor_Controller3;

    private void Start()
    {
        SetTexts();
        rentUpdateDisplay.onEnable += SetTexts;
        rentUpdateDisplay.onEnableUpdate += buttonColor_Controller3.SetRent;
    }

    public void PurchaseFacility(int item)
    {
        if (NumOfFacilities[item] < GetMaxLimit(item))
        {
            if (NumOfFacilities[item] < GetTotalLimit(item))
            {
                if (AssetMoneyCalculator.instance.ArithmeticOperation(facilityDatas[item].priceLow, facilityDatas[item].priceHigh, false))
                {
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
    /// 해당 시설의 시간형 수익 보상을 구함
    /// </summary>
    private ulong GetTimeMoney(int index)
    {
        int line = GetLine(index);
        return facilityDatas[index].standardTimeMoney[line] + facilityDatas[index].incrementTimeMoney[line] * ((ulong)GetAmountPerLine(index) - 1);
    }

    /// <summary>
    /// 해당 인덱스의 시설이 어느 노선에 해당되는지 리턴함
    /// </summary>
    private int GetLine(int index)
    {
        int line = NumOfFacilities[index] / facilityDatas[index].limit;
        if (line > (int)Line.Gyeonggang)
            return (int)Line.Gyeonggang;
        else
            return line;
    }

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
        return facilityDatas[index].limit * ((int)lineManager.GetRecentlyOpenedLine() + 1);
    }

    /// <summary>
    /// 모든 노선을 확장했을 때의 최대 제한 수치
    /// </summary>
    public int GetMaxLimit(int index)
    {
        return facilityDatas[index].limit * ((int)Line.Gyeonggang + 1);
    }

    /// <summary>
    /// 노선마다의 구매 횟수를 구함
    /// </summary>
    private int GetAmountPerLine(int index)
    {
        int amount = (NumOfFacilities[index] + 1) % facilityDatas[index].limit;

        if (amount.Equals(0))
            return facilityDatas[index].limit;
        else
            return amount;
    }


    private void LackOfSpace()
    {
        messageManager.ShowMessage("해당 시설을 위한 공간이 가득찼습니다.\n새로운 노선의 확장권을 구입해주세요.");
    }

    private void FacilityMax()
    {
        messageManager.ShowMessage("해당 시설의 설치를 모두 완료하였습니다.");
    }
}

[System.Serializable]
public class FacilityData
{
    public string name;
    public int limit;
    public ulong priceLow;
    public ulong priceHigh;
    public ulong[] standardTimeMoney;
    public ulong[] incrementTimeMoney;
    public bool isTMLargeUnit;
}
