namespace AFV2
{
    using System.Collections.Generic;
    using UnityEngine;
    using System;

    public class CharacterDefaultEquipment : MonoBehaviour
    {
        [Header("Default Equipment")]
        public Weapon fallbackWeapon;
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
        [SerializeField] CharacterApi characterApi;

        private void Awake()
        {
            SyncInventoryWithEquipment();
        }

        void SyncInventoryWithEquipment()
        {
            EquipWeapons(characterApi.characterWeapons.leftWeapons, defaultLeftWeapon, (weapon, index) =>
            {
                characterApi.characterWeapons.EquipLeftWeapon(weapon, index);
            });
            EquipWeapons(characterApi.characterWeapons.rightWeapons, defaultRightWeapon, (weapon, index) =>
            {
                characterApi.characterWeapons.EquipRightWeapon(weapon, index);
            });

            for (int i = 0; i < defaultArrows.Count; i++)
            {
                if (defaultArrows[i] != null)
                {
                    for (int j = 0; j < defaultArrowNumber; j++)
                    {
                        ArrowInstance arrowInstance = characterApi.characterInventory.AddItem(defaultArrows[i]) as ArrowInstance;
                        characterApi.characterArchery.EquipArrow(arrowInstance, i);
                    }
                }
            }

            for (int i = 0; i < defaultSkills.Count; i++)
            {
                if (defaultSkills[i] != null)
                {
                    SkillInstance skillInstance = characterApi.characterInventory.AddItem(defaultSkills[i]) as SkillInstance;
                    characterApi.characterSkills.EquipSkill(skillInstance, i);
                }
            }

            for (int i = 0; i < defaultAccessories.Count; i++)
            {
                if (defaultAccessories[i] != null)
                {
                    AccessoryInstance accessoryInstance = characterApi.characterInventory.AddItem(defaultAccessories[i]) as AccessoryInstance;
                    characterApi.characterEquipment.EquipAccessory(accessoryInstance, i);
                }
            }

            for (int i = 0; i < defaultConsumables.Count; i++)
            {
                if (defaultConsumables[i] != null)
                {
                    ConsumableInstance consumableInstance = characterApi.characterInventory.AddItem(defaultConsumables[i]) as ConsumableInstance;
                    characterApi.characterConsumables.EquipConsumable(consumableInstance, i);
                }
            }

            if (defaultHeadgear != null)
            {
                HeadgearInstance headgearInstance = characterApi.characterInventory.AddItem(defaultHeadgear) as HeadgearInstance;
                characterApi.characterEquipment.EquipHeadgear(headgearInstance);
            }

            if (defaultArmor != null)
            {
                ArmorInstance armorInstance = characterApi.characterInventory.AddItem(defaultArmor) as ArmorInstance;
                characterApi.characterEquipment.EquipArmor(armorInstance);
            }

            if (defaultBoots != null)
            {
                BootInstance bootInstance = characterApi.characterInventory.AddItem(defaultBoots) as BootInstance;
                characterApi.characterEquipment.EquipBoots(bootInstance);
            }
        }

        private void EquipWeapons(WeaponInstance[] weaponInstances, List<Weapon> defaultWeapons, Action<WeaponInstance, int> equipAction)
        {
            for (int i = 0; i < weaponInstances.Length; i++)
            {
                var weaponToEquip = (i < defaultWeapons.Count && defaultWeapons[i] != null)
                    ? defaultWeapons[i]
                    : fallbackWeapon;

                WeaponInstance weaponInstance = new(fallbackWeapon);

                if (weaponToEquip != null && !weaponToEquip.isFallbackWeapon)
                {
                    weaponInstance = characterApi.characterInventory.AddItem(weaponToEquip) as WeaponInstance;
                }

                equipAction.Invoke(weaponInstance, i);
            }
        }

    }
}
