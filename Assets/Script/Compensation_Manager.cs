using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 보상 및 보정 관리 클래스
/// </summary>
public class Compensation_Manager : MonoBehaviour {

    public MessageManager messageManager;
    public ItemManager itemManager;
    public LineManager lineManager;

    /// <summary>
    /// 보상 지급/보정 여부 기록 데이터
    /// </summary>
    public CompensationData compensationData;
    public CompanyReputationManager companyReputationManager;
    public LotteryTicketManager lotteryTicketManager;

    /// <summary>
    /// 부산 노선 관련 보상 지급 여부
    /// </summary>
    private bool bsCompensationChecked { get { return compensationData.bsCompensationChecked; } set { compensationData.bsCompensationChecked = value; } }
    /// <summary>
    /// 열차 확장 개수 오류 복구 여부
    /// </summary>
    private bool expandTrainChecked { get { return compensationData.expandTrainChecked; } set { compensationData.expandTrainChecked = value; } }
    /// <summary>
    /// 열차 확장 승객수 복구 여부
    /// </summary>
    private bool expandTrainPassengerChecked { get { return compensationData.expandTrainPassengerChecked; } set { compensationData.expandTrainPassengerChecked = value; } }
    /// <summary>
    /// 3.0.1 보상 수령 여부
    /// </summary>
    private bool version3_0_1_checked { get { return compensationData.version3_0_1_checked; } set { compensationData.version3_0_1_checked = value; } }
    
    /// <summary>
    /// 열차 확장 오류 보상 수령 여부
    /// </summary>
    private bool allExpandErrorChecked { get { return compensationData.allExpandErrorChecked; } set { compensationData.allExpandErrorChecked = value; } }
    
    /// <summary>
    /// 고객 만족도 수치 변환 여부
    /// </summary>
    private bool reviseReputation { get { return compensationData.reviseReputation;  } set { compensationData.reviseReputation = value; } }
    
    /// <summary>
    /// 경전철 보상 수령 여부
    /// </summary>
    private bool LightRailCompenstaionCheck { get { return compensationData.lightRailCompensationChecked; } set { compensationData.lightRailCompensationChecked = value; } }

    /// <summary>
    /// 복권 오류 복원 여부
    /// </summary>
    private bool LotteryErrorRecovered { get { return compensationData.lotteryErrorRecovered; } set { compensationData.lotteryErrorRecovered = value; } }

    /// <summary>
    /// 보상 메시지 오브젝트
    /// </summary>
    public GameObject compensationMessage;
    /// <summary>
    /// 보상 메시지 텍스트
    /// </summary>
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
        if(!LightRailCompenstaionCheck)
        {
            LightRailCompenstaionCheck = true;
            if(lineManager.lineCollections[0].lineData.numOfTrain > 0)
            {
                OfferLightRailCompensation();
            }
        }
        if(!LotteryErrorRecovered)
        {
            LotteryErrorRecovered = true;
            RecoverLotteryError();
        }
    }

    /// <summary>
    /// 열차 확장 개수 오류 복구
    /// </summary>
    private void RecoverExpandTrain()
    {
        for(int i = 0; i < lineManager.lineCollections.Length; i++)
        {
            int sum = 0;

            if (!lineManager.lineCollections[i].purchaseStation.priceData.IsLightRail)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (lineManager.lineCollections[i].lineData.trainExpandStatus[j] < 0)
                    {
                        lineManager.lineCollections[i].lineData.trainExpandStatus[j] = 0;
                    }
                    sum += lineManager.lineCollections[i].lineData.trainExpandStatus[j];
                }

                if (sum < lineManager.lineCollections[i].lineData.numOfTrain)
                {
                    lineManager.lineCollections[i].lineData.trainExpandStatus[0] += lineManager.lineCollections[i].lineData.numOfTrain - sum;
                }
                else if (sum > lineManager.lineCollections[i].lineData.numOfTrain)
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
        } 
        DataManager.instance.SaveAll();
    }

    /// <summary>
    /// 고객 만족도 수치 변환
    /// </summary>
    private void ReviseReputation()
    {
        int reputation = GetRevisedReputation(); 
       
        companyReputationManager.ReputationValue = reputation;

        DataManager.instance.SaveAll();
    }

    /// <summary>
    /// 변경된 고객 만족도 계산
    /// </summary>
    /// <returns>변환된 고객 만족도 수치</returns>
    private int GetRevisedReputation()
    {
        int reputation = 0;

        for (int i = 0; i < lineManager.lineCollections.Length; i++)
        {
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

    /// <summary>
    /// 경전철 오류 보상 지급
    /// </summary>
    private void OfferLightRailCompensation()
    {
        itemManager.FeverRefillAmount += 10;
        itemManager.CardPoint += 750;
        compensationMessage.SetActive(true);
    }

    /// <summary>
    /// 보상 메시지 비활성화
    /// </summary>
    public void CloseCompensationMessage()
    {
        DataManager.instance.SaveAll();
        compensationMessage.SetActive(false);
    }

    /// <summary>
    /// 복권 오류 복구 및 보상 지급
    /// </summary>
    private void RecoverLotteryError()
    {
        bool recovered = false;
        LargeVariable price = lotteryTicketManager.GetPricePerTicket();
        for (int i = 0; i < lotteryTicketManager.JackpotWinRecords.Count; i++)
        {
            if(lotteryTicketManager.JackpotWinRecords[i].pastReward == LargeVariable.zero)
            {
                int rank = lotteryTicketManager.JackpotWinRecords[i].pastRank - 1;
                LargeVariable recoveredReward = price * lotteryTicketManager.drawSets[(int)DrawProduct.Jackpot].rewardMultiple[rank];
                lotteryTicketManager.UnreceivedReward += recoveredReward;
                lotteryTicketManager.JackpotWinRecords[i].pastReward = recoveredReward;
                recovered = true;
            }
        }
        for (int i = 0; i < lotteryTicketManager.NormalWinRecords.Count; i++)
        {
            if (lotteryTicketManager.NormalWinRecords[i].pastReward == LargeVariable.zero)
            {
                int rank = lotteryTicketManager.NormalWinRecords[i].pastRank - 1;
                LargeVariable recoveredReward = price * lotteryTicketManager.drawSets[(int)DrawProduct.Normal].rewardMultiple[rank];
                lotteryTicketManager.UnreceivedReward += recoveredReward;
                lotteryTicketManager.NormalWinRecords[i].pastReward = recoveredReward;
                recovered = true;
            }
        }
        for (int i = 0; i < lotteryTicketManager.SimpleWinRecords.Count; i++)
        {
            if (lotteryTicketManager.SimpleWinRecords[i].pastReward == LargeVariable.zero)
            {
                int rank = lotteryTicketManager.SimpleWinRecords[i].pastRank - 1;
                LargeVariable recoveredReward = price * lotteryTicketManager.drawSets[(int)DrawProduct.Simple].rewardMultiple[rank];
                lotteryTicketManager.UnreceivedReward += recoveredReward;
                lotteryTicketManager.SimpleWinRecords[i].pastReward = recoveredReward;
                recovered = true;
            }
        }

        if(recovered)
        {
            itemManager.CardPoint += 400;
            messageManager.ShowMessage("0원으로 계산된 복권 당첨금이 미지급 당첨금으로 입금되었으며, <color=blue>카드포인트 400P</color>를 지급해드렸습니다.\n이용에 불편을 드려 죄송합니다.", 5.0f);
        }

        DataManager.instance.SaveAll();
    }
}
