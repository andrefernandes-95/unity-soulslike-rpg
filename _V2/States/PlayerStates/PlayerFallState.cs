namespace AFV2
{
    using UnityEngine;

    public class PlayerFallState : FallState
    {
        [Header("Movement While Falling")]
        public float FallMoveSpeed = 3f;
        public float SprintSpeedMultiplier = 1.2f;

        [Header("Player Grounded States")]
        public PlayerIdleState playerIdleState;
        public PlayerRunState playerRunState;

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
                    FallMoveSpeed * (playerController.IsSprinting() ? SprintSpeedMultiplier : 1), playerController.GetPlayerRotation());

            if (characterApi.characterGravity.Grounded)
                return playerController.IsMoving() ? playerRunState : playerIdleState;

            return this;
        }
    }
}
