using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : InteractObject
{
    public static Radio instance;

    [SerializeField] GameObject dialObj;

    [SerializeField] float currentFrequency;
    [SerializeField] float targetFrequency;
    [SerializeField] float maxFrequency = 100f;
    [SerializeField] float rotateSpeed;
    [SerializeField] AudioSource staticAudio;

    bool interacting;
    [SerializeField] bool foundStation;


    public override void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        base.Start();
        interacting = false;
        foundStation = false;
        InitializeFrequency();
    }

    public void InitializeFrequency()
    {
        targetFrequency = Random.Range(0, maxFrequency);
    }

    public override void Update()
    {
        base.Update();

        currentFrequency = Mathf.Clamp(currentFrequency, 0, maxFrequency);

        //TODO
        //Add logic to have the player tune the radio to a randomized station value in order to get the instructions for the current document
        if (interacting)
        {
            PlayerController.instance.SetState(PlayerController.States.interacting);

            if (Input.GetAxis("Mouse X") > 0)
            {
                currentFrequency += Time.deltaTime * 2f * rotateSpeed;
                dialObj.transform.Rotate(Vector3.up * Time.deltaTime * -rotateSpeed);
            }
            else if (Input.GetAxis("Mouse X") < 0)
            {
                currentFrequency -= Time.deltaTime * 2f * rotateSpeed;
                dialObj.transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
            }

            if (currentFrequency <= targetFrequency + 7.5f && currentFrequency >= targetFrequency - 7.5f)
            {
                foundStation = true;
            }
            else
            {
                foundStation = false;
            }

            if (foundStation && PlayerController.instance.hasDocument)
            {
                if (PlayerController.instance.GetCurrentDocument().toBeShredded)
                    DialogueController.instance.UpdateText("This document should be shredded");
                else
                    DialogueController.instance.UpdateText("This document should be sent out");
            }
            else
            {
                DialogueController.instance.UpdateText(".........");
            }
        }
    }

    public override void Interact()
    {
        base.Interact();
        interacting = !interacting;       
    }
}
