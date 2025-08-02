using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{

    private Rigidbody _rb;
    private RaycastHit[] _groundHits;
    private PlayerAnimator _animator;
    private Collider[] _poleHits;

    [SerializeField] private Pole northPole;
    [SerializeField] private Pole southPole;
    public Pole lastPole;
    public Pole nextPole =>
        lastPole.which == Polarity.South
            ? northPole : southPole;

    public IntegerVariable lives;
    public FloatConstant playerSpeed;
    private const float HEIGHT = 2f;
    private float currentSpeed;
    public bool canTurn;
    private int doTurn;
    public Location location;
    private bool hasExitedStartingPose;

    public Heading CurrentHeading => currentSpeed < 0 ? Heading.Forward : Heading.Backward;

    private void OnEnable()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<PlayerAnimator>();
        _animator.completeAnimation.AddListener(ResetVertical);
        _groundHits = new RaycastHit[10];
        _poleHits = new Collider[10];
    }

    private void OnDisable() { _rb = null; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        location.Reset();
        hasExitedStartingPose = false;
        var pose = location.DeriveWorldPose();
        _rb.Move(pose.position, pose.rotation);
        currentSpeed = playerSpeed.Value;
        lives.Value = 100;
    }

    private void FixedUpdate()
    {
        var multiplier = GameManager.CurrentDifficulty.speedMultiplier;
        var next = location.ApplySpeed(currentSpeed * multiplier * Time.fixedDeltaTime, Track.Z);

        var hits = Physics.RaycastNonAlloc(
            next + (0.5f * HEIGHT * next.normalized),
            -next.normalized,
            _groundHits,
            2 * HEIGHT,
            LayerMask.GetMask("Ground"));
        if (hits < 1) return;

        var rbt = _rb.transform;
        var nextPosition = rbt.position;
        Quaternion nextRotation;
        if (doTurn != 0) {
            nextRotation = Quaternion.LookRotation(doTurn * rbt.right, rbt.up);
            doTurn = 0;
        } else {
            var right = Vector3.Cross(_groundHits[0].normal, rbt.rotation * Vector3.forward);
            var nextForward = Vector3.Cross(right, _groundHits[0].normal);
            nextRotation = Quaternion.LookRotation(nextForward, _groundHits[0].normal);
            nextPosition = _groundHits[0].point + (nextRotation * location.LaneOffset);
        }

        _rb.MoveRotation(nextRotation);
        _rb.MovePosition(nextPosition);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) {
            if (canTurn && lastPole) {
                canTurn = false; Heading nextDir;
                var currentDir = currentSpeed > 0 ? Heading.Forward : Heading.Backward;
                (location.track, nextDir, location.lane, location.theta) =
                    lastPole.GetParamsForTurn(location.track, currentDir, location.lane, Turn.Left);
                currentSpeed = (int)nextDir * Mathf.Abs(currentSpeed);
                doTurn = (int)Turn.Left; var rbt = _rb.transform;
                _rb.MoveRotation(Quaternion.LookRotation(-rbt.right, rbt.up));
            }
            else if (location.lane != Lane.Left)
            {
                location.lane = location.lane is Lane.Center ? Lane.Left : Lane.Center;
            }

        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (canTurn && lastPole) { canTurn = false; Heading nextDir;
                var currentDir = currentSpeed > 0 ? Heading.Forward : Heading.Backward;
                (location.track, nextDir, location.lane, location.theta) =
                    lastPole.GetParamsForTurn(location.track, currentDir, location.lane, Turn.Right);
                currentSpeed = (int)nextDir * Mathf.Abs(currentSpeed);
                doTurn = (int)Turn.Right; var rbt = _rb.transform;
                _rb.MoveRotation(Quaternion.LookRotation(rbt.right, rbt.up));
            }
            else if (location.lane != Lane.Right)
            {
                location.lane = location.lane is Lane.Center ? Lane.Right : Lane.Center;
            }
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            Debug.Log("Jump");
            if (location.row is Row.Span) {
                Debug.Log("allowing Jump");
                _animator.Play(_animator.Jump);
                location.row = Row.Upper;
            }
        } else if (Input.GetKeyDown(KeyCode.S)) {
            Debug.Log("Slide");
            if (location.row is Row.Span) {
                Debug.Log("allowing Slide");
                _animator.Play(_animator.Slide);
                location.row = Row.Lower;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q)) Constants.FieldOfView -= 10f;
        else if (Input.GetKeyDown(KeyCode.E)) Constants.FieldOfView += 10f;
        else if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    private void ResetVertical() { location.row = Row.Span; }

    private void OnTriggerEnter(Collider other)
    {
        var pole = other.gameObject.GetComponent<Pole>();
        if (pole) {
            GameManager.EnterPole(pole);
            lastPole = pole;
            canTurn = true;
            return;
        }

        var obstacle = other.gameObject.GetComponent<Obstacle>();
        if (obstacle) {
            var position = location.TrackOccupation;
            if (obstacle.IsObstructiveTo(position)) {
                if (lives.Value >= 0)
                {
                    lives.Value -= 1;
                    _animator.Play(_animator.Trip);
                }
                else
                {
                    _animator.Play(_animator.Fall);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Pole>(out var p)) return;

        GameManager.ExitPole(p);
        hasExitedStartingPose = true;
        canTurn = false;
    }
}

