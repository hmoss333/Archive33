using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    public static DialogueController instance;

    [SerializeField] TMP_Text textUI;
    [SerializeField] float fadeTime = 3.5f;
    float timer;
    bool fade;

    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        fade = false;
        textUI.SetText(string.Empty);
    }

    private void Update()
    {
        if (fade && textUI.text != string.Empty)
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

    public void UpdateText(string newText, bool fadeVal)
    {
        textUI.text = newText;
        fade = fadeVal;
        print(newText);
    }
}
