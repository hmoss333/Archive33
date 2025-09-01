using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueController : MonoBehaviour
{
    public static DialogueController instance;

    [SerializeField] TMP_Text textUI;
    [SerializeField] Image background;
    [SerializeField] float fadeTime = 5f;
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
        background.gameObject.SetActive(textUI.text != string.Empty);

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
