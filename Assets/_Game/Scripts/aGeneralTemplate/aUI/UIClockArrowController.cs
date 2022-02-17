using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIClockArrowController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField]
    private RectTransform arrowRectTransform;

    [SerializeField]
    [Tooltip("In degrees")]
    private float speedOfTimeDeg;

    [SerializeField]
    private float speedOfApproachRad;

    [SerializeField]
    private float approachEpsilon;

    private RectTransform rect;
    private bool isPositive;
    private bool shouldTimePass;

    private float currentAngle;

    private float angleToApproach;
    private bool haveApproached;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        timer = 0;
        isPositive = false;
        shouldTimePass = true;

        currentAngle = 0;
    }

    private void Update()
    {
        if (shouldTimePass)
        { 
            float step = speedOfTimeDeg * Time.deltaTime;
            arrowRectTransform.eulerAngles += new Vector3(0, 0, -step);
            currentAngle = arrowRectTransform.eulerAngles.z * Mathf.Deg2Rad;

            EventsContainer.ClockArrowMoved?.Invoke(arrowRectTransform.eulerAngles.z * Mathf.Deg2Rad);
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        haveApproached = false;
        shouldTimePass = false;
        OnDrag(data);
    }

    private float timer; 
    public void OnDrag(PointerEventData data)
    {
        float angle = ComputePressAngle(data.position);

        print("OnDrag");
        if (!haveApproached)
        {
            print("Inside if");
            angleToApproach = angle;
            if (!isApproaching)
            {
                print("StartingCoroutine");
                StartCoroutine(ApproachingCoroutine());
            }
            print("Returning");
            return;
        }
        print(currentAngle + " : " + angle);
        currentAngle = angle;
        EventsContainer.ClockArrowMoved?.Invoke(currentAngle);
        arrowRectTransform.eulerAngles = new Vector3(0, 0, currentAngle * Mathf.Rad2Deg);
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

    public void OnPointerUp(PointerEventData data)
    {
        shouldTimePass = true;
    }
}