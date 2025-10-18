using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObject : MonoBehaviour
{
    public bool highlighted; //testing

    // Update is called once per frame
    public virtual void Update()
    {
        if (!highlighted && GetComponent<Outline>())
        {
            Outline outlineScript = GetComponent<Outline>();
            Destroy(outlineScript);
        }

        highlighted = false;
    }

    public virtual void Interact()
    {
        print($"Interacted with {this.gameObject.name}");
    }
}
