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

    private int[] openedAmount; // 0: normal line, 1: light rail
    private int[] lineAmount; // 0: normal line, 1: light rail

    private void Start()
    {
        SetTexts();
        rentUpdateDisplay.onEnable += SetTexts;
        rentUpdateDisplay.onEnableUpdate += buttonColor_Controller3.SetRent;
        rentUpdateDisplay.onEnable += SetLineAmount;
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
                facilityPriceTexts[i].text = "���: " + unitHigh + unitLow + "$";

                if (facilityDatas[i].isTMLargeUnit)
                    PlayManager.ArrangeUnit(0, GetTimeMoney(i), ref unitLow, ref unitHigh, true);
                else
                    PlayManager.ArrangeUnit(GetTimeMoney(i), 0, ref unitLow, ref unitHigh, true);
                facilityTimeMoneyTexts[i].alignment = TextAnchor.MiddleRight;
                facilityTimeMoneyTexts[i].text = "�ʴ� +" + unitHigh + unitLow + "$";
            }
            else
            {
                facilityPriceTexts[i].text = "";
                facilityTimeMoneyTexts[i].alignment = TextAnchor.MiddleCenter;
                facilityTimeMoneyTexts[i].text = "��ġ �Ϸ�";
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

        // ���̵� ����� ���� �ð��� ������ ���� 1���� ���� ���� ���� �ܼ� �� ���, ������ high ������ ����Ͽ� ���� �ð��� ������ 1������ ������ �Ҽ������� ����� high ���� ���� ���
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

    private void SetLineAmount()
    {
        openedAmount[0] = lineManager.GetOpenedNormalLineAmount();
        openedAmount[1] = lineManager.GetOpenedLightRailAmount();

        lineAmount[0] = lineManager.GetNormalLineAmount();
        lineAmount[1] = lineManager.GetLightRailAmount();
    }

    /// <summary>
    /// �ش� �ü��� �ð��� ���� ������ ����
    /// </summary>
    private ulong GetTimeMoney(int index)
    {
        int line = GetLine(index);
        return facilityDatas[index].standardTimeMoney[line] + facilityDatas[index].incrementTimeMoney[line] * ((ulong)GetAmountPerLine(index) - 1);
    }
    public ulong GetTimeMoney(int index, int num)
    {
        int line = GetLine(index, num);
        return facilityDatas[index].standardTimeMoney[line] + facilityDatas[index].incrementTimeMoney[line] * ((ulong)GetAmountPerLine(index, num) - 1);
    }

    /// <summary>
    /// �ش� �ε����� �ü��� ��� �뼱�� �ش�Ǵ��� ������
    /// </summary>
    private int GetLine(int index)
    {
        return GetLine(index, NumOfFacilities[index]);
    }
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

    public bool CheckLimit(int index)
    {
        if (NumOfFacilities[index] < GetTotalLimit(index) && NumOfFacilities[index] < GetMaxLimit(index))
            return true;
        else
            return false;
    }

    /// <summary>
    /// ������� �뼱 Ȯ����� ������ ���� ����� ���� ��ġ�� ����
    /// </summary>
    public int GetTotalLimit(int index)
    {
        return facilityDatas[index].limit * openedAmount[0] + facilityDatas[index].lightRailLimit * openedAmount[1];
    }

    /// <summary>
    /// ��� �뼱�� Ȯ������ ���� �ִ� ���� ��ġ
    /// </summary>
    public int GetMaxLimit(int index)
    {
        return facilityDatas[index].limit * lineAmount[0] + facilityDatas[index].lightRailLimit * lineAmount[1];
    }

    /// <summary>
    /// �뼱������ ���� Ƚ���� ����
    /// </summary>
    private int GetAmountPerLine(int index)
    {
        return GetAmountPerLine(index, NumOfFacilities[index]);
    }
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

        return amountLeft;
    }


    private void LackOfSpace()
    {
        messageManager.ShowMessage("�ش� �ü��� ���� ������ ����á���ϴ�.\n���ο� �뼱�� Ȯ����� �������ּ���.");
    }

    private void FacilityMax()
    {
        messageManager.ShowMessage("�ش� �ü��� ��ġ�� ��� �Ϸ��Ͽ����ϴ�.");
    }
}

[System.Serializable]
public class FacilityData
{
    public string name;
    public int limit;
    public int lightRailLimit;
    public ulong priceLow;
    public ulong priceHigh;
    public ulong[] standardTimeMoney;
    public ulong[] incrementTimeMoney;
    public bool isTMLargeUnit;
}
