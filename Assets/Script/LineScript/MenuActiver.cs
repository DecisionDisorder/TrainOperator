using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuType { BuyTrain, HireDriver, VehicleBase, Station, LineExpand, LineConnect, TrainExpand, Screendoor, ManageSubway, Rent, AD, Bank, 
    ConditionVehicleBase, CompanyReputation, SubwayCondition, DriverCondition, RentCondition, StationValueCondition }
public class MenuActiver : MonoBehaviour
{
    public AudioSource buttonsound_source;

    public GameObject[] basicMenus;
    public GameObject[] buyTrainRegionMenus;
    public GameObject[] buyVehicleBaseRegionMenus;
    public GameObject[] buyStationRegionMenus;
    public GameObject[] buyExpandRegionMenus;
    public GameObject[] lineConnectRegionMenus;
    public GameObject[] trainExpandRegionMenus;
    public GameObject[] screendoorRegionMenus;

    public void OpenMenu(MenuTypeSelect menuTypeSelect)
    {
        basicMenus[(int)menuTypeSelect.menuType].SetActive(true);
        buttonsound_source.Play();
    }

    public void CloseMenu(MenuTypeSelect menuTypeSelect)
    {
        basicMenus[(int)menuTypeSelect.menuType].SetActive(false);
    }

    public void OpenBuyTrainMenu(int regionIndex)
    {
        buyTrainRegionMenus[regionIndex].SetActive(true);
    }
    public void CloseBuyTrainMenu(int regionIndex)
    {
        buyTrainRegionMenus[regionIndex].SetActive(false);
    }
    public void OpenBuyVehicleBaseMenu(int regionIndex)
    {
        buyVehicleBaseRegionMenus[regionIndex].SetActive(true);
    }
    public void CloseBuyVehicleBaseMenu(int regionIndex)
    {
        buyVehicleBaseRegionMenus[regionIndex].SetActive(false);
    }
    public void OpenBuyStationMenu(int regionIndex)
    {
        buyStationRegionMenus[regionIndex].SetActive(true);
    }
    public void CloseBuyStationMenu(int regionIndex)
    {
        buyStationRegionMenus[regionIndex].SetActive(false);
    }
    public void OpenBuyExpandMenu(int regionIndex)
    {
        buyExpandRegionMenus[regionIndex].SetActive(true);
    }
    public void CloseBuyExpandMenu(int regionIndex)
    {
        buyExpandRegionMenus[regionIndex].SetActive(false);
    }
    public void OpenLineConnectMenu(int regionIndex)
    {
        lineConnectRegionMenus[regionIndex].SetActive(true);
    }
    public void CloseLineConnectMenu(int regionIndex)
    {
        lineConnectRegionMenus[regionIndex].SetActive(false);
    }
    public void OpenTrainExpandMenu(int regionIndex)
    {
        trainExpandRegionMenus[regionIndex].SetActive(true);
    }
    public void CloseTrainExpandMenu(int regionIndex)
    {
        trainExpandRegionMenus[regionIndex].SetActive(false);
    }
    public void OpenScreendoorMenu(int regionIndex)
    {
        screendoorRegionMenus[regionIndex].SetActive(true);
    }
    public void CloseScreendoorMenu(int regionIndex)
    {
        screendoorRegionMenus[regionIndex].SetActive(false);
    }

}
