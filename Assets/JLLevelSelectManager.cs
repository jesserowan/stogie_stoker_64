using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JLLevelSelectManager : MonoBehaviour
{
    public int sceneIndex = 3;
    public Image background;
    public Biome selectedBiome;
    public Biome defaultBiome = Biome.Country;
    public Sprite cityBackground;
    public Sprite countryBackground;
    public Sprite suburbsBackground;
    public PersistentState persistentState;

    private void Awake()
    {
        selectedBiome = defaultBiome;
        background.sprite = GetBackground();
    }

    private Sprite GetBackground()
    {
        return selectedBiome switch
        {
            Biome.City => cityBackground,
            Biome.Country => countryBackground,
            Biome.Suburbs => suburbsBackground,
            _ => cityBackground,
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            // Debug.Log($"LEVEL SELECT: Left arrow pressed; moving from ${selectedBiome} to...");
            selectedBiome = selectedBiome switch
            {
                Biome.City => Biome.Suburbs,
                Biome.Suburbs => Biome.Country,
                Biome.Country => Biome.City,
                _ => Biome.Country,
            };
            // Debug.Log($" ######### new biome: ${selectedBiome}!");
            persistentState.SelectedBiome = selectedBiome;
            background.sprite = GetBackground();
        }
        
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            // Debug.Log($"LEVEL SELECT: Right arrow pressed; moving from ${selectedBiome} to...");
            selectedBiome = selectedBiome switch
            {
                Biome.City => Biome.Country,
                Biome.Country => Biome.Suburbs,
                Biome.Suburbs => Biome.City,
                _ => Biome.Country,
            };
            // Debug.Log($" ######### new biome: ${selectedBiome}!");
            background.sprite = GetBackground();
        }
        
        if (Input.GetKeyDown(KeyCode.Return)) OnPlay();
        if (Input.GetKeyDown(KeyCode.Escape)) OnBack();
    }

    private void OnPlay()
    {
        // Debug.Log($" OnPlay pressed: selectedbiome: ${selectedBiome}; persistent state biome: ${persistentState.SelectedBiome}; game manager biome: ${GameManager.CurrentBiome}!");
        persistentState.SelectedBiome = selectedBiome;
        GameManager.CurrentBiome = selectedBiome;
        // Debug.Log($" OnPlay pressed 2: selectedbiome: ${selectedBiome}; persistent state biome: ${persistentState.SelectedBiome}; game manager biome: ${GameManager.CurrentBiome}!");
        SceneManager.LoadScene(sceneIndex);
    }

    private void OnBack()
    {
        //Debug.Log("Back!");
        SceneManager.LoadScene(0);
    }
}
