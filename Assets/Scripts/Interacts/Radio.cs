using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : InteractObject
{
    public static Radio instance;

    [SerializeField] GameObject dialObj;

    [SerializeField] float currentFrequency;
    public float targetFrequency { get; private set; }
    [SerializeField] List<float> badFrequencies;
    [SerializeField] float maxFrequency = 100f;
    [SerializeField] float rotateSpeed;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip staticAudio, shredAudio, fileAudio;
    [SerializeField] List<AudioClip> badAudio;

    bool interacting;
    [SerializeField] bool foundStation;


    public void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        interacting = false;
        foundStation = false;
        audioSource.clip = staticAudio;
        audioSource.loop = true;
        audioSource.Play();
        InitializeFrequency();
    }

    public void InitializeFrequency()
    {
        targetFrequency = Random.Range(0, maxFrequency);
        GenerateBadFrequencies();
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
                currentFrequency += Time.deltaTime * rotateSpeed;
                dialObj.transform.Rotate(Vector3.up * Time.deltaTime * -rotateSpeed * 10f);
            }
            else if (Input.GetAxis("Mouse X") < 0)
            {
                currentFrequency -= Time.deltaTime * rotateSpeed;
                dialObj.transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed * 10f);
            }

            if (GameplayController.instance.shiftNum < 1)
            {
                if (currentFrequency <= targetFrequency + 5f && currentFrequency >= targetFrequency - 5f)
                {
                    interacting = false;
                    //GameplayController.instance.IncrementPhase();
                }
                else
                {
                    DialogueController.instance.UpdateText(".........", true);
                    if (audioSource.clip != staticAudio)
                    {
                        audioSource.Stop();
                        audioSource.clip = staticAudio;
                        audioSource.Play();
                        print("Set static audio");
                    }
                }
            }
            else
            {
                if (currentFrequency <= targetFrequency + 5f && currentFrequency >= targetFrequency - 5f)
                {
                    foundStation = true;
                }
                else
                {
                    foundStation = false;
                }

                if (GameplayController.instance.shiftNum > 3)
                {
                    foreach (float frequency in badFrequencies)
                    {
                        if (currentFrequency <= frequency + 5f && currentFrequency >= frequency - 5f && audioSource.clip == staticAudio)
                        {
                            int randClip = Random.Range(0, badAudio.Count - 1);
                            audioSource.clip = badAudio[randClip];
                        }
                    }
                }

                if (foundStation && PlayerController.instance.hasDocument)
                {
                    if (PlayerController.instance.GetCurrentDocument().toBeShredded)
                    {
                        DialogueController.instance.UpdateText("This document should be shredded", true);
                        if (audioSource.clip != shredAudio)
                        {
                            audioSource.Stop();
                            audioSource.clip = shredAudio;
                            audioSource.Play();
                            print("Set shred audio");
                        }
                    }
                    else
                    {
                        DialogueController.instance.UpdateText("This document should be sent out", true);
                        if (audioSource.clip != fileAudio)
                        {
                            audioSource.Stop();
                            audioSource.clip = fileAudio;
                            audioSource.Play();
                            print("Set file audio");
                        }
                    }
                }
                else
                {
                    DialogueController.instance.UpdateText(".........", true);
                    if (audioSource.clip != staticAudio)
                    {
                        audioSource.Stop();
                        audioSource.clip = staticAudio;
                        audioSource.Play();
                        print("Set static audio");
                    }
                }
            }
        }
    }

    public override void Interact()
    {
        base.Interact();
        interacting = !interacting;       
    }

    void GenerateBadFrequencies()
    {
        badFrequencies.Clear();

        int randNum = Random.Range(2, 6);
        for (int i = 0; i < randNum; i++)
        {
            float randStation = Random.Range(0, maxFrequency);
            randStation = Mathf.Round(randStation);
            if (!badFrequencies.Contains(randStation))
            {
                badFrequencies.Add(randStation);
            }
            else
            {
                i--;
            }
        }
    }
}
