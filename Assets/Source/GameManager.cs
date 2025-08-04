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
    [SerializeField] public ControlPanelUI controlPanel;
    [SerializeField] public PlanetManager planetManager;
    [SerializeField] public WorldAudio worldAudio;
    [SerializeField] public RuntimeUI runtimeUI;

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
        worldAudio ??= FindFirstObjectByType<WorldAudio>();
        controlPanel ??= FindFirstObjectByType<ControlPanelUI>();
        planetManager ??= FindFirstObjectByType<PlanetManager>();
        obstacleManager ??= FindFirstObjectByType<ObstacleManager>();
        if (controlPanel) controlPanel.gameObject.SetActive(false);
        LoadMap();
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     if (Instance.CurrentGameState == GameState.GameOver) return;
        //     var wasPaused = controlPanel.gameObject.activeSelf;
        //     controlPanel.gameObject.SetActive(!wasPaused);
        //     CurrentGameState = wasPaused ? GameState.Playing : GameState.Paused;
        // }
        //
        // if (Input.GetKeyDown(KeyCode.Return))
        // {
        //     SceneManager.LoadScene(1);
        // }
    }


    // ====================== ## util ## ======================
    public void LoadMap()
    {
        Debug.Log($"GameManager.LoadMap()");
        worldAudio.PlayTheme();
        Debug.Log($"GameManager.LoadMap() planet manager zen: {planetManager.zenith}; nad: {planetManager.nadir}");
        CurrentPole = planetManager.zenith;
        CurrentPlanet = planetManager.SpawnPlanet();
        obstacleManager.PopulateTrack(Vector3.forward);
    }

    public static void EnterPole(Pole pole)
    {
        if (Instance == null) return;
        Debug.Log($"GameManager.EnterPole(): {pole}");
        Instance.CurrentPole = pole;
        Debug.Log($"GameManager.EnterPole(): new CurrentPole {Instance.CurrentPole}");
        OnPoleEntered?.Invoke(pole);
    }

    public static void ExitPole(Pole poleExited)
    {
        if (Instance == null) return;
        Debug.Log($"GameManager.ExitPole(): current pole {Instance.CurrentPole}");
        Instance.CurrentPole = null;
        if (Instance.CurrentGameState == GameState.Initializing)
            Instance.CurrentGameState = GameState.Playing;
        Debug.Log($"GameManager.ExitPole(): previous pole: {poleExited}");
        OnPoleExited?.Invoke(poleExited);
    }

    public static void CompleteCourse()
    {
        if (Instance == null) return;
        Instance.Win();
    }

    public void Win()
    {
        Instance.CurrentGameState = GameState.GameOver;
        Debug.Log($"GameManager.Win(): current time scale: {Time.timeScale}");
        StartCoroutine(FadeToVictory());
    }

    public float fadeDur = 5;
    private IEnumerator FadeToVictory()
    {
        float fadeDuration = fadeDur;
        Time.timeScale = 0.5f;
        while (fadeDuration > 0f)
        {
            var step = Mathf.Clamp(fadeDuration / fadeDur, 0, 1);
            Debug.Log($"GameManager.Fade(): {fadeDuration} / {fadeDur} = {step}");
            Time.timeScale = step / 2;
            Debug.Log($"GameManager.Fade(): new time scale: {Time.timeScale}");
            worldAudio.ModulateLowPass(step);
            fadeDuration -= 0.05f;
            yield return new WaitForSecondsRealtime(0.05f);
        }
        
        Time.timeScale = 1;
        SceneManager.LoadScene("JLWinScreen");
    }
}
