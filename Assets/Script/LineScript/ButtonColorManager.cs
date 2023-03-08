using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �뼱 ������ ���� ��ư ���� ���� Ŭ����(�뼱�� �Ҵ�)
/// </summary>
public class ButtonColorManager : MonoBehaviour
{
    /// <summary>
    /// ���� ���� ��ư �̹���
    /// </summary>
    public Image buyTrainImg;
    /// <summary>
    /// �������� ���� ��ư �̹���
    /// </summary>
    public Image vehicleBaseImg;
    /// <summary>
    /// �������� Ȯ�� ��ư �̹���
    /// </summary>
    public Image vehicleBaseExImg;
    /// <summary>
    /// ���� �� Ȯ��� ���� ��ư �̹���
    /// </summary>
    public Image[] expandImgs;
    /// <summary>
    /// ���� �� �뼱 ���� ���� ��ư �̹���
    /// </summary>
    public Image[] connectImgs;
    /// <summary>
    /// ���� �� ��ũ������ ���� ��ư �̹���
    /// </summary>
    public Image[] screendoorImgs;

    /// <summary>
    /// �뼱 ����
    /// </summary>
    public Color lineColor;

    public LineCollection lineCollection;
    public PriceData priceData;

    /// <summary>
    /// Ư�� �޴��� Ȱ��ȭ �Ǿ��� �� ������ ������Ʈ�� �����ִ� �ν��Ͻ�
    /// </summary>
    public UpdateDisplay buyTrainUpdateDisplay;
    public UpdateDisplay vehicleBaseUpdateDiaplay;
    public UpdateDisplay expandUpdateDisplay;
    public UpdateDisplay connectionUpdateDisplay;
    public UpdateDisplay screenDoorUpdateDisplay;

    private void Start()
    {
        buyTrainUpdateDisplay.onEnableUpdate += SetTrainColor;
        vehicleBaseUpdateDiaplay.onEnableUpdate += SetVehicleBaseColor;
        expandUpdateDisplay.onEnableUpdate += SetColorExpand;
        connectionUpdateDisplay.onEnableUpdate += SetColorConnection;
        screenDoorUpdateDisplay.onEnableUpdate += SetColorScreenDoor;
    }

    /// <summary>
    /// ���� ���� ���� ���ο� ���� ���� ����
    /// </summary>
    public void SetTrainColor()
    {
        LargeVariable price = LargeVariable.zero;
        priceData.GetTrainPrice(lineCollection.lineData.numOfTrain, ref price);
        if (MyAsset.instance.Money >= price && lineCollection.lineData.numOfTrain < lineCollection.lineData.limitTrain)
        {
            if(TouchMoneyManager.CheckLimitValid(priceData.GetTrainPassenger(lineCollection.lineData.numOfTrain), 0))
            {
                buyTrainImg.color = lineColor;
                return;
            }
        }

        buyTrainImg.color = Color.gray;
    }

    /// <summary>
    /// �������� ����/Ȯ�� ���� ���ο� ���� ���� ����
    /// </summary>
    public void SetVehicleBaseColor()
    {
        if (IsEnoughMoney(priceData.GetVehicleBasePrice(lineCollection.lineData.numOfBase)) && IsExpandedAtLeast() && lineCollection.lineData.numOfBase < lineCollection.purchaseTrain.BaseLimit)
            vehicleBaseImg.color = lineColor;
        else
            vehicleBaseImg.color = Color.gray;

        if (IsEnoughMoney(priceData.GetVehicleBaseExPrice(lineCollection.lineData.numOfBaseEx)) && IsExpandedAtLeast() && lineCollection.lineData.numOfBaseEx < lineCollection.lineData.numOfBase * 3)
            vehicleBaseExImg.color = lineColor;
        else
            vehicleBaseExImg.color = Color.gray;
    }

    /// <summary>
    /// �ش� �뼱�� �뼱 Ȯ��ǵ� �߿� �ϳ��� �����ߴ��� ���� Ȯ��
    /// </summary>
    private bool IsExpandedAtLeast()
    {
        for(int i = 0; i < lineCollection.lineData.sectionExpanded.Length; i++)
        {
            if (lineCollection.lineData.sectionExpanded[i])
                return true;
        }
        
        return false;
    }

    /// <summary>
    /// Ư�� ���ݿ� ���� ����� ���� ��������
    /// </summary>
    /// <param name="price">����</param>
    /// <returns>���� ���� ����</returns>
    private bool IsEnoughMoney(ulong price)
    {
        if (priceData.IsLargeUnit)
        {
            if (MyAsset.instance.MoneyHigh < price)
                return false;
            else
                return true;
        }
        else
        {
            if (MyAsset.instance.MoneyHigh > 0)
                return true;
            else if (MyAsset.instance.MoneyLow < price)
                return false;
            else
                return true;
        }
    }

    /// <summary>
    /// ��� ������ ���� Ȯ��� ���� ���� ���ο� ���� ���� ����
    /// </summary>
    public void SetColorExpand()
    {
        lineCollection.expandPurchase.SetQualification();
        for(int i = 0; i < expandImgs.Length; i++)
        {
            SetColorExpand(i);
        }
    }
    /// <summary>
    /// Ư�� ������ Ȯ��� ���� ���� ���ο� ���� ���� ����
    /// </summary>
    /// <param name="section">������ ������ ���� ��ȣ</param>
    public void SetColorExpand(int section)
    {
        if (lineCollection.lineData.sectionExpanded[section])
            expandImgs[section].color = lineColor;
        else if (IsEnoughMoney(priceData.ConnectPrice[section]))
            expandImgs[section].color = Color.white;
        else
            expandImgs[section].color = Color.gray;
    }

    /// <summary>
    /// ��� ������ ���� �뼱 ���� ���� ���ο� ���� ���� ����
    /// </summary>
    public void SetColorConnection()
    {
        for (int i = 0; i < connectImgs.Length; i++)
        {
            SetColorConnection(i);
        }
    }
    /// <summary>
    /// Ư�� ������ �뼱 ���� ���� ���ο� ���� ���� ����
    /// </summary>
    /// <param name="section">������ ������ ���� ��ȣ</param>
    public void SetColorConnection(int section)
    {
        if (!lineCollection.lineData.hasAllStations[section])
            lineCollection.lineConnection.CheckHasAllStations();

        if (lineCollection.lineData.connected[section])
            connectImgs[section].color = lineColor;
        else if (lineCollection.lineData.hasAllStations[section] && IsEnoughMoney(priceData.ConnectPrice[section]))
            connectImgs[section].color = Color.white;
        else
            connectImgs[section].color = Color.gray;
    }
    /// <summary>
    /// ��� ������ ���� ��ũ������ ��ġ ���� ���ο� ���� ���� ����
    /// </summary>
    public void SetColorScreenDoor()
    {
        for(int i = 0; i < screendoorImgs.Length; i++)
        {
            SetColorScreenDoor(i);
        }
    }
    /// <summary>
    /// Ư�� ������ ��ũ������ ��ġ ���� ���ο� ���� ���� ����
    /// </summary>
    /// <param name="section">������ ������ ���� ��ȣ</param>
    public void SetColorScreenDoor(int section)
    {
        if(!lineCollection.lineData.hasAllStations[section])
            lineCollection.lineConnection.CheckHasAllStations();

        if (lineCollection.lineData.installed[section])
            screendoorImgs[section].color = lineColor;
        else if (lineCollection.lineData.hasAllStations[section] && IsEnoughMoney(priceData.ConnectPrice[section]))
            screendoorImgs[section].color = Color.white;
        else
            screendoorImgs[section].color = Color.gray;
    }
}
