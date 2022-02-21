using System.Collections;
using UnityEngine;

public class HellBlade : Timeable
{
    [SerializeField]
    private float angularSpeed = 90;
    private float prevTimeState;

    private Vector3 initialEuler;

    protected override void Awake()
    {
        base.Awake();

        prevTimeState = timeOffset;
        initialEuler = transform.eulerAngles;

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

        float delta = timeState - prevTimeState;
        transform.eulerAngles += Vector3.forward * delta * angularSpeed;
        prevTimeState = timeState;

        return 0;
    }
}
