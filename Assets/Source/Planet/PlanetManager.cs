using Source;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public PlanetData planetData;

    public Planet activePlanet;
    
    public Planet SpawnPlanet()
    {
        // Debug.Log($"PlanetManager.SpawnPlanet(): Spawning planet for biome: {GameManager.CurrentBiome} current planet: {activePlanet}");
        if (activePlanet) Destroy(activePlanet.gameObject);
        activePlanet = null;
        var biome = GameManager.CurrentBiome;
        activePlanet = planetData.GetPlanet(biome);
        // Debug.Log($" planet manager ######## got new planet: {activePlanet}; biome: {biome}");
        activePlanet.transform.SetParent(gameObject.transform);
        return activePlanet;
    }
}
