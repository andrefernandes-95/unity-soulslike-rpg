namespace AFV2
{
    using System.Threading.Tasks;
    using UnityEngine;

    public class IdleState : State
    {
        [SerializeField] private string HASH_IDLE = "Idle";

        public CharacterApi characterApi;

        public override void OnStateEnter()
        {
            characterApi.animatorManager.BlendTo(HASH_IDLE, 0.2f);
        }

        public override Task OnStateExit()
        {
            return Task.CompletedTask;
        }

        public override State Tick()
        {
            return this;
        }
    }
}
