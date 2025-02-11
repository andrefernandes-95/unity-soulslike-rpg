namespace AFV2
{
    using AF;
    using UnityEngine;

    public class CameraController : MonoBehaviour
    {
        public CharacterApi characterApi;

        [Header("Components")]
        [SerializeField] LockOnManager lockOnManager;
        [SerializeField] InputListener inputListener;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;
        public bool rotateWithCamera = false;
        // cinemachine
        private float _cinemachineTargetYaw = 0f;
        private float _cinemachineTargetPitch = 0f;
        private float _targetRotation = 0.0f;
        public float TargetRotation
        {
            get { return _targetRotation; }
        }

        private float _rotationVelocity;
        private const float _threshold = 0.01f;


        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        }

        void Update()
        {
            Rotate();
            CameraRotation();
        }

        void Rotate()
        {
            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving

            if (rotateWithCamera)
            {
                RotateWithCamera();
            }
            else if (inputListener.Move != Vector2.zero)
            {
                if (IsLockedOn())
                {
                    HandleLockOnRotation();
                    return;
                }

                if (characterApi.characterMovement.CanRotate)
                    HandleRotation();
            }
        }

        void RotateWithCamera()
        {
            Vector3 targetDir = Camera.main.transform.forward;
            targetDir.y = 0;
            targetDir.Normalize();

            if (targetDir == Vector3.zero)
                targetDir = characterApi.transform.forward;

            float rs = 12;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(characterApi.transform.rotation, tr, rs * Time.deltaTime);

            characterApi.transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        }

        void HandleLockOnRotation()
        {
            Vector3 targetRot = lockOnManager.nearestLockOnTarget.transform.position - characterApi.transform.position;
            targetRot.y = 0;
            var t = Quaternion.LookRotation(targetRot);

            characterApi.transform.rotation = Quaternion.Lerp(
                characterApi.transform.rotation, t, 100f * Time.deltaTime);
        }

        void HandleRotation()
        {
            // normalise input direction
            Vector3 inputDirection = new Vector3(
                inputListener.Move.x, 0.0f, inputListener.Move.y).normalized;

            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                     Camera.main.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(
                characterApi.transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            // rotate to face input direction relative to camera position
            characterApi.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (inputListener.Look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = 1.0f;

                _cinemachineTargetYaw += inputListener.Look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += inputListener.Look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        bool IsLockedOn() => lockOnManager != null
                    && lockOnManager.nearestLockOnTarget != null
                    && lockOnManager.isLockedOn;
        //&& playerManager.dodgeController.isDodging == false;
    }
}
