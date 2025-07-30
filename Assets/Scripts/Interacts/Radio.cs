using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Radio : InteractObject
{
    public static Radio instance;

    [SerializeField] GameObject dialObj;
    //[SerializeField] List<RadioStation> radioStations;

    [SerializeField][Range(0, 25)] float currentFrequency;
    [SerializeField] TMP_Text radioText;
    public float targetFrequency { get; private set; } //public in order to display frequency on document
    [SerializeField] float rotateSpeed, focusTime = 3f;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip staticAudio, badAudio;

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
        //radioStations.Clear();

        //RadioStation newStation = new RadioStation();
        targetFrequency = Random.Range(0f, 25f);
        //newStation.frequency = targetFrequency;
        //newStation.clip = correctAudio;
        //radioStations.Add(newStation);

        //for (int i = 0; i < badAudio.Count; i++)
        //{
        //    RadioStation newBadStation = new RadioStation();
        //    float randomFrequency = Random.Range(0f, 25f);
        //    newBadStation.frequency = randomFrequency;
        //    newBadStation.clip = badAudio[i];
        //    radioStations.Add(newBadStation);
        //}
    }

    public override void Update()
    {
        base.Update();

        currentFrequency = Mathf.Clamp(currentFrequency, 0f, 25f);
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

        if (GameplayController.instance.spawnStaticMan)
        {
            if (currentFrequency <= targetFrequency + 3.5f && currentFrequency >= targetFrequency - 3.5f)
            {
                //Safe station
                DialogueController.instance.UpdateText("Good audio", false);
                GameplayController.instance.spawnStaticMan = false;
                if (audioSource.clip != staticAudio)
                {
                    audioSource.Stop();
                    audioSource.clip = staticAudio;
                    audioSource.Play();
                }
            }
            else
            {
                DialogueController.instance.UpdateText("Bad audio", false);
                if (audioSource.clip != badAudio)
                {
                    audioSource.Stop();
                    audioSource.clip = badAudio;
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
