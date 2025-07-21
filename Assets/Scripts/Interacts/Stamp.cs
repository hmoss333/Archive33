using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamp : InteractObject
{
    public override void Interact()
    {
        base.Interact();
        if (PlayerController.instance.GetCurrentDocument() != null)
        {
            PlayerController.instance.GetCurrentDocument().Stamp();
            DialogueController.instance.UpdateText("Document approved for filing");
        }
    }
}
