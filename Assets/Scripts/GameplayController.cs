using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;

public class GameplayController : MonoBehaviour
{
    public static GameplayController instance;

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

    [Header("Power Outage Values")]
    [SerializeField] float powerOutageTimer = 20f;
    private int powerOutage; //0 = Off; 1 = Start outage; 2 = Active outage
    [SerializeField] List<Light> lights;
    Coroutine powerOutageCo;

    [NaughtyAttributes.HorizontalLine]

    [Header("Dialogue Values")]
    [SerializeField] List<DialogueContainer> uniqueDialogue;
    Coroutine introDialogueCo;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        shiftNum = 0;
        powerOutage = 0;
        powerOutageCo = null;
        introDialogueCo = null;
        state = State.dialogue;
    }

    private void Update()
    {
        SetProps(shiftNum);

        switch (state)
        {
            case State.dialogue:
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
                }
                if (shiftNum >= 2)
                {
                    //Power outage
                    //FuseBox + fuses
                    //Zombie enemy
                    if (powerOutage == 0)
                    {
                        if (powerOutageTimer > 0)
                        {
                            powerOutageTimer -= Time.deltaTime;
                        }
                        else
                        {
                            powerOutageTimer = 20f;
                            powerOutage = 1;
                        }
                    }
                    else if (powerOutage == 1)
                    {
                        if (powerOutageCo == null)
                            powerOutageCo = StartCoroutine(PowerOutageRoutine(false));
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
                if (shiftNum < 5)
                {
                    shiftNum++;
                    score = 0;
                    penalty = 0;
                    SetState(State.dialogue);
                }
                else
                {
                    //You win!
                }
                break;
            case State.death:
                //Logic for if the player dies
                //Other hazards will change the state from gameplay to this
                DialogueController.instance.UpdateText("[TODO]: handle death logic here", true);
                score = 0;
                penalty = 0;
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

    public void Success()
    {
        score++;

        if (score >= 10)
            SetState(State.victory);
    }

    public void Failure()
    {
        penalty++;

        if (penalty >= 5)
            SetState(State.death);
    }

    public void RestartPower()
    {
        print("RestartPower");
        powerOutage = 0;
        if (powerOutageCo != null) { StopCoroutine(powerOutageCo); }
        powerOutageCo = StartCoroutine(PowerOutageRoutine(true));
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

    IEnumerator PowerOutageRoutine(bool value)
    {
        //Flicker lights
        //Turn lights on/off depending on state
        foreach (Light light in lights)
        {
            light.enabled = true;
            light.GetComponent<LightFlicker>().enabled = true;
        }

        yield return new WaitForSeconds(1.25f);

        foreach (Light light in lights)
        {
            light.enabled = value;
            light.GetComponent<LightFlicker>().enabled = false;
        }

        powerOutage = value ? 0 : 2;
        powerOutageCo = null;
    }
}

[System.Serializable]
class DialogueContainer
{
    public List<string> dialogueLines;
}
