using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// The arrow is a bit async with the player reverted to the pos.
/// </summary>
public class UIClockArrowController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField]
    private RectTransform arrowRectTransform;

    [SerializeField]
    private float speedOfUsualTimeDeg;

    [SerializeField]
    private float speedOfApproachRad;

    [SerializeField]
    private float speedOfRevertingDeg;

    [SerializeField]
    private float approachEpsilon;

    private RectTransform rect;

    private bool shouldTimePass;
    private bool shouldRespondToInput;

    private float currentAngleRad;
    private float totalAddedAngleDeg;

    private float angleToApproachRad;
    private bool haveApproached;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        shouldTimePass = true;
        shouldRespondToInput = true;

        currentAngleRad = 0;
        totalAddedAngleDeg = 0;

        EventsContainer.PlayerCollidedWithDeath += OnPlayerCollidedWithDeath;
    }

    private void OnDestroy()
    { 
        EventsContainer.PlayerCollidedWithDeath -= OnPlayerCollidedWithDeath;
    }

    private void OnPlayerCollidedWithDeath()
    {
        shouldTimePass = false;
        shouldRespondToInput = false;
        EventsContainer.PlayerShouldStartReverting(speedOfRevertingDeg / speedOfUsualTimeDeg);
        StartCoroutine(RevertingTime());
    }

    private void Update()
    {
        if (false)
        { 
            float stepDeg = speedOfUsualTimeDeg * Time.deltaTime;
            Vector3 toAddDeg = new Vector3(0, 0, -stepDeg);
            arrowRectTransform.eulerAngles += toAddDeg;
            totalAddedAngleDeg += stepDeg;

            currentAngleRad = arrowRectTransform.eulerAngles.z * Mathf.Deg2Rad;

            EventsContainer.ClockArrowMoved?.Invoke(arrowRectTransform.eulerAngles.z * Mathf.Deg2Rad);
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (!shouldRespondToInput)
        {
            return;
        }

        haveApproached = false;
        shouldTimePass = false;
        OnDrag(data);
    }

    public void OnDrag(PointerEventData data)
    {
        if (!shouldRespondToInput)
        {
            return;
        }

        float angleRad = ComputePressAngleRad(data.position);
        totalAddedAngleDeg += (angleRad - currentAngleRad) * Mathf.Rad2Deg;

        // if (!haveApproached)
        // {
        //     angleToApproachRad = angleRad;
        //     if (!isApproaching)
        //     {
        //         StartCoroutine(ApproachingCoroutine());
        //     }
        //     print("Returning");
        //     return;
        // }
        currentAngleRad = angleRad;
        EventsContainer.ClockArrowMoved?.Invoke(currentAngleRad);
        arrowRectTransform.eulerAngles = new Vector3(0, 0, currentAngleRad * Mathf.Rad2Deg);
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (!shouldRespondToInput)
        {
            return;
        }

        shouldTimePass = true;
    }

    private float ComputePressAngleRad(Vector3 pressPos)
    { 
        Vector2 origin = Vector2.up;

        if (!RectTransformUtility.RectangleContainsScreenPoint(rect, pressPos))
        {
            //return;
        }

        Vector3 pressDirection = pressPos - arrowRectTransform.position;
        pressDirection = Quaternion.Euler(0, 0, 90) * pressDirection; // rotate for atan2

        float angleRad = Mathf.Atan2(pressDirection.y, pressDirection.x);
        angleRad += Mathf.PI;
        if (angleRad < 0)
        {
            angleRad = 2 * Mathf.PI + angleRad;
        }
        
        return angleRad;
    }

    private bool isApproaching;
    private IEnumerator ApproachingCoroutine()
    {
        isApproaching = true;
        while(!haveApproached)
        {
            print("Approaching");
            ApproachTouchPos(angleToApproachRad);
            yield return null;
        }
        isApproaching = false;
    }

    private void ApproachTouchPos(float angleRad)
    {
        float sign = 1;
        if (currentAngleRad > angleRad)
        {
            sign = -1;
        }
        float prevAngleRad = currentAngleRad;
        currentAngleRad += sign * speedOfApproachRad * Time.deltaTime;
        totalAddedAngleDeg += (currentAngleRad - prevAngleRad) * Mathf.Rad2Deg;
        if (sign > 0 && currentAngleRad > angleRad
            ||
            sign < 0 && currentAngleRad < angleRad)
        {
            haveApproached = true;
            totalAddedAngleDeg += (angleRad - currentAngleRad) * Mathf.Rad2Deg;
            currentAngleRad = angleRad;
        }

        EventsContainer.ClockArrowMoved?.Invoke(currentAngleRad);
        arrowRectTransform.eulerAngles = new Vector3(0, 0, currentAngleRad * Mathf.Rad2Deg);
    }

    private IEnumerator RevertingTime()
    {
        int revolutionsCount = (int)(totalAddedAngleDeg / 360);
        float remainedAngleDeg = 0;

        float revoultionStartAngleDeg = totalAddedAngleDeg % 360;

        for (int i = 0; i < revolutionsCount + 1; i++)
        {
            if (i != revolutionsCount)
            { 
                remainedAngleDeg = 360;
            }
            else
            { 
                remainedAngleDeg = revoultionStartAngleDeg;
            }
            while (remainedAngleDeg > 0)
            { 
                float stepDeg = speedOfRevertingDeg * Time.deltaTime;
                remainedAngleDeg -= stepDeg;
                if (remainedAngleDeg >= 0)
                {
                    arrowRectTransform.eulerAngles -= new Vector3(0, 0, -stepDeg);
                    currentAngleRad = arrowRectTransform.eulerAngles.z * Mathf.Deg2Rad;
                    EventsContainer.ClockArrowMoved?.Invoke(currentAngleRad);
                }
                else
                {
                    if (i != revolutionsCount)
                    { 
                        remainedAngleDeg = 0;
                        arrowRectTransform.eulerAngles = -Vector3.forward * revoultionStartAngleDeg;
                        currentAngleRad = revoultionStartAngleDeg * Mathf.Deg2Rad;
                        EventsContainer.ClockArrowMoved?.Invoke(currentAngleRad);
                    }
                    // exit code
                    else
                    {
                        arrowRectTransform.eulerAngles = Vector3.zero;
                        currentAngleRad = 0;
                        totalAddedAngleDeg = 0;
                        EventsContainer.ClockArrowMoved?.Invoke(currentAngleRad);
                        ContinueTime();
                        yield break;
                    }
                }
                yield return null;
            }
        }
    }

    /// <summary>
    /// Called after reverting time
    /// </summary>
    private void ContinueTime()
    {
        shouldTimePass = true;
        shouldRespondToInput = true;
    }
}