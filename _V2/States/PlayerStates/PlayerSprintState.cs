namespace AFV2
{
    using System.Threading.Tasks;
    using UnityEngine;

    public class PlayerSprintState : State
    {
        [Header("Movement Settings")]
        public float sprintSpeed = 8.5f;
        [SerializeField] private string HASH_SPRINT = "Sprint";

        [Header("Components")]
        public CharacterApi characterApi;
        public PlayerController playerController;

        [Header("Transition States")]
        public PlayerIdleState idleState;
        public PlayerRunState runState;
        public FallState fallState;
        public JumpState jumpState;
        public AttackState attackState;

        public override void OnStateEnter()
        {
            characterApi.animatorManager.BlendTo(HASH_SPRINT, 0.2f);
        }

        public override Task OnStateExit() => Task.CompletedTask;

        public override State Tick()
        {
            if (!playerController.IsSprinting())
                return runState;

            if (!characterApi.characterGravity.Grounded)
                return fallState;

            if (!playerController.IsMoving())
                return idleState;

            if (playerController.IsJumping())
                return jumpState;

            if (playerController.IsLightAttacking())
                return attackState;

            characterApi.characterStats.CharacterStamina.UseSprint();
            characterApi.characterMovement.Move(sprintSpeed, playerController.GetPlayerRotation());
            return this;
        }
    }
}
