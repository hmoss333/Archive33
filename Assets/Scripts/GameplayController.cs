using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameplayController : MonoBehaviour
{
    public static GameplayController instance;

    public int phase { get; private set; }
    [SerializeField] int score;
    [SerializeField] int penalty;
    [SerializeField] GameObject zombiePrefab;
    [SerializeField] GameObject staticManPrefab;
    public bool powerOutage { get; private set; }
    [SerializeField] List<Light> lights;
    Coroutine poRoutine;

    [SerializeField] List<string> uniqueDialogue;
    [SerializeField] List<Transform> zombieSpawns;
    [SerializeField] List<Transform> staticManSpawns;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        phase = 0;
        score = 0;
        penalty = 0;
        powerOutage = false;

        SetPhase(phase);
    }

    private void Update()
    {
        //lights.SetActive(!powerOutage);

        if (powerOutage)
        {
            //Spawn Zombies
            if (phase >= 2)
            {
                //GameObject zombieObj = Instantiate(zombiePrefab, zombieSpawns[Random.Range(0, zombieSpawns.Count - 1)].position, Quaternion.identity);
                DialogueController.instance.UpdateText("[TODO]: Spawn zombie");
            }

            //Spawn Static Man
            if (phase >= 3)
            {
                //GameObject staticManObj = Instantiate(staticManPrefab, staticManSpawns[Random.Range(0, staticManSpawns.Count - 1)].position, Quaternion.identity);
                DialogueController.instance.UpdateText("[TODO]: Spawn static man");
            }
        }
    }

    void SetPhase(int phaseNum)
    {
        switch (phase)
        {
            case 0:
                //Initial tutorial
                //Radio gives initial message from supervisor explaining how to play
                DialogueController.instance.UpdateText("[TODO]: Put intro dialogue here");
                break;
            case 1:
                //Introduce power outages
                //Radio gives another message about the power outage, directs the player to use the fuse box to restore power
                //Quick look at zombie enemy as lights come back
                DialogueController.instance.UpdateText("[TODO]: Add dialogue talking about power outages and how to fix them");
                break;
            case 2:
                //Power outage starts on a timer
                //If power is out, spawn zombie and have them move closer during each outage
                //If zombie touches player, game over
                DialogueController.instance.UpdateText("[TODO]: Have radio voice talk about strange readings in the area");
                break;
            case 3:
                //Radio message is garbled and more ominous
                //Add 'bad' stations that will cause the player stress if they listen to them for too long
                //If the player is too stressed, start spawning Static man
                //Player must shred documents to lower stress (all documents while stressed must be shredded)
                //Power outages still happen on a timer
                DialogueController.instance.UpdateText("[TODO]: Radio voice starts talking about a dark king and black skies, 'he is coming. listen for his Voice'");
                break;
            case 4:
                //Final scenario
                DialogueController.instance.UpdateText("[TODO]: Add final dialogue from radio explaining that the rest of the site is falling and that the player has to finish the work");
                break;
            default:
                DialogueController.instance.UpdateText("[TODO]: Default Scenario Text");
                break;
        }
    }

    public void Success()
    {
        score++;

        int randVal = Random.Range(3, 8);
        if (score >= randVal)
        {
            phase++;

            if (phase > 0)
            {
                powerOutage = true;
                if (poRoutine == null)
                    poRoutine = StartCoroutine(PowerOutageRoutine(false));
                FuseBox.instance.SetBroken();
                score = 0;
            }
        }
    }

    public void Failure()
    {
        penalty++;
    }

    public void RestartPower()
    {
        if (poRoutine != null)
            StopCoroutine(poRoutine);
        poRoutine = StartCoroutine(PowerOutageRoutine(true));
        powerOutage = false;
        score = 0;
        penalty = 0;
        phase++;
        SetPhase(phase);
    }

    IEnumerator PowerOutageRoutine(bool turnLightsOn)
    {
        foreach (Light light in lights)
        {
            light.enabled = true;
            light.GetComponent<LightFlicker>().enabled = true;
        }

        yield return new WaitForSeconds(1.5f);

        foreach (Light light in lights)
        {
            light.enabled = turnLightsOn;
            light.GetComponent<LightFlicker>().enabled = false;
        }

        poRoutine = null;
    }
}
