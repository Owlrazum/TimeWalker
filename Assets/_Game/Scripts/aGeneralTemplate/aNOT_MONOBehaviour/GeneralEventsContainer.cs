using System;
using GeneralTemplate;

public static class GeneralEventsContainer
{
    public static Action Initialization;
    public static Action GameStart;
    public static Action GameEnd;

    public static Action<LevelData> LevelLoaded;
    
    public static Action ShouldLoadNextScene;
    public static Action LevelFailed;
    public static Action LevelCompleted;

    #region InputCommands
    public static Action<InputCommand> InputCommanded;
    public static Action<TouchCommand> TouchCommanded;
    public static Action<JoystickCommand> JoystickCommanded;
    public static Action<DragCommand> DragCommanded;
    #endregion
    

    public static Action ProgressWasMade;
}