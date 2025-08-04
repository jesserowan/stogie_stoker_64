using System;
using UnityEngine;

public class WorldAudio : MonoBehaviour
{

    public AudioClip mainTheme;
    private AudioSource audioSource;
    private AudioLowPassFilter lowPassFilter;

    private void Awake()
    {
        audioSource ??= GetComponent<AudioSource>();
        audioSource.clip = mainTheme;
        lowPassFilter = GetComponent<AudioLowPassFilter>();
    }

    public void PlayTheme()
    {
        audioSource.loop = true;
        lowPassFilter.cutoffFrequency = 5000;
        lowPassFilter.enabled = false;
        audioSource.Play();
    }

    public void ModulateLowPass(float normalizedValue)
    {
        if (!lowPassFilter.enabled) lowPassFilter.enabled = true;
        lowPassFilter.cutoffFrequency = Mathf.Lerp(0, 5000, normalizedValue);
    }

    public void StopTheme()
    {
        audioSource.Stop();
    }
}
