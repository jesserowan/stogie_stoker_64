// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleFactory : MonoBehaviour
{
    public ObstacleData ObstacleData;
    
    public List<Obstacle> Obstacles;

    private void Awake()
    {
        Obstacles = new List<Obstacle>();
    }

    public Obstacle GenerateObstacle()
    {
        Obstacle obstacle = ObstacleData.GetRandomObstacle();
        Obstacles.Add(obstacle);
        return obstacle;
    }

    public void DestroyObstacles()
    {
        foreach (var o in Obstacles) Destroy(o.gameObject);
        Obstacles.Clear();
    }
}
