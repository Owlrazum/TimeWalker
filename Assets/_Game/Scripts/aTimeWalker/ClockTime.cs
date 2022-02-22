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
    private bool isPositive;

    public ClockTime()
    {
        isPositive = true;
    }

    public float GetTime()
    {
        if (isPositive)
        {
            return GetFractionPart();
        }
        else
        {
            return CustomMath.Flip(GetFractionPart());
        }
    }

    public void UpdateTimeWithDelta(float timeDelta)
    {
        time += timeDelta;
        if (time > 0)
        {
            isPositive = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>resulting delta after update</returns>
    public void UpdateWithAngleRad(float angleRad)
    {
        float angleTime = angleRad / (2 * Mathf.PI);
        if (isPositive)
        {
            PositiveUpdateWithAngleRad(angleTime);
        }
        else
        {
            NegativeUpdateWithAngleRad(angleTime);
        }
    }

    private void PositiveUpdateWithAngleRad(float angleTime)
    { 
        float fractionPart = GetFractionPart();
        float delta = angleTime - fractionPart;
//        Debug.Log("AngleTime " + angleTime + "FractionPart " + fractionPart + " Delta " + delta);
        time = GetRevolutionsCount() + angleTime;
        if (Mathf.Abs(delta) > QueriesContainer.QeuryMaxClockTimeInFrame())
        {
            if (delta < 0)
            {
                time++;
            }
            else if (GetRevolutionsCount() > 0 && delta > 0)
            {
                time--;
            }
            else
            { 
                time = -angleTime;

                Debug.Log("C2 " + isPositive);
                isPositive = false;
            }
            // Debug.Log("Changed revolutionsCount:" 
            //     + " AngleTime " + angleTime + "FractionPart " + fractionPart + " Delta " + delta);
        }
    }

    private void NegativeUpdateWithAngleRad(float angleTime)
    {
        angleTime = CustomMath.Flip(angleTime);
        float fractionPart = GetFractionPart();
        float delta = angleTime - fractionPart;
        time = GetRevolutionsCount() - angleTime;
        if (Mathf.Abs(delta) > QueriesContainer.QeuryMaxClockTimeInFrame())
        {
            if (delta < 0)
            {
                time--;
            }
            else if (GetRevolutionsCount() > 0 && delta > 0)
            {
                time++;
            }
            else
            { 
                time = angleTime;
                Debug.Log("C2 " + isPositive);
                isPositive = true;
            }
            //Debug.Log("Changed revolutionsCount:" 
            //    + " AngleTime " + angleTime + "FractionPart " + fractionPart + " Delta " + delta);
        }
    }

    public int GetRevolutionsCount()
    {
        return (int)(time);
    }

    private float GetFractionPart()
    { 
        return Mathf.Abs(time - (int)(time));
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
        isPositive = true;
    }
}