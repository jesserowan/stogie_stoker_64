// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class Planet : MonoBehaviour
{
    public new SphereCollider collider;

    private void OnEnable()
    {
        if (collider == null)
        {
            collider = GetComponent<SphereCollider>();
        }

        collider.radius = Constants.Instance.worldRadius;
    }

}

