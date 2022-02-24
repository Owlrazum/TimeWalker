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
    private bool editorShouldMove;

    [SerializeField]
    private SingleUnityLayer deathLayer;

    private CharacterController characterController;
    private Vector3 posOfMaxSpeed;
    private Vector3 initialPos;
    
    private bool shouldDeathEventRaise;

    private float currentMoveSpeed;
    private IEnumerator movingCoroutine;

    protected override void Awake()
    {
        base.Awake();
        characterController = GetComponent<CharacterController>();

        GeneralEventsContainer.LevelStart += StartMoving;

        EventsContainer.RevertingTimeFlow += OnRevertingTimeFlow;

        EventsContainer.PlayerReachedGates += OnReachedGates;

        QueriesContainer.PlayerMoveSpeed += GetMoveSpeed;


        initialPos = transform.position;
    }

    private void OnDestroy()
    {
        GeneralEventsContainer.LevelStart -= StartMoving;
        
        EventsContainer.RevertingTimeFlow -= OnRevertingTimeFlow;

        EventsContainer.PlayerReachedGates -= OnReachedGates;

        QueriesContainer.PlayerMoveSpeed -= GetMoveSpeed;
    }

    private void StartMoving()
    {
        if (animationState != AnimationState.Running)
        {
            shouldDeathEventRaise = true;
            movingCoroutine = Moving();
            StartCoroutine(movingCoroutine);
        }
        else
        {
            Debug.LogError("Invalid animation state");
        }
    }

    private IEnumerator Moving()
    {
        SetAnimationState(AnimationState.Running);
        currentMoveSpeed = 0;
        bool shouldAccelerate = true;

        while (true)
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
            if (editorShouldMove)
            {
                characterController.Move(currentMoveSpeed * Time.deltaTime * transform.forward);
            }
            yield return null;
        }
    }

    private float GetMoveSpeed()
    {
        return moveSpeed;
    }

    private void OnRevertingTimeFlow()
    {
        float ratioToUsualSpeed = QueriesContainer.QueryRevertToUsualTimeRelation();

        StopCoroutine(movingCoroutine);
        movingCoroutine = ReverseMoving(ratioToUsualSpeed);
        StartCoroutine(movingCoroutine);
    }

    private IEnumerator ReverseMoving(float ratioToUsualSpeed)
    {
        SetAnimationState(AnimationState.ReverseRunning);
        SetReverseRunningAnimationSpeed(-ratioToUsualSpeed);
        currentMoveSpeed = moveSpeed;
        while (true)
        {
            if (transform.position.z <= posOfMaxSpeed.z)
            {
                SetAnimationState(AnimationState.Idle);
                currentMoveSpeed -= moveSpeed * ratioToUsualSpeed * Time.deltaTime;
                if (currentMoveSpeed <= 0)
                {
                    currentMoveSpeed = 0;
                    StartMoving();
                    yield break;
                }
            }
            characterController.Move(currentMoveSpeed * ratioToUsualSpeed * Time.deltaTime * -transform.forward);
            yield return null;
        }
    }

    private void OnReachedGates()
    {
        print("Reached Gates");
        StopCoroutine(movingCoroutine);
        movingCoroutine = StoppingMoveCoroutine();
        StartCoroutine(movingCoroutine);
    }

    private IEnumerator StoppingMoveCoroutine()
    {
        print("It started");
        float time = 0;
        bool shouldDecelerate = true;
        shouldDeathEventRaise = false;
        while (true)
        {
            time += Time.deltaTime;
            if (shouldDecelerate)
            { 
                currentMoveSpeed -= moveSpeed / 2 * Time.deltaTime;
                if (currentMoveSpeed <= 0)
                {
                    shouldDecelerate = false;
                    currentMoveSpeed = 0;
                    SetAnimationState(AnimationState.Idle);
                    movingCoroutine = null;
                    yield break;
                }
            }
            if (editorShouldMove)
            { 
                characterController.Move(currentMoveSpeed * Time.deltaTime * transform.forward);
            }
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == deathLayer.LayerIndex)
        {
            if (shouldDeathEventRaise)
            {
                shouldDeathEventRaise = false;
                EventsContainer.PlayerCollidedWithDeath?.Invoke();
            }
        }
    }

}

