using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : InteractObject
{
    [SerializeField] Light light;
    public bool isOn;// { get; private set; }

    [SerializeField] bool blownOut;
    [SerializeField] float blowOutTime = 10f;
    [SerializeField] float rechargeTime = 15f;
    float timer;


    public override void Start()
    {
        base.Start();
        isOn = false;
    }

    public override void Update()
    {
        base.Update();

        if (isOn)
        {
            timer += Time.deltaTime;
            if (timer >= blowOutTime)
            {
                blownOut = true;
                timer = 0f;
            }
        }

        if (blownOut)
        {
            isOn = false;
            timer += Time.deltaTime;
            if (timer >= rechargeTime)
            {
                blownOut = false;
                timer = 0f;
            }
        }

        if (!isOn && !blownOut)
        {
            timer = 0f;
        }

        light.enabled = isOn;
    }

    public override void Interact()
    {
        base.Interact();
        isOn = !isOn;
    }
}
