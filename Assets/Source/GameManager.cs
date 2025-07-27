using System;
using Source;
using UnityEngine;

public enum GameState
{
    Initializing,
    Playing,
    Paused,
    GameOver,
}

public enum Biome
{
    City,
    Suburbs,
    Country,
}

public enum Difficulty
{
    Easy,
    Mid,
    Hard,
}

public class GameManager : MonoBehaviour
{
    // ====================== ## singleton ## ======================
    public static GameManager Instance;
    
    
    // ====================== ## data ## ======================
    [SerializeField] public ObstacleManager obstacleManager;
    [SerializeField] public PlanetManager planetManager;
    [SerializeField] public ControlPanelUI controlPanel;
    [SerializeField] public RuntimeUI runtimeUI;
    [SerializeField] public WorldAudio worldAudio;
    
    
    // ====================== ## state ## ======================
    public GameState CurrentGameState { get; set; }
    public Planet CurrentPlanet { get; set; }
    public Pole CurrentPole { get; set; }
    public static Difficulty CurrentDifficulty { get; set; }
    public static Biome CurrentBiome { get; set; }
    
    
    // ====================== ## events ## ======================
    public static event Action<Pole> OnPoleEntered;
    public static event Action<Pole> OnPoleExited;
    

    // ====================== ## lifecycle ## ======================
    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else
        {
            Instance = this;
            Instance.CurrentGameState = GameState.Initializing;
        }
    }

    private void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
    
    private void Start()
    {
        runtimeUI ??= FindFirstObjectByType<RuntimeUI>();
        controlPanel ??= FindFirstObjectByType<ControlPanelUI>();
        planetManager ??= FindFirstObjectByType<PlanetManager>();
        obstacleManager ??= FindFirstObjectByType<ObstacleManager>();
        worldAudio ??= FindFirstObjectByType<WorldAudio>();
        if (controlPanel) controlPanel.gameObject.SetActive(false);
        LoadMap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Instance.CurrentGameState == GameState.GameOver) return;
            var wasPaused = controlPanel.gameObject.activeSelf;
            controlPanel.gameObject.SetActive(!wasPaused);
            CurrentGameState = wasPaused ? GameState.Playing : GameState.Paused;
        }
    }
    
    
    // ====================== ## util ## ======================
    public void LoadMap()
    {
        // Debug.Log($"GameManager.LoadMap()");
        worldAudio.PlayTheme();
        CurrentPlanet = planetManager.SpawnPlanet();
        obstacleManager.PopulateTrack(Vector3.forward);
    }

    public static void EnterPole(Pole pole)
    {
        if (Instance == null) return;
        // Debug.Log($"GameManager.EnterPole(): {pole}");
        Instance.CurrentPole = pole;
        OnPoleEntered?.Invoke(pole);
    }

    public static void ExitPole()
    {
        if (Instance == null) return;
        // Debug.Log($"GameManager.ExitPole(): current pole {Instance.CurrentPole}");
        var previousPole = Instance.CurrentPole;
        Instance.CurrentPole = null;
        if (Instance.CurrentGameState == GameState.Initializing) 
            Instance.CurrentGameState = GameState.Playing;
        OnPoleExited?.Invoke(previousPole);
    }

    public static void CompleteCourse()
    {
        // Debug.Log($"GameManager.CompleteCourse()");
        if (Instance == null) return;
        Instance.worldAudio.StopTheme();
        Instance.runtimeUI.OnWin();
        
        // Debug.Log($"GameManager.CompleteCourse(): we have the instance");
        // Instance.CurrentGameState = GameState.GameOver;
        // Debug.Log($"GameManager.CompleteCourse(): should be game over: {Instance.CurrentGameState}");
        // Instance.controlPanel.gameObject.SetActive(true);
        // Debug.Log($"GameManager.CompleteCourse(): UI should have opened");
    }
}
