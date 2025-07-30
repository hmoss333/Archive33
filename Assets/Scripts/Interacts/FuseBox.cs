using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseBox : InteractObject
{
    public static FuseBox instance;

    [SerializeField] List<Fuse> fuses;
    Renderer renderer;
    bool isBroken;

    public void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        renderer = GetComponent<Renderer>();
    }

    public override void Update()
    {
        base.Update();

        renderer.material.color = isBroken ? Color.red : Color.green;
    }

    public void SetBroken()
    {
        isBroken = true;

        for (int i = 0; i < 2; i++)
        {
            int randFuse = Random.Range(0, 3);
            fuses[randFuse].SetBroken();
        }
    } 

    public override void Interact()
    {
        base.Interact();
        for (int i = 0; i < fuses.Count; i++)
        {
            if (fuses[i].isBroken)
                break;
        }

        isBroken = false; //should trigger if all fuses are currently not broken
        GameplayController.instance.RestartPower();
    }
}
