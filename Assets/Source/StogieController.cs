// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using System;
using UnityEngine;

public enum TokingState { Null, Hover, Hold, Smoke, }

public class StogieController : MonoBehaviour
{
    // data
    private Rigidbody rb;
    public Camera mainCamera;

    // cursor textures
    public Texture2D cursorDefault;
    public Texture2D cursorHovering;
    public Texture2D cursorHolding;
    public Texture2D cursorSmoking;
    
    // cigar control
    [SerializeField]
    private GameObject ember;
    private Renderer _emberRenderer;
    private Color _emissiveColor;
    private (float min, float max, float current, float speed) glow = (0, 1000, 0, 3);
    private static readonly int EmissiveColor = Shader.PropertyToID("_EmissionColor");
    
    // state
    private float distance;
    private Vector3 startPosition;
    public TokingState state = TokingState.Null;
    
    // props
    public bool IsSmoking => state == TokingState.Smoke;
    
    
    // =================== ## Lifecycle ## =======================
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = rb.position;
        
        _emberRenderer = ember.GetComponent<Renderer>();
        _emissiveColor = _emberRenderer.material.GetColor(EmissiveColor);
        glow.current = glow.min;
        BurnEmber();
        
        SetCursor();
    }
    
    private void Update() { DragStogie(); SetCursor(); BurnEmber(); }


    // =================== ## Mouse Events ## =======================
    private void OnMouseEnter() { if (state == TokingState.Null) state = TokingState.Hover; }

    private void OnMouseExit() { if (state == TokingState.Hover) state = TokingState.Null; }

    private void OnMouseDrag() { if (state != TokingState.Smoke) state = TokingState.Hold; }

    private void OnMouseDown()
    { state = TokingState.Hold;
        distance = Vector3.Distance(transform.position, mainCamera.transform.position); }
    
    private void OnMouseUp()
    { state = TokingState.Null;
        rb.MovePosition(startPosition); }
    
    
    // =================== ## Collision Events ## =======================
    private void OnTriggerEnter(Collider other) { if (other.CompareTag("Mouth")) state = TokingState.Smoke; }

    private void OnTriggerExit(Collider other) { if (other.CompareTag("Mouth")) state = TokingState.Null; }
    
    
    // =================== ## Ember Utilities ## =======================
    public void BurnEmber()
    {
        var newGlow = glow.current + glow.speed * (IsSmoking ? 1f : -1f);
        glow.current = Mathf.Clamp(newGlow, glow.min, glow.max);
        if (state == TokingState.Smoke)
            _emberRenderer.material.SetColor(EmissiveColor, _emissiveColor * glow.current);
    }


    // =================== ## Smoking Utilities ## =======================
    public Vector2 defaultOffset;
    public Vector2 hoverOffset;
    public Vector2 holdOffset;
    public Vector2 smokeOffset;
    public void SetCursor()
    { switch (state)
        { case TokingState.Null: 
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

    public void DragStogie()
    {
        if (state != TokingState.Hold && state != TokingState.Smoke) return;
        
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        rb.MovePosition(ray.GetPoint(distance));
    }
}