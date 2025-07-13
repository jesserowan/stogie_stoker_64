// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

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
    [SerializeField] public GameObject planet;
    [SerializeField] public GameObject body;
    [SerializeField] private CinemachineCamera vCam;
    [SerializeField] private CinemachineFollow vCamFollow;
    [SerializeField] public Vector2 position = Vector2.zero;
    [SerializeField] public float radius = 16.8f;
    [SerializeField] public Lane lane = Lane.Middle;


    private bool isTurning;


    private void Start()
    {
        speed = 10f;
        vCam ??= FindFirstObjectByType<CinemachineCamera>();
        vCam.Lens.FieldOfView = 70;
        vCamFollow = vCam.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineFollow;


    }

    private void Update()
    {
        transform.RotateAround(planet.transform.position, transform.right, speed * Time.deltaTime);


        if (Input.GetKeyDown(KeyCode.W)) ChangeSpeed(10f);
        else if (Input.GetKeyDown(KeyCode.A)) Turn(TurnDirection.Left);
        else if (Input.GetKeyDown(KeyCode.S)) ChangeSpeed(-10f);
        else if (Input.GetKeyDown(KeyCode.D)) Turn(TurnDirection.Right);

        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            body.transform.localPosition = new Vector3(0f, 0, 0f);
            body.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            body.transform.localPosition = new Vector3(0f, -.33f, 0f);
            body.transform.localScale = new Vector3(1.1f, 0.33f, 1.1f);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            body.transform.localPosition = new Vector3(0f, 0f, 0f);
            body.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            body.transform.localPosition = new Vector3(0f, 1f, 0f);
            body.transform.localScale = new Vector3(1.1f, 0.33f, 1.1f);
        }

        else if (Input.GetKeyDown(KeyCode.Q)) vCam.Lens.FieldOfView = Mathf.Clamp(vCam.Lens.FieldOfView - 10f, 20f, 170f);
        else if (Input.GetKeyDown(KeyCode.E)) vCam.Lens.FieldOfView = Mathf.Clamp(vCam.Lens.FieldOfView + 10f, 20f, 170f);

        else if (Input.GetKeyDown(KeyCode.R)) vCamFollow.FollowOffset.z = 5;
        else if (Input.GetKeyUp(KeyCode.R)) vCamFollow.FollowOffset.z = -3;

        else if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

    }

    private void ChangeSpeed(float delta)
    {
        speed = Mathf.Clamp(speed + delta, 0f, 180f);
    }

    private void Turn(TurnDirection turnDir)
    {
        if (isTurning) return;
        isTurning = true;
        StartCoroutine(RotateNinetyDegrees(turnDir));
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
