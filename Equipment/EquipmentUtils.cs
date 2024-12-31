
namespace AF
{
    using System;
    using System.Linq;
    using AF.Stats;
    using AF.StatusEffects;

    public static class EquipmentUtils
    {

        public enum AttributeType { VITALITY, ENDURANCE, DEXTERITY, STRENGTH, INTELLIGENCE, REPUTATION }
        public enum AccessoryAttributeType { HEALTH_BONUS, STAMINA_BONUS, MANA_BONUS }

        public static int GetPoiseChangeFromItem(int playerMaxPoiseHits, EquipmentDatabase equipmentDatabase, Item itemToEquip)
        {
            int currentPoise = playerMaxPoiseHits;

            // Get the equipped poise bonus based on the type of armor
            int equippedPoiseBonus = itemToEquip switch
            {
                Helmet _ => equipmentDatabase.helmet.GetItem()?.poiseBonus ?? 0,
                Armor _ => equipmentDatabase.armor.GetItem()?.poiseBonus ?? 0,
                Gauntlet _ => equipmentDatabase.gauntlet.GetItem()?.poiseBonus ?? 0,
                Legwear _ => equipmentDatabase.legwear.GetItem()?.poiseBonus ?? 0,
                _ => 0
            };

            // Subtract equipped poise bonus from current poise, ensuring it doesn't go negative
            currentPoise = Math.Max(0, currentPoise - equippedPoiseBonus);

            // Get the new poise bonus from the item (armorBase)
            int itemPoiseBonus = itemToEquip switch
            {
                Helmet helmet => helmet.poiseBonus,
                Armor armor => armor.poiseBonus,
                Gauntlet gauntlet => gauntlet.poiseBonus,
                Legwear legwear => legwear.poiseBonus,
                Accessory accessory => equipmentDatabase.IsAccessoryEquiped(accessory) ? 0 : accessory.poiseBonus,
                _ => 0
            };

            // Return the updated poise (current poise + new item poise bonus)
            return currentPoise + itemPoiseBonus;
        }

        public static int GetPostureChangeFromItem(int playerMaxPostureDamage, EquipmentDatabase equipmentDatabase, Item itemToEquip)
        {
            int currentPosture = playerMaxPostureDamage;

            // Get the equipped posture bonus based on armor type
            int equippedPostureBonus = itemToEquip switch
            {
                Helmet _ => equipmentDatabase.helmet?.GetItem()?.postureBonus ?? 0,
                Armor _ => equipmentDatabase.armor?.GetItem()?.postureBonus ?? 0,
                Gauntlet _ => equipmentDatabase.gauntlet?.GetItem()?.postureBonus ?? 0,
                Legwear _ => equipmentDatabase.legwear?.GetItem()?.postureBonus ?? 0,
                _ => 0
            };

            // Subtract equipped posture bonus from current posture, ensure it's non-negative
            currentPosture = Math.Max(0, currentPosture - equippedPostureBonus);

            // Get the new posture bonus from the item (armorBase)
            int itemPostureBonus = itemToEquip switch
            {
                Helmet helmet => helmet.postureBonus,
                Armor armor => armor.postureBonus,
                Gauntlet gauntlet => gauntlet.postureBonus,
                Legwear legwear => legwear.postureBonus,
                Accessory accessory => equipmentDatabase.IsAccessoryEquiped(accessory) ? 0 : accessory.postureBonus,
                _ => 0
            };

            // Return the updated posture (current posture + new item posture bonus)
            return currentPosture + itemPostureBonus;
        }

        public static int GetElementalAttackForCurrentWeapon(WeaponInstance weaponInstance, WeaponElementType elementType, AttackStatManager attackStatManager)
        {
            if (weaponInstance == null || weaponInstance.IsEmpty()) return 0;


            if (elementType == WeaponElementType.None)
            {
                return 0;
            }

            return weaponInstance.GetItem().weaponDamage.GetWeaponAttack(
                    elementType,
                    attackStatManager.playerManager,
                    attackStatManager.playerManager.statsBonusController.GetCurrentStrength(),
                    attackStatManager.playerManager.statsBonusController.GetCurrentDexterity(),
                    attackStatManager.playerManager.statsBonusController.GetCurrentIntelligence(),
                    weaponInstance);
        }

        public static int GetElementalDefenseFromItem(ArmorBase armorBase, WeaponElementType weaponElementType, DefenseStatManager defenseStatManager, EquipmentDatabase equipmentDatabase)
        {
            int baseElementalDefense = weaponElementType switch
            {
                WeaponElementType.Fire => (int)defenseStatManager.GetFireDefense(),
                WeaponElementType.Frost => (int)defenseStatManager.GetFrostDefense(),
                WeaponElementType.Lightning => (int)defenseStatManager.GetLightningDefense(),
                WeaponElementType.Magic => (int)defenseStatManager.GetMagicDefense(),
                WeaponElementType.Darkness => (int)defenseStatManager.GetDarknessDefense(),
                WeaponElementType.Water => (int)defenseStatManager.GetWaterDefense(),
                WeaponElementType.None => (int)defenseStatManager.GetDefenseAbsorption(),
                _ => 0
            };

            ArmorBase equippedArmor = armorBase switch
            {
                Helmet => equipmentDatabase.helmet.GetItem(),
                Armor => equipmentDatabase.armor.GetItem(),
                Gauntlet => equipmentDatabase.gauntlet.GetItem(),
                Legwear => equipmentDatabase.legwear.GetItem(),
                Accessory accessory when !equipmentDatabase.IsAccessoryEquiped(accessory) => accessory,
                _ => null
            };

            int currentDefenseFromItem = 0;
            if (equippedArmor != null)
            {
                currentDefenseFromItem = weaponElementType switch
                {
                    WeaponElementType.Fire => (int)equippedArmor.fireDefense,
                    WeaponElementType.Frost => (int)equippedArmor.frostDefense,
                    WeaponElementType.Lightning => (int)equippedArmor.lightningDefense,
                    WeaponElementType.Magic => (int)equippedArmor.magicDefense,
                    WeaponElementType.Darkness => (int)equippedArmor.darkDefense,
                    WeaponElementType.Water => (int)equippedArmor.waterDefense,
                    WeaponElementType.None => (int)equippedArmor.physicalDefense,
                    _ => 0
                };
            }

            int newValue = Math.Max(0, baseElementalDefense - currentDefenseFromItem);

            int newDefenseFromItem = equipmentDatabase.IsEquipped(armorBase) ? 0 : weaponElementType switch
            {
                WeaponElementType.Fire => (int)armorBase.fireDefense,
                WeaponElementType.Frost => (int)armorBase.frostDefense,
                WeaponElementType.Lightning => (int)armorBase.lightningDefense,
                WeaponElementType.Magic => (int)armorBase.magicDefense,
                WeaponElementType.Darkness => (int)armorBase.darkDefense,
                WeaponElementType.Water => (int)armorBase.waterDefense,
                WeaponElementType.None => (int)armorBase.physicalDefense,
                _ => 0
            };

            return newValue + newDefenseFromItem;
        }

        public static float GetEquipLoadFromItem(Item itemToEquip, float currentWeightPenalty, EquipmentDatabase equipmentDatabase)
        {
            // Define a function to retrieve the current speed penalty from an equipped item.
            Func<ItemInstance, float> GetSpeedPenalty = (item) =>
            {
                if (item == null)
                    return 0;

                if (item is AccessoryInstance accessory)
                    return accessory.GetItem()?.speedPenalty ?? 0;

                if (item is WeaponInstance weapon)
                    return weapon.GetItem()?.speedPenalty ?? 0;

                if (item is ArmorBaseInstance armor)
                    return armor.GetItem()?.speedPenalty ?? 0;

                return 0;
            };

            // Adjust the weight penalty by the currently equipped item based on type.
            switch (itemToEquip)
            {
                case Weapon weapon:
                    currentWeightPenalty -= GetSpeedPenalty(equipmentDatabase.GetCurrentWeapon());
                    return Math.Max(0, currentWeightPenalty) + weapon.speedPenalty;

                case Shield shield:
                    currentWeightPenalty -= GetSpeedPenalty(equipmentDatabase.GetCurrentShield());
                    return Math.Max(0, currentWeightPenalty) + shield.speedPenalty;

                case Helmet helmet:
                    currentWeightPenalty -= GetSpeedPenalty(equipmentDatabase.helmet);
                    return Math.Max(0, currentWeightPenalty) + helmet.speedPenalty;

                case Armor armor:
                    currentWeightPenalty -= GetSpeedPenalty(equipmentDatabase.armor);
                    return Math.Max(0, currentWeightPenalty) + armor.speedPenalty;

                case Gauntlet gauntlet:
                    currentWeightPenalty -= GetSpeedPenalty(equipmentDatabase.gauntlet);
                    return Math.Max(0, currentWeightPenalty) + gauntlet.speedPenalty;

                case Legwear legwear:
                    currentWeightPenalty -= GetSpeedPenalty(equipmentDatabase.legwear);
                    return Math.Max(0, currentWeightPenalty) + legwear.speedPenalty;

                case Accessory accessory:
                    // Sum speed penalties of all equipped accessories.
                    currentWeightPenalty -= equipmentDatabase.accessories.Sum(GetSpeedPenalty);
                    return Math.Max(0, currentWeightPenalty) + accessory.speedPenalty;

                default:
                    return 0f;
            }
        }

        public static int GetAttributeFromEquipment(ArmorBase armorBase, AttributeType attributeType, StatsBonusController playerStatsBonusController, EquipmentDatabase equipmentDatabase)
        {
            // Get current value based on attribute type
            int currentValue = attributeType switch
            {
                AttributeType.VITALITY => playerStatsBonusController.GetCurrentVitality(),
                AttributeType.ENDURANCE => playerStatsBonusController.GetCurrentEndurance(),
                AttributeType.STRENGTH => playerStatsBonusController.GetCurrentStrength(),
                AttributeType.DEXTERITY => playerStatsBonusController.GetCurrentDexterity(),
                AttributeType.INTELLIGENCE => playerStatsBonusController.GetCurrentIntelligence(),
                AttributeType.REPUTATION => playerStatsBonusController.GetCurrentReputation(),
                _ => 0 // Fallback for safety
            };

            // Determine bonus from the armor base and currently equipped item
            int bonusFromEquipment = 0;
            int valueFromCurrentEquipment = 0;

            // Retrieve bonus from armorBase
            if (!equipmentDatabase.IsEquipped(armorBase))
            {
                bonusFromEquipment = attributeType switch
                {
                    AttributeType.VITALITY => armorBase.vitalityBonus,
                    AttributeType.ENDURANCE => armorBase.enduranceBonus,
                    AttributeType.STRENGTH => armorBase.strengthBonus,
                    AttributeType.DEXTERITY => armorBase.dexterityBonus,
                    AttributeType.INTELLIGENCE => armorBase.intelligenceBonus,
                    AttributeType.REPUTATION => armorBase.reputationBonus,
                    _ => 0 // Fallback for safety
                };
            }

            // Check currently equipped items
            if (armorBase is Helmet && equipmentDatabase.helmet != null)
            {
                valueFromCurrentEquipment = equipmentDatabase.helmet switch
                {
                    HelmetInstance equippedHelmet => attributeType switch
                    {
                        AttributeType.VITALITY => equippedHelmet.GetItem()?.vitalityBonus ?? 0,
                        AttributeType.ENDURANCE => equippedHelmet.GetItem()?.enduranceBonus ?? 0,
                        AttributeType.STRENGTH => equippedHelmet.GetItem()?.strengthBonus ?? 0,
                        AttributeType.DEXTERITY => equippedHelmet.GetItem()?.dexterityBonus ?? 0,
                        AttributeType.INTELLIGENCE => equippedHelmet.GetItem()?.intelligenceBonus ?? 0,
                        AttributeType.REPUTATION => equippedHelmet.GetItem()?.reputationBonus ?? 0,
                        _ => 0
                    },
                    _ => 0
                };
            }
            else if (armorBase is Armor && equipmentDatabase.armor != null)
            {
                valueFromCurrentEquipment = equipmentDatabase.armor switch
                {
                    ArmorInstance equippedArmor => attributeType switch
                    {
                        AttributeType.VITALITY => equippedArmor.GetItem()?.vitalityBonus ?? 0,
                        AttributeType.ENDURANCE => equippedArmor.GetItem()?.enduranceBonus ?? 0,
                        AttributeType.STRENGTH => equippedArmor.GetItem()?.strengthBonus ?? 0,
                        AttributeType.DEXTERITY => equippedArmor.GetItem()?.dexterityBonus ?? 0,
                        AttributeType.INTELLIGENCE => equippedArmor.GetItem()?.intelligenceBonus ?? 0,
                        AttributeType.REPUTATION => equippedArmor.GetItem()?.reputationBonus ?? 0,
                        _ => 0
                    },
                    _ => 0
                };
            }
            else if (armorBase is Gauntlet && equipmentDatabase.gauntlet != null)
            {
                valueFromCurrentEquipment = equipmentDatabase.gauntlet switch
                {
                    GauntletInstance equippedGauntlet => attributeType switch
                    {
                        AttributeType.VITALITY => equippedGauntlet.GetItem()?.vitalityBonus ?? 0,
                        AttributeType.ENDURANCE => equippedGauntlet.GetItem()?.enduranceBonus ?? 0,
                        AttributeType.STRENGTH => equippedGauntlet.GetItem()?.strengthBonus ?? 0,
                        AttributeType.DEXTERITY => equippedGauntlet.GetItem()?.dexterityBonus ?? 0,
                        AttributeType.INTELLIGENCE => equippedGauntlet.GetItem()?.intelligenceBonus ?? 0,
                        AttributeType.REPUTATION => equippedGauntlet.GetItem()?.reputationBonus ?? 0,
                        _ => 0
                    },
                    _ => 0
                };
            }
            else if (armorBase is Legwear && equipmentDatabase.legwear != null)
            {
                valueFromCurrentEquipment = equipmentDatabase.legwear switch
                {
                    LegwearInstance equippedLegwear => attributeType switch
                    {
                        AttributeType.VITALITY => equippedLegwear.GetItem()?.vitalityBonus ?? 0,
                        AttributeType.ENDURANCE => equippedLegwear.GetItem()?.enduranceBonus ?? 0,
                        AttributeType.STRENGTH => equippedLegwear.GetItem()?.strengthBonus ?? 0,
                        AttributeType.DEXTERITY => equippedLegwear.GetItem()?.dexterityBonus ?? 0,
                        AttributeType.INTELLIGENCE => equippedLegwear.GetItem()?.intelligenceBonus ?? 0,
                        AttributeType.REPUTATION => equippedLegwear.GetItem()?.reputationBonus ?? 0,
                        _ => 0
                    },
                    _ => 0
                };
            }
            else if (armorBase is Accessory)
            {
                // Loop through each accessory in the accessories collection
                foreach (var equippedAccessory in equipmentDatabase.accessories)
                {
                    // Switch based on the specific type of attribute for the accessory
                    valueFromCurrentEquipment += attributeType switch
                    {
                        AttributeType.VITALITY => equippedAccessory?.GetItem()?.vitalityBonus ?? 0,
                        AttributeType.ENDURANCE => equippedAccessory?.GetItem()?.enduranceBonus ?? 0,
                        AttributeType.STRENGTH => equippedAccessory?.GetItem()?.strengthBonus ?? 0,
                        AttributeType.DEXTERITY => equippedAccessory?.GetItem()?.dexterityBonus ?? 0,
                        AttributeType.INTELLIGENCE => equippedAccessory?.GetItem()?.intelligenceBonus ?? 0,
                        AttributeType.REPUTATION => equippedAccessory?.GetItem()?.reputationBonus ?? 0,
                        _ => 0
                    };
                }
            }

            // Adjust current value by the bonuses
            currentValue = Math.Max(0, currentValue - valueFromCurrentEquipment); // Ensure non-negative
            return currentValue + bonusFromEquipment;
        }

        public static int GetStatusEffectResistanceFromEquipment(
            ArmorBase itemToEquip,
            StatusEffect statusEffect,
            PlayerStatusController playerStatusController,
            EquipmentDatabase equipmentDatabase)
        {
            // Get current value based on attribute type
            int currentValue = playerStatusController.GetResistanceForStatusEffect(statusEffect);

            // Determine bonus from the armor base and currently equipped item
            int bonusFromEquipment = 0;
            int valueFromCurrentEquipment = 0;

            // Retrieve bonus from armorBase
            if (itemToEquip != null)
            {
                ArmorBase.StatusEffectResistance match = itemToEquip.statusEffectResistances
                    .FirstOrDefault(x => x.statusEffect == statusEffect);
                bonusFromEquipment = (int)(match?.resistanceBonus ?? 0);
            }

            // Check currently equipped items
            if (itemToEquip is Helmet && equipmentDatabase.helmet.Exists())
            {
                ArmorBase.StatusEffectResistance match = equipmentDatabase.helmet.GetItem().statusEffectResistances
                    .FirstOrDefault(x => x.statusEffect == statusEffect);
                valueFromCurrentEquipment = (int)(match?.resistanceBonus ?? 0);
            }
            else if (itemToEquip is Armor && equipmentDatabase.armor.Exists())
            {
                ArmorBase.StatusEffectResistance match = equipmentDatabase.armor.GetItem().statusEffectResistances
                    .FirstOrDefault(x => x.statusEffect == statusEffect);
                valueFromCurrentEquipment = (int)(match?.resistanceBonus ?? 0);
            }
            else if (itemToEquip is Gauntlet && equipmentDatabase.gauntlet.Exists())
            {
                ArmorBase.StatusEffectResistance match = equipmentDatabase.gauntlet.GetItem().statusEffectResistances
                    .FirstOrDefault(x => x.statusEffect == statusEffect);
                valueFromCurrentEquipment = (int)(match?.resistanceBonus ?? 0);
            }
            else if (itemToEquip is Legwear && equipmentDatabase.legwear.Exists())
            {
                ArmorBase.StatusEffectResistance match = equipmentDatabase.legwear.GetItem().statusEffectResistances
                    .FirstOrDefault(x => x.statusEffect == statusEffect);
                valueFromCurrentEquipment = (int)(match?.resistanceBonus ?? 0);
            }

            // Adjust current value by the bonuses
            currentValue = Math.Max(0, currentValue - valueFromCurrentEquipment); // Ensure non-negative
            return currentValue + bonusFromEquipment;
        }


        public static int GetAttributeFromAccessory(Accessory accessory, AccessoryAttributeType attributeType, PlayerManager playerManager, EquipmentDatabase equipmentDatabase)
        {
            // Get current value based on attribute type
            int currentValue = attributeType switch
            {
                AccessoryAttributeType.HEALTH_BONUS => playerManager.health.GetMaxHealth(),
                AccessoryAttributeType.STAMINA_BONUS => playerManager.staminaStatManager.GetMaxStamina(),
                AccessoryAttributeType.MANA_BONUS => playerManager.manaManager.GetMaxMana(),
                _ => 0 // Fallback for safety
            };

            // Determine bonus from the accessory and currently equipped item
            int bonusFromEquipment = 0;
            int valueFromCurrentEquipment = 0;

            // Retrieve bonus from accessory if not equipped
            if (accessory != null && !equipmentDatabase.IsAccessoryEquiped(accessory))
            {
                bonusFromEquipment = attributeType switch
                {
                    AccessoryAttributeType.HEALTH_BONUS => accessory.healthBonus,
                    AccessoryAttributeType.STAMINA_BONUS => accessory.staminaBonus,
                    AccessoryAttributeType.MANA_BONUS => accessory.magicBonus,
                    _ => 0 // Fallback for safety
                };
            }

            // Loop through each accessory in the accessories collection
            foreach (var equippedAccessory in equipmentDatabase.accessories)
            {
                // Switch based on the specific type of attribute for the accessory
                valueFromCurrentEquipment += attributeType switch
                {
                    AccessoryAttributeType.HEALTH_BONUS => equippedAccessory?.GetItem()?.healthBonus ?? 0,
                    AccessoryAttributeType.STAMINA_BONUS => equippedAccessory?.GetItem()?.staminaBonus ?? 0,
                    AccessoryAttributeType.MANA_BONUS => equippedAccessory?.GetItem()?.magicBonus ?? 0,
                    _ => 0
                };
            }

            // Adjust current value by the bonuses
            currentValue = Math.Max(0, currentValue - valueFromCurrentEquipment); // Ensure non-negative
            return currentValue + bonusFromEquipment;
        }
    }
}
