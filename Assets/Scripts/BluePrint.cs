using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class BluePrint
{
    public string itemName { get; private set; }
    public string Req1 { get; private set; }
    public string Req2 { get; private set; }
    public int Req1amount { get; private set; }
    public int Req2amount { get; private set; }
    public int numOfRequirements { get; private set; }

    public BluePrint(string name, int reqNum, string R1, int R1num, string R2, int R2num)
    {
        if (string.IsNullOrEmpty(name)) throw new System.ArgumentException("Item name cannot be null or empty");
        if (reqNum < 1 || reqNum > 2) throw new System.ArgumentException("Number of requirements must be 1 or 2");
        if (string.IsNullOrEmpty(R1)) throw new System.ArgumentException("First requirement cannot be null or empty");
        if (R1num <= 0) throw new System.ArgumentException("First requirement amount must be positive");
        
        itemName = name;
        numOfRequirements = reqNum;
        Req1 = R1;
        Req2 = R2;
        Req1amount = R1num;
        Req2amount = R2num;
    }
}
