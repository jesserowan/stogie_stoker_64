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

    public Obstacle GetRandomObstacle()
    {
        _random ??= new Random();
        var randomIndex = _random.Next(obstaclePrefabs.Count);
        var prefab = obstaclePrefabs[randomIndex];
        var newObstacle = Instantiate(prefab, Vector3.zero, Quaternion.Euler(-90, 0, 0));
        return newObstacle;
    }
}
