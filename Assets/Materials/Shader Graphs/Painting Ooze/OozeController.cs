using UnityEngine;

public class OozeController : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] float seepSpeed = 0.4f;   // units of progress per second
    [SerializeField] float retractSpeed = 1.0f;

    static readonly int ProgressId = Shader.PropertyToID("_Progress");
    [SerializeField] float current, target;
    [SerializeField] Material mat;

    void Awake()
    {
        mat = sr.material; // instanced copy, so multiple oozes don't share state
        mat.SetFloat(ProgressId, 0f);
        sr.enabled = true;//false;
        mat.SetVector("_BoundsMin", sr.sprite.bounds.min);
        mat.SetVector("_Size", sr.sprite.bounds.size);
    }

    public void SetActive(bool on) => target = on ? 1f : 0f;

    void Update()
    {
        if (Mathf.Approximately(current, target)) return;

        float speed = target > current ? seepSpeed : retractSpeed;
        current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);
        mat.SetFloat(ProgressId, current);
        sr.enabled = current > 0f;
    }

    void OnDestroy() { if (mat) Destroy(mat); }
}