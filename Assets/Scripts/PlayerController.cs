using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public enum States { idle, interacting, attacked };
    public States state;

    [SerializeField] Transform camTransform;
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] float checkDist = 10f;
    [SerializeField] LayerMask layer;
    [SerializeField] InteractObject interactObj;

    public bool hasDocument { get; private set; }
    [SerializeField] Document currentDoc;
    [SerializeField] GameObject documentPrefab;

    Vector2 viewPos;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        state = States.idle;
        hasDocument = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == States.idle)
        {
            UpdateLook();
            InteractCheck();
        }

        if (Input.GetMouseButtonUp(0) && interactObj != null)
        {
            interactObj.Interact();
        }

        documentPrefab.SetActive(hasDocument);
        SetState(States.idle);
    }

    void UpdateLook()
    {
        viewPos.x += Input.GetAxis("Mouse X") * mouseSensitivity / 20f;
        viewPos.y += Input.GetAxis("Mouse Y") * mouseSensitivity / 20f;

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
                    interactObj.highlighted = true;
                    Renderer R = hit.collider.GetComponent<Renderer>();
                    Outline OL = R.GetComponent<Outline>();
                    if (OL == null) // if no script is attached, attach one
                    {
                        print($"Adding autotransparent from {this.name}");
                        OL = R.gameObject.AddComponent<Outline>();
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

    public void SetCurrentDocument(Document newDoc)
    {
        currentDoc = newDoc;
        hasDocument = true;
    }

    public Document GetCurrentDocument()
    {
        return currentDoc;
    }

    public void RemoveCurrentDocument()
    {
        currentDoc = null;
        hasDocument = false;
        Radio.instance.InitializeFrequency();
    }

    public void SetState(States setState)
    {
        state = setState;
    }
}
