using System;
using UnityEngine;

/// <summary>
/// All clockTime is constructed as follows.
/// The integral part is the number of revolutions.
/// The fraction part is the normalized angle in [0...1] inside the revolution 
/// relative to the 2 * Mathf.PI
/// </summary>
public class ClockTime
{
    private float time;
    
    public ClockTime()
    { 
    }

    public void UpdateTimeWithDelta(float timeDelta)
    {
        time += timeDelta;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>resulting delta after update</returns>
    public void UpdateWithAngleRad(float angleRad)
    {
        float angleTime = angleRad / (2 * Mathf.PI);
        time = angleTime;
    }

    public int GetRevolutionsCount()
    {
        return (int)(time);
    }

    public float GetFractionPart()
    {
        return time - (int)(time);
    }

    public Func<bool> GetCheckForRevertCompletion()
    {
        if (time > 0)
        {
            return IsTimeNegative;
        }
        else
        { 
            return IsTimePositive;
        }
    }

    private bool IsTimePositive()
    {
        return time >= 0;
    }

    private bool IsTimeNegative()
    {
        return time <= 0;
    }

    public float UpdateDeltaForReverting(float delta)
    {
        if (time > 0)
        {
            return -delta;
        }
        return delta;
    }

    public void Reset()
    {
        time = 0;
    }
}