using System;
using UnityEngine;

public class Stogie : MonoBehaviour
{
    private static readonly int EmissiveColor = Shader.PropertyToID("_EmissionColor");
    public GameObject ember;
    public Color emissiveColor;
    public float intensity = 1;
    public float glowSpeed = 3f;
    public float minIntensity;
    public float maxIntensity;

    public Renderer rend;

    public bool smoking;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = ember.GetComponent<Renderer>();
        emissiveColor = rend.material.GetColor(EmissiveColor);
        smoking = false;
        intensity = minIntensity;
        rend.material.SetColor(EmissiveColor, emissiveColor * intensity);
    }

    // Update is called once per frame
    void Update()
    {
        if (smoking)
        {
            Debug.Log($"Currently smoking, should be getting brighter");
            intensity = Mathf.Clamp(intensity + glowSpeed, minIntensity, maxIntensity);
            rend.material.SetColor(EmissiveColor, emissiveColor * intensity);
        }
        else
        {
            intensity = Mathf.Clamp(intensity - glowSpeed * glowSpeed, minIntensity, maxIntensity);
            Debug.Log($"Intensity: {intensity}");
            rend.material.SetColor(EmissiveColor, emissiveColor * intensity);
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (smoking) return;
    //     if (other.CompareTag("Mouth")) smoking = true;
    // }
    //
    // private void OnTriggerExit(Collider other)
    // {
    //     if (!smoking) return;
    //     if (other.CompareTag("Mouth")) smoking = false;
    // }
}
