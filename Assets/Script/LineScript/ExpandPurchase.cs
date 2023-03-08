using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �뼱 Ȯ��� ���� ���� Ŭ����
/// </summary>
[System.Serializable]
public class ExpandPurchase : MonoBehaviour
{
    public MessageManager messageManager;
    public ButtonColorManager buttonColorManager;

    /// <summary>
    /// �Ϲ� �뼱 ���� ���� ���� �� (���� �Ϲ� �뼱�� ���� ���� ����)
    /// </summary>
    private int normalLineCriteria = 100;
    /// <summary>
    /// ����ö �뼱 ���� ���� ���� �� (���� ����ö �뼱�� ���� ���� ����)
    /// </summary>
    private int lightRailCriteria = 25;

    /// <summary>
    /// �뼱 �޴� ������Ʈ
    /// </summary>
    public GameObject lineMenu;

    /// <summary>
    /// ���� �׷� ������Ʈ
    /// </summary>
    public GameObject sectionGroup;
    /// <summary>
    /// ���� �� �뼱�� �̹��� ������Ʈ
    /// </summary>
    public GameObject[] sectionImgs;
    /// <summary>
    /// ���� ���� �ȳ� ������Ʈ
    /// </summary>
    public GameObject[] LessTrainImages;
    /// <summary>
    /// ���� �� ���� �ؽ�Ʈ �迭
    /// </summary>
    public Text[] sectionPriceTexts;
    /// <summary>
    /// ���� �� ���� ��ư �迭
    /// </summary>
    public Button[] purchaseButtons;


    public PriceData priceData;
    public LineCollection lineCollection;
    public LineManager lineManager;
    public LineDataManager lineDataManager;
    public SettingManager buttonOption;
    public StationCustomizeManager stationCustomizeManager;
    public BackgroundImageManager backgroundImageManager;
    public UpdateDisplay expandUpdateDisplay;

    /// <summary>
    /// ���� ��� ����
    /// </summary>
    private int targetSection = -1;

    /// <summary>
    /// ���� ȿ����
    /// </summary>
    public AudioSource purchaseSound;

    private void Start()
    {
        expandUpdateDisplay.onEnable += SetSectionPriceTexts;
    }

    /// <summary>
    /// �뼱 �޴� Ȱ��ȭ/��Ȱ��ȭ
    /// </summary>
    /// <param name="onOff">Ȱ��ȭ ����</param>
    public void SetLineMenu(bool onOff)
    {
        lineMenu.SetActive(onOff);
    }
    /// <summary>
    /// �� ������ ���� ǥ��
    /// </summary>
    private void SetSectionPriceTexts()
    {
        string priceLow = "", priceHigh = "";
        for (int i = 0; i < sectionPriceTexts.Length; i++)
        {
            if (sectionPriceTexts[i] != null)
            {
                if (priceData.IsLargeUnit)
                    PlayManager.ArrangeUnit(0, priceData.ExpandPrice[i], ref priceLow, ref priceHigh);
                else
                    PlayManager.ArrangeUnit(priceData.ExpandPrice[i], 0, ref priceLow, ref priceHigh);

                sectionPriceTexts[i].text = "����: " + priceHigh + priceLow + "$";
            }
        }
    }

    /// <summary>
    /// Ư�� ���� ���� Ȯ�� �޴� ����
    /// </summary>
    /// <param name="section">���� �ε���</param>
    public void OpenSection(int section)
    {
        // �̹� ������ �������� Ȯ�� �� ���� �޴� Ȱ��ȭ
        if (!lineCollection.lineData.sectionExpanded[section])
        {
            if(targetSection != -1)
                sectionImgs[targetSection].SetActive(false);
            lineManager.currentLine = lineCollection.line;
            targetSection = section;
            sectionGroup.SetActive(true);
            sectionImgs[section].SetActive(true);
        }
        else
            AlreadyPurchased();
    }

    /// <summary>
    /// ���� �뼱�� ������ �ľ��Ͽ� ���� ������ ���� �� ��ȯ
    /// </summary>
    private int GetTrainAmountCriteria()
    {
        return lineManager.lineCollections[(int)lineCollection.line - 1].expandPurchase.priceData.IsLightRail ? lightRailCriteria : normalLineCriteria;
    }

    /// <summary>
    /// Ȯ��� ���� �ڰ��� �Ǵ��� Ȯ��
    /// </summary>
    public void SetQualification()
    {
        // 1ȣ���� �����ϰ� ���� �ڰ� Ȯ��
        if (!lineCollection.line.Equals(Line.Line1))
        {
            if (lineManager.lineCollections[(int)lineCollection.line - 1].lineData.numOfTrain < GetTrainAmountCriteria())
            {
                SetLessTrainImgs(true);
            }
            else
            {
                SetLessTrainImgs(false);
            }
        }
    }

    /// <summary>
    /// ���� ���� �ȳ� UI Ȱ��ȭ/��Ȱ��ȭ
    /// </summary>
    /// <param name="onOff">Ȱ��ȭ ����</param>
    private void SetLessTrainImgs(bool onOff)
    {
        for (int i = 0; i < LessTrainImages.Length; i++)
        {
            LessTrainImages[i].SetActive(onOff);
            purchaseButtons[i].enabled = !onOff;
        }
    }

    /// <summary>
    /// ���� Ȯ��/��� ó��
    /// </summary>
    /// <param name="accept">���� ����</param>
    public void CheckConfirm(bool accept)
    {
        if (accept)
            BuyExpand();
        sectionGroup.SetActive(false);
        sectionImgs[targetSection].SetActive(false);
        targetSection = -1;
    }

    /// <summary>
    /// �ش� �뼱�� ó������ Ȯ���ϴ� ������ Ȯ��
    /// </summary>
    /// <returns></returns>
    private bool IsFirstExpand()
    {
        for (int i = 0; i < lineCollection.lineData.sectionExpanded.Length; i++)
            if (lineCollection.lineData.sectionExpanded[i])
                return false;

        return true;
    }

    /// <summary>
    /// Ȯ��� ���� ó��
    /// </summary>
    private void BuyExpand()
    {
        // ��� ����
        bool result;
        if (priceData.IsLargeUnit)
            result = AssetMoneyCalculator.instance.ArithmeticOperation(0, priceData.ExpandPrice[targetSection], false);
        else
            result = AssetMoneyCalculator.instance.ArithmeticOperation(priceData.ExpandPrice[targetSection], 0, false);

        // ��� ������ �����ϸ�
        if (result)
        {
            // ó�� Ȯ���� �� ��� ��õ UI�� ����.
            bool firstExpand = IsFirstExpand();
            if (firstExpand && buttonOption.BackupRecommend)
            {
                messageManager.OpenCommonCheckMenu("Ŭ���� ����Ϸ� ���ðڽ��ϱ�?", "�ű� �뼱�� Ȯ����� �����ϼ̽��ϴ�.\nȤ�� �� ��Ȳ�� ����Ͽ� Ŭ���� ����� �����մϴ�.", Color.cyan, OpenCloud);
            }
            
            // ó�� Ȯ���� �� �°��� ����� ���ο� �뼱���� ����
            if(firstExpand)
            {
                try
                {
                    backgroundImageManager.ChangeBackground(0);
                    stationCustomizeManager.SetBackgroundToRecentLine(lineCollection.line);
                    messageManager.ShowMessage("���ο� �뼱�� ������� ����Ǿ����ϴ�!\n��� �뼱/���� �����Ϸ���\n\"����-�뼱/�� ����\"�� �̿����ּ���!", 2f);
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                    messageManager.ShowMessage("�Ͻ����� ������ ���� �뼱 ����� ������� �ʾҽ��ϴ�.\n��� �뼱/���� �����Ϸ���\n\"����-�뼱/�� ����\"�� �̿����ּ���!", 2f);
                }
            }

            // 2ȣ���� �� ��� ������ Ȯ����� ���� ó��
            if(lineCollection.line.Equals(Line.Line2))
            {
                messageManager.ShowMessage("2ȣ�� ��ü Ȯ����� �����Ͽ����ϴ�.");
                for(int i = 0; i < lineCollection.lineData.sectionExpanded.Length; i++)
                    lineCollection.lineData.sectionExpanded[i] = true;
            }
            // �ƴҶ� ���� ���� ó��
            else
            {
                messageManager.ShowMessage(priceData.Sections[targetSection].name + " �뼱 Ȯ����� �����Ͽ����ϴ�.");
                lineCollection.lineData.sectionExpanded[targetSection] = true;
            }
            // ������ Ȯ��� ��ư�� ������ �ٲٰ� ���� �� ���� ó�� ȿ���� ���
            buttonColorManager.SetColorExpand(targetSection);
            DataManager.instance.SaveAll();
            purchaseSound.Play();
        }
        // ���� �����Ͽ� ��� ������ ������ ���
        else
            LackOfMoney();
    }

    /// <summary>
    /// Ŭ���� ��� �޴� Ȱ��ȭ
    /// </summary>
    private void OpenCloud()
    {
        CloudSubManager.instance.SetCloudMenu(true);
    }

    /// <summary>
    /// �̹� ������ �����̶�� �ȳ� �޽��� ���
    /// </summary>
    private void AlreadyPurchased()
    {
        messageManager.ShowMessage("�̹� �����ϼ̽��ϴ�.");
    }

    /// <summary>
    /// ���� �����ϴٴ� �ȳ� �޽��� ���
    /// </summary>
    private void LackOfMoney()
    {
        messageManager.ShowMessage("���� �����մϴ�.");
    }
}
