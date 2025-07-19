// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Source;
using UnityEngine;
using Random = UnityEngine.Random;

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

    public Vector3 currentTrack;

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

        GameManager.OnPoleEntered += HandlePoleEntered;
        GameManager.OnPoleExited += HandlePoleExited;
        
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

    private void OnDisable()
    {
        GameManager.OnPoleEntered -= HandlePoleEntered;
        GameManager.OnPoleExited -= HandlePoleExited;
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
        ClearTrack(track);
        StartCoroutine(SpawnAlongTrack(track));
    }

    public void ClearTrack(Vector3 track)
    {
        if (track == Vector3.up) SlaughterChildren(Zenith);
        else if (track == Vector3.down) SlaughterChildren(Nadir);
        else if (track == Vector3.forward) SlaughterChildren(Z1);
        else if (track == Vector3.back) SlaughterChildren(ZMinus1);
        else if (track == Vector3.right) SlaughterChildren(X1);
        else if (track == Vector3.left) SlaughterChildren(XMinus1);
    }

    public void SlaughterChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public Dictionary<Vector3, GameObject> northPoleIntersection = new Dictionary<Vector3, GameObject>
    {
        { Vector3.forward, null },
        { Vector3.back, null },
        { Vector3.left, null },
        { Vector3.right, null },
    };

    public Dictionary<Vector3, GameObject> southPoleIntersection = new Dictionary<Vector3, GameObject>
    {
        { Vector3.forward, null },
        { Vector3.back, null },
        { Vector3.left, null },
        { Vector3.right, null },
    };

    public List<Vector3> PopulatePole(PoleType poleType, Vector3 arrivalTrack)
    {
        Debug.Log($"ObstacleManager.PopulatePole(): {poleType}");
        var intersection = poleType == PoleType.Zenith 
            ? northPoleIntersection
            : southPoleIntersection;
        int closedTurns = 0;
        var openTracks = new List<Vector3>();
        foreach (var turn in intersection.Keys.ToList())
        {
            if (turn == arrivalTrack)
            {
                if (intersection[turn] != null) Destroy(intersection[turn]);
                intersection[turn] = null;
            }
            else if (closedTurns < 2)
            {
                if (Random.value < 0.5)
                {
                    var rb = SpawnRoadblock(GetTrack(turn));
                    DeployRoadblock(rb, poleType, turn);
                    intersection[turn] = rb.gameObject;
                    closedTurns++;
                    ClearTrack(turn);
                }
                else openTracks.Add(turn);
            }
        }

        Debug.Log($"ObstacleManager.PopulatePole(): Generated {closedTurns} roadblocks at pole: {poleType}");
        return openTracks;
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
        roadblock.transform.SetParent(poleType == PoleType.Zenith ? Zenith.transform : Nadir.transform);
    }


    // ====================== ## event handlers ## ======================
    public void HandlePoleEntered(Pole pole)
    {
        if (pole.which == PoleType.Zenith) ClearTrack(Vector3.down);
        else if (pole.which == PoleType.Nadir) ClearTrack(Vector3.up);
    }

    public PoleType GetNextPole(Pole p) => p.which == PoleType.Nadir ? PoleType.Zenith : PoleType.Nadir;
    public PoleType GetNextPole(PoleType p) => p == PoleType.Nadir ? PoleType.Zenith : PoleType.Nadir;
    public void HandlePoleExited(Pole pole)
    {
        // TODO populate next pole...
        // TODO populate obstacles...
        Debug.Log($"ObstacleManager.HandlePoleExited(): {pole}");
        var openTracks = PopulatePole(GetNextPole(pole), Vector3.forward);
        foreach (var track in openTracks)
        {
            ClearTrack(track);
            PopulateTrack(track);
        }
    }
    
    
    // ====================== ## Internal ## ======================
    private bool IsPositive(Vector3 track) => track == Vector3.right || track == Vector3.forward; 
    private bool IsX(Vector3 track) => track == Vector3.right || track == Vector3.left;
    private Track GetTrack(Vector3 trackVector) => IsX(trackVector) ? Track.X : Track.Z;
    
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

