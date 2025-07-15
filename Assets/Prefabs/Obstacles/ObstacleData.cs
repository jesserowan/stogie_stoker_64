using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[CreateAssetMenu(fileName = "ObstacleData", menuName = "ObstacleData")]
public class ObstacleData : ScriptableObject
{
    [SerializeField]
    public List<Obstacle> obstaclePrefabs = new ();
    
    [SerializeField]
    public Obstacle roadblockPrefab;

    private Random _random;

    private void Awake()
    {
        _random = new Random();
    }

    public Obstacle SpawnRoadblock() => Instantiate(roadblockPrefab);
    
    public Obstacle SpawnObstacle(int index)
    {
        var prefab = obstaclePrefabs[index];
        var newObstacle = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        newObstacle.name = $"Obstacle{index}_{prefab.name}";
        return newObstacle;
    }

    public Obstacle SpawnRandomObstacle()
    {
        _random ??= new Random();
        var randomIndex = _random.Next(obstaclePrefabs.Count);
        return SpawnObstacle(randomIndex);
    }
}
