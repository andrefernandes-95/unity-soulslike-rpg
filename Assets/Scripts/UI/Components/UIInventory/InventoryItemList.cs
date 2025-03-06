namespace AFV2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class InventoryItemList : MonoBehaviour
    {
        [SerializeField] CharacterEquipment characterEquipment;
        [SerializeField] CharacterInventory characterInventory;

        [Header("Inventory UI Components")]
        [SerializeField] UICharacterInventory uICharacterInventory;
        [SerializeField] InventoryFilter inventoryFilter;
        [SerializeField] ItemTooltip itemTooltip;

        [SerializeField] ScrollRect itemsScrollRect;
        [SerializeField] InventorySlotButton inventorySlotButtonPrefab;


        Dictionary<EquipmentSlotType, Func<EquipmentSlotType, int, ItemInstance>> EquipmentSlotGetters = new();

        void Awake()
        {
            IntiializeEquipmentSlotGetters();

            Invoke(nameof(HandleEventSystemSelection), 0f);
        }

        void OnDisable()
        {
            ClearPreviewItem();
        }

        #region Initialization
        void IntiializeEquipmentSlotGetters()
        {

            EquipmentSlotGetters = new Dictionary<EquipmentSlotType, Func<EquipmentSlotType, int, ItemInstance>>
            {
                { EquipmentSlotType.RIGHT_HAND, (slotType, index) => {
                    return characterEquipment.characterWeapons.rightWeapons[index]; } },
                { EquipmentSlotType.LEFT_HAND, (slotType, index) => {
                    return characterEquipment.characterWeapons.leftWeapons[index]; } },
                { EquipmentSlotType.SKILL, (slotType, index) => {
                    return characterEquipment.characterWeapons.skills[index]; } },
                { EquipmentSlotType.ARROW, (slotType, index) => {
                    return characterEquipment.characterWeapons.arrows[index]; } },
                { EquipmentSlotType.ACCESSORY, (slotType, index) => {
                    return characterEquipment.accessories[index]; } },
                { EquipmentSlotType.CONSUMABLE, (slotType, index) => {
                    return characterEquipment.consumables[index]; } },
                { EquipmentSlotType.HEADGEAR, (slotType, index) => {
                    return characterEquipment.headgear; } },
                { EquipmentSlotType.ARMOR, (slotType, index) => {
                    return characterEquipment.armor; } },
                { EquipmentSlotType.BOOTS, (slotType, index) => {
                    return characterEquipment.boot; } }
            };
        }

        void HandleEventSystemSelection()
        {
            if (itemsScrollRect.content.childCount > 0)
                EventSystem.current.SetSelectedGameObject(itemsScrollRect.content.GetChild(0).gameObject);
        }
        #endregion

        #region Preview Item Tooltip
        public void PreviewItem(ItemInstance itemInstance)
        {
            itemTooltip.Show(itemInstance);
        }

        void ClearPreviewItem()
        {
            itemTooltip.Hide();
        }
        #endregion

        #region Item List

        public void RenderItemsList()
        {
            ClearScrollRectContent();

            ShowWeapons();
            ShowSkills();
            ShowArrows();
            ShowAccessories();
            ShowConsumables();
            ShowHeadgears();
            ShowArmors();
            ShowBoots();
            ShowAll();
        }

        #endregion

        #region Filter and select items to display in the scroll rect
        List<ItemInstance> GetItemsToDisplay(List<ItemInstance> items)
        {
            return items;
        }

        bool IsWeaponAvailable(WeaponInstance weaponInstance)
        {
            if (weaponInstance.item is Weapon weapon && weapon.isFallbackWeapon)
            {
                return false;
            }

            int leftSlotIndex = Array.IndexOf(characterEquipment.characterWeapons.leftWeapons, weaponInstance);
            int rightSlotIndex = Array.IndexOf(characterEquipment.characterWeapons.rightWeapons, weaponInstance);

            bool isRightHandFilter = inventoryFilter.Filter == EquipmentSlotType.RIGHT_HAND;
            bool isLeftHandFilter = inventoryFilter.Filter == EquipmentSlotType.LEFT_HAND;

            // Not equipped yet
            if (leftSlotIndex == -1 && rightSlotIndex == -1)
            {
                return true;
            }

            if (isRightHandFilter)
            {
                // Equipped this slot and not equipped elsewhere
                return rightSlotIndex == inventoryFilter.SlotFilter && leftSlotIndex < 0;
            }

            if (isLeftHandFilter)
            {
                // Equipped on this slot and not equipped elsewhere
                return leftSlotIndex == inventoryFilter.SlotFilter && rightSlotIndex < 0;
            }

            return false;
        }

        bool IsItemAvailable(List<Item> equippedItems, Item itemToEquip)
        {
            int equippedIndex = equippedItems.FindIndex(equippedItem => equippedItem == itemToEquip);

            // Unavailable is item is equipped on another slot
            return equippedIndex != -1 && equippedIndex != inventoryFilter.SlotFilter;
        }

        void ShowWeapons()
        {
            if (inventoryFilter.Filter != EquipmentSlotType.LEFT_HAND && inventoryFilter.Filter != EquipmentSlotType.RIGHT_HAND)
            {
                return;
            }

            List<ItemInstance> weapons = characterInventory.GetItems<Weapon>()
                .Where(itemInstance =>
            {
                if (itemInstance.item is not Weapon weapon)
                {
                    return false;
                }

                if (inventoryFilter.SlotFilter != -1)
                {
                    // If weapon is equipped in another slot, do not show
                    if (!IsWeaponAvailable(itemInstance as WeaponInstance))
                    {
                        return false;
                    }
                }

                return true;
            }).Cast<ItemInstance>().ToList();

            RenderItems(GetItemsToDisplay(weapons));
        }

        void ShowSkills()
        {
            if (inventoryFilter.Filter != EquipmentSlotType.SKILL)
            {
                return;
            }

            List<ItemInstance> skills = characterInventory.GetItems<Skill>()
                .Where(item =>
            {
                if (inventoryFilter.SlotFilter != -1)
                {
                    if (IsItemAvailable(characterEquipment.characterWeapons.skills.Cast<Item>().ToList(), item.item))
                    {
                        return false;
                    }
                }

                return true;
            }).ToList();

            RenderItems(GetItemsToDisplay(skills));
        }

        void ShowArrows()
        {
            if (inventoryFilter.Filter != EquipmentSlotType.ARROW)
            {
                return;
            }

            List<ItemInstance> arrows = characterInventory.GetItems<Arrow>()
                .Where(item =>
            {
                if (inventoryFilter.SlotFilter != -1)
                {
                    if (IsItemAvailable(characterEquipment.characterWeapons.arrows.Cast<Item>().ToList(), item.item))
                    {
                        return false;
                    }
                }

                return true;
            }).ToList<ItemInstance>();

            RenderItems(GetItemsToDisplay(arrows));
        }

        void ShowAccessories()
        {
            if (inventoryFilter.Filter != EquipmentSlotType.ACCESSORY)
            {
                return;
            }

            List<ItemInstance> accessories = characterInventory.GetItems<Accessory>()
                .Where(item =>
            {
                if (inventoryFilter.SlotFilter != -1)
                {
                    if (IsItemAvailable(characterEquipment.accessories.Cast<Item>().ToList(), item.item))
                    {
                        return false;
                    }
                }

                return true;
            }).ToList();

            RenderItems(GetItemsToDisplay(accessories));
        }

        void ShowConsumables()
        {
            if (inventoryFilter.Filter != EquipmentSlotType.CONSUMABLE)
            {
                return;
            }

            List<ItemInstance> consumables = characterInventory.GetItems<Consumable>()
                .Where(item =>
            {
                if (inventoryFilter.SlotFilter != -1)
                {
                    if (IsItemAvailable(characterEquipment.consumables.Cast<Item>().ToList(), item.item))
                    {
                        return false;
                    }
                }

                return true;
            }).ToList();

            RenderItems(GetItemsToDisplay(consumables));
        }

        void ShowHeadgears()
        {
            if (inventoryFilter.Filter != EquipmentSlotType.HEADGEAR)
            {
                return;
            }

            List<ItemInstance> headgears = characterInventory.GetItems<Headgear>().ToList();

            RenderItems(GetItemsToDisplay(headgears));
        }

        void ShowArmors()
        {
            if (inventoryFilter.Filter != EquipmentSlotType.ARMOR)
            {
                return;
            }

            List<ItemInstance> armors = characterInventory.GetItems<Armor>().ToList();

            RenderItems(GetItemsToDisplay(armors));
        }

        void ShowBoots()
        {
            if (inventoryFilter.Filter != EquipmentSlotType.BOOTS)
            {
                return;
            }

            List<ItemInstance> boots = characterInventory.GetItems<Boot>().ToList();
            RenderItems(GetItemsToDisplay(boots));
        }

        void ShowAll()
        {
            if (inventoryFilter.Filter != EquipmentSlotType.ALL)
            {
                return;
            }

            RenderItems(GetItemsToDisplay(characterInventory.GetItems<Item>().ToList()));
        }
        #endregion

        #region Render Items
        void RenderItems(List<ItemInstance> items)
        {
            foreach (ItemInstance itemInstance in items)
            {
                Item item = itemInstance.item;

                InventorySlotButton slotButton = RenderItem(itemInstance);
                slotButton.onSelect += () => PreviewItem(itemInstance);
                slotButton.onDeselect += ClearPreviewItem;
                slotButton.onPointerEnter += () => PreviewItem(itemInstance);
                slotButton.onPointerExit += ClearPreviewItem;
                HandleIcons(itemInstance, slotButton);
            }
        }

        InventorySlotButton RenderItem(ItemInstance itemInstance)
        {
            InventorySlotButton itemButton = Instantiate(inventorySlotButtonPrefab, itemsScrollRect.content.transform).GetComponent<InventorySlotButton>();

            itemButton.SetBackgroundIcon(itemInstance.item.Sprite);

            itemButton.Button.onClick.AddListener(() =>
            {
                uICharacterInventory.OnItemSelect(itemInstance);
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
        public void HandleIcons(ItemInstance itemInstance, InventorySlotButton slotButton)
        {
            HandleEquippedIcons(itemInstance, slotButton);
        }

        void HandleEquippedIcons(ItemInstance itemInstance, InventorySlotButton slotButton)
        {
            if (inventoryFilter.SlotFilter == -1) return;

            ItemInstance equippedItem = EquipmentSlotGetters[inventoryFilter.Filter](inventoryFilter.Filter, inventoryFilter.SlotFilter);

            if (itemInstance.ID == equippedItem.ID)
            {
                slotButton.ShowEquippedIcon();
            }
            else
            {
                slotButton.HideEquippedIcon();
            }
        }
        #endregion

    }
}
