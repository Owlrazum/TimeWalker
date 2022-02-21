using System.Collections;
using UnityEngine;

public class SpikeTrap : Timeable
{
    [SerializeField]
    private float maxHeight;

    [SerializeField]
    [Tooltip("Used for the sin wave")]
    private float divisorForPI = 12;

    [SerializeField]
    [Tooltip("Should be the same and manually set")]
    private float distanceBetweenRows;

    [SerializeField]
    [Tooltip("Those less and greater will be affected")]
    private float affectRange;

    [SerializeField]
    [Tooltip("Uses its own hierarchy for the logic." +
            "Each child is assumed as the row of the spikes.")]
    private string readTooltip;

    private float totalRowWiseDistance;

    private float startHeight;

    private Transform[] spikeRows;

    protected override void Awake()
    {
        base.Awake();

        float deltaRange = 1.0f / transform.childCount;

        spikeRows = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            spikeRows[i] = transform.GetChild(i);
        }

        startHeight = spikeRows[0].transform.position.y;
        totalRowWiseDistance = spikeRows.Length * distanceBetweenRows;
        
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

        timeState *= 2;
        timeState -= 0.5f;

        for (int i = 0; i < spikeRows.Length; i++)
        {
            float distanceFromOrigin = i * distanceBetweenRows;
            float distParam = distanceFromOrigin / totalRowWiseDistance;
            //distParam = CustomMath.Flip(distParam);

            // Debug.Log(
            //     "DistParam " + distParam + "\n" +
            //     "TimeState " + timeState + "\n"
            // );
            //Debug.Break();
            float distTimeRelation = Mathf.Abs(distParam - timeState);
            if (distTimeRelation > affectRange)
            {
                continue;
            }

            distParam -= timeState - affectRange;
            float angleParam = distParam / (affectRange * 2);
            angleParam *= Mathf.PI / 2;

            float lerpParam = Mathf.Sin(angleParam);
            ChangeSpikeRowHeight(spikeRows[i], lerpParam);
        }

        return 0;
    }

    private void ChangeSpikeRowHeight(Transform spikeRow, float lerpParam)
    {
        float newHeight;
        if (lerpParam <= 0.5f)
        {
            lerpParam *= 2;
            newHeight = Mathf.Lerp(startHeight, maxHeight, lerpParam);
        }
        else
        {
            lerpParam = (lerpParam - 0.5f) * 2;
            newHeight = Mathf.Lerp(maxHeight, startHeight, lerpParam);
        }
        Vector3 pos = spikeRow.transform.position;
        pos.y = newHeight;
        spikeRow.transform.position = pos;
    }
}

 // protected override void OnTimeStateChange(float timeState)
    // {
    //     base.OnTimeStateChange(timeState);

    //     for (int i = 0; i < timeStateRange.Length; i++)
    //     {
    //         if (i == 0) 
    //         {
    //             if (timeState <= timeStateRange[0])
    //             {
    //                 float lerpParam = timeState / timeStateRange[0];
    //                 ChangeSpikeRowHeight(spikeRows[0], lerpParam);
    //                 ChangeSpikeRowHeight(spikeRows[1], lerpParam - offset);
    //             }
    //             continue;
    //         }
    //         if (timeState > timeStateRange[i - 1] && timeState <= timeStateRange[i])
    //         {
    //             float lerpParam = (timeState - timeStateRange[i - 1]) / (timeStateRange[i] - timeStateRange[i - 1]);
    //             ChangeSpikeRowHeight(spikeRows[i], lerpParam);
    //             ChangeSpikeRowHeight(spikeRows[i - 1], lerpParam - offset);
    //             if (i + 1 < timeStateRange.Length)
    //             {
    //                 //ChangeSpikeRowHeight(spikeRows[1], lerpParam - offset);
    //             }
    //         }
    //     }
    // }