using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO: create a Kylebot script
/// Bot script should handle moving into position and waiting for a corrupted document
/// If a corrupted document is not given within X seconds, kill the player
/// If a corrupted document IS given, the bot will leave the room and reset his position/wait to be called again
public class BotController : InteractObject
{
    public static BotController instance;

    public enum State { idle, moveToPosition, waitForDoc, exitPosition, attack }
    public State state;

    [SerializeField] float speed;
    [SerializeField] float waitTime;
    [SerializeField] List<Transform> movePoints;


    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Update()
    {
        switch (state)
        {
            case State.idle:
                break;
            case State.moveToPosition:
                break;
            case State.waitForDoc:
                break;
            case State.exitPosition:
                break;
            case State.attack:
                break;
            default:
                break;
        }
    }

    public void SetState(State setState)
    {
        state = setState;
    }

    public void CallBot()
    {
        //TODO: add logic here to start bot movement into position for recieving a corrupted document
        SetState(State.moveToPosition);
    }

    public override void Interact()
    {
        if (state == State.waitForDoc)
        {
            base.Interact();
            //TODO: if the player has a corrupted document they can hand it off to the bot here
            /// If the document is not corrupted, count as a failure (not instant death)
        }
    }
}


