using UnityEngine;

public class Timeable : MonoBehaviour
{
    protected virtual void Awake()
    {
        EventsContainer.TimeStateChange += OnTimeStateChange;
    }

    protected virtual void OnDestroy()
    {
        EventsContainer.TimeStateChange -= OnTimeStateChange;
    }

    protected virtual void OnTimeStateChange(float newTimeStateChange)
    { 

    }
}