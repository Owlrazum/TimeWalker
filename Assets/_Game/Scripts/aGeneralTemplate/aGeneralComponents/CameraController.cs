using UnityEngine;

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

        private void Awake()
        {
            QueriesContainer.CurrentCameraYaw += GetCameraYaw;
        }

        private void OnDisable()
        {
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
    }
}
