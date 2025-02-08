namespace AFV2
{
    using System.Threading.Tasks;
    using UnityEngine;

    public class JumpState : State
    {
        [SerializeField] private string HASH_JUMP = "Jump";

        [Header("Components")]
        public CharacterApi characterApi;

        [Header("Transition State")]
        public FallState fallState;

        public override void OnStateEnter()
        {
            characterApi.animatorManager.BlendTo(HASH_JUMP, 0.2f);
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
