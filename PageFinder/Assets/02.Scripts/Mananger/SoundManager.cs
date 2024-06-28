using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] audioClip = new AudioClip[3];

    AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();  
    }

    public void PlayAudioClip(string name)
    {
        Debug.Log(name);
        switch(name)
        {
            case "CommonAttack":
                //audioSource.clip = audioClip[0];
                audioSource.PlayOneShot(audioClip[0]);
                break;
            case "SkillAttack":
                audioSource.PlayOneShot(audioClip[1]);
                break;
            case "Hit":
                audioSource.PlayOneShot(audioClip[2]);
                break;
            default:
                Debug.LogWarning(name);
                audioSource.PlayOneShot(audioClip[0]);
                break;
        }
    }
}
