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

        [Header("Aim Settings")]
        float cachedDistance;
        [SerializeField] float aimDistance = 2f;
        Vector3 cachedShoulderOffset;
        [SerializeField] Vector3 aimShoulderOffset = Vector3.zero;

        CinemachineThirdPersonFollow cinemachineThirdPersonFollow => GetComponent<CinemachineThirdPersonFollow>();

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
                cinemachineThirdPersonFollow.CameraDistance = gameSettings.cameraDistance;
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


        #region Aiming


        public void BeginAiming()
        {
            cachedDistance = cinemachineThirdPersonFollow.CameraDistance;
            cinemachineThirdPersonFollow.CameraDistance = aimDistance;

            cachedShoulderOffset = cinemachineThirdPersonFollow.ShoulderOffset;
            cinemachineThirdPersonFollow.ShoulderOffset = aimShoulderOffset;
        }
        public void EndAiming()
        {
            cinemachineThirdPersonFollow.CameraDistance = cachedDistance;
            cinemachineThirdPersonFollow.ShoulderOffset = cachedShoulderOffset;
        }
        #endregion
    }
}
