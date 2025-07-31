using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutBox : InteractObject
{
    [SerializeField] enum FileColor { Red, Blue, Yellow }
    [SerializeField] FileColor fileColor;
    AudioSource audioSource;
    [SerializeField] AudioClip fileClip;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = fileClip;
    }

    public override void Interact()
    {
        base.Interact();
        if (PlayerController.instance.hasDocument)
        {
            if (!PlayerController.instance.GetCurrentDocument().toBeShredded
                && PlayerController.instance.GetCurrentDocument().fileColor.ToString() == fileColor.ToString())
            {
                DialogueController.instance.UpdateText("Document filed", true);
                GameplayController.instance.Success();
            }
            else
            {
                DialogueController.instance.UpdateText("That one should not have been sent out...", true);
                GameplayController.instance.Failure();
            }
            audioSource.PlayOneShot(fileClip);
            PlayerController.instance.RemoveCurrentDocument();
        }
    }
}
