// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlanetCatalogue", menuName = "PlanetCatalogue")]
public class PlanetCatalogue : ScriptableObject
{
    public Planet Metropolis;
    public Planet Suburbia;
    public Planet Boonies;

    public Planet GetPlanet(Biome biome)
    {
        var prefab = biome switch {
            Biome.City => Metropolis,
            Biome.Country => Boonies,
            Biome.Suburbs => Suburbia,
            _ => throw new ArgumentOutOfRangeException(nameof(biome), biome, null)
        }; return Instantiate(prefab, Vector3.zero, Quaternion.identity);;
    }

}
