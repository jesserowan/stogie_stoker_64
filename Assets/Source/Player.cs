using Source;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour
{

    private Rigidbody _rb;
    // private CinemachineCamera _vCam;
    // private CinemachineFollow _vCamFollow;
    // private CinemachineThirdPersonAim _vCamAim;
    private PlayerAnimator _animator;
    public SpherePosition spherePosition;
    public Planet planet;
    private RaycastHit[] _groundHits;
    private Collider[] _poleHits;
    
    [SerializeField] private Pole northPole; 
    [SerializeField] private Pole southPole;
    public Pole currentPole;
    public Pole nextPole => currentPole.which == PoleType.Nadir ? northPole : southPole;

    public float speed;
    public float height = 2f;
    public Pole inPole = null;
    public bool canTurn = false;
    private int doTurn = 0;

    public Heading CurrentHeading => speed < 0 ? Heading.Forward : Heading.Backward;

    private void OnEnable()
    {
        Debug.Log("Player.OnEnable");
        _rb = GetComponent<Rigidbody>();
        // _vCam = FindFirstObjectByType<CinemachineCamera>();
        _animator = GetComponentInChildren<PlayerAnimator>();
        // _vCamFollow = _vCam.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineFollow;
        // _vCamAim = _vCam.GetCinemachineComponent(CinemachineCore.Stage.Aim) as CinemachineThirdPersonAim;
        _groundHits = new RaycastHit[10];
        _poleHits = new Collider[10];
        
        var poles = FindObjectsByType<Pole>(0);
        Assert.IsTrue(poles.Length == 2);
        foreach (var p in poles)
        { if (p.which == PoleType.Nadir) southPole = p;
            if (p.which == PoleType.Zenith) northPole = p; }
    }

    private void OnDisable()
    {
        _rb = null;
        // _vCam = null;
        // _vCamFollow = null;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Debug.Log("Starting Player");
        // _vCam.Lens.FieldOfView = Constants.FieldOfView;

        spherePosition.Reset();
        // currentPole = northPole;
        _rb.MovePosition(spherePosition.VectorVectorPosition);
        _rb.MoveRotation(Quaternion.identity);
        // _vCamFollow.FollowOffset.z = -20;
        // _vCamFollow.FollowOffset.y = 5;
        // canTurn = true;
    }

    private void FixedUpdate()
    {
        // Debug.Log("Updating Player");
        var next = spherePosition.ApplySpeed(speed * Time.fixedDeltaTime, Track.Z);
        // Debug.Log($"Next position {next}");

        var hits = Physics.RaycastNonAlloc(
            next + (0.5f * height * next.normalized),
            -next.normalized,
            _groundHits,
            2 * height,
            LayerMask.GetMask("Ground"));
        if (hits < 1) return;

        var rbt = _rb.transform;
        Vector3 nextPosition = rbt.position;
        Quaternion nextRotation;
        if (doTurn != 0) {
            nextRotation = Quaternion.LookRotation(doTurn * rbt.right, rbt.up);
            doTurn = 0;
        } else {
            var right = Vector3.Cross(_groundHits[0].normal, rbt.rotation * Vector3.forward);
            var nextForward = Vector3.Cross(right, _groundHits[0].normal);
            nextRotation = Quaternion.LookRotation(nextForward, _groundHits[0].normal);
            nextPosition = _groundHits[0].point + (nextRotation * spherePosition.LaneOffset);
        }
        _rb.MoveRotation(nextRotation);
        _rb.MovePosition(nextPosition);

        // var poleHits = Physics.OverlapBoxNonAlloc(nextPosition, Vector3.one * 0.5f, _poleHits, Quaternion.identity, LayerMask.GetMask("Trigger"));
        // if (poleHits < 1) return;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (canTurn)
            {
                canTurn = false;
                Heading nextDir;
                var currentDir = speed < 0 ? Heading.Forward : Heading.Backward;
                (spherePosition.track, nextDir, spherePosition.lane, spherePosition.theta) =
                    inPole.GetParamsForTurn(spherePosition.track, currentDir, spherePosition.lane, Turn.Left);
                speed = (int)nextDir * Mathf.Abs(speed);
                var rbt = _rb.transform;
                _rb.MoveRotation(Quaternion.LookRotation(-rbt.right, rbt.up));
                doTurn = (int)Turn.Left;
            }
            else if (spherePosition.lane != Lane.Left)
                spherePosition.lane = spherePosition.lane is Lane.Middle ? Lane.Left : Lane.Middle;

        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (canTurn)
            {
                canTurn = false;
                Heading nextDir;
                var currentDir = speed < 0 ? Heading.Forward : Heading.Backward;
                (spherePosition.track, nextDir, spherePosition.lane, spherePosition.theta) =
                    inPole.GetParamsForTurn(spherePosition.track, currentDir, spherePosition.lane, Turn.Right);
                speed = (int)nextDir * Mathf.Abs(speed);
                doTurn = (int)Turn.Right;
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
        // _vCam.Lens.FieldOfView = Constants.FieldOfView;
        // _vCam.Lens.FieldOfView = Constants.FieldOfView;

        // if (Input.GetKeyDown(KeyCode.R)) _vCamFollow.FollowOffset.z = 5;
        // else if (Input.GetKeyUp(KeyCode.R)) _vCamFollow.FollowOffset.z -= 3;
    }

    private void OnTriggerEnter(Collider other)
    {
        inPole = other.gameObject.GetComponent<Pole>();
        GameManager.EnterPole(inPole);
        if (GameManager.Instance.CurrentGameState == GameState.Initializing) return;
        canTurn = true;
    }
    private void OnTriggerExit(Collider other)
    {
        GameManager.ExitPole();
        inPole = null;
        canTurn = false;
    }
}
