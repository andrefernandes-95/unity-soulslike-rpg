namespace AFV2
{
    using UnityEngine;

    public class PlayerIdleState : IdleState
    {
        public PlayerController playerController;

        [Header("Transition States")]
        public PlayerRunState runState;
        public JumpState jumpState;

        public override State Tick()
        {
            if (playerController.IsMoving())
                return runState;

            if (playerController.IsJumping())
                return jumpState;

            return this;
        }
    }
}
