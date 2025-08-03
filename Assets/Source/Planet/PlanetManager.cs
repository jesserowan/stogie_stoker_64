using Source;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public PlanetData planetData;

    public Planet activePlanet;

    public Pole zenith;
    public Pole nadir;

    public Planet SpawnPlanet()
    {
        Debug.Log($"PlanetManager.SpawnPlanet(): Spawning planet for biome: {GameManager.CurrentBiome}");
        if (activePlanet) Destroy(activePlanet.gameObject);
        activePlanet = null;
        var biome = GameManager.CurrentBiome;
        activePlanet = planetData.GetPlanet(biome);
        activePlanet.transform.SetParent(gameObject.transform);
        return activePlanet;
    }
}
