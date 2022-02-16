using System;

public static class EventsContainer
{
    // Custom events go here.
    public static Action PlayerShouldStartMoving;

    public static Action<float> ClockArrowMoved;
    public static Action<float> TimeStateChange;
}