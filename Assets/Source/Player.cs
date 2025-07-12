using System;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Planet planet;
    public Transform Transform => transform;
    
    private void Awake()
    {
        Debug.Log("Awake()");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log($"Start(): planet: {planet}");
    }
}
