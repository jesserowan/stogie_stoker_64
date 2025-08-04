using System;
using UnityEngine;

public class WorldAudio : MonoBehaviour
{
    public AudioClip mainTheme;
    private AudioSource audioSource;
    private AudioLowPassFilter lowPassFilter;
    private AudioHighPassFilter hiPassFilter;

    private void Awake()
    {
        audioSource ??= GetComponent<AudioSource>();
        audioSource.clip = mainTheme;
        lowPassFilter = GetComponent<AudioLowPassFilter>();
        hiPassFilter = GetComponent<AudioHighPassFilter>();
    }

    public void PlayTheme()
    {
        audioSource.loop = true;
        lowPassFilter.cutoffFrequency = 11000;
        lowPassFilter.enabled = false;
        hiPassFilter.cutoffFrequency = 7000;
        lowPassFilter.enabled = false;
        audioSource.Play();
    }

    public void ModulateLowPass(float normalizedValue)
    {
        if (!lowPassFilter.enabled) lowPassFilter.enabled = true;
        lowPassFilter.cutoffFrequency = Mathf.Lerp(0, 7000, normalizedValue);
    }

    public void ModulateHighPass(float normalizedValue)
    {
        if (!hiPassFilter.enabled) hiPassFilter.enabled = true;
        hiPassFilter.cutoffFrequency = Mathf.Lerp(7000, 0, normalizedValue);
    }

    public void StopTheme()
    {
        audioSource.Stop();
    }
}
