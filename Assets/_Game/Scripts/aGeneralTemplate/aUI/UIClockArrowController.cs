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

    private bool isPositive;
    private bool shouldTimePass;
    private bool shouldRespondToInput;

    private float currentAngle;
    private float totalAddedAngle;

    private float angleToApproach;
    private bool haveApproached;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        timer = 0;
        isPositive = false;
        shouldTimePass = true;
        shouldRespondToInput = true;

        currentAngle = 0;
        totalAddedAngle = 0;

        EventsContainer.PlayerCollidedWithDeath += OnPlayerCollidedWithDeath;
        //EventsContainer.PlayerRevertedToStartPos += OnPlayerRevertedToStartPos;
    }

    private void OnDestroy()
    { 
        EventsContainer.PlayerCollidedWithDeath -= OnPlayerCollidedWithDeath;
        //EventsContainer.PlayerRevertedToStartPos -= OnPlayerRevertedToStartPos;
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
        if (shouldTimePass)
        { 
            float step = speedOfUsualTimeDeg * Time.deltaTime;
            Vector3 toAdd = new Vector3(0, 0, -step);
            arrowRectTransform.eulerAngles += toAdd;
            totalAddedAngle += step;

            currentAngle = arrowRectTransform.eulerAngles.z * Mathf.Deg2Rad;

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

    private float timer; 
    public void OnDrag(PointerEventData data)
    {
        if (!shouldRespondToInput)
        {
            return;
        }

        float angle = ComputePressAngle(data.position);
        totalAddedAngle += angle - currentAngle;

        if (!haveApproached)
        {
            angleToApproach = angle;
            if (!isApproaching)
            {
                StartCoroutine(ApproachingCoroutine());
            }
            print("Returning");
            return;
        }
        currentAngle = angle;
        EventsContainer.ClockArrowMoved?.Invoke(currentAngle);
        arrowRectTransform.eulerAngles = new Vector3(0, 0, currentAngle * Mathf.Rad2Deg);
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (!shouldRespondToInput)
        {
            return;
        }

        shouldTimePass = true;
    }

    private float ComputePressAngle(Vector3 pressPos)
    { 
        Vector2 origin = Vector2.up;

        if (!RectTransformUtility.RectangleContainsScreenPoint(rect, pressPos))
        {
            //return;
        }

        Vector3 pressDirection = pressPos - arrowRectTransform.position;
        pressDirection = Quaternion.Euler(0, 0, 90) * pressDirection; // rotate for atan2

        float angle = Mathf.Atan2(pressDirection.y, pressDirection.x);
        angle += Mathf.PI;
        if (angle < 0)
        {
            angle = 2 * Mathf.PI + angle;
        }
        
        return angle;
    }

    private bool isApproaching;
    private IEnumerator ApproachingCoroutine()
    {
        isApproaching = true;
        while(!haveApproached)
        {
            print("Approaching");
            ApproachTouchPos(angleToApproach);
            yield return null;
        }
        isApproaching = false;
    }

    private void ApproachTouchPos(float angle)
    {
        float sign = 1;
        if (currentAngle > angle)
        {
            sign = -1;
        }
        currentAngle += sign * speedOfApproachRad * Time.deltaTime;
        if (sign > 0 && currentAngle > angle)
        {
            haveApproached = true;
            currentAngle = angle;
        }
        else if (sign < 0 && currentAngle < angle)
        {
            haveApproached = true;
            currentAngle = angle;
        }
        //print("cur : " + currentAngle + "\nappr " + angle);

        EventsContainer.ClockArrowMoved?.Invoke(currentAngle);
        arrowRectTransform.eulerAngles = new Vector3(0, 0, currentAngle * Mathf.Rad2Deg);
    }

    private IEnumerator RevertingTime()
    {
        int revolutionsCount = (int)totalAddedAngle / 360;
        float remainedAngle = 0;

        for (int i = 0; i < revolutionsCount; i++)
        {
            remainedAngle = 360;
            while (remainedAngle > 0)
            { 
                float step = speedOfRevertingDeg * Time.deltaTime;
                arrowRectTransform.eulerAngles -= new Vector3(0, 0, -step);

                currentAngle = arrowRectTransform.eulerAngles.z * Mathf.Deg2Rad;
                EventsContainer.ClockArrowMoved?.Invoke(currentAngle);

                remainedAngle -= step;
                yield return null;
            }
        }

        remainedAngle = totalAddedAngle % 360;
        while (remainedAngle > 0)
        {
            float step = speedOfRevertingDeg * Time.deltaTime;
            arrowRectTransform.eulerAngles -= new Vector3(0, 0, -step);

            currentAngle = arrowRectTransform.eulerAngles.z * Mathf.Deg2Rad;
            EventsContainer.ClockArrowMoved?.Invoke(currentAngle);

            remainedAngle -= step;
            yield return null;
        }
        arrowRectTransform.eulerAngles = Vector3.zero;

        ContinueTime();
    }

    /// <summary>
    /// Called after reverting time
    /// </summary>
    private void ContinueTime()
    {
        shouldTimePass = true;
        shouldRespondToInput = true;
        totalAddedAngle = 0;
    }
}