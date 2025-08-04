using System;
using UnityEngine;

public class Burn : MonoBehaviour
{
    public Renderer rend;
    public Renderer ember;

    public Vector2 tile;

    public BooleanVariable isSmoking;

    public float DifficultyMultiplier => GameManager.CurrentDifficulty.burnMultiplier;

    [SerializeField] public float baseScaler;
    public float ScaleStep => Time.deltaTime / baseScaler * DifficultyMultiplier;

    public float threshold = 0.08f;

    void Awake()
    {
        rend ??= GetComponent<Renderer>();
        tile = rend.material.mainTextureScale;
        ember = transform.GetChild(0).gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameState.GameOver)
        {
            return;
        }
        if (isSmoking.Value) Shrink();
    }

    public void Shrink()
    {
        // Debug.Log("Shrink()");
        Debug.Log($"Shrink(): local scale: {transform.localScale.y}");
        if (transform.localScale.y < threshold)
        {
            // Debug.Log($"DONE!!!!!!!!!!!!!!");
            GameManager.CompleteCourse();
            return;
        }
        
        transform.localScale -= new Vector3(0, ScaleStep, 0);
        transform.position -= new Vector3(ScaleStep, 0, 0);
        rend.material.mainTextureScale -= new Vector2(0, ScaleStep * 3);
    }
}
