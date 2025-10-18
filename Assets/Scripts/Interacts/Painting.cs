using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        waitTimer = 12f;
        killTimer = 20f;
        effected = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = whisperClip;
        audioSource.loop = true;
        renderer = GetComponentInChildren<Renderer>();
    }

    private void FixedUpdate()
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
                    waitTimer = 12f;
                    killTimer = 20f;
                }
            }

            if (effected)
            {
                if (!renderer.sharedMaterials.Contains(effectedMat)) { ModifyMaterials(renderer, effectedMat); }
                if (!audioSource.isPlaying) { audioSource.PlayOneShot(whisperClip); }
                killTimer -= Time.deltaTime;
                if (killTimer <= 0)
                {
                    GameplayController.instance.Suffocate();
                }
            }
        }
    }

    public override void Interact()
    {
        base.Interact();

        ModifyMaterials(renderer, defaultMat);
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
        List<Material> matList = new List<Material>();
        for (int i = 0; i < 2; i++)
        {
            matList.Add(tempMats[i]);
        }

        matList[1] = mat;

        rend.materials = matList.ToArray();
    }
}
