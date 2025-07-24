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
    }
}
