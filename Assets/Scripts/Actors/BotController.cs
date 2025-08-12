using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BotController : InteractObject
{
    public override void Interact()
    {
        if (isActiveAndEnabled)
        {
            base.Interact();
            //TODO: if the player has a corrupted document they can hand it off to the bot here
            /// If the document is not corrupted, count as a failure (not instant death)
            if (PlayerController.instance.hasDocument)
            {
                if (!PlayerController.instance.GetCurrentDocument().corrupted)
                    GameplayController.instance.Failure();

                PlayerController.instance.RemoveCurrentDocument();
            }
        }
    }
}


