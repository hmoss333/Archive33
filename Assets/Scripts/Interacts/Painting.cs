using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painting : InteractObject
{
    [SerializeField] Material defaultMat, effectedMat;
    [SerializeField] float waitTimer, killTimer;
    AudioSource audioSource;
    [SerializeField] AudioClip whisperClip;
    bool effected;
    Renderer renderer;

    

    private void Start()
    {
        effected = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = whisperClip;
        audioSource.loop = true;
        renderer = GetComponentInChildren<Renderer>();
    }

    private void Update()
    {
        if (GameplayController.instance.state == GameplayController.State.gameplay)
        {
            if (!renderer.isVisible && !effected)
            {
                audioSource.Stop();
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0)
                {
                    effected = true;
                    waitTimer = 8f;
                    killTimer = 13f;
                }
            }

            if (effected)
            {
                audioSource.Play();
                killTimer -= Time.deltaTime;
                if (killTimer <= 0)
                {
                    GameplayController.instance.Suffocate();//.SetState(GameplayController.State.death);
                }
            }

            ModifyMaterials(renderer, effected ? effectedMat : defaultMat);
        }
    }

    public override void Interact()
    {
        base.Interact();

        if (effected)
        {
            effected = false;
            waitTimer = 8f;
            killTimer = 13f;
        }
    }

    void ModifyMaterials (Renderer rend, Material mat)
    {
        Material[] tempMats = rend.materials;

        tempMats[1] = mat;

        rend.materials = tempMats;
    }
}
