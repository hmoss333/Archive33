using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Painting : InteractObject
{
    [SerializeField] Transform focusPoint;
    [SerializeField] Material defaultMat, effectedMat;
    [SerializeField] float waitTimer, killTimer;
    AudioSource audioSource;
    [SerializeField] AudioClip whisperClip;
    bool effected;
    Renderer rd;

    

    private void Start()
    {
        waitTimer = 12f;
        killTimer = 20f;
        effected = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = whisperClip;
        audioSource.loop = true;
        rd = GetComponentInChildren<Renderer>();
    }

    private void FixedUpdate()
    {
        if (GameplayController.instance.state == GameplayController.State.gameplay)
        {
            if (!rd.isVisible && !effected)
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
                if (!rd.sharedMaterials.Contains(effectedMat)) { ModifyMaterials(rd, effectedMat); }
                if (!audioSource.isPlaying) { audioSource.PlayOneShot(whisperClip); }
                killTimer -= Time.deltaTime;
                if (killTimer <= 0)
                {
                    PlayerController.instance.SetState(PlayerController.States.interacting);
                    CamFocusController.instance.FocusTarget(focusPoint);
                    GameplayController.instance.Suffocate();
                }
            }
        }
    }

    public override void Interact()
    {
        base.Interact();

        ModifyMaterials(rd, defaultMat);
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
