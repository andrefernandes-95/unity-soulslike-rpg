namespace AFV2
{
    using UnityEngine;

    public class PlayerJumpState : JumpState
    {
        [Header("Movement While Jumping")]
        public float AirMoveSpeed = 6f;

        [Header("Components")]
        [SerializeField] PlayerController playerController;

        public override State Tick()
        {
            if (playerController.IsMoving())
                playerController.Move(AirMoveSpeed * (playerController.IsSprinting() ? 1.5f : 1));

            return base.Tick();
        }
    }
}
