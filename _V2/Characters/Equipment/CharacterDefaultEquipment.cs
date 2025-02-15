namespace AFV2
{
    using System.Collections.Generic;
    using UnityEngine;
    using System;

    public class CharacterDefaultEquipment : MonoBehaviour
    {
        [Header("Default Equipment")]
        [SerializeField] List<Weapon> defaultLeftWeapon = new();
        [SerializeField] List<Weapon> defaultRightWeapon = new();
        [SerializeField] List<Skill> defaultSkills = new();
        [SerializeField] List<Arrow> defaultArrows = new();
        [SerializeField] List<Consumable> defaultConsumables = new();
        [SerializeField] List<Accessory> defaultAccessories = new();
        [SerializeField] Headgear defaultHeadgear;
        [SerializeField] Armor defaultArmor;
        [SerializeField] Boot defaultBoots;

        [Header("Options")]
        [SerializeField] int defaultArrowNumber = 15;

        [Header("Components")]
        [SerializeField] CharacterWeapons characterWeapons;
        [SerializeField] CharacterEquipment characterEquipment;
        [SerializeField] CharacterInventory characterInventory;

        private void Awake()
        {
            SyncInventoryWithEquipment();
        }

        void SyncInventoryWithEquipment()
        {
            EquipWeapons(characterWeapons.LeftWeapons, defaultLeftWeapon, (weapon, index) => characterWeapons.EquipLeftWeapon(weapon, index));
            EquipWeapons(characterWeapons.RightWeapons, defaultRightWeapon, (weapon, index) => characterWeapons.EquipRightWeapon(weapon, index));

            for (int i = 0; i < defaultArrows.Count; i++)
            {
                if (defaultArrows[i] != null)
                {
                    Arrow addedArrowItem = characterInventory.AddItem(defaultArrows[i], defaultArrowNumber) as Arrow;
                    characterEquipment.EquipArrow(addedArrowItem, i);
                }
            }

            for (int i = 0; i < defaultSkills.Count; i++)
            {
                if (defaultSkills[i] != null)
                {
                    Skill defaultSkillItem = characterInventory.AddItem(defaultSkills[i], 1) as Skill;
                    characterEquipment.EquipSkill(defaultSkillItem, i);
                }
            }

            for (int i = 0; i < defaultAccessories.Count; i++)
            {
                if (defaultAccessories[i] != null)
                {
                    Accessory defaultAccessoryItem = characterInventory.AddItem(defaultAccessories[i], 1) as Accessory;
                    characterEquipment.EquipAccessory(defaultAccessoryItem, i);
                }
            }

            for (int i = 0; i < defaultConsumables.Count; i++)
            {
                if (defaultConsumables[i] != null)
                {
                    Consumable defaultConsumableItem = characterInventory.AddItem(defaultConsumables[i], 1) as Consumable;
                    characterEquipment.EquipConsumable(defaultConsumableItem, i);
                }
            }

            if (defaultHeadgear != null)
            {
                Headgear defaultHeadgearItem = characterInventory.AddItem(defaultHeadgear, 1) as Headgear;
                characterEquipment.EquipHeadgear(defaultHeadgearItem);
            }

            if (defaultArmor != null)
            {
                Armor defaultArmorItem = characterInventory.AddItem(defaultArmor, 1) as Armor;
                characterEquipment.EquipArmor(defaultArmorItem);
            }

            if (defaultBoots != null)
            {
                Boot defaultBootsItem = characterInventory.AddItem(defaultBoots, 1) as Boot;
                characterEquipment.EquipBoots(defaultBootsItem);
            }
        }

        private void EquipWeapons(Weapon[] weapons, List<Weapon> defaultWeapons, Action<Weapon, int> equipAction)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                var weaponToEquip = (i < defaultWeapons.Count && defaultWeapons[i] != null)
                    ? defaultWeapons[i]
                    : characterWeapons.FallbackWeapon;

                if (weaponToEquip != characterWeapons.FallbackWeapon)
                {
                    characterInventory.AddItem(weaponToEquip, 1);
                }

                equipAction.Invoke(weaponToEquip, i);
            }
        }

    }
}
