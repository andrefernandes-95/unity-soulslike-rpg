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
            if (weaponHitbox != null)
            {
                weaponHitbox.DisableHitbox();
            }

            if (isFeetWeapon)
            {
                return;
            }

            if (weaponInstance != null && weaponInstance.item == equippedWeapon)
            {
                if (slotType == EquipmentSlotType.RIGHT_HAND && IsRightHandWeapon())
                {
                    characterApi.characterWeapons.CurrentRightWeaponInstance = this;

                    if (characterApi.characterWeapons.IsTwoHanding)
                    {
                        twoHandWeaponAnimations.ApplyAnimations(false);
                    }
                    else
                    {
                        rightWeaponAnimations.ApplyAnimations(false);
                    }

                    this.gameObject.SetActive(true);
                }
                else if (slotType == EquipmentSlotType.LEFT_HAND && IsLeftHandWeapon() && !characterApi.characterWeapons.IsTwoHanding)
                {
                    characterApi.characterWeapons.CurrentLeftWeaponInstance = this;
                    leftWeaponAnimations.ApplyAnimations(true);
                    this.gameObject.SetActive(true);
                }
            }
            else if (slotType == EquipmentSlotType.RIGHT_HAND && IsRightHandWeapon() || slotType == EquipmentSlotType.LEFT_HAND && IsLeftHandWeapon())
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
