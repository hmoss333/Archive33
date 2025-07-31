using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : InteractObject
{
    [SerializeField] Light light;
    private bool isOn;
    AudioSource audioSource;
    [SerializeField] AudioClip lightToggleClip;

    public void Start()
    {
        isOn = true;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = lightToggleClip;
    }

    public override void Update()
    {
        light.enabled = isOn;
    }

    public override void Interact()
    {
        base.Interact();
        isOn = !isOn;
        audioSource.PlayOneShot(lightToggleClip);
    }
}
