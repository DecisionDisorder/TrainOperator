using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 노선 열거형
/// </summary>
public enum Line { Line1, Line2, Line3, Line4, UJB, Line5, Line6, Line7, Line8, Line9, GimpoGold, Busan1, Busan2, Busan3, Busan4, BusanKH, BusanDH, Daegu1, Daegu2, Daegu3,
    ULRT, SinBundang, SuinBundang, Everline, Incheon1, Incheon2, GyeonguiJungang, Gyeongchun, Gyeonggang, Sillim, Airline, Seohae, Gwangju1, Daejeon1 }

/// <summary>
/// 경전철 노선 열거형
/// </summary>
public enum LightRailLine { UJB, GimpoGold, Busan4, BusanKH, Daegu3, ULRT, Everline, Incheon2, Sillim }

/// <summary>
/// 노선을 총괄하여 관리 하는 클래스
/// </summary>
public class LineManager : MonoBehaviour
{
    /// <summary>
    /// 노선 관리자에 대한 싱글톤 인스턴스
    /// </summary>
    public static LineManager instance;
    /// <summary>
    /// 모든 노선의 관리 클래스를 배열로 정리
    /// </summary>
    public LineCollection[] lineCollections;
    /// <summary>
    /// 현재 작업중인 노선
    /// </summary>
    public Line? currentLine = null;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 역 구매 확인 처리
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
    /// 역 구매 취소 처리
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
    /// 개별 역 구매 확인 처리
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
    /// 개별 역 구매 취소 처리
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
    /// 노선 연결 확인 처리
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
    /// 노선 연결 취소 처리
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
    /// 스크린도어 설치 확인 처리
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
    /// 스크린도어 설치 취소 처리
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
    /// 전체 열차 확장 선택
    /// </summary>
    public void ExpandTrainAmountAll()
    {
        if(currentLine != null)
        {
            lineCollections[(int)currentLine].expandTrain.SetAmountAll();
        }
    }

    /// <summary>
    /// 열차 확장 수량 선택
    /// </summary>
    /// <param name="amount">확장 수량</param>
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
    /// 열차 확장 확인 처리
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
    /// 열차 확장 취소 처리
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
    /// 가장 최근에 노선 확장권을 얻은 최상위 노선
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
    /// 일반 노선 중 확장권을 보유한 노선의 개수 계산
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
    /// 경전철 노선 중 확장권을 보유한 노선의 개수 계산
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
    /// 일반 노선의 개수 계산
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
    /// 경전철 노선의 개수 계산
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