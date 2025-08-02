// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class Planet : MonoBehaviour
{
    public SphereCollider sphereCol;
    public Biome biome;

    private void OnEnable()
    {
        sphereCol ??= GetComponent<SphereCollider>() 
                      ?? gameObject.AddComponent<SphereCollider>();
        sphereCol.radius = Constants.Instance.worldRadius;
    }

}

