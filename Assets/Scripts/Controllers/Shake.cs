using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public static Shake instance;

    private bool start = false;
    [SerializeField] private AnimationCurve curve;
    Vector3 startPosition;

    Coroutine shakeRoutine;

    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        startPosition = transform.position;
    }

    public void StartShake()
    {
        start = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (start)// && shakeRoutine == null)
        {
            transform.position = startPosition + Random.insideUnitSphere * curve.Evaluate(Time.deltaTime);
        }
    }
}
