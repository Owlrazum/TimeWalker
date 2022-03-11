using System.Collections;
using UnityEngine;

public class BladeMill : Timeable
{
    [SerializeField]
    private float angularSpeed = 90;

    private float prevTimeState;

    protected override void Awake()
    {
        base.Awake();

        //Offset if AfterPI;
        prevTimeState = timeOffset;
        OnTimeStateChange(0);
    }

    protected override float OnTimeStateChange(float timeState)
    {
        if (!shouldRespondToTimeChange)
        {
            return -1;
        }

        timeState = base.OnTimeStateChange(timeState);

        float delta = timeState - prevTimeState;
        transform.eulerAngles += Vector3.forward * delta * angularSpeed;
        prevTimeState = timeState;

        return 0;
    }
}
