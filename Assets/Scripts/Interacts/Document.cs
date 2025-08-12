using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Document
{
    public enum FileColor { Red, Blue, Yellow }
    public FileColor fileColor;
    public bool toBeShredded { get; private set; }
    public bool corrupted { get; private set; }
    [SerializeField] Material defaultMat, corruptedMat;

    public void InitializeDoc()
    {
        Array enumValues = FileColor.GetValues(typeof(FileColor));
        int randomIndex = UnityEngine.Random.Range(0, enumValues.Length);
        FileColor randomColor = (FileColor)enumValues.GetValue(randomIndex);
        fileColor = randomColor;


        int randVal = (int)UnityEngine.Random.Range(0, 2);
        toBeShredded =
            randVal == 0
                ? false
                : true;

        if (GameplayController.instance.shiftNum > 3)
        {
            int randVal_Corrupted = (int)UnityEngine.Random.Range(0, 2);
            corrupted =
                randVal_Corrupted == 0
                    ? false
                    : true;
        }
        else
        {
            corrupted = false;
        }

        //TODO: Modify document material and text based on values
        UpdateDocVisuals(fileColor, corrupted);
    }

    void UpdateDocVisuals(FileColor color, bool isCorrupted)
    {

    }
}
