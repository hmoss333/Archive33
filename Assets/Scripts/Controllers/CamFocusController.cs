using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFocusController : MonoBehaviour
{
    public static CamFocusController instance;

    [SerializeField] float moveTime;
    Vector3 focusPos;
    Vector3 defaultPos;
    Vector3 targetPos;
    Quaternion targetRot;


    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        defaultPos = transform.position;
        focusPos = defaultPos;
    }

    //private void Update()
    //{
    //    if (transform.position != focusPos)
    //    {
    //        transform.position = Vector3.Lerp(transform.position, focusPos, focusPos != defaultPos ? moveTime * Time.deltaTime : 1f);
    //        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, focusPos != defaultPos ? moveTime * Time.deltaTime : 1f);
    //    }
    //}

    public void FocusTarget(Transform target)
    {
        targetPos = target.position;
        targetRot = target.rotation;
        focusPos = targetPos;
    }

    public void FocusReset()
    {
        focusPos = defaultPos;
    }
}
