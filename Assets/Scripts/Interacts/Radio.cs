using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : InteractObject
{
    [SerializeField] float currentFrequency;
    [SerializeField] float targetFrequency;
    [SerializeField] float rotateSpeed;
    [SerializeField] AudioSource staticAudio;

    public override void Update()
    {
        base.Update();
        //TODO
        //Add logic to have the player tune the radio to a randomized station value in order to get the instructions for the current document
    }

    public override void Interact()
    {
        base.Interact();

        if (PlayerController.instance.hasDocument)
        {
            if (PlayerController.instance.GetCurrentDocument().toBeShredded)
                DialogueController.instance.UpdateText("This document should be shredded");
            else
                DialogueController.instance.UpdateText("This document should be sent out");
        }
        else
        {
            DialogueController.instance.UpdateText("Pick up a new document");
        }
    }
}
