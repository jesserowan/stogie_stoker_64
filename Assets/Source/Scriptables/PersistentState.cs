// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using UnityEngine;


[CreateAssetMenu(fileName = "PersistentState", menuName = "Scriptables/Persistent State")]
public class PersistentState : ScriptableObject
{
    [SerializeField] private DifficultyValue selectedDifficulty;
    [SerializeField] private Biome selectedBiome;
    
    public Biome SelectedBiome { get => selectedBiome; set => selectedBiome = value; }
    public DifficultyValue SelectedDifficulty { get => selectedDifficulty; set => selectedDifficulty = value; }
    
    
}
