using UnityEngine;

public class Hammer : Timeable
{
    [SerializeField]
    private float timeOffset;

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
    }

    protected override void OnTimeStateChange(float timeState)
    {
        if (timeState < 0.5f)
        {
            timeState *= 2;
            transform.eulerAngles = Vector3.Lerp(
                startingEuler,
                endingEuler,
                timeState
            );
        }
        else
        { 
            timeState = (timeState - 0.5f) * 2;
            transform.eulerAngles = Vector3.Lerp(
                endingEuler, 
                startingEuler, 
                CustomMath.EaseOut(timeState)
            );
        }
    }
}