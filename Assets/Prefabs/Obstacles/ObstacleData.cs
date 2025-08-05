using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "ObstacleData", menuName = "ObstacleData")]
public class ObstacleData : ScriptableObject
{
    [SerializeField]
    public List<Obstacle> cityObstacles = new (18);
    [SerializeField]
    public List<Obstacle> cityRoadblocks = new (2);

    [SerializeField]
    public List<Obstacle> countryObstacles = new (18);
    [SerializeField]
    public List<Obstacle> countryRoadblocks = new (2);

    [SerializeField]
    public List<Obstacle> suburbsObstacles = new (18);
    [SerializeField]
    public List<Obstacle> suburbsRoadblocks = new (2);


    public Obstacle GetObstacle(Biome biome, int index = -1)
    {
        Debug.Log($"GetObstacle({index})");
        if (index < 0) index = Random.Range(0, 17);
        return biome switch {
            Biome.City => cityObstacles[index],
            Biome.Country => countryObstacles[index],
            Biome.Suburbs => suburbsObstacles[index],
            _ => throw new ArgumentOutOfRangeException(nameof(biome), biome, null)
        };
    }

    public Obstacle GetRoadblock(Biome biome, int index = -1)
    {
        Debug.Log($"GetRoadblock({biome}, {index})");
        if (index < 0) index = Random.Range(0, 2);
        return biome switch {
            Biome.City => cityRoadblocks[index],
            Biome.Country => countryRoadblocks[index],
            Biome.Suburbs => suburbsRoadblocks[index],
            _ => throw new ArgumentOutOfRangeException(nameof(biome), biome, null)
        };
    }

    public Obstacle SpawnRoadblock()
    {
        Debug.Log($"SpawnRoadblock(): current biome: {GameManager.CurrentBiome}");
        var prefab = GetRoadblock(GameManager.CurrentBiome);
        var roadblock = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        Debug.Log($"SpawnRoadblock(): roadblock: {roadblock} {roadblock.gameObject.name}");
        return roadblock;
    }

    public Obstacle SpawnObstacle()
    {
        Debug.Log($"SpawnObstacle: current biome: {GameManager.CurrentBiome}");
        var prefab = GetObstacle(GameManager.CurrentBiome);
        Debug.Log($"SpawnObstacle: prefab: {prefab}");
        var newObstacle = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        return newObstacle;
    }
}
