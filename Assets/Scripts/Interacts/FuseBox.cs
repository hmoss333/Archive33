using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseBox : InteractObject
{
    public static FuseBox instance;

    [SerializeField] List<Fuse> fuses;
    Renderer renderer;
    bool isBroken;
    AudioSource audioSource;
    [SerializeField] AudioClip fuseClip;
    [SerializeField] AudioClip outageClip;

    public void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        renderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = fuseClip;
    }

    public override void Update()
    {
        base.Update();

        renderer.material.color = isBroken ? Color.red : Color.green;
    }

    public void SetBroken()
    {
        isBroken = true;

        for (int i = 0; i < 2; i++)
        {
            int randFuse = Random.Range(0, 3);
            fuses[randFuse].SetBroken();
        }

        audioSource.PlayOneShot(outageClip);
    } 

    public override void Interact()
    {
        base.Interact();
        for (int i = 0; i < fuses.Count; i++)
        {
            if (fuses[i].isBroken)
                break;
        }

        isBroken = false; //should trigger if all fuses are currently not broken
        audioSource.PlayOneShot(fuseClip);
        GameplayController.instance.RestartPower();
    }
}
