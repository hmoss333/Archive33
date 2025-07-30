using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : InteractObject
{
    [SerializeField] Light light;
    private bool isOn;


    public void Start()
    {
        isOn = true;
    }

    public override void Update()
    {
        light.enabled = isOn;
    }

    public override void Interact()
    {
        base.Interact();
        isOn = !isOn;
    }
}
