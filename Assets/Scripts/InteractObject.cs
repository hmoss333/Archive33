using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObject : MonoBehaviour
{
    public bool highlighted; //testing

    // Update is called once per frame
    public virtual void Update()
    {
        highlighted = false;
    }

    private void FixedUpdate()
    {
        if (!highlighted && GetComponent<Outline>())
        {
            Outline outlineScript = GetComponent<Outline>();
            Destroy(outlineScript);
        }
    }

    public virtual void Interact()
    {
        print($"Put interact logic for {this.gameObject.name} here");
    }
}
