using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    public Image background;
    public Biome selectedBiome;
    public Sprite cityBackground;
    public Sprite countryBackground;
    public Sprite suburbsBackground;

    private void Awake()
    {
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
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedBiome = selectedBiome switch
            {
                Biome.City => Biome.Country,
                Biome.Country => Biome.Suburbs,
                Biome.Suburbs => Biome.City,
                _ => Biome.City,
            };
            background.sprite = GetBackground();
        }
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedBiome = selectedBiome switch
            {
                Biome.City => Biome.Suburbs,
                Biome.Country => Biome.City,
                Biome.Suburbs => Biome.Country,
                _ => Biome.City,
            };
            background.sprite = GetBackground();
        }
        
        if (Input.GetKeyDown(KeyCode.Return)) OnPlay();
        if (Input.GetKeyDown(KeyCode.Escape)) OnBack();
    }

    private void OnPlay()
    {
        GameManager.CurrentBiome = selectedBiome;
        Debug.Log("Play!");
        SceneManager.LoadScene(2);
    }

    private void OnBack()
    {
        Debug.Log("Back!");
        SceneManager.LoadScene(0);
    }
}
