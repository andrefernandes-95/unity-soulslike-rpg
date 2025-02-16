namespace AFV2
{
    using System.Collections.Generic;
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

            (List<string> availableAttacks, float staminaCost, CombatDecision combatDecision) = characterCombat.CharacterCombatDecision.GetNextAttack();
            await characterCombat.CharacterAttack.Attack(availableAttacks, staminaCost, combatDecision);

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
