using UnityEngine;
using UnityEngine.Serialization;

public class PlanetManager : MonoBehaviour
{
    public static Planet planet;
    [FormerlySerializedAs("planetData")] public PlanetCatalogue catalogue;

    public Planet SpawnPlanet()
    {
        Debug.Log($"PlanetManager.SpawnPlanet(): Spawning planet for biome: {GameManager.CurrentBiome}");
        if (planet != null) Destroy(planet.gameObject);
        planet = null;
        var biome = GameManager.CurrentBiome;
        planet = catalogue.GetPlanet(biome);
        planet.transform.SetParent(gameObject.transform);
        return planet;
    }
}
