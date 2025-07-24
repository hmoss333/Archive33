using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;

public class GameplayController : MonoBehaviour
{
    public static GameplayController instance;

    [SerializeField] InteractObject radio;
    [SerializeField] InteractObject inbox;
    [SerializeField] InteractObject outbox;
    [SerializeField] InteractObject stamp;
    [SerializeField] InteractObject shredder;
    [SerializeField] InteractObject lamp;
    [SerializeField] InteractObject fuseBox;
    [SerializeField] InteractObject fuse1;
    [SerializeField] InteractObject fuse2;
    [SerializeField] InteractObject fuse3;

    public enum State { dialogue, gameplay, victory, death }
    public State state;
    public int shiftNum { get; private set; }

    private int score;
    private int penalty;

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
        introDialogueCo = null;
        state = State.dialogue;
    }

    private void Update()
    {
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
}

[System.Serializable]
class DialogueContainer
{
    public List<string> dialogueLines;
}
