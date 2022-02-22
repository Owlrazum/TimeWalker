using System.Collections;
using UnityEngine;

public enum TimeFlowType
{ 
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

    private TimeFlowType timeFlow;
    private IEnumerator timeFlowCoroutine;

    private ClockTime currentClockTime;

    private void Awake()
    {
        currentClockTime = new ClockTime();

        EventsContainer.ClockInputStart  += OnClockInputStart;
        EventsContainer.ClockInputUpdate += OnClockInputUpdate;
        EventsContainer.ClockInputEnd    += OnClockInputEnd;

        EventsContainer.PlayerCollidedWithDeath += OnPlayerCollidedWithDeath;

        QueriesContainer.RevertToUsualTimeRelation += GetRevertToUsualTimeRelation;

        SetTimeFlow(TimeFlowType.Usual);
    }

    private void OnDestroy()
    {
        EventsContainer.ClockInputStart  -= OnClockInputStart;
        EventsContainer.ClockInputUpdate -= OnClockInputUpdate;
        EventsContainer.ClockInputEnd    -= OnClockInputEnd;

        EventsContainer.PlayerCollidedWithDeath -= OnPlayerCollidedWithDeath;

        QueriesContainer.RevertToUsualTimeRelation -= GetRevertToUsualTimeRelation;
    }

    private void OnClockInputStart()
    { 
        SetTimeFlow(TimeFlowType.PlayerControlled);
    }

    private void OnClockInputUpdate(float angleRad)
    {
        float inputClockTime = angleRad / (2 * Mathf.PI);
        //print("ClockTime " + currentClockTime.GetFractionPart() + "\n" + "AngleRad " + angleRad);

        currentClockTime.UpdateWithAngleRad(angleRad);
        //print("-=-=-=ClockTime " + currentClockTime.GetFractionPart() + "\n" + "AngleRad " + angleRad);
        EventsContainer.ClockTimeChange?.Invoke(currentClockTime.GetFractionPart());
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
            //currentClockTime.UpdateTimeWithDelta(step);
            //EventsContainer.ClockTimeChange?.Invoke(currentClockTime.GetFractionPart());
            yield return null;
        }
    }

    private IEnumerator RevertingTime()
    {
        var checkRevertCompletion = currentClockTime.GetCheckForRevertCompletion();
        print(checkRevertCompletion.GetInvocationList()[0].Method.Name);
        //Debug.Break();
        while (!checkRevertCompletion.Invoke())
        {
            float delta = speedOfRevertingTimeFlow * Time.deltaTime;
            delta = currentClockTime.UpdateDeltaForReverting(delta);
            Debug.Log("Delta " + delta + " PrevTime " + currentClockTime.GetFractionPart());
            currentClockTime.UpdateTimeWithDelta(delta);
            Debug.Log("Time " + currentClockTime.GetFractionPart());
            EventsContainer.ClockTimeChange?.Invoke(currentClockTime.GetFractionPart());
            yield return null;
        }
        currentClockTime.Reset();
        EventsContainer.ClockTimeChange?.Invoke(currentClockTime.GetFractionPart());
        SetTimeFlow(TimeFlowType.Usual);
    }
}