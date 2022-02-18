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

        prevTimeState = 0;
        initialEuler = transform.eulerAngles;
    }

    protected override void OnTimeStateChange(float timeState)
    {
        if (!shouldRespondToTimeChange)
        {
            return;
        }

        float delta = timeState - prevTimeState;
        transform.eulerAngles += Vector3.forward * delta * angularSpeed;
        prevTimeState = timeState;
    }

    // protected override void OnAllTimeablesShouldDefault(float timeDefault)
    // {
    //     base.OnAllTimeablesShouldDefault(timeDefault);
    //     print("HellBlade default");
    //     StartCoroutine(ReturnToDefault(timeDefault));
    // }

    // private IEnumerator ReturnToDefault(float timeDefault)
    // { 
    //     shouldRespondToTimeChange = false;
    //     float lerpParam = 0;
    //     Vector3 prevEuler = transform.eulerAngles;
    //     while (lerpParam < 1)
    //     {
    //         lerpParam += Time.deltaTime / timeDefault;
    //         transform.eulerAngles = Vector3.Lerp(
    //             prevEuler,
    //             initialEuler,
    //             CustomMath.EaseOut(lerpParam)
    //         );
    //         yield return null;
    //     }
    //     shouldRespondToTimeChange = true;
    // }
}
