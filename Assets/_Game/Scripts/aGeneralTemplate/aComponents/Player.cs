using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : AnimatedPlayerCharacter
{
    [Header("Player")]
    [Space]
    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private SingleUnityLayer deathLayer;

    private CharacterController characterController;
    private Vector3 posOfMaxSpeed;

    protected override void Awake()
    {
        base.Awake();
        characterController = GetComponent<CharacterController>();

        EventsContainer.PlayerShouldStartMoving += StartMoving;
        EventsContainer.PlayerShouldStartMoving?.Invoke();

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

    private bool isMoving;
    private IEnumerator Moving()
    {
        isMoving = true;
        float currentMoveSpeed = 0;
        bool shouldAccelerate = true;
        while (isMoving)
        {
            if (shouldAccelerate)
            { 
                currentMoveSpeed += moveSpeed * Time.deltaTime;
                if (currentMoveSpeed >= moveSpeed)
                {
                    shouldAccelerate = false;
                    currentMoveSpeed = moveSpeed;
                    posOfMaxSpeed = transform.position;
                }
            }
            characterController.Move(currentMoveSpeed * Time.deltaTime * transform.forward);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == deathLayer.LayerIndex)
        {
            EventsContainer.PlayerCollidedWithDeath?.Invoke();
            StartReverseMoving();
        }
    }

    private void StartReverseMoving()
    {
        isMoving = false;
        SetAnimationState(AnimationState.ReverseRunning);
        StartCoroutine(ReverseMoving());
    }

    private bool isReverseMoving;
    private IEnumerator ReverseMoving()
    {
        isReverseMoving = true;
        float currentMoveSpeed = moveSpeed;
        bool isDeleratingEventRaised = false;
        while (isReverseMoving)
        { 
            if (transform.position.z <= posOfMaxSpeed.z)
            {
                if (!isDeleratingEventRaised)
                {
                    EventsContainer.PlayerStartedDecelerating?.Invoke();
                    isDeleratingEventRaised = true;
                }
                currentMoveSpeed -= moveSpeed * Time.deltaTime;
                if (currentMoveSpeed <= 0)
                {
                    currentMoveSpeed = 0;
                    SetAnimationState(AnimationState.Idle);
                }
            }
            characterController.Move(currentMoveSpeed * Time.deltaTime * -transform.forward);
            yield return null;
        }
    }
}

