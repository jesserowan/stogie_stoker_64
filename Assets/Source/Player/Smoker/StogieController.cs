// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using System;
using UnityEngine;

public enum TokingState { Null, Hover, Hold, Smoke }

public class StogieController : MonoBehaviour
{
    // data
    private Rigidbody _rb;
    public Camera mainCamera;

    // cursor textures
    public Texture2D cursorDefault;
    public Texture2D cursorHovering;
    public Texture2D cursorHolding;
    public Texture2D cursorSmoking;

    // cigar control
    [SerializeField] private GameObject ember;
    [SerializeField] private Burn stogieBurnMeter;
    [SerializeField] private GhostStogie ghostStogie;
    private Renderer _emberRenderer;
    private Renderer renderer;
    private Color _emissiveColor;
    private (float min, float max, float current, float speed) glow = (0, 1000, 0, 3);
    private static readonly int EmissiveColor = Shader.PropertyToID("_EmissionColor");

    // state
    private Vector3 mousePos;
    private Vector3 startPosition;

    private TokingState _state;
    public TokingState State {
        get => _state;
        set {
            isSmoking.Value = value is TokingState.Smoke;
            _state = value;
        }
    }

    // props
    [SerializeField] public BooleanVariable isSmoking;
    public bool IsSmoking => isSmoking.Value;


    // =================== ## Lifecycle ## =======================
    private void OnEnable()
    {
        GameManager.OnImpact += DropStogie;
        GameManager.OnLose += DropStogie;
    }

    private void OnDisable()
    {
        GameManager.OnImpact -= DropStogie;
        GameManager.OnLose -= DropStogie;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        startPosition = _rb.position;
        stogieBurnMeter ??= transform.parent.GetComponentInChildren<Burn>();
        renderer = GetComponent<Renderer>();
        _emberRenderer = ember.GetComponent<Renderer>();
        _emissiveColor = _emberRenderer.material.GetColor(EmissiveColor);
        glow.current = glow.min;
        State = TokingState.Null;
        ToggleRenderers(true);
        BurnEmber();
        SetCursor();
    }

    private bool gameoverTriggered;
    private void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameState.GameOver)
        {
            if (!gameoverTriggered)
            {
                gameoverTriggered = true;
                State = TokingState.Smoke;
                ToggleRenderers(false);
                SetCursor();
                BurnEmber();
            }

            return;
        }
        
        SetCursor();
        BurnEmber();
    }


    // =================== ## Mouse Events ## =======================
    private void OnMouseEnter() { if (State is TokingState.Null) State = TokingState.Hover; }

    private void OnMouseExit() { if (State is TokingState.Hover) State = TokingState.Null; }

    private void OnMouseDown()
    { 
        if (gameoverTriggered) return;
        State = TokingState.Hold;
        mousePos = Input.mousePosition - GetPositionInScreenSpace();
    }

    private void OnMouseDrag()
    {
        if (gameoverTriggered) return;
        if (State == TokingState.Null) return;
        if (State != TokingState.Smoke) State = TokingState.Hold;
        transform.position = mainCamera.ScreenToWorldPoint(Input.mousePosition - mousePos);
    }

    private void OnMouseUp()
    {
        if (gameoverTriggered) return;
        DropStogie();
    }

    private Vector3 GetPositionInScreenSpace() => mainCamera.WorldToScreenPoint(transform.position);


    // =================== ## Collision Events ## =======================
    private void OnTriggerEnter(Collider other)
    {
        if (State is TokingState.Null or TokingState.Hover) return; // only start smoking if cigar is held
        if (other.CompareTag("Mouth")) State = TokingState.Smoke;
    }

    private void OnTriggerExit(Collider other)
    {
        if (State is TokingState.Null or TokingState.Hover) return; // only start smoking if cigar is held
        if (other.CompareTag("Mouth")) State = TokingState.Hold;
    }


    // =================== ## Ember Utilities ## =======================
    private void BurnEmber()
    {
        if (GameManager.Instance.CurrentGameState == GameState.GameOver) // maintain max glow during fadeout
        {
            if (gameoverTriggered) return;
            ApplyGlow(glow.max);
            return;
        }
        
        // always approach min or max
        if (IsSmoking && glow.current >= glow.max) return;
        if (!IsSmoking && glow.current <= glow.min) return;

        var newGlow = Mathf.Clamp(glow.current + glow.speed * (IsSmoking ? 1f : -1f), glow.min, glow.max);
        glow.current = newGlow;
        
        // set glow for draggable cigar, ghost cigar, and UI cigar
        ApplyGlow(glow.current);

        // if smoking, enable ghost cigar (locked to mouth) and disable this one
        ToggleRenderers(!IsSmoking);
    }

    private void ApplyGlow(float glowFactor)
    {
        _emberRenderer.material.SetColor(EmissiveColor, _emissiveColor * glowFactor);
        stogieBurnMeter.ember.material.SetColor(EmissiveColor, _emissiveColor * glowFactor);
        ghostStogie.emb.material.SetColor(EmissiveColor, _emissiveColor * glowFactor);
    }

    private void ToggleRenderers(bool on)
    {
        renderer.enabled = on;
        _emberRenderer.enabled = on;
    }


    // =================== ## Smoking Utilities ## =======================
    // pixel offsets for each cursor; (x,y) represents pixels from top left to set as cursor hotspot
    // e.g. if cursor is 64x64, to make the click hotspot in the middle the offset would be (32,32);
    public Vector2 defaultOffset;
    public Vector2 hoverOffset;
    public Vector2 holdOffset;
    public Vector2 smokeOffset;

    private void SetCursor()
    {
        switch (State) {
            case TokingState.Null:
                Cursor.SetCursor(cursorDefault, defaultOffset, CursorMode.Auto);
                break;
            case TokingState.Hover:
                Cursor.SetCursor(cursorHovering, hoverOffset, CursorMode.Auto);
                break;
            case TokingState.Hold:
                Cursor.SetCursor(cursorHolding, holdOffset, CursorMode.Auto);
                break;
            case TokingState.Smoke:
                Cursor.SetCursor(cursorSmoking, smokeOffset, CursorMode.Auto);
                break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    public void DropStogie()
    {
        mousePos = Input.mousePosition;
        State = TokingState.Null;
        transform.position = startPosition;
        ApplyGlow(glow.min);
        SetCursor();
        BurnEmber();
    }
}
