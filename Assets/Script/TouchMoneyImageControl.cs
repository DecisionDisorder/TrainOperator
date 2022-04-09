using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TouchMoneyImageControl : MonoBehaviour {

	public Image peopleImage;

    public Sprite[] lessPeopleSprite;
    public Sprite[] manyPeopleSprite;

    public Animation adEffectAni;
    public ItemManager itemManager;

    public int currentIndex = 0;

    private void Start()
    {
        StartCoroutine(Timer(1.0f));
    }

    IEnumerator Timer(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (TouchEarning.passengerRandomFactor >= 140)
        {
            delay = 0.5f;
            peopleImage.sprite = manyPeopleSprite[GetNextIndex()];
            adEffectAni.Play();
        }
        else if (TouchEarning.passengerRandomFactor >= 100 && TouchEarning.passengerRandomFactor < 140)//터치당돈이 기준값보다 높을때
        {
            delay = 0.8f;
            peopleImage.sprite = manyPeopleSprite[GetNextIndex()];
            adEffectAni.Stop();
            peopleImage.color = Color.white;
        }
        else if (TouchEarning.passengerRandomFactor < 100)
        {
            delay = 1.2f;
            peopleImage.sprite = lessPeopleSprite[GetNextIndex()];
            adEffectAni.Stop();
            peopleImage.color = Color.white;
        }

        StartCoroutine(Timer(delay));
    }

    private int GetNextIndex()
    {
        currentIndex = currentIndex == 0 ? 1 : 0;
        return currentIndex;
    }
}
