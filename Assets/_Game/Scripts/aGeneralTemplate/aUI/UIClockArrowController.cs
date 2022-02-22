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
    private float speedOfApproachRad;

    /// <summary>
    /// Needed for the determining whether the finger is outside the pointable area.
    /// </summary>
    private RectTransform pressRectangle;

    private bool shouldRespondToInput;

    private float angleToApproachRad;
    private bool haveApproached;

    private void Awake()
    {
        pressRectangle = GetComponent<RectTransform>();
        shouldRespondToInput = true;

        EventsContainer.ClockTimeChange += OnClockTimeChange;

        EventsContainer.UsualTimeFlow += OnUsualTimeFlow;
        EventsContainer.RevertingTimeFlow += OnRevertingTimeFlow;
    }

    private void OnDestroy()
    { 
        EventsContainer.ClockTimeChange -= OnClockTimeChange;

        EventsContainer.UsualTimeFlow -= OnUsualTimeFlow;
        EventsContainer.RevertingTimeFlow -= OnRevertingTimeFlow;
    }

    private float OnClockTimeChange(float timeState)
    {
        float angleRad = timeState * 2 * Mathf.PI;
        arrowRectTransform.eulerAngles = -Vector3.forward * angleRad * Mathf.Rad2Deg;
        return 0;
    }

    private void OnUsualTimeFlow()
    {
        shouldRespondToInput = true;
    }

    private void OnRevertingTimeFlow()
    {
        shouldRespondToInput = false;
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (!shouldRespondToInput)
        {
            return;
        }
        EventsContainer.ClockInputStart?.Invoke();
        OnDrag(data);
    }

    public void OnDrag(PointerEventData data)
    {
        if (!shouldRespondToInput)
        {
            return;
        }

        float angleRad = ComputePressAngleRad(data.position);
        //print(angleRad);
        //Debug.Break();
        EventsContainer.ClockInputUpdate?.Invoke(angleRad);
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (!shouldRespondToInput)
        {
            return;
        }

        EventsContainer.ClockInputEnd?.Invoke();
    }

    private float ComputePressAngleRad(Vector3 pressPos)
    { 
        Vector2 origin = Vector2.up;

        if (!RectTransformUtility.RectangleContainsScreenPoint(pressRectangle, pressPos))
        {
            //return;
        }

        Vector3 pressDirection = pressPos - arrowRectTransform.position;
        pressDirection = Quaternion.Euler(0, 0, 90) * pressDirection; // rotate for atan2

        float angleRad = Mathf.Atan2(pressDirection.y, pressDirection.x);
        angleRad += Mathf.PI;

        angleRad = 2 * Mathf.PI - angleRad;

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
        if (arrowRectTransform.eulerAngles.z > angleRad)
        {
            sign = -1;
        }
        arrowRectTransform.eulerAngles += 
            Vector3.forward * 
            sign * speedOfApproachRad * Time.deltaTime *
            Mathf.Rad2Deg;
    }
}