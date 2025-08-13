using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shredder : InteractObject
{
    AudioSource audioSource;
    [SerializeField] AudioClip shredClip, incorrectClip;
    [SerializeField] GameObject shredderOne, shredderTwo;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = shredClip;
    }

    public override void Interact()
    {
        base.Interact();
        if (PlayerController.instance.hasDocument)
        {
            Document currentDoc = PlayerController.instance.GetCurrentDocument();
            if (currentDoc.toBeShredded && !currentDoc.corrupted)
            {
                DialogueController.instance.UpdateText("Document destroyed", true);
                GameplayController.instance.Success();
                audioSource.PlayOneShot(shredClip);
            }
            else
            {
                DialogueController.instance.UpdateText("That file should not have been shredded...", true);
                GameplayController.instance.Failure();
                audioSource.PlayOneShot(incorrectClip);
            }
            PlayerController.instance.RemoveCurrentDocument();
        }
    }
}
