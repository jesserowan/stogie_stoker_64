using UnityEngine;

public enum Polarity { North = 1, Neutral = 0, South = -1 }

[RequireComponent(typeof(BoxCollider))]
public class Pole : MonoBehaviour
{
    public float height = 1;
    public Polarity polarity = Polarity.North;
    private BoxCollider _box;

    private void OnValidate() { Init(); }
    private void OnEnable() { Init(); }
    private void Start() { Init(); }

    private void Init()
    {
        var width = Constants.PoleWidth;
        if (!_box) _box = GetComponent<BoxCollider>(); _box.size = new Vector3(width, height, width);
        transform.position = new Vector3(0, (int) polarity * (Constants.WorldRadius + 0.5f * height), 0);
        _box.isTrigger = true;
    }

    public (Track outTrack, Heading outDir, Lane outLane, float outTheta)
        InterpretTurn(Track inTrack, Heading inDir, Lane inLane, Turn inTurn)
    {
        var inTrackN = (int)inTrack;
        var inLaneN = (int)inLane;
        var inTurnN = (int)inTurn;
        var whichN = (int)polarity;
        var inDirN = (int)inDir;

        var outTrackN = -inTrackN;
        var outDirN = inDirN * inTurnN * -1;
        var outLaneN = inTurn > 0 ? inLaneN : -inLaneN;
        var outTheta = polarity is Polarity.North
            ? Location.ANGLE_HALF_PI
            : Location.ANGLE_TWO_PI - Location.ANGLE_HALF_PI;

        var outTrack = (Track)outTrackN;
        var outLane = (Lane)outLaneN;
        var outDir = (Heading)outDirN;

        Debug.Log($"Turning on {polarity} ({whichN}):");
        Debug.Log($" i: ({inTrack} ({inTrackN}), {inDir} ({inDirN}), {inLane} ({inLaneN}), {inTurn} ({inTurnN}))");
        Debug.Log($" o: ({outTrack} ({outTrackN}), {outDir} ({outDirN}), {outLane} ({outLaneN}), {outTheta})");
        return (outTrack, outDir, outLane, outTheta);
    }
}

