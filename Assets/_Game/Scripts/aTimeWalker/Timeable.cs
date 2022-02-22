using UnityEngine;

public class Timeable : MonoBehaviour
{
    [System.Serializable]
    protected enum PeriodOfFreePassageType
    {
        BeforePI,
        AfterPI
    }

    [SerializeField]
    protected PeriodOfFreePassageType periodOfFreePassage;

    protected float timeOffset;

    protected bool shouldRespondToTimeChange;

    protected virtual void Awake()
    {
        shouldRespondToTimeChange = true;

        EventsContainer.ClockTimeChange += OnTimeStateChange;
        //EventsContainer.AllTimeablesShouldDefault += OnAllTimeablesShouldDefault;

        if (periodOfFreePassage == PeriodOfFreePassageType.AfterPI)
        {
            timeOffset = 0.5f;
        }
    }

    protected virtual void OnDestroy()
    {
        EventsContainer.ClockTimeChange -= OnTimeStateChange;
        //EventsContainer.AllTimeablesShouldDefault -= OnAllTimeablesShouldDefault;
    }

    protected virtual float OnTimeStateChange(float timeState)
    { 
        timeState += timeOffset;
        if (timeState > 1)
        {
            timeState -= 1;
        }
        return timeState;
    }

    // protected virtual void OnAllTimeablesShouldDefault(float timeDefault)
    // {
    //     shouldRespondToTimeChange = false;
    // }
}