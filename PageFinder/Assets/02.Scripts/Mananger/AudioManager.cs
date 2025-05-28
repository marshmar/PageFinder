using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum AudioClipType
{
    Bgm,
    BaSfx,
    DashSfx,
    HitSfx,
    SequenceSfx,
    InkMarkSfx,
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
    public const short battleBgm = 0;
    public const short questBgm = 1;
    public const short commaBgm = 2;
    #endregion

    #region Hit
    public const short hit1Sfx = 0;
    public const short hit2Sfx = 1;
    public const short hit3Sfx = 2;
    #endregion

    #region Sequence
    public const short dead = 0;
    public const short defeat = 1;
    public const short victory = 2;
    public const short end = 3;
    #endregion


    #region InkMark
    public const short swampCreated = 0;
    public const short swampDeleted = 1;
    public const short mistCreated = 2;
    public const short mistDeleted = 3;
    public const short fireCreated = 4;
    public const short fireDeleted = 5;
    #endregion
}

public class AudioManager : Singleton<AudioManager>, IListener
{
    [SerializeField] private AudioClip[] Bgms;
    [SerializeField] private AudioClip[] BaSfx;
    [SerializeField] private AudioClip[] HitSfx;
    [SerializeField] private AudioClip[] DashSfx;
    [SerializeField] private AudioClip[] SequenceSfx;
    [SerializeField] private AudioClip[] InkMarkSfx;

    private AudioSource[] audioSources = new AudioSource[(int)SoundType.MaxCount];

    private void Start()
    {
        Init();
        Play(Sound.battleBgm, AudioClipType.Bgm);
        // ToDo: UI Changed;
        EventManager.Instance.AddListener(EVENT_TYPE.Stage_Start, this);
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
        audioSources[(int)SoundType.Bgm].volume = 0.05f;
        audioSources[(int)SoundType.Effect].volume = 0.2f;

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
            case AudioClipType.SequenceSfx:
                audioClip = SequenceSfx[index];
                soundType = SoundType.Effect;
                break;
            case AudioClipType.InkMarkSfx:
                audioClip = InkMarkSfx[index];
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
            // // ToDo: UI Changed;
            case EVENT_TYPE.Stage_Start:
                Node node = (Node)Param;
                CheckBGM(node.type);
                break;
        }
    }

    private void CheckBGM(NodeType nextNode)
    {
        switch (nextNode)
        {
            case NodeType.Start:
            case NodeType.Battle_Normal:
            case NodeType.Battle_Elite:
            case NodeType.Battle_Elite1:
            case NodeType.Battle_Elite2:
            case NodeType.Treasure:
            case NodeType.Market:
            case NodeType.Boss:
                if (!CheckAudioIsPlaying(GetAudioClip(Sound.battleBgm, AudioClipType.Bgm).Item1))
                    Play(Sound.battleBgm, AudioClipType.Bgm);
                break;
            case NodeType.Comma:
                if (!CheckAudioIsPlaying(GetAudioClip(Sound.commaBgm, AudioClipType.Bgm).Item1))
                    Play(Sound.commaBgm, AudioClipType.Bgm);
                break;
            case NodeType.Quest:
                if (!CheckAudioIsPlaying(GetAudioClip(Sound.questBgm, AudioClipType.Bgm).Item1))
                    Play(Sound.questBgm, AudioClipType.Bgm);
                break;
        }
    }

    private bool CheckAudioIsPlaying(AudioClip clip)
    {
        return audioSources[0].clip == clip;
    }
}
