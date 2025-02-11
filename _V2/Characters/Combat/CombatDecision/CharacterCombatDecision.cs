namespace AFV2
{
    using UnityEngine;

    public class CharacterCombatDecision : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] protected CharacterApi characterApi;
        [SerializeField] protected CharacterCombat characterCombat;

        public virtual CombatDecision GetCombatDecision()
        {
            // Reactions should go first

            // Chase actions should go here

            // Close combat should go here

            if (TryRightOrLightAttack(out CombatDecision combatDecision))
                return combatDecision;

            if (TryRightAttack(out combatDecision))
                return combatDecision;

            if (TryLeftAttack(out combatDecision))
                return combatDecision;

            return CombatDecision.NONE;
        }

        bool TryRightOrLightAttack(out CombatDecision combatDecision)
        {
            combatDecision = CombatDecision.NONE;

            if (!characterApi.characterStats.HasEnoughStamina(characterCombat.LightAttackStaminaCost))
                return false;

            float normalizedStamina = characterApi.characterStats.GetNormalizedStamina();

            // More right attacks when stamina is high
            combatDecision = (Random.value <= MathHelpers.PositiveSigmoid(normalizedStamina))
                ? CombatDecision.RIGHT_LIGHT_ATTACK
                : CombatDecision.LEFT_LIGHT_ATTACK;
            return true;
        }

        bool TryRightAttack(out CombatDecision combatDecision)
        {
            combatDecision = CombatDecision.NONE;

            if (ShouldAttemptHeavyAttack())
            {
                combatDecision = CombatDecision.HEAVY_ATTACK;
                return true;
            }

            if (CanRightAttack())
            {
                combatDecision = CombatDecision.RIGHT_LIGHT_ATTACK;
                return true;
            }

            return false;
        }

        bool TryLeftAttack(out CombatDecision combatDecision)
        {
            combatDecision = CombatDecision.NONE;

            if (ShouldAttemptHeavyAttack())
            {
                combatDecision = CombatDecision.HEAVY_ATTACK;
                return true;
            }

            if (CanLeftAttack())
            {
                combatDecision = CombatDecision.LEFT_LIGHT_ATTACK;
                return true;
            }

            return false;
        }

        bool ShouldAttemptHeavyAttack()
        {
            if (!characterApi.characterStats.HasEnoughStamina(characterCombat.HeavyAttackStaminaCost))
            {
                return false;
            }

            // Heavy attacks should occur more often when health is low because the character would be desperate
            return Random.value <= MathHelpers.NegativeSigmoid(characterApi.characterStats.GetNormalizedHealth());
        }

        bool CanRightAttack()
        {
            if (!characterApi.characterEquipment.characterWeapons.TryGetActiveRightWeapon(out _))
                return false;

            return CanLightAttack();
        }

        bool CanLeftAttack()
        {
            if (!characterApi.characterEquipment.characterWeapons.TryGetActiveLeftWeapon(out _))
                return false;

            return CanLightAttack();
        }

        bool CanLightAttack() => characterApi.characterStats.HasEnoughStamina(characterCombat.LightAttackStaminaCost);

    }
}
