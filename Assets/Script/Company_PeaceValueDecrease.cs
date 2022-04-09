using UnityEngine;
using System.Collections;

public class Company_PeaceValueDecrease : MonoBehaviour {

    int waitTime;
    public Company_Peace_Controller peace_Controller;
    void Start()
    {
        waitTime = SetTime();
        StartCoroutine(Timer(waitTime));
    }

    IEnumerator Timer(float waittime)
    {
        yield return new WaitForSeconds(waittime);

        peace_Controller.PeaceValue -= MinusPeace();
        if (peace_Controller.PeaceValue < -100)
        {
            peace_Controller.PeaceValue = -100;
        }
        peace_Controller.SetInfoText();
        waittime = SetTime();
        StartCoroutine(Timer(waittime));
    }
    
    int SetTime()
    {
        int Timeset;
        if (!LineManager.instance.lineCollections[4].isExpanded())
        {
            Timeset = Random.Range(50, 71);
        }
        else if (!LineManager.instance.lineCollections[9].isExpanded())
        {
            Timeset = Random.Range(45, 61);
        }
        else
        {
            Timeset = Random.Range(40, 51);
        }
        return Timeset;
    }
    int MinusPeace()
    {
        int Score;
        if (!LineManager.instance.lineCollections[4].isExpanded())
        {
            Score = Random.Range(0, 5);
        }
        else if (!LineManager.instance.lineCollections[9].isExpanded())
        {
            Score = Random.Range(2, 7);
        }
        else
        {
            Score = Random.Range(4, 9);
        }
        return Score;
    }
}
