using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuse : InteractObject
{
    public bool isBroken { get; private set; }
    Renderer renderer;

    public void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    public override void Update()
    {
        renderer.material.color = isBroken ? Color.red : Color.green;
    }

    public override void Interact()
    {
        if (isBroken)
            isBroken = !isBroken;
    }

    public void SetBroken()
    {
        isBroken = true;
    }
}
