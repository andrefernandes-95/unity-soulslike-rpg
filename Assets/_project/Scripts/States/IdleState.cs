namespace AFV2
{
    using System.Threading.Tasks;
    using UnityEngine;

    public class IdleState : State
    {
        [SerializeField] private string HASH_IDLE = "Idle";
        [SerializeField] private float blendTime = .05f;

        public CharacterApi characterApi;

        public override void OnStateEnter()
        {
            characterApi.animatorManager.BlendTo(HASH_IDLE, blendTime);
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
