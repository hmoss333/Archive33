using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;

public class GameplayController : MonoBehaviour
{
    public static GameplayController instance;

    [NaughtyAttributes.HorizontalLine]

    [Header("Warning References")]
    [SerializeField] List<GameObject> warningLights;

    [Header("Prop References")]
    [SerializeField] GameObject radio;
    [SerializeField] GameObject fuseBox;
    [SerializeField] GameObject bell;

    public enum State { dialogue, gameplay, victory, death }
    public State state;
    public int shiftNum { get; private set; }

    private int score; //Increments on correct filing; 10 = win
    private int penalty; //Increments on incorrect filing; 5 = death

    [NaughtyAttributes.HorizontalLine]

    [Header("Sanity Variables")]
    [SerializeField] private float sanity = 100f;
    private bool loseSanity = false;
    [SerializeField] float sanityLossRate;
    [SerializeField] float sanityGainRate;
    [SerializeField] GameObject staticMan;

    [NaughtyAttributes.HorizontalLine]

    [Header("Power Outage Variables")]
    [SerializeField] private float powerOutageTimer = 20f;
    private bool powerOutage;
    private float zombieMoveTimer = 5f;
    private int zombieMoveNum;
    [SerializeField] GameObject zombie;
    [SerializeField] List<Transform> zombiePoints;
    [SerializeField] List<Light> lights;

    [NaughtyAttributes.HorizontalLine]

    [Header("Dialogue Variables")]
    [SerializeField] List<DialogueContainer> uniqueDialogue;
    Coroutine introDialogueCo;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        shiftNum = 1; // 0;
        powerOutage = false;
        zombieMoveNum = 0;
        zombie.SetActive(false);
        staticMan.SetActive(false);
        introDialogueCo = null;
        FadeController.instance.StartFade(1f, 0.01f);
        state = State.dialogue;
    }

    private void Update()
    {
        SetProps(shiftNum);
        SetWarningLights(penalty);

        switch (state)
        {
            case State.dialogue:
                FadeController.instance.StartFade(0f, 2f);

                //Play dialogue set for current shift
                if (introDialogueCo == null)
                    introDialogueCo = StartCoroutine(IntroDialogueRoutine(uniqueDialogue[shiftNum].dialogueLines));
                break;
            case State.gameplay:
                //Handle all gameplay loop logic
                //Adds more features based on shiftNum count
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
                    if (loseSanity)
                    {
                        if (sanity > 0f)
                            sanity -= sanityLossRate * Time.deltaTime;
                    }
                    else
                    {
                        if (sanity < 100f)
                            sanity += sanityGainRate * Time.deltaTime;
                    }


                    if (sanity <= 75f)
                    {
                        //First phase
                    }
                    else if (sanity <= 50f)
                    {
                        //Second phase
                    }
                    else if (sanity <= 25f)
                    {
                        //Third phase
                    }
                    else if (sanity <= 0)
                    {
                        //Death
                        SetState(State.death);
                    }
                    else
                    {
                        //Default
                    }

                    loseSanity = false;
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
                            powerOutageTimer = 20f;
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
                                zombieMoveTimer = 5f;
                            }
                            else
                            {
                                SetState(State.death);
                            }
                        }
                    }
                }
                if (shiftNum >= 3)
                {
                    //'The Button'
                    //Malformed Documents
                }
                if (shiftNum >= 4)
                {
                    //Lower timers for all hazards
                }
                break;
            case State.victory:
                //Logic for if the player makes it to the end of their shift
                DialogueController.instance.UpdateText("[TODO]: display win screen here", true);

                if (!FadeController.instance.isFading)
                {
                    //Reset scene for next shift
                    if (shiftNum < 5)
                    {
                        score = 0;
                        penalty = 0;
                        powerOutage = false;
                        sanity = 100f;
                        shiftNum++;
                        SetState(State.dialogue);
                    }
                    //Win game
                    else
                    {
                        //TODO add win game logic here
                    }
                }
                break;
            case State.death:
                //Logic for if the player dies
                //Other hazards will change the state from gameplay to this
                DialogueController.instance.UpdateText("[TODO]: handle death logic here", true);
                FadeController.instance.StartFade(1f, 3f);
                break;
            default:
                DialogueController.instance.UpdateText($"Current state: {state}", true);
                break;
        }
    }

    public void SetState(State stateVal)
    {
        state = stateVal;
    }

    void SetProps(int shiftVal)
    {
        radio.SetActive(shiftVal >= 1);
        fuseBox.SetActive(shiftVal >= 2);
        bell.SetActive(shiftVal >= 3);
    }

    void SetWarningLights(int penaltyVal)
    {
        for (int i = 0; i < warningLights.Count; i++)
        {
            warningLights[i].GetComponent<Renderer>().material.color = i <= penaltyVal - 1 ? Color.red : Color.gray;
        }
    }

    void ToggleInteracts(bool value)
    {
        InteractObject[] interacts = FindObjectsOfType<InteractObject>();
        foreach (InteractObject interact in interacts)
        {
            interact.enabled = value;
        }
    }

    public void Success()
    {
        score++;

        if (score >= 3)//10)
        {
            ToggleInteracts(false);
            FadeController.instance.StartFade(1f, 5f);           
            SetState(State.victory);
        }
    }

    public void Failure()
    {
        penalty++;

        if (penalty >= 5)
            SetState(State.death);
    }

    public void RestartPower()
    {
        powerOutage = false;
    }

    public void BadRadioStation()
    {
        loseSanity = true;
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
        ToggleInteracts(true);
        SetState(State.gameplay);
        introDialogueCo = null;
    }
}

[System.Serializable]
class DialogueContainer
{
    public List<string> dialogueLines;
}
