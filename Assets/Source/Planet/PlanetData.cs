// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using System;
using UnityEngine;

namespace Source
{
    [CreateAssetMenu(fileName = "PlanetData", menuName = "PlanetData")]
    public class PlanetData : ScriptableObject
    {
        public Planet Metropolis;
        public Planet Suburbia;
        public Planet Boonies;

        public Planet GetPlanet(Biome biome)
        {
            var prefab = biome switch
            {
                Biome.City => Metropolis,
                Biome.Country => Boonies,
                Biome.Suburbs => Suburbia,
                _ => throw new ArgumentOutOfRangeException(nameof(biome), biome, null)
            };
            var planet = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            return planet;
        }
        
        
    }
}