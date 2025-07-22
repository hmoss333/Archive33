using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InBox : InteractObject
{
    [SerializeField] GameObject documentObj;
    [SerializeField] List<Document> documents;

    [SerializeField] float documentGenTime = 7.5f;
    float baseTime;

    public override void Start()
    {
        baseTime = 0f;
        documentObj.SetActive(false);
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        if (GameplayController.instance.phase > 0 && documents.Count <= 0)
        {
            baseTime += Time.deltaTime;
            if (baseTime >= documentGenTime)
            {
                baseTime = 0f;
                GenerateNewDocument();
            }
        }

        documentObj.SetActive(documents.Count > 0);
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
