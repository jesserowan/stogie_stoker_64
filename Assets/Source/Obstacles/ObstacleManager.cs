// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using Source;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    // ====================== ## Controls ## ======================
    public bool generateOnStart;
    
    // ====================== ## Data ## ======================
    public Player player;
    public GameObject planet;
    public ObstacleData obstacleData;
    public SpherePosition spherePosition;
    public (Pole north, Pole south) Poles;
    public Heading heading = Heading.Forward;
    
    // ====================== ## Config ## ======================
    [Range(0, 1)]
    public float obstacleDensity; // how many obstacles to spawn per track

    public float obstacleDifficulty; // how large the obstacles are, i.e. how small the evasion window is

    public float advanceNotice; // the distance from the player at which obstacles should spawn

    
    // ====================== ## State ## ======================
    public List<Obstacle> activeObstacles;
    public List<Obstacle> activeRoadblocks;
    public Dictionary<Vector3, List<Obstacle>> ObstacleMap;
    public Dictionary<Vector3, List<Obstacle>> RoadblockMap;

    public GameObject Z1;
    public GameObject ZMinus1;
    public GameObject X1;
    public GameObject XMinus1;
    public GameObject Zenith;
    public GameObject Nadir;
    
    
    // ====================== ## Lifecycle ## ======================
    private void Start()
    {
        Debug.Log("ObstacleManager.Start()");
        
        
        var poles = FindObjectsByType<Pole>(FindObjectsSortMode.None);
        if (poles.Length != 2) throw new Exception("There must be 2 poles");
        foreach (var pole in poles)
        { if (pole.which == PoleType.Zenith) Poles.north = pole;
            else Poles.south = pole; }

        if (!generateOnStart) return;
        
        player ??= FindFirstObjectByType<Player>();
        spherePosition = player.spherePosition;
        Debug.Log($"ObstacleManager.Start(): Done initializing:" +
                  $"\n    > player: {player}\n    > spherePosition: {spherePosition}" +
                  $"\n    > Poles: {Poles}");
        // StartCoroutine(SpawnAlongCurrentTrack(Track.Z));
    }


    // ====================== ## API ## ======================
    public Obstacle Spawn(Track track)
    {
        var obstacle = obstacleData.SpawnRandomObstacle();
        obstacle.RotateAxis(track);
        activeObstacles.Add(obstacle);
        return obstacle;
    }

    public Obstacle SpawnRoadblock(Track track)
    {
        var roadblock = obstacleData.SpawnRoadblock();
        roadblock.RotateAxis(track);
        activeRoadblocks.Add(roadblock);
        return roadblock;
    }

    public void PopulateTrack(Vector3 track)
    {
        Debug.Log($"ObstacleManager.PopulateTrack(): {track}");
        StartCoroutine(SpawnAlongTrack(track));
    }

    public void PopulatePole(PoleType poleType)
    {
        Debug.Log($"ObstacleManager.PopulatePole(): {poleType}");
        var roadblock = SpawnRoadblock(Track.Z);
        var roadblock2 = SpawnRoadblock(Track.Z);
        var roadblock3 = SpawnRoadblock(Track.X);
        var roadblock4 = SpawnRoadblock(Track.X);
        DeployRoadblock(roadblock, poleType, Vector3.forward);
        DeployRoadblock(roadblock2, poleType, Vector3.back);
        DeployRoadblock(roadblock3, poleType, Vector3.left);
        DeployRoadblock(roadblock4, poleType, Vector3.right);
    }

    public void DeployObstacle(Obstacle obstacle, Vector3 axis, float angle)
    {
        obstacle.transform.Rotate(axis, angle);
        obstacle.transform.Translate(Vector3.up * Constants.WorldRadius, Space.Self);
    }

    public void DeployRoadblock(Obstacle roadblock, PoleType poleType, Vector3 track)
    {
        if (poleType == PoleType.Nadir) roadblock.transform.Rotate(Vector3.left, 180);
        
        roadblock.transform.Translate(Vector3.up * Constants.WorldRadius, Space.Self);
        roadblock.transform.position += track * 2;
        
        if (poleType == PoleType.Nadir) {roadblock.transform.SetParent(Nadir.transform);}
        else roadblock.transform.SetParent(Zenith.transform);
    }


    // ====================== ## Internal ## ======================
    private bool IsPositive(Vector3 track) => track == Vector3.right || track == Vector3.forward; 
    private bool IsX(Vector3 track) => track == Vector3.right || track == Vector3.left; 
    private IEnumerator SpawnAlongTrack(Vector3 track)
    {
        var isX = IsX(track);
        var angle = IsPositive(track) ? 5 : 185;
        var endAngle = angle + 170;
        var axis = isX ? Vector3.forward : Vector3.right;
        while (angle < endAngle)
        {
            var o = Spawn(isX ? Track.X : Track.Z);
            DeployObstacle(o, axis, angle);
            Reparent(o, track);
            angle += (int)Constants.ObstacleSpacing;
            yield return new WaitForSeconds(Constants.ObstacleSpawnDelay);
        }
    }
    
    private void Reparent(Obstacle obstacle, Vector3 track)
    {
        if (track == Vector3.forward) obstacle.transform.SetParent(Z1.transform);
        else if (track == Vector3.back) obstacle.transform.SetParent(ZMinus1.transform);
        else if (track == Vector3.right) obstacle.transform.SetParent(X1.transform);
        else if (track == Vector3.left) obstacle.transform.SetParent(XMinus1.transform);
        else if (track == Vector3.up) obstacle.transform.SetParent(Zenith.transform);
        else if (track == Vector3.down) obstacle.transform.SetParent(Nadir.transform);
    }
}

