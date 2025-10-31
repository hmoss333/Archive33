using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Radio : InteractObject
{
    public static Radio instance;

    [SerializeField] Transform focusPoint;
    [SerializeField] GameObject dialObj;
    [SerializeField] SpriteRenderer arrowLeft, arrowRight;
    [SerializeField] Color arrowDefault, arrowActive;
    [SerializeField][Range(30, 120)] float currentFrequency; //Use LF (low frequency) band for radio stations
    [SerializeField] List<RadioStation> activeStations;
    [SerializeField] List<AudioClip> stationClips;

    [SerializeField] TMP_Text radioText;
    public float targetFrequency;// { get; private set; } //public in order to display frequency on document
    [SerializeField] float rotateSpeed, focusTime = 1f;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip targetAudio, staticAudio, badAudio;

    bool interacting, tunedToStation;
    Vector2 viewPos;
    Vector2 mouseDownPos;


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
        float offsetVal = Random.Range(15f, 25f);
        int randDirection = Random.Range(0, 2);
        returnFrequency = randDirection == 0
                                ? returnFrequency - offsetVal
                                : returnFrequency + offsetVal;

        returnFrequency = Mathf.Clamp(returnFrequency, 30f, 120f);

        //If result is at either the max or min, re-roll the new station
        if (returnFrequency == 30f || returnFrequency == 120f)
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
        string currentColor = PlayerController.instance.GetCurrentDocument().fileColor.ToString();
        try
        {
            targetStation.message = PlayerController.instance.GetCurrentDocument().toBeShredded
                                        ? "Shred File"
                                        : currentColor;
        }
        catch
        {
            targetStation.message = "......";
        }
        switch (targetStation.message.ToLower())
        {
            case "red":
                targetStation.clip = stationClips[0];
                break;
            case "yellow":
                targetStation.clip = stationClips[1];
                break;
            case "blue":
                targetStation.clip = stationClips[2];
                break;
            default:
                targetStation.clip = stationClips[3];
                break;
        }
        activeStations.Add(targetStation);

        //Active stations include Red, Blue, Yellow, Destroy, and Bad
        for (int i = 0; i < 4; i++)
        {
            float randFrequency = Random.Range(30f, 120f);
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
                    switch (newStation.message.ToLower())
                    {
                        case "red":
                            newStation.clip = stationClips[0];
                            break;
                        case "yellow":
                            newStation.clip = stationClips[1];
                            break;
                        case "blue":
                            newStation.clip = stationClips[2];
                            break;
                        default:
                            newStation.clip = stationClips[3];
                            break;
                    }
                    activeStations.Add(newStation);
                    break;
                }
            }
        }
    }

    public override void Update()
    {
        base.Update();

        currentFrequency = Mathf.Clamp(currentFrequency, 30f, 120f);
        radioText.text = currentFrequency.ToString("F2") + "kHz";
        radioText.gameObject.SetActive(interacting);
        arrowLeft.gameObject.SetActive(interacting);
        arrowRight.gameObject.SetActive(interacting);


        viewPos.x = Input.GetAxis("Mouse X");
        viewPos.y = Input.GetAxis("Mouse Y");


        //TODO
        //Add logic to have the player tune the radio to a randomized station value in order to get the instructions for the current document
        if (interacting && GameplayController.instance.state == GameplayController.State.gameplay)
        {
            PlayerController.instance.SetState(PlayerController.States.interacting);

            if (Input.GetAxisRaw("Horizontal") > 0)//viewPos.x > mouseDownPos.x)
            {
                //scroll frequency down
                currentFrequency += Time.deltaTime * rotateSpeed;
                dialObj.transform.Rotate(Vector3.up * Time.deltaTime * -rotateSpeed);
            }
            else if (Input.GetAxisRaw("Horizontal") < 0)//viewPos.x < mouseDownPos.x)
            {
                //scroll frequency up
                currentFrequency -= Time.deltaTime * rotateSpeed;
                dialObj.transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
            }

            //Check active stations
            for (int i = 0; i < activeStations.Count; i++)
            {
                if (currentFrequency <= activeStations[i].frequency + 1.5f && currentFrequency >= activeStations[i].frequency - 1.5f)
                {
                    tunedToStation = true;

                    //Play station audio
                    if (audioSource.clip != activeStations[i].clip)
                    {
                        audioSource.Stop();
                        audioSource.clip = activeStations[i].clip;
                        audioSource.Play();
                    }
                    string stationMessage = $"File document as {activeStations[i].message}";
                    DialogueController.instance.UpdateText(stationMessage, false);
                    if (GameplayController.instance.spawnStaticMan)
                    {
                        focusTime -= Time.deltaTime;
                        if (focusTime <= 0f)
                        {
                            focusTime = 1f;
                            GameplayController.instance.ToggleStaticMan(false);
                        }
                    }
                    break;
                }
                else
                {
                    tunedToStation = false;
                    DialogueController.instance.UpdateText("......", false);
                }
            }
        }
        else
        {
            tunedToStation = false;
        }

        if (!tunedToStation)
        {
            //If not tuned to a specific station, play default audio
            if (GameplayController.instance.spawnStaticMan)
            {
                if (audioSource.clip != badAudio)
                {
                    audioSource.Stop();
                    audioSource.clip = badAudio;
                    audioSource.Play();
                }
            }
            else
            {
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
        DialogueController.instance.UpdateText(string.Empty, false);
        mouseDownPos = viewPos;

        if (interacting)
        {
            CamFocusController.instance.FocusTarget(focusPoint);
        }
        else
        {
            CamFocusController.instance.FocusReset();
        }
    }
}


[System.Serializable]
struct RadioStation
{
    public float frequency;
    public string message;
    public AudioClip clip;
}
