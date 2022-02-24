using UnityEngine;

using Cinemachine;

namespace GeneralTemplate
{
    public enum CameraLocation
    {
        Default,
        Custom
    }
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private Camera renderingCamera;

        [SerializeField]
        private CinemachineVirtualCamera followerCamera;

        private void Awake()
        {
            EventsContainer.PlayerReachedGates += OnPlayerReachedGates;

            QueriesContainer.CurrentCameraYaw += GetCameraYaw;
        }

        private void OnDisable()
        {
            EventsContainer.PlayerReachedGates -= OnPlayerReachedGates;

            QueriesContainer.CurrentCameraYaw -= GetCameraYaw;
        }

        private float GetCameraYaw()
        {
            return renderingCamera.transform.eulerAngles.y;
        }

        public Ray GetScreenToWorldRay(Vector2 screenPos)
        {
            return renderingCamera.ScreenPointToRay(screenPos);
        }

        private void OnPlayerReachedGates()
        {
            followerCamera.enabled = false;
        }
    }
}
