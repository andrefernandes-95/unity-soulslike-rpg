namespace AFV2
{
    using UnityEngine;

    public class PlayerCombatDecision : CharacterCombatDecision
    {
        [SerializeField] PlayerController playerController;

        protected override bool ShouldAttemptHeavyAttack()
        {
            characterApi.characterWeapons.TryGetActiveRightWeapon(out WorldWeapon weapon);

            return playerController.HasHeavyAttackQueued && characterApi.characterStamina.HasEnoughStamina(weapon.equippedWeapon.heavyAttackStaminaCost);
        }

        protected override bool CanRightAttack() =>
            playerController.HasRightAttackQueued && base.CanRightAttack();

        protected override bool CanLeftAttack() =>
            playerController.HasLeftAttackQueued && base.CanLeftAttack();

        // Override this method since it's not applicable to player
        protected override bool TryRightOrLeftAttack(out CombatDecision combatDecision)
        {
            combatDecision = CombatDecision.NONE;
            return false;
        }
    }
}
