// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;


public enum TurnDirection
{
    Left,
    Right,
}

public class Orbit : MonoBehaviour
{
    
    [SerializeField] private float speed;
    [SerializeField] private GameObject orbitObject;
    [SerializeField] private GameObject body;
    [SerializeField] private CinemachineCamera vCam;
    [SerializeField] private CinemachineFollow vCamFollow;
    private bool isTurning;
    

    private void Start()
    {
        speed = 10f;
        body = transform.GetChild(0).gameObject;
        vCam = FindFirstObjectByType<CinemachineCamera>();
        vCam.Lens.FieldOfView = 70;
        vCamFollow = vCam.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineFollow;
    }

    private void Update()
    {
        transform.RotateAround(orbitObject.transform.position, transform.right, speed * Time.deltaTime);
        
        if (Input.GetKeyDown(KeyCode.W)) ChangeSpeed(10f);
        else if (Input.GetKeyDown(KeyCode.A)) Turn(TurnDirection.Left);
        else if (Input.GetKeyDown(KeyCode.S)) ChangeSpeed(-10f);
        else if (Input.GetKeyDown(KeyCode.D)) Turn(TurnDirection.Right);
        
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            body.transform.localPosition = new Vector3(0f, 0, 0f);
            body.transform.localScale = new Vector3(.5f, .5f, .5f);
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            body.transform.localPosition = new Vector3(0f, -.25f, 0f);
            body.transform.localScale = new Vector3(.6f, 0.25f, .6f);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            body.transform.localPosition = new Vector3(0f, 0f, 0f);
            body.transform.localScale = new Vector3(.5f, .5f, .5f);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            body.transform.localPosition = new Vector3(0f, 1f, 0f);
            body.transform.localScale = new Vector3(.6f, 0.25f, .6f);
        }
        
        else if (Input.GetKeyDown(KeyCode.Q)) vCam.Lens.FieldOfView = Mathf.Clamp(vCam.Lens.FieldOfView - 10f, 20f, 170f);
        else if (Input.GetKeyDown(KeyCode.E)) vCam.Lens.FieldOfView = Mathf.Clamp(vCam.Lens.FieldOfView + 10f, 20f, 170f);
        
        else if (Input.GetKeyDown(KeyCode.R)) vCamFollow.FollowOffset.z = 5;
        else if (Input.GetKeyUp(KeyCode.R)) vCamFollow.FollowOffset.z = -3;
        
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void ChangeSpeed(float delta)
    {
        speed = Mathf.Clamp(speed + delta, 0f, 180f);
    }

    private void Turn(TurnDirection turnDir)
    {
        // if (isTurning) return;
        // isTurning = true;
        // StartCoroutine(RotateNinetyDegrees(turnDir));
        var step = turnDir == TurnDirection.Right ? 1f : -1f;
        transform.position += Vector3.right * step;
    }

    private IEnumerator RotateNinetyDegrees(TurnDirection turnDir)
    {
        var turned = 0f;
        var step = turnDir == TurnDirection.Right ? 1f : -1f;
        while (turned < 180)
        {
            transform.Rotate(Vector3.up, step);
            turned += 1f;
            yield return null;
        }
        
        isTurning = false;
    }
}
