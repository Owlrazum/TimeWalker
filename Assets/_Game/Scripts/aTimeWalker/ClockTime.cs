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
    private float clockTime;
    private bool isPositive;

    private AnimationCurve revertingAnimation;
    private float startTime;
    private float currentTime;
    private Keyframe keyFrame;

    public ClockTime()
    {
        isPositive = true;
        revertingAnimation = new AnimationCurve();
        keyFrame = new Keyframe(0, 0);
        revertingAnimation.AddKey(keyFrame);
        startTime = Time.time;
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

    public AnimationCurve GetRevertingAnimation()
    {
        return revertingAnimation;
    }

    private void RecordClockTime()
    { 
        keyFrame.time = Time.time - startTime;
        keyFrame.value = GetTime();
        revertingAnimation.AddKey(keyFrame);
    }

    public void UpdateTimeWithDelta(float timeDelta)
    {
        clockTime += timeDelta;
        RecordClockTime();
        if (clockTime > 0)
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
        RecordClockTime();
    }

    private void PositiveUpdateWithAngleRad(float angleTime)
    { 
        float fractionPart = GetFractionPart();
        float delta = angleTime - fractionPart;
//        Debug.Log("AngleTime " + angleTime + "FractionPart " + fractionPart + " Delta " + delta);
        clockTime = GetRevolutionsCount() + angleTime;
        if (Mathf.Abs(delta) > QueriesContainer.QeuryMaxClockTimeInFrame())
        {
            if (delta < 0)
            {
                clockTime++;
            }
            else if (GetRevolutionsCount() > 0 && delta > 0)
            {
                clockTime--;
            }
            else
            { 
                clockTime = -angleTime;

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
        clockTime = GetRevolutionsCount() - angleTime;
        if (Mathf.Abs(delta) > QueriesContainer.QeuryMaxClockTimeInFrame())
        {
            if (delta < 0)
            {
                clockTime--;
            }
            else if (GetRevolutionsCount() > 0 && delta > 0)
            {
                clockTime++;
            }
            else
            { 
                clockTime = angleTime;
                Debug.Log("C2 " + isPositive);
                isPositive = true;
            }
            //Debug.Log("Changed revolutionsCount:" 
            //    + " AngleTime " + angleTime + "FractionPart " + fractionPart + " Delta " + delta);
        }
    }

    public int GetRevolutionsCount()
    {
        return (int)(clockTime);
    }

    private float GetFractionPart()
    { 
        return Mathf.Abs(clockTime - (int)(clockTime));
    }

    public Func<bool> GetCheckForRevertCompletion()
    {
        if (clockTime > 0)
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
        return clockTime >= 0;
    }

    private bool IsTimeNegative()
    {
        return clockTime <= 0;
    }

    public float UpdateDeltaForReverting(float delta)
    {
        if (clockTime > 0)
        {
            return -delta;
        }
        return delta;
    }

    public void Reset()
    {
        clockTime = 0;
        isPositive = true;
        ResetRevertingAnimation();
    }

    private void ResetRevertingAnimation()
    { 
        startTime = Time.time;
        keyFrame = new Keyframe(0, 0);
        revertingAnimation = new AnimationCurve();
        revertingAnimation.AddKey(keyFrame);
    }
}