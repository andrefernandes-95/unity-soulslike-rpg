namespace AFV2
{
    using UnityEngine;
    public class CharacterMovement : MonoBehaviour, IResetCharacterStatesOnStateEnterListener
    {
        public CharacterApi characterApi;

        private bool canRotate = true;
        public bool CanRotate => canRotate;

        public void Move(float targetSpeed, Quaternion rotation)
        {
            Vector3 targetDirection = rotation * Vector3.forward;

            characterApi.characterController.Move(
                            targetDirection.normalized * (targetSpeed * Time.deltaTime));
        }

        public void DisableRotation() => canRotate = false;

        public void ResetStates()
        {
            canRotate = true;
        }
    }
}
