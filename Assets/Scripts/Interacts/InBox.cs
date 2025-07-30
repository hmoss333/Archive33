using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InBox : InteractObject
{
    [Header("Door Variables")]
    [SerializeField] GameObject door;
    private Quaternion startRotation;
    private Quaternion endRotation;
    [SerializeField] float rotationSpeed = 1.0f;
    private float _lerpTime = 0f;

    [Header("Document Variables")]
    [SerializeField] GameObject documentObj;
    [SerializeField] List<Document> documents;
    [SerializeField] float documentGenTime = 7.5f;

    [Header("Air Variables")]
    [SerializeField] GameObject airArrow;
    [SerializeField] float airTime = 30f;


    float baseTime;
    float aTimer;

    public void Start()
    {
        baseTime = 0f;
        aTimer = airTime;
        documentObj.SetActive(false);

        startRotation = door.transform.rotation;
        endRotation = Quaternion.Euler(90, 0, 0);
    }

    private void OnDisable()
    {
        documents.Clear();
        documentObj.SetActive(false);
    }

    public override void Update()
    {
        base.Update();
        if (GameplayController.instance.state == GameplayController.State.gameplay)
        {
            if (documents.Count < 5)
            {
                baseTime += Time.deltaTime;
                if (baseTime >= documentGenTime)
                {
                    baseTime = 0f;
                    GenerateNewDocument();
                }
            }

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

        if (documents.Count > 0)
        {
            _lerpTime += Time.deltaTime * rotationSpeed;
            door.transform.rotation = Quaternion.Slerp(startRotation, endRotation, _lerpTime);
            if (_lerpTime >= 1.0f)
            {
                _lerpTime = 1.0f; // Ensure it reaches the end exactly
            }
        }
        else
        {
            _lerpTime -= Time.deltaTime * rotationSpeed;
            door.transform.rotation = Quaternion.Slerp(startRotation, endRotation, _lerpTime);
            if (_lerpTime <= 0f)
            {
                _lerpTime = 0f; // Ensure it reaches the start exactly
            }
        }

        //airArrow.transform.rotation = 
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
