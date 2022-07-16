using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Line { Line1, Line2, Line3, Line4, Line5, Line6, Line7, Line8, Line9, Busan1, Busan2, Busan3, Busan4, BusanKH, BusanDH, Daegu1, Daegu2, Daegu3, 
    Bundang, SinBundang, SuinBundang, Incheon1, Incheon2, GyeonguiJungang, Gyeongchun, Gyeonggang }

public class LineManager : MonoBehaviour
{
    public static LineManager instance;
    public LineCollection[] lineCollections;
    public Line? currentLine = null;

    private void Awake()
    {
        instance = this;
    }

    public void StationPurchaseConfirm()
    {
        if (currentLine != null)
        {
            lineCollections[(int)currentLine].purchaseStation.BuyStation();
            currentLine = null;
        }
    }
    public void StationPurchaseCancel()
    {
        if(currentLine != null)
        {
            lineCollections[(int)currentLine].purchaseStation.CancelPurchase();
            currentLine = null;
        }
    }

    public void IDVStationPurchaseConfirm()
    {
        if(currentLine != null)
        {
            lineCollections[(int)currentLine].purchaseStation.IDVBuyStation();
            currentLine = null;
        }
    }

    public void IDVStationPUrchaseCancel()
    {
        if(currentLine != null)
        {
            lineCollections[(int)currentLine].purchaseStation.IDVCancelPurchase();
            currentLine = null;
        }
    }

    public void LineConnectConfirm()
    {
        if(currentLine != null)
        {
            lineCollections[(int)currentLine].lineConnection.ConnectLine();
            currentLine = null;
        }
    }
    public void LineConnectCancel()
    {
        if (currentLine != null)
        {
            lineCollections[(int)currentLine].lineConnection.CloseCheck();
            currentLine = null;
        }
    }

    public void ScreenDoorConfirm()
    {
        if(currentLine != null)
        {
            lineCollections[(int)currentLine].setupScreenDoor.ScreenDoorPurchase();
            currentLine = null;
        }
    }
    public void ScreenDoorCancel()
    {
        if (currentLine != null)
        {
            lineCollections[(int)currentLine].setupScreenDoor.CloseScreenDoor();
            currentLine = null;
        }
    }

    public void ExpandTrainAmountAll()
    {
        if(currentLine != null)
        {
            lineCollections[(int)currentLine].expandTrain.SetAmountAll();
        }
    }

    public void ExpandTrainSetAmount(string amount)
    {
        if(currentLine != null)
        {
            int iAmount = int.Parse(amount);
            if (iAmount < 0)
                iAmount = 0;
            lineCollections[(int)currentLine].expandTrain.SetExpandAmount(iAmount);
        }
    }

    public void ExpandTrainConfirm()
    {
        if (currentLine != null)
        {
            lineCollections[(int)currentLine].expandTrain.ExpandTrainProcess(true);
            currentLine = null;
        }
    }
    public void ExpandTrainCancel()
    {
        if (currentLine != null)
        {
            lineCollections[(int)currentLine].expandTrain.ExpandTrainProcess(false);
            currentLine = null;
        }
    }

    public Line GetRecentlyOpenedLine()
    {
        for(int i = lineCollections.Length - 1; i >= 0; i--)
        {
            if (lineCollections[i].isExpanded())
                return (Line)i;
        }

        return Line.Line1;
    }
}