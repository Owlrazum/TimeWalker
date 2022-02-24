using System;

public static class EventsContainer
{
    // Custom events go here.
    public static Action PlayerCollidedWithDeath;

    public static Action RevertingTimeFlow;
    public static Action UsualTimeFlow;

    //public static Action PlayerStartedDecelerating;
    //public static Action PlayerRevertedToStartPos;

    //public static Action<float> AllTimeablesShouldDefault;

    public static Action ClockInputStart;
    public static Action<float> ClockInputUpdate;
    public static Action ClockInputEnd;

    // It is a func as an exception case because of inheritance.
    public static Func<float, float> ClockTimeChange;

    public static Action PlayerReachedGates;
}