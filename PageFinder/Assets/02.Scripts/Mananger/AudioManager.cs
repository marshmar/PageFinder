using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum AudioClipType
{
    Bgm,
    BaSfx,
    DashSfx,
    HitSfx
}

public enum SoundType
{
    Bgm,
    Effect,
    MaxCount // Sound Enum의 개수 체크
}

public static class Sound
{
    #region Player
    // BasicAttack
    public const short attack1Sfx = 0;
    public const short attack2Sfx = 1;
    public const short attack3Sfx = 2;

    // Dash
    public const short dashVfx1 = 0;
    #endregion

    #region BGM
    //public const string bgmPath = "Sounds/PageFinder Title_02";
    public const short bgm1 = 0;
    #endregion

    #region Hit
    public const short hit1Sfx = 0;
    public const short hit2Sfx = 1;
    public const short hit3Sfx = 2;
    #endregion

}

public class AudioManager : Singleton<AudioManager>, IListener
{
    [SerializeField] private AudioClip[] Bgms;
    [SerializeField] private AudioClip[] BaSfx;
    [SerializeField] private AudioClip[] HitSfx;
    [SerializeField] private AudioClip[] DashSfx;

    private AudioSource[] audioSources = new AudioSource[(int)SoundType.MaxCount];

    private void Start()
    {
        Init();
        Play(Sound.bgm1, AudioClipType.Bgm);
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

    public void Play(AudioClip audioClip, SoundType type, float pitch = 1.0f)
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
        else
        {
            AudioSource audioSource = audioSources[(int)SoundType.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void Play(short index, AudioClipType type, float pitch = 1.0f)
    {
        System.Tuple<AudioClip, SoundType> audio = GetAudioClip(index, type);
        Play(audio.Item1, audio.Item2, pitch);
    }


    System.Tuple<AudioClip, SoundType> GetAudioClip(short index, AudioClipType type)
    {
        AudioClip audioClip = null;
        SoundType soundType = SoundType.Effect;
        switch (type)
        {
            case AudioClipType.Bgm:
                audioClip = Bgms[index];
                soundType = SoundType.Bgm;
                break;
            case AudioClipType.HitSfx:
                audioClip = HitSfx[index];
                soundType = SoundType.Effect;
                break;
            case AudioClipType.BaSfx:
                audioClip = BaSfx[index];
                soundType = SoundType.Effect;
                break;
            case AudioClipType.DashSfx:
                audioClip = DashSfx[index];
                soundType = SoundType.Effect;
                break;
        }

        if (audioClip == null)
            Debug.LogError($"AudioClip Missing !");

        return new System.Tuple<AudioClip, SoundType>(audioClip, soundType);
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
