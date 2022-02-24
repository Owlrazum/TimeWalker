using UnityEngine;

public class LevelController : MonoBehaviour
{
    private void Awake()
    {
        GeneralEventsContainer.LevelLoaded += OnLevelLoad;
        EventsContainer.PlayerReachedGates += CompleteLevel;
    }

    private void OnDestroy()
    { 
        GeneralEventsContainer.LevelLoaded -= OnLevelLoad;
        EventsContainer.PlayerReachedGates -= CompleteLevel;
    }

    private void OnLevelLoad(LevelData levelData)
    {
        GeneralEventsContainer.LevelStart?.Invoke();
    }

    private void CompleteLevel()
    {
        GeneralEventsContainer.LevelCompleted?.Invoke();
    }
}
