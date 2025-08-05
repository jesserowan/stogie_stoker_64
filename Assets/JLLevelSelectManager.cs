using System;
using System.Collections;
using System.Collections.Generic;
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

    private Coroutine debounceNavigationInput;

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
        if (debounceNavigationInput != null) return;
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log($"LEVEL SELECT: Left arrow pressed; moving from ${selectedBiome} to...");
            selectedBiome = selectedBiome switch
            {
                Biome.City => Biome.Suburbs,
                Biome.Suburbs => Biome.Country,
                Biome.Country => Biome.City,
                _ => Biome.Country,
            };
            Debug.Log($" ######### new biome: ${selectedBiome}!");
            persistentState.SelectedBiome = selectedBiome;
            background.sprite = GetBackground();
            debounceNavigationInput = StartCoroutine(DebounceNav());
        }
        
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log($"LEVEL SELECT: Right arrow pressed; moving from ${selectedBiome} to...");
            selectedBiome = selectedBiome switch
            {
                Biome.City => Biome.Country,
                Biome.Country => Biome.Suburbs,
                Biome.Suburbs => Biome.City,
                _ => Biome.Country,
            };
            Debug.Log($" ######### new biome: ${selectedBiome}!");
            background.sprite = GetBackground();
            debounceNavigationInput = StartCoroutine(DebounceNav());
        }
        
        else if (Input.GetKeyDown(KeyCode.Return)) OnPlay();
        else if (Input.GetKeyDown(KeyCode.Escape)) OnBack();
    }

    private void OnPlay()
    {
        Debug.Log($" OnPlay pressed: selectedbiome: ${selectedBiome}; persistent state biome: ${persistentState.SelectedBiome}; game manager biome: ${GameManager.CurrentBiome}!");
        persistentState.SelectedBiome = selectedBiome;
        GameManager.CurrentBiome = selectedBiome;
        Debug.Log($" OnPlay pressed 2: selectedbiome: ${selectedBiome}; persistent state biome: ${persistentState.SelectedBiome}; game manager biome: ${GameManager.CurrentBiome}!");
        SceneManager.LoadScene("Sandbox.beta");
    }

    private void OnBack()
    {
        //Debug.Log("Back!");
        SceneManager.LoadScene(0);
    }

    private IEnumerator DebounceNav()
    {
        Debug.Log($"DEBOUNCE START");
        yield return new WaitForSeconds(1f);
        debounceNavigationInput = null;
        Debug.Log($"DEBOUNCE END");
    }
}
