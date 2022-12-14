using System;

using UnityEngine;

public static class QueriesContainer
{
    public static Func<int> ScenesCount;
    public static int QuerySceneCount()
    { 
#if UNITY_EDITOR
        if (ScenesCount.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif        

        return ScenesCount.Invoke();
    }

    public static Func<bool> AreAllLevelsPassed;
    public static bool QueryAreAllLevelsPassed()
    { 
#if UNITY_EDITOR
        if (AreAllLevelsPassed.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif        

        return AreAllLevelsPassed.Invoke();
    }

    public static Func<LevelData> LevelData;
    public static LevelData QueryLevelData()
    {
#if UNITY_EDITOR
        if (LevelData.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif        

        return LevelData.Invoke();
    }

    public static Func<bool> ShouldShowTutorial;
    public static bool QueryShouldShowTutorial()
    {
#if UNITY_EDITOR
        if (ShouldShowTutorial.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif        

        return ShouldShowTutorial.Invoke();
    }

    public static Func<float> CurrentCameraYaw;
    public static float QueryCurrentCameraYaw()
    { 
#if UNITY_EDITOR
        if (CurrentCameraYaw.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return CurrentCameraYaw.Invoke();
    }

    public static Func<Vector3, Ray> CameraScreenPointToRay;
    public static Ray QueryCameraScreenPointToRay(Vector3 screenPos)
    { 
#if UNITY_EDITOR
        if (CameraScreenPointToRay.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return CameraScreenPointToRay.Invoke(screenPos);
    }

    public static Func<Transform> CurrentPlayerTransform;
    public static Transform QueryCurrentPlayerTranfsorm()
    {
#if UNITY_EDITOR
        if (CurrentPlayerTransform.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif        

        return CurrentPlayerTransform.Invoke();
    }

    public static Func<float> PlayerMoveSpeed;
    public static float QueryPlayerMoveSpeed()
    {
#if UNITY_EDITOR
        if (PlayerMoveSpeed.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif        

        return PlayerMoveSpeed.Invoke();
    }

    public static Func<float> RevertToUsualTimeRelation;
    public static float QueryRevertToUsualTimeRelation()
    { 
#if UNITY_EDITOR
        if (RevertToUsualTimeRelation.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif        

        return RevertToUsualTimeRelation.Invoke();
    }

    public static Func<float> MaxClockTimeChange;
    public static float QeuryMaxClockTimeInFrame()
    { 
#if UNITY_EDITOR
        if (MaxClockTimeChange.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif        

        return MaxClockTimeChange.Invoke();
    }

    public static Func<bool> AreTimeablesFinishedReverting;
    public static bool QueryAreTimeablesFinishedReverting()
    { 
#if UNITY_EDITOR
        if (AreTimeablesFinishedReverting.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif        

        return AreTimeablesFinishedReverting.Invoke();
    }

    public static Func<bool> IsPlayerFinishedReverting;
    public static bool QueryIsPlayerFinishedReverting()
    { 
#if UNITY_EDITOR
        if (IsPlayerFinishedReverting.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif        

        return IsPlayerFinishedReverting.Invoke();
    }
}
