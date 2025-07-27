// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class Planet : MonoBehaviour
{
    public SphereCollider sphereCol;
    public Pole northPole;
    public Pole southPole;
    public Biome biome;

    private void OnEnable()
    {
        sphereCol ??= GetComponent<SphereCollider>() ?? gameObject.AddComponent<SphereCollider>();
        sphereCol.radius = Constants.Instance.worldRadius;

        if (northPole == null || southPole == null)
        {
            var existingPoles = GetComponentsInChildren<Pole>();
            if (existingPoles.Length > 0) {
                foreach (var pole in existingPoles) {
                    switch (pole.polarity) {
                        case Polarity.North when northPole == null: northPole = pole; break;
                        case Polarity.South when southPole == null: southPole = pole; break;
                    }
                }
            }
        }
    }

}

