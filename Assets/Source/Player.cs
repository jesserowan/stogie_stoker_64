using System;
using Source;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{

    private Rigidbody _rb;
    private CinemachineCamera _vCam;
    private CinemachineFollow _vCamFollow;
    // private CinemachineThirdPersonAim _vCamAim;
    public SpherePosition spherePosition;
    public Planet planet;
    private RaycastHit[] _groundHits;
    private Collider[] _poleHits;

    public float speed;
    public float height = 2f;
    public Pole inPole = null;
    public bool canTurn = false;


    private void OnEnable()
    {
        Debug.Log("Player.OnEnable");
        _rb = GetComponent<Rigidbody>();
        _vCam = FindFirstObjectByType<CinemachineCamera>();
        _vCamFollow = _vCam.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineFollow;
        // _vCamAim = _vCam.GetCinemachineComponent(CinemachineCore.Stage.Aim) as CinemachineThirdPersonAim;
        _groundHits = new RaycastHit[10];
        _poleHits = new Collider[10];
    }

    private void OnDisable()
    {
        _rb = null;
        _vCam = null;
        _vCamFollow = null;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Debug.Log("Starting Player");
        _vCam.Lens.FieldOfView = Constants.FieldOfView;

        spherePosition.Reset();
        _rb.MovePosition(spherePosition.VectorVectorPosition);
        _rb.MoveRotation(Quaternion.identity);
        _vCamFollow.FollowOffset.z = -20;
        _vCamFollow.FollowOffset.y = 5;
        canTurn = true;
    }

    private void FixedUpdate()
    {
        Debug.Log("Updating Player");
        var next = spherePosition.ApplySpeed(speed * Time.fixedDeltaTime, Track.Z);
        Debug.Log($"Next position {next}");

        var hits = Physics.RaycastNonAlloc(
            next + (0.5f * height * next.normalized),
            -next.normalized,
            _groundHits,
            2 * height,
            LayerMask.GetMask("Ground"));
        if (hits < 1) return;
        var right = Vector3.Cross(_groundHits[0].normal, _rb.rotation * Vector3.forward);
        var nextForward = Vector3.Cross(right, _groundHits[0].normal);
        var nextRotation = Quaternion.LookRotation(nextForward, _groundHits[0].normal);
        _rb.MoveRotation(nextRotation);
        var nextPosition = _groundHits[0].point + (nextRotation * spherePosition.LaneOffset);
        _rb.MovePosition(nextPosition);

        // var poleHits = Physics.OverlapBoxNonAlloc(nextPosition, Vector3.one * 0.5f, _poleHits, Quaternion.identity, LayerMask.GetMask("Trigger"));
        // if (poleHits < 1) return;
        // po
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (canTurn)
            {
                canTurn = false;
                Heading currentDir = speed < 0 ? Heading.Forward : Heading.Backward;
                Heading nextDir;
                (spherePosition.track, nextDir, spherePosition.lane, spherePosition.theta) =
                    inPole.GetParamsForTurn(spherePosition.track, currentDir, spherePosition.lane, Turn.Left);
                speed *= (int)nextDir;
            }
            else if (spherePosition.lane != Lane.Left)
                spherePosition.lane = spherePosition.lane is Lane.Middle ? Lane.Left : Lane.Middle;

        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (canTurn)
            {
                canTurn = false;
                Heading currentDir = speed < 0 ? Heading.Forward : Heading.Backward;
                Heading nextDir;
                (spherePosition.track, nextDir, spherePosition.lane, spherePosition.theta) =
                    inPole.GetParamsForTurn(spherePosition.track, currentDir, spherePosition.lane, Turn.Right);
                speed *= (int)nextDir;
            }
            else if (spherePosition.lane != Lane.Right)
                spherePosition.lane = spherePosition.lane is Lane.Middle ? Lane.Right : Lane.Middle;
        }
        else if (Input.GetKeyDown(KeyCode.Q)) Constants.FieldOfView -= 10f;
        else if (Input.GetKeyDown(KeyCode.E)) Constants.FieldOfView += 10f;

        else if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    private void LateUpdate()
    {
        _vCam.Lens.FieldOfView = Constants.FieldOfView;
        _vCam.Lens.FieldOfView = Constants.FieldOfView;

        // if (Input.GetKeyDown(KeyCode.R)) _vCamFollow.FollowOffset.z = 5;
        // else if (Input.GetKeyUp(KeyCode.R)) _vCamFollow.FollowOffset.z -= 3;
    }

    private void OnTriggerEnter(Collider other)
    {
        inPole = other.gameObject.GetComponent<Pole>();
        canTurn = true;
    }
    private void OnTriggerExit(Collider other)
    {
        inPole = null;
        canTurn = false;
    }
}
