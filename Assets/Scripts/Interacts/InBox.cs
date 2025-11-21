using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InBox : InteractObject
{
    public static InBox instance;

    [Header("Door Variables")]
    [SerializeField] GameObject door;
    private Quaternion startRotation;
    private Quaternion endRotation;
    [SerializeField] float rotationSpeed = 1.0f;
    private float _lerpTime = 0f;

    [Header("Document Variables")]
    [SerializeField] GameObject documentObj;
    [SerializeField] public List<Document> documents;
    [SerializeField] float documentGenTime = 12.5f;
    [SerializeField] TMP_Text documentCount;

    [Header("Air Variables")]
    [SerializeField] GameObject airArrow;
    [SerializeField] float arrowRotSpeed = 1.0f;
    public float airTime { get; private set; } //; = 30f;
    private Quaternion arrowStartRotation, arrowEndRotation;
    private float _arrowLerpTime = 0f;

    [Header("Audio Variables")]
    [SerializeField] AudioSource doorAudio;
    [SerializeField] AudioSource documentAudio;
    [SerializeField] AudioClip doorOpen, takeDocument;


    float baseTime;
    float aTimer;

    public void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        baseTime = 0f;
        airTime = 30f;
        aTimer = airTime;
        documentObj.SetActive(false);

        startRotation = door.transform.rotation;
        endRotation = Quaternion.Euler(90, 0, 0);

        arrowRotSpeed = 250f / airTime / 60f / 5f;
        arrowStartRotation = Quaternion.Euler(-180, 0, 0);
        arrowEndRotation = Quaternion.Euler(-180, 0, 250);

        doorAudio.clip = doorOpen;
        documentAudio.clip = takeDocument;
    }

    public override void Update()
    {
        base.Update();
        if (GameplayController.instance.state == GameplayController.State.gameplay)
        {
            //If less than max documents, countdown to generate a new document
            if (documents.Count < 5)
            {
                baseTime += Time.deltaTime;
                if (baseTime >= documentGenTime)
                {
                    baseTime = 0f;
                    GenerateNewDocument();                    
                }
            }

            //Air level and arrow rotation
            if (documents.Count > 0)
            {
                _arrowLerpTime += Time.deltaTime * arrowRotSpeed;
                airArrow.transform.localRotation = Quaternion.Slerp(arrowStartRotation, arrowEndRotation, _arrowLerpTime);
                if (_arrowLerpTime >= 1.0f)
                {
                    _arrowLerpTime = 1.0f; // Ensure it reaches the end exactly
                }

                print(2.5f * documents.Count);
                airTime -= Time.deltaTime / 2.25f * documents.Count;
                if (airTime <= 0)
                {
                    GameplayController.instance.Suffocate();
                }
            }
            else
            {
                _arrowLerpTime = 0f;
                airArrow.transform.localRotation = arrowStartRotation;
                airTime += Time.deltaTime * 5f;
                if (airTime >= aTimer)
                {
                    airTime = aTimer;
                }
            }
        }

        //Door open/close & audio
        if (documents.Count > 0)
        {
            _lerpTime += Time.deltaTime * rotationSpeed;
            door.transform.rotation = Quaternion.Slerp(startRotation, endRotation, _lerpTime);
            if (_lerpTime >= 1.0f)
            {
                _lerpTime = 1.0f; // Ensure it reaches the end exactly
            }
            else
            {
                if (!doorAudio.isPlaying)
                {
                    doorAudio.PlayOneShot(doorOpen);
                }
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
            else
            {
                if (!doorAudio.isPlaying)
                {
                    doorAudio.PlayOneShot(doorOpen);
                }
            }
        }

        //Toggle document model
        documentObj.SetActive(documents.Count > 0);
        documentCount.text = documents.Count.ToString();
    }

    public void GenerateNewDocument()
    {
        Document newDoc = new Document();
        newDoc.InitializeDoc();
        documents.Add(newDoc);

        float maxTime = GameplayController.instance.shiftNum >= 3 ? 9.25f : 10.5f;
        documentGenTime = Random.Range(3.5f, maxTime);
    }

    public override void Interact()
    {
        base.Interact();
        if (!PlayerController.instance.hasDocument && documents.Count > 0)
        {
            PlayerController.instance.SetCurrentDocument(documents[documents.Count - 1]);
            documents.RemoveAt(documents.Count - 1);
            documentAudio.PlayOneShot(takeDocument);
        }
    }

    public void Reset()
    {
        documents.Clear();
        documentObj.SetActive(false);
    }
}
