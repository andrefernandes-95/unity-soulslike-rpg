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
        public CharacterStats characterStats;

        [Header("Transition States")]
        public PlayerIdleState idleState;
        public PlayerRunState runState;
        public FallState fallState;
        public JumpState jumpState;

        public override void OnStateEnter()
        {
            characterApi.animatorManager.BlendTo(HASH_SPRINT, 0.2f);
        }

        public override Task OnStateExit() => Task.CompletedTask;

        public override State Tick()
        {
            if (!CanSprint())
                return runState;

            if (!characterApi.characterGravity.Grounded)
                return fallState;

            if (!playerController.IsMoving())
                return idleState;

            if (playerController.IsJumping())
                return jumpState;

            playerController.Move(sprintSpeed);
            return this;
        }

        bool CanSprint() => playerController.IsSprinting() && characterStats.Stamina > 0;
    }
}
