using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Drives the ScreenEdgeTentacles material on a full-screen UI Graphic (e.g. Image).
/// Canvas must be Screen Space - Camera, and URP "Opaque Texture" must be enabled.
/// </summary>
[RequireComponent(typeof(Graphic))]
public class ScreenEdgeTentacles : MonoBehaviour
{
    [Range(0f, 1f)] public float intensity = 0f;
    public bool randomizeSeedOnEnable = true;

    static readonly int IntensityId = Shader.PropertyToID("_Intensity");
    static readonly int SeedId = Shader.PropertyToID("_Seed");

    Graphic graphic;
    Material original;
    Material instance;

    void OnEnable()
    {
        graphic = GetComponent<Graphic>();
        original = graphic.material;
        instance = new Material(original);
        graphic.material = instance;

        if (randomizeSeedOnEnable)
            instance.SetFloat(SeedId, Random.value * 100f);

        Apply();
    }

    void OnDisable()
    {
        if (graphic != null) graphic.material = original;
        if (instance != null) Destroy(instance);
    }

    void Update() => Apply();

    /// <summary>Hook this to animations, UnityEvents or timelines. Clamped to 0-1.</summary>
    public void SetIntensity(float value) => intensity = Mathf.Clamp01(value);

    /// <summary>Picks a new random tentacle layout. Calling this mid-effect will make the shapes jump.</summary>
    public void Reseed()
    {
        if (instance != null) instance.SetFloat(SeedId, Random.value * 100f);
    }

    void Apply()
    {
        if (instance == null) return;
        instance.SetFloat(IntensityId, Mathf.Clamp01(intensity));
        graphic.SetMaterialDirty();
    }
}
