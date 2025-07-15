// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleFactory : MonoBehaviour
{
    public Transform planet;
    public ObstacleData ObstacleData;
    
    public List<Obstacle> Obstacles;
    public float start = 0;

    private void Awake()
    {
        Obstacles = new List<Obstacle>();
    }

    public Obstacle GenerateObstacle()
    {
        Obstacle obstacle = ObstacleData.GetRandomObstacle();
        obstacle.transform.RotateAround(planet.transform.position, transform.right, start);
        start += 15;
        // transform.RotateAround(orbitObject.transform.position, transform.right, speed * Time.deltaTime);
        Obstacles.Add(obstacle);
        return obstacle;
    }

    public void DestroyObstacles()
    {
        foreach (var o in Obstacles) Destroy(o.gameObject);
        Obstacles.Clear();
    }
}
