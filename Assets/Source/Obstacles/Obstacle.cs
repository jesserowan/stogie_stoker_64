// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public bool isRoadblock;
    public TrackOccupation collisionMask;

    public bool IsObstructiveTo(TrackOccupation query) => (collisionMask & query) > 0;

    public void RotateAxis(Track track)
    {
        // Debug.Log($"Obstacle.RotateAxis(): Track: {track}");
        // var child = transform.GetChild(0);
        transform.rotation = Quaternion.Euler(0, track == Track.Z ? 0 : 90, 0); 
        // child.rotation = Quaternion.Euler(-90, track == Track.Z ? 0 : 90, 0);
    }
}
