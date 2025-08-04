using System;
using Source;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControlPanelUI : MonoBehaviour
{
    public ObstacleManager manager;
    public Button startOver;
    public TMP_Dropdown setDifficulty;
    public TMP_Dropdown setBiome;
    public Texture2D gameCursor;

    private void OnEnable()
    {
        // Debug.Log("Control Panel OnEnable");
        // Time.timeScale = 0;
        // Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        // startOver.onClick.AddListener(StartOver);
        // setDifficulty.onValueChanged.AddListener(SetDifficulty);
        // setBiome.onValueChanged.AddListener(SetBiome);

        // Debug.Log($"Setting biome and difficulty to: {GameManager.CurrentBiome}, {GameManager.CurrentDifficulty}");
        // setBiome.value = GetBiomeIndex(GameManager.CurrentBiome);
        // setDifficulty.value = GetDifficultyIndex();
    }

    private void OnDisable()
    {
        // Debug.Log("Control Panel OnDisable");
        // Time.timeScale = 1;
        // Cursor.SetCursor(gameCursor, Vector2.zero, CursorMode.Auto);
        // setDifficulty.onValueChanged.RemoveAllListeners();
        // setBiome.onValueChanged.RemoveAllListeners();
        // startOver.onClick.RemoveAllListeners();
    }

    public void SetDifficulty(int difficultyIndex)
    {
        // Debug.Log($"SetDifficulty(): Setting difficulty: {difficulty}");
        // GameManager.CurrentDifficulty = GetDifficulty(difficultyIndex);
    }

    public void SetBiome(int biome)
    {
        // Debug.Log($"SetBiome(): Setting biome: {biome}");
        // GameManager.CurrentBiome = GetBiome(biome);
    }

    // public Biome GetBiome(int i) => i switch { 2 => Biome.Country, 1 => Biome.Suburbs, _ => Biome.City };
    // public int GetBiomeIndex(Biome b) => b switch { Biome.Country => 2, Biome.Suburbs => 1, _ => 0 };

    // public DifficultyValue GetDifficulty(int i)
    // {
    //     // if (i == 2) return DifficultyCatalogue.instance.Hard;
    //     // if (i == 1) return DifficultyCatalogue.instance.Mid;
    //     // return DifficultyCatalogue.instance.Easy;
    // }

    // public int GetDifficultyIndex()
    // {
        // if (GameManager.CurrentDifficulty == DifficultyCatalogue.instance.Hard) return 2;
        // if (GameManager.CurrentDifficulty == DifficultyCatalogue.instance.Mid) return 1;
        // return 0;
    // }

    public void StartOver()
    {
        // Debug.Log("StartOver() called.");
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
