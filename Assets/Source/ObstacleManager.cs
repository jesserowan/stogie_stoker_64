// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GlobeTrack
{
    North,
    South,
    East,
    West,
}

public enum Intersection
{
    Zenith,
    Nadir,
}


public class ObstacleManager : MonoBehaviour
{
    public IDictionary<GlobeTrack, float> GlobeTracks;
    public IDictionary<Intersection, GameObject> Intersections;
    public Intersection direction = Intersection.Nadir;
    public ObstacleFactory factory;
    
    // optional parameters
    [Range(0, 1)]
    public float obstacleDensity; // how many obstacles to spawn per track

    public float obstacleDifficulty; // how large the obstacles are, i.e. how small the evasion window is

    public float advanceNotice; // the distance from the player at which obstacles should spawn

    private void Awake()
    {
        factory = GetComponent<ObstacleFactory>();
        GlobeTracks = new Dictionary<GlobeTrack, float>
        {
            { GlobeTrack.North, 0f }, { GlobeTrack.South, 0f },
            { GlobeTrack.East, 0f }, { GlobeTrack.West, 0f },
        };

        // var zenith = FindFirstObjectByType<Zenith>();
        // var nadir = FindFirstObjectByType<Nadir>();
        // Intersections = new Dictionary<Intersection, GameObject>
        // {
        //     { Intersection.Zenith, zenith.gameObject },
        //     { Intersection.Nadir, nadir.gameObject },
        // };

    }

    private void Start()
    {
        Debug.Log("ObstacleManager.Start()");
        StartCoroutine(RandomizeObstacle());
    }

    public IEnumerator RandomizeObstacle()
    {
        var i = 0;
        while (i < 24)
        {
            i++;
            // factory.DestroyObstacles();
            factory.GenerateObstacle();
            yield return new WaitForSeconds(.5f);
        }
    }

    public void PopulateTrack(GlobeTrack track)
    {
        // TODO - find track and get coordinate range or hotspot range
        //      - Iterate through with config parameters and generate/enable obstacles
        //      - Update state
    }

    public void PopulateTracks(List<GlobeTrack> tracks)
    {
        foreach (var track in tracks)
        {
            PopulateTrack(track);
        }
    }

    public void DepopulateTrack(GlobeTrack track)
    {
        // TODO - find track and child obstacles
        //      - Iterate through and reset all fields if necessary for rereoll
        //      - Disable all obstacles and update state
    }
    
    public void DepopulateTracks(List<GlobeTrack> tracks)
    {
        foreach (var track in tracks)
        {
            DepopulateTrack(track);
        }
    }

    public void PopulateIntersection(Intersection intersection)
    {
        // TODO - Choose available next tracks and generate/enable intersection obstacles
        //      - Update state and assign next tracks to load
        //      - Load tracks?
    }

    public void DepopulateIntersection(Intersection intersection)
    {
        // TODO - Reset intersection config and state
        //      - Disable intersection
    }

}

