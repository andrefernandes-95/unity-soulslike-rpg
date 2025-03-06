namespace AFV2
{
    using Unity.Cinemachine;
    using UnityEngine;

    public class PlayerCamera : MonoBehaviour
    {
        private CinemachineCamera cinemachineVirtualCamera;
        public GameSettings gameSettings;

        [Header("Components")]
        [SerializeField] InputListener inputListener;

        void Awake()
        {
            AssignListeners();

            SetupCamera();
        }

        void AssignListeners()
        {
            inputListener.onZoomIn += ZoomIn;
            inputListener.onZoomOut += ZoomOut;
        }

        void SetupCamera()
        {
            cinemachineVirtualCamera = GetComponent<CinemachineCamera>();
            UpdateCameraDistance();
        }

        void UpdateCameraDistance()
        {
            if (gameSettings != null)
            {
                cinemachineVirtualCamera.GetComponent<CinemachineThirdPersonFollow>().CameraDistance = gameSettings.cameraDistance;
            }
        }

        #region Zoom Logic
        void ZoomIn(float scrollDelta)
        {
            if (gameSettings != null)
            {
                UpdateZoom(gameSettings.cameraDistance - scrollDelta * gameSettings.zoomSpeed);
            }
        }

        void ZoomOut(float scrollDelta)
        {
            if (gameSettings != null)
            {
                UpdateZoom(gameSettings.cameraDistance + scrollDelta * gameSettings.zoomSpeed);
            }
        }

        void UpdateZoom(float value)
        {
            if (gameSettings != null)
            {
                gameSettings.cameraDistance = Mathf.Clamp(value, gameSettings.minimumCameraDistance, gameSettings.maximumCameraDistance);
                UpdateCameraDistance();
            }
        }
        #endregion

    }
}
