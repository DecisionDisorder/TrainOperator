using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CompanyData
{
    public int reputationTotalValue;
    public int additionalPercentage;
    public int reputationPercentage;

    public int averageSanitoryCondition;
    
    public int peaceValue;
    public int peaceCoolTime;
    public int sanitoryCoolTime;

    public int valuePoint;
    public int numOfSuccess;

    public int inspectSanitoryTime;

    public CompanyData()
    {
        reputationPercentage = 100;
        reputationPercentage = 10;
        averageSanitoryCondition = 20;
        peaceValue = 20;
        peaceCoolTime = 5;
        sanitoryCoolTime = 5;
        inspectSanitoryTime = 240;
    }
}
