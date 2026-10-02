using UnityEngine;

public class FlipBehaviour : StateMachineBehaviour
{
    // Called when the Animator starts playing this state
    public override void OnStateEnter(Animator animator,
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Let the roll animation itself move the character forward
        animator.applyRootMotion = true;
    }

    // Called when the Animator finishes this state
    public override void OnStateExit(Animator animator,
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Hand movement back to the PlayerMovement script
        animator.applyRootMotion = false;
    }
}
