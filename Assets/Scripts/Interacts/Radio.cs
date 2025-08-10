using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Radio : InteractObject
{
    public static Radio instance;

    [SerializeField] GameObject dialObj;
    [SerializeField] SpriteRenderer arrowLeft, arrowRight;
    [SerializeField] Color arrowDefault, arrowActive;
    //[SerializeField] List<RadioStation> radioStations;

    [SerializeField][Range(88, 108)] float currentFrequency; //88 - 108
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
        currentFrequency = 88f;
        InitializeFrequency();
    }

    private void OnDisable()
    {
        currentFrequency = 88f;
        radioText.text = currentFrequency.ToString("F2");
    }

    public void InitializeFrequency()
    {
        print("Updating targetFrequency");
        float lastFrequency = targetFrequency;
        targetFrequency = Random.Range(lastFrequency - 5f, lastFrequency + 5f);
        int randDirection = Random.Range(0, 1);
        targetFrequency = randDirection == 0
                                ? targetFrequency + 2.5f
                                : targetFrequency - 2.5f;

        targetFrequency = Mathf.Clamp(targetFrequency, 88f, 108f);
    }

    public override void Update()
    {
        base.Update();

        currentFrequency = Mathf.Clamp(currentFrequency, 88f, 108f);
        radioText.text = currentFrequency.ToString("F2") + "FM";
        radioText.gameObject.SetActive(interacting);
        arrowLeft.gameObject.SetActive(interacting);
        arrowRight.gameObject.SetActive(interacting);

        //TODO
        //Add logic to have the player tune the radio to a randomized station value in order to get the instructions for the current document
        if (interacting)
        {
            PlayerController.instance.SetState(PlayerController.States.interacting);
            float xInput = Input.GetAxis("Mouse X");
            arrowLeft.color = xInput < 0 ? arrowActive : arrowDefault;
            arrowRight.color = xInput > 0 ? arrowActive : arrowDefault;

            if (xInput > 0)
            {
                currentFrequency += Time.deltaTime * rotateSpeed;
                dialObj.transform.Rotate(Vector3.up * Time.deltaTime * -rotateSpeed * 10f);
            }
            else if (xInput < 0)
            {
                currentFrequency -= Time.deltaTime * rotateSpeed;
                dialObj.transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed * 10f);
            }
        }

        if (GameplayController.instance.spawnStaticMan)
        {
            if (currentFrequency <= targetFrequency + 0.5f && currentFrequency >= targetFrequency - 0.5f)
            {
                //DialogueController.instance.UpdateText("Good audio", false);
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
                //DialogueController.instance.UpdateText("Bad audio", false);
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
