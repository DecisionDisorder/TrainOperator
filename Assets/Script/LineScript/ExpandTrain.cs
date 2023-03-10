using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

/// <summary>
/// 열차 확장 관리 클래스
/// </summary>
[System.Serializable]
public class ExpandTrain : MonoBehaviour
{
    /// <summary>
    /// 구매 확인 메뉴 오브젝트
    /// </summary>
    public GameObject CheckMenu;
    /// <summary>
    /// 확장할 길이 텍스트
    /// </summary>
    public Text lengthText;
    /// <summary>
    /// 확장 비용 텍스트
    /// </summary>
    public Text priceText;
    /// <summary>
    /// 확장 시, 증가하는 승객 수 안내 텍스트
    /// </summary>
    public Text passengerText;
    /// <summary>
    /// 차량기지 현황에서의 차량 길이 별 현황 텍스트
    /// </summary>
    public Text trainExpandStatusText;
    /// <summary>
    /// 확장 수량 입력 필드
    /// </summary>
    public InputField expandAmountInputField;

    public MessageManager messageManager;
    public PriceData priceData;
    public LineCollection lineCollection;
    public LineDataManager lineDataManager;
    public LineManager lineManager;
    public UpdateDisplay trainConditionUpdateDisplay;

    /// <summary>
    /// 확장 종류별 Pair
    /// </summary>
    private static int[,] fromToLength = { { 4, 6 }, { 4, 8 }, { 4, 10 }, { 6, 8 }, { 6, 10 }, { 8, 10 } };
    /// <summary>
    /// 확장 종류
    /// </summary>
    private int targetType;

    /// <summary>
    /// 노선 메뉴 오브젝트
    /// </summary>
    public GameObject lineMenu;

    /// <summary>
    /// 가이드 오브젝트 (1호선 한정)
    /// </summary>
    public GameObject guide;

    /// <summary>
    /// 최종 가격
    /// </summary>
    private ulong totalPrice;
    /// <summary>
    /// 최종 승객 수 증가량(Low 단위)
    /// </summary>
    private ulong totalPassengerLow;
    /// <summary>
    /// 최종 승객 수 증가량(High 단위)
    /// </summary>
    private ulong totalPassengerHigh;
    /// <summary>
    /// 확장 수량
    /// </summary>
    private int expandAmount;
    /// <summary>
    /// 구매 효과음
    /// </summary>
    public AudioSource purchaseSound;

    void Start()
    {
        trainConditionUpdateDisplay.onEnableUpdate += SetTrainExpandText;
    }

    /// <summary>
    /// 노선 메뉴 활성화/비활성화
    /// </summary>
    /// <param name="onOff">활성화 여부</param>
    public void SetLineMenu(bool onOff)
    {
        lineMenu.SetActive(onOff);
    }

    /// <summary>
    /// 확장 안내 텍스트 Formatting
    /// </summary>
    /// <param name="type">확장 종류 인덱스</param>
    /// <param name="maxAmount">확장 가능한 최대 개수</param>
    /// <returns>안내 텍스트</returns>
    public string GetExpandTargetLength(int type, int maxAmount)
    {
        return string.Format("{0}량 ▶▶ {1}량 ({2}개 확장 가능)", fromToLength[type, 0], fromToLength[type, 1], maxAmount);
    }

    /// <summary>
    /// 차량 확장 종류 설정
    /// </summary>
    /// <param name="type">확장 종류 인덱스</param>
    public void SetUpgradeType(int type)
    {
        targetType = type;
        int maxAmount = lineCollection.lineData.trainExpandStatus[GetIndexFromLength(fromToLength[targetType, 0])];
        CheckMenu.SetActive(true);
        lineManager.currentLine = lineCollection.line;
        lengthText.text = GetExpandTargetLength(type, maxAmount);
        if (guide != null)
            guide.SetActive(true);
    }

    /// <summary>
    /// 열차 확장 처리
    /// </summary>
    /// <param name="apply">확장 확인 여부</param>

    public void ExpandTrainProcess(bool apply)
    {
        // 확장 확인한 경우
        if(apply)
        { 
            // 확장 개수 최대치를 초과하는 경우에 대한 예외 처리
            if(expandAmount > lineCollection.lineData.trainExpandStatus[GetIndexFromLength(fromToLength[targetType, 0])])
                SetAmountAll();

            // 승객 수 제한을 초과하지 않는지 확인
            if (TouchMoneyManager.CheckLimitValid(totalPassengerLow, totalPassengerHigh))
            {
                // 비용 지불 처리
                bool result;
                if(priceData.IsLargeUnit)
                    result = AssetMoneyCalculator.instance.ArithmeticOperation(0, totalPrice, false);
                else
                    result = AssetMoneyCalculator.instance.ArithmeticOperation(totalPrice, 0, false);

                // 지불 처리 성공 시, 열차 확장 적용
                if (result)
                {
                    ApplyExpandTrain();
                    TouchMoneyManager.ArithmeticOperation(totalPassengerLow, totalPassengerHigh, true);
                    CompanyReputationManager.instance.RenewPassengerBase();
                    ExpandSuccess(expandAmount);
                    SetTrainExpandText();
                    DataManager.instance.SaveAll();
                    if (!totalPrice.Equals(0))
                        purchaseSound.Play();
                }
                // 비용 부족으로 인한 메시지 출력
                else
                    PlayManager.instance.LackOfMoney();
            }
            // 승객 수 제한으로 인한 구매 불가 메시지 출력
            else
                PlayManager.instance.LackOfPassengerLimit();
        }

        // 관련 UI 및 값 초기화 및 비활성화
        expandAmount = 0;
        totalPrice = 0;
        expandAmountInputField.text = expandAmount.ToString();
        priceText.text = "가격: 0$";
        passengerText.text = "승객 수+ 0명";
        CheckMenu.SetActive(false);

        if (guide != null)
            guide.SetActive(false);
    }

    /// <summary>
    /// 확장 수량 설정
    /// </summary>
    /// <param name="amount">수량</param>
    public void SetExpandAmount(int amount)
    {
        // 최대 수량을 넘지 않는지 확인
        expandAmount = amount;
        int maximumAmount = lineCollection.lineData.trainExpandStatus[GetIndexFromLength(fromToLength[targetType, 0])];
        if (expandAmount > maximumAmount)
            expandAmount = maximumAmount;

        // 가격 및 텍스트 설정
        SetTotalPrice();
        SetCheckTexts();
    }
    
    /// <summary>
    /// 확장 가능한 모든 열차를 확장하도록 설정
    /// </summary>
    public void SetAmountAll()
    {
        SetExpandAmount(lineCollection.lineData.trainExpandStatus[GetIndexFromLength(fromToLength[targetType, 0])]);
    }

    /// <summary>
    /// 확장 비용 및 보상 확인 텍스트 갱신
    /// </summary>
    private void SetCheckTexts()
    {
        string m1 = "", m2 = "", p1 = "", p2 = "";
        if (priceData.IsLargeUnit)
        {
            PlayManager.ArrangeUnit(0, totalPrice, ref m1, ref m2, true);
        }
        else
        {
            PlayManager.ArrangeUnit(totalPrice, 0, ref m1, ref m2, true);
        }

        PlayManager.ArrangeUnit(totalPassengerLow, totalPassengerHigh, ref p1, ref p2, true);

        if (m1.Equals(m2) && m1.Equals(""))
        {
            priceText.text = "가격: 0$";
            passengerText.text = "승객 수 +0명";
        }
        else
        {
            priceText.text = "가격: " + m2 + m1 + "$";
            passengerText.text = "승객 수 +" + p2 + p1 + "명";
        }
        expandAmountInputField.text = expandAmount.ToString();
    }

    /// <summary>
    /// 열차 확장 현황 텍스트 설정
    /// </summary>
    public void SetTrainExpandText()
    {
        if (!priceData.IsLightRail)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Clear();
            for (int j = 0; j < 4; j++)
            {
                stringBuilder.Append(4 + j * 2);
                stringBuilder.Append("량(칸): ");
                stringBuilder.Append(lineCollection.lineData.trainExpandStatus[j]);
                stringBuilder.Append("개\n");
            }
            trainExpandStatusText.text = stringBuilder.ToString();
        }
    }

    /// <summary>
    /// 열차 칸 수로 인덱스 계산
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private int GetIndexFromLength(int num)
    {
        return (num - 4) / 2;
    }

    /// <summary>
    /// 열차 길이 데이터 조정 적용
    /// </summary>
    private void ApplyExpandTrain()
    {
        lineCollection.lineData.trainExpandStatus[GetIndexFromLength(fromToLength[targetType, 0])] -= expandAmount;
        lineCollection.lineData.trainExpandStatus[GetIndexFromLength(fromToLength[targetType, 1])] += expandAmount;
    }

    /// <summary>
    /// 최종 가격 및 보상 계산
    /// </summary>
    private void SetTotalPrice()
    {
        totalPrice = priceData.TrainExpandPrice * ((ulong)(fromToLength[targetType, 1] - fromToLength[targetType, 0]) / 2) * (ulong)expandAmount;
        totalPassengerLow = priceData.TrainExapndPassenger * ((ulong)(fromToLength[targetType, 1] - fromToLength[targetType, 0]) / 2) * (ulong)expandAmount;
        totalPassengerHigh = 0;
        MoneyUnitTranslator.Arrange(ref totalPassengerLow, ref totalPassengerHigh);
    }
    /// <summary>
    /// 확장 성공 메시지 출력
    /// </summary>
    /// <param name="expand"></param>
    private void ExpandSuccess(int expand)
    {
        messageManager.ShowMessage("차량 " + expand + "대를 확장하였습니다.");
    }
}
