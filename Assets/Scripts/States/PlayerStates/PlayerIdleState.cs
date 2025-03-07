namespace AFV2
{
    using UnityEngine;

    public class PlayerIdleState : IdleState
    {
        public PlayerController playerController;

        [Header("Transition States")]
        public PlayerRunState runState;
        public JumpState jumpState;
        public AttackState attackState;
        public DodgeState dodgeState;

        public override State Tick()
        {
            if (playerController.IsMoving())
                return runState;

            if (playerController.IsJumping())
                return jumpState;

            if (playerController.IsLightAttacking() || playerController.IsHeavyAttacking())
                return attackState;

            if (playerController.IsDodging())
                return dodgeState;

            return this;
        }
    }
}
