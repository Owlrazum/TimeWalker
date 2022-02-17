using UnityEngine;

public enum TimeState
{ 
    Forward,
    Frozen,
    Backward
}

public class TimeController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Time to return to the default state")]
    private float timeDefault;
    private TimeState timeState;

    private void Awake()
    {
        EventsContainer.ClockArrowMoved += OnClockArrowMoved;
        EventsContainer.PlayerCollidedWithDeath += OnPlayerCollidedWithDeath;
    }

    private void OnDestroy()
    {
        EventsContainer.ClockArrowMoved -= OnClockArrowMoved;
        EventsContainer.PlayerCollidedWithDeath -= OnPlayerCollidedWithDeath;
    }

    private void OnClockArrowMoved(float angle)
    {
        float timeState = angle / (2 * Mathf.PI);
        EventsContainer.TimeStateChange?.Invoke(timeState);
    }

    private void OnPlayerCollidedWithDeath()
    {
        print("AllTimeablesShouldDefault");
        EventsContainer.AllTimeablesShouldDefault?.Invoke(timeDefault);
    }
}