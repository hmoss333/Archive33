using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallBell : InteractObject
{
    public AudioSource audioSource;
[SerializeField]  public AudioClip sound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
    }

    public override void Update()
    {}

    public override void Interact()
    {
        base.Interact();
        audioSource.PlayOneShot(sound, 0.7F);
    }
}

