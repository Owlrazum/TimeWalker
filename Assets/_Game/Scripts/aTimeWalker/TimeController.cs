using System.Collections;
using UnityEngine;

public enum TimeFlowType
{ 
    None,
    Usual,
    PlayerControlled,
    Reverting
}

public class TimeController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("1 revolution is equal to the number 1")]
    private float speedOfUsualTimeFlow;

    [SerializeField]
    private float speedOfRevertingTimeFlow;

    [SerializeField]
    private float maxClockTimeChange;

    private TimeFlowType timeFlow;
    private IEnumerator timeFlowCoroutine;

    private ClockTime currentClockTime;

    private bool areTimeablesFinishedReverting;

    private void Awake()
    {
        currentClockTime = new ClockTime();

        EventsContainer.ClockInputStart  += OnClockInputStart;
        EventsContainer.ClockInputUpdate += OnClockInputUpdate;
        EventsContainer.ClockInputEnd    += OnClockInputEnd;

        EventsContainer.PlayerCollidedWithDeath += OnPlayerCollidedWithDeath;
        
        EventsContainer.PlayerTimeControllerSynced += OnPlayerTimeControllerSynced;

        QueriesContainer.RevertToUsualTimeRelation += GetRevertToUsualTimeRelation;
        QueriesContainer.MaxClockTimeChange += GetMaxClockTimeChange;

        QueriesContainer.AreTimeablesFinishedReverting += GetAreTimeablesFinishedReverting;

        SetTimeFlow(TimeFlowType.Usual);
    }

    private void OnDestroy()
    {
        EventsContainer.ClockInputStart  -= OnClockInputStart;
        EventsContainer.ClockInputUpdate -= OnClockInputUpdate;
        EventsContainer.ClockInputEnd    -= OnClockInputEnd;

        EventsContainer.PlayerCollidedWithDeath -= OnPlayerCollidedWithDeath;

        EventsContainer.PlayerTimeControllerSynced -= OnPlayerTimeControllerSynced;

        QueriesContainer.RevertToUsualTimeRelation -= GetRevertToUsualTimeRelation;
        QueriesContainer.MaxClockTimeChange -= GetMaxClockTimeChange;

        QueriesContainer.AreTimeablesFinishedReverting += GetAreTimeablesFinishedReverting;
    }

    private void OnClockInputStart()
    { 
        SetTimeFlow(TimeFlowType.PlayerControlled);
    }

    private void OnClockInputUpdate(float angleRad)
    {
        float inputClockTime = angleRad / (2 * Mathf.PI);

        currentClockTime.UpdateWithAngleRad(angleRad);
        EventsContainer.ClockTimeChange?.Invoke(currentClockTime.GetTime());
    }

    private void OnClockInputEnd()
    {
        SetTimeFlow(TimeFlowType.Usual);
    }

    private void OnPlayerCollidedWithDeath()
    {
        SetTimeFlow(TimeFlowType.Reverting);
    }

    private float GetRevertToUsualTimeRelation()
    {
        return speedOfRevertingTimeFlow / speedOfUsualTimeFlow;
    }

    private float GetMaxClockTimeChange()
    {
        return maxClockTimeChange;
    }

    private void SetTimeFlow(TimeFlowType newTimeFlow)
    {
        if (timeFlow == newTimeFlow)
        {
            return;
        }

        switch (newTimeFlow)
        { 
            case TimeFlowType.Usual:
                EventsContainer.UsualTimeFlow?.Invoke();
                if (timeFlowCoroutine != null)
                {
                    StopCoroutine(timeFlowCoroutine);
                }
                timeFlowCoroutine = UsualFlowOfTime();
                StartCoroutine(timeFlowCoroutine);
                break;
            case TimeFlowType.Reverting:
                EventsContainer.RevertingTimeFlow?.Invoke();
                if (timeFlowCoroutine != null)
                {
                    StopCoroutine(timeFlowCoroutine);
                }
                timeFlowCoroutine = RevertingTime();
                StartCoroutine(timeFlowCoroutine);
                break;
            case TimeFlowType.PlayerControlled:
                if (timeFlow == TimeFlowType.Reverting)
                {
                    Debug.LogError("Unexpected");
                }
                if (timeFlowCoroutine != null)
                {
                    StopCoroutine(timeFlowCoroutine);
                }
                timeFlowCoroutine = null;
                break;
        }
        timeFlow = newTimeFlow;
    }   

    private IEnumerator UsualFlowOfTime()
    {
        while (true)
        { 
            float step = speedOfUsualTimeFlow * Time.deltaTime;
            currentClockTime.UpdateTimeWithDelta(step);
            EventsContainer.ClockTimeChange?.Invoke(currentClockTime.GetTime());
            yield return null;
        }
    }

    private IEnumerator RevertingTime()
    {
        AnimationCurve revertingAnimation = currentClockTime.GetRevertingAnimation();
        float revertTime = revertingAnimation.keys[revertingAnimation.length - 1].time;
        float ratioToUsualSpeed = GetRevertToUsualTimeRelation();
        while (revertTime > 0)
        {
            float revertClockTime = revertingAnimation.Evaluate(revertTime);
            EventsContainer.ClockTimeChange?.Invoke(revertClockTime);
            revertTime -= Time.deltaTime * ratioToUsualSpeed;
            yield return null;
        }
        currentClockTime.Reset();
        EventsContainer.ClockTimeChange?.Invoke(currentClockTime.GetTime());

        areTimeablesFinishedReverting = true;
        while (!QueriesContainer.QueryIsPlayerFinishedReverting())
        {
            yield return null;
        }
        if (!areTimeablesFinishedReverting)
        {
            // Already processed event
            yield break;
        }
        EventsContainer.PlayerTimeControllerSynced?.Invoke();
    }

    private void OnPlayerTimeControllerSynced()
    {
        areTimeablesFinishedReverting = false;
        EventsContainer.RevertingTimeFlowFinished?.Invoke();
        SetTimeFlow(TimeFlowType.Usual);
    }

    private bool GetAreTimeablesFinishedReverting()
    {
        return areTimeablesFinishedReverting;
    }
}