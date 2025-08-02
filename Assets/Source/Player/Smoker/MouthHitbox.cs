// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MouthHitbox : MonoBehaviour
{
    [SerializeField] public PositionVariable mouthPosition;
    [SerializeField] public Vector3 offset = new (0f, 0f, -0.15f);

    private void Update()
    {
        transform.position = mouthPosition.Value + offset;
    }
}
