using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BotController : InteractObject
{
    AudioSource audioSource;
    [SerializeField] AudioClip correctClip, incorrectClip;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        if (TryGetComponent<Outline>(out Outline outline))
        {
            Destroy(outline);
        }
    }

    public override void Interact()
    {
        if (isActiveAndEnabled)
        {
            base.Interact();
            if (PlayerController.instance.hasDocument)
            {
                if (!PlayerController.instance.GetCurrentDocument().corrupted)
                {
                    audioSource.PlayOneShot(incorrectClip);
                    GameplayController.instance.Failure();
                }
                else
                {
                    audioSource.PlayOneShot(correctClip);
                }

                GameplayController.instance.CallBot();
                PlayerController.instance.RemoveCurrentDocument();
            }
        }
    }
}


