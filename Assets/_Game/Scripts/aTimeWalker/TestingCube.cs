using UnityEngine;
public class TestingCube : Timeable
{
    [SerializeField]
    private Transform startingTransform;

    [SerializeField]
    private Transform endingTransform;

    private Vector3 startingPosition;
    private Vector3 endingPosition;


    protected override void Awake()
    {
        base.Awake();
        startingPosition = startingTransform.position;
        endingPosition = endingTransform.position;
    }

    protected override float OnTimeStateChange(float timeState)
    {
        timeState = base.OnTimeStateChange(timeState);
        if (timeState <= 0.5f)
        {
            timeState *= 2;
            transform.position = Vector3.Lerp(
                startingPosition, 
                endingPosition, 
                CustomMath.EaseInOut(timeState)
            );
        }
        else
        { 
            timeState = (timeState - 0.5f) * 2;
            transform.position = Vector3.Lerp(
                endingPosition, 
                startingPosition, 
                CustomMath.EaseInOut(timeState)
            );
        }

        return timeState;
    }
}