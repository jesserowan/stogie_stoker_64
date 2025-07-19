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
        if (index < 0) index = Random.Range(0, 17);
        return biome switch
        {
            Biome.City => cityObstacles[index],
            Biome.Country => countryObstacles[index],
            Biome.Suburbs => suburbsObstacles[index],
            _ => throw new ArgumentOutOfRangeException(nameof(biome), biome, null)
        };
    }

    public Obstacle GetRoadblock(Biome biome, int index = -1)
    {
        if (index < 0) index = Random.Range(0, 1);
        return biome switch
        {
            Biome.City => cityRoadblocks[index],
            Biome.Country => countryRoadblocks[index],
            Biome.Suburbs => suburbsRoadblocks[index],
            _ => throw new ArgumentOutOfRangeException(nameof(biome), biome, null)
        };
    }

    public Obstacle SpawnRoadblock()
    {
        var prefab = GetRoadblock(GameManager.CurrentBiome);
        var roadblock = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        return roadblock;
    }
    
    public Obstacle SpawnObstacle()
    {
        var prefab = GetObstacle(GameManager.CurrentBiome);
        var newObstacle = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        return newObstacle;
    }
}
