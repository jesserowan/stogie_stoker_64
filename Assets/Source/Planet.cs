// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using System;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public Player player;
    public Transform Transform => transform;
    
    private void Awake()
    {
        Debug.Log("Awake()");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log($"Planet.Start(): player: {player}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
