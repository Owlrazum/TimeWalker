using UnityEngine;

public class Guillotine : Timeable
{
    [SerializeField]
    private Transform blade;

    [Tooltip("The end at the default position of blade before PI")]
    [SerializeField]
    private Transform upEnd;

    [SerializeField]
    private Transform downEnd;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override float OnTimeStateChange(float timeState)
    {
        if (!shouldRespondToTimeChange)
        {
            return -1;
        }

        timeState = base.OnTimeStateChange(timeState);
        
        if (timeState < 0.2f)
        {
            timeState *= 5;
            blade.position = Vector3.Lerp(
                upEnd.position,
                downEnd.position,
                CustomMath.EaseIn(timeState)
            );
        }
        else if (timeState < 0.5f)
        { 
            timeState = (timeState - 0.2f) * (10 / 3.0f);
            transform.rotation = Quaternion.Slerp(
                Quaternion.identity,
                Quaternion.Euler(new Vector3(0, 0, 180)),
                CustomMath.EaseInOut(CustomMath.EaseInOut(timeState))
            );
            blade.position = downEnd.position;
        }
        else if (timeState < 0.7f)
        {
            timeState = (timeState - 0.5f) * 5;
            blade.position = Vector3.Lerp(
                downEnd.position,
                upEnd.position,
                CustomMath.EaseIn(timeState)
            );
        }
        else
        {
            timeState = (timeState - 0.7f) * (10 / 3.0f);
            transform.rotation = Quaternion.Slerp(
                Quaternion.Euler(new Vector3(0, 0, 180)),
                Quaternion.identity,
                CustomMath.EaseInOut(CustomMath.EaseInOut(timeState))
            );
            blade.position = upEnd.position;
        }
        return 0;
    }
}
