using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : InteractObject
{
    public static Radio instance;

    [SerializeField] GameObject dialObj;
    [SerializeField] List<RadioStation> radioStations;

    [SerializeField] float currentFrequency;
    public float targetFrequency { get; private set; } //used to write frequency to document
    [SerializeField] float maxFrequency = 100f;
    [SerializeField] float rotateSpeed;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip staticAudio, correctAudio;
    [SerializeField] List<float> badFrequencies;
    [SerializeField] List<AudioClip> badAudio;

    bool interacting;


    public void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        interacting = false;
        audioSource.clip = staticAudio;
        audioSource.loop = true;
        audioSource.Play();
        InitializeFrequency();
    }

    public void InitializeFrequency()
    {
        radioStations.Clear();

        RadioStation newStation = new RadioStation();
        targetFrequency = Random.Range(0, maxFrequency);
        newStation.frequency = targetFrequency;
        newStation.clip = correctAudio;
        radioStations.Add(newStation);

        for (int i = 0; i < badAudio.Count; i++)
        {
            RadioStation newBadStation = new RadioStation();
            float randomFrequency = Random.Range(0, maxFrequency);
            newBadStation.frequency = randomFrequency;
            newBadStation.clip = badAudio[i];
            radioStations.Add(newBadStation);
        }
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


            //Check Bad Station match
            //Check Good station match
            //Else play static


            for (int i = 0; i < radioStations.Count; i++)
            {
                if (currentFrequency <= radioStations[i].frequency + 2.5f && currentFrequency >= radioStations[i].frequency - 2.5f)
                {
                    if (radioStations[i].frequency <= targetFrequency + 2.5f && radioStations[i].frequency >= targetFrequency - 2.5f)
                    {
                        //Correct station
                        DialogueController.instance.UpdateText(
                            PlayerController.instance.GetCurrentDocument().toBeShredded
                                ? "This document should be shredded"
                                : "This document should be filed", true);
                        if (audioSource.clip != correctAudio)
                        {
                            audioSource.Stop();
                            audioSource.clip = correctAudio;
                            audioSource.Play();
                            print("Set correct audio");
                        }
                        break;
                    }
                    else
                    {
                        //Bad station
                        GameplayController.instance.BadRadioStation();
                        DialogueController.instance.UpdateText("[TODO]: bad audio", true);
                        if (audioSource.clip != radioStations[i].clip)
                        {
                            audioSource.Stop();
                            audioSource.clip = radioStations[i].clip;
                            audioSource.Play();
                            print("Set bad audio");
                        }
                        break;
                    }
                }
                else
                {
                    //Static
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


[System.Serializable]
class RadioStation
{
    public float frequency;
    public AudioClip clip;
}
