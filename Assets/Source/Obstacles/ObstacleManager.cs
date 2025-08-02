// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class ObstacleManager : MonoBehaviour
{
    // ====================== ## Controls ## ======================
    public bool showDebugPanel;
    private ObstacleDebugger _debugPanel;


    // ====================== ## Data ## ======================
    public Player player;
    public GameObject planet;
    public Location playerLocation;
    public ObstacleData obstacleData;
    public (Pole north, Pole south) Poles;


    // ====================== ## State ## ======================
    public Vector3 CurrentTrack { get; set; }

    public Dictionary<Vector3, GameObject> Parents = new ();


    // ====================== ## Lifecycle ## ======================
    private void Awake()
    {
        Parents = GenerateParents();
    }

    private void Start()
    {
        // Debug.Log("ObstacleManager.Start()");
        _debugPanel ??= FindFirstObjectByType<ObstacleDebugger>(FindObjectsInactive.Include);
        if (_debugPanel) _debugPanel.gameObject.SetActive(showDebugPanel);

        var poles = FindObjectsByType<Pole>(FindObjectsSortMode.None);
        if (poles.Length != 2)
        {
            Debug.Log($"We found {poles.Length} poles");
            throw new Exception("There must be 2 poles");
        }
        foreach (var pole in poles)
        { if (pole.which == Polarity.North) Poles.north = pole;
            else Poles.south = pole; }

        Parents ??= GenerateParents();

        GameManager.OnPoleExited += HandlePoleExited;
        GameManager.OnPoleEntered += HandlePoleEntered;

        player ??= FindFirstObjectByType<Player>();
        playerLocation = player.location;
        // Debug.Log($"ObstacleManager.Start(): Done initializing:" +
                  // $"\n    > player: {player}\n    > spherePosition: {spherePosition}" +
                  // $"\n    > Poles: {Poles}");
    }

    private Dictionary<Vector3, GameObject> GenerateParents() => new ()
        { { Vector3.forward, GenerateParent("Forward") }, { Vector3.back, GenerateParent("Backward") },
            { Vector3.left, GenerateParent("Left") }, { Vector3.right, GenerateParent("Right") },
            { Vector3.up, GenerateParent("Zenith") }, { Vector3.down, GenerateParent("Nadir") } };

    private GameObject GenerateParent(string parentName)
    {
        var newParent = new GameObject($"Mama{parentName}");
        newParent.transform.SetParent(transform);
        return newParent;
    }

    private void OnDisable()
    {
        GameManager.OnPoleEntered -= HandlePoleEntered;
        GameManager.OnPoleExited -= HandlePoleExited;
    }


    // ====================== ## API ## ======================
    public Obstacle Spawn(Track track)
    {
        // Debug.Log($"Spawn(): Track: {track}");
        var obstacle = obstacleData.SpawnObstacle();
        obstacle.RotateAxis(track);
        return obstacle;
    }

    public Obstacle SpawnRoadblock(Track track)
    {
        var roadblock = obstacleData.SpawnRoadblock();
        roadblock.RotateAxis(track); return roadblock;
    }

    public void PopulateTrack(Vector3 track)
    {
        Debug.Log($"ObstacleManager.PopulateTrack(): track: {track}; " +
                  $"difficulty: {GameManager.CurrentDifficulty}; biome: {GameManager.CurrentBiome}");
        SlaughterChildren(Parents[track]);
        StartCoroutine(SpawnAlongTrack(track));
    }

    public void SlaughterChildren(GameObject parent)
    {
        Debug.Log($"ObstacleManager.SlaughterChildren(): Emptying parent {parent.name}");
        foreach (Transform child in parent.transform) Destroy(child.gameObject);
    }

    public Dictionary<Polarity, Dictionary<Vector3, GameObject>> Intersections = new ()
    { { Polarity.North,
            new Dictionary<Vector3, GameObject>
            { { Vector3.forward, null }, { Vector3.back, null }, { Vector3.left, null }, { Vector3.right, null } } },
        { Polarity.South,
            new Dictionary<Vector3, GameObject>
            { { Vector3.forward, null }, { Vector3.back, null }, { Vector3.left, null }, { Vector3.right, null } } } };

    public List<Vector3> PopulatePole(Polarity polarity, Vector3 arrivalTrack)
    {
        // Debug.Log($"ObstacleManager.PopulatePole(): {poleType}; arrival track: {arrivalTrack}");
        int closedTurns = 0;
        var openTracks = new List<Vector3>();
        ClearIntersection(polarity);
        foreach (var turn in Intersections[polarity].Keys.ToList())
        {
            if (turn == arrivalTrack) continue;
            if (closedTurns >= 2) openTracks.Add(turn);
            else
            {
                if (Random.value < 0.5)
                {
                    var rb = SpawnRoadblock(GetTrack(turn));
                    DeployRoadblock(rb, polarity, turn);
                    Intersections[polarity][turn] = rb.gameObject;
                    closedTurns++;
                }
                else openTracks.Add(turn);
            }
        }

        return openTracks;
    }

    // TODO -- likely redundant,
    private void ClearIntersection(Polarity pole)
    {
        // Debug.Log($"ClearIntersection(): {pole}");
        foreach (var poleTrack in Intersections[pole].Keys.ToList())
        { Destroy(Intersections[pole][poleTrack]);
            Intersections[pole][poleTrack] = null; }
    }

    public void DeployObstacle(Obstacle obstacle, Vector3 axis, float angle)
    {
        // Debug.Log($"DeployObstacle(): {obstacle}; angle: {angle}; axis: {axis}");
        obstacle.transform.Rotate(axis, angle);
        obstacle.transform.Translate(Vector3.up * Constants.WorldRadius, Space.Self);
    }

    public void DeployRoadblock(Obstacle roadblock, Polarity polarity, Vector3 track)
    {
        if (polarity == Polarity.South) roadblock.transform.Rotate(Vector3.left, 180);

        roadblock.transform.Translate(Vector3.up * Constants.WorldRadius, Space.Self);
        roadblock.transform.position += track * 2;
        var parentKey = polarity == Polarity.North ? Vector3.up : Vector3.down;
        roadblock.transform.SetParent(Parents[parentKey].transform);
    }


    // ====================== ## event handlers ## ======================
    public void HandlePoleEntered(Pole pole) { }

    public Polarity GetNextPole(Pole p) => p.which == Polarity.South ? Polarity.North : Polarity.South;
    public void HandlePoleExited(Pole pole)
    {
        Debug.Log($"ObstacleManager.HandlePoleExited(): {pole}");

        Vector3 currentTrack;
        var playerPos = player.transform.position;
        if (playerPos.z > 1.2) currentTrack = Vector3.forward;
        else if (playerPos.z < -1.2) currentTrack = Vector3.back;
        else if (playerPos.x > 1.2) currentTrack = Vector3.right;
        else if (playerPos.x < -1.2) currentTrack = Vector3.left;
        else throw new Exception("Unable to determine player exit track");

        Debug.Log($"HandlePoleExited(): Determined exit track: {currentTrack}");
        var currentPole = pole.which == Polarity.North ? Vector3.up : Vector3.down;
        foreach (var parentKey in Parents.Keys.ToList())
        {
            if (parentKey == currentTrack || parentKey == currentPole) continue;
            SlaughterChildren(Parents[parentKey]);
        }

        var openTracks = PopulatePole(GetNextPole(pole), currentTrack);
        // Debug.Log($"HandlePoleExited(): open tracks: {openTracks.Count}");
        foreach (var track in openTracks)
        {
            // Debug.Log($"HandlePoleExited(): populating open track: {track}");
            PopulateTrack(track);
        }
    }


    // ====================== ## Internal ## ======================
    private bool IsPositive(Vector3 track) => track == Vector3.left || track == Vector3.forward;
    private bool IsX(Vector3 track) => track == Vector3.right || track == Vector3.left;
    private Track GetTrack(Vector3 trackVector) => IsX(trackVector) ? Track.X : Track.Z;

    private IEnumerator SpawnAlongTrack(Vector3 track)
    {
        var arrangement = GameManager.CurrentDifficulty.obstacleBlueprint;
        var index = 0;
        var isX = IsX(track);
        // Debug.Log($"SpawnAlongTrack(): Track: {track}; IsX: {isX}; isPositive: {IsPositive(track)}");
        var baseAngle = IsPositive(track) ? 0 : 180;
        // Debug.Log($"SpawnAlongTrack(): BaseAngle: {baseAngle}");
        var axis = isX ? Vector3.forward : Vector3.right;
        // Debug.Log($"SpawnAlongTrack(): Axis: {axis}");
        while (index < arrangement.Count)
        {
            var angle = baseAngle + arrangement[index];
            index += 1;
            var o = Spawn(isX ? Track.X : Track.Z);
            DeployObstacle(o, axis, angle);
            o.transform.SetParent(Parents[track].transform);
            yield return new WaitForSeconds(Constants.ObstacleSpawnDelay);
        }
    }
}

// var parentName = key switch { { x: 1 } => "Right", { x: -1 } => "Left",
//                               { y: 1 } => "Zenith", { y: -1 } => "Nadir",
//                               { z: 1 } => "Forward", { z: -1 } => "Backward",
//                               _ => throw new ArgumentOutOfRangeException(nameof(key), key, null) };
