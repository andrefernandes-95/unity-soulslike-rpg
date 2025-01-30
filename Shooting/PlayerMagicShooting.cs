namespace AF
{
    using AF.Health;
    using UnityEngine;

    public class PlayerMagicShooting : MonoBehaviour
    {
        [Header("Components")]
        public PlayerManager playerManager;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;
        public GameSession gameSession;

        readonly int hashTwoHandCast = Animator.StringToHash("Two Hand Casting");
        readonly int hashOneHandCast = Animator.StringToHash("One Hand Casting");
        readonly string TWO_HAND_ANIMATION_OVERRIDE_CLIP_NAME = "Cacildes - Spell - Two Handing Casting";

        // For cache purposes
        private Spell previousSpell;

        public void CastSpell()
        {
            if (!CanCastSpell())
            {
                return;
            }

            Spell currentSpell = equipmentDatabase.GetCurrentSpell().GetItem();

            playerManager.manaManager.DecreaseMana(currentSpell.manaCostPerCast);
            playerManager.staminaStatManager.DecreaseStamina(currentSpell.staminaCostPerCast);

            HandleSpellCastAnimationOverrides();

            PlayCastingAnimation();
        }

        bool CanCastSpell()
        {
            if (!equipmentDatabase.IsStaffEquipped())
            {
                return false;
            }

            if (!equipmentDatabase.GetCurrentSpell().Exists())
            {
                return false;
            }

            Spell spell = equipmentDatabase.GetCurrentSpell().GetItem();

            if (!playerManager.manaManager.HasEnoughManaForSpell(spell))
            {
                return false;
            }

            if (!playerManager.staminaStatManager.HasEnoughStaminaForAction(spell.staminaCostPerCast))
            {
                return false;
            }

            return true;
        }

        void HandleSpellCastAnimationOverrides()
        {
            Spell currentSpell = equipmentDatabase.GetCurrentSpell()?.GetItem();

            if (currentSpell == previousSpell)
            {
                return;
            }

            previousSpell = currentSpell;

            bool ignoreSpellsAnimationClips = false;
            if (
                currentSpell.animationCanNotBeOverriden == false &&
                equipmentDatabase.GetCurrentWeapon().Exists() &&
                equipmentDatabase.GetCurrentWeapon().GetItem().ignoreSpellsAnimationClips)
            {
                ignoreSpellsAnimationClips = true;
            }

            if (currentSpell.castAnimationOverride != null && ignoreSpellsAnimationClips == false)
            {
                playerManager.UpdateAnimatorOverrideControllerClip(
                    TWO_HAND_ANIMATION_OVERRIDE_CLIP_NAME,
                    currentSpell.castAnimationOverride);

                playerManager.RefreshAnimationOverrideState();
            }
        }

        void PlayCastingAnimation()
        {
            if (equipmentDatabase.isTwoHanding || !HasWeaponOrShieldOnSecondarySlot())
            {
                playerManager.PlayBusyHashedAnimationWithRootMotion(hashTwoHandCast);
            }
            else
            {
                playerManager.PlayBusyHashedAnimationWithRootMotion(hashOneHandCast);
            }
        }

        bool HasWeaponOrShieldOnSecondarySlot()
            => equipmentDatabase.GetCurrentSecondaryWeapon() != null || equipmentDatabase.GetCurrentShield() != null;

        public Damage ScaleSpellDamage(Damage damage, Spell currentSpell)
        {
            if (currentSpell != null)
            {
                var attackStatManager = playerManager.attackStatManager;
                var equipmentDatabase = attackStatManager.equipmentDatabase;
                var currentWeapon = equipmentDatabase.GetCurrentWeapon()?.GetItem();
                var isNightTime = gameSession.IsNightTime();
                var shouldDoubleDamage = currentWeapon != null && (
                    (currentWeapon.doubleDamageDuringNightTime && isNightTime) ||
                    (currentWeapon.doubleDamageDuringDayTime && !isNightTime)
                );
                float multiplier = shouldDoubleDamage ? 2 : 1f;

                if (playerManager.statsBonusController.spellDamageBonusMultiplier > 0)
                {
                    multiplier += playerManager.statsBonusController.spellDamageBonusMultiplier;
                }

                damage = damage.ScaleSpell(attackStatManager, multiplier);
            }

            return damage;
        }
    }
}
