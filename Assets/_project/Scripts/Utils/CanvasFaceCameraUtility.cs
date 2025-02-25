namespace AFV2
{
    using UnityEngine;

    public class CanvasFaceCameraUtility : MonoBehaviour
    {
        public Camera mainCamera;  // Assign the camera in the inspector (or use Camera.main)

        void Start()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main; // Fallback to Camera.main if not assigned
            }
        }

        private void LateUpdate()
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        }
    }
}
