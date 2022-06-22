using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Compensation_Manager : MonoBehaviour {

    public MessageManager messageManager;
    //public OpeningCardPack openingCardPack;
    public ItemManager itemManager;
    public LineManager lineManager;

    public CompensationData compensationData;
    public CompanyReputationManager companyReputationManager;

    private bool bsCompensationChecked { get { return compensationData.bsCompensationChecked; } set { compensationData.bsCompensationChecked = value; } }
    private bool expandTrainChecked { get { return compensationData.expandTrainChecked; } set { compensationData.expandTrainChecked = value; } }
    private bool expandTrainPassengerChecked { get { return compensationData.expandTrainPassengerChecked; } set { compensationData.expandTrainPassengerChecked = value; } }
    private bool version3_0_1_checked { get { return compensationData.version3_0_1_checked; } set { compensationData.version3_0_1_checked = value; } }

    private bool allExpandErrorChecked { get { return compensationData.allExpandErrorChecked; } set { compensationData.allExpandErrorChecked = value; } }

    private bool reviseReputation { get { return compensationData.reviseReputation;  } set { compensationData.reviseReputation = value; } }

    //private bool isCompensationMessageOpened = false;
    private string tempMessage;

    public GameObject compensationMessage;
    public Text compensationMessageText;

    void Start()
    {
        if (!expandTrainChecked)
        {
            expandTrainChecked = true;
            RecoverExpandTrain();
        }
        if(!reviseReputation)
        {
            reviseReputation = true;
            ReviseReputation();
        }
    }
    private void RecoverExpandTrain()
    {
        for(int i = 0; i < lineManager.lineCollections.Length; i++)
        {
            int sum = 0;
            for(int j = 0; j < 4; j++)
            {
                if(lineManager.lineCollections[i].lineData.trainExpandStatus[j] < 0)
                {
                    lineManager.lineCollections[i].lineData.trainExpandStatus[j] = 0;
                }
                sum += lineManager.lineCollections[i].lineData.trainExpandStatus[j];
            }

            if (sum < lineManager.lineCollections[i].lineData.numOfTrain)
            {
                lineManager.lineCollections[i].lineData.trainExpandStatus[0] += lineManager.lineCollections[i].lineData.numOfTrain - sum;
            }
            else if(sum > lineManager.lineCollections[i].lineData.numOfTrain)
            {
                int cut = sum - lineManager.lineCollections[i].lineData.numOfTrain;
                for (int j = 0; j < 4; j++)
                {
                    if (cut <= 0)
                        break;

                    if (cut <= lineManager.lineCollections[i].lineData.trainExpandStatus[j])
                    {
                        lineManager.lineCollections[i].lineData.trainExpandStatus[j] -= cut;
                        cut = 0;
                    }
                    else
                    {
                        cut -= lineManager.lineCollections[i].lineData.trainExpandStatus[j];
                        lineManager.lineCollections[i].lineData.trainExpandStatus[j] = 0;
                    }
                }
            }
        } 
        DataManager.instance.SaveAll();
    }

    private void ReviseReputation()
    {
        int reputation = GetRevisedReputation(); 
       
        companyReputationManager.ReputationValue = reputation;

        DataManager.instance.SaveAll();
    }

    private int GetRevisedReputation()
    {
        int reputation = 0;

        for (int i = 0; i < lineManager.lineCollections.Length; i++)
        {
            if (!lineManager.lineCollections[i].isExpanded())
                break;

            for (int section = 0; section < lineManager.lineCollections[i].lineData.sectionExpanded.Length; section++)
            {
                if (lineManager.lineCollections[i].lineData.installed[section])
                {
                    reputation += lineManager.lineCollections[i].setupScreenDoor.priceData.ScreenDoorReputation[section];
                }
            }
        }
        return reputation;
    }
}
