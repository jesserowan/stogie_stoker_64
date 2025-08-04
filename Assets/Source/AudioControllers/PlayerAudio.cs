// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public enum DiscreteAudio
{
    Jump,
    Slide,
    Impact,
    Fall,
    LaneSwitch,
    CoughA,
    CoughB,
    CoughC,
    Breathing,
}

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioSource loopingSource;
    [SerializeField] private AudioSource discreteSource;

    [Serializable]
    public class AudioData
    {
        public AudioClip clip;
        public DiscreteAudio audioType;
    }
    
    [SerializeField] public AudioData[] clipMap = new AudioData[8];
    
    private void OnEnable()
    {
        GameManager.OnImpact += Impact;
        GameManager.OnLaneSwitch += LaneSwitch;
        GameManager.OnLose += Fall;
        GameManager.OnJump += Jump;
        GameManager.OnSlide += Slide;
    }

    private void OnDisable()
    {
        GameManager.OnImpact -= Impact;
        GameManager.OnLaneSwitch -= LaneSwitch;
        GameManager.OnLose -= Fall;
        GameManager.OnJump -= Jump;
        GameManager.OnSlide -= Slide;
    }

    private void Start()
    {
        coughing = StartCoroutine(Coughing());
    }

    private void Update()
    {
        if (coughing == null) coughing = StartCoroutine(Coughing());
    }

    public void PlayClip(DiscreteAudio audioType)
    {
        AudioClip clip = clipMap.First(data => data.audioType == audioType).clip;
        discreteSource.PlayOneShot(clip);
    }

    private Coroutine coughing;
    private IEnumerator Coughing()
    {
        var rand = Random.Range(3, 10);
        yield return new WaitForSeconds(rand);
        Cough();
        coughing = null;
    }

    public void Cough()
    {
        var rand = Random.Range(0, 4);
        AudioClip cough = rand switch
        {
            0 => clipMap.First(data => data.audioType == DiscreteAudio.CoughA).clip,
            1 => clipMap.First(data => data.audioType == DiscreteAudio.CoughB).clip,
            2 => clipMap.First(data => data.audioType == DiscreteAudio.CoughC).clip,
            _ => clipMap.First(data => data.audioType == DiscreteAudio.Breathing).clip
        };
        loopingSource.PlayOneShot(cough);
    }

    public void Jump() => PlayClip(DiscreteAudio.Jump);
    public void Slide() => PlayClip(DiscreteAudio.Slide);
    public void Fall() => PlayClip(DiscreteAudio.Fall);
    public void Impact() => PlayClip(DiscreteAudio.Impact);
    public void LaneSwitch(Lane lane) => PlayClip(DiscreteAudio.LaneSwitch);
    
}
