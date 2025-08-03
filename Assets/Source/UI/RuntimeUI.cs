using System;
using UnityEngine;
using UnityEngine.UI;

public class RuntimeUI : MonoBehaviour
{
    public Image image;

    public Sprite cityWinSprite;
    public Sprite countryWinSprite;
    public Sprite suburbsWinSprite;

    private void Start()
    {
        image.enabled = false;
    }

    public void OnWin()
    {
        image.sprite = GetBiomeSprite();
        image.enabled = true;
    }

    private Sprite GetBiomeSprite()
    {
        return GameManager.CurrentBiome switch
        {
            Biome.City => cityWinSprite,
            Biome.Country => countryWinSprite,
            Biome.Suburbs => suburbsWinSprite,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
