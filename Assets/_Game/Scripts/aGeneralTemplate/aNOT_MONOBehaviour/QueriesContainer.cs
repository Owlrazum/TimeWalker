using System;

using UnityEngine;

public static class QueriesContainer
{
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
}
