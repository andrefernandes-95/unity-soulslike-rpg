namespace AFV2
{
    using System.Threading.Tasks;
    using UnityEngine;

    public class PlayerRunState : State
    {
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

        public override void OnStateEnter()
        {
            characterApi.animatorManager.BlendTo(HASH_RUN, 0.2f);
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

            if (playerController.IsJumping())
                return jumpState;

            if (playerController.IsLightAttacking())
                return attackState;

            playerController.Move(moveSpeed);
            return this;
        }
    }
}
