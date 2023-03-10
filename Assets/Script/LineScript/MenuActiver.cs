using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 메뉴 종류를 관리하는 열거형
/// </summary>
public enum MenuType { BuyTrain, HireDriver, VehicleBase, Station, LineExpand, LineConnect, TrainExpand, Screendoor, ManageSubway, Rent, AD, Bank, 
    ConditionVehicleBase, CompanyReputation, SubwayCondition, DriverCondition, RentCondition, StationValueCondition }

/// <summary>
/// 대분류상의 메뉴의 활성화/비활성화를 관리하는 클래스
/// </summary>
public class MenuActiver : MonoBehaviour
{
    /// <summary>
    /// 버튼 효과음
    /// </summary>
    public AudioSource buttonsound_source;

    public GameObject[] basicMenus;
    public GameObject[] buyTrainRegionMenus;
    public GameObject[] buyVehicleBaseRegionMenus;
    public GameObject[] buyStationRegionMenus;
    public GameObject[] buyExpandRegionMenus;
    public GameObject[] lineConnectRegionMenus;
    public GameObject[] trainExpandRegionMenus;
    public GameObject[] screendoorRegionMenus;

    /// <summary>
    /// 메뉴 활성화
    /// </summary>
    /// <param name="menuTypeSelect">활성화할 메뉴의 종류</param>
    public void OpenMenu(MenuTypeSelect menuTypeSelect)
    {
        basicMenus[(int)menuTypeSelect.menuType].SetActive(true);
        buttonsound_source.Play();
    }

    /// <summary>
    /// 메뉴 비활성화
    /// </summary>
    /// <param name="menuTypeSelect">비활성화할 메뉴의 종류</param>
    public void CloseMenu(MenuTypeSelect menuTypeSelect)
    {
        basicMenus[(int)menuTypeSelect.menuType].SetActive(false);
    }

    /// <summary>
    /// 열차 구매 메뉴 구역별 활성화
    /// </summary>
    /// <param name="regionIndex">활성화할 구역 번호</param>
    public void OpenBuyTrainMenu(int regionIndex)
    {
        buyTrainRegionMenus[regionIndex].SetActive(true);
    }
    /// <summary>
    /// 열차 구매 메뉴 구역별 비활성화
    /// </summary>
    /// <param name="regionIndex">비활성화할 구역 번호</param>
    public void CloseBuyTrainMenu(int regionIndex)
    {
        buyTrainRegionMenus[regionIndex].SetActive(false);
    }
    /// <summary>
    /// 차량기지 메뉴 구역별 활성화
    /// </summary>
    /// <param name="regionIndex">활성화할 구역 번호</param>
    public void OpenBuyVehicleBaseMenu(int regionIndex)
    {
        buyVehicleBaseRegionMenus[regionIndex].SetActive(true);
    }
    /// <summary>
    /// 차량기지 메뉴 구역별 비활성화
    /// </summary>
    /// <param name="regionIndex">비활성화할 구역 번호</param>
    public void CloseBuyVehicleBaseMenu(int regionIndex)
    {
        buyVehicleBaseRegionMenus[regionIndex].SetActive(false);
    }
    /// <summary>
    /// 역 구매 메뉴 구역별 활성화
    /// </summary>
    /// <param name="regionIndex">활성화할 구역 번호</param>
    public void OpenBuyStationMenu(int regionIndex)
    {
        buyStationRegionMenus[regionIndex].SetActive(true);
    }
    /// <summary>
    /// 역 구매 메뉴 구역별 비활성화
    /// </summary>
    /// <param name="regionIndex">활성화할 구역 번호</param>
    public void CloseBuyStationMenu(int regionIndex)
    {
        buyStationRegionMenus[regionIndex].SetActive(false);
    }
    /// <summary>
    /// 확장권 구매 메뉴 구역별 활성화
    /// </summary>
    /// <param name="regionIndex">활성화할 구역 번호</param>
    public void OpenBuyExpandMenu(int regionIndex)
    {
        buyExpandRegionMenus[regionIndex].SetActive(true);
    }
    /// <summary>
    /// 확장권 메뉴 구역별 비활성화
    /// </summary>
    /// <param name="regionIndex">활성화할 구역 번호</param>
    public void CloseBuyExpandMenu(int regionIndex)
    {
        buyExpandRegionMenus[regionIndex].SetActive(false);
    }
    /// <summary>
    /// 노선 연결 메뉴 구역별 활성화
    /// </summary>
    /// <param name="regionIndex">활성화할 구역 번호</param>
    public void OpenLineConnectMenu(int regionIndex)
    {
        lineConnectRegionMenus[regionIndex].SetActive(true);
    }
    /// <summary>
    /// 노선 연결 메뉴 구역별 비활성화
    /// </summary>
    /// <param name="regionIndex">활성화할 구역 번호</param>
    public void CloseLineConnectMenu(int regionIndex)
    {
        lineConnectRegionMenus[regionIndex].SetActive(false);
    }
    /// <summary>
    /// 열차 확장 메뉴 구역별 활성화
    /// </summary>
    /// <param name="regionIndex">활성화할 구역 번호</param>
    public void OpenTrainExpandMenu(int regionIndex)
    {
        trainExpandRegionMenus[regionIndex].SetActive(true);
    }
    /// <summary>
    /// 열차 확장 메뉴 구역별 비활성화
    /// </summary>
    /// <param name="regionIndex">활성화할 구역 번호</param>
    public void CloseTrainExpandMenu(int regionIndex)
    {
        trainExpandRegionMenus[regionIndex].SetActive(false);
    }
    /// <summary>
    /// 스크린도어 설치 메뉴 구역별 활성화
    /// </summary>
    /// <param name="regionIndex">활성화할 구역 번호</param>
    public void OpenScreendoorMenu(int regionIndex)
    {
        screendoorRegionMenus[regionIndex].SetActive(true);
    }
    /// <summary>
    /// 스크린도어 설치 메뉴 구역별 비활성화
    /// </summary>
    /// <param name="regionIndex">활성화할 구역 번호</param>
    public void CloseScreendoorMenu(int regionIndex)
    {
        screendoorRegionMenus[regionIndex].SetActive(false);
    }

}
