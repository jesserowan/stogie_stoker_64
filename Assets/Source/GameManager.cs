using System;
using Source;
using UnityEngine;

public enum GameState
{
    Initializing,
    Playing,
    Paused,
}

public class GameManager : MonoBehaviour
{
    // ====================== ## singleton ## ======================
    public static GameManager Instance;
    
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
        
    }


    // ====================== ## events ## ======================
    public static event Action<Pole> OnPoleEntered;
    public static event Action<Pole> OnPoleExited;
    
    
    // ====================== ## state ## ======================
    public GameState CurrentGameState { get; set; }
    public Pole CurrentPole { get; set; }

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
