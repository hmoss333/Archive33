using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shredder : InteractObject
{
    public override void Interact()
    {
        base.Interact();
        if (PlayerController.instance.hasDocument)
        {
            if (PlayerController.instance.GetCurrentDocument().toBeShredded)
            {
                DialogueController.instance.UpdateText("Good job");
                GameplayController.instance.Success();
            }
            else
            {
                DialogueController.instance.UpdateText("That file should not have been shredded");
                GameplayController.instance.Failure();
            }
            PlayerController.instance.RemoveCurrentDocument();
        }
    }
}
