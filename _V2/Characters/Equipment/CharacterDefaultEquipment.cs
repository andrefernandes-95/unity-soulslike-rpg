namespace AFV2
{
    using System.Collections.Generic;
    using UnityEngine;
    using System;

    public class CharacterDefaultEquipment : MonoBehaviour
    {
        [SerializeField] List<Weapon> defaultLeftWeapon = new();
        [SerializeField] List<Weapon> defaultRightWeapon = new();

        [Header("Components")]
        [SerializeField] CharacterWeapons characterWeapons;

        private void Awake()
        {
            EquipWeapons(characterWeapons.LeftWeapons, defaultLeftWeapon, (weapon, index) => characterWeapons.EquipLeftWeapon(weapon, index));
            EquipWeapons(characterWeapons.RightWeapons, defaultRightWeapon, (weapon, index) => characterWeapons.EquipRightWeapon(weapon, index));
        }

        private void EquipWeapons(Weapon[] weapons, List<Weapon> defaultWeapons, Action<Weapon, int> equipAction)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                var weaponToEquip = (i < defaultWeapons.Count && defaultWeapons[i] != null)
                    ? defaultWeapons[i]
                    : characterWeapons.FallbackWeapon;

                equipAction.Invoke(weaponToEquip, i);
            }
        }

    }
}
