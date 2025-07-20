using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InBox : InteractObject
{
    [SerializeField] List<Document> documents;

    [SerializeField] float documentGenTime = 7.5f;
    float baseTime;

    public override void Start()
    {
        baseTime = 0f;
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        if (documents.Count <= 0)
        {
            baseTime += Time.deltaTime;
            if (baseTime >= documentGenTime)
            {
                baseTime = 0f;
                Document newDoc = new Document();
                newDoc.InitializeDoc();
                documents.Add(newDoc);
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
