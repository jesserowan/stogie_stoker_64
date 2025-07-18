using System;
using UnityEngine;

namespace Source
{
public enum PoleType
{
    Zenith = 1,
    Nadir = -1
}

[RequireComponent(typeof(BoxCollider))]
public class Pole : MonoBehaviour
{
    public float height = 1;
    public PoleType which = PoleType.Zenith;
    private BoxCollider _box;

    private void Init() {
        var width = Constants.PoleWidth;
        if (!_box) _box = GetComponent<BoxCollider>(); _box.size = new Vector3(width, height, width);
        transform.position = new Vector3(0, (int)which * (Constants.WorldRadius + 0.5f * height), 0);
        _box.isTrigger = true;
    }

    private void OnValidate() { Init(); }
    private void OnEnable() { Init(); }
    private void Start() { Init(); }

    public (Track outTrack, Heading outDir, Lane outLane, float outTheta)
        GetParamsForTurn(Track inTrack, Heading inDir, Lane inLane, Turn inTurn)
    {
        var inTrackN = (int)inTrack;
        var inDirN = (int)inDir;
        var inLaneN = (int)inLane;
        var inTurnN = (int)inTurn;
        var whichN = (int)which;

        var outTrackN = -inTrackN;
        var outDirN = inDirN * inTurnN * -1;
        var outLaneN = inTurn > 0 ? inLaneN : -inLaneN;
        var outTheta = which is PoleType.Zenith
            ? SpherePosition.ANGLE_HALF_PI
            : SpherePosition.ANGLE_TWO_PI - SpherePosition.ANGLE_HALF_PI;

        var outTrack = (Track)outTrackN;
        var outDir = (Heading)outDirN;
        var outLane = (Lane)outLaneN;

        Debug.Log($"Turning on {whichN}: ({inTrack} ({inTrackN}), {inDir} ({inDirN}), {inLane} ({inLaneN}), {inTurn} ({inTurnN}))");
        Debug.Log($"                   > ({outTrack} ({outTrackN}), {outDir} ({outDirN}), {outLane} ({outLaneN}), {outTheta})");
        return (outTrack, outDir, outLane, outTheta);
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log($"Pole.OnTriggerEnter: {other.name} -- collision detected");
    //     if (other.CompareTag("Player"))
    //     {
    //         
    //     }
    // }
}
}
