using UnityEngine;
using UnityEngine.UI;

public class ButtonColorManager : MonoBehaviour
{
    public Image buyTrainImg;
    public Image vehicleBaseImg;
    public Image vehicleBaseExImg;
    public Image[] expandImgs;
    public Image[] connectImgs;
    public Image[] screendoorImgs;

    public Color lineColor;

    public LineCollection lineCollection;
    public PriceData priceData;

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

    public void SetVehicleBaseColor()
    {
        if (IsEnoughMoney(priceData.GetVehicleBasePrice(lineCollection.lineData.numOfBase)) && IsExpandedAtLeast())
            vehicleBaseImg.color = lineColor;
        else
            vehicleBaseImg.color = Color.gray;

        if (IsEnoughMoney(priceData.GetVehicleBaseExPrice(lineCollection.lineData.numOfBaseEx)) && IsExpandedAtLeast())
            vehicleBaseExImg.color = lineColor;
        else
            vehicleBaseExImg.color = Color.gray;
    }

    private bool IsExpandedAtLeast()
    {
        for(int i = 0; i < lineCollection.lineData.sectionExpanded.Length; i++)
        {
            if (lineCollection.lineData.sectionExpanded[i])
                return true;
        }
        
        return false;
    }
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

    public void SetColorExpand()
    {
        lineCollection.expandPurchase.SetQualification();
        for(int i = 0; i < expandImgs.Length; i++)
        {
            SetColorExpand(i);
        }
    }
    public void SetColorExpand(int section)
    {
        if (lineCollection.lineData.sectionExpanded[section])
            expandImgs[section].color = lineColor;
        else if (IsEnoughMoney(priceData.ConnectPrice[section]))
            expandImgs[section].color = Color.white;
        else
            expandImgs[section].color = Color.gray;
    }

    public void SetColorConnection()
    {
        for (int i = 0; i < connectImgs.Length; i++)
        {
            SetColorConnection(i);
        }
    }
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

    public void SetColorScreenDoor()
    {
        for(int i = 0; i < screendoorImgs.Length; i++)
        {
            SetColorScreenDoor(i);
        }
    }
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
