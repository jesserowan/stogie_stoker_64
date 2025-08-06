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


    // ====================== ## State ## ======================
    public Vector3 CurrentTrack { get; set; }

    public Dictionary<Vector3, GameObject> Parents = new ();

    private Dictionary<Vector3, GameObject> GenerateParents() => new () {
        { Vector3.forward, GenerateParent("Forward") },
        { Vector3.back, GenerateParent("Backward") },
        { Vector3.left, GenerateParent("Left") },
        { Vector3.right, GenerateParent("Right") },
        { Vector3.up, GenerateParent("Zenith") },
        { Vector3.down, GenerateParent("Nadir") }
    };

    private GameObject GenerateParent(string parentName)
    {
        var newParent = new GameObject($"Mama{parentName}");
        newParent.transform.SetParent(transform);
        return newParent;
    }


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

        Parents ??= GenerateParents();

        GameManager.OnPoleExited += HandlePoleExited;
        GameManager.OnPoleEntered += HandlePoleEntered;

        player ??= FindFirstObjectByType<Player>();
        playerLocation = player.location;
        // Debug.Log($"ObstacleManager.Start(): Done initializing:" +
                  // $"\n    > player: {player}\n    > spherePosition: {spherePosition}" +
                  // $"\n    > Poles: {Poles}");
    }

    private void OnDisable()
    {
        GameManager.OnPoleEntered -= HandlePoleEntered;
        GameManager.OnPoleExited -= HandlePoleExited;
    }


    // ====================== ## API ## ======================

    public Obstacle SpawnRoadblock(Track track)
    {
        var roadblock = obstacleData.SpawnRoadblock();
        roadblock.RotateAxis(track); // if x, rotate 90 degrees
        return roadblock;
    }


    public void SlaughterChildren(GameObject parent)
    {
        Debug.Log($"ObstacleManager.SlaughterChildren(): Emptying parent {parent.name}");
        foreach (Transform child in parent.transform) Destroy(child.gameObject);
    }

    public Dictionary<Polarity, Dictionary<Vector3, GameObject>> Intersections = new ()
    {
        {
            Polarity.North,
            new Dictionary<Vector3, GameObject> {
                { Vector3.forward, null },
                { Vector3.back, null },
                { Vector3.left, null },
                { Vector3.right, null }
            }
        },
        {
            Polarity.South,
            new Dictionary<Vector3, GameObject> {
                { Vector3.forward, null },
                { Vector3.back, null },
                { Vector3.left, null },
                { Vector3.right, null }
            }
        }
    };

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

    private void ClearIntersection(Polarity pole)
    {
        // Debug.Log($"ClearIntersection(): {pole}");
        foreach (var poleTrack in Intersections[pole].Keys.ToList()) {
            Destroy(Intersections[pole][poleTrack]);
            Intersections[pole][poleTrack] = null;
        }
    }

    public void DeployRoadblock(Obstacle roadblock, Polarity polarity, Vector3 track)
    {
        if (polarity == Polarity.South) roadblock.transform.Rotate(Vector3.left, 180);

        roadblock.transform.Translate(Vector3.up * (Constants.WorldRadius - 0.1f), Space.Self);
        roadblock.transform.position += track * 2;
        var parentKey = polarity == Polarity.North ? Vector3.up : Vector3.down;
        roadblock.transform.SetParent(Parents[parentKey].transform);
    }


    // ====================== ## event handlers ## ======================
    public void HandlePoleEntered(Pole pole, Track track, Heading heading)
    {

    }

    public Polarity GetNextPole(Pole p) => p.which == Polarity.South ? Polarity.North : Polarity.South;
    public void HandlePoleExited(Pole pole, Track track, Heading heading)
    {
        Debug.Log($"ObstacleManager.HandlePoleExited(): {pole}");

        var currentTrack = (int)pole.which * (int)heading * (track is Track.X ? Vector3.right : Vector3.forward);

        // Debug.Log($"HandlePoleExited(): Determined exit track: {currentTrack}");
        var currentPole = pole.which == Polarity.North ? Vector3.up : Vector3.down;
        foreach (var parentKey in Parents.Keys.ToList())
        {
            if (parentKey == currentTrack || parentKey == currentPole) continue;
            SlaughterChildren(Parents[parentKey]);
        }

        var nextPole = GetNextPole(pole);
        var openTracks = PopulatePole(GetNextPole(pole), currentTrack);
        // Debug.Log($"HandlePoleExited(): open tracks: {openTracks.Count}");
        foreach (var openTrack in openTracks)
        {
            PopulateTrack(openTrack, (int)nextPole * GetTrackValue(openTrack));
        }
    }


    // ====================== ## Internal ## ======================
    private bool IsPositive(Vector3 track) => track == Vector3.right || track == Vector3.forward;
    private bool IsX(Vector3 trackVector) => trackVector == Vector3.right || trackVector == Vector3.left;
    private Track GetTrack(Vector3 trackVector) => IsX(trackVector) ? Track.X : Track.Z;

    private int GetTrackValue(Vector3 trackVector) => IsX(trackVector)
        ? trackVector.x > 0 ? 1 : -1
        : trackVector.z > 0 ? 1 : -1;

    public void PopulateTrack(Vector3 track, int direction)
    {
        // polarity is what pole the obstacles are being generated from
        Debug.Log($"ObstacleManager.PopulateTrack(): track: {track}");
        SlaughterChildren(Parents[track]);
        StartCoroutine(SpawnAlongTrack(track, direction));
    }

    private IEnumerator SpawnAlongTrack(Vector3 track, int direction)
    {
        var arrangement = GameManager.CurrentDifficulty.obstacleBlueprint;
        var index = 0;
        var isX = IsX(track);
        var axis = isX ? Vector3.forward : Vector3.right;
        while (index < arrangement.Count)
        {
            // the angle around the globe
            var angle = arrangement[index] * (IsPositive(track) ? 1f : -1f);

            index += 1;
            var o = Spawn(isX ? Track.X : Track.Z, direction < 0);
            DeployObstacle(o, axis, angle);
            o.transform.SetParent(Parents[track].transform);
            yield return new WaitForSeconds(Constants.ObstacleSpawnDelay);
        }
    }

    private Obstacle Spawn(Track track, bool turnaround)
    {
        Debug.Log($"Spawn(): Track: {track}");
        var obstacle = obstacleData.SpawnObstacle();
        if (turnaround)
            obstacle.transform.rotation *= Quaternion.AngleAxis(180, Vector3.up);
        obstacle.RotateAxis(track);
        return obstacle;
    }

    public void DeployObstacle(Obstacle obstacle, Vector3 axis, float angle)
    {
        Debug.Log($"DeployObstacle(): {obstacle}; angle: {angle}; axis: {axis}");
        Debug.Log($"######## obstacle position: {obstacle.transform.position}; rotation: {obstacle.transform.rotation.eulerAngles}");
        obstacle.transform.Rotate(axis, angle, Space.World);
        obstacle.transform.Translate(Vector3.up * (Constants.WorldRadius - 0.075f), Space.Self);
    }

}
