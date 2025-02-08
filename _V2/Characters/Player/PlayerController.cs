namespace AFV2
{
    using UnityEngine;

    public class PlayerController : MonoBehaviour
    {
        public CharacterApi characterApi;

        [Header("Components")]
        [SerializeField] InputListener inputListener;
        [SerializeField] CameraController cameraController;

        public void Move(float targetSpeed)
        {
            Vector3 targetDirection = Quaternion.Euler(0.0f, cameraController.TargetRotation, 0.0f) * Vector3.forward;

            characterApi.characterController.Move(
                            targetDirection.normalized * (targetSpeed * Time.deltaTime));
        }

        public bool IsMoving() => inputListener.Move != Vector2.zero;
        public bool IsSprinting() => IsMoving() && inputListener.Sprint;
        public bool IsJumping() => inputListener.Jump && characterApi.characterGravity.Grounded;
    }
}
