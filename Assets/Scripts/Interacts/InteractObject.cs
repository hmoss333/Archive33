using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObject : MonoBehaviour
{
    public bool highlighted; //testing
    public bool interacting;

    // Update is called once per frame
    public virtual void Update()
    {
        highlighted = GetComponent<Outline>();

        if (PlayerController.instance.interactObj != this || PlayerController.instance.state == PlayerController.States.interacting
            && highlighted == true)
        {
            Destroy(GetComponent<Outline>());
        }
    }

    public virtual void Interact()
    {
        print($"Interacted with {this.gameObject.name}");
    }
}
