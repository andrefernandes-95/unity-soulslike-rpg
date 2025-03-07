namespace AFV2
{
    using System;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class UICharacterEquipment : MonoBehaviour
    {
        public CharacterApi characterApi;

        [Header("Components")]
        public UICharacterInventory uICharacterInventory;

        [Header("Screens")]
        [SerializeField] MainMenu mainMenu;
        [SerializeField] MainMenuScreen equipmentScreen;
        [SerializeField] MainMenuScreen inventoryScreen;

        [Header("Labels")]
        [SerializeField] TextMeshProUGUI selectedSlotLabel;


        Dictionary<EquipmentSlotType, Action<ItemInstance, int>> EquipmentSlotSetters = new();

        void Awake()
        {
            InitializeEquipmentSlotSetters();
        }

        #region Awake Logic
        void InitializeEquipmentSlotSetters()
        {
            EquipmentSlotSetters = new Dictionary<EquipmentSlotType, Action<ItemInstance, int>>
            {
                { EquipmentSlotType.RIGHT_HAND, (item, slotIndex) =>
                    characterApi.characterWeapons.EquipRightWeapon(item as WeaponInstance, slotIndex) },
                { EquipmentSlotType.LEFT_HAND, (item, slotIndex) =>
                    characterApi.characterWeapons.EquipLeftWeapon(item as WeaponInstance, slotIndex) },
                { EquipmentSlotType.SKILL, (item, slotIndex) =>
                    characterApi.characterSkills.EquipSkill(item as SkillInstance, slotIndex) },
                { EquipmentSlotType.ARROW, (item, slotIndex) =>
                    characterApi.characterArchery.EquipArrow(item as ArrowInstance, slotIndex) },
                { EquipmentSlotType.ACCESSORY, (item, slotIndex) =>
                    characterApi.characterEquipment.EquipAccessory(item as AccessoryInstance, slotIndex) },
                { EquipmentSlotType.CONSUMABLE, (item, slotIndex) =>
                    characterApi.characterConsumables.EquipConsumable(item as ConsumableInstance, slotIndex) },
                { EquipmentSlotType.HEADGEAR, (item, slotIndex) =>
                    characterApi.characterEquipment.EquipHeadgear(item as HeadgearInstance) },
                { EquipmentSlotType.ARMOR, (item, slotIndex) =>
                    characterApi.characterEquipment.EquipArmor(item as ArmorInstance) },
                { EquipmentSlotType.BOOTS, (item, slotIndex) =>
                    characterApi.characterEquipment.EquipBoots(item as BootInstance) }
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
            mainMenu.SetScreen(inventoryScreen);
        }

        #region Slot Click
        public void OnSlotSelect(EquipmentSlotType equipmentSlotType, int slotIndex)
        {
            uICharacterInventory.SetEquipMode(equipmentSlotType, slotIndex);
            ShowItemList();
        }
        #endregion

        #region On Item Equipped from Inventory Screen

        public void OnItemEquipped(ItemInstance itemInstance, EquipmentSlotType equipmentType, int slotToEquip)
        {
            // Equip
            if (equipmentType != EquipmentSlotType.ALL && slotToEquip != -1)
            {
                EquipmentSlotSetters[equipmentType](itemInstance, slotToEquip);
            }

            mainMenu.SetScreen(equipmentScreen);
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
