using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InBox : InteractObject
{
    [SerializeField] GameObject documentObj;
    [SerializeField] List<Document> documents;

    [SerializeField] float documentGenTime = 7.5f;
    [SerializeField] float airTime = 30f;
    float baseTime;
    float aTimer;

    public void Start()
    {
        baseTime = 0f;
        aTimer = airTime;
        documentObj.SetActive(false);
    }

    public override void Update()
    {
        base.Update();
        if (GameplayController.instance.state == GameplayController.State.gameplay)
        {
            baseTime += Time.deltaTime;
            if (baseTime >= documentGenTime)
            {
                baseTime = 0f;
                GenerateNewDocument();
            }

            documentObj.SetActive(documents.Count > 0);
            if (documents.Count > 0)
            {
                airTime -= Time.deltaTime / 2f;
                if (airTime <= 0)
                {
                    airTime = aTimer;
                    GameplayController.instance.SetState(GameplayController.State.death);
                }
            }
            else
            {
                airTime = aTimer;
            }
        }    
    }

    public void GenerateNewDocument()
    {
        Document newDoc = new Document();
        newDoc.InitializeDoc();
        documents.Add(newDoc);
    }

    public override void Interact()
    {
        base.Interact();
        if (!PlayerController.instance.hasDocument && documents.Count > 0)
        {
            PlayerController.instance.SetCurrentDocument(documents[documents.Count - 1]);
            documents.RemoveAt(documents.Count - 1);
        }
    }
}
