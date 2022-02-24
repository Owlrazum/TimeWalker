using UnityEngine;

public class Pendulum : Timeable
{
    [SerializeField]
    private float halfAngle;

    private Vector3 startingEuler;
    private Vector3 endingEuler;

    protected override void Awake()
    {
        base.Awake();

        startingEuler = new Vector3(0, 0, -halfAngle);
        endingEuler = new Vector3(0, 0, halfAngle);
    }

    protected override float OnTimeStateChange(float timeState)
    {
        if (!shouldRespondToTimeChange)
        {
            return -1;
        }

        timeState = base.OnTimeStateChange(timeState);
        
        if (timeState < 0.5f)
        {
            timeState *= 2;
            transform.rotation = Quaternion.Slerp(
                Quaternion.Euler(startingEuler),
                Quaternion.Euler(endingEuler),
                CustomMath.EaseInOut(CustomMath.EaseInOut(timeState))
            );
        }
        else
        { 
            timeState = (timeState - 0.5f) * 2;
            transform.rotation = Quaternion.Slerp(
                Quaternion.Euler(endingEuler),
                Quaternion.Euler(startingEuler),
                CustomMath.EaseInOut(CustomMath.EaseInOut(timeState))
            );
        }

        return 0;
    }
}
