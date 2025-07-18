// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using System;
using UnityEngine;

public enum TokingState
{
    Null,
    Hover,
    Hold,
    Smoke,
}

public class StogieController : MonoBehaviour
{
    // data
    public Rigidbody rb;
    public Camera mainCamera;
    public Texture2D cursorDefault;
    public Texture2D cursorHovering;
    public Texture2D cursorHolding;
    public Texture2D cursorSmoking;
    public Stogie stogie;
    
    // state
    private bool dragging;
    private float distance;
    public Vector3 startPosition;
    public TokingState state = TokingState.Null;
    
    
    // =================== ## Lifecycle ## =======================
    private void Start()
    {
        mainCamera ??= Camera.main;
        rb = GetComponent<Rigidbody>();
        startPosition = rb.position;
        stogie = GetComponent<Stogie>();
        SetCursor();
    }
    
    private void Update()
    {
        SetCursor();
        if (!dragging) return;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 rayPoint = ray.GetPoint(distance);
        rb.MovePosition(rayPoint);
    }


    // =================== ## Mouse Events ## =======================
    private void OnMouseEnter()
    {
        if (state == TokingState.Null) state = TokingState.Hover;
    }

    private void OnMouseExit()
    {
        if (state == TokingState.Hover) state = TokingState.Null;
    }

    private void OnMouseDrag()
    {
        if (state != TokingState.Smoke) state = TokingState.Hold;
    }

    private void OnMouseDown()
    {
        state = TokingState.Hold;
        distance = Vector3.Distance(transform.position, mainCamera.transform.position);
        dragging = true;
    }
    
    private void OnMouseUp()
    {
        dragging = false;
        state = TokingState.Null;
        rb.MovePosition(startPosition);
    }
    
    
    // =================== ## Collision Events ## =======================
    private void OnTriggerEnter(Collider other)
    {
        // if (smoking) return;
        if (other.CompareTag("Mouth")) state = TokingState.Smoke;
        stogie.smoking = true;
    }

    private void OnTriggerExit(Collider other)
    {
        // if (!smoking) return;
        if (other.CompareTag("Mouth")) state = TokingState.Null;
        stogie.smoking = false;
    }

    
    // =================== ## Utilities ## =======================
    public Vector2 defaultOffset;
    public Vector2 hoverOffset;
    public Vector2 holdOffset;
    public Vector2 smokeOffset;
    public void SetCursor()
    {
        switch (state)
        {
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
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}