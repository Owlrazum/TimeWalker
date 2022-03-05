using UnityEngine;

public class UIClockCanvas : UIBaseFadingCanvas
{
    protected override void Awake()
    {
        base.Awake();

        GeneralEventsContainer.LevelStart += ShowItselfOnLevelStart;
        EventsContainer.PlayerReachedGates += HideItself;
    }

    private void OnDestroy()
    { 
        GeneralEventsContainer.LevelStart -= ShowItselfOnLevelStart;
        EventsContainer.PlayerReachedGates -= HideItself;
    }

    private void ShowItselfOnLevelStart(int notUsed)
    {
        ShowItself();
    }
}
