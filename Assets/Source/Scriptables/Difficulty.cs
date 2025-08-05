using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Difficulty", menuName = "Enums/Difficulty")]
public class DifficultyValue : ScriptableObject
{
    public float fieldOfView;
    public float speedMultiplier;
    public float burnMultiplier;

    [Header("Obstacles")]
    public bool at15deg;
    public bool at30deg;
    public bool at45deg;
    public bool at60deg;
    public bool at75deg;
    public bool at90deg;
    public bool at105deg;
    public bool at120deg;
    public bool at135deg;
    public bool at150deg;
    public bool at165deg;

    [HideInInspector] [DoNotSerialize]
    public List<int> obstacleBlueprint;

    private void OnEnable()
    {
        if (obstacleBlueprint.Count > 0)
            obstacleBlueprint.Clear();
        if (at15deg) obstacleBlueprint.Add(15);
        if (at30deg) obstacleBlueprint.Add(30);
        if (at45deg) obstacleBlueprint.Add(45);
        if (at60deg) obstacleBlueprint.Add(60);
        if (at75deg) obstacleBlueprint.Add(75);
        if (at90deg) obstacleBlueprint.Add(90);
        if (at105deg) obstacleBlueprint.Add(105);
        if (at120deg) obstacleBlueprint.Add(120);
        if (at135deg) obstacleBlueprint.Add(135);
        if (at150deg) obstacleBlueprint.Add(150);
        if (at165deg) obstacleBlueprint.Add(165);
    }
}
