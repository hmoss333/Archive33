using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Document
{
    public bool stamped { get; private set; }
    public bool toBeShredded { get; private set; }
    public bool corrupted { get; private set; }

    public void InitializeDoc()
    {
        stamped = false;
        int randVal = (int)Random.Range(0, 2);
        //corrupted = GameplayController.instance.powerOutage;
        //toBeShredded = corrupted
        //    ? true
        //    : randVal == 0
        //        ? false
        //        : true;
        toBeShredded = true; //for testing
    }

    public void Stamp()
    {
        stamped = true;
    }
}
