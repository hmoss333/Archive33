using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UIOverlayController : MonoBehaviour
{
    InteractObject baseInteract;
    [SerializeField] Image uiImage;
    [SerializeField] string playerPrefTag;
    int playerPrefVal;


    // Start is called before the first frame update
    void Start()
    {
        baseInteract = GetComponentInParent<InteractObject>();
        playerPrefVal = PlayerPrefs.GetInt(playerPrefTag, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!baseInteract.interacting && playerPrefVal == 0)
        {
            if (baseInteract.highlighted && uiImage.color.a <= 1)
            {
                FadeInUI();
            }
            else if (!baseInteract.highlighted && uiImage.color.a >= 0)
            {
                FadeOutUI();
            }
        }

        else
        {
            playerPrefVal = 1;
            PlayerPrefs.SetInt(playerPrefTag, playerPrefVal);
        }

        if (playerPrefVal == 1)
        {
            FadeOutUI();
        }
    }

    void FadeInUI()
    {
        float alpha = uiImage.color.a;
        alpha += 1.5f * Time.deltaTime;
        uiImage.color = new Color(uiImage.color.r, uiImage.color.g, uiImage.color.b, alpha);
    }

    void FadeOutUI()
    {
        float alpha = uiImage.color.a;
        alpha -= Time.deltaTime;
        uiImage.color = new Color(uiImage.color.r, uiImage.color.g, uiImage.color.b, alpha);
    }
}
