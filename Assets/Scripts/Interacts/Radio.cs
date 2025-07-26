using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Radio : InteractObject
{
    public static Radio instance;

    [SerializeField] GameObject dialObj;
    [SerializeField] List<RadioStation> radioStations;

    [SerializeField][Range(0, 25)] float currentFrequency;
    [SerializeField] TMP_Text radioText;
    public float targetFrequency { get; private set; } //public in order to display frequency on document
    [SerializeField] float rotateSpeed;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip staticAudio, correctAudio;
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

    private void OnDisable()
    {
        currentFrequency = 0f;
        radioText.text = currentFrequency.ToString("F2");
    }

    public void InitializeFrequency()
    {
        radioStations.Clear();

        RadioStation newStation = new RadioStation();
        targetFrequency = Random.Range(0f, 25f);
        newStation.frequency = targetFrequency;
        newStation.clip = correctAudio;
        radioStations.Add(newStation);

        for (int i = 0; i < badAudio.Count; i++)
        {
            RadioStation newBadStation = new RadioStation();
            float randomFrequency = Random.Range(0f, 25f);
            newBadStation.frequency = randomFrequency;
            newBadStation.clip = badAudio[i];
            radioStations.Add(newBadStation);
        }
    }

    public override void Update()
    {
        base.Update();

        radioText.text = currentFrequency.ToString("F2");

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
        }

        for (int i = 0; i < radioStations.Count; i++)
        {
            if (currentFrequency <= radioStations[i].frequency + 0.5f && currentFrequency >= radioStations[i].frequency - 0.5f)
            {
                if (radioStations[i].frequency <= targetFrequency + 0.5f && radioStations[i].frequency >= targetFrequency - 0.5f)
                {
                    if (PlayerController.instance.hasDocument && interacting)
                    {
                        //Correct station
                        DialogueController.instance.UpdateText(
                            PlayerController.instance.GetCurrentDocument().toBeShredded
                                ? "This document should be shredded"
                                : "This document should be filed", true);
                    }
                    if (audioSource.clip != correctAudio)
                    {
                        audioSource.Stop();
                        audioSource.clip = correctAudio;
                        audioSource.Play();
                    }
                    break;
                }
                else
                {
                    GameplayController.instance.LoseSanity();
                    if (interacting)
                    {
                        //Bad station
                        DialogueController.instance.UpdateText("[TODO]: Bad audio", true);
                    }
                    if (audioSource.clip != radioStations[i].clip)
                    {
                        audioSource.Stop();
                        audioSource.clip = radioStations[i].clip;
                        audioSource.Play();
                    }
                    break;
                }
            }
            else
            {
                if (interacting)
                {
                    //Static
                    DialogueController.instance.UpdateText(".........", true);
                }
                if (audioSource.clip != staticAudio)
                {
                    audioSource.Stop();
                    audioSource.clip = staticAudio;
                    audioSource.Play();
                }
            }
        }
    }

    public override void Interact()
    {
        base.Interact();
        interacting = !interacting;       
    }
}


[System.Serializable]
class RadioStation
{
    public float frequency;
    public AudioClip clip;
}
