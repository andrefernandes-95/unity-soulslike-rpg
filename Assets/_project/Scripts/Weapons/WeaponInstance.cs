namespace AFV2
{
    using System.Collections.Generic;
    using UnityEngine;

    public class WeaponInstance : MonoBehaviour
    {
        [Header("Weapon")]
        public Weapon equippedWeapon;
        [SerializeField] CharacterApi characterApi;

        [Header("Components")]
        public WeaponHitbox weaponHitbox;
        public RightWeaponAnimations rightWeaponAnimations;
        public LeftWeaponAnimations leftWeaponAnimations;

        public void OnWeaponSwitched(EquipmentSlotType slotType, Weapon weapon)
        {
            if (weaponHitbox != null)
            {
                weaponHitbox.DisableHitbox();
            }

            if (rightWeaponAnimations != null)
            {
                rightWeaponAnimations.OnWeaponSwitched(slotType, weapon, equippedWeapon);
            }
            else if (leftWeaponAnimations != null)
            {
                leftWeaponAnimations.OnWeaponSwitched(slotType, weapon, equippedWeapon);
            }

            if (weapon.DisplayName == equippedWeapon.DisplayName)
            {
                if (slotType == EquipmentSlotType.RIGHT_HAND && IsRightHandWeapon())
                {
                    characterApi.characterEquipment.characterWeapons.CurrentRightWeaponInstance = this;
                }
                else if (IsLeftHandWeapon())
                {
                    characterApi.characterEquipment.characterWeapons.CurrentLeftWeaponInstance = this;
                }

                this.gameObject.SetActive(true);
            }
            else
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
