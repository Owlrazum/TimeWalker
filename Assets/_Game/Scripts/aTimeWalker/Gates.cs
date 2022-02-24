using UnityEngine;

public class Gates : MonoBehaviour
{
    [SerializeField]
    private SingleUnityLayer playerLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer.LayerIndex)
        {
            EventsContainer.PlayerReachedGates?.Invoke();
        }
    }
}
