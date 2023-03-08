using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����ö ���� �ý��� ���� Ŭ����
/// </summary>
public class LightRailControlManager : MonoBehaviour
{
    /// <summary>
    /// ���� Ȯ�� �޴� ������Ʈ
    /// </summary>
    public GameObject trainExpandMenu;
    /// <summary>
    /// �뼱 ���׷��̵� �޴� ������Ʈ
    /// </summary>
    public GameObject upgradeLineMenu;

    /// <summary>
    /// Ư�� �뼱 ���׷��̵� �޴� ������Ʈ
    /// </summary>
    public GameObject upgradeMenu;
    /// <summary>
    /// ���׷��̵� ��� �뼱 �ؽ�Ʈ
    /// </summary>
    public Text targetLineText;

    /// <summary>
    /// �׸� �� ���׷��̵� �ܰ踦 ��Ÿ���� �����̴�
    /// </summary>
    public Slider[] upgradeSliders;
    /// <summary>
    /// �׸� �� ���׷��̵� ��� �ؽ�Ʈ
    /// </summary>
    public Text[] upgradePriceTexts;
    /// <summary>
    /// �׸� �� ���׷��̵� ����(�°� ��) �ؽ�Ʈ
    /// </summary>
    public Text[] upgradePassengerTexts;

    /// <summary>
    /// �׸� ���/���� ����
    /// </summary>

    public float[] division = { 0.1f, 0.15f, 0.2f, 0.25f, 0.3f};
    
    /// <summary>
    /// ��� ����ö �뼱
    /// </summary>

    private LightRailLine targetLine;

    /// <summary>
    /// ���׷��̵� �ִ� ����
    /// </summary>
    public const int maxLevel = 5;

    public PriceData[] lightRailPriceData;
    public LineManager lineManager;
    public MessageManager messageManager;

    /// <summary>
    /// ���� Ȯ�� �޴� Ȱ��ȭ/��Ȱ��ȭ
    /// </summary>
    /// <param name="active">Ȱ��ȭ ����</param>
    public void SetTrainExpandMenu(bool active)
    {
        trainExpandMenu.SetActive(active);
    }
    /// <summary>
    /// �뼱 ���׷��̵� �޴� Ȱ��ȭ/��Ȱ��ȭ
    /// </summary>
    /// <param name="active">Ȱ��ȭ ����</param>
    public void SetUpgradeLineMenu(bool active)
    {
        upgradeLineMenu.SetActive(active);
    }

    /// <summary>
    /// ���׷��̵� ��� �뼱 ����
    /// </summary>
    /// <param name="index">����ö �뼱 �ε���</param>
    public void SelectTargetLine(int index)
    {
        targetLine = (LightRailLine)index;
        targetLineText.text = "��� �뼱: " + lineManager.lineCollections[GetEntireLineIndex((LightRailLine)index)].purchaseTrain.lineName;
        UpdateUpgradeState();
        upgradeMenu.SetActive(true);
    }

    /// <summary>
    /// ���׷��̵� �޴� ��Ȱ��ȭ
    /// </summary>
    public void CloseUpgradeMenu()
    {
        upgradeMenu.SetActive(false);
    }

    /// <summary>
    /// ���׷��̵� ���� ����
    /// </summary>
    private void UpdateUpgradeState()
    {
        for(int i = 0; i < upgradeSliders.Length; i++)
        {
            int level = GetLineControlLevel(i);
            upgradeSliders[i].value = level;

            if (level < maxLevel)
            {
                upgradePriceTexts[i].text = "����: " + GetPrice(i).ToString() + "$";
                upgradePassengerTexts[i].text = "�°� �� +" + GetPassenger(i).ToString() + "��";
            }
            else
            {
                upgradePriceTexts[i].text = "";
                upgradePassengerTexts[i].text = "";
            }
        }
    }
    
    /// <summary>
    /// ���� �ý��� ���׷��̵�
    /// </summary>
    /// <param name="product">���׷��̵� �� ��ǰ</param>
    public void UpgradeControlSystem(int product)
    {
        // ���׷��̵� �� ��ǰ�� �ִ� ������ ���� ������
        if (GetLineControlLevel(product) < maxLevel)
        {
            // ��� ���� ó��
            if (AssetMoneyCalculator.instance.ArithmeticOperation(GetPrice(product), false))
            {
                // ���׷��̵� ���� ���� ó��
                LargeVariable passenger = GetPassenger(product);
                TouchMoneyManager.ArithmeticOperation(passenger.lowUnit, passenger.highUnit, true);
                CompanyReputationManager.instance.RenewPassengerBase();
                AddLineControlLevel(product);
                UpdateUpgradeState();
            }
            // ��� ���� �޽��� ���
            else
                messageManager.ShowMessage("���� �����Ͽ� ���׷��̵��� �� �����ϴ�.");
        }
        // �ִ� ���׷��̵� ���� �޽��� ���
        else
        {
            messageManager.ShowMessage("�ִ� ���׷��̵� �����Դϴ�.");
        }
    }
    
    /// <summary>
    /// ���׷��̵� ��� ã��
    /// </summary>
    /// <param name="product">��� ��ǰ</param>
    /// <returns>���׷��̵� ���</returns>
    private LargeVariable GetPrice(int product)
    {
        int level = GetLineControlLevel(product);
        return GetPrice(product, level);
    }

    /// <summary>
    /// Ư�� ������ ���� ���׷��̵� ��� ã��
    /// </summary>
    /// <param name="product">��� ��ǰ</param>
    /// <param name="level">��� ����</param>
    /// <returns>���׷��̵� ���</returns>
    public LargeVariable GetPrice(int product, int level)
    {
        if (level < 5)
        {
            LargeVariable variable = lightRailPriceData[(int)targetLine].LineControlUpgradePrice[product] * division[level];
            variable.detailed = true;
            return variable;
        }
        else
            return LargeVariable.zero;
    }

    /// <summary>
    /// ���׷��̵忡 ���� �°� �� ���� ���
    /// </summary>
    /// <param name="product">��� ��ǰ</param>
    /// <returns>�°� ��</returns>
    private LargeVariable GetPassenger(int product)
    {
        int level = GetLineControlLevel(product);
        return GetPassenger(product, level);
    }

    /// <summary>
    /// Ư�� ������ ���� ���׷��̵� �°� �� ���� ���
    /// </summary>
    /// <param name="product">��� ��ǰ</param>
    /// <param name="level">�°� ��</param>
    /// <returns></returns>
    public LargeVariable GetPassenger(int product, int level)
    {
        if (level < 5)
        {
            LargeVariable variable = lightRailPriceData[(int)targetLine].LineControlUpgradePassenger[product] * division[level];
            variable.detailed = true;
            return variable;
        }
        else
            return LargeVariable.zero;
    }

    /// <summary>
    /// Ư�� ��ǰ�� ���׷��̵� ���� ã��
    /// </summary>
    /// <param name="product">��� ��ǰ</param>
    /// <returns></returns>
    public int GetLineControlLevel(int product)
    {
        return lineManager.lineCollections[GetEntireLineIndex(targetLine)].LineControlLevels[product];
    }

    /// <summary>
    /// Ư�� ��ǰ�� ���׷��̵� ���� ����
    /// </summary>
    /// <param name="product">��� ��ǰ</param>
    private void AddLineControlLevel(int product)
    {
        lineManager.lineCollections[GetEntireLineIndex(targetLine)].lineData.lineControlLevels[product]++;
    }

    /// <summary>
    /// ����ö�� ��ü �뼱�󿡼��� �뼱 �ε���
    /// </summary>
    /// <param name="lightRailIndex">����ö ���� �뼱 �ε���</param>
    /// <returns>��ü �뼱 �ε���</returns>
    private int GetEntireLineIndex(LightRailLine lightRailIndex)
    {
        for(int i = 0; i < lineManager.lineCollections.Length; i++)
        {
            if (((Line)i).ToString().Equals(lightRailIndex.ToString()))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// ���� ���� ���
    /// </summary>
    /// <param name="lineIndex">��ü �뼱 �ε���</param>
    /// <returns></returns>
    public int GetTotalLevel(int lineIndex)
    {
        int level = 0;
        for (int i = 0; i < lineManager.lineCollections[lineIndex].LineControlLevels.Length; i++)
            level += lineManager.lineCollections[lineIndex].LineControlLevels[i];
        return level;
    }
}
