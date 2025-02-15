namespace AFV2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class CharacterInventoryUI : MonoBehaviour
    {
        [SerializeField] CharacterInventory characterInventory;
        [SerializeField] CharacterEquipment characterEquipment;

        public UnityAction<Item, EquipmentSlotType, int> OnItemSelect;

        [Header("Components")]
        [SerializeField] ScrollRect itemsScrollRect;
        [SerializeField] InventorySlotButton inventorySlotButtonPrefab;
        InventoryFilterButton[] inventoryFilterButtons => GetComponentsInChildren<InventoryFilterButton>();

        [Header("Selected Item Label")]
        [SerializeField] TextMeshProUGUI selectedItemLabel;
        [SerializeField] Image selectedItemIcon;

        private EquipmentSlotType filter = EquipmentSlotType.ALL;
        private int slotFilter = -1;
        private bool enableFiltering = true;
        public bool EnableFiltering
        {
            get
            {
                return this.enableFiltering;
            }
            set
            {
                this.enableFiltering = value;
            }
        }

        Dictionary<EquipmentSlotType, Func<EquipmentSlotType, int, Item>> EquipmentSlotGetters = new();

        void Awake()
        {
            IntiializeEquipmentSlotGetters();

            AssignFilterButtonFilters();
        }

        void IntiializeEquipmentSlotGetters()
        {

            EquipmentSlotGetters = new Dictionary<EquipmentSlotType, Func<EquipmentSlotType, int, Item>>
            {
                { EquipmentSlotType.RIGHT_HAND, (slotType, index) => {
                    return characterEquipment.characterWeapons.RightWeapons[index]; } },
                { EquipmentSlotType.LEFT_HAND, (slotType, index) => {
                    return characterEquipment.characterWeapons.LeftWeapons[index]; } },
                { EquipmentSlotType.SKILL, (slotType, index) => {
                    return characterEquipment.Skills[index]; } },
                { EquipmentSlotType.ARROW, (slotType, index) => {
                    return characterEquipment.Arrows[index]; } },
                { EquipmentSlotType.ACCESSORY, (slotType, index) => {
                    return characterEquipment.Accessories[index]; } },
                { EquipmentSlotType.CONSUMABLE, (slotType, index) => {
                    return characterEquipment.Consumables[index]; } },
                { EquipmentSlotType.HEADGEAR, (slotType, index) => {
                    return characterEquipment.Headgear; } },
                { EquipmentSlotType.ARMOR, (slotType, index) => {
                    return characterEquipment.Armor; } },
                { EquipmentSlotType.BOOTS, (slotType, index) => {
                    return characterEquipment.Boots; } }
            };
        }

        void OnEnable()
        {
            ClearSelectedItemLabel();

            RenderItemsList();

            HandleFilterButtons();

            Invoke(nameof(HandleEventSystemSelection), 0f);
        }

        void OnDisable()
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        void HandleEventSystemSelection()
        {
            if (itemsScrollRect.content.childCount > 0)
                EventSystem.current.SetSelectedGameObject(itemsScrollRect.content.GetChild(0).gameObject);
        }

        #region Filtering
        void AssignFilterButtonFilters()
        {
            foreach (InventoryFilterButton inventoryFilterButton in inventoryFilterButtons)
            {
                inventoryFilterButton.Button.onClick.AddListener(() =>
                {
                    SetFilter(inventoryFilterButton.filter);

                    RenderItemsList();
                });
            }
        }

        void HandleFilterButtons()
        {
            foreach (InventoryFilterButton inventoryFilterButton in inventoryFilterButtons)
            {
                inventoryFilterButton.SetEnabled(inventoryFilterButton.filter == filter || enableFiltering);
                inventoryFilterButton.SetIsActive(filter);
            }
        }

        public void SetFilter(EquipmentSlotType filter) => this.filter = filter;
        public void SetSlotFilter(int slotFilter) => this.slotFilter = slotFilter;
        public void ClearSlotFilter(int slotFilter) => this.slotFilter = -1;
        #endregion

        #region Item List

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
                slotButton.OnSelectEvent += () => SetSelectedItemLabel(item.DisplayName);
                slotButton.OnDeselectEvent += () => ClearSelectedItemLabel();
                HandleIcons(item, slotButton);
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


        public void ClearScrollRectContent()
        {
            foreach (Transform child in itemsScrollRect.content)
            {
                Destroy(child.gameObject);
            }
        }

        #endregion

        #region  Item Button
        public void HandleIcons(Item item, InventorySlotButton slotButton)
        {
            HandleEquippedIcons(item, slotButton);
        }

        void HandleEquippedIcons(Item item, InventorySlotButton slotButton)
        {
            if (slotFilter == -1) return;

            Item equippedItem = EquipmentSlotGetters[filter](filter, slotFilter);

            if (item == equippedItem)
            {
                slotButton.ShowEquippedIcon();
            }
            else
            {
                slotButton.HideEquippedIcon();
            }
        }
        #endregion

        #region Selected Item Label
        public void SetSelectedItemLabel(string itemName)
        {
            selectedItemIcon.gameObject.SetActive(true);
            selectedItemLabel.text = itemName;
        }

        void ClearSelectedItemLabel()
        {
            selectedItemIcon.gameObject.SetActive(false);
            selectedItemLabel.text = "";
        }
        #endregion

    }
}
