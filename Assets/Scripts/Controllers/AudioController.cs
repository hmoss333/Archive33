using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioController : MonoBehaviour
{
    public static AudioController instance; 

    [SerializeField] public float volume { get; private set; }
    [SerializeField] List<AudioSource> audioSources;


    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        volume = PlayerPrefs.GetFloat("volume", 1f);
        audioSources = FindObjectsOfType<AudioSource>().ToList();
    }


    public void ModifyVolume()
    {
        for (int i = 0; i < audioSources.Count; i++)
        {
            audioSources[i].volume = volume;
        }
    }

    public void ModifyVolume(float volumeToSet)
    {
        for (int i = 0; i < audioSources.Count; i++)
        {
            audioSources[i].volume = volumeToSet;
        }
    }
}
