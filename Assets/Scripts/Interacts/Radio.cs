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
    [SerializeField] List<RadioStation> activeStations;
    [SerializeField] List<AudioClip> stationClips;

    [SerializeField] TMP_Text radioText;
    public float targetFrequency { get; private set; } //public in order to display frequency on document
    [SerializeField] float rotateSpeed, focusTime = 1f;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip targetAudio, staticAudio, badAudio;

    bool interacting;


    public void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        InitializeRadio();
    }

    private void OnDisable()
    {
        currentFrequency = 36.9f;
        radioText.text = currentFrequency.ToString("F2");
    }

    public void InitializeRadio()
    {
        interacting = false;
        audioSource.clip = staticAudio;
        audioSource.loop = true;
        audioSource.Play();
        currentFrequency = 36.9f;
        targetFrequency = currentFrequency;
        GenerateActiveStations();
    }

    private void InitializeFrequency()
    {
        float returnFrequency = targetFrequency;
        float offsetVal = Random.Range(35f, 50f);
        int randDirection = Random.Range(0, 2);
        returnFrequency = randDirection == 0
                                ? returnFrequency + offsetVal
                                : returnFrequency - offsetVal;

        returnFrequency = Mathf.Clamp(returnFrequency, 30f, 300f);

        //If result is at either the max or min, re-roll the new station
        if (returnFrequency == 30f || returnFrequency == 300f)
            InitializeFrequency();
        else
            targetFrequency = returnFrequency;
    }

    public void GenerateActiveStations()
    {
        activeStations.Clear();
        InitializeFrequency();
        RadioStation targetStation = new RadioStation();
        targetStation.frequency = targetFrequency;
        targetStation.message = PlayerController.instance.GetCurrentDocument().toBeShredded ? "Shred File" : PlayerController.instance.GetCurrentDocument().fileColor.ToString();
        targetStation.clip = targetAudio;
        activeStations.Add(targetStation);

        //Active stations include Red, Blue, Yellow, Destroy, and Bad
        for (int i = 0; i < 4; i++)
        {
            float randFrequency = Random.Range(30f, 300f);
            print ($"RandFrequency: {randFrequency}");
            foreach (RadioStation station in activeStations)
            {
                if (station.frequency < randFrequency + 7.5f && station.frequency > randFrequency - 7.5f)
                {
                    print("Station already exists in range");
                }
                else
                {
                    print("Added station");
                    RadioStation newStation = new RadioStation();
                    newStation.frequency = randFrequency;
                    newStation.message = PlayerController.instance.GetRandomColor().ToString();
                    newStation.clip = stationClips[i];
                    activeStations.Add(newStation);
                    break;
                }
            }
        }
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
            float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            arrowLeft.color = scrollDelta < 0 ? arrowActive : arrowDefault;
            arrowRight.color = scrollDelta > 0 ? arrowActive : arrowDefault;

            if (scrollDelta > 0)
            {
                currentFrequency += Time.deltaTime * rotateSpeed;
                dialObj.transform.Rotate(Vector3.up * Time.deltaTime * -rotateSpeed * 10f);
            }
            else if (scrollDelta < 0)
            {
                currentFrequency -= Time.deltaTime * rotateSpeed;
                dialObj.transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed * 10f);
            }



            //Check active stations
            foreach (RadioStation station in activeStations)
            {
                if (currentFrequency <= station.frequency + 1.5f && currentFrequency >= station.frequency - 1.5f)
                {
                    if (!audioSource.isPlaying)
                        audioSource.PlayOneShot(station.clip);
                    DialogueController.instance.UpdateText(station.message, false);
                    if (GameplayController.instance.spawnStaticMan)
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
                    break;
                }
                else
                {
                    if (!audioSource.isPlaying)
                        audioSource.PlayOneShot(staticAudio);
                    DialogueController.instance.UpdateText("......", false);
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
    }

    public override void Interact()
    {
        base.Interact();
        interacting = !interacting;
        DialogueController.instance.UpdateText(string.Empty, false);
    }
}


[System.Serializable]
class RadioStation
{
    public float frequency;
    public string message;
    public AudioClip clip;
}
