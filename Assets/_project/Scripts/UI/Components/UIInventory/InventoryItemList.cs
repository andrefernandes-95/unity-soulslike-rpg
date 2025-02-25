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

        [SerializeField] ScrollRect itemsScrollRect;
        [SerializeField] InventorySlotButton inventorySlotButtonPrefab;

        [Header("Selected Item Label")]
        [SerializeField] TextMeshProUGUI selectedItemLabel;
        [SerializeField] Image selectedItemIcon;

        Dictionary<EquipmentSlotType, Func<EquipmentSlotType, int, Item>> EquipmentSlotGetters = new();

        void Awake()
        {
            IntiializeEquipmentSlotGetters();

            Invoke(nameof(HandleEventSystemSelection), 0f);
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

        void HandleEventSystemSelection()
        {
            if (itemsScrollRect.content.childCount > 0)
                EventSystem.current.SetSelectedGameObject(itemsScrollRect.content.GetChild(0).gameObject);
        }

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

        #region Filter and select items to display in the scroll rect
        List<Item> GetItemsToDisplay(List<Item> items)
        {
            return items.Where(item => item.hideFromUi == false).ToList();
        }

        bool IsWeaponAvailable(Weapon weapon)
        {
            int leftSlotIndex = Array.IndexOf(characterEquipment.characterWeapons.LeftWeapons, weapon);
            int rightSlotIndex = Array.IndexOf(characterEquipment.characterWeapons.RightWeapons, weapon);

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

            List<Item> weapons = characterInventory.Items.Where(item =>
            {
                if (item is not Weapon weapon)
                {
                    return false;
                }

                if (inventoryFilter.SlotFilter != -1)
                {
                    // If weapon is equipped in another slot, do not show
                    if (!IsWeaponAvailable(weapon))
                    {
                        return false;
                    }
                }

                return true;
            }).ToList();

            RenderItems(GetItemsToDisplay(weapons));
        }

        void ShowSkills()
        {
            if (inventoryFilter.Filter != EquipmentSlotType.SKILL)
            {
                return;
            }

            List<Item> skills = characterInventory.Items.Where(item =>
            {
                if (item is not Skill skill)
                {
                    return false;
                }

                if (inventoryFilter.SlotFilter != -1)
                {
                    if (IsItemAvailable(characterEquipment.Skills.Cast<Item>().ToList(), skill))
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

            List<Item> arrows = characterInventory.Items.Where(item =>
            {
                if (item is not Arrow arrow)
                {
                    return false;
                }

                if (inventoryFilter.SlotFilter != -1)
                {
                    if (IsItemAvailable(characterEquipment.Arrows.Cast<Item>().ToList(), arrow))
                    {
                        return false;
                    }
                }

                return true;
            }).ToList();

            RenderItems(GetItemsToDisplay(arrows));
        }

        void ShowAccessories()
        {
            if (inventoryFilter.Filter != EquipmentSlotType.ACCESSORY)
            {
                return;
            }

            List<Item> accessories = characterInventory.Items.Where(item =>
            {
                if (item is not Accessory accessory)
                {
                    return false;
                }

                if (inventoryFilter.SlotFilter != -1)
                {
                    if (IsItemAvailable(characterEquipment.Accessories.Cast<Item>().ToList(), accessory))
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

            List<Item> consumables = characterInventory.Items.Where(item =>
            {
                if (item is not Consumable consumable)
                {
                    return false;
                }

                if (inventoryFilter.SlotFilter != -1)
                {
                    if (IsItemAvailable(characterEquipment.Consumables.Cast<Item>().ToList(), consumable))
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

            List<Item> headgears = characterInventory.Items.Where(item =>
            {
                if (item is not Headgear headgear)
                {
                    return false;
                }

                return true;
            }).ToList();

            RenderItems(GetItemsToDisplay(headgears));
        }

        void ShowArmors()
        {
            if (inventoryFilter.Filter != EquipmentSlotType.ARMOR)
            {
                return;
            }

            List<Item> armors = characterInventory.Items.Where(item =>
            {
                if (item is not Armor armor)
                {
                    return false;
                }

                return true;
            }).ToList();

            RenderItems(GetItemsToDisplay(armors));
        }

        void ShowBoots()
        {
            if (inventoryFilter.Filter != EquipmentSlotType.BOOTS)
            {
                return;
            }

            List<Item> boots = characterInventory.Items.Where(item =>
            {
                if (item is not Boot boot)
                {
                    return false;
                }

                return true;
            }).ToList();

            RenderItems(GetItemsToDisplay(boots));
        }

        void ShowAll()
        {
            if (inventoryFilter.Filter != EquipmentSlotType.ALL)
            {
                return;
            }

            RenderItems(GetItemsToDisplay(characterInventory.Items));
        }
        #endregion

        void RenderItems(List<Item> items)
        {
            foreach (Item item in items)
            {
                InventorySlotButton slotButton = RenderItem(item);
                slotButton.onSelect += () => SetSelectedItemLabel(item.DisplayName);
                slotButton.onDeselect += () => ClearSelectedItemLabel();
                slotButton.onPointerEnter += () => SetSelectedItemLabel(item.DisplayName);
                slotButton.onPointerExit += () => ClearSelectedItemLabel();
                HandleIcons(item, slotButton);
            }
        }

        InventorySlotButton RenderItem(Item item)
        {
            InventorySlotButton itemButton = Instantiate(inventorySlotButtonPrefab, itemsScrollRect.content.transform).GetComponent<InventorySlotButton>();
            itemButton.BackgroundIcon = item.Sprite;

            itemButton.Button.onClick.AddListener(() =>
            {
                uICharacterInventory.OnItemSelect(item);
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
            if (inventoryFilter.SlotFilter == -1) return;

            Item equippedItem = EquipmentSlotGetters[inventoryFilter.Filter](inventoryFilter.Filter, inventoryFilter.SlotFilter);

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

    }
}