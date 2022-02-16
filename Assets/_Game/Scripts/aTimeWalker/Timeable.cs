using UnityEngine;

public class Timeable : MonoBehaviour
{
    protected float timeStateParameter;

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