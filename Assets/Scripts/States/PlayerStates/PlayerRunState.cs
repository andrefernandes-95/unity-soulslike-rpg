namespace AFV2
{
    using System.Threading.Tasks;
    using UnityEngine;

    public class PlayerRunState : State
    {
        [Header("Blend In Settings")]
        [SerializeField] float blendInTime = 0.05f;

        [Header("Movement Settings")]
        public float moveSpeed = 5.5f;
        [SerializeField] private string HASH_RUN = "Run";
        public CharacterApi characterApi;
        public PlayerController playerController;

        [Header("Transition States")]
        public PlayerIdleState idleState;
        public PlayerSprintState sprintState;
        public FallState fallState;
        public JumpState jumpState;
        public AttackState attackState;
        public DodgeState dodgeState;

        public override void OnStateEnter()
        {
            characterApi.animatorManager.BlendTo(HASH_RUN, blendInTime);
        }

        public override Task OnStateExit()
        {
            return Task.CompletedTask;
        }

        public override State Tick()
        {
            if (!characterApi.characterGravity.Grounded)
                return fallState;

            if (!playerController.IsMoving())
                return idleState;

            if (playerController.IsSprinting())
                return sprintState;

            if (playerController.IsDodging())
                return dodgeState;

            if (playerController.IsJumping())
                return jumpState;

            if (playerController.IsLightAttacking() || playerController.IsHeavyAttacking())
                return attackState;

            characterApi.characterMovement.Move(moveSpeed, playerController.GetPlayerRotation());
            return this;
        }
    }
}
