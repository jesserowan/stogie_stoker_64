using System;
using Source;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GameState
{
    Initializing,
    Playing,
    Paused,
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
    
    [SerializeField] public ObstacleManager obstacleManager;
    [SerializeField] public ControlPanelUI controlPanel;
    
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
    
    
    // ====================== ## lifecycle ## ======================
    private void Start()
    {
        controlPanel.gameObject.SetActive(false);
        LoadMap();
    }

    public void LoadMap()
    {
        LoadNewBiome();
        obstacleManager.PopulateTrack(Vector3.forward);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            controlPanel.gameObject.SetActive(!controlPanel.gameObject.activeSelf);
        }
    }


    // ====================== ## events ## ======================
    public static event Action<Pole> OnPoleEntered;
    public static event Action<Pole> OnPoleExited;

    public static event Action OnStart;
    
    
    // ====================== ## state ## ======================
    public GameState CurrentGameState { get; set; }
    public Pole CurrentPole { get; set; }
    public static Difficulty CurrentDifficulty { get; set; }
    public static Biome CurrentBiome { get; set; }
    
    
    // ====================== ## state ## ======================
    public static void LoadNewBiome()
    {
        Debug.Log($"GameManager.LoadNewBiome(): current biome: {CurrentBiome}; difficulty: {CurrentDifficulty}");
        // var randomBiome = (Biome)Random.Range(0, 3);
        // CurrentBiome = randomBiome;
        // CurrentDifficulty = Difficulty.Easy;
    }
    

    public static void EnterPole(Pole pole)
    {
        if (Instance == null) return;
        Debug.Log($"GameManager.EnterPole(): {pole}");
        Instance.CurrentPole = pole;
        OnPoleEntered?.Invoke(pole);
    }

    public static void ExitPole()
    {
        if (Instance == null) return;
        Debug.Log($"GameManager.ExitPole(): current pole {Instance.CurrentPole}");
        var previousPole = Instance.CurrentPole;
        Instance.CurrentPole = null;
        if (Instance.CurrentGameState == GameState.Initializing) 
            Instance.CurrentGameState = GameState.Playing;
        OnPoleExited?.Invoke(previousPole);
    }
}
