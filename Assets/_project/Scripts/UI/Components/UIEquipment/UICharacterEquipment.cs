namespace AFV2
{
    using System;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class UICharacterEquipment : MonoBehaviour
    {
        public CharacterEquipment characterEquipment;

        [Header("Components")]
        public UICharacterInventory uICharacterInventory;

        [Header("Labels")]
        [SerializeField] TextMeshProUGUI selectedSlotLabel;


        Dictionary<EquipmentSlotType, Action<Item, int>> EquipmentSlotSetters = new();

        void Awake()
        {
            InitializeEquipmentSlotSetters();
        }

        #region Awake Logic
        void InitializeEquipmentSlotSetters()
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
        }

        #endregion

        void OnEnable()
        {
            UpdateSelectedSlotLabel(GetDefaultLabel());
        }

        void OnDisable()
        {
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ShowItemList()
        {
            gameObject.SetActive(false);

            // mainMenu.SetScreen(characterInventoryScreen);
        }

        #region Slot Click
        public void OnSlotSelect(EquipmentSlotType equipmentSlotType, int slotIndex)
        {
            uICharacterInventory.SetEquipMode(equipmentSlotType, slotIndex);
            ShowItemList();
        }
        #endregion

        #region On Item Equipped from Inventory Screen

        public void OnItemEquipped(Item item, EquipmentSlotType equipmentType, int slotToEquip)
        {
            // Equip
            if (equipmentType != EquipmentSlotType.ALL && slotToEquip != -1)
            {
                EquipmentSlotSetters[equipmentType](item, slotToEquip);
            }

            gameObject.SetActive(true);
        }

        #endregion

        #region Selected Slot Label
        public void UpdateSelectedSlotLabel(string label)
        {
            if (string.IsNullOrEmpty(label)) label = GetDefaultLabel();
            this.selectedSlotLabel.text = label;
        }

        string GetDefaultLabel() => Glossary.IsPortuguese() ? "Equipamento" : "Equipment";
        #endregion


    }
}
