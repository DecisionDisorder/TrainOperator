using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Compensation_Manager : MonoBehaviour {

    public MessageManager messageManager;
    //public OpeningCardPack openingCardPack;
    public ItemManager itemManager;
    public LineManager lineManager;

    public CompensationData compensationData;

    private bool bsCompensationChecked { get { return compensationData.bsCompensationChecked; } set { compensationData.bsCompensationChecked = value; } }
    private bool expandTrainChecked { get { return compensationData.expandTrainChecked; } set { compensationData.expandTrainChecked = value; } }
    private bool expandTrainPassengerChecked { get { return compensationData.expandTrainPassengerChecked; } set { compensationData.expandTrainPassengerChecked = value; } }
    private bool version3_0_1_checked { get { return compensationData.version3_0_1_checked; } set { compensationData.version3_0_1_checked = value; } }

    private bool allExpandErrorChecked { get { return compensationData.allExpandErrorChecked; } set { compensationData.allExpandErrorChecked = value; } }

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
    }
        /* if (!bsCompensationChecked)
        {
            bsCompensationChecked = true;
            StartCoroutine(CheckCom(1.0f));
        }
        if(!expandTrainPassengerChecked)
        {
            expandTrainPassengerChecked = true;
            RecoverExpandTrainPassenger();
        }
        if (!version3_0_1_checked)
        {
            version3_0_1_checked = true;
            Compensate3_0_1();
        }
        if(!allExpandErrorChecked)
        {
            allExpandErrorChecked = true;
            CompensateAllExpandError();
        }
    }
    IEnumerator CheckCom(float wTime)
    {
        yield return new WaitForSeconds(wTime);
        if(lineManager.lineCollections[(int)Line.Busan1].lineData.numOfTrain > 0)
        {
            itemManager.CardPoint += 600;
            messageManager.ShowMessage("부산 확장권 저장 오류로 인하여\n 카드 포인트 600P가 지급되었습니다.\n 이용에 불편을 드려 죄송합니다.", 4.0f);
        }
        DataManager.instance.SaveAll();
    }
    
    private void SaveData()
    {
        EncryptedPlayerPrefs.SetString("bsCompensationChecked", bsCompensationChecked.ToString());
        EncryptedPlayerPrefs.SetString("expandTrainCompChecked", expandTrainCompChecked.ToString());
        EncryptedPlayerPrefs.SetString("expandTrainPassengerChecked", expandTrainPassengerChecked.ToString());
    }
    private void LoadData()
    {
        bsCompensationChecked = bool.Parse(EncryptedPlayerPrefs.GetString("bsCompensationChecked", "false"));
        expandTrainCompChecked = bool.Parse(EncryptedPlayerPrefs.GetString("expandTrainCompChecked", "false"));
        expandTrainPassengerChecked = bool.Parse(EncryptedPlayerPrefs.GetString("expandTrainPassengerChecked", "false"));
    }
    */
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
    /*
    private void RecoverExpandTrainPassenger()
    {
        ulong addable = 0;
        for(int i = 20; i < 23; i++)
        {
            ulong diff;
            if(i == 20)
                diff = 3500000000000 - 200000000000;
            else if (i == 21)
                diff = 25000000000000 - 200000000000;
            else
                diff = 150000000000000 - 200000000000;
            
            int amount = lineManager.lineCollections[i].lineData.trainExpandStatus[1] + lineManager.lineCollections[i].lineData.trainExpandStatus[2] * 2 + lineManager.lineCollections[i].lineData.trainExpandStatus[3] * 3;
            addable = diff * (ulong)amount;
            TouchMoneyManager.ArithmeticOperation(addable, 0, true);
        }

        if (addable > 0)
        {
            openingCardPack.SilverCardAmount += 10;
            tempMessage = "안녕하세요, 개발자 DecisionDisorder입니다.\n수인분당선~인천2호선을 확장하면 추가되는 승객의 수가\n 의도된 수치보다 낮게 적용된 문제가 있었습니다.\n " +
                "이 오류는 해당 노선이 추가된 시점에 발생한 오류로 추정됩니다.\n이번 업데이트 때 열차 확장 기능을 전체적으로 손보면서 오류를 발견하였으며\n" +
                "수정 후 승객 수가 부족하게 추가된 부분은 복구 작업을 진행하였습니다.\n해당 오류로 불편을 겪으셨을 분들을 대상으로\n<color=red>실버 카드팩 10장</color>을 지급해 드리고자합니다.\n" +
                "꼼꼼하지 못한 작업 및 검수로 인해 오류를 야기한 점 다시한번 죄송합니다.\n" +
                "Decision Disorder 올림.";
            if (!isCompensationMessageOpened)
            {
                compensationMessage.SetActive(true);
                compensationMessageText.text = tempMessage;
            }
        }
        DataManager.instance.SaveAll();
    }

    public void CloseCompMessage()
    {
        if(isCompensationMessageOpened)
        {
            compensationMessageText.text = tempMessage;
            isCompensationMessageOpened = false;
        }
        else
            compensationMessage.SetActive(false);
    }
    private void Compensate3_0_1()
    {
        int target = 0;
        
        if (lineManager.lineCollections[(int)Line.Daegu1].lineData.connected[0])
        {
            TouchMoneyManager.ArithmeticOperation(90000000000, 0, true);
            target = 1;
        }
        if (lineManager.lineCollections[(int)Line.Daegu1].lineData.connected[0])
        {
            TouchMoneyManager.ArithmeticOperation(270000000000, 0, true);
            target = 1;
        }
        if (lineManager.lineCollections[(int)Line.Daegu1].lineData.connected[0])
        {
            TouchMoneyManager.ArithmeticOperation(720000000000, 0, true);
            target = 1;
        }
        for(int i = (int)Line.Line5; i <= (int)Line.Line9; i++)
        {
            LineData lineData = lineManager.lineCollections[i].lineData;
            if (lineData.trainExpandStatus[0].Equals(0) && lineData.numOfTrain > 0)
            {
                lineManager.lineCollections[i].lineData.trainExpandStatus[0] = lineData.numOfTrain;
                target = 2;
            }
        }

        if (lineManager.lineCollections[(int)Line.Line9].lineData.sectionExpanded[2])
            target = 3;

        if (target > 0)
        {
            openingCardPack.SilverCardAmount += 15;
            switch (target)
            {
                case 1:
                    messageManager.ShowMessage("대구 노선 연결 보상 오류로 인한\n이용에 불편을 드려 죄송합니다.\n실버 카드팩 15장이 지급되었습니다.", 5.0f);
                    break;
                case 2:
                    messageManager.ShowMessage("5~9호선 열차 확장 데이터 저장 오류로 인한\n이용에 불편을 드려 죄송합니다.\n실버 카드팩 15장이 지급되었습니다.", 5.0f);
                    break;
                case 3:
                    messageManager.ShowMessage("9호선 스크린도어 데이터 저장 오류로 인한\n이용에 불편을 드려 죄송합니다.\n실버 카드팩 15장이 지급되었습니다.", 5.0f);
                    break;
            }
        }
        else
        {
            if (lineManager.lineCollections[0].lineData.numOfTrain > 0)
            {
                openingCardPack.SilverCardAmount += 5;
                messageManager.ShowMessage("잦은 오류로 이용에 불편을 드려 죄송합니다.\n실버 카드팩 5장이 지급되었습니다.", 5.0f);
            }
        }

        DataManager.instance.SaveAll();
    }

    private void CompensateAllExpandError()
    {
        if(lineManager.lineCollections[0].lineData.numOfTrain > 0)
        {
            openingCardPack.SilverCardAmount += 5;
            messageManager.ShowMessage("확장권 구매 오류로 이용에 불편을 드려 죄송합니다.\n실버 카드팩 5장이 지급되었습니다.", 5.0f);
        }
        DataManager.instance.SaveAll();
    }*/
}
