using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIOverlayController : MonoBehaviour
{
    InteractObject baseInteract;
    [SerializeField] Image uiImage;
    //[SerializeField] private bool interacting;


    // Start is called before the first frame update
    void Start()
    {
        baseInteract = GetComponentInParent<InteractObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (baseInteract.highlighted && uiImage.color.a <= 1f)
        {
            float alpha = uiImage.color.a;
            alpha += Time.deltaTime;
            uiImage.color = new Color(uiImage.color.r, uiImage.color.g, uiImage.color.b, alpha);
        }
        else if (!baseInteract.highlighted && uiImage.color.a >= 0f)
        {
            float alpha = uiImage.color.a;
            alpha -= Time.deltaTime;
            uiImage.color = new Color(uiImage.color.r, uiImage.color.g, uiImage.color.b, alpha);
        }
    }
}
