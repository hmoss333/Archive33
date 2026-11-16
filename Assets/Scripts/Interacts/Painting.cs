using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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


    [Header("UI Elements")]
    [SerializeField] Image image;
    [Range(0f, 1f)] public float intensity;

    Material mat;
    static readonly int IntensityId = Shader.PropertyToID("_Intensity");
    static readonly int SeedId = Shader.PropertyToID("_Seed");



    void Awake()
    {
        mat = Instantiate(image.material);   // instance, so the asset isn't edited
        image.material = mat;
        mat.SetFloat(SeedId, Random.value * 100f);
    }

    private void Start()
    {
        waitTimer = 12f;
        killTimer = 20f;
        effected = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = whisperClip;
        audioSource.loop = true;
        rd = GetComponentInChildren<Renderer>();
        intensity = 0f;
    }

    private void FixedUpdate()
    {
        intensity = Mathf.Clamp(intensity, 0f, 1f);
        mat.SetFloat(IntensityId, intensity);

        if (GameplayController.instance.state == GameplayController.State.gameplay)
        {
            if (!rd.isVisible && !effected)
            {
                audioSource.Stop();
                intensity -= 1.5f * Time.deltaTime;

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

                intensity += 0.5f * Time.deltaTime;
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
