using UnityEngine;

public class UIClockCanvas : UIBaseFadingCanvas
{
    protected override void Awake()
    {
        base.Awake();

        GeneralEventsContainer.LevelStart += ShowItself;
        EventsContainer.PlayerReachedGates += HideItself;
    }

    private void OnDestroy()
    { 
        GeneralEventsContainer.LevelStart -= ShowItself;
        EventsContainer.PlayerReachedGates -= HideItself;
    }
}
