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
    private Renderer _emberRenderer;
    private Color _emissiveColor;
    private (float min, float max, float current, float speed) glow = (0, 1000, 0, 3);
    private static readonly int EmissiveColor = Shader.PropertyToID("_EmissionColor");

    // state
    private float distance;
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
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        startPosition = _rb.position;
        stogieBurnMeter ??= transform.parent.GetComponentInChildren<Burn>();
        _emberRenderer = ember.GetComponent<Renderer>();
        _emissiveColor = _emberRenderer.material.GetColor(EmissiveColor);
        glow.current = glow.min;
        State = TokingState.Null;
        BurnEmber();
        SetCursor();
    }

    private void Update()
    {
        DragStogie();
        SetCursor();
        BurnEmber();
    }


    // =================== ## Mouse Events ## =======================
    private void OnMouseEnter() { if (State is TokingState.Null) State = TokingState.Hover; }

    private void OnMouseExit() { if (State is TokingState.Hover) State = TokingState.Null; }

    private void OnMouseDrag()
    {
        if (State is not TokingState.Smoke) State = TokingState.Hold;
        transform.position = mainCamera.ScreenToWorldPoint(Input.mousePosition - mousePos);
    }

    private void OnMouseDown()
    { 
        State = TokingState.Hold;
        mousePos = Input.mousePosition - GetMousePosition();
        // distance = Vector3.Distance(transform.position, mainCamera.transform.position); 
    }

    private Vector3 mousePos;

    private void OnMouseUp() { State = TokingState.Null; _rb.MovePosition(startPosition); }

    private Vector3 GetMousePosition() => mainCamera.WorldToScreenPoint(transform.position);


    // =================== ## Collision Events ## =======================
    private void OnTriggerEnter(Collider other)
    {
        if (State is TokingState.Null or TokingState.Hover) return;
        if (other.CompareTag("Mouth"))
        {
            Debug.Log($"WE ARE SMOKING NOW");
            State = TokingState.Smoke;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (State is TokingState.Null or TokingState.Hover) return;
        if (other.CompareTag("Mouth"))
        {
            Debug.Log($"WE ARE done smoking");
            State = TokingState.Null;
        }
    }


    // =================== ## Ember Utilities ## =======================
    private void BurnEmber()
    {
        if (IsSmoking && glow.current >= glow.max) return;
        if (!IsSmoking && glow.current <= glow.min) return;

        var newGlow = Mathf.Clamp(glow.current + glow.speed * (IsSmoking ? 1f : -1f), glow.min, glow.max);
        Debug.Log($"BurnEmber(): newGlow: {newGlow}");
        glow.current = newGlow;
        _emberRenderer.material.SetColor(EmissiveColor, _emissiveColor * glow.current);
        stogieBurnMeter.ember.material.SetColor(EmissiveColor, _emissiveColor * glow.current);
    }


    // =================== ## Smoking Utilities ## =======================
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
                Debug.Log($"SetCursor(): state: {State}");
                Cursor.SetCursor(cursorSmoking, smokeOffset, CursorMode.Auto);
                break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private void DragStogie()
    {
        // if (State != TokingState.Hold && State != TokingState.Smoke) return;
        // var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        // _rb.MovePosition(ray.GetPoint(distance));
    }
}
