using UnityEngine;

public class Timeable : MonoBehaviour
{
    protected bool shouldRespondToTimeChange;

    protected virtual void Awake()
    {
        shouldRespondToTimeChange = true;

        EventsContainer.TimeStateChange += OnTimeStateChange;
        EventsContainer.AllTimeablesShouldDefault += OnAllTimeablesShouldDefault;
    }

    protected virtual void OnDestroy()
    {
        EventsContainer.TimeStateChange -= OnTimeStateChange;
        EventsContainer.AllTimeablesShouldDefault -= OnAllTimeablesShouldDefault;
    }

    protected virtual void OnTimeStateChange(float newTimeStateChange)
    { 

    }

    protected virtual void OnAllTimeablesShouldDefault(float timeDefault)
    {
        shouldRespondToTimeChange = false;
    }
}