using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painting : InteractObject
{
    [SerializeField] GameObject curtain;
    [SerializeField] float waitTimer, killTimer;
    AudioSource audioSource;
    [SerializeField] AudioClip whisperClip;
    bool covered;


    private void Start()
    {
        covered = true;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = whisperClip;
        audioSource.loop = true;
    }

    private void Update()
    {
        if (covered)
        {
            audioSource.Stop();
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                covered = false;
                waitTimer = 8f;
                killTimer = 13f;
            }
        }
        else
        {
            audioSource.Play();
            killTimer -= Time.deltaTime;
            if (killTimer <= 0)
            {
                GameplayController.instance.SetState(GameplayController.State.death);
            }
        }
    }

    public override void Interact()
    {
        base.Interact();
        covered = true;
        waitTimer = 8f;
        killTimer = 13f;
    }
}
