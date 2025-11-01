using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine.SceneManagement;

public class GameplayController : MonoBehaviour
{
    public static GameplayController instance;

    [NaughtyAttributes.HorizontalLine]

    [Header("Warning References")]
    [SerializeField] List<GameObject> warningLights;
    [SerializeField] AudioSource incorrectAudio;
    [SerializeField] AudioClip incorrectClip;

    [Header("Prop References")]
    [SerializeField] GameObject radio;
    [SerializeField] GameObject fuseBoxCover;
    [SerializeField] GameObject bell;
    [SerializeField] GameObject painting;

    public enum State { dialogue, gameplay, victory, death }
    public State state;

    [Header("Shift Values")]
    [SerializeField] private float shiftDuration;
    private float shiftTime, longNightTime;
    public int shiftNum { get; private set; }
    private int penalty; //Increments on incorrect filing; 5 = death
    [SerializeField] TMP_Text clockText;

    [HorizontalLine]

    [Header("Radio Static Values")]
    [SerializeField] CamEffectController camEffectController;
    [SerializeField] float stationResetTimer = 14f;
    public bool spawnStaticMan { get; private set; }
    [SerializeField] GameObject staticMan;
    Vector3 staticManDefaultPos;
    ObjectFlicker staticManFlicker;

    [HorizontalLine]

    [Header("Power Outage Values")]
    [SerializeField] private float powerOutageTimer = 20f;
    private bool powerOutage;
    private float zombieMoveTimer = 3.5f;
    private float lightOutTimer = 0.45f;
    private int zombieMoveNum;
    [SerializeField] GameObject zombie;
    [SerializeField] List<Transform> zombiePoints;
    [SerializeField] List<Light> lights;

    [HorizontalLine]

    [Header("Bell and Robot Values")]
    [SerializeField] GameObject robot;
    private bool moveRobot;
    [SerializeField] float robotSpeed = 1f;
    [SerializeField] float robotWaitTime = 6f;
    [SerializeField] List<Transform> robotMovePoints;
    private int currentPoint;

    [HorizontalLine]

    [Header("Jump Scare Values")]
    [SerializeField] GameObject jumpScare;
    [SerializeField] List<GameObject> js_Models;
    private int js_ModelNum;
    AudioSource jumpScareAudio;
    [SerializeField] AudioClip jumpScareClip;

    [HorizontalLine]

    [Header("Suffocate Values")]
    [SerializeField] AudioSource suffocateAudio;
    [SerializeField] AudioClip suffocateClip;

    [HorizontalLine]

    [Header("Dialogue Values")]
    [SerializeField] TMP_Text shiftOverText;
    [SerializeField] TMP_Text shiftCompleteText;
    [SerializeField] TMP_Text holdToSkipText;
    [SerializeField] GameObject retryMenu;
    [SerializeField] List<DialogueContainer> uniqueDialogue;
    [SerializeField] DialogueContainer winDialogue;
    [SerializeField] private float skipTimer = 0f;
    Coroutine introDialogueCo;
    Coroutine nextNightCo;
    Coroutine winGameCo;
    Coroutine gameOverCo;

    [HorizontalLine]

    [Header("Document Text")]
    [SerializeField] DocumentTextContainer documentTextContainer;


    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        //TODO set the lock state based on pause menu
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;

        AudioController.instance.ModifyVolume();

        LoadDocumentText();
        jumpScareAudio = jumpScare.GetComponent<AudioSource>();

        CamFocusController.instance.FocusReset();
        shiftNum = PlayerPrefs.GetInt("longNightMode") == 2
            ? 5 //if longNightMode is enabled, skip to last night
            : PlayerPrefs.GetInt("shiftNum", 0); //else load last completed night; default to first night
        shiftTime = 0f;
        longNightTime = 0f;
        powerOutage = false;
        zombieMoveNum = 0;
        zombie.SetActive(false);
        ToggleStaticMan(false);
        staticMan.SetActive(false);
        staticManDefaultPos = staticMan.transform.position;
        staticManFlicker = GetComponent<ObjectFlicker>();
        moveRobot = false;
        robotWaitTime = 6f;
        currentPoint = 0;
        InBox.instance.Reset();
        retryMenu.SetActive(false);
        state = State.dialogue; //State.gameplay;
        PlayerController.instance.SetState(PlayerController.States.idle);


        foreach (Light light in lights)
        {
            light.enabled = true;
            light.GetComponent<LightFlicker>().enabled = false;
            light.intensity = 1.5f;
        }
        FadeController.instance.StartFade(0f, 2f);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("newGame", 0);
    }

    private void Update()
    {
        //Reset props, lights, and volume based on the current shift number
        SetProps(shiftNum);
        SetWarningLights(penalty);
        AudioController.instance.ModifyVolume();


        //Only display the 'Hold to Skip' text element during the dialogue state
        holdToSkipText.enabled = state == State.dialogue;


        //Suffocattion audio controller
        //Scale all audiosources based on the current remaining airTime using the volume PlayerPref as a max
        float suffocateVolume = InBox.instance.airTime / 15f;
        suffocateVolume = Mathf.Clamp(suffocateVolume, 0f, AudioController.instance.volume);
        AudioController.instance.ModifyVolume(suffocateVolume);
        suffocateAudio.volume = AudioController.instance.volume; //always play this at max volume
        if (suffocateVolume <= 0.45f && !suffocateAudio.isPlaying)
        {
            suffocateAudio.PlayOneShot(suffocateClip);
        }


        //State Machine
        switch (state)
        {
            case State.dialogue:
                //Clamp shiftNum to avoid out-of-range errors
                shiftNum = Mathf.Clamp(shiftNum, 0, uniqueDialogue.Count - 1);

                //Play dialogue set for current shift
                if (introDialogueCo == null)
                    introDialogueCo = StartCoroutine(IntroDialogueRoutine(uniqueDialogue[shiftNum].dialogueLines));
                else
                {
                    if (Input.GetKey(KeyCode.Space))
                    {
                        skipTimer += Time.deltaTime;
                        if (skipTimer >= 1.5f)
                        {
                            skipTimer = 0f;
                            StopCoroutine(introDialogueCo);
                            DialogueController.instance.UpdateText("", false);
                            SetState(State.gameplay);
                            InBox.instance.GenerateNewDocument();
                        }
                    }
                    else
                    {
                        skipTimer = 0f;
                    }
                }

                break;
            case State.gameplay:
                //Handle all gameplay loop logic

                //Shift Timer
                if (shiftNum < 4)
                {
                    shiftDuration = shiftNum > 0 ? 360f : 240f;
                    System.TimeSpan time = System.TimeSpan.FromSeconds(shiftTime);
                    clockText.text = time.ToString(@"mm\:ss");

                    //Countdown shift timer
                    shiftTime += Time.deltaTime;
                    if (shiftTime >= shiftDuration)
                    {
                        shiftTime = 0f;
                        SetState(State.victory);
                    }
                }
                else
                {
                    longNightTime += Time.deltaTime;
                    PlayerPrefs.SetFloat("longNightScore", longNightTime);
                    System.TimeSpan time = System.TimeSpan.FromSeconds(longNightTime);
                    clockText.text = time.ToString(@"mm\:ss");
                }


                //Shift interact logic
                if (shiftNum >= 0)
                {
                    //Inbox
                    //Outbox
                    //Shredder
                    //Radio
                    //Static man enemy
                    staticMan.SetActive(spawnStaticMan);
                    camEffectController.SetEffectState(spawnStaticMan);
                    float dist = Vector3.Distance(staticMan.transform.position, PlayerController.instance.transform.position);

                    if (spawnStaticMan)
                    {
                        staticManFlicker.StartFlicker(0.5f);
                        staticMan.transform.position = Vector3.MoveTowards(staticMan.transform.position, PlayerController.instance.transform.position, 1f * Time.deltaTime);
                    }
                    else
                    {
                        staticMan.transform.position = staticManDefaultPos;
                        stationResetTimer -= Time.deltaTime;
                        if (stationResetTimer <= 0)
                        {
                            float maxStationResetTime = shiftNum >= 3 ? 25f : 28f;
                            stationResetTimer = Random.Range(10f, maxStationResetTime);
                            ToggleStaticMan(true);
                        }
                    }

                    if (dist <= 2.5f)
                    {
                        js_ModelNum = 2;
                        SetState(State.death);
                    }
                }
                if (shiftNum >= 1)
                {
                    //Power outage
                    //FuseBox + fuses
                    //Zombie enemy
                    zombie.SetActive(powerOutage);

                    //If no powerOutage, reset zombie position and light intensity values
                    if (!powerOutage)
                    {
                        zombieMoveNum = 0;
                        powerOutageTimer -= Time.deltaTime;
                        if (powerOutageTimer <= 0)
                        {
                            float maxPowerOutageTime = shiftNum >= 3 ? 17f : 20f;
                            powerOutageTimer = Random.Range(15f, 20f);
                            powerOutage = true;
                            FuseBox.instance.SetBroken();
                        }

                        foreach (Light light in lights)
                        {
                            light.enabled = true;
                            light.GetComponent<LightFlicker>().enabled = false;
                            light.intensity = 1.5f;
                        }
                    }
                    //Else if powerOutage event, do zombie move logic
                    else
                    {
                        //Enable light flicker
                        foreach (Light light in lights)
                        {
                            light.enabled = true;
                            light.GetComponent<LightFlicker>().enabled = true;
                        }

                        //Update Zombie position every X seconds
                        zombie.transform.position = zombiePoints[zombieMoveNum].position;
                        zombieMoveTimer -= Time.deltaTime;
                        if (zombieMoveTimer <= 0)
                        {
                            //If zombie is not at the last point
                            if (zombieMoveNum < zombiePoints.Count - 1)
                            {
                                //Turn lights out while zombie is updating position
                                lightOutTimer -= Time.deltaTime;
                                if (lightOutTimer >= 0)
                                {
                                    foreach (Light light in lights)
                                    {
                                        light.intensity = 0f;
                                    }
                                }
                                //Update zombie position and reset timers
                                else
                                {
                                    zombieMoveNum++;
                                    zombieMoveTimer = zombieMoveNum < zombiePoints.Count - 1 ? 3.5f : 4.5f;
                                    print($"Zombie Timer: {zombieMoveTimer}");
                                    lightOutTimer = 0.45f;
                                }
                            }
                            //Else play zombie jumpscare
                            else
                            {
                                js_ModelNum = 0;
                                SetState(State.death);
                            }
                        }
                    }
                }
                if (shiftNum >= 2 || penalty >= 5)
                {
                    //'The Button'
                    //Malformed Documents
                    int midPoint = (int)robotMovePoints.Count / 2 + 1;

                    if (moveRobot)
                    {
                        robot.transform.position = Vector3.MoveTowards(robot.transform.position, robotMovePoints[currentPoint].position, robotSpeed * Time.deltaTime);

                        Vector3 lookDirection = robotMovePoints[currentPoint].position - robot.transform.position;
                        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                        robot.transform.rotation = Quaternion.RotateTowards(robot.transform.rotation, targetRotation, 350f * Time.deltaTime);

                        if (robot.transform.position == robotMovePoints[currentPoint].position)
                        {
                            currentPoint++;
                            if (currentPoint == midPoint)
                            {
                                moveRobot = false;
                            }
                            else if (currentPoint == robotMovePoints.Count)
                            {
                                robot.transform.position = robotMovePoints[0].position;
                                currentPoint = 0;
                                moveRobot = false;
                            }
                        }
                    }
                    else
                    {
                        if (currentPoint == midPoint)
                        {
                            //If robot is in front of the player and they reach 5 penalties, trigger jump scare
                            if (penalty >= 5)
                            {
                                js_ModelNum = 1;
                                SetState(State.death);
                            }

                            robotWaitTime -= Time.deltaTime;
                            if (robotWaitTime <= 0)
                            {
                                robotWaitTime = 6f;
                                currentPoint++;
                                moveRobot = true;
                            }
                        }
                    }

                    robot.GetComponent<Animator>().SetBool("isMoving", moveRobot);
                    robot.GetComponent<Animator>().SetBool("isWaiting", currentPoint == midPoint && !moveRobot);
                    robot.GetComponent<BotController>().enabled = currentPoint == 3;
                }
                if (shiftNum >= 3)
                {
                    //Lower timers for all hazards
                }
                break;
            case State.victory:
                //Logic for if the player makes it to the end of their shift
                if (!FadeController.instance.isFading)
                {
                    //Reset scene for next shift
                    if (shiftNum < 3)
                    {
                        if (nextNightCo == null)
                            nextNightCo = StartCoroutine(EndOfNightRoutine());
                    }
                    //Win game
                    else
                    {
                        if (winGameCo == null)
                            winGameCo = StartCoroutine(WinGameRoutine());
                    }
                }
                break;
            case State.death:
                //Logic for if the player dies
                //Other hazards will change the state from gameplay to this
                if (gameOverCo == null)
                    gameOverCo = StartCoroutine(GameOverRoutine(true));
                break;
            default:
                DialogueController.instance.UpdateText($"Current state: {state}", true);
                break;
        }
    }


    //Document Text Functions
    private void LoadDocumentText()
    {
        string documentTextLocation = Path.Combine(Application.streamingAssetsPath, "documentText.json");

        print("Loading documentText data");
        string jsonData = "";
        jsonData = File.ReadAllText(documentTextLocation);
        documentTextContainer.documentText = JsonUtility.FromJson<DocumentTextContainer>(jsonData).documentText;
        documentTextContainer.corruptedText = JsonUtility.FromJson<DocumentTextContainer>(jsonData).corruptedText;
    }

    public string GetDocumentText(bool isCorrupted)
    {
        string returnString = "";
        int randVal;
        randVal = Random.Range(0, isCorrupted ? documentTextContainer.corruptedText.Count : documentTextContainer.documentText.Count);
        returnString = isCorrupted ? documentTextContainer.corruptedText[randVal] : documentTextContainer.documentText[randVal];

        return returnString;
    }


    //Initialization Functions
    public void SetState(State stateVal)
    {
        state = stateVal;
    }

    void SetProps(int shiftVal)
    {
        radio.SetActive(shiftVal >= 0);
        fuseBoxCover.SetActive(shiftVal < 1);
        painting.SetActive(shiftVal >= 1);
        bell.SetActive(shiftVal >= 2);
    }

    void SetWarningLights(int penaltyVal)
    {
        for (int i = 0; i < warningLights.Count; i++)
        {
            warningLights[i].GetComponent<Renderer>().material.color = i <= penaltyVal - 1 ? Color.red : Color.gray;
        }
    }


    //Gameplay Functions
    public void Success()
    {
        if (spawnStaticMan)
            ToggleStaticMan(false);
    }

    public void Failure()
    {
        penalty++;
        incorrectAudio.PlayOneShot(incorrectClip);

        if (penalty >= 5)
        {
            //If robot is in front of the player, trigger jump scare
            if (currentPoint == 2 || currentPoint == 3)
            {
                js_ModelNum = 1;
                SetState(State.death);
            }
            //Reset robot if he is not already in front of the player
            else
            {
                currentPoint = 0;
                robot.transform.position = robotMovePoints[currentPoint].transform.position;
                CallBot();
            }
        }
    }

    public void RestartPower()
    {
        powerOutage = false;
    }

    public void ToggleStaticMan(bool value)
    {
        spawnStaticMan = value;
    }

    public void CallBot()
    {
        if (moveRobot != true)
        {
            moveRobot = true;
            robotWaitTime = 6f;
        }
    }


    //Death Functions
    void JumpScare(int js_num)
    {
        js_Models[js_num].SetActive(true);
        jumpScare.SetActive(true);
        jumpScareAudio.PlayOneShot(jumpScareClip);
    }

    public void Suffocate()
    {
        //suffocateAudio.PlayOneShot(suffocateClip);
        if (gameOverCo == null)
            gameOverCo = StartCoroutine(GameOverRoutine(false));

        SetState(State.death);
    }


    //Retry Menu Buttons
    public void MainMenu()
    {
        retryMenu.SetActive(false);
        SceneManager.LoadScene(0);
    }

    public void Retry()
    {
        retryMenu.SetActive(false);
        SceneManager.LoadScene(1);
    }

    void ResetScene()
    {
        CamFocusController.instance.FocusReset();
        shiftTime = 0f;
        longNightTime = 0f;
        penalty = 0;
        powerOutage = false;
        zombieMoveNum = 0;
        zombie.SetActive(false);
        ToggleStaticMan(false);
        staticManDefaultPos = staticMan.transform.position;
        moveRobot = false;
        introDialogueCo = null;
        shiftNum++;
        FuseBox.instance.SetFixed();
        Radio.instance.InitializeRadio();
        moveRobot = false;
        robotWaitTime = 6f;
        currentPoint = 0;
        PlayerController.instance.RemoveCurrentDocument();
        InBox.instance.Reset();
        AudioController.instance.ModifyVolume();
        PlayerPrefs.SetInt("shiftNum", shiftNum);
        PlayerPrefs.SetInt("maxShift", shiftNum);
        foreach (Light light in lights)
        {
            light.enabled = true;
            light.GetComponent<LightFlicker>().enabled = false;
            light.intensity = 1.5f;
        }
    }


    //Coroutines
    IEnumerator IntroDialogueRoutine(List<string> dialogueItems)
    {
        yield return new WaitForSeconds(3.5f);

        for (int i = 0; i < dialogueItems.Count; i++)
        {
            DialogueController.instance.UpdateText(dialogueItems[i], false);
            yield return new WaitForSeconds(0.5f);
            while (DialogueController.instance.textActive)
            {
                yield return null;

                if (Input.GetMouseButtonUp(0))
                    break;
            }
        }

        DialogueController.instance.UpdateText(string.Empty, false);
        SetState(State.gameplay);

        yield return new WaitForSeconds(0.5f);
        InBox.instance.GenerateNewDocument();

        introDialogueCo = null;
    }

    IEnumerator GameOverRoutine(bool jumpScare)
    {
        DialogueController.instance.UpdateText(string.Empty, false);

        if (jumpScare)
            JumpScare(js_ModelNum);

        FadeController.instance.StartFade(1f, 3f);
        FadeController.instance.StartFadeText(shiftOverText, 1f, 1f);

        yield return new WaitForSeconds(2.5f);

        retryMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;

        while (retryMenu.activeSelf)
            yield return null;

        gameOverCo = null;
    }

    IEnumerator EndOfNightRoutine()
    {
        DialogueController.instance.UpdateText(string.Empty, false);

        FadeController.instance.StartFade(1f, 1f);
        FadeController.instance.StartFadeText(shiftCompleteText, 1f, 1f);

        while (FadeController.instance.isFading)
            yield return null;

        yield return new WaitForSeconds(3f);

        FadeController.instance.StartFadeText(shiftCompleteText, 0f, 1f);

        while (FadeController.instance.isFading)
            yield return null;

        CamFocusController.instance.FocusReset();
        shiftTime = 0f;
        penalty = 0;
        powerOutage = false;
        ToggleStaticMan(false);
        introDialogueCo = null;
        shiftNum++;
        FuseBox.instance.SetFixed();
        Radio.instance.InitializeRadio();
        moveRobot = false;
        robotWaitTime = 6f;
        currentPoint = 0;
        PlayerController.instance.RemoveCurrentDocument();
        InBox.instance.Reset();
        AudioController.instance.ModifyVolume();
        PlayerPrefs.SetInt("shiftNum", shiftNum);
        PlayerPrefs.SetInt("maxShift", shiftNum);
        foreach (Light light in lights)
        {
            light.enabled = true;
            light.GetComponent<LightFlicker>().enabled = false;
            light.intensity = 1.5f;
        }

        yield return new WaitForSeconds(2f);

        FadeController.instance.StartFade(0f, 2f);

        while (FadeController.instance.isFading)
            yield return null;

        SetState(State.dialogue);
        nextNightCo = null;
    }

    IEnumerator WinGameRoutine()
    {
        PlayerPrefs.SetInt("longNightMode", 1);
        Shake.instance.StartShake();

        for (int i = 0; i < winDialogue.dialogueLines.Count; i++)
        {
            DialogueController.instance.UpdateText(winDialogue.dialogueLines[i], false);
            yield return new WaitForSeconds(0.5f);
            while (DialogueController.instance.textActive)
            {
                yield return null;

                if (Input.GetMouseButtonUp(0))
                    break;
            }
        }

        DialogueController.instance.UpdateText(string.Empty, false);

        yield return new WaitForSeconds(1.5f);

        FadeController.instance.StartFade(1f, 3f);

        while (FadeController.instance.isFading)
            yield return null;

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(0);

        winGameCo = null;
    }
}

[System.Serializable]
struct DialogueContainer
{
    public List<string> dialogueLines;
}

[System.Serializable]
struct DocumentTextContainer
{
    public List<string> documentText;
    public List<string> corruptedText;
}
