using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineStatus : MonoBehaviour
{
    public LineCollection lineCollection;
    public PriceData priceData;

    /// <summary>
    /// 상태 정보 텍스트 
    /// </summary>
    public Text[] statusTexts;
    /// <summary>
    /// 총합 진행도 슬라이더
    /// </summary>
    public Slider progressSlider;
    /// <summary>
    /// 슬라이더의 Fill 이미지
    /// </summary>
    public Image progressSliderFillImg;
    /// <summary>
    /// 노선 색상
    /// </summary>
    public Color lineColor;

    public UpdateDisplay statusUpdateDisplay;
    public LightRailControlManager lightRailControlManager;

    private void Start()
    {
        statusUpdateDisplay.onEnableUpdate += SetStatus;
    }

    /// <summary>
    /// 상태 정보 설정
    /// </summary>
    private void SetStatus()
    {
        string lowUnit = "", highUnit = "";

        #region 시간형 수익
        ulong timeEarning = 0;
        for (int i = 0; i < lineCollection.lineData.connected.Length; i++)
            timeEarning += lineCollection.lineData.connected[i] ? priceData.ConnectTimeMoney[i] : 0;
        if (!priceData.ConnectTMLargeUnit)
        {
            PlayManager.ArrangeUnit(timeEarning, 0, ref lowUnit, ref highUnit, false);
            statusTexts[0].text = lowUnit + "$";
        }
        else
        {
            PlayManager.ArrangeUnit(0, timeEarning, ref lowUnit, ref highUnit, false);
            if(highUnit.Equals(""))
                statusTexts[0].text = "0$";
            else
                statusTexts[0].text = highUnit + "$";
        }
        #endregion
        #region 터치형 수익
        ulong touchEarningLow = MoneyUnitTranslator.GetSumOfIncreasing((ulong)lineCollection.lineData.numOfTrain, priceData.TrainPassenger, priceData.TrainPassengerAdd), touchEarningHigh = 0;
        int expandAmount = 0;
        if (!priceData.IsLightRail)
        {
            expandAmount = lineCollection.GetTrainExpandAmount();
            touchEarningLow += (ulong)expandAmount * lineCollection.expandTrain.priceData.TrainExapndPassenger;
        }
        MoneyUnitTranslator.Arrange(ref touchEarningLow, ref touchEarningHigh);
        PlayManager.ArrangeUnit(touchEarningLow, touchEarningHigh, ref lowUnit, ref highUnit, false);
        if(highUnit.Equals(""))
            statusTexts[1].text = lowUnit + "$";
        else
            statusTexts[1].text = highUnit + "$";
        #endregion
        #region 진행률
        int[] progresses = new int[6];
        if (!priceData.IsLightRail)
        {
            progresses[1] = lineCollection.lineData.numOfTrain > 100 ? 100 : lineCollection.lineData.numOfTrain;
            progresses[4] = 100 * expandAmount / 300 > 100 ? 100 : 100 * expandAmount / 300;
        }
        else
        {
            progresses[1] = lineCollection.lineData.numOfTrain > 25 ? 25 : lineCollection.lineData.numOfTrain * 4;
            progresses[4] = 100 * lightRailControlManager.GetTotalLevel((int)lineCollection.line) / 25;
        }
        progresses[2] = 100 * lineCollection.GetExpandedAmount() / lineCollection.lineData.sectionExpanded.Length;
        progresses[3] = 100 * lineCollection.GetConnectionAmount() / lineCollection.lineData.connected.Length;
        progresses[5] = 100 * lineCollection.GetScreendoorAmount() / lineCollection.lineData.installed.Length;
        progresses[0] = (progresses[1] + progresses[2] + progresses[3] + progresses[4] + progresses[5]) / 5;
        statusTexts[2].text = progresses[1] + "%";
        statusTexts[3].text = progresses[2] + "%";
        statusTexts[4].text = progresses[3] + "%";
        statusTexts[5].text = progresses[4] + "%";
        statusTexts[6].text = progresses[5] + "%";
        progressSlider.value = progresses[0];
        #endregion

        for(int i = 0; i < statusTexts.Length; i++)
        {
            if (lineCollection.IsExpanded())
                progressSliderFillImg.color = statusTexts[i].color = lineColor;
            else
                progressSliderFillImg.color = statusTexts[i].color = Color.gray;
        }
    }
}
