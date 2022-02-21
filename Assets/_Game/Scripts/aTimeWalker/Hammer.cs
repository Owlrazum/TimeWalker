using System.Collections;
using UnityEngine;

public class Hammer : Timeable
{
    [SerializeField]
    private float startingRotation;

    [SerializeField]
    private float endingRotation;

    private Vector3 startingEuler;
    private Vector3 endingEuler;

    protected override void Awake()
    {
        base.Awake();
        startingEuler = new Vector3(0, 0, startingRotation);
        endingEuler = new Vector3(0, 0, endingRotation);
        
        //Offset if AfterPI;
        OnTimeStateChange(0);
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
                timeState
            );
        }
        else
        { 
            timeState = (timeState - 0.5f) * 2;
            transform.rotation = Quaternion.Slerp(
                Quaternion.Euler(endingEuler),
                Quaternion.Euler(startingEuler),
                CustomMath.EaseOut(timeState)
            );
        }

        return 0;
    }
}