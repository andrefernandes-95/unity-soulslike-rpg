namespace AFV2
{
    using System.Threading.Tasks;
    using UnityEngine;

    public class AttackState : State
    {

        [Header("Components")]
        public CharacterCombat characterCombat;

        [Header("Transition State")]
        public State idleState;

        State returnState;

        public override async void OnStateEnter()
        {
            returnState = this;

            await characterCombat.Attack();

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
