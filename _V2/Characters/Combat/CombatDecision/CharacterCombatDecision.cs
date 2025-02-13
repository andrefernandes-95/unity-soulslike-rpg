namespace AFV2
{
    using System.Collections.Generic;
    using UnityEngine;

    public class CharacterCombatDecision : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] protected CharacterApi characterApi;
        [SerializeField] protected CharacterCombat characterCombat;

        private (Weapon rightHandWeapon, Weapon leftHandWeapon) GetActiveWeapons()
        {
            characterApi.characterEquipment.characterWeapons.TryGetActiveRightWeapon(out Weapon rightHandWeapon);
            characterApi.characterEquipment.characterWeapons.TryGetActiveLeftWeapon(out Weapon leftHandWeapon);
            return (rightHandWeapon, leftHandWeapon);
        }

        #region Ground Combat
        public virtual (List<string> availableAttacks, float staminaCost, CombatDecision combatDecision) GetNextAttack()
        {
            CombatDecision combatDecision = GetCombatDecision();

            var (rightHandWeapon, leftHandWeapon) = GetActiveWeapons();

            if (combatDecision == CombatDecision.RIGHT_LIGHT_ATTACK)
            {
                if (rightHandWeapon != null)
                {
                    var availableAttacks = rightHandWeapon.GetAttacksForCombatDecision(combatDecision);
                    var staminaCost = characterCombat.LightAttackStaminaCost;
                    return (availableAttacks, staminaCost, combatDecision);
                }
            }
            else if (combatDecision == CombatDecision.LEFT_LIGHT_ATTACK)
            {
                if (leftHandWeapon != null)
                {
                    var availableAttacks = leftHandWeapon.GetAttacksForCombatDecision(combatDecision);
                    var staminaCost = characterCombat.LightAttackStaminaCost;
                    return (availableAttacks, staminaCost, combatDecision);
                }
            }
            else if (combatDecision == CombatDecision.HEAVY_ATTACK)
            {
                List<string> heavyAttacks = new();

                if (rightHandWeapon != null)
                {
                    heavyAttacks = rightHandWeapon.GetAttacksForCombatDecision(combatDecision);
                }
                else if (leftHandWeapon != null)
                {
                    heavyAttacks = leftHandWeapon.GetAttacksForCombatDecision(combatDecision);
                }

                var staminaCost = characterCombat.HeavyAttackStaminaCost;
                return (heavyAttacks, staminaCost, combatDecision);
            }

            return (new(), 0f, CombatDecision.NONE);
        }

        public virtual CombatDecision GetCombatDecision()
        {
            // Reactions should go first

            // Chase actions should go here

            // Close combat should go here

            if (TryRightOrLeftAttack(out CombatDecision combatDecision))
                return combatDecision;

            if (TryRightAttack(out combatDecision))
                return combatDecision;

            if (TryLeftAttack(out combatDecision))
                return combatDecision;

            return CombatDecision.NONE;
        }

        protected virtual bool TryRightOrLeftAttack(out CombatDecision combatDecision)
        {
            combatDecision = CombatDecision.NONE;

            if (!CanRightAttack())
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

        protected virtual bool CanRightAttack()
        {
            if (!characterApi.characterEquipment.characterWeapons.TryGetActiveRightWeapon(out _))
                return false;

            return CanLightAttack();
        }

        protected virtual bool CanLeftAttack()
        {
            if (!characterApi.characterEquipment.characterWeapons.TryGetActiveLeftWeapon(out _))
                return false;

            return CanLightAttack();
        }

        bool CanLightAttack() => characterApi.characterStats.HasEnoughStamina(characterCombat.LightAttackStaminaCost);

        #endregion

        #region  Air Combat

        private (List<string> availableAttacks, float staminaCost) GetAttackDetails(Weapon weapon, CombatDecision combatDecision)
        {
            List<string> availableAttacks = new();
            float staminaCost = 0f;

            if (weapon != null)
            {
                availableAttacks = weapon.GetAttacksForCombatDecision(combatDecision);
                staminaCost = characterCombat.LightAttackStaminaCost;
            }

            return (availableAttacks, staminaCost);
        }

        public virtual (List<string> availableAttacks, float staminaCost, CombatDecision combatDecision) GetNextAirAttack()
        {
            CombatDecision combatDecision = GetAirCombatDecision();
            float staminaCost = 0f;
            List<string> availableAttacks = new();

            var (rightHandWeapon, leftHandWeapon) = GetActiveWeapons();

            if (combatDecision == CombatDecision.RIGHT_AIR_ATTACK)
            {
                (availableAttacks, staminaCost) = GetAttackDetails(rightHandWeapon, combatDecision);
            }
            else if (combatDecision == CombatDecision.LEFT_AIR_ATTACK)
            {
                (availableAttacks, staminaCost) = GetAttackDetails(leftHandWeapon, combatDecision);
            }

            return (availableAttacks, staminaCost, combatDecision);
        }

        public virtual CombatDecision GetAirCombatDecision()
        {
            if (TryRightAirAttack(out CombatDecision combatDecision))
                return combatDecision;

            if (TryLeftAirAttack(out combatDecision))
                return combatDecision;

            return CombatDecision.NONE;
        }

        bool TryRightAirAttack(out CombatDecision combatDecision)
        {
            combatDecision = CombatDecision.NONE;

            if (CanRightAttack() && !characterApi.characterGravity.Grounded)
            {
                combatDecision = CombatDecision.RIGHT_AIR_ATTACK;
                return true;
            }

            return false;
        }

        bool TryLeftAirAttack(out CombatDecision combatDecision)
        {
            combatDecision = CombatDecision.NONE;

            if (CanLeftAttack() && !characterApi.characterGravity.Grounded)
            {
                combatDecision = CombatDecision.LEFT_AIR_ATTACK;
                return true;
            }

            return false;
        }
        #endregion

    }
}
