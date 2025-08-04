using System;
using System.Collections;
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

    public IntegerVariable lives;
    public FloatConstant playerSpeed;
    private const float HEIGHT = 2f;
    public float currentSpeed;
    public bool canTurn;
    private int doTurn;
    public Location location;
    private bool hasExitedStartingPose;

    public bool hasStarted;
    public bool IsTrackOpen(Vector3 targetTrack)
    {
        Debug.Log($"IsTrackOpen: local/inpole: {canTurn}; target track: {targetTrack}");
        if (!canTurn) return false;
        Debug.Log($"IsTrackOpen: checking for roadblock at [{lastPole.which}][{targetTrack}]");
        var obstacleSlot = GameManager.Instance.obstacleManager.Intersections[lastPole.which][targetTrack];
        Debug.Log($"######## slot occupant: {obstacleSlot}");
        return obstacleSlot == null;
    }

    public Heading CurrentHeading => currentSpeed > 0 ? Heading.Forward : Heading.Backward;

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
    private IEnumerator Start()
    {
        hasStarted = false;
        currentSpeed = 0f;
        location.Reset();
        hasExitedStartingPose = false;
        var pose = location.DeriveWorldPose();
        _rb.Move(pose.position, pose.rotation);
        lives.Reset();
        yield return new WaitForSeconds(3f);
        currentSpeed = playerSpeed.Value;
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

    public bool banJumping;
    private void Update()
    {
        if (!hasStarted) return;
        if (Input.GetKeyDown(KeyCode.A)) QueueTurn(Vector3.left);
        else if (Input.GetKeyDown(KeyCode.D)) QueueTurn(Vector3.right);

        if (Input.GetKeyDown(KeyCode.W)) {
            Debug.Log("Jump");
            if (location.row is Row.Span && !banJumping) { // temp; 
                // Debug.Log("allowing Jump");
                GameManager.BroadcastJump();
                _animator.Play(_animator.Jump);
                location.row = Row.Upper;
            }
        } else if (Input.GetKeyDown(KeyCode.S)) {
            Debug.Log("Slide");
            if (location.row is Row.Span && !banJumping) {
                // Debug.Log("allowing Slide");
                GameManager.BroadcastSlide();
                _animator.Play(_animator.Slide);
                location.row = Row.Lower;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q)) Constants.FieldOfView -= 10f;
        else if (Input.GetKeyDown(KeyCode.E)) Constants.FieldOfView += 10f;
        // else if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    private void ResetVertical() { location.row = Row.Span; }

    private void OnTriggerEnter(Collider other)
    {
        var pole = other.gameObject.GetComponent<Pole>();
        if (pole) {
            GameManager.EnterPole(pole);
            lastPole = pole;
            if (hasStarted) canTurn = true;
            return;
        }

        var obstacle = other.gameObject.GetComponent<Obstacle>();
        if (obstacle) {
            // Debug.Log($"Encountered Obstacle: {obstacle.name} {obstacle}");
            if (obstacle.isRoadblock)
            {
                // Debug.Log($"######## Roadblock detected");
                var noTurn = GetTurnVector(Vector3.forward);
                var left = GetTurnVector(Vector3.left);
                var right = GetTurnVector(Vector3.right);
                // Debug.Log($"######## mapped local turn options to global track vector3 keys: " +
                //           $"straight: {noTurn} left: {left} right: {right}");
                if (!IsTrackOpen(noTurn))
                {
                    // Debug.Log($"TRIGGER ######## straight ahead should be closed since we hit it");
                    if (IsTrackOpen(left))
                    {
                        // Debug.Log($"Can turn left: {left}");
                        QueueTurn(Vector3.left);
                    }
                    else if (IsTrackOpen(right))
                    {
                        // Debug.Log($"Can turn right: {right}");
                        QueueTurn(Vector3.right);
                    }
                    else throw new Exception($"TEST: No available tracks to turn onto.");
                }
            }
            var position = location.TrackOccupation;
            if (obstacle.IsObstructiveTo(position)) {
                if (lives.Value >= 0)
                {
                    lives.Value -= 1;
                    _animator.Play(_animator.Trip);
                    GameManager.BroadcastImpact();
                }
                else
                {
                    _animator.Play(_animator.Fall);
                    GameManager.CompleteCourse(false);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Pole>(out var p)) return;

        if (!hasStarted) hasStarted = true;
        GameManager.ExitPole(p);
        hasExitedStartingPose = true;
        canTurn = false;
    }

    private bool Forward => CurrentHeading == Heading.Forward;
    private bool North => lastPole.which == Polarity.North;
    private Vector3 GetTurnVector(Vector3 turnRequest)
    {
        // turnRequest is LOCAL not global (e.g. from the player's perspective not in world space
        // this fn takes local turn and maps to global Vector3 track id
        Vector3 turnVector;
        Debug.Log($"GetTurnVector: local direction request: {turnRequest}; heading: {CurrentHeading}; forward: {Forward}; north: {North}");
        if (turnRequest == Vector3.left)
        {
            if (location.track == Track.Z) turnVector = Forward ? Vector3.left : Vector3.right;
            else turnVector = Forward ? Vector3.forward : Vector3.back;
            // Debug.Log($"######## left turn leads to track: {turnVector}");
        }
        else if (turnRequest == Vector3.right)
        {
            if (location.track == Track.Z) turnVector = Forward ? Vector3.right : Vector3.left;
            else turnVector = Forward ? Vector3.back : Vector3.forward;
            // Debug.Log($"######## right turn leads to track: {turnVector}");
        }
        else if (turnRequest == Vector3.forward)
        {
            if (location.track == Track.Z)
            {
                turnVector = North
                    ? Forward ? Vector3.forward : Vector3.back
                    : Forward ? Vector3.back : Vector3.forward; 
            }
            else
            {
                turnVector = North
                    ? Forward ? Vector3.right : Vector3.left
                    : Forward ? Vector3.left : Vector3.right; 
            }
            // Debug.Log($"######## no turn leads to track: {turnVector}");
        }
        else throw new Exception("invalid turn?");
        return turnVector;
    }

    private void QueueTurn(Vector3 turnDirection)
    {
        if (turnDirection == Vector3.left)
        {
            if (canTurn && lastPole) {
                Heading nextDir;
                Vector3 turnVector = GetTurnVector(Vector3.left);
                if (!IsTrackOpen(turnVector))
                {
                    // Debug.Log($"QueueTurn(): Requested left turn to track {turnVector} is unavailable.");
                    canTurn = false; // make sure it's set if we exit before updating it
                    return;
                }

                canTurn = false;
                (location.track, nextDir, location.lane, location.theta) =
                    lastPole.GetParamsForTurn(location.track, CurrentHeading, location.lane, Turn.Left);
                currentSpeed = (int)nextDir * Mathf.Abs(currentSpeed);
                doTurn = (int)Turn.Left; var rbt = _rb.transform;
                _rb.MoveRotation(Quaternion.LookRotation(-rbt.right, rbt.up));
            }
            else if (location.lane != Lane.Left)
            {
                location.lane = location.lane is Lane.Center ? Lane.Left : Lane.Center;
                GameManager.BroadcastLaneSwitch(location.lane);
            }
        } else if (turnDirection == Vector3.right)
        {
            if (canTurn && lastPole) { 
                Heading nextDir;
                Vector3 turnVector = GetTurnVector(Vector3.right);
                if (!IsTrackOpen(turnVector))
                {
                    // Debug.Log($"QueueTurn(): Requested right turn to track {turnVector} is unavailable.");
                    canTurn = false; // make sure it's set if we exit before updating it 
                    return;
                }
                canTurn = false; 
                (location.track, nextDir, location.lane, location.theta) =
                    lastPole.GetParamsForTurn(location.track, CurrentHeading, location.lane, Turn.Right);
                currentSpeed = (int)nextDir * Mathf.Abs(currentSpeed);
                doTurn = (int)Turn.Right; var rbt = _rb.transform;
                _rb.MoveRotation(Quaternion.LookRotation(rbt.right, rbt.up));
            }
            else if (location.lane != Lane.Right)
            {
                location.lane = location.lane is Lane.Center ? Lane.Right : Lane.Center;
                GameManager.BroadcastLaneSwitch(location.lane);
            }
        }
    }
}

