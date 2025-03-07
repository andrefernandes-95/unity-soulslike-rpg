namespace AFV2
{
    using UnityEngine;

    public class PlayerCombat : CharacterCombat
    {
        [SerializeField] PlayerController playerController;

        public override bool CanCombo(float staminaCost, CombatDecision combatDecision)
        {
            if (!characterApi.characterStamina.HasEnoughStamina(staminaCost))
                return false;

            if (combatDecision == CombatDecision.RIGHT_LIGHT_ATTACK || combatDecision == CombatDecision.RIGHT_AIR_ATTACK)
            {
                return playerController.HasRightAttackQueued;
            }

            if (combatDecision == CombatDecision.LEFT_LIGHT_ATTACK || combatDecision == CombatDecision.LEFT_AIR_ATTACK)
            {
                return playerController.HasLeftAttackQueued;
            }

            if (combatDecision == CombatDecision.HEAVY_ATTACK)
            {
                return playerController.HasHeavyAttackQueued;
            }

            return false;
        }
    }
}
