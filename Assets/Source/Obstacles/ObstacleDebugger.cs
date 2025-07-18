using System;
using Source;
using UnityEngine;
using UnityEngine.UI;

public class ObstacleDebugger : MonoBehaviour
{
    public ObstacleManager manager;
    public Button populateX1;
    public Button populateXMinus1;
    public Button populateZ1;
    public Button populateZMinus1;
    public Button populateZenith;
    public Button populateNadir;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        populateX1.onClick.AddListener(PopulateX1);
        populateXMinus1.onClick.AddListener(PopulateXMinus1);
        
        populateZ1.onClick.AddListener(PopulateZ1);
        populateZMinus1.onClick.AddListener(PopulateZMinus1);
        
        populateNadir.onClick.AddListener(PopulateNadir);
        populateZenith.onClick.AddListener(PopulateZenith);
    }

    private void OnDisable()
    {
        populateNadir.onClick.RemoveAllListeners();
        populateZenith.onClick.RemoveAllListeners();
        
        populateX1.onClick.RemoveAllListeners();
        populateXMinus1.onClick.RemoveAllListeners();
        
        populateZ1.onClick.RemoveAllListeners();
        populateZMinus1.onClick.RemoveAllListeners();
    }

    public void PopulateX1()
    {
        manager.PopulateTrack(Vector3.right);
    }

    public void PopulateXMinus1()
    {
        manager.PopulateTrack(Vector3.left); 
    }

    public void PopulateZ1()
    {
        manager.PopulateTrack(Vector3.forward);
    }

    public void PopulateZMinus1()
    {
        manager.PopulateTrack(Vector3.back);
    }

    public void PopulateZenith()
    {
        manager.ClearTrack(Vector3.up);
        var openTracks = manager.PopulatePole(PoleType.Zenith, Vector3.forward);
        Debug.Log($"PopulateZenith(): Open Tracks: {openTracks.Count}");
        foreach (var track in openTracks)
        {
            Debug.Log($"PopulateZenith(): Populating track: {track}");
            manager.ClearTrack(track);
            manager.PopulateTrack(track);
        }
    }

    public void PopulateNadir()
    {
        manager.ClearTrack(Vector3.down);
        var openTracks = manager.PopulatePole(PoleType.Nadir, Vector3.forward);
        foreach (var track in openTracks)
        {
            manager.ClearTrack(track);
            manager.PopulateTrack(track);
        }
    }

}
