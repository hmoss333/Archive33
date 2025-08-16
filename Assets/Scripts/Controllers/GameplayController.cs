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

    public enum State { dialogue, gameplay, victory, death }
    public State state;

    [Header("Shift Values")]
    [SerializeField] private float shiftDuration;
    private float shiftTime;
    public int shiftNum { get; private set; }
    private int penalty; //Increments on incorrect filing; 5 = death
    [SerializeField] TMP_Text clockText;

    [NaughtyAttributes.HorizontalLine]

    [Header("Radio Static Values")]
    [SerializeField] CamEffectController camEffectController;
    [SerializeField] float stationResetTimer = 14f;
    public bool spawnStaticMan { get; private set; }
    [SerializeField] GameObject staticMan;
    Vector3 staticManDefaultPos;
    ObjectFlicker staticManFlicker;

    [NaughtyAttributes.HorizontalLine]

    [Header("Power Outage Values")]
    [SerializeField] private float powerOutageTimer = 20f;
    private bool powerOutage;
    private float zombieMoveTimer = 3.5f;
    private int zombieMoveNum;
    [SerializeField] GameObject zombie;
    [SerializeField] List<Transform> zombiePoints;
    [SerializeField] List<Light> lights;

    [NaughtyAttributes.HorizontalLine]

    [Header("Bell and Robot Values")]
    [SerializeField] GameObject robot;
    private bool moveRobot;
    [SerializeField] float robotSpeed = 1f;
    [SerializeField] float robotWaitTime = 6f;
    [SerializeField] List<Transform> robotMovePoints;
    [SerializeField] private int currentPoint;

    [NaughtyAttributes.HorizontalLine]

    [Header("Jump Scare Values")]
    [SerializeField] GameObject jumpScare;
    [SerializeField] List<GameObject> js_Models;
    private int js_ModelNum;
    AudioSource jumpScareAudio;
    [SerializeField] AudioClip jumpScareClip;

    [NaughtyAttributes.HorizontalLine]

    [Header("Dialogue Values")]
    [SerializeField] TMP_Text shiftOverText;
    [SerializeField] TMP_Text shiftCompleteText;
    [SerializeField] TMP_Text winGameText;
    [SerializeField] List<DialogueContainer> uniqueDialogue;
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

        LoadDocumentText();
        jumpScareAudio = jumpScare.GetComponent<AudioSource>();

        shiftNum = 0;//PlayerPrefs.GetInt("longNightMode") == 2
            //? 5 //if longNightMode is enabled, skip to last night
            //: PlayerPrefs.GetInt("shiftNum", 0); //else load last completed night; default to first night
        shiftTime = 0f;
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
        //FadeController.instance.StartFade(1f, 0.01f);
        state = State.dialogue;


        foreach (Light light in lights)
        {
            light.enabled = true;
            light.GetComponent<LightFlicker>().enabled = false;
            light.intensity = 3f;
        }
        FadeController.instance.StartFade(0f, 2f);
    }

    private void Update()
    {
        SetProps(shiftNum);
        SetWarningLights(penalty);

        if ((state == State.dialogue || state == State.gameplay) && shiftNum < 5)
        {
            shiftDuration = shiftNum > 0 ? 360f : 90f;
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

        switch (state)
        {
            case State.dialogue:
                //Play dialogue set for current shift
                if (introDialogueCo == null)
                    introDialogueCo = StartCoroutine(IntroDialogueRoutine(uniqueDialogue[shiftNum].dialogueLines));
                break;
            case State.gameplay:
                //Handle all gameplay loop logic
                if (shiftNum >= 0)
                {
                    //Inbox
                    //Outbox
                    //Shredder
                }
                if (shiftNum >= 1)
                {
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
                            stationResetTimer = Random.Range(10f, 14f); //Reset to default value
                            ToggleStaticMan(true);
                            Radio.instance.InitializeFrequency();
                        }
                    }

                    if (dist <= 2.5f)
                    {
                        js_ModelNum = 2;
                        SetState(State.death);
                        //Jump scare
                    }
                }
                if (shiftNum >= 2)
                {
                    //Power outage
                    //FuseBox + fuses
                    //Zombie enemy
                    zombie.SetActive(powerOutage);

                    if (!powerOutage)
                    {
                        zombieMoveNum = 0;
                        powerOutageTimer -= Time.deltaTime;
                        if (powerOutageTimer <= 0)
                        {
                            powerOutageTimer = Random.Range(15f, 20f);
                            powerOutage = true;
                            FuseBox.instance.SetBroken();
                        }

                        foreach (Light light in lights)
                        {
                            light.enabled = true;
                            light.GetComponent<LightFlicker>().enabled = false;
                            light.intensity = 3f;
                        }
                    }
                    else
                    {
                        foreach (Light light in lights)
                        {
                            light.enabled = true;
                            light.GetComponent<LightFlicker>().enabled = true;
                        }

                        zombie.transform.position = zombiePoints[zombieMoveNum].position;
                        zombieMoveTimer -= Time.deltaTime;
                        if (zombieMoveTimer <= 0)
                        {
                            if (zombieMoveNum < zombiePoints.Count - 1)
                            {
                                zombieMoveNum++;
                                zombieMoveTimer = 3.5f;
                            }
                            else
                            {
                                js_ModelNum = 0;
                                SetState(State.death);
                            }
                        }
                    }
                }
                if (shiftNum >= 3 || penalty >= 5)
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
                if (shiftNum >= 4)
                {
                    //Lower timers for all hazards
                }
                break;
            case State.victory:
                //Logic for if the player makes it to the end of their shift
                if (!FadeController.instance.isFading)
                {
                    //Reset scene for next shift
                    if (shiftNum < 5)
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
                    gameOverCo = StartCoroutine(GameOverRoutine());
                break;
            default:
                DialogueController.instance.UpdateText($"Current state: {state}", true);
                break;
        }
    }

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

    public void SetState(State stateVal)
    {
        state = stateVal;
    }

    void SetProps(int shiftVal)
    {
        radio.SetActive(shiftVal >= 1);
        fuseBoxCover.SetActive(shiftVal < 2);
        bell.SetActive(shiftVal >= 3);
    }

    void SetWarningLights(int penaltyVal)
    {
        for (int i = 0; i < warningLights.Count; i++)
        {
            warningLights[i].GetComponent<Renderer>().material.color = i <= penaltyVal - 1 ? Color.red : Color.gray;
        }
    }

    //TODO determine if this is still needed
    ///Probably can be removed since we're no longer using score
    public void Success()
    {

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

    void JumpScare(int js_num)
    {
        js_Models[js_num].SetActive(true);
        jumpScare.SetActive(true);
        jumpScareAudio.PlayOneShot(jumpScareClip);
    }

    IEnumerator IntroDialogueRoutine(List<string> dialogueItems)
    {
        yield return new WaitForSeconds(3.5f);

        for (int i = 0; i < dialogueItems.Count; i++)
        {
            DialogueController.instance.UpdateText(dialogueItems[i], false);
            yield return new WaitForSeconds(3f);
        }

        DialogueController.instance.UpdateText(string.Empty, false);
        SetState(State.gameplay);
        introDialogueCo = null;
    }

    IEnumerator GameOverRoutine()
    {
        JumpScare(js_ModelNum);

        FadeController.instance.StartFade(1f, 3f);
        FadeController.instance.StartFadeText(shiftOverText, 1f, 1f);

        //yield return new WaitForSeconds(0.5f);

        //while (FadeController.instance.isFading)
        //    yield return null;

        yield return new WaitForSeconds(2.5f);

        SceneManager.LoadScene(0);

        gameOverCo = null;
    }

    IEnumerator EndOfNightRoutine()
    {
        FadeController.instance.StartFade(1f, 1f);
        FadeController.instance.StartFadeText(shiftCompleteText, 1f, 1f);

        while (FadeController.instance.isFading)
            yield return null;

        yield return new WaitForSeconds(3f);

        FadeController.instance.StartFadeText(shiftCompleteText, 0f, 1f);

        while (FadeController.instance.isFading)
            yield return null;

        shiftTime = 0f;
        penalty = 0;
        powerOutage = false;
        ToggleStaticMan(false);
        introDialogueCo = null;
        shiftNum++;
        FuseBox.instance.SetFixed();
        moveRobot = false;
        robotWaitTime = 6f;
        currentPoint = 0;
        PlayerController.instance.RemoveCurrentDocument();
        InBox.instance.Reset();
        PlayerPrefs.SetInt("shiftNum", shiftNum);
        foreach (Light light in lights)
        {
            light.enabled = true;
            light.GetComponent<LightFlicker>().enabled = false;
            light.intensity = 3f;
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
        FadeController.instance.StartFade(1f, 3f);
        FadeController.instance.StartFadeText(winGameText, 1f, 1f);

        while (FadeController.instance.isFading)
            yield return null;

        yield return new WaitForSeconds(6f);

        PlayerPrefs.SetInt("longNightMode", 1);
        SceneManager.LoadScene(0);

        winGameCo = null;
    }
}

[System.Serializable]
class DialogueContainer
{
    public List<string> dialogueLines;
}

[System.Serializable]
class DocumentTextContainer
{
    public List<string> documentText;
    public List<string> corruptedText;
}
