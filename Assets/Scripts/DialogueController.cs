using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    public static DialogueController instance;

    [SerializeField] TMP_Text textUI;
    [SerializeField] float fadeTime = 1.5f;
    float timer;

    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        textUI.SetText(string.Empty);
    }

    private void Update()
    {
        if (textUI.text != string.Empty)
        {
            timer += Time.deltaTime;
            if (timer >= fadeTime)
            {
                timer = 0f;
                textUI.text = string.Empty;
            }
        }
        else
        {
            timer = 0f;
        }
    }

    public void UpdateText(string newText)
    {
        textUI.text = newText;
        print(newText);
    }
}
