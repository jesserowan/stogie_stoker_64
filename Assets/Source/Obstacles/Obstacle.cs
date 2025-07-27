// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using UnityEngine;
using UnityEngine.Serialization;

public class Obstacle : MonoBehaviour
{
    [FormerlySerializedAs("obstructiveTo")] public TrackOccupation collisionMask;

    public bool IsObstructiveTo(TrackOccupation query) => (collisionMask & query) > 0;

    public void RotateAxis(Track track)
    {
        // Debug.Log($"Obstacle.RotateAxis(): Track: {track}");
        var child = transform.GetChild(0);
        child.rotation = Quaternion.Euler(-90, track == Track.Z ? 0 : 90, 0);
    }
}
