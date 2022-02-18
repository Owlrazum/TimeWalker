using UnityEngine;

public class AnimatedPlayerCharacter : MonoBehaviour
{
    [Header("AnimatedDancerCharacter")]
    [Space]
    [Tooltip("The animator for this class should be the first child in the hiearchy")]
    [SerializeField]
    private string readToolTip;

    private Animator animator;

    protected enum AnimationState
    {
        Idle,
        Running,
        ReverseRunning
    }

    protected AnimationState animationState;

    protected virtual void Awake()
    {
        animationState = AnimationState.Idle;
        animator = transform.GetChild(0).GetComponent<Animator>();
        animator.applyRootMotion = false;
    }

    protected void SetAnimationState(AnimationState newState)
    {
        if (animationState == newState)
        {
            return;
        }
        switch (newState)
        { 
            case AnimationState.Idle:
                animator.SetInteger("AnimationState", 0);
                break;
            case AnimationState.Running:
                animator.SetInteger("AnimationState", 1);
                break;
            case AnimationState.ReverseRunning:
                print("ReverseRunning Animatoin state set");
                animator.SetInteger("AnimationState", 2);
                animator.applyRootMotion = false;
                break;
        }
        animationState = newState;
    }

    protected void SetReverseRunningAnimationSpeed(float speed)
    {
        animator.SetFloat("ReverseRunningSpeed", speed);
    }
}