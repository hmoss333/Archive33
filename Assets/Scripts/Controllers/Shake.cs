using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public static Shake instance;

    private bool start = false;
    [SerializeField] private AnimationCurve curve;
    private float duration = 1f;

    Coroutine shakeRoutine;

    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (start && shakeRoutine == null)
        {
            start = false;
            shakeRoutine = StartCoroutine(Shaking());
        }
    }

    public void StartShake()
    {
        duration = 1f;
        start = true;
    }

    public void StartShake(float durationTime)
    {
        duration = durationTime;
        start = true;
    }

    IEnumerator Shaking()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPosition;
        shakeRoutine = null;
    }
}
