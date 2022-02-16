using UnityEngine;

public class HellBlade : Timeable
{
    [SerializeField]
    private float angularSpeed = 90;
    private float prevTimeState;

    protected override void Awake()
    {
        base.Awake();

        prevTimeState = 0;
    }

    protected override void OnTimeStateChange(float timeState)
    {
        base.OnTimeStateChange(timeState);

        float delta = timeState - prevTimeState;
        transform.eulerAngles += Vector3.forward * delta * angularSpeed;
        prevTimeState = timeState;
    }
}
