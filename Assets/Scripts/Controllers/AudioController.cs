using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioController : MonoBehaviour
{
    public static AudioController instance; 

    [SerializeField] public float volume { get; private set; }
    //[SerializeField] List<AudioSource> audioSources;
    [SerializeField] List<AudioObject> audioObjs;


    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        volume = PlayerPrefs.GetFloat("volume", 1f);


        List<AudioSource> audioSources = FindObjectsOfType<AudioSource>().ToList();
        for (int i = 0; i < audioSources.Count; i++)
        {
            AudioObject ao = new AudioObject();
            ao.source = audioSources[i];
            ao.defaultVolume = audioSources[i].volume;
            audioObjs.Add(ao);
        }
    }


    public void ModifyVolume()
    {
        for (int i = 0; i < audioObjs.Count; i++)
        {
            audioObjs[i].source.volume = volume;
        }
    }

    public void ModifyVolume(float volumeToSet)
    {
        for (int i = 0; i < audioObjs.Count; i++)
        {
            audioObjs[i].source.volume = volumeToSet;
        }
    }

    public void ResetVolume()
    {
        for (int i = 0; i < audioObjs.Count; i++)
        {
            audioObjs[i].source.volume = audioObjs[i].defaultVolume;
        }
    }
}

[System.Serializable]
class AudioObject
{
    public AudioSource source;
    public float defaultVolume;
}