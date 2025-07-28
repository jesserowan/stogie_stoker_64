using UnityEngine.Events;
using UnityEngine;


public class PlayerAnimator: MonoBehaviour
{
    public readonly int Fall = Animator.StringToHash("Fall");
    public readonly int Idle = Animator.StringToHash("Idle");
    public readonly int Trip = Animator.StringToHash("Trip");
    public readonly int Win = Animator.StringToHash("Win");
    public readonly int Run = Animator.StringToHash("Run");
    public readonly int Jump = Animator.StringToHash("Jump");
    public readonly int Slide = Animator.StringToHash("Slide");
    public readonly int RunSmoking = Animator.StringToHash("Run/Smoking");
    public readonly int JumpSmoking = Animator.StringToHash("Jump/Smoking");
    public readonly int SlideSmoking = Animator.StringToHash("Slide/Smoking");

    public UnityEvent completeAnimation = new ();
    private Animator _animator;

    private void OnEnable() { _animator = GetComponent<Animator>(); }
    private void Start() { _animator.CrossFadeInFixedTime(Run, 0); }
    public void Play(int state, float blend = 0f) { _animator.CrossFadeInFixedTime(state, blend); }
}
