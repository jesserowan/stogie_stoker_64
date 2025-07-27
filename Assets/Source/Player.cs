using UnityEngine;

public class Player : MonoBehaviour
{

    private Rigidbody _rb;
    private RaycastHit[] _groundHits;
    private PlayerAnimator _animator;
    private Collider[] _poleHits;

    [SerializeField] private Pole northPole;
    [SerializeField] private Pole southPole;
    public Pole lastPole;
    public Pole nextPole => lastPole.which == Polarity.South ? northPole : southPole;

    public const int MAX_LIVES = 100;

    public int lives;
    public float speed;
    public float height = 2f;
    public bool canTurn;
    private int doTurn;
    public Location location;
    private bool hasExitedStartingPose;

    public Heading CurrentHeading => speed < 0 ? Heading.Forward : Heading.Backward;

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
        lives = MAX_LIVES;
        location.Reset();
        hasExitedStartingPose = false;
        var pose = location.DeriveWorldPose();
        _rb.Move(pose.position, pose.rotation);
    }

    private void FixedUpdate()
    {
        var multiplier = GameManager.CurrentDifficulty switch {
            Difficulty.Hard => 1f,
            Difficulty.Mid => 0.75f,
            _ => 0.5f
        };

        var next = location.ApplySpeed(speed * multiplier * Time.fixedDeltaTime, Track.Z);

        var hits = Physics.RaycastNonAlloc(
            next + (0.5f * height * next.normalized),
            -next.normalized,
            _groundHits,
            2 * height,
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
                canTurn = false;
                Heading nextDir;
                var currentDir = speed < 0 ? Heading.Forward : Heading.Backward;
                (location.track, nextDir, location.lane, location.theta) =
                    lastPole.GetParamsForTurn(location.track, currentDir, location.lane, Turn.Left);
                speed = (int)nextDir * Mathf.Abs(speed);
                var rbt = _rb.transform;
                _rb.MoveRotation(Quaternion.LookRotation(-rbt.right, rbt.up));
                doTurn = (int)Turn.Left;
            }
            else if (location.lane != Lane.Left)
            {
                location.lane = location.lane is Lane.Center ? Lane.Left : Lane.Center;
            }

        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (canTurn && lastPole)
            {
                canTurn = false;
                Heading nextDir;
                var currentDir = speed < 0 ? Heading.Forward : Heading.Backward;
                (location.track, nextDir, location.lane, location.theta) =
                    lastPole.GetParamsForTurn(location.track, currentDir, location.lane, Turn.Right);
                speed = (int)nextDir * Mathf.Abs(speed);
                doTurn = (int)Turn.Right;
            }
            else if (location.lane != Lane.Right)
            {
                location.lane = location.lane is Lane.Center ? Lane.Right : Lane.Center;
            }
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            Debug.Log("Jump");
            if (location.row is Row.Span)
            {
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
            lastPole = pole;
            canTurn = true;
            return;
        }

        var obstacle = other.gameObject.GetComponent<Obstacle>();
        if (obstacle) {
            var position = location.TrackOccupation;
            if (obstacle.IsObstructiveTo(position)) {
                if (--lives >= 0)
                {
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
        if (!other.GetComponent<Pole>()) return;
        hasExitedStartingPose = true;
        canTurn = false;
    }
}

