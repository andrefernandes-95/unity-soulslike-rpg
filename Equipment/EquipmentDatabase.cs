
namespace AF
{
    using UnityEditor;
    using UnityEngine;
    using TigerForge;
    using AF.Events;
    using System.Linq;
    using AF.Inventory;
    using System;
    using CI.QuickSave;

    [CreateAssetMenu(fileName = "Equipment Database", menuName = "System/New Equipment Database", order = 0)]
    public class EquipmentDatabase : ScriptableObject
    {
        [Header("Offensive Gear")]
        public WeaponInstance[] weapons = new WeaponInstance[3]; // Fixed size array for weapons
        public WeaponInstance[] secondaryWeapons = new WeaponInstance[3]; // Fixed size array for weapons

        public ShieldInstance[] shields = new ShieldInstance[3]; // Fixed size array for shields

        public ArrowInstance[] arrows = new ArrowInstance[2];

        public SpellInstance[] spells = new SpellInstance[5];

        public ConsumableInstance[] consumables = new ConsumableInstance[10];

        [Header("Defensive Gear")]
        public HelmetInstance helmet;
        public ArmorInstance armor;
        public GauntletInstance gauntlet;
        public LegwearInstance legwear;

        [Header("Accessories")]
        public AccessoryInstance[] accessories = new AccessoryInstance[4];

        public int currentWeaponIndex, currentShieldIndex, currentConsumableIndex, currentSpellIndex, currentArrowIndex = 0;


        [Header("Flags")]
        public bool isTwoHanding = false;
        public bool isUsingShield = false;

        [Header("Fallbacks")]
        public WeaponInstance unarmedWeaponInstance;
        public Weapon kickWeapon;

        [Header("Databases")]
        public InventoryDatabase inventoryDatabase;

        public bool shouldClearOnExit = false;

#if UNITY_EDITOR
        private void OnEnable()
        {
            // No need to populate the list; it's serialized directly
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode && shouldClearOnExit)
            {
                // Clear the list when exiting play mode
                Clear();
            }
        }
#endif

        public void Clear()
        {
            currentWeaponIndex = 0;
            currentShieldIndex = 0;
            currentConsumableIndex = 0;
            currentSpellIndex = 0;
            currentArrowIndex = 0;

            for (int i = 0; i < weapons.Length; i++)
            {
                weapons[i].Clear();
            }

            for (int i = 0; i < secondaryWeapons.Length; i++)
            {
                secondaryWeapons[i].Clear();
            }

            for (int i = 0; i < shields.Length; i++)
            {
                shields[i].Clear();
            }

            for (int i = 0; i < spells.Length; i++)
            {
                spells[i].Clear();
            }

            for (int i = 0; i < accessories.Length; i++)
            {
                accessories[i].Clear();
            }

            for (int i = 0; i < consumables.Length; i++)
            {
                consumables[i].Clear();
            }
            for (int i = 0; i < arrows.Length; i++)
            {
                arrows[i].Clear();
            }

            helmet.Clear();
            armor.Clear();
            gauntlet.Clear();
            legwear.Clear();
        }
        public void SwitchToNextWeapon()
        {
            currentWeaponIndex++;

            if (currentWeaponIndex >= weapons.Length)
            {
                currentWeaponIndex = 0;
            }

            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }

        public void EquipWeapon(WeaponInstance weaponInstanceToEquip, int slotIndex)
        {
            if (weaponInstanceToEquip == null)
            {
                Debug.LogWarning("WeaponInstance to equip can not be null");
                return;
            }

            weapons[slotIndex] = weaponInstanceToEquip.Clone();

            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }
        public void UnequipWeapon(int slotIndex)
        {
            weapons[slotIndex].Clear();

            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }
        public void EquipSecondaryWeapon(WeaponInstance weaponInstanceToEquip, int slotIndex)
        {
            if (weaponInstanceToEquip == null)
            {
                Debug.LogWarning("WeaponInstance to equip can not be null");
                return;
            }

            secondaryWeapons[slotIndex] = weaponInstanceToEquip.Clone();
            shields[slotIndex].Clear();
            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }
        public void UnequipSecondaryWeapon(int slotIndex)
        {
            secondaryWeapons[slotIndex].Clear();

            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }

        public void SwitchToNextShield()
        {
            currentShieldIndex++;

            if (currentShieldIndex >= shields.Length)
            {
                currentShieldIndex = 0;
            }

            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
            EventManager.EmitEvent(EventMessages.ON_SHIELD_EQUIPMENT_CHANGED);
        }

        public void EquipShield(ShieldInstance shieldInstanceToEquip, int slotIndex)
        {
            if (shieldInstanceToEquip == null)
            {
                Debug.LogWarning("ShieldInstance to equip can not be null");
                return;
            }

            shields[slotIndex] = shieldInstanceToEquip.Clone();
            secondaryWeapons[slotIndex].Clear();
            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
            EventManager.EmitEvent(EventMessages.ON_SHIELD_EQUIPMENT_CHANGED);
        }
        public void UnequipShield(int slotIndex)
        {
            shields[slotIndex].Clear();
            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
            EventManager.EmitEvent(EventMessages.ON_SHIELD_EQUIPMENT_CHANGED);
        }

        public void SwitchToNextArrow()
        {
            currentArrowIndex = UpdateIndex(currentArrowIndex, arrows);

            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }

        public void EquipArrow(ArrowInstance arrowInstanceToEquip, int slotIndex)
        {
            if (arrowInstanceToEquip == null)
            {
                Debug.LogWarning("ArrowInstance to equip can not be null");
                return;
            }

            arrows[slotIndex] = arrowInstanceToEquip.Clone();
            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }
        public void UnequipArrow(int slotIndex)
        {
            arrows[slotIndex].Clear();
            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }

        public void SwitchToNextSpell()
        {
            currentSpellIndex = UpdateIndex(currentSpellIndex, spells);

            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }

        public void EquipSpell(SpellInstance spellInstanceToEquip, int slotIndex)
        {
            if (spellInstanceToEquip == null)
            {
                Debug.LogWarning("SpellInstance to equip can not be null");
                return;
            }

            spells[slotIndex] = spellInstanceToEquip.Clone();

            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }
        public void UnequipSpell(int slotIndex)
        {
            spells[slotIndex].Clear();

            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }

        public void SwitchToNextConsumable()
        {
            currentConsumableIndex = UpdateIndex(currentConsumableIndex, consumables);
        }

        public void EquipConsumable(ConsumableInstance consumableInstanceToEquip, int slotIndex)
        {
            if (consumableInstanceToEquip == null)
            {
                Debug.LogWarning("ConsumableInstance to equip can not be null");
                return;
            }

            consumables[slotIndex] = consumableInstanceToEquip.Clone();

            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }

        public void UnequipConsumable(int slotIndex)
        {
            consumables[slotIndex].Clear();

            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }

        public int UpdateIndex<T>(int index, T[] targetList) where T : ItemInstance
        {
            index++;

            int nextIndexWithEquippedSlot = Array.FindIndex(targetList, index, equippedItem => equippedItem.Exists());

            if (nextIndexWithEquippedSlot != -1)
            {
                index = nextIndexWithEquippedSlot;
            }
            else
            {
                int fallbackIndex = Array.FindIndex(targetList, 0, equippedItem => equippedItem.Exists());
                index = fallbackIndex == -1 ? 0 : fallbackIndex;
            }

            return index;
        }

        public void EquipHelmet(HelmetInstance helmetInstanceToEquip)
        {
            if (helmetInstanceToEquip == null)
            {
                Debug.LogWarning("HelmetInstance to equip can not be null");
                return;
            }

            helmet = helmetInstanceToEquip.Clone();
            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }
        public void UnequipHelmet()
        {
            helmet.Clear();

            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }

        public void EquipArmor(ArmorInstance armorInstanceToEquip)
        {
            if (armorInstanceToEquip == null)
            {
                Debug.LogWarning("ArmorInstance to equip can not be null");
                return;
            }

            armor = armorInstanceToEquip.Clone();
            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }

        public void UnequipArmor()
        {
            armor.Clear();
            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }

        public void EquipGauntlet(GauntletInstance gauntletInstanceToEquip)
        {
            if (gauntletInstanceToEquip == null)
            {
                Debug.LogWarning("GauntletInstance to equip can not be null");
                return;
            }

            gauntlet = gauntletInstanceToEquip.Clone();
            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }

        public void UnequipGauntlet()
        {
            gauntlet.Clear();
            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }

        public void EquipLegwear(LegwearInstance legwearInstanceToEquip)
        {
            if (legwearInstanceToEquip == null)
            {
                Debug.LogWarning("LegwearInstance to equip can not be null");
                return;
            }

            legwear = legwearInstanceToEquip.Clone();
            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }

        public void UnequipLegwear()
        {
            legwear.Clear();
            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }

        public void EquipAccessory(AccessoryInstance accessoryInstanceToEquip, int slotIndex)
        {
            if (accessoryInstanceToEquip == null)
            {
                Debug.LogWarning("AccessoryInstance to equip can not be null");
                return;
            }

            accessories[slotIndex] = accessoryInstanceToEquip.Clone();
            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }

        public void UnequipAccessory(int slotIndex)
        {
            accessories[slotIndex].Clear();
            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }

        public WeaponInstance GetCurrentWeapon() => weapons[currentWeaponIndex];
        public WeaponInstance GetCurrentSecondaryWeapon() => secondaryWeapons[currentShieldIndex];
        public ShieldInstance GetCurrentShield() => shields[currentShieldIndex];
        public SpellInstance GetCurrentSpell() => spells[currentSpellIndex];

        public ArrowInstance GetCurrentArrow() => arrows[currentArrowIndex];

        public ConsumableInstance GetCurrentConsumable() => consumables[currentConsumableIndex];

        public bool IsAccessoryEquiped(Accessory accessory) =>
            accessories.Any(equippedAccessory => equippedAccessory.HasItem(accessory));

        public bool IsBowEquipped()
        {
            WeaponInstance currentWeaponInstance = GetCurrentWeapon();
            if (currentWeaponInstance.IsEmpty())
            {
                return false;
            }

            return currentWeaponInstance.GetItem().weaponDamage.GetWeaponUpgradeLevel(currentWeaponInstance.level).damage.weaponAttackType == WeaponAttackType.Range;
        }

        public bool IsStaffEquipped()
        {
            WeaponInstance currentWeaponInstance = GetCurrentWeapon();

            if (currentWeaponInstance.IsEmpty())
            {
                return false;
            }

            return currentWeaponInstance.GetItem().weaponDamage.GetWeaponUpgradeLevel(currentWeaponInstance.level).damage.weaponAttackType == WeaponAttackType.Staff;
        }

        public bool IsUnarmed() => GetCurrentWeapon().IsEmpty();

        public bool IsPowerStancing()
        {
            if (!IsDualWielding())
            {
                return false;
            }

            return GetCurrentSecondaryWeapon().GetItem().weaponDamage == GetCurrentWeapon().GetItem().weaponDamage;
        }

        public bool HasEnoughCurrentArrows()
        {
            ArrowInstance currentArrow = GetCurrentArrow();

            if (currentArrow.IsEmpty())
            {
                return false;
            }

            return inventoryDatabase.GetItemAmount(currentArrow.GetItem()) > 0;
        }

        public bool IsPlayerNaked() => helmet.IsEmpty() && armor.IsEmpty() && legwear.IsEmpty() && gauntlet.IsEmpty();

        public bool IsDualWielding() => GetCurrentWeapon().Exists() && GetCurrentSecondaryWeapon().Exists();

        public int GetEquippedWeaponSlot(ItemInstance weapon) => Array.FindIndex(weapons, equippedWeapon =>
            equippedWeapon.Exists() && equippedWeapon.IsId(weapon.GetId()));

        public int GetEquippedSecondaryWeaponSlot(ItemInstance weapon) => Array.FindIndex(secondaryWeapons, equippedWeapon =>
            equippedWeapon.Exists() && equippedWeapon.IsId(weapon.GetId()));

        public int GetEquippedShieldSlot(ItemInstance shield) => Array.FindIndex(shields, equippedShield =>
            equippedShield.Exists() && equippedShield.IsId(shield.GetId()));

        public int GetEquippedArrowsSlot(ItemInstance arrow) => Array.FindIndex(arrows, equippedArrow =>
            equippedArrow.Exists() && equippedArrow.IsId(arrow.GetId()));

        public int GetEquippedSpellSlot(ItemInstance spell) => Array.FindIndex(spells, equippedSpell =>
            equippedSpell.Exists() && equippedSpell.IsId(spell.GetId()));

        public int GetEquippedAccessoriesSlot(ItemInstance accessory) => Array.FindIndex(accessories, equippedAccessory =>
            equippedAccessory.Exists() && equippedAccessory.IsId(accessory.GetId()));

        public int GetEquippedConsumablesSlot(ItemInstance consumable) => Array.FindIndex(consumables, equippedConsumable =>
            equippedConsumable.Exists() && equippedConsumable.IsId(consumable.GetId()));

        public void UnequipItem(ItemInstance item)
        {
            int equippedIndex = GetEquippedWeaponSlot(item);
            if (equippedIndex != -1)
            {
                weapons[equippedIndex].Clear();
                EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
                return;
            }

            equippedIndex = GetEquippedShieldSlot(item);
            if (equippedIndex != -1)
            {
                shields[equippedIndex].Clear();
                EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
                return;
            }

            equippedIndex = GetEquippedArrowsSlot(item);
            if (equippedIndex != -1)
            {
                arrows[equippedIndex].Clear();
                EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
                return;
            }

            equippedIndex = GetEquippedSpellSlot(item);
            if (equippedIndex != -1)
            {
                spells[equippedIndex].Clear();
                EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
                return;
            }

            equippedIndex = GetEquippedAccessoriesSlot(item);
            if (equippedIndex != -1)
            {
                accessories[equippedIndex].Clear();
                EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
                return;
            }

            equippedIndex = GetEquippedConsumablesSlot(item);
            if (equippedIndex != -1)
            {
                consumables[equippedIndex].Clear();
                EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
                return;
            }

            if (helmet.Exists() && helmet.IsId(item.GetId()))
            {
                helmet.Clear();
                EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
                return;
            }

            if (armor.Exists() && armor.IsId(item.GetId()))
            {
                armor.Clear();
                EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
                return;
            }

            if (gauntlet.Exists() && gauntlet.IsId(item.GetId()))
            {
                gauntlet.Clear();
                EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
                return;
            }

            if (legwear.Exists() && legwear.IsId(item.GetId()))
            {
                legwear.Clear();
                EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
                return;
            }

            // Item not found equipped
            Debug.LogWarning($"UnequipItem: Item with Name: {item.GetItem()?.name} and Id: {item.GetId()} not found equipped");
        }


        public void SetIsTwoHanding(bool value)
        {
            isTwoHanding = value;

            EventManager.EmitEvent(EventMessages.ON_TWO_HANDING_CHANGED);
        }

        public bool ShouldBlockShieldSlot()
        {
            if (IsBowEquipped())
            {
                return true;
            }


            if (IsStaffEquipped())
            {
                return GetCurrentSecondaryWeapon() == null;
            }

            return false;
        }

        public bool CanAim()
        {
            if (IsBowEquipped())
            {
                return true;
            }

            if (IsStaffEquipped())
            {
                return isTwoHanding || !GetCurrentSecondaryWeapon().Exists();
            }

            return false;
        }

        public bool CanBlock()
        {
            if (IsBowEquipped())
            {
                return false;
            }

            // If using a staff, only block if a shield is on the left hand
            if (IsStaffEquipped())
            {
                return GetCurrentShield() != null;
            }

            return true;
        }

        public bool IsEquipped(ArmorBase armorBase)
        {
            if (armorBase == null)
            {
                return false;
            }

            if (armorBase is Helmet)
            {
                return helmet?.HasItem(armorBase) ?? false;
            }
            else if (armorBase is Gauntlet)
            {
                return gauntlet?.HasItem(armorBase) ?? false;
            }
            else if (armorBase is Armor)
            {
                return armor?.HasItem(armorBase) ?? false;
            }
            else if (armorBase is Legwear)
            {
                return legwear?.HasItem(armorBase) ?? false;
            }
            else if (armorBase is Accessory)
            {
                return accessories.Any(accessory => accessory?.HasItem(armorBase) ?? false);
            }

            return false;
        }

        public Weapon GetUnarmedWeapon()
        {
            if (unarmedWeaponInstance.Exists())
            {
                return unarmedWeaponInstance.GetItem();
            }

            return null;
        }

        public void LoadEquipmentFromSaveFile(QuickSaveReader quickSaveReader)
        {

            quickSaveReader.TryRead<int>("currentWeaponIndex", out int currentWeaponIndex);
            this.currentWeaponIndex = currentWeaponIndex;

            quickSaveReader.TryRead<int>("currentShieldIndex", out int currentShieldIndex);
            this.currentShieldIndex = currentShieldIndex;

            quickSaveReader.TryRead<int>("currentArrowIndex", out int currentArrowIndex);
            this.currentArrowIndex = currentArrowIndex;

            quickSaveReader.TryRead<int>("currentSpellIndex", out int currentSpellIndex);
            this.currentSpellIndex = currentSpellIndex;

            quickSaveReader.TryRead<int>("currentConsumableIndex", out int currentConsumableIndex);
            this.currentConsumableIndex = currentConsumableIndex;

            quickSaveReader.TryRead<string[]>("weapons", out string[] weapons);
            if (weapons != null && weapons.Length > 0)
            {
                for (int idx = 0; idx < weapons.Length; idx++)
                {
                    string weaponId = weapons[idx];

                    if (!string.IsNullOrEmpty(weaponId))
                    {
                        if (inventoryDatabase.FindItemById(weaponId) is WeaponInstance weaponInstance)
                        {
                            EquipWeapon(weaponInstance, idx);
                        }
                    }
                }
            }

            // Try to read shields
            quickSaveReader.TryRead<string[]>("shields", out string[] shields);
            if (shields != null && shields.Length > 0)
            {
                for (int idx = 0; idx < shields.Length; idx++)
                {
                    string shieldId = shields[idx];

                    if (!string.IsNullOrEmpty(shieldId))
                    {
                        if (inventoryDatabase.FindItemById(shieldId) is ShieldInstance shieldInstance)
                        {
                            EquipShield(shieldInstance, idx);
                        }
                    }
                }
            }

            // Try to read arrows
            quickSaveReader.TryRead<string[]>("arrows", out string[] arrows);
            if (arrows != null && arrows.Length > 0)
            {
                for (int idx = 0; idx < arrows.Length; idx++)
                {
                    string arrowId = arrows[idx];

                    if (!string.IsNullOrEmpty(arrowId))
                    {
                        if (inventoryDatabase.FindItemById(arrowId) is ArrowInstance arrowInstance)
                        {
                            EquipArrow(arrowInstance, idx);
                        }
                    }
                }
            }

            // Try to read spells
            quickSaveReader.TryRead<string[]>("spells", out string[] spells);
            if (spells != null && spells.Length > 0)
            {
                for (int idx = 0; idx < spells.Length; idx++)
                {
                    string spellId = spells[idx];

                    if (!string.IsNullOrEmpty(spellId))
                    {
                        if (inventoryDatabase.FindItemById(spellId) is SpellInstance spellInstance)
                        {
                            EquipSpell(spellInstance, idx);
                        }
                    }
                }
            }

            // Try to read accessories
            quickSaveReader.TryRead<string[]>("accessories", out string[] accessories);
            if (accessories != null && accessories.Length > 0)
            {
                for (int idx = 0; idx < accessories.Length; idx++)
                {
                    string accessoryId = accessories[idx];

                    if (!string.IsNullOrEmpty(accessoryId))
                    {
                        if (inventoryDatabase.FindItemById(accessoryId) is AccessoryInstance accessoryInstance)
                        {
                            EquipAccessory(accessoryInstance, idx);
                        }
                    }
                }
            }

            // Try to read consumables
            quickSaveReader.TryRead<string[]>("consumables", out string[] consumables);
            if (consumables != null && consumables.Length > 0)
            {
                for (int idx = 0; idx < consumables.Length; idx++)
                {
                    string consumableId = consumables[idx];

                    if (!string.IsNullOrEmpty(consumableId))
                    {
                        if (inventoryDatabase.FindItemById(consumableId) is ConsumableInstance consumableInstance)
                        {
                            EquipConsumable(consumableInstance, idx);
                        }
                    }
                }
            }

            // Try to read helmet
            quickSaveReader.TryRead<string>("helmet", out string helmetId);
            if (!string.IsNullOrEmpty(helmetId))
            {
                if (inventoryDatabase.FindItemById(helmetId) is HelmetInstance helmetInstance)
                {
                    EquipHelmet(helmetInstance);
                }
            }
            else
            {
                UnequipHelmet();
            }

            // Try to read armor
            quickSaveReader.TryRead<string>("armor", out string armorId);
            if (!string.IsNullOrEmpty(armorId))
            {
                if (inventoryDatabase.FindItemById(armorId) is ArmorInstance armorInstance)
                {
                    EquipArmor(armorInstance);
                }
            }
            else
            {
                UnequipArmor();
            }

            // Try to read gauntlet
            quickSaveReader.TryRead<string>("gauntlet", out string gauntletId);
            if (!string.IsNullOrEmpty(gauntletId))
            {
                if (inventoryDatabase.FindItemById(gauntletId) is GauntletInstance gauntletInstance)
                {
                    EquipGauntlet(gauntletInstance);
                }
            }
            else
            {
                UnequipGauntlet();
            }

            // Try to read legwear
            quickSaveReader.TryRead<string>("legwear", out string legwearId);
            if (!string.IsNullOrEmpty(legwearId))
            {
                LegwearInstance legwearInstance = inventoryDatabase.FindItemById(legwearId) as LegwearInstance;

                if (legwearInstance != null)
                {
                    EquipLegwear(legwearInstance);
                }
            }
            else
            {
                UnequipLegwear();
            }

            quickSaveReader.TryRead<bool>("isTwoHanding", out bool isTwoHanding);
            this.isTwoHanding = isTwoHanding;
        }
    }
}
