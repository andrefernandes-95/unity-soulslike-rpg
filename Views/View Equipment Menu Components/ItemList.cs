
namespace AF
{
    using System.Collections.Generic;
    using System.Linq;
    using AF.Inventory;
    using AF.UI.EquipmentMenu;
    using UnityEngine;
    using UnityEngine.Localization.Settings;
    using UnityEngine.UIElements;

    public class ItemList : MonoBehaviour, INestedView
    {
        public enum EquipmentType
        {
            WEAPON,
            SHIELD,
            ARROW,
            SPELL,
            HELMET,
            ARMOR,
            GAUNTLET,
            BOOTS,
            ACCESSORIES,
            CONSUMABLES,
            OTHER_ITEMS,
        }

        ScrollView itemsScrollView;

        public const string SCROLL_ITEMS_LIST = "ItemsList";

        [Header("UI Components")]
        public VisualTreeAsset itemButtonPrefab;
        public ItemTooltip itemTooltip;
        public PlayerStatsAndAttributesUI playerStatsAndAttributesUI;
        public EquipmentSlots equipmentSlots;
        [Header("UI Documents")]
        public UIDocument uIDocument;
        public VisualElement root;

        [Header("Components")]
        public MenuManager menuManager;
        public CursorManager cursorManager;
        public PlayerManager playerManager;
        public StarterAssetsInputs inputs;
        public Soundbank soundbank;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;
        public PlayerStatsDatabase playerStatsDatabase;
        public InventoryDatabase inventoryDatabase;

        Button returnButton;

        [HideInInspector] public bool shouldRerender = true;

        int lastScrollElementIndex = -1;

        public NotificationManager notificationManager;


        private void OnEnable()
        {
            if (shouldRerender)
            {
                shouldRerender = false;

                SetupRefs();
            }

            returnButton.transform.scale = new Vector3(1, 1, 1);
            root.Q<VisualElement>("ItemList").style.display = DisplayStyle.Flex;
        }

        private void OnDisable()
        {
            root.Q<VisualElement>("ItemList").style.display = DisplayStyle.None;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnUseItem()
        {
            /*
            if (isActiveAndEnabled && focusedItem != null && focusedItem is Consumable c)
            {
                playerManager.playerInventory.PrepareItemForConsuming(c);
            }*/
        }

        public void SetupRefs()
        {
            root = uIDocument.rootVisualElement;

            returnButton = root.Q<Button>("ReturnButton");
            UIUtils.SetupButton(returnButton, () =>
            {
                ReturnToEquipmentSlots();
            }, soundbank);

            itemsScrollView = root.Q<ScrollView>(SCROLL_ITEMS_LIST);
            UIUtils.SetupSlider(itemsScrollView);
        }

        public void ReturnToEquipmentSlots()
        {
            equipmentSlots.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }

        public void DrawUI(EquipmentType equipmentType, int slotIndex)
        {

            if (equipmentType == EquipmentType.WEAPON)
            {
                PopulateScrollView<Weapon>(false, slotIndex, false);
            }
            else if (equipmentType == EquipmentType.SHIELD)
            {
                PopulateScrollView<Shield>(false, slotIndex, true);
            }
            else if (equipmentType == EquipmentType.ARROW)
            {
                PopulateScrollView<Arrow>(false, slotIndex, false);
            }
            else if (equipmentType == EquipmentType.SPELL)
            {
                PopulateScrollView<Spell>(false, slotIndex, false);
            }
            else if (equipmentType == EquipmentType.HELMET)
            {
                PopulateScrollView<Helmet>(false, slotIndex, false);
            }
            else if (equipmentType == EquipmentType.ARMOR)
            {
                PopulateScrollView<Armor>(false, slotIndex, false);
            }
            else if (equipmentType == EquipmentType.GAUNTLET)
            {
                PopulateScrollView<Gauntlet>(false, slotIndex, false);
            }
            else if (equipmentType == EquipmentType.BOOTS)
            {
                PopulateScrollView<Legwear>(false, slotIndex, false);
            }
            else if (equipmentType == EquipmentType.ACCESSORIES)
            {
                PopulateScrollView<Accessory>(false, slotIndex, false);
            }
            else if (equipmentType == EquipmentType.CONSUMABLES)
            {
                PopulateScrollView<Consumable>(false, slotIndex, false);
            }
            else if (equipmentType == EquipmentType.OTHER_ITEMS)
            {
                PopulateScrollView<Item>(true, slotIndex, false);
            }

            // Delay the focus until the next frame, required as a hack for now
            Invoke(nameof(GiveFocus), 0f);
        }

        bool IsItemEquipped(ItemInstance item, int slotIndex, bool isShieldSlot)
        {
            if (item is WeaponInstance)
            {
                if (isShieldSlot)
                {
                    return equipmentDatabase.secondaryWeapons[slotIndex].IsId(item.GetId());
                }

                return equipmentDatabase.weapons[slotIndex].IsId(item.GetId());
            }
            else if (item is ShieldInstance)
            {
                return equipmentDatabase.shields[slotIndex].IsId(item.GetId());
            }
            else if (item is ArrowInstance)
            {
                return equipmentDatabase.arrows[slotIndex].IsId(item.GetId());
            }
            else if (item is SpellInstance)
            {
                return equipmentDatabase.spells[slotIndex].IsId(item.GetId());
            }
            else if (item is AccessoryInstance)
            {
                return equipmentDatabase.accessories[slotIndex].IsId(item.GetId());
            }
            else if (item is ConsumableInstance)
            {
                return equipmentDatabase.consumables[slotIndex].IsId(item.GetId());
            }
            else if (item is HelmetInstance)
            {
                return equipmentDatabase.helmet.IsId(item.GetId());
            }
            else if (item is ArmorInstance)
            {
                return equipmentDatabase.armor.IsId(item.GetId());
            }
            else if (item is GauntletInstance)
            {
                return equipmentDatabase.gauntlet.IsId(item.GetId());
            }
            else if (item is LegwearInstance)
            {
                return equipmentDatabase.legwear.IsId(item.GetId());
            }

            return false;
        }

        public bool IsKeyItem(Item item)
        {
            return !(item is Weapon || item is Shield || item is Helmet || item is Armor || item is Gauntlet || item is Legwear
                        || item is Accessory || item is Consumable || item is Spell || item is Arrow);
        }

        public bool ShouldShowItem<T>(ItemInstance itemInstance, int slotIndexToEquip, bool showOnlyKeyItems)
        {
            Item item = itemInstance.GetItem();

            if (item is not T)
            {
                return false;
            }

            if (showOnlyKeyItems && !IsKeyItem(item))
            {
                return false;
            }

            int equippedSlotIndex = -1;

            if (item is Weapon && itemInstance is WeaponInstance weaponInstance)
            {
                equippedSlotIndex = equipmentDatabase.GetEquippedWeaponSlot(weaponInstance);
            }
            else if (item is Shield && itemInstance is ShieldInstance shieldInstance)
            {
                equippedSlotIndex = equipmentDatabase.GetEquippedShieldSlot(shieldInstance);
            }
            else if (item is Arrow && itemInstance is ArrowInstance arrowInstance)
            {
                equippedSlotIndex = equipmentDatabase.GetEquippedArrowsSlot(arrowInstance);
            }
            else if (item is Spell && itemInstance is SpellInstance spellInstance)
            {
                equippedSlotIndex = equipmentDatabase.GetEquippedSpellSlot(spellInstance);
            }
            else if (item is Accessory && itemInstance is AccessoryInstance accessoryInstance)
            {
                equippedSlotIndex = equipmentDatabase.GetEquippedAccessoriesSlot(accessoryInstance);
            }
            else if (item is Consumable && itemInstance is ConsumableInstance consumableInstance)
            {
                equippedSlotIndex = equipmentDatabase.GetEquippedConsumablesSlot(consumableInstance);
            }

            if (equippedSlotIndex >= 0 && equippedSlotIndex != slotIndexToEquip)
            {
                return false;
            }

            return true;
        }

        void PopulateScrollView<ItemType>(bool showOnlyKeyItems, int slotIndex, bool isShieldSlot) where ItemType : Item
        {
            this.itemsScrollView.Clear();

            var query = inventoryDatabase.ownedItems
                .Where(item =>
                {
                    if (isShieldSlot && item is WeaponInstance)
                    {
                        return true;
                    }

                    return ShouldShowItem<ItemType>(item, slotIndex, showOnlyKeyItems);
                });

            Dictionary<Item, int> stackableItems = new();
            Dictionary<Item, int> stackableItemsPositionInScrollView = new();

            for (int i = 0; i < query.Count(); i++)
            {
                var itemInstance = query.ElementAt(i);
                var item = itemInstance.GetItem();

                bool isEquipped = IsItemEquipped(itemInstance, slotIndex, isShieldSlot);

                var instance = itemButtonPrefab.CloneTree();
                instance.Q<VisualElement>("Sprite").style.backgroundImage = new StyleBackground(item.sprite);

                instance.Q<VisualElement>("CardSprite").style.display = item is Card ? DisplayStyle.Flex : DisplayStyle.None;

                instance.Q<VisualElement>("Sprite").style.backgroundImage = new StyleBackground(item.sprite);
                var itemName = instance.Q<Label>("ItemName");
                itemName.text = item.GetName();

                if (item is Consumable || item is Arrow || showOnlyKeyItems)
                {
                    if (stackableItems.ContainsKey(item))
                    {
                        stackableItems[item]++;
                    }
                    else
                    {
                        stackableItems.Add(item, 1);
                    }
                }

                if (isEquipped)
                {
                    itemName.text += " " + LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", "(Equipped)");
                }

                var equipmentColorIndicator = GetEquipmentColorIndicator(itemInstance);
                if (equipmentColorIndicator == Color.black)
                {
                    instance.Q<VisualElement>("Indicator").style.display = DisplayStyle.None;
                }
                else
                {
                    instance.Q<VisualElement>("Indicator").style.backgroundColor = GetEquipmentColorIndicator(itemInstance);
                    instance.Q<VisualElement>("Indicator").style.display = DisplayStyle.Flex;
                }

                var btn = instance.Q<Button>("EquipButton");

                int index = i;
                btn.clicked += () =>
                {
                    lastScrollElementIndex = index;

                    soundbank.PlaySound(soundbank.uiEquip);

                    HandleButtonClick(itemInstance, isEquipped, slotIndex, isShieldSlot, out bool ignoreRerender);

                    if (!ignoreRerender)
                    {
                        ReturnToEquipmentSlots();
                    }
                };

                void ShowTooltipAndStats(ItemInstance itemInstance)
                {
                    itemTooltip.gameObject.SetActive(true);
                    itemTooltip.PrepareTooltipForItem(itemInstance);
                    itemTooltip.DisplayTooltip(btn);

                    playerStatsAndAttributesUI.DrawStats(itemInstance);
                }

                void HideTooltipAndClearStats()
                {
                    itemTooltip.gameObject.SetActive(false);
                    playerStatsAndAttributesUI.DrawStats(null);
                }

                btn.RegisterCallback<MouseEnterEvent>(ev =>
                {
                    itemsScrollView.ScrollTo(instance);
                    ShowTooltipAndStats(itemInstance);
                });
                btn.RegisterCallback<FocusInEvent>(ev =>
                {
                    itemsScrollView.ScrollTo(instance);
                    ShowTooltipAndStats(itemInstance);
                });
                btn.RegisterCallback<MouseOutEvent>(ev =>
                {
                    HideTooltipAndClearStats();
                });
                btn.RegisterCallback<FocusOutEvent>(ev =>
                {
                    HideTooltipAndClearStats();
                });

                instance.RegisterCallback<KeyDownEvent>(ev =>
                {
                    if (inputs.jump && item is Consumable)
                    {
                        inputs.jump = false;

                        if (playerStatsDatabase.currentHealth <= 0)
                        {
                            return;
                        }

                        playerManager.playerInventory.PrepareItemForConsuming(item as Consumable);
                        menuManager.CloseMenu();
                    }
                });

                if (stackableItemsPositionInScrollView.ContainsKey(item))
                {
                    this.itemsScrollView
                        .ElementAt(stackableItemsPositionInScrollView[item])
                        .Q<Label>("ItemName").text = item.GetName() + $" ({stackableItems[item]})";
                }
                else
                {
                    this.itemsScrollView.Add(instance);

                    if (stackableItems.ContainsKey(item) && !stackableItemsPositionInScrollView.ContainsKey(item))
                    {
                        stackableItemsPositionInScrollView.Add(item, i);
                    }
                }
            }

            Invoke(nameof(GiveFocus), 0f);
        }

        bool HandleButtonClick(ItemInstance itemInstance, bool isEquipped, int slotIndex, bool isShieldSlot, out bool ignoreRerender)
        {
            ignoreRerender = false;

            Item item = itemInstance.GetItem();

            if (item is Weapon weapon)
            {
                if (!isEquipped)
                {
                    if (!weapon.AreRequirementsMet(playerManager.statsBonusController))
                    {
                        notificationManager.ShowNotification(LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", "Can't equip weapon. Requirements are not met."), notificationManager.systemError);
                        ignoreRerender = true;
                    }
                    else
                    {
                        if (playerManager.statsBonusController.ignoreWeaponRequirements)
                        {
                            playerManager.statsBonusController.SetIgnoreNextWeaponToEquipRequirements(false);
                        }

                        if (isShieldSlot)
                        {
                            playerManager.playerWeaponsManager.EquipSecondaryWeapon(itemInstance as WeaponInstance, slotIndex);
                        }
                        else
                        {
                            playerManager.playerWeaponsManager.EquipWeapon(itemInstance as WeaponInstance, slotIndex);
                        }
                    }
                }
                else
                {
                    if (isShieldSlot)
                    {
                        playerManager.playerWeaponsManager.UnequipSecondaryWeapon(slotIndex);
                    }
                    else
                    {
                        playerManager.playerWeaponsManager.UnequipWeapon(slotIndex);
                    }
                }
            }
            else if (itemInstance is ShieldInstance shieldInstance)
            {
                if (!isEquipped)
                {
                    playerManager.playerWeaponsManager.EquipShield(shieldInstance, slotIndex);
                }
                else
                {
                    playerManager.playerWeaponsManager.UnequipShield(slotIndex);
                }
            }
            else if (itemInstance is HelmetInstance helmetInstance)
            {
                if (!isEquipped)
                {
                    playerManager.equipmentGraphicsHandler.EquipHelmet(helmetInstance);
                }
                else
                {
                    playerManager.equipmentGraphicsHandler.UnequipHelmet();
                }
            }
            else if (itemInstance is ArmorInstance armorInstance)
            {
                if (!isEquipped)
                {
                    playerManager.equipmentGraphicsHandler.EquipArmor(armorInstance);
                }
                else
                {
                    playerManager.equipmentGraphicsHandler.UnequipArmor();
                }
            }
            else if (itemInstance is GauntletInstance gauntletInstance)
            {
                if (!isEquipped)
                {
                    playerManager.equipmentGraphicsHandler.EquipGauntlet(gauntletInstance);
                }
                else
                {
                    playerManager.equipmentGraphicsHandler.UnequipGauntlet();
                }
            }
            else if (itemInstance is LegwearInstance legwearInstance)
            {
                if (!isEquipped)
                {
                    playerManager.equipmentGraphicsHandler.EquipLegwear(legwearInstance);
                }
                else
                {
                    playerManager.equipmentGraphicsHandler.UnequipLegwear();
                }
            }
            else if (itemInstance is AccessoryInstance accessoryInstance)
            {
                if (!isEquipped)
                {
                    playerManager.equipmentGraphicsHandler.EquipAccessory(accessoryInstance, slotIndex);
                }
                else
                {
                    playerManager.equipmentGraphicsHandler.UnequipAccessory(slotIndex);
                }
            }
            else if (itemInstance is ArrowInstance arrowInstance)
            {
                if (!isEquipped)
                {
                    equipmentDatabase.EquipArrow(arrowInstance, slotIndex);
                }
                else
                {
                    equipmentDatabase.UnequipArrow(slotIndex);
                }
            }
            else if (itemInstance is ConsumableInstance consumableInstance)
            {
                if (!isEquipped)
                {
                    equipmentDatabase.EquipConsumable(consumableInstance, slotIndex);
                }
                else
                {
                    equipmentDatabase.UnequipConsumable(slotIndex);
                }
            }
            else if (itemInstance is SpellInstance spellInstance)
            {
                if (!isEquipped)
                {
                    if (!spellInstance.GetItem()?.AreRequirementsMet(playerManager.statsBonusController) ?? false)
                    {
                        notificationManager.ShowNotification(
                            LocalizationSettings.StringDatabase.GetLocalizedString("Glossary", "Can not equip spell. Requirements not met!"), notificationManager.systemError);
                        ignoreRerender = true;
                    }
                    else
                    {
                        equipmentDatabase.EquipSpell(spellInstance, slotIndex);
                    }
                }
                else
                {
                    equipmentDatabase.UnequipSpell(slotIndex);
                }
            }

            return true;
        }

        void GiveFocus()
        {
            if (lastScrollElementIndex == -1)
            {
                returnButton.Focus();
            }
            else
            {
                UIUtils.ScrollToLastPosition(
                    lastScrollElementIndex,
                    itemsScrollView,
                    () =>
                    {
                        lastScrollElementIndex = -1;
                    }
                );
            }
        }

        public Color GetEquipmentColorIndicator<T>(T itemInstance) where T : ItemInstance
        {
            bool shouldReturn = false;
            int value = 0;
            if (itemInstance is WeaponInstance weaponInstance)
            {
                value = playerManager.attackStatManager.CompareWeapon(weaponInstance);
                shouldReturn = true;
            }
            else if (itemInstance is HelmetInstance helmetInstance)
            {
                value = playerManager.defenseStatManager.CompareHelmet(helmetInstance);
                shouldReturn = true;
            }
            else if (itemInstance is ArmorInstance armorInstance)
            {
                value = playerManager.defenseStatManager.CompareArmor(armorInstance);
                shouldReturn = true;
            }
            else if (itemInstance is GauntletInstance gauntletInstance)
            {
                value = playerManager.defenseStatManager.CompareGauntlet(gauntletInstance);
                shouldReturn = true;
            }
            else if (itemInstance is Legwear legwearInstance)
            {
                value = playerManager.defenseStatManager.CompareLegwear(legwearInstance);
                shouldReturn = true;
            }

            if (shouldReturn)
            {
                if (value > 0) return Color.green;
                else if (value == 0) return Color.yellow;
                else if (value < 0) return Color.red;
            }

            return Color.black;
        }

        public bool IsActive() => this.isActiveAndEnabled;

        public void Close() => ReturnToEquipmentSlots();

    }
}
