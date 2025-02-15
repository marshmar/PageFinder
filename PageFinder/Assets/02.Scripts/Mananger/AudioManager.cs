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
    public const string attack1SfxPath = "Sounds/sfx_basic_attack_01";
    public const string attack2SfxPath = "Sounds/sfx_basic_attack_02";
    public const string attack3SfxPath = "Sounds/sfx_basic_attack_03";

    // Dash
    public const string dashVfx1Path = "Sounds/sfx_ink_dash_01";
    #endregion

    #region BGM
    public const string bgmPath = "Sounds/PageFinder Title_02";
    #endregion

    #region Hit
    public const string hit1SfxPath = "Sounds/Hit_01";
    public const string hit2SfxPath = "Sounds/Hit_02";
    public const string hit3SfxPath = "Sounds/Hit_03";
    #endregion

}
public class AudioManager : Singleton<AudioManager>, IListener
{
    private AudioSource[] audioSources = new AudioSource[(int)SoundType.MaxCount];
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    private void Start()
    {
        Init();
        Play(SoundPath.bgmPath, SoundType.Bgm);
        EventManager.Instance.AddListener(EVENT_TYPE.UI_Changed, this);
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

    public void BGMPause()
    {
        audioSources[0].Pause();
    }

    public void BGMUnPause()
    {
        audioSources[0].UnPause();
    }

    public void OnEvent(EVENT_TYPE eventType, Component Sender, object Param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.UI_Changed:
                CheckBGMStop((UIType)Param);
                break;
        }
    }

    private void CheckBGMStop(UIType param)
    {
        switch (param)
        {
            case UIType.Battle:
            case UIType.RiddleBook:
            case UIType.Shop:
            case UIType.Reward:
            case UIType.BackDiaryFromReward:
            case UIType.BackDiaryFromShop:
                BGMUnPause();
                break;
            case UIType.Setting:
            case UIType.Diary:
            case UIType.Help:
            case UIType.RewardToDiary:
            case UIType.ShopToDiary:
                BGMPause();
                break;
        }
    }
}
