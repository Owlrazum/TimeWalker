using UnityEngine;
using UnityEngine.EventSystems;

public class UIClockArrowController : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField]
    private RectTransform arrowRectTransform;

    [SerializeField]
    [Tooltip("In degrees")]
    private float speedOfTime;

    private RectTransform rect;
    private bool isPositive;
    private bool shouldTimePass;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        timer = 0;
        isPositive = false;
        shouldTimePass = true;
    }

    private void Update()
    {
        if (shouldTimePass)
        { 
            float step = speedOfTime * Time.deltaTime;
            arrowRectTransform.eulerAngles += new Vector3(0, 0, -step);
            EventsContainer.ClockArrowMoved?.Invoke(arrowRectTransform.eulerAngles.z * Mathf.Deg2Rad);
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        OnDrag(data);
        shouldTimePass = false;
    }

    private float timer; 
    public void OnDrag(PointerEventData data)
    {
        Vector2 origin = Vector2.up;
        Vector3 pressPos = data.position;
        if (!RectTransformUtility.RectangleContainsScreenPoint(rect, pressPos))
        {
            //return;
        }
        Vector3 direction = pressPos - arrowRectTransform.position;
        //Debug.DrawRay(Vector3.zero, direction * 10, Color.white, 10, false);
        direction = Quaternion.Euler(0, 0, 90) * direction;

        //Debug.DrawRay(Vector3.zero, direction * 10, Color.red, 10, false);


        float angle = Mathf.Atan2(direction.y, -direction.x);
        if (angle < 0)
        {
            angle = 2 * Mathf.PI + angle;
        }

        EventsContainer.ClockArrowMoved?.Invoke(angle);
        arrowRectTransform.eulerAngles = new Vector3(0, 0, -angle * Mathf.Rad2Deg);
    }

}