using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseBox : InteractObject
{
    public static FuseBox instance;

    [SerializeField] Transform focusPoint;
    bool interacting = false;

    [SerializeField] GameObject light;
    [SerializeField] Fuse[] fuses;
    [SerializeField] Fuse selectedFuse;
    int fuseIndex = 0;
    [SerializeField] SpriteRenderer arrowUp, arrowDown;

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

        fuses = GetComponentsInChildren<Fuse>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = fuseClip;
    }

    public override void Update()
    {
        base.Update();

        arrowUp.gameObject.SetActive(interacting);
        arrowDown.gameObject.SetActive(interacting);
        light.GetComponent<Renderer>().material.color = isBroken ? Color.red : Color.green;

        if (interacting)
        {
            PlayerController.instance.SetState(PlayerController.States.interacting);

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                fuseIndex--;
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                fuseIndex++;
            }

            fuseIndex = Mathf.Clamp(fuseIndex, 0, 2);
            selectedFuse = fuses[fuseIndex];
            selectedFuse.highlighted = true;
            Renderer R = selectedFuse.GetComponent<Renderer>();
            Outline OL = R.GetComponent<Outline>();
            if (OL == null) // if no script is attached, attach one
            {
                OL = R.gameObject.AddComponent<Outline>();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                selectedFuse.Interact();
                CheckFuses();
            }
        }

        foreach (Fuse fuse in fuses)
        {
            fuse.GetComponent<BoxCollider>().enabled = interacting;
        }
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

    public void SetFixed()
    {
        isBroken = false;

        for (int i = 0; i < 2; i++)
        {
            fuses[i].SetFixed();
        }
    }

    public override void Interact()
    {
        base.Interact();
        interacting = !interacting;
        DialogueController.instance.UpdateText(string.Empty, false);

        if (interacting)
        {
            CamFocusController.instance.FocusTarget(focusPoint);
        }
        else
        {
            CamFocusController.instance.FocusReset();
        }
    }

    void CheckFuses()
    {
        for (int i = 0; i < fuses.Length; i++)
        {
            if (fuses[i].isBroken)
                return;
        }

        isBroken = false; //should trigger if all fuses are currently not broken
        audioSource.PlayOneShot(fuseClip);
        GameplayController.instance.RestartPower();
    }

    public void InitializeFuseBox()
    {
        interacting = false;
        SetFixed();
    }
}
