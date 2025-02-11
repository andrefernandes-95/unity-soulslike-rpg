namespace AFV2
{
    using UnityEngine;

    public class PlayerCombatDecision : CharacterCombatDecision
    {
        [SerializeField] PlayerController playerController;

        public override CombatDecision GetCombatDecision()
        {
            // Reactions should go first
            if (ShouldLightAttack(out CombatDecision combatDecision))
                return combatDecision;

            return CombatDecision.NONE;
        }

        bool ShouldLightAttack(out CombatDecision combatDecision)
        {
            combatDecision = CombatDecision.NONE;

            if (!characterApi.characterStats.HasEnoughStamina(characterCombat.LightAttackStaminaCost))
                return false;

            if (playerController.HasRightAttackQueued && characterApi.characterEquipment.characterWeapons.TryGetActiveRightWeapon(out _))
            {
                combatDecision = CombatDecision.RIGHT_LIGHT_ATTACK;
                return true;
            }
            else if (playerController.HasLeftAttackQueued && characterApi.characterEquipment.characterWeapons.TryGetActiveLeftWeapon(out _))
            {
                combatDecision = CombatDecision.LEFT_LIGHT_ATTACK;
                return true;
            }

            return false;
        }

    }
}
