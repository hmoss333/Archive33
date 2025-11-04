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
    public string documentText;


    public void InitializeDoc()
    {
        Array enumValues = FileColor.GetValues(typeof(FileColor));
        int randomIndex = UnityEngine.Random.Range(0, enumValues.Length);
        FileColor randomColor = (FileColor)enumValues.GetValue(randomIndex);
        fileColor = randomColor;

        int randVal = (int)UnityEngine.Random.Range(0, 5);
        toBeShredded =
            randVal == 0
                ? true
                : false;

        if (GameplayController.instance.shiftNum >= 2)//3)
        {
            int maxCorruptPercent = GameplayController.instance.shiftNum >= 3 ? 4 : 5;
            int randVal_Corrupted = (int)UnityEngine.Random.Range(0, maxCorruptPercent);
            corrupted =
                randVal_Corrupted == 0
                    ? true
                    : false;
        }
        else
        {
            corrupted = false;
        }

        documentText = GameplayController.instance.GetDocumentText(corrupted);
    }
}
