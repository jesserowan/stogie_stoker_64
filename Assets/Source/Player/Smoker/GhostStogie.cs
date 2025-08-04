using System;
using UnityEngine;

public class GhostStogie : MonoBehaviour
{
    public Renderer rend;
    public Renderer emb;

    public BooleanVariable IsSmoking;
    private bool Show => IsSmoking.Value || GameManager.Instance.CurrentGameState == GameState.GameOver;

    private void Update()
    {
        emb.enabled = Show;
        rend.enabled = Show;
    }
}
