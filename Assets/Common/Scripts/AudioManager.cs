using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public List<AudioClip> audioClips;
    public AudioSource audioSource1;
    public AudioSource audioSource2;
    public void PlayBattleSound()
    {
        audioSource1.clip = audioClips[0];
        audioSource1.loop = true;
        audioSource1.volume = 0.8f;
        audioSource1.Play();
    }

    public void PlayFreeMoveSound()
    {
        audioSource1.clip = audioClips[1];
        audioSource1.loop = true;
        audioSource1.volume = 0.8f;
        audioSource1.Play();
    }

    public void PlayPassSound()
    {
        audioSource2.clip = audioClips[2];
        audioSource2.loop = false;
        audioSource2.Play();
    }

    public void SoundsStop()
    {
        audioSource1.Stop();
        audioSource2.Stop();
    }
}
