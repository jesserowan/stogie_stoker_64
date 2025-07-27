using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    // ====================== ## singleton ## ======================
    public static GameManager Instance;

    public static GameState InitialState;
    public static GameState PauseState;
    public static GameState PlayState;
    public static GameState WinState;
    public static GameState LoseState;


    // ====================== ## data ## ======================
    [SerializeField] public ObstacleManager obstacleManager;
    [SerializeField] public ControlPanelUI controlPanel;
    [SerializeField] public PlanetManager planetManager;

    [SerializeField] public Pole northPole;
    [SerializeField] public Pole southPole;


    // ====================== ## state ## ======================
    public static Biome CurrentBiome { get; set; }
    public static Difficulty CurrentDifficulty { get; set; }

    private GameState _currentGameState;

    public static GameState CurrentGameState
    {
        get => Instance._currentGameState;
        set {
            if (Instance.controlPanel.gameObject)
                Instance.controlPanel.gameObject.SetActive(value != PlayState);
            Instance._currentGameState = value;
        }
    }

    public Planet CurrentPlanet { get; set; }
    public Pole CurrentPole { get; set; }


    // ====================== ## events ## ======================
    public static event Action<Pole> OnPoleEntered;
    public static event Action<Pole> OnPoleExited;


    // ====================== ## lifecycle ## ======================
    private void Awake()
    {
        if (Instance == null)
         Destroy(gameObject);
        else Instance = this;
    }

    private void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }

    private void Start()
    {
        controlPanel ??= FindFirstObjectByType<ControlPanelUI>();
        planetManager ??= FindFirstObjectByType<PlanetManager>();
        obstacleManager ??= FindFirstObjectByType<ObstacleManager>();
        LoadMap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) CurrentGameState
            = CurrentGameState == LoseState ? InitialState
            : CurrentGameState == WinState ? InitialState
            : CurrentGameState == PlayState ? PauseState
            : CurrentGameState == PauseState ? PlayState
            : CurrentGameState;

    }


    // ====================== ## util ## ======================
    public void LoadMap()
    {
        // Debug.Log($"GameManager.LoadMap()");
        CurrentPlanet = planetManager.SpawnPlanet();
        obstacleManager.PopulateTrack(Vector3.forward);
    }

    public static void EnterPole(Pole pole)
    {
        if (Instance == null) return;
        Instance.CurrentPole = pole;
        OnPoleEntered?.Invoke(pole);
    }

    public static void ExitPole()
    {
        if (Instance == null) return;
        var previousPole = Instance.CurrentPole;
        Instance.CurrentPole = null;
        OnPoleExited?.Invoke(previousPole);
    }

    public static void CompleteCourse()
    {
        if (Instance == null) return;
        Instance._currentGameState = WinState;
    }

    public static void GameOver()
    {
        if (Instance == null) return;
        Instance._currentGameState = LoseState;
    }
}

