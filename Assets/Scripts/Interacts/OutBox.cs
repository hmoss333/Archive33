using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutBox : InteractObject
{
    public override void Interact()
    {
        base.Interact();
        if (PlayerController.instance.hasDocument)
        {
            if (PlayerController.instance.GetCurrentDocument().stamped)
            {
                if (!PlayerController.instance.GetCurrentDocument().toBeShredded)
                {
                    DialogueController.instance.UpdateText("Good job");
                    GameplayController.instance.Success();
                }
                else
                {
                    DialogueController.instance.UpdateText("That one should not have been sent out");
                    GameplayController.instance.Failure();
                }
                PlayerController.instance.RemoveCurrentDocument();
            }
            else
            {
                DialogueController.instance.UpdateText("This document needs a stamp");
            }
        }
    }
}
