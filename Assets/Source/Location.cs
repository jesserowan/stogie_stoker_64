using System;
using System.Collections.Generic;
using UnityEngine;


public enum Row
{
    Lower,
    Upper,
    Span,
}

public enum Lane { Center = 0, Right = 1, Left = -1 }

public enum Heading { Forward = 1, Backward = -1 }

public enum Turn { Left = -1, Right = 1 }

public enum Track { X = -1, Z = 1 }



[Flags]
public enum TrackOccupation {
    UpperLeft   = 0b000_001,
    UpperCenter = 0b000_010,
    UpperRight  = 0b000_100,
    LowerLeft   = 0b001_000,
    LowerCenter = 0b010_000,
    LowerRight  = 0b100_000,
}

[Serializable]
public struct Location
{
    public float theta;
    public float radius;
    public Track track;
    public Lane lane;
    public Row row;

    public const float ANGLE_TWO_PI = Mathf.PI * 2.0f;
    public const float ANGLE_HALF_PI = Mathf.PI * 0.5f;
    private static Dictionary<Lane, TrackOccupation> _laneTrackMask = new () {
        { Lane.Center, TrackOccupation.LowerCenter | TrackOccupation.UpperCenter },
        { Lane.Right,  TrackOccupation.LowerRight  | TrackOccupation.UpperRight },
        { Lane.Left,   TrackOccupation.LowerLeft   | TrackOccupation.UpperLeft }
    };
    private static Dictionary<Row, TrackOccupation> _rowTrackMask = new () {
        { Row.Upper, TrackOccupation.UpperLeft | TrackOccupation.UpperCenter | TrackOccupation.UpperRight },
        { Row.Lower, TrackOccupation.LowerLeft | TrackOccupation.LowerCenter | TrackOccupation.LowerRight },
        { Row.Span,  TrackOccupation.UpperLeft | TrackOccupation.UpperCenter | TrackOccupation.UpperRight |
                     TrackOccupation.LowerLeft | TrackOccupation.LowerCenter | TrackOccupation.LowerRight }
    };

    public Location(Track track = Track.Z, Lane lane = Lane.Center, Row row = Row.Span)
    {
        this.track = track;
        this.lane = lane;
        this.row = row;
        radius = Constants.WorldRadius;
        theta = 0f;
    }

    public Vector3 DerivePosition() => track switch {
        Track.Z => new Vector3(0f, radius * Mathf.Sin(theta), -radius * Mathf.Cos(theta)),
        Track.X => new Vector3(-radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), 0f),
        _ => Vector3.zero
    };

    public Quaternion DeriveRotation(Polarity polarity = Polarity.Neutral) {
        var upwards = DerivePosition().normalized;
        var forward = track switch { Track.Z => new Vector3(0f, -upwards.z, upwards.y),
                                     Track.X => new Vector3(upwards.y, -upwards.x, 0f),
                                     _ => Vector3.forward };
        if (polarity is not Polarity.Neutral &&
            forward.y.Sign() != (int)polarity)
            forward *= -1;
        return Quaternion.LookRotation(forward, upwards);
    }

    public Pose DeriveWorldPose(Polarity polarity = Polarity.Neutral)
    {
        var position = DerivePosition();
        var upwards = position.normalized;
        var forward = track switch {
            Track.Z => new Vector3(0f, -upwards.z, upwards.y),
            Track.X => new Vector3(upwards.y, -upwards.x, 0f),
            _ => Vector3.forward };
        if (polarity is not Polarity.Neutral &&
            forward.y.Sign() != (int)polarity)
            forward *= -1;
        var rotation = Quaternion.LookRotation(forward, upwards);
        return new Pose(position, rotation);
    }

    public TrackOccupation TrackOccupation => _rowTrackMask[row] & _laneTrackMask[lane];

    public Vector3 LaneOffset => lane switch {
        Lane.Right => new Vector3(1f * Constants.LaneWidth, 0, 0),
        Lane.Left => new Vector3(-1f * Constants.LaneWidth, 0, 0),
        _ => Vector3.zero
    };

    public Vector3 ApplySpeed(float speed, Track on)
    {
        if (speed is float.NaN or 0)
            return DerivePosition();
        var angularSpeed = speed / radius;
        theta += angularSpeed;
        if (theta < 0) theta = ANGLE_TWO_PI + theta;
        if (theta >= ANGLE_TWO_PI) theta = 0f;
        return DerivePosition();
    }

    public void Reset()
    {
        radius = Constants.Instance.worldRadius;
        theta = ANGLE_HALF_PI;
        track = Track.Z;
        lane = Lane.Center;
        row = Row.Span;
    }
}
