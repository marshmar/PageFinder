using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class AudioClips
{
    public const string dashClipName = "dashFX";
}

[RequireComponent(typeof(AudioSource))]
public class AudioManager : Singleton<AudioManager>
{
    private AudioSource audioSource;
    public Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    public void PlayAudioOneShot(string clipName)
    {
        audioSource.PlayOneShot(audioClips[clipName]);
    }

    public void PlayAudioOneShot(string clipName, float duration)
    {
        audioSource.PlayOneShot(audioClips[clipName]);
    }
}
