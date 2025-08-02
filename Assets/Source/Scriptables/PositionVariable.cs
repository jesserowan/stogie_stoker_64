// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using UnityEngine;

[CreateAssetMenu(fileName = "PositionVarialbe", menuName = "Scriptables/Position Variable")]
public class PositionVariable : ScriptableObject
{
    [SerializeField] private Vector3 value;

    public Vector3 Value
    { get => value;
        set => this.value = value; }
}
