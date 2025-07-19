// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Collider collider;

    private void OnValidate()
    {
        collider = GetComponentInChildren<Collider>();
        
    }

    private void Awake()
    {
        // Debug.Log($"Obstacle::{gameObject.name}.Awake()");
    }

    private void OnCollisionEnter(Collision other)
    {
        
    }

    public void RotateAxis(Track track)
    {
        var child = transform.GetChild(0);
        if (track == Track.Z) child.rotation = Quaternion.Euler(-90, 0, 0);
        else child.rotation = Quaternion.Euler(-90, 90, 0);
    }
}
