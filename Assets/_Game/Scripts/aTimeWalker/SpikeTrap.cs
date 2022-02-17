using System.Collections;
using UnityEngine;

public class SpikeTrap : Timeable
{
    [SerializeField]
    private float maxHeight;

    [SerializeField]
    private float offset;

    [SerializeField]
    [Tooltip("Uses its own hierarchy for the logic. Each child is assumed as the row of the spikes.")]
    private string readTooltip;

    private float startHeight;

    private Transform[] spikeRows;
    private float[] timeStateRange;

    protected override void Awake()
    {
        base.Awake();

        float deltaRange = 1.0f / transform.childCount;

        spikeRows = new Transform[transform.childCount];
        timeStateRange = new float[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            spikeRows[i] = transform.GetChild(i);
            timeStateRange[i] = (i + 1) * deltaRange;
        }

        startHeight = spikeRows[0].transform.position.y;
    }

    protected override void OnTimeStateChange(float timeState)
    {
        if (!shouldRespondToTimeChange)
        {
            return;
        }

        timeState *= 2 * Mathf.PI;
        float delta = Mathf.PI / 12;

        for (int i = 0; i < spikeRows.Length; i++)
        {
            float lerpParam = Mathf.Sin(timeState) + Mathf.Sin(delta) * i;
            ChangeSpikeRowHeight(spikeRows[i], lerpParam);
        }
    }

    protected override void OnAllTimeablesShouldDefault(float timeDefault)
    {
        base.OnAllTimeablesShouldDefault(timeDefault);

        foreach (Transform spikeRow in spikeRows)
        { 
            Vector3 prevPos = spikeRow.transform.position;
            if (prevPos.y == startHeight)
            {
                continue;
            }

            StartCoroutine(ReturnToDefaultSpikeRow(timeDefault, spikeRow));
        }
    }

    private IEnumerator ReturnToDefaultSpikeRow(float timeDefault, Transform spikeRow)
    { 
        shouldRespondToTimeChange = false;
        float lerpParam = 0;
        Vector3 prevPos = spikeRow.position;
        Vector3 startPos = spikeRow.position;
        startPos.y = startHeight;
        while (lerpParam < 1)
        {
            lerpParam += Time.deltaTime / timeDefault;
            spikeRow.position = Vector3.Lerp(
                prevPos,
                startPos,
                CustomMath.EaseOut(lerpParam)
            );
            yield return null;
        }
        shouldRespondToTimeChange = true;
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