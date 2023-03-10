using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �Ӵ� �ý��� ���� Ŭ����
/// </summary>
public class RentManager : MonoBehaviour
{
    /// <summary>
    /// �Ӵ� �ü� ������ ������
    /// </summary>
    public FacilityData[] facilityDatas;

    /// <summary>
    /// �Ӵ� �ü� ���� ���� ������ ������Ʈ
    /// </summary>
    public RentData rentData;

    /// <summary>
    /// �ü� ���� ���� ���� �Ӵ� �ü� ����
    /// </summary>
    public int[] NumOfFacilities
    {
        get { return rentData.numOfFacilities; }
        set { rentData.numOfFacilities = value; }
    }

    /// <summary>
    /// �ü� ���� ȹ���� ���� �ð��� ����
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
    /// �ü� �� �ִ� ������
    /// </summary>
    public int[] maxFacilityAmount;

    /// <summary>
    /// �ü� �� ���� ���� �ؽ�Ʈ �迭
    /// </summary>
    public Text[] facilityPriceTexts;
    /// <summary>
    /// �ü� �� �ð��� ���� ���� ���� �ؽ�Ʈ �迭
    /// </summary>
    public Text[] facilityTimeMoneyTexts;
    /// <summary>
    /// ���� ���� �ü� ���� �ؽ�Ʈ �迭
    /// </summary>
    public Text[] conditionTexts;

    public LineManager lineManager;
    public MessageManager messageManager;
    public UpdateDisplay rentUpdateDisplay;
    public ButtonColor_Controller3 buttonColor_Controller3;

    /// <summary>
    /// �Ϲ� �뼱, ����ö �뼱�� ���� ���� (0��: �Ϲ�, 1��: ����ö)
    /// </summary>
    private int[] openedAmount = new int[2];
    /// <summary>
    /// �Ϲ� �뼱, ����ö �뼱 �� ���� (0��: �Ϲ�, 1��: ����ö)
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
    /// �Ӵ� �ü� ���� ó��
    /// </summary>
    /// <param name="item">�ü� �ε���</param>
    public void PurchaseFacility(int item)
    {
        // �ü��� ���� �ִ�ġ Ȯ��
        if (NumOfFacilities[item] < GetMaxLimit(item))
        {
            // �� ���� ���� ���� Ȯ��
            if (NumOfFacilities[item] < GetTotalLimit(item))
            {
                // ��� ���� ó��
                if (AssetMoneyCalculator.instance.ArithmeticOperation(facilityDatas[item].priceLow, facilityDatas[item].priceHigh, false))
                {
                    // �ð��� ���� ���� ���� �� ��ġ ����
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
    /// �� �Ӵ� �ü� �� �ؽ�Ʈ ���� ������Ʈ
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

    /// <summary>
    /// �Ӵ� �ü��� ���� ��� ������Ʈ
    /// </summary>
    /// <param name="index">�Ӵ� �ü� �ε���</param>
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

    /// <summary>
    /// �뼱 ���� ���� ������Ʈ
    /// </summary>
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
    /// <summary>
    /// �ش� �ü��� �ð��� ���� ������ ����
    /// </summary>
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
    /// <summary>
    /// �ش� �ε����� �ü��� ��� �뼱�� �ش�Ǵ��� ������
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
    /// ���� ���� Ȯ��
    /// </summary>
    /// <param name="index">�ü� �ε���</param>
    /// <returns>���� ���� ����</returns>
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
    /// <summary>
    /// �뼱������ ���� Ƚ���� ����
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
    /// �Ӵ� �ü� ���� ���� �ȳ� �޽���
    /// </summary>
    private void LackOfSpace()
    {
        messageManager.ShowMessage("�ش� �ü��� ���� ������ ����á���ϴ�.\n���ο� �뼱�� Ȯ����� �������ּ���.");
    }

    /// <summary>
    /// �Ӵ� �ü� ���� �ִ�ġ �ȳ�
    /// </summary>
    private void FacilityMax()
    {
        messageManager.ShowMessage("�ش� �ü��� ��ġ�� ��� �Ϸ��Ͽ����ϴ�.");
    }
}

/// <summary>
/// �Ӵ�ü� ���� ������ ������ Ŭ����
/// </summary>
[System.Serializable]
public class FacilityData
{
    /// <summary>
    /// �ü� �̸�
    /// </summary>
    public string name;
    /// <summary>
    /// �Ϲ� �뼱 1���� ���� ����
    /// </summary>
    public int limit;
    /// <summary>
    /// ����ö �뼱 1���� ���� ����
    /// </summary>
    public int lightRailLimit;
    /// <summary>
    /// 1�� �̸� ������ ����
    /// </summary>
    public ulong priceLow;
    /// <summary>
    /// 1�� �̻� ������ ����
    /// </summary>
    public ulong priceHigh;
    /// <summary>
    /// �뼱 ������ �ð��� ���� ���� ����
    /// </summary>
    public ulong[] standardTimeMoney;
    /// <summary>
    /// �뼱 ������ �ð��� ���� ���� ������
    /// </summary>
    public ulong[] incrementTimeMoney;
    /// <summary>
    /// �ð��� ���� ������ 1�� �̻��� �������� ����
    /// </summary>
    public bool isTMLargeUnit;
}
