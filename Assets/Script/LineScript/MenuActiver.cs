using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �޴� ������ �����ϴ� ������
/// </summary>
public enum MenuType { BuyTrain, HireDriver, VehicleBase, Station, LineExpand, LineConnect, TrainExpand, Screendoor, ManageSubway, Rent, AD, Bank, 
    ConditionVehicleBase, CompanyReputation, SubwayCondition, DriverCondition, RentCondition, StationValueCondition }

/// <summary>
/// ��з����� �޴��� Ȱ��ȭ/��Ȱ��ȭ�� �����ϴ� Ŭ����
/// </summary>
public class MenuActiver : MonoBehaviour
{
    /// <summary>
    /// ��ư ȿ����
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
    /// �޴� Ȱ��ȭ
    /// </summary>
    /// <param name="menuTypeSelect">Ȱ��ȭ�� �޴��� ����</param>
    public void OpenMenu(MenuTypeSelect menuTypeSelect)
    {
        basicMenus[(int)menuTypeSelect.menuType].SetActive(true);
        buttonsound_source.Play();
    }

    /// <summary>
    /// �޴� ��Ȱ��ȭ
    /// </summary>
    /// <param name="menuTypeSelect">��Ȱ��ȭ�� �޴��� ����</param>
    public void CloseMenu(MenuTypeSelect menuTypeSelect)
    {
        basicMenus[(int)menuTypeSelect.menuType].SetActive(false);
    }

    /// <summary>
    /// ���� ���� �޴� ������ Ȱ��ȭ
    /// </summary>
    /// <param name="regionIndex">Ȱ��ȭ�� ���� ��ȣ</param>
    public void OpenBuyTrainMenu(int regionIndex)
    {
        buyTrainRegionMenus[regionIndex].SetActive(true);
    }
    /// <summary>
    /// ���� ���� �޴� ������ ��Ȱ��ȭ
    /// </summary>
    /// <param name="regionIndex">��Ȱ��ȭ�� ���� ��ȣ</param>
    public void CloseBuyTrainMenu(int regionIndex)
    {
        buyTrainRegionMenus[regionIndex].SetActive(false);
    }
    /// <summary>
    /// �������� �޴� ������ Ȱ��ȭ
    /// </summary>
    /// <param name="regionIndex">Ȱ��ȭ�� ���� ��ȣ</param>
    public void OpenBuyVehicleBaseMenu(int regionIndex)
    {
        buyVehicleBaseRegionMenus[regionIndex].SetActive(true);
    }
    /// <summary>
    /// �������� �޴� ������ ��Ȱ��ȭ
    /// </summary>
    /// <param name="regionIndex">��Ȱ��ȭ�� ���� ��ȣ</param>
    public void CloseBuyVehicleBaseMenu(int regionIndex)
    {
        buyVehicleBaseRegionMenus[regionIndex].SetActive(false);
    }
    /// <summary>
    /// �� ���� �޴� ������ Ȱ��ȭ
    /// </summary>
    /// <param name="regionIndex">Ȱ��ȭ�� ���� ��ȣ</param>
    public void OpenBuyStationMenu(int regionIndex)
    {
        buyStationRegionMenus[regionIndex].SetActive(true);
    }
    /// <summary>
    /// �� ���� �޴� ������ ��Ȱ��ȭ
    /// </summary>
    /// <param name="regionIndex">Ȱ��ȭ�� ���� ��ȣ</param>
    public void CloseBuyStationMenu(int regionIndex)
    {
        buyStationRegionMenus[regionIndex].SetActive(false);
    }
    /// <summary>
    /// Ȯ��� ���� �޴� ������ Ȱ��ȭ
    /// </summary>
    /// <param name="regionIndex">Ȱ��ȭ�� ���� ��ȣ</param>
    public void OpenBuyExpandMenu(int regionIndex)
    {
        buyExpandRegionMenus[regionIndex].SetActive(true);
    }
    /// <summary>
    /// Ȯ��� �޴� ������ ��Ȱ��ȭ
    /// </summary>
    /// <param name="regionIndex">Ȱ��ȭ�� ���� ��ȣ</param>
    public void CloseBuyExpandMenu(int regionIndex)
    {
        buyExpandRegionMenus[regionIndex].SetActive(false);
    }
    /// <summary>
    /// �뼱 ���� �޴� ������ Ȱ��ȭ
    /// </summary>
    /// <param name="regionIndex">Ȱ��ȭ�� ���� ��ȣ</param>
    public void OpenLineConnectMenu(int regionIndex)
    {
        lineConnectRegionMenus[regionIndex].SetActive(true);
    }
    /// <summary>
    /// �뼱 ���� �޴� ������ ��Ȱ��ȭ
    /// </summary>
    /// <param name="regionIndex">Ȱ��ȭ�� ���� ��ȣ</param>
    public void CloseLineConnectMenu(int regionIndex)
    {
        lineConnectRegionMenus[regionIndex].SetActive(false);
    }
    /// <summary>
    /// ���� Ȯ�� �޴� ������ Ȱ��ȭ
    /// </summary>
    /// <param name="regionIndex">Ȱ��ȭ�� ���� ��ȣ</param>
    public void OpenTrainExpandMenu(int regionIndex)
    {
        trainExpandRegionMenus[regionIndex].SetActive(true);
    }
    /// <summary>
    /// ���� Ȯ�� �޴� ������ ��Ȱ��ȭ
    /// </summary>
    /// <param name="regionIndex">Ȱ��ȭ�� ���� ��ȣ</param>
    public void CloseTrainExpandMenu(int regionIndex)
    {
        trainExpandRegionMenus[regionIndex].SetActive(false);
    }
    /// <summary>
    /// ��ũ������ ��ġ �޴� ������ Ȱ��ȭ
    /// </summary>
    /// <param name="regionIndex">Ȱ��ȭ�� ���� ��ȣ</param>
    public void OpenScreendoorMenu(int regionIndex)
    {
        screendoorRegionMenus[regionIndex].SetActive(true);
    }
    /// <summary>
    /// ��ũ������ ��ġ �޴� ������ ��Ȱ��ȭ
    /// </summary>
    /// <param name="regionIndex">Ȱ��ȭ�� ���� ��ȣ</param>
    public void CloseScreendoorMenu(int regionIndex)
    {
        screendoorRegionMenus[regionIndex].SetActive(false);
    }

}
