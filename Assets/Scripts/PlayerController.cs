using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum States { idle, interacting, attacked };
    public States state;

    [SerializeField] Transform camTransform;
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] float checkDist = 10f;
    [SerializeField] LayerMask layer;
    [SerializeField] InteractObject interactObj;

    Vector2 viewPos;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        state = States.idle;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLook();
        InteractCheck();
    }

    void UpdateLook()
    {
        viewPos.x += Input.GetAxis("Mouse X") * mouseSensitivity;
        viewPos.y += Input.GetAxis("Mouse Y") * mouseSensitivity;

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
                interactObj = hit.transform.gameObject.GetComponent<InteractObject>();
                interactObj.highlighted = true;
            }
            else
            {
                interactObj = null;
            }
        }
    }
}
