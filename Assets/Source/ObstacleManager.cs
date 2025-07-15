// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    // ====================== ## Data ## ======================
    public GameObject planet;
    public ObstacleData obstacleData;
    public SpherePosition spherePosition;
    
    
    // ====================== ## Config ## ======================
    [Range(0, 1)]
    public float obstacleDensity; // how many obstacles to spawn per track

    public float obstacleDifficulty; // how large the obstacles are, i.e. how small the evasion window is

    public float advanceNotice; // the distance from the player at which obstacles should spawn

    
    // ====================== ## State ## ======================
    public List<Obstacle> activeObstacles;
    public List<Obstacle> activeRoadblocks;
    
    
    // ====================== ## Lifecycle ## ======================
    private void Start()
    {
        Debug.Log("ObstacleManager.Start()");
        spherePosition ??= FindFirstObjectByType<SpherePosition>();
        StartCoroutine(PopulateTrack(Track.Z, Heading.Forward));
    }

    
    // ====================== ## API ## ======================
    public Obstacle Spawn(bool roadblock = false)
    {
        var obstacle = roadblock 
            ? obstacleData.SpawnRoadblock() 
            : obstacleData.SpawnRandomObstacle();
        if (roadblock) activeRoadblocks.Add(obstacle);
        else activeObstacles.Add(obstacle);
        obstacle.transform.SetParent(transform);
        return obstacle;
    }

    public void DeployObstacle(Obstacle obstacle, Vector3 axis, float angle)
    {
        obstacle.transform.Rotate(axis, angle);
        obstacle.transform.Translate(Vector3.up * Constants.WorldRadius, Space.Self);
        obstacle.transform.Rotate(Vector3.right, -90f, Space.Self);
    }
    
    // ====================== ## Internal ## ======================
    private IEnumerator PopulateTrack(Track track, Heading heading)
    {
        var angle = heading == Heading.Forward ? 0 : 180;
        var endAngle = angle + 180;
        var axis = track == Track.Z ? Vector3.right : Vector3.forward;
        while (angle < endAngle)
        {
            angle += (int)Constants.ObstacleSpacing;
            var o = Spawn();
            DeployObstacle(o, axis, angle);
            yield return new WaitForSeconds(Constants.ObstacleSpawnDelay);
        }
    }
}

