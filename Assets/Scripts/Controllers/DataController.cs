using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoBehaviour
{
    public static DataController instance;

    public int shiftNum { get; private set; }
    public int maxShiftNum { get; private set; }
    public int longNightMode { get; private set; }
    public float longNightScore { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        LoadValues();
    }

    void LoadValues()
    {
        shiftNum = PlayerPrefs.GetInt("shiftNum", 0);
        maxShiftNum = PlayerPrefs.GetInt("maxShift", 0);
        longNightMode = PlayerPrefs.GetInt("longNightMode", 0);
        longNightScore = PlayerPrefs.GetFloat("longNightScore", 0f);
    }

    public void SaveValues()
    {
        PlayerPrefs.SetInt("shiftNum", shiftNum);
        PlayerPrefs.SetInt("maxShift", maxShiftNum);
        PlayerPrefs.SetInt("longNightMode", longNightMode);
        PlayerPrefs.SetFloat("longNightScore", longNightScore);
    }

    public void UpdateShiftNum(int num)
    {
        shiftNum = num;
        if (shiftNum > maxShiftNum)
            UpdateMaxShiftNum(num);
    }

    public void UpdateMaxShiftNum(int num)
    {
        maxShiftNum = num;
    }
}
