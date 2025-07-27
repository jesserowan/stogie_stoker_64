using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControlPanelUI : MonoBehaviour
{
    public TMP_Dropdown setDifficulty;
    public TMP_Dropdown setBiome;
    public Texture2D gameCursor;
    public Button startOver;

    private void OnEnable()
    {
        // Debug.Log("Control Panel OnEnable");
        Time.timeScale = 0;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        startOver.onClick.AddListener(StartOver);
        setDifficulty.onValueChanged.AddListener(SetDifficulty);
        setBiome.onValueChanged.AddListener(SetBiome);

        // Debug.Log($"Setting biome and difficulty to: {GameManager.CurrentBiome}, {GameManager.CurrentDifficulty}");
        setBiome.value = GetBiomeIndex(GameManager.CurrentBiome);
        setDifficulty.value = GetDifficultyIndex(GameManager.CurrentDifficulty);
    }

    private void OnDisable()
    {
        // Debug.Log("Control Panel OnDisable");
        Time.timeScale = 1;
        Cursor.SetCursor(gameCursor, Vector2.zero, CursorMode.Auto);
        setDifficulty.onValueChanged.RemoveAllListeners();
        setBiome.onValueChanged.RemoveAllListeners();
        startOver.onClick.RemoveAllListeners();
    }

    public void SetDifficulty(int difficulty)
    {
        // Debug.Log($"SetDifficulty(): Setting difficulty: {difficulty}");
        GameManager.CurrentDifficulty = GetDifficulty(difficulty);
    }

    public void SetBiome(int biome)
    {
        // Debug.Log($"SetBiome(): Setting biome: {biome}");
        GameManager.CurrentBiome = GetBiome(biome);
    }

    public Biome GetBiome(int i) => i switch { 2 => Biome.Country, 1 => Biome.Suburbs, _ => Biome.City };
    public int GetBiomeIndex(Biome b) => b switch { Biome.Country => 2, Biome.Suburbs => 1, _ => 0 };

    public Difficulty GetDifficulty(int i) => i switch { 2 => Difficulty.Hard, 1 => Difficulty.Mid, _ => Difficulty.Easy };
    public int GetDifficultyIndex(Difficulty d) => d switch { Difficulty.Hard => 2, Difficulty.Mid => 1, _ => 0 };

    public void StartOver()
    {
        // Debug.Log("StartOver() called.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
