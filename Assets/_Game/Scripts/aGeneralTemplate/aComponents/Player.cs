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
    private Vector3 initialPos;
    private bool isDeathEventRaised;

    protected override void Awake()
    {
        base.Awake();
        characterController = GetComponent<CharacterController>();

        EventsContainer.PlayerShouldStartMoving += StartMoving;
        EventsContainer.PlayerShouldStartMoving?.Invoke();

        EventsContainer.PlayerShouldStartReverting += StartReverting;

        QueriesContainer.PlayerMoveSpeed += GetMoveSpeed;


        initialPos = transform.position;
    }

    private void OnDestroy()
    { 
        EventsContainer.PlayerShouldStartMoving -= StartMoving;
        EventsContainer.PlayerShouldStartReverting -= StartReverting;
        
        QueriesContainer.PlayerMoveSpeed -= GetMoveSpeed;
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

    private float GetMoveSpeed()
    {
        return moveSpeed;
    }

    private bool isReverseMoving;
    private void StartReverting(float ratioToUsualSpeed)
    {
        isMoving = false;
        SetAnimationState(AnimationState.ReverseRunning);
        SetReverseRunningAnimationSpeed(-ratioToUsualSpeed);

        StartCoroutine(ReverseMoving(ratioToUsualSpeed));
    }

    private IEnumerator ReverseMoving(float ratioToUsualSpeed)
    {
        isReverseMoving = true;
        float currentMoveSpeed = moveSpeed;
        while (isReverseMoving)
        {
            if (transform.position.z <= posOfMaxSpeed.z)
            {
                SetAnimationState(AnimationState.Idle);
                //currentMoveSpeed += moveSpeed * Time.deltaTime;
                
                currentMoveSpeed -= moveSpeed * ratioToUsualSpeed * Time.deltaTime;
                if (currentMoveSpeed <= 0)
                {
                    currentMoveSpeed = 0;
                    isReverseMoving = false;
                    StartMoving();
                    //SetAnimationSpeed(1);
                }
            }
            //characterController.Move(currentMoveSpeed * Time.deltaTime * transform.forward);
            characterController.Move(currentMoveSpeed * ratioToUsualSpeed * Time.deltaTime * -transform.forward);
            yield return null;
        }
        isDeathEventRaised = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == deathLayer.LayerIndex)
        {
            if (!isDeathEventRaised)
            {
                isDeathEventRaised = true;
                EventsContainer.PlayerCollidedWithDeath?.Invoke();
            }
        }
    }
}

