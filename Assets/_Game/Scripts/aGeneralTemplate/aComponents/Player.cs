using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : AnimatedPlayerCharacter
{
    [Header("Player")]
    [Space]
    [SerializeField]
    private float moveSpeed;

    private CharacterController characterController;
    private bool isMoving;

    protected override void Awake()
    {
        base.Awake();
        characterController = GetComponent<CharacterController>();

        EventsContainer.PlayerShouldStartMoving += StartMoving;
    }

    private void OnDestroy()
    { 
        EventsContainer.PlayerShouldStartMoving -= StartMoving;
    }

    private void StartMoving()
    {
        if (animationState != AnimationState.Running)
        {
            SetAnimationState(AnimationState.Running);
            StartCoroutine(Moving());
        }
        else
        { 
            Debug.LogError("Invalid animation state");
        }
    }

    private IEnumerator Moving()
    {
        isMoving = true;
        while (isMoving)
        {
            characterController.Move(moveSpeed * Time.deltaTime * transform.forward);
            yield return null;
        }
    }

    private void StopMoving()
    { 
        if (animationState != AnimationState.Idle)
        {
            SetAnimationState(AnimationState.Idle);
            isMoving = false;
        }
        else
        {
            Debug.LogError("Invalid animation state");
        }
    }
}

