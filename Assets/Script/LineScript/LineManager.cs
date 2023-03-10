using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �뼱 ������
/// </summary>
public enum Line { Line1, Line2, Line3, Line4, UJB, Line5, Line6, Line7, Line8, Line9, GimpoGold, Busan1, Busan2, Busan3, Busan4, BusanKH, BusanDH, Daegu1, Daegu2, Daegu3,
    ULRT, SinBundang, SuinBundang, Everline, Incheon1, Incheon2, GyeonguiJungang, Gyeongchun, Gyeonggang, Sillim, Airline, Seohae, Gwangju1, Daejeon1 }

/// <summary>
/// ����ö �뼱 ������
/// </summary>
public enum LightRailLine { UJB, GimpoGold, Busan4, BusanKH, Daegu3, ULRT, Everline, Incheon2, Sillim }

/// <summary>
/// �뼱�� �Ѱ��Ͽ� ���� �ϴ� Ŭ����
/// </summary>
public class LineManager : MonoBehaviour
{
    /// <summary>
    /// �뼱 �����ڿ� ���� �̱��� �ν��Ͻ�
    /// </summary>
    public static LineManager instance;
    /// <summary>
    /// ��� �뼱�� ���� Ŭ������ �迭�� ����
    /// </summary>
    public LineCollection[] lineCollections;
    /// <summary>
    /// ���� �۾����� �뼱
    /// </summary>
    public Line? currentLine = null;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// �� ���� Ȯ�� ó��
    /// </summary>
    public void StationPurchaseConfirm()
    {
        if (currentLine != null)
        {
            lineCollections[(int)currentLine].purchaseStation.BuyStation();
            currentLine = null;
        }
    }
    /// <summary>
    /// �� ���� ��� ó��
    /// </summary>
    public void StationPurchaseCancel()
    {
        if(currentLine != null)
        {
            lineCollections[(int)currentLine].purchaseStation.CancelPurchase();
            currentLine = null;
        }
    }

    /// <summary>
    /// ���� �� ���� Ȯ�� ó��
    /// </summary>
    public void IDVStationPurchaseConfirm()
    {
        if(currentLine != null)
        {
            lineCollections[(int)currentLine].purchaseStation.IDVBuyStation();
            currentLine = null;
        }
    }

    /// <summary>
    /// ���� �� ���� ��� ó��
    /// </summary>
    public void IDVStationPUrchaseCancel()
    {
        if(currentLine != null)
        {
            lineCollections[(int)currentLine].purchaseStation.IDVCancelPurchase();
            currentLine = null;
        }
    }

    /// <summary>
    /// �뼱 ���� Ȯ�� ó��
    /// </summary>
    public void LineConnectConfirm()
    {
        if(currentLine != null)
        {
            lineCollections[(int)currentLine].lineConnection.ConnectLine();
            currentLine = null;
        }
    }
    /// <summary>
    /// �뼱 ���� ��� ó��
    /// </summary>
    public void LineConnectCancel()
    {
        if (currentLine != null)
        {
            lineCollections[(int)currentLine].lineConnection.CloseCheck();
            currentLine = null;
        }
    }

    /// <summary>
    /// ��ũ������ ��ġ Ȯ�� ó��
    /// </summary>
    public void ScreenDoorConfirm()
    {
        if(currentLine != null)
        {
            lineCollections[(int)currentLine].setupScreenDoor.ScreenDoorPurchase();
            currentLine = null;
        }
    }
    /// <summary>
    /// ��ũ������ ��ġ ��� ó��
    /// </summary>
    public void ScreenDoorCancel()
    {
        if (currentLine != null)
        {
            lineCollections[(int)currentLine].setupScreenDoor.CloseScreenDoor();
            currentLine = null;
        }
    }

    /// <summary>
    /// ��ü ���� Ȯ�� ����
    /// </summary>
    public void ExpandTrainAmountAll()
    {
        if(currentLine != null)
        {
            lineCollections[(int)currentLine].expandTrain.SetAmountAll();
        }
    }

    /// <summary>
    /// ���� Ȯ�� ���� ����
    /// </summary>
    /// <param name="amount">Ȯ�� ����</param>
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

    /// <summary>
    /// ���� Ȯ�� Ȯ�� ó��
    /// </summary>
    public void ExpandTrainConfirm()
    {
        if (currentLine != null)
        {
            lineCollections[(int)currentLine].expandTrain.ExpandTrainProcess(true);
            currentLine = null;
        }
    }
    /// <summary>
    /// ���� Ȯ�� ��� ó��
    /// </summary>
    public void ExpandTrainCancel()
    {
        if (currentLine != null)
        {
            lineCollections[(int)currentLine].expandTrain.ExpandTrainProcess(false);
            currentLine = null;
        }
    }
    /// <summary>
    /// ���� �ֱٿ� �뼱 Ȯ����� ���� �ֻ��� �뼱
    /// </summary>
    /// <returns></returns>
    public Line GetRecentlyOpenedLine()
    {
        for(int i = lineCollections.Length - 1; i >= 0; i--)
        {
            if (lineCollections[i].IsExpanded())
                return (Line)i;
        }

        return Line.Line1;
    }

    /// <summary>
    /// �Ϲ� �뼱 �� Ȯ����� ������ �뼱�� ���� ���
    /// </summary>
    public int GetOpenedNormalLineAmount()
    {
        int count = 0;
        for(int i = 0; i < lineCollections.Length; i++)
        {
            if (!lineCollections[i].purchaseStation.priceData.IsLightRail)
                if (lineCollections[i].IsExpanded())
                    count++;
        }
        return count;
    }
    /// <summary>
    /// ����ö �뼱 �� Ȯ����� ������ �뼱�� ���� ���
    /// </summary>
    /// <returns></returns>
    public int GetOpenedLightRailAmount()
    {
        int count = 0;
        for (int i = 0; i < lineCollections.Length; i++)
        {
            if (lineCollections[i].purchaseStation.priceData.IsLightRail)
                if (lineCollections[i].IsExpanded())
                    count++;
        }
        return count;
    }
    /// <summary>
    /// �Ϲ� �뼱�� ���� ���
    /// </summary>
    public int GetNormalLineAmount()
    {
        int count = 0;
        for (int i = 0; i < lineCollections.Length; i++)
        {
            if (!lineCollections[i].purchaseStation.priceData.IsLightRail)
                count++;
        }
        return count;
    }
    /// <summary>
    /// ����ö �뼱�� ���� ���
    /// </summary>
    public int GetLightRailAmount()
    {
        int count = 0;
        for (int i = 0; i < lineCollections.Length; i++)
        {
            if (lineCollections[i].purchaseStation.priceData.IsLightRail)
                count++;
        }
        return count;
    }
}