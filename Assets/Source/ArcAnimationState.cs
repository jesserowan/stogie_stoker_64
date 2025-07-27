using Source;
using UnityEngine;

namespace Source
{
public class ArcAnimationState : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var hub = animator.gameObject.GetComponent<PlayerAnimator>();
        if (hub) hub.completeAnimation.Invoke();
    }

}
}
