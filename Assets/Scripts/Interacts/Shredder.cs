using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shredder : InteractObject
{
    AudioSource audioSource;
    [SerializeField] AudioClip shredClip;


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
            if (PlayerController.instance.GetCurrentDocument().toBeShredded)
            {
                DialogueController.instance.UpdateText("Document destroyed", true);
                GameplayController.instance.Success();
            }
            else
            {
                DialogueController.instance.UpdateText("That file should not have been shredded...", true);
                GameplayController.instance.Failure();
            }
            audioSource.PlayOneShot(shredClip);
            PlayerController.instance.RemoveCurrentDocument();
        }
    }
}
