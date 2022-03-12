using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    AudioSource audio_source;
    public AudioClip audio0;

    private void Start()
    {
        audio_source = GetComponent<AudioSource>();
    }
    void playAudio(AudioClip clip)
    {
        audio_source.clip = clip;
        audio_source.Play();
    }

    public void playAudio0()
    {
        playAudio(clip: audio0);
    }
}
