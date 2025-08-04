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
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedBiome = selectedBiome switch
            {
                Biome.City => Biome.Suburbs,
                Biome.Suburbs => Biome.Country,
                Biome.Country => Biome.City,
                _ => Biome.Country,
            };
            background.sprite = GetBackground();
        }
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedBiome = selectedBiome switch
            {
                Biome.City => Biome.Country,
                Biome.Country => Biome.Suburbs,
                Biome.Suburbs => Biome.City,
                _ => Biome.Country,
            };
            background.sprite = GetBackground();
        }
        
        if (Input.GetKeyDown(KeyCode.Return)) OnPlay();
        if (Input.GetKeyDown(KeyCode.Escape)) OnBack();
    }

    private void OnPlay()
    {
        GameManager.CurrentBiome = selectedBiome;
        SceneManager.LoadScene(sceneIndex);
    }

    private void OnBack()
    {
        //Debug.Log("Back!");
        SceneManager.LoadScene(0);
    }
}
