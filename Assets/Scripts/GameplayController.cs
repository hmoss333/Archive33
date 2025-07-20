using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public static GameplayController instance;

    [SerializeField] GameObject lights;

    [SerializeField] int phase;
    [SerializeField] int score;
    [SerializeField] int penalty;
    [SerializeField] GameObject zombiePrefab;
    [SerializeField] GameObject staticManPrefab;
    public bool powerOutage { get; private set; }

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
        lights.SetActive(!powerOutage);

        if (powerOutage)
        {
            //Spawn Zombies
            if (phase >= 2)
            {
                for (int i = 0; i < phase; i++)
                {
                    GameObject zombieObj = Instantiate(zombiePrefab, zombieSpawns[Random.Range(0, zombieSpawns.Count - 1)].position, Quaternion.identity);
                }
            }

            //Spawn Static Man
            if (phase >= 3)
            {
                GameObject staticManObj = Instantiate(staticManPrefab, staticManSpawns[Random.Range(0, staticManSpawns.Count - 1)].position, Quaternion.identity);
            }
        }
    }

    void SetPhase(int phaseNum)
    {
        switch (phase)
        {
            case 0:
                //Initial tutorial
                break;
            case 1:
                //Introduce power outages
                break;
            case 2:
                //Introduce zombies
                break;
            case 3:
                //Introduce strange signals
                //Introduce static people hallucinations
                break;
            case 4:
                //Final scenario
                break;
            default:
                break;
        }
    }

    public void Success()
    {
        score++;

        int randVal = Random.Range(3, 8);
        if (score >= randVal)
        {
            print("TODO: flicker lights as they go off");
            powerOutage = true;
            score = 0;
        }

        if (powerOutage && score >= 3)
        {
            RestartPower();
        }
    }

    public void Failure()
    {
        penalty++;
    }

    public void RestartPower()
    {
        powerOutage = false;
        score = 0;
        penalty = 0;
        phase++;
        SetPhase(phase);
    }
}
