using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuse : InteractObject
{
    public bool isBroken { get; private set; }
    Renderer renderer;
    AudioSource audioSource;
    [SerializeField] AudioClip fuseClip;

    public void Start()
    {
        renderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = fuseClip;
    }

    public override void Update()
    {
        renderer.material.color = isBroken ? Color.red : Color.green;
    }

    public override void Interact()
    {
        if (isBroken)
        {
            isBroken = !isBroken;
            audioSource.PlayOneShot(fuseClip);
        }
    }

    public void SetBroken()
    {
        isBroken = true;
    }
}
