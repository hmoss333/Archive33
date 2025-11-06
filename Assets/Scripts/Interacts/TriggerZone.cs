using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    [Header("Event Triggers")]
    public UnityEvent m_OnTrigger = new UnityEvent();
    bool hasTriggered;

    private void Start()
    {
        hasTriggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.tag == "Player")
        {
            m_OnTrigger.Invoke();
            hasTriggered = true;
        }
    }
}
