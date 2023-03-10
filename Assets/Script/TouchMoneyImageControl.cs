using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 배경 승객 이미지 관리 클래스
/// </summary>
public class TouchMoneyImageControl : MonoBehaviour {

    /// <summary>
    /// 승강장 승객 이미지
    /// </summary>
	public Image peopleImage;

    /// <summary>
    /// 사람이 적을 때의 이미지 스프라이트 리소스 배열
    /// </summary>
    public Sprite[] lessPeopleSprite;
    /// <summary>
    /// 사람이 많을 때의 이미지 스프라이트 리소스 배열
    /// </summary>
    public Sprite[] manyPeopleSprite;

    /// <summary>
    /// 컬러 카드 아이템 애니메이션 효과
    /// </summary>
    public Animation adEffectAni;
    public ItemManager itemManager;

    /// <summary>
    /// 현재 표시중인 스프라이트의 인덱스
    /// </summary>
    public int currentIndex = 0;

    private void Start()
    {
        StartCoroutine(Timer(1.0f));
    }

    /// <summary>
    /// 반복하여 승객 이미지를 교체해주는 코루틴
    /// </summary>
    /// <param name="delay">교체 딜레이</param>
    IEnumerator Timer(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 기준치 140% 이상의 승객 수일 때 (아이템 효과 적용)
        if (TouchEarning.PassengerRandomFactor >= 140)
        {
            delay = 0.5f;
            peopleImage.sprite = manyPeopleSprite[GetNextIndex()];
            adEffectAni.Play();
        }
        // 기준치 100~140% 의 승객 수 일 때 (많음 이미지 적용)
        else if (TouchEarning.PassengerRandomFactor >= 100 && TouchEarning.PassengerRandomFactor < 140)
        {
            delay = 0.8f;
            peopleImage.sprite = manyPeopleSprite[GetNextIndex()];
            adEffectAni.Stop();
            peopleImage.color = Color.white;
        }
        // 기준치 100% 미만의 승객 수 일 때 (적음 이미지 적용)
        else if (TouchEarning.PassengerRandomFactor < 100)
        {
            delay = 1.2f;
            peopleImage.sprite = lessPeopleSprite[GetNextIndex()];
            adEffectAni.Stop();
            peopleImage.color = Color.white;
        }

        StartCoroutine(Timer(delay));
    }
    /// <summary>
    /// 다음 스프라이트 인덱스 계산
    /// </summary>
    /// <returns>적용할 스프라이트 인덱스</returns>
    private int GetNextIndex()
    {
        currentIndex = currentIndex == 0 ? 1 : 0;
        return currentIndex;
    }
}
