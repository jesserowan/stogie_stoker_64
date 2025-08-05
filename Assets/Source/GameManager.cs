using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] public WorldAudio worldAudio;
    [SerializeField] public PlayerAudio playerAudio;
    [SerializeField] public Player player;


    // ====================== ## state ## ======================
    [SerializeField] private IntegerVariable remainingLives;
    [SerializeField] private DifficultyValue currentDifficulty;
    // [SerializeField] private PersistentState persistentState;

    [SerializeField] private DifficultyValue easy;
    [SerializeField] private DifficultyValue med;
    [SerializeField] private DifficultyValue hard;

    public static DifficultyValue CurrentDifficulty {
        get => Instance.currentDifficulty;
        set => Instance.currentDifficulty = value;
    }

    public static Biome CurrentBiome { get; set; }
    public GameState CurrentGameState { get; set; }
    public Planet CurrentPlanet { get; set; }
    public Pole CurrentPole { get; set; }


    // ====================== ## events ## ======================
    public static event Action<Pole> OnPoleEntered;
    public static event Action<Pole> OnPoleExited;
    public static event Action OnImpact;
    public static event Action OnWin;
    public static event Action OnLose;
    
    public static event Action OnJump;
    
    public static event Action OnSlide;
    public static event Action<Lane> OnLaneSwitch;


    // ====================== ## lifecycle ## ======================
    private void Awake()
    {
        // Debug.Log($"<color=green><b>GameManager::Awake :: Biome: {CurrentBiome}; currentDifficulty: {currentDifficulty}</b></color>");
        if (Instance != null) Destroy(gameObject);
        else
        {
            Instance = this;
            Instance.CurrentGameState = GameState.Initializing;
            // Debug.Log($"<color=green><b>GameManager::Awake2 :: Biome: {CurrentBiome}; currentDifficulty: {currentDifficulty}</b></color>");
        }
    }

    private void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        // Debug.Log($"<color=cyan><b>GameManager::OnEnable :: Biome: {CurrentBiome}; Difficulty: {currentDifficulty}</b></color>");
    }

    private void Start()
    {
        worldAudio ??= FindFirstObjectByType<WorldAudio>();
        playerAudio ??= FindFirstObjectByType<PlayerAudio>();
        planetManager ??= FindFirstObjectByType<PlanetManager>();
        obstacleManager ??= FindFirstObjectByType<ObstacleManager>();
        // Debug.Log($"GameManager::Start::planetManager: {planetManager}; obstacleManager: {obstacleManager}");
        // Debug.Log($"<color=orange><b>GameManager::Start :: Biome: {CurrentBiome}; Difficulty: {currentDifficulty}</b></color>");
        LoadMap();
    }


    // ====================== ## util ## ======================
    public void LoadMap()
    {
        // Debug.Log($"<color=orange><b>GameManager::LoadMap1 :: PersistentState biome: {persistentState.SelectedBiome}; current biome: {CurrentBiome}</b></color>");
        // Debug.Log($"<color=orange><b>GameManager::LoadMap1 :: Difficulty: {currentDifficulty}</b></color>");
        CurrentDifficulty = CurrentBiome switch
        {
            Biome.City => hard,
            Biome.Suburbs => med,
            _ => easy
        };
        CurrentPlanet = planetManager.SpawnPlanet();
        CurrentPole = planetManager.zenith;
        obstacleManager.PopulateTrack(Vector3.forward);
        worldAudio.PlayTheme();
        // Debug.Log($"<color=orange><b>GameManager::LoadMap2:: PersistentState biome: {persistentState.SelectedBiome}; current biome: {CurrentBiome}</b></color>");
        // Debug.Log($"<color=orange><b>GameManager::LoadMap2 :: Difficulty: {currentDifficulty}</b></color>");
    }

    public static void EnterPole(Pole pole)
    {
        if (Instance == null) return;
        // Debug.Log($"GameManager.EnterPole(): {pole}");
        Instance.CurrentPole = pole;
        // Debug.Log($"GameManager.EnterPole(): new CurrentPole {Instance.CurrentPole}");
        OnPoleEntered?.Invoke(pole);
    }

    public static void ExitPole(Pole poleExited)
    {
        if (Instance == null) return;
        // Debug.Log($"GameManager.ExitPole(): current pole {Instance.CurrentPole}");
        Instance.CurrentPole = null;
        if (Instance.CurrentGameState == GameState.Initializing)
            Instance.CurrentGameState = GameState.Playing;
        // Debug.Log($"GameManager.ExitPole(): previous pole: {poleExited}");
        OnPoleExited?.Invoke(poleExited);
    }

    public static void BroadcastImpact()
    {
        // if (Instance == null) return;
        Debug.Log($"<color=yellow><b>IMPACT DETECTED</b></color>");
        OnImpact?.Invoke();
    }

    public static void BroadcastLaneSwitch(Lane lane)
    {
        Debug.Log($"<color=blue><b>LANE SWITCH DETECTED</b></color>");
        OnLaneSwitch?.Invoke(lane);
    }
    
    public static void BroadcastJump() => OnJump?.Invoke();
    public static void BroadcastSlide() => OnSlide?.Invoke();
    
    public static void CompleteCourse(bool win = true)
    {
        if (Instance == null) return;
        if (win) Instance.Win();
        else Instance.Lose();
    }

    public void Win()
    {
        Instance.CurrentGameState = GameState.GameOver;
        OnWin?.Invoke();
        StartCoroutine(FadeToVictory());
    }

    public void Lose()
    {
        Instance.CurrentGameState = GameState.GameOver;
        OnLose?.Invoke();
        StartCoroutine(FadeToVictory(false));
    }

    public float fadeDur = 5;
    private IEnumerator FadeToVictory(bool win = true)
    {
        float fadeDuration = fadeDur;
        Time.timeScale = 0.5f;
        while (fadeDuration > 0f)
        {
            var step = Mathf.Clamp(fadeDuration / fadeDur, 0, 1);
            Time.timeScale = step / 2;
            if (win) worldAudio.ModulateLowPass(step);
            else worldAudio.ModulateHighPass(step);
            fadeDuration -= 0.05f;
            yield return new WaitForSecondsRealtime(0.05f);
        }
        
        Time.timeScale = 1;
        if (win) SceneManager.LoadScene("JLWinScreen");
        else SceneManager.LoadScene("JLLoseScreen");
    }
}
