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
    [SerializeField][Range(30, 300)] float currentFrequency; //Use LF (low frequency) band for radio stations
    [SerializeField] TMP_Text radioText;
    public float targetFrequency { get; private set; } //public in order to display frequency on document
    [SerializeField] float rotateSpeed, focusTime = 1f;
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
        currentFrequency = 36f;
        InitializeFrequency();
    }

    private void OnDisable()
    {
        currentFrequency = 36f;
        radioText.text = currentFrequency.ToString("F2");
    }

    public void InitializeFrequency()
    {
        print("Updating targetFrequency");
        float lastFrequency = targetFrequency;
        targetFrequency = Random.Range(lastFrequency - 20f, lastFrequency + 20f);
        int randDirection = Random.Range(0, 1);
        targetFrequency = randDirection == 0
                                ? targetFrequency + 7f
                                : targetFrequency - 7f;

        targetFrequency = Mathf.Clamp(targetFrequency, 30f, 300f);

        if (targetFrequency == 30f || targetFrequency == 300f)
            InitializeFrequency();
    }

    public override void Update()
    {
        base.Update();

        currentFrequency = Mathf.Clamp(currentFrequency, 30f, 300f);
        radioText.text = currentFrequency.ToString("F2") + "kHz";
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
            if (currentFrequency <= targetFrequency + 1.125f && currentFrequency >= targetFrequency - 1.125f)
            {
                focusTime -= Time.deltaTime;
                if (focusTime <= 0f)
                {
                    focusTime = 1f;
                    GameplayController.instance.ToggleStaticMan(false);
                    if (audioSource.clip != staticAudio)
                    {
                        audioSource.Stop();
                        audioSource.clip = staticAudio;
                        audioSource.Play();
                    }
                }
            }
            else
            {
                focusTime = 1f;
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
