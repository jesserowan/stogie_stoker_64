// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using System;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    public MeshRenderer obstacleRenderer;
    
    [SerializeField]
    public MeshFilter obstacleMesh;

    private void OnEnable()
    {
        obstacleMesh = GetComponentInChildren<MeshFilter>();
        obstacleRenderer = GetComponentInChildren<MeshRenderer>();
    }

    private void Awake()
    {
        // Debug.Log($"Obstacle::{gameObject.name}.Awake()");
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    public void RotateAxis(Track track)
    {
        var child = transform.GetChild(0);
        if (track == Track.Z) child.rotation = Quaternion.Euler(-90, 0, 0);
        else child.rotation = Quaternion.Euler(-90, 90, 0);
    }
}
