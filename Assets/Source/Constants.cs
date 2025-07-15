using UnityEngine;


[CreateAssetMenu(menuName = "Config/Constants")]
public class Constants : ScriptableObject
{
    // ====================== ## singleton ## ======================
    private static Constants _instance;
    public static Constants Instance {
        get {
            if (!_instance) _instance = Resources.Load<Constants>("Constants");
            if (!_instance) Debug.LogError("Could not find Constants instance in `Assets/Resources/Constants.asset`). Does it need to be created?");
            return _instance;
        }
    }

    // ====================== ## static properties ## ======================
    public static float WorldRadius => Instance.worldRadius;
    public static float LaneWidth => Instance.laneWidth;
    public static float PoleWidth => Instance.poleWidth;

    public static float FieldOfView {
        get => Instance.fieldOfView;
        set => Instance.fieldOfView = Mathf.Clamp(value, 20f, 170f);
    }

    // Obstacle config
    public static float ObstacleSpawnDelay => Instance.obstacleSpawnDelay;
    public static float ObstacleSpacing => Instance.obstacleSpacing;

    
    // ====================== ## instance properties ## ======================
    public float worldRadius = 10f;
    public float laneWidth = 1.2f;
    public float poleWidth = 4f;

    [Range(20f, 170f)]
    public float fieldOfView = 20f;

    [Tooltip("How long to wait before spawning the next obstacle.")]
    public float obstacleSpawnDelay = 0f;

    [Tooltip("How many degrees apart to spawn each obstacle.")]
    public float obstacleSpacing = 15f;
}
