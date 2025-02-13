namespace AFV2
{
    using System;
    using System.Collections.Generic;
    using AF;
    using TMPro;
    using UnityEngine;

    public class CharacterEquipmentUI : MonoBehaviour
    {
        public CharacterEquipment characterEquipment;

        [Header("Components")]
        public CharacterEquipmentUI characterEquipmentUI;
        public CharacterInventoryUI characterInventoryUI;

        [Header("Labels")]
        [SerializeField] TextMeshProUGUI selectedSlotLabel;


        Dictionary<EquipmentSlotType, Action<Item, int>> EquipmentSlotSetters = new();

        void Awake()
        {
            EquipmentSlotSetters = new Dictionary<EquipmentSlotType, Action<Item, int>>
            {
                { EquipmentSlotType.RIGHT_HAND, (item, slotIndex) =>
                    characterEquipment.characterWeapons.EquipRightWeapon(item as Weapon, slotIndex) },
                { EquipmentSlotType.LEFT_HAND, (item, slotIndex) =>
                    characterEquipment.characterWeapons.EquipLeftWeapon(item as Weapon, slotIndex) },
                { EquipmentSlotType.SKILL, (item, slotIndex) =>
                    characterEquipment.EquipSkill(item as Skill, slotIndex) },
                { EquipmentSlotType.ARROW, (item, slotIndex) =>
                    characterEquipment.EquipArrow(item as Arrow, slotIndex) },
                { EquipmentSlotType.ACCESSORY, (item, slotIndex) =>
                    characterEquipment.EquipAccessory(item as Accessory, slotIndex) },
                { EquipmentSlotType.CONSUMABLE, (item, slotIndex) =>
                    characterEquipment.EquipConsumable(item as Consumable, slotIndex) },
                { EquipmentSlotType.HEADGEAR, (item, slotIndex) =>
                    characterEquipment.EquipHeadgear(item as Headgear) },
                { EquipmentSlotType.ARMOR, (item, slotIndex) =>
                    characterEquipment.EquipArmor(item as Armor) },
                { EquipmentSlotType.BOOTS, (item, slotIndex) =>
                    characterEquipment.EquipBoots(item as Boot) }
            };

            characterInventoryUI.OnItemSelect += (Item item, EquipmentSlotType equipmentSlotType, int slotIndex) =>
            {
                // Equip
                if (equipmentSlotType != EquipmentSlotType.ALL && slotIndex != -1)
                {
                    EquipmentSlotSetters[equipmentSlotType](item, slotIndex);
                }

                ShowEquipmentSlots();
            };
        }

        void OnEnable()
        {
            UpdateSelectedSlotLabel(GetDefaultLabel());
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ShowEquipmentSlots()
        {
            UIUtils.FadeIn(characterEquipmentUI.gameObject);
            UIUtils.FadeOut(characterInventoryUI.gameObject);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ShowItemList()
        {
            UIUtils.FadeIn(characterInventoryUI.gameObject);
            UIUtils.FadeOut(characterEquipmentUI.gameObject);
        }

        public void OnSlotSelect(EquipmentSlotType equipmentSlotType, int slotIndex)
        {
            characterInventoryUI.SetFilter(equipmentSlotType);
            characterInventoryUI.SetSlotFilter(slotIndex);

            ShowItemList();
        }

        public void UpdateSelectedSlotLabel(string label)
        {
            if (string.IsNullOrEmpty(label)) label = GetDefaultLabel();
            this.selectedSlotLabel.text = label;
        }


        string GetDefaultLabel() => Glossary.IsPortuguese() ? "Equipamento" : "Equipment";
    }
}
