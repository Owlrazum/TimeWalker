using System.Collections.Generic;
using UnityEngine;

public class HellBlade : Timeable
{
    [SerializeField]
    private Transform leftBlades;

    [SerializeField]
    private Transform rightBlades;

    [SerializeField]
    [Range(-1, 1)]
    private int leftSign;

    [SerializeField]
    [Range(-1, 1)]
    private int rightSign;

    [SerializeField]
    private float angleStepDeg;

    [SerializeField]
    private int changesCount;

    [SerializeField]
    [Tooltip("Depends on changesCount")]
    [ReadOnly]
    private float maxRange;

    [SerializeField]
    [Tooltip("Range of time loop which also determines the speed of change")]
    private float rangeLength;

    /// <summary>
    /// minArg and maxArg should be normalized to [0, 1], not clamped
    /// </summary>
    [System.Serializable]
    private class TimeRange
    {
        public float min { get; private set; }
        public float max { get; private set; }

        public float minEd;
        public float maxEd;

        public TimeRange(float minArg, float maxArg)
        {
            min = minArg;
            max = maxArg;

            minEd = minArg;
            maxEd = maxArg;
        }

        public int Index { get; set; }

        public bool isStatic;

        public bool IsStatic()
        {
            return isStatic;
        }

        public void SetStatic(bool newValue)
        {
            isStatic = newValue;
        }

        public enum ParameterState
        { 
            Inside,
            OutsideToTheLeft,
            OutsideToTheRight
        }

        public ParameterState GetParameterState(float time)
        {
            if (min > max)
            {
                if (time >= min || time <= max)
                {
                    return ParameterState.Inside;
                }
                else
                { 
                    float minDelta = Mathf.Abs(time - min);
                    float maxDelta = Mathf.Abs(time - max);
                    if (minDelta < maxDelta)
                    {
                        return ParameterState.OutsideToTheLeft;
                    }
                    else
                    {
                        return ParameterState.OutsideToTheRight;
                    }
                }
            }
            if (min <= time && time <= max)
            {
                return ParameterState.Inside;
            }
            if (time > max)
            {
                return ParameterState.OutsideToTheRight;
            }
            else
            {
                return ParameterState.OutsideToTheLeft;
            }
        }

        public float GetNearest(float time)
        { 
            float minDelta = Mathf.Abs(time - min);
            float maxDelta = Mathf.Abs(time - max);
            if (minDelta < maxDelta)
            {
                return min;
            }
            else
            {
                return max;
            }
        }

        /// <summary>
        /// Should be called only if ParameterState is inside and if not static; 
        /// </summary>
        /// <returns></returns>
        public float GetLerpParam(float time)
        {
            if (isStatic)
            {
                Debug.LogError("GetLerpParam should not be called is static timeRange");
                return -1;
            }
            if (time < min || time > max)
            {
                if (min < max)
                {
                    Debug.LogError("Not in proper time range");
                }
            }
            if (min > max)
            {
                float dist = max + 1 - min;
                if (time > min)
                {
                    time = time - min;
                }
                else
                {
                    time += 1 - min;
                }
                float toReturn = time / dist;
                return toReturn;
            }
            return Mathf.InverseLerp(min, max, time);
        }
    }

    [SerializeField]
    private List<TimeRange> ranges;

    private TimeRange currentTimeRange;

    private float leftStartYaw;
    private float leftEndYaw;

    private float rightStartYaw;
    private float rightEndYaw;

    protected override void Awake()
    {
        base.Awake();

        InitalizeTimeRanges(changesCount);

        //Offset if AfterPI;
        OnTimeStateChange(0);

        leftStartYaw = 0;
        rightStartYaw  = 0;

        leftEndYaw  = 1 * angleStepDeg;
        rightEndYaw = rightSign * angleStepDeg;
    }

    private void InitalizeTimeRanges(int anchorsAmount)
    { 
        ranges = new List<TimeRange>();
        List<float> anchors = new List<float>();
        float anchorDelta = 1.0f / anchorsAmount;
        maxRange = anchorDelta / 2;
        for (int i = 0; i < anchorsAmount; i++)
        {
            anchors.Add(i * anchorDelta + anchorDelta / 2);
        }

        int rangeIndex = 0;

        currentTimeRange = null;
        for (int i = 0; i < anchors.Count; i++)
        {
            TimeRange timeRange = new TimeRange(anchors[i] - rangeLength, anchors[i] + rangeLength);
            timeRange.Index = rangeIndex++;
            ranges.Add(timeRange);
            if (currentTimeRange == null)
            {
                if (timeRange.GetParameterState(timeOffset) == TimeRange.ParameterState.Inside)
                { 
                    currentTimeRange = timeRange;
                }
            }
            int nextAnchorIndex = i + 1;
            if (nextAnchorIndex >= anchorsAmount)
            {
                nextAnchorIndex = 0;
            }
            TimeRange staticRange = new TimeRange(anchors[i] + rangeLength, anchors[nextAnchorIndex] - rangeLength);
            staticRange.SetStatic(true);
            staticRange.Index = rangeIndex++;
            ranges.Add(staticRange);
            if (currentTimeRange == null)
            {
                if (staticRange.GetParameterState(timeOffset) == TimeRange.ParameterState.Inside)
                { 
                    currentTimeRange = staticRange;
                }
            }
        }
    }

    protected override float OnTimeStateChange(float timeState)
    {
        if (!shouldRespondToTimeChange)
        {
            return -1;
        }

        timeState = base.OnTimeStateChange(timeState);
        
        bool isCurrentTimeRangeRelevant = false;
        int iterationsLimit = 1000;
        int safePillow = 0;
        while (!isCurrentTimeRangeRelevant && safePillow < iterationsLimit)
        { 
            TimeRange.ParameterState parameterState = currentTimeRange.GetParameterState(timeState);
            switch (parameterState)
            { 
                case TimeRange.ParameterState.Inside:
                    isCurrentTimeRangeRelevant = true;
                    break;
                case TimeRange.ParameterState.OutsideToTheRight:
                    if (currentTimeRange.IsStatic())
                    {
                        UpdateLerpRotations(Side.ToTheRight);
                    }
                    currentTimeRange = ranges[GetNextIndex(currentTimeRange)];
                    if (currentTimeRange.IsStatic())
                    { 
                        leftBlades.rotation  = Quaternion.Euler(Vector3.up * leftEndYaw); 
                        rightBlades.rotation = Quaternion.Euler(Vector3.up * rightEndYaw);
                    }
                    isCurrentTimeRangeRelevant = true;
                    break;
                case TimeRange.ParameterState.OutsideToTheLeft:
                    if (currentTimeRange.IsStatic())
                    { 
                        UpdateLerpRotations(Side.ToTheLeft);
                    }
                    currentTimeRange = ranges[GetPrevIndex(currentTimeRange)];
                    if (currentTimeRange.IsStatic())
                    {
                        leftBlades.rotation  = Quaternion.Euler(Vector3.up * leftStartYaw); 
                        rightBlades.rotation = Quaternion.Euler(Vector3.up * rightStartYaw);
                    } 
                    isCurrentTimeRangeRelevant = true;

                    break;
            }
            safePillow++;
        }

        if (!currentTimeRange.IsStatic())
        {
            float lerpParam = currentTimeRange.GetLerpParam(timeState);
            float angle = Mathf.Lerp(leftStartYaw, leftEndYaw, lerpParam);
            leftBlades.rotation  = Quaternion.Euler(Vector3.up * angle);
            angle = Mathf.Lerp(rightStartYaw, rightEndYaw, lerpParam);
            rightBlades.rotation = Quaternion.Euler(Vector3.up * angle);
        }

        return 0;
    }

    private int GetNextIndex(TimeRange timeRange)
    {
        int nextIndex = timeRange.Index + 1;
        if (nextIndex >= ranges.Count)
        {
            nextIndex = 0;
        }
        return nextIndex;
    }

    private int GetPrevIndex(TimeRange timeRange)
    { 
        int prevIndex = timeRange.Index - 1;
        if (prevIndex < 0)
        {
            prevIndex = ranges.Count - 1;
        }
        return prevIndex;
    }

    private enum Side
    { 
        ToTheLeft,
        ToTheRight
    }

    private void UpdateLerpRotations(Side side)
    {
        if (side == Side.ToTheRight)
        { 
            leftStartYaw = leftEndYaw;
            leftEndYaw += angleStepDeg;

            rightStartYaw = rightEndYaw;
            rightEndYaw -= angleStepDeg;
        }
        else if (side == Side.ToTheLeft)
        {
            leftEndYaw = leftStartYaw;
            leftStartYaw -= angleStepDeg;

            rightEndYaw = rightStartYaw;
            rightStartYaw += angleStepDeg;
        }
    }
}
