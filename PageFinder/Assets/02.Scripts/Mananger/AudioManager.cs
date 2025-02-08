using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum SoundType
{
    Bgm,
    Effect,
    MaxCount // Sound Enum의 개수 체크
}

public static class SoundPath
{
    #region Player
    // BasicAttack
    public const string attack1VfxPath = "Sounds/sfx_basic_attack_01";
    public const string attack2VfxPath = "Sounds/sfx_basic_attack_02";
    public const string attack3VfxPath = "Sounds/sfx_basic_attack_03";

    // Dash
    public const string dashVfx1Path = "Sounds/sfx_ink_dash_01";
    #endregion

    #region BGM
    public const string bgmPath = "Sounds/BattleBGM";
    #endregion

}
public class AudioManager : Singleton<AudioManager>
{
    private AudioSource[] audioSources = new AudioSource[(int)SoundType.MaxCount];
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    private void Start()
    {
        Init();
        Play(SoundPath.bgmPath, SoundType.Bgm);
    }

    private void OnDestroy()
    {
        Clear();
    }

    private void Init()
    {
        string[] soundTypes = System.Enum.GetNames(typeof(SoundType));
        for(int i = 0; i < soundTypes.Length -1; i++)
        {
            GameObject soundPlayer = new GameObject { name = soundTypes[i] };
            audioSources[i] = soundPlayer.AddComponent<AudioSource>();
            soundPlayer.transform.parent = this.transform;
        }

        audioSources[(int)SoundType.Bgm].loop = true; // bgm 재생기는 반복 재생
    }

    public void Clear()
    {
        foreach(AudioSource audioSource in audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
    }

    public void Play(AudioClip audioClip, SoundType type = SoundType.Effect, float pitch = 1.0f)
    {
        if (audioClip is null) return;

        if(type == SoundType.Bgm)
        {
            AudioSource audioSource = audioSources[(int)SoundType.Bgm];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
            Debug.Log("BGM 재생");
        }
        else if(type == SoundType.Effect)
        {
            AudioSource audioSource = audioSources[(int)SoundType.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void Play(string path, SoundType type = SoundType.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
    }

    AudioClip GetOrAddAudioClip(string path, SoundType type = SoundType.Effect)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}"; // Sound 폴더 안에 없으면 Sound 폴더안에 저장

        AudioClip audioClip = null;

        if(type == SoundType.Bgm)
        {
            if(audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Resources.Load<AudioClip>(path);
                audioClips.Add(path, audioClip);
            }
        }
        else
        {
            if(audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Resources.Load<AudioClip>(path);
                audioClips.Add(path, audioClip);
            }
        }

        if (audioClip == null)
            Debug.LogError($"AudioClip Missing ! {path}");

        return audioClip;
    }
}
