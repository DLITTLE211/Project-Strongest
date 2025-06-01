using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Character_AudioManager : MonoBehaviour
{
    [SerializeField] private Character_Base _base;
    [SerializeField] private AudioSource oneShotSource;
    [SerializeField] private AudioSource playSource;

    public void PlayNextAudioClip(AudioClipData newAudio) 
    {
        if (newAudio.isOneShot) 
        {
            oneShotSource.volume = newAudio.volume;
            oneShotSource.loop = newAudio.isLooped;
            oneShotSource.PlayOneShot(newAudio.nextPlayedClip);
        }
        else 
        {
            playSource.volume = newAudio.volume;
            playSource.loop = newAudio.isLooped;
            playSource.clip = newAudio.nextPlayedClip;
            playSource.Play();
        }
    }
    public void StopAudio()
    {
        if (oneShotSource.isPlaying) 
        {
            oneShotSource.Stop();
        }
        if (playSource.isPlaying) 
        {
            playSource.Stop();
        }
    }
}
[Serializable]
public class AudioClipData 
{
    public AudioClip nextPlayedClip;
    public float volume;
    public bool isLooped;
    public bool isOneShot;
    public AudioClipData(AudioClip _nextClip,bool _isOneShot, float _volume = 1f, bool _isLooped = false) 
    {
        nextPlayedClip = _nextClip;
        volume = _volume;
        isLooped = _isLooped;
        isOneShot = _isOneShot;
    }
}
