namespace AFV2
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;

    public class AirAttackState : State
    {

        [Header("Components")]
        [SerializeField] CharacterWeapons characterWeapons;
        public CharacterGravity characterGravity;
        public CharacterCombat characterCombat;

        [Header("Transition State")]
        public FallState fallState;
        public State groundedState;

        State returnState;

        public override async void OnStateEnter()
        {
            returnState = this;

            (List<string> availableAttacks, float staminaCost, CombatDecision combatDecision) = characterCombat.CharacterCombatDecision.GetNextAirAttack();

            await characterCombat.CharacterAttack.Attack(availableAttacks, staminaCost, combatDecision);

            returnState = characterGravity.Grounded ? groundedState : fallState;
        }

        public override async Task OnStateExit()
        {
        }

        public override State Tick()
        {
            return returnState;
        }
    }
}
