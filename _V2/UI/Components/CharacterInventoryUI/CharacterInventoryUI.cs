namespace AFV2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class CharacterInventoryUI : MonoBehaviour
    {
        [SerializeField] CharacterInventory characterInventory;
        [SerializeField] CharacterEquipment characterEquipment;

        public UnityAction<Item, EquipmentSlotType, int> OnItemSelect;

        [Header("Components")]
        [SerializeField] ScrollRect itemsScrollRect;
        [SerializeField] InventorySlotButton inventorySlotButtonPrefab;

        private EquipmentSlotType filter = EquipmentSlotType.ALL;
        private int slotFilter = -1;

        Dictionary<EquipmentSlotType, Func<EquipmentSlotType, int, Item>> EquipmentSlotGetters = new();

        void Awake()
        {
            EquipmentSlotGetters = new Dictionary<EquipmentSlotType, Func<EquipmentSlotType, int, Item>>
            {
                { EquipmentSlotType.RIGHT_HAND, (slotType, index) => characterEquipment.characterWeapons.RightWeapons[index] },
                { EquipmentSlotType.LEFT_HAND, (slotType, index) => characterEquipment.characterWeapons.LeftWeapons[index] },
                { EquipmentSlotType.SKILL, (slotType, index) => characterEquipment.Skills[index] },
                { EquipmentSlotType.ARROW, (slotType, index) => characterEquipment.Arrows[index] },
                { EquipmentSlotType.ACCESSORY, (slotType, index) => characterEquipment.Accessories[index] },
                { EquipmentSlotType.CONSUMABLE, (slotType, index) => characterEquipment.Consumables[index] },
                { EquipmentSlotType.HEADGEAR, (slotType, index) => characterEquipment.Headgear },
                { EquipmentSlotType.ARMOR, (slotType, index) => characterEquipment.Armor },
                { EquipmentSlotType.BOOTS, (slotType, index) => characterEquipment.Boots }
            };
        }

        void OnEnable()
        {
            RenderItemsList();
        }

        public void SetFilter(EquipmentSlotType filter) => this.filter = filter;

        public void SetSlotFilter(int slotFilter) => this.slotFilter = slotFilter;
        public void ClearSlotFilter(int slotFilter) => this.slotFilter = -1;

        public void RenderItemsList()
        {
            ClearScrollRectContent();

            if (filter == EquipmentSlotType.RIGHT_HAND || filter == EquipmentSlotType.LEFT_HAND) RenderItems(typeof(Weapon));
            else if (filter == EquipmentSlotType.ARROW) RenderItems(typeof(Arrow));
            else if (filter == EquipmentSlotType.SKILL) RenderItems(typeof(Skill));
            else if (filter == EquipmentSlotType.ACCESSORY) RenderItems(typeof(Accessory));
            else if (filter == EquipmentSlotType.CONSUMABLE) RenderItems(typeof(Consumable));
            else if (filter == EquipmentSlotType.HEADGEAR) RenderItems(typeof(Headgear));
            else if (filter == EquipmentSlotType.ARMOR) RenderItems(typeof(Armor));
            else if (filter == EquipmentSlotType.BOOTS) RenderItems(typeof(Boot));
            else if (filter == EquipmentSlotType.ALL) RenderItems();
        }

        void RenderItems(Type itemType = null)
        {
            var itemsToShow = characterInventory.Items.Where(item => itemType == null || itemType.IsInstanceOfType(item));

            foreach (Item item in itemsToShow)
            {
                InventorySlotButton slotButton = RenderItem(item);
                HandleIcons(slotButton);
            }
        }

        InventorySlotButton RenderItem(Item item)
        {
            InventorySlotButton itemButton = Instantiate(inventorySlotButtonPrefab, itemsScrollRect.content.transform).GetComponent<InventorySlotButton>();
            itemButton.itemIcon.sprite = item.Sprite;

            itemButton.Button.onClick.AddListener(() =>
            {
                if (OnItemSelect != null)
                {
                    OnItemSelect(item, filter, slotFilter);
                }
            });

            return itemButton;
        }

        public void HandleIcons(InventorySlotButton slotButton)
        {
            HandleEquippedIcons(slotButton);
        }

        void HandleEquippedIcons(InventorySlotButton slotButton)
        {
            if (slotFilter == -1) return;

            Item item = EquipmentSlotGetters[filter](filter, slotFilter);

            if (item != null)
            {
                slotButton.ShowEquippedIcon();
            }
            else
            {
                slotButton.HideEquippedIcon();
            }
        }

        public void ClearScrollRectContent()
        {
            foreach (Transform child in itemsScrollRect.content)
            {
                Destroy(child.gameObject);
            }
        }

    }
}
