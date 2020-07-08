using UnityEngine.Audio;
using UnityEngine;

// this script is not on any object
[System.Serializable]
public class Sound
{
    //public string name;
    //[SerializeField]
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;

    public bool loop;

    //[HideInInspector]

    //public AudioSource source;

}
