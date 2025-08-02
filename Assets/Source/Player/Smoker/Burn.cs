using System;
using UnityEngine;

public class Burn : MonoBehaviour
{
    public Renderer rend;
    public Renderer ember;

    public Vector2 tile;

    public BooleanVariable isSmoking;

    public float DifficultyMultiplier => GameManager.CurrentDifficulty.burnMultiplier;

    public float ScaleStep => Time.deltaTime / 10 * DifficultyMultiplier;
    public float TextureScaleStep => Time.deltaTime / 8 * DifficultyMultiplier;

    public float threshold = 0.1f;

    void Awake()
    {
        rend ??= GetComponent<Renderer>();
        tile = rend.material.mainTextureScale;
        ember = transform.GetChild(0).gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameState.GameOver) return;
        if (isSmoking.Value) Shrink();
    }

    public void Shrink()
    {
        // Debug.Log("Shrink()");
        if (transform.localScale.y < threshold)
        {
            // Debug.Log($"DONE!!!!!!!!!!!!!!");
            GameManager.CompleteCourse();
            return;
        }
        
        transform.localScale -= new Vector3(0, ScaleStep, 0);
        transform.position += new Vector3(ScaleStep, 0, 0);
        tile = new Vector2(1, tile.y - Time.deltaTime / 4);
        rend.material.mainTextureScale -= new Vector2(0, TextureScaleStep);
    }
}
