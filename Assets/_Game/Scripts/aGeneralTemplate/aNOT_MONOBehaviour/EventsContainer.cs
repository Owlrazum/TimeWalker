using System;

public static class EventsContainer
{
    // Custom events go here.
    public static Action PlayerShouldStartMoving;
    public static Action PlayerCollidedWithDeath;
    public static Action PlayerStartedDecelerating;
    public static Action PlayerRevertedToStartPos;

    public static Action<float> AllTimeablesShouldDefault;

    public static Action<float> ClockArrowMoved;
    public static Action<float> TimeStateChange;
}