using System;
using UnityEngine;
using UnityEngine.Serialization;


public enum Row { Lower, Upper }

public enum Lane { Middle = 0, Right = 1, Left = -1 }

public enum Track { X = -1, Z = 1 }

public enum Heading { Forward = -1, Backward = 1 }

public enum Turn { Left = -1, Right = 1 }


[CreateAssetMenu(menuName = "Player/SpherePosition")]
public class SpherePosition : ScriptableObject
{
    public const float ANGLE_HALF_PI = Mathf.PI * 0.5f;
    public const float ANGLE_TWO_PI = Mathf.PI * 2.0f;
    public float radius;
    public float theta;


    public Track track = Track.Z;
    public Lane lane = Lane.Middle;
    public Row row = Row.Lower;

    private Vector3 _vectorPosition;
    public Vector3 VectorVectorPosition {
        get {
            switch (track) {
                case Track.Z:
                    _vectorPosition.z = radius * Mathf.Cos(theta);
                    _vectorPosition.y = radius * Mathf.Sin(theta);
                    _vectorPosition.x = 0f; return _vectorPosition;
                case Track.X:
                    _vectorPosition.x = radius * Mathf.Cos(theta);
                    _vectorPosition.y = radius * Mathf.Sin(theta);
                    _vectorPosition.z = 0f; return _vectorPosition;
                default: return _vectorPosition;
            }
        }
    }

    public Vector3 LaneOffset => lane switch
    {
        Lane.Right => new Vector3(1f * Constants.LaneWidth, 0, 0),
        Lane.Left => new Vector3(-1f * Constants.LaneWidth, 0, 0),
        _ => Vector3.zero
    };

    public Vector3 ApplySpeed(float speed, Track on)
    {
        if (speed is float.NaN or 0)
        {
            Debug.Log($"Invalid speed: {speed}");
            return VectorVectorPosition;
        }
        var angularSpeed = speed / radius;
        Debug.Log($"Applying Speed: {speed} (angular {angularSpeed}) along track {on}");

        // var prevTheta = theta;
        var prevPhi = theta;
        // switch (track)
        // {
        //     case Track.Z:
        //         phi += angularSpeed;
        //         Debug.Log($"  adding angular speed {angularSpeed} to phi ({prevPhi}) => ({phi})");
        //         break;
        //     case Track.Y:
        //         theta += angularSpeed;
        //         Debug.Log($"  adding angular speed {angularSpeed} to theta ({prevTheta}) => ({theta})");
        //         break;
        //     case Track.X:
        //         phi += angularSpeed;
        //         theta += angularSpeed;
        //         Debug.Log($"  adding angular speed {angularSpeed} to theta & phi ({prevTheta}, {prevPhi}) => ({theta}, {phi})");
        //         break;
        // }

        theta += angularSpeed;
        Debug.Log($"  adding angular speed {angularSpeed} to phi ({prevPhi}) => ({theta})");

        if (theta < 0) theta = ANGLE_TWO_PI + theta;
        if (theta < 0) theta = ANGLE_TWO_PI + theta;
        if (theta >= ANGLE_TWO_PI) theta = 0f;
        if (theta >= ANGLE_TWO_PI) theta = 0f;
        return VectorVectorPosition;
    }

    public void Reset()
    {
        Debug.Log($"Resetting SpherePosition {Constants.Instance.worldRadius}");
        radius = Constants.Instance.worldRadius;
        theta = ANGLE_HALF_PI;
        theta = ANGLE_HALF_PI;
    }
}
