namespace AFV2
{
    using System.Threading.Tasks;
    using UnityEngine;

    public class AttackState : State
    {
        [Header("Transition")]
        [SerializeField] private float attackBlendTime = 0.1f;

        [Header("Components")]
        public CharacterApi characterApi;
        public CharacterCombat characterCombat;
        public ActionClip actionClipTest;

        [Header("Transition State")]
        public State idleState;

        State returnState;

        public override async void OnStateEnter()
        {
            returnState = this;
            await actionClipTest.Play(characterApi.animatorManager);

            returnState = idleState;
        }

        public override async Task OnStateExit()
        {
            return;
        }

        public override State Tick()
        {
            return returnState;
        }
    }
}
