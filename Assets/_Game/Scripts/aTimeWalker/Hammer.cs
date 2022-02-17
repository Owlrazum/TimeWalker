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
    }

    protected override void OnTimeStateChange(float timeState)
    {
        if (!shouldRespondToTimeChange)
        {
            return;
        }

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

    protected override void OnAllTimeablesShouldDefault(float timeDefault)
    {
        base.OnAllTimeablesShouldDefault(timeDefault);
        StartCoroutine(ReturnToDefault(timeDefault));
    }

    private IEnumerator ReturnToDefault(float timeDefault)
    {
        shouldRespondToTimeChange = false;
        float lerpParam = 0;
        Vector3 prevEuler = transform.eulerAngles;
        while (lerpParam < 1)
        {
            lerpParam += Time.deltaTime / timeDefault;
            transform.eulerAngles = Vector3.Lerp(
                prevEuler,
                startingEuler,
                CustomMath.EaseOut(lerpParam)
            );
            yield return null;
        }
        shouldRespondToTimeChange = true;
    }
}