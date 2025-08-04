using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

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
        worldAudio ??= FindFirstObjectByType<WorldAudio>();
        playerAudio ??= FindFirstObjectByType<PlayerAudio>();
        planetManager ??= FindFirstObjectByType<PlanetManager>();
        obstacleManager ??= FindFirstObjectByType<ObstacleManager>();
        LoadMap();
    }


    // ====================== ## util ## ======================
    public void LoadMap()
    {
        // Debug.Log($"GameManager.LoadMap()");
        worldAudio.PlayTheme();
        // Debug.Log($"GameManager.LoadMap() planet manager zen: {planetManager.zenith}; nad: {planetManager.nadir}");
        CurrentPole = planetManager.zenith;
        CurrentPlanet = planetManager.SpawnPlanet();
        obstacleManager.PopulateTrack(Vector3.forward);
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
