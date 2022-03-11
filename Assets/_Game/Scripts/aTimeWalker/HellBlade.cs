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
    [Min(3)]
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

    private enum NearestPoint
    { 
        Left,
        Right
    }

    protected override void Awake()
    {
        base.Awake();

        ranges = new List<TimeRange>();

        List<float> anchors = new List<float>();
        anchors = new List<float>();
        float anchorDelta = 1.0f / changesCount;
        for (int i = 1; i < changesCount; i++)
        {
            anchors.Add(i * anchorDelta);
        }

        #region Initializing timeRanges
        int rangeIndex = 0;

        currentTimeRange = null;
        bool initializedCurrentTimeRange = false;
        // anchor at i == 0 are processed by initial TimeRange;
        TimeRange initial = new TimeRange(1 - rangeLength, rangeLength);
        ranges.Add(initial);
        initial.Index = rangeIndex++;
        if (initial.GetParameterState(timeOffset) == TimeRange.ParameterState.Inside)
        {
            currentTimeRange = initial;
            initializedCurrentTimeRange = true;
        }
        
        TimeRange initialStatic = new TimeRange(rangeLength, anchors[0] - rangeLength);
        initialStatic.SetStatic(true);
        ranges.Add(initialStatic);
        initialStatic.Index = rangeIndex++;
        if (!initializedCurrentTimeRange)
        {
            if (initial.GetParameterState(timeOffset) == TimeRange.ParameterState.Inside)
            { 
                currentTimeRange = initialStatic;
                initializedCurrentTimeRange = true;
            }
        }

        for (int i = 0; i < anchors.Count - 1; i++)
        {
            TimeRange timeRange = new TimeRange(anchors[i] - rangeLength, anchors[i] + rangeLength);
            ranges.Add(timeRange);
            timeRange.Index = rangeIndex++;
            if (!initializedCurrentTimeRange)
            {
                if (initial.GetParameterState(timeOffset) == TimeRange.ParameterState.Inside)
                { 
                    currentTimeRange = timeRange;
                    initializedCurrentTimeRange = true;
                }
            }
            TimeRange staticRange = new TimeRange(anchors[i] + rangeLength, anchors[i + 1] - rangeLength);
            staticRange.SetStatic(true);
            staticRange.Index = rangeIndex++;
            ranges.Add(staticRange);
            if (!initializedCurrentTimeRange)
            {
                if (initial.GetParameterState(timeOffset) == TimeRange.ParameterState.Inside)
                { 
                    currentTimeRange = staticRange;
                    initializedCurrentTimeRange = true;
                }
            }
        }

        TimeRange endingRange = new TimeRange(
            anchors[changesCount - 2] - rangeLength, 
            anchors[changesCount - 2] + rangeLength
        );
        ranges.Add(endingRange);
        endingRange.Index = rangeIndex++;

        if (!initializedCurrentTimeRange)
        {
            if (initial.GetParameterState(timeOffset) == TimeRange.ParameterState.Inside)
            { 
                currentTimeRange = endingRange;
                initializedCurrentTimeRange = true;
            }
        }

        TimeRange endingStaticRange = new TimeRange(
            anchors[changesCount - 2] + rangeLength,
            1 - rangeLength
        );
        endingStaticRange.SetStatic(true);
        ranges.Add(endingStaticRange);
        endingStaticRange.Index = rangeIndex++;
        if (!initializedCurrentTimeRange)
        {
            currentTimeRange = endingStaticRange;
            initializedCurrentTimeRange = true;
        }
        #endregion
        
        //Offset if AfterPI;
        OnTimeStateChange(0);

        leftStartYaw = 0;
        rightStartYaw  = 0;

        leftEndYaw  = 1 * angleStepDeg;
        rightEndYaw = rightSign * angleStepDeg;
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
                        UpdateLerpRotations(Mathf.Sign(1));
                    }
                    currentTimeRange = ranges[GetNextIndex(currentTimeRange)];
                    isCurrentTimeRangeRelevant = true;
                    break;
                case TimeRange.ParameterState.OutsideToTheLeft:
                if (currentTimeRange.IsStatic())
                    { 
                        UpdateLerpRotations(Mathf.Sign(-1));
                    }
                    currentTimeRange = ranges[GetPrevIndex(currentTimeRange)];
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

    private void UpdateLerpRotations(float sign)
    {
        leftStartYaw = leftEndYaw;
        leftEndYaw += angleStepDeg * sign;

        rightStartYaw = rightEndYaw;
        rightEndYaw += angleStepDeg * -sign;
    }
}
