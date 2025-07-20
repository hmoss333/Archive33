using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObject : MonoBehaviour
{
    Renderer renderer;
    public bool highlighted; //testing

    // Start is called before the first frame update
    public virtual void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //testing
        renderer.material.color = highlighted ? Color.green : Color.red;
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
