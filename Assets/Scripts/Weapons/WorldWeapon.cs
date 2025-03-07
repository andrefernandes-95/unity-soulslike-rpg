namespace AFV2
{
    using System.Collections.Generic;
    using UnityEngine;

    public class WorldWeapon : MonoBehaviour
    {
        [Header("Weapon")]
        public Weapon equippedWeapon;
        [SerializeField] CharacterApi characterApi;

        [Header("Settings")]
        public bool isFeetWeapon = false;

        [Header("Components")]
        public WeaponHitbox weaponHitbox;

        [Header("Applicable to Right Hand")]
        public WeaponAnimations rightWeaponAnimations;
        public WeaponAnimations twoHandWeaponAnimations;
        [Header("Applicable to Left Hand")]
        public WeaponAnimations leftWeaponAnimations;
        public void OnWeaponSwitched(EquipmentSlotType slotType, WeaponInstance weaponInstance)
        {
            // Disable the weapon hitbox if it exists
            weaponHitbox?.DisableHitbox();

            // Exit early if this is a feet weapon
            if (isFeetWeapon)
            {
                return;
            }

            // Check if the weapon instance matches the equipped weapon
            if (weaponInstance != null && weaponInstance.item == equippedWeapon)
            {
                HandleEquippedWeapon(slotType);
            }
            else
            {
                HandleUnequippedWeapon(slotType);
            }
        }

        private void HandleEquippedWeapon(EquipmentSlotType slotType)
        {
            // Handle right-hand weapon
            if (slotType == EquipmentSlotType.RIGHT_HAND && IsRightHandWeapon())
            {
                characterApi.characterWeapons.CurrentRightWeaponInstance = this;

                // Apply two-handed or right-handed animations
                if (characterApi.characterWeapons.IsTwoHanding && twoHandWeaponAnimations != null)
                {
                    twoHandWeaponAnimations.ApplyAnimations(false);
                }
                else
                {
                    rightWeaponAnimations.ApplyAnimations(false);
                }

                this.gameObject.SetActive(true);
            }
            // Handle left-hand weapon (only if not two-handing)
            else if (slotType == EquipmentSlotType.LEFT_HAND && IsLeftHandWeapon() && !characterApi.characterWeapons.IsTwoHanding)
            {
                characterApi.characterWeapons.CurrentLeftWeaponInstance = this;
                leftWeaponAnimations.ApplyAnimations(true);
                this.gameObject.SetActive(true);
            }
            // Hide left-hand weapon if two-handing
            else if (characterApi.characterWeapons.IsTwoHanding && IsLeftHandWeapon())
            {
                this.gameObject.SetActive(false);
            }
        }

        private void HandleUnequippedWeapon(EquipmentSlotType slotType)
        {
            // Hide the weapon if it's in the right or left hand but not equipped
            if ((slotType == EquipmentSlotType.RIGHT_HAND && IsRightHandWeapon()) ||
                (slotType == EquipmentSlotType.LEFT_HAND && IsLeftHandWeapon()))
            {
                this.gameObject.SetActive(false);
            }
        }

        public List<string> GetAttacksForCombatDecision(CombatDecision combatDecision)
        {
            if (combatDecision == CombatDecision.RIGHT_AIR_ATTACK)
            {
                return rightWeaponAnimations.AirAttackAnimations;
            }

            if (combatDecision == CombatDecision.RIGHT_LIGHT_ATTACK)
            {
                return rightWeaponAnimations.LightAttackAnimations;
            }

            if (combatDecision == CombatDecision.HEAVY_ATTACK)
            {
                return rightWeaponAnimations.HeavyAttackAnimations;
            }

            if (combatDecision == CombatDecision.LEFT_AIR_ATTACK)
            {
                return leftWeaponAnimations.AirAttackAnimations;
            }

            if (combatDecision == CombatDecision.LEFT_LIGHT_ATTACK)
            {
                return leftWeaponAnimations.LightAttackAnimations;
            }

            return new List<string>();
        }

        public void EnableHitbox()
        {
            weaponHitbox.EnableHitbox();
        }
        public void DisableHitbox()
        {
            weaponHitbox.DisableHitbox();
        }

        bool IsRightHandWeapon() => rightWeaponAnimations != null;
        bool IsLeftHandWeapon() => leftWeaponAnimations != null;

    }
}
