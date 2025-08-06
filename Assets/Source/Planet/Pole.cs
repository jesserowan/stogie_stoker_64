using UnityEngine;

public enum Polarity
{
    North = 1,
    Neutral = 0,
    South = -1
}

[RequireComponent(typeof(BoxCollider))]
public class Pole : MonoBehaviour
{
    public float height = 1;
    public Polarity which = Polarity.North;
    private BoxCollider _box;

    private void Init() {
        var width = Constants.PoleWidth;
        if (!_box) _box = GetComponent<BoxCollider>();
        _box.size = new Vector3(width, height, width);
        transform.position = new Vector3(0, (int)which * (Constants.WorldRadius + 0.5f * height), 0);
        _box.isTrigger = true;
    }

    private void Start() { Init(); }
    private void OnEnable() { Init(); }
    private void OnValidate() { Init(); }
    public (Track outTrack, Heading outDir, Lane outLane, float outTheta)
        GetParamsForTurn(Track inTrack, Heading inDir, Lane inLane, Turn inTurn) {
        var whichN = (int)which;
        var inLaneN = (int)inLane;
        var inTrackN = (int)inTrack;
        var inTurnN = (int)inTurn;
        var inDirN = (int)inDir;

        var outTrackN = -inTrackN;
        var outDirN = whichN * inTrackN * inDirN * inTurnN;
        var outLaneN = inTurn > 0 ? inLaneN : -inLaneN;
        var outTheta = which is Polarity.North
            ? Location.ANGLE_HALF_PI
            : Location.ANGLE_TWO_PI - Location.ANGLE_HALF_PI;

        var outTrack = (Track)outTrackN;
        var outDir = (Heading)outDirN;
        var outLane = (Lane)outLaneN;

        // Debug.Log($"Turning {inTurn} on {which} ({inTurnN} on {whichN})");
        // Debug.Log($" i: track: {inTrack} ({inTrackN}), dir: {inDir} ({inDirN}), lane: {inLane} ({inLaneN}), turn{inTurn} ({inTurnN})");
        // Debug.Log($" o: track: {outTrack} ({outTrackN}), dir: {outDir} ({outDirN}), lane: {outLane} ({outLaneN}), {outTheta})");
        return (outTrack, outDir, outLane, outTheta);
    }
}
