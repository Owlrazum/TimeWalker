using UnityEngine;

public enum TimeState
{ 
    Forward,
    Frozen,
    Backward
}

public class TimeController : MonoBehaviour
{
    private TimeState timeState;

    private void Awake()
    {
        EventsContainer.ClockArrowMoved += OnClockArrowMoved;
    }

    private void OnDestroy()
    {
        EventsContainer.ClockArrowMoved -= OnClockArrowMoved;
    }

    private void OnClockArrowMoved(float angle)
    {
        float timeState = angle / (2 * Mathf.PI);
        EventsContainer.TimeStateChange?.Invoke(timeState);
    }
}