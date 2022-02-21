using System;

public static class EventsContainer
{
    // Custom events go here.
    public static Action PlayerShouldStartMoving;
    public static Action PlayerCollidedWithDeath;

    public static Action<float> PlayerShouldStartReverting;

    //public static Action PlayerStartedDecelerating;
    //public static Action PlayerRevertedToStartPos;

    //public static Action<float> AllTimeablesShouldDefault;

    public static Action<float> ClockArrowMoved;
    // It is a func as an exception case because of inheritance.
    public static Func<float, float> TimeStateChange;
}