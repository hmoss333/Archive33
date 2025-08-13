using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BotController : InteractObject
{
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
                    GameplayController.instance.Failure();
                    GameplayController.instance.CallBot();
                }

                PlayerController.instance.RemoveCurrentDocument();
            }
        }
    }
}


