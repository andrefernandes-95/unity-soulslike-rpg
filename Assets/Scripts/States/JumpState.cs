namespace AFV2
{
    using System.Threading.Tasks;
    using UnityEngine;

    public class JumpState : State
    {
        [SerializeField] private string HASH_JUMP = "Jump";
        [SerializeField] float jumpBlend = 0.05f;

        [Header("Components")]
        public CharacterApi characterApi;

        [Header("Transition State")]
        public FallState fallState;
        public AirAttackState airAttackState;

        public override void OnStateEnter()
        {
            characterApi.animatorManager.BlendTo(HASH_JUMP, jumpBlend);
            characterApi.characterGravity.Jump();
        }


        public override Task OnStateExit() => Task.CompletedTask;

        public override State Tick()
        {
            if (characterApi.characterGravity.VerticalVelocity <= 0)
            {
                return fallState;
            }

            return this;
        }
    }
}
