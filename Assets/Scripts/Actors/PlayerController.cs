using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public enum States { idle, interacting };
    public States state;

    [SerializeField] Transform camTransform;
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] float checkDist = 10f;
    [SerializeField] LayerMask layer;
    InteractObject interactObj;

    public bool hasDocument { get; private set; }
    [SerializeField] Document currentDoc;
    [SerializeField] GameObject documentPrefab;
    [SerializeField] TMP_Text documentInstructions;
    [SerializeField] TMP_Text documentText;

    Vector2 viewPos;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        state = States.idle;
        hasDocument = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == States.idle)
        {
            UpdateLook();
        }

        if (GameplayController.instance.state == GameplayController.State.gameplay)
        {
            InteractCheck();

            if (Input.GetMouseButtonUp(0)
                && interactObj != null)
            {
                interactObj.Interact();
            }
        }

        documentPrefab.SetActive(hasDocument);
        SetState(States.idle);
    }

    void UpdateLook()
    {
        viewPos.x += Input.GetAxis("Mouse X") * mouseSensitivity / 2f;
        viewPos.y += Input.GetAxis("Mouse Y") * mouseSensitivity / 2f;

        viewPos.y = Mathf.Clamp(viewPos.y, -89f, 89f);

        camTransform.localRotation = Quaternion.Euler(-viewPos.y, 0, 0);
        transform.localRotation = Quaternion.Euler(0, viewPos.x, 0);
    }

    void InteractCheck()
    {
        Ray ray = new Ray(camTransform.position, camTransform.forward);
        RaycastHit hit;

        if (state != States.interacting)
        {
            if (Physics.Raycast(ray, out hit, checkDist, layer))
            {
                try
                {
                    interactObj = hit.transform.gameObject.GetComponent<InteractObject>();
                    if (interactObj.enabled)
                    {
                        interactObj.highlighted = true;
                        Renderer R = hit.collider.GetComponent<Renderer>();
                        Outline OL = R.GetComponent<Outline>();
                        if (OL == null) // if no script is attached, attach one
                        {
                            OL = R.gameObject.AddComponent<Outline>();
                        }
                    }
                }
                catch (Exception e)
                {
                    print(e);
                }
            }
            else
            {
                interactObj = null;
            }
        }
    }


    //Document Functions
    public void SetCurrentDocument(Document newDoc)
    {
        currentDoc = newDoc;
        hasDocument = true;
        Radio.instance.GenerateActiveStations();

        if (currentDoc != null)
        {
            documentInstructions.text = "Station: " + Radio.instance.targetFrequency.ToString("F2");
            documentText.text = currentDoc.documentText;
        }
    }

    public Document GetCurrentDocument()
    {
        return currentDoc;
    }

    public void RemoveCurrentDocument()
    {
        currentDoc = null;
        hasDocument = false;
    }

    public Document.FileColor GetRandomColor()
    {
        System.Array enumValues = System.Enum.GetValues(typeof(Document.FileColor));

        // Generate a random index within the array's bounds
        int randomIndex = UnityEngine.Random.Range(0, enumValues.Length);

        // Retrieve the value at the random index and cast it to the enum type
        return (Document.FileColor)enumValues.GetValue(randomIndex);
    }

    public void SetState(States setState)
    {
        state = setState;
    }
}
