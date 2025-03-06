namespace AFV2
{
    using UnityEngine;

    public class PlayerJumpState : JumpState
    {
        [Header("Movement While Jumping")]
        public float AirMoveSpeed = 6f;
        public float SprintSpeedMultiplier = 1.2f;

        [Header("Components")]
        [SerializeField] PlayerController playerController;


        public override State Tick()
        {
            if (playerController.IsJumpAttacking())
            {
                return airAttackState;
            }

            if (playerController.IsMoving())
                characterApi.characterMovement.Move(
                    AirMoveSpeed * (playerController.IsSprinting() ? SprintSpeedMultiplier : 1), playerController.GetPlayerRotation());

            return base.Tick();
        }
    }
}
