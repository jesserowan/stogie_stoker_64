using System;
using UnityEngine;

public class WorldAudio : MonoBehaviour
{

    public AudioClip mainTheme;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource ??= GetComponent<AudioSource>();
        audioSource.clip = mainTheme;
    }

    public void PlayTheme()
    {
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopTheme()
    {
        audioSource.Stop();
    }
}
