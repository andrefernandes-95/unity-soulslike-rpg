namespace AF.Stats
{
    using System.Collections.Generic;
    using System.Linq;
    using AF.Events;
    using AF.StatusEffects;
    using TigerForge;
    using UnityEngine;
    using static AF.ArmorBase;

    public class StatsBonusController : MonoBehaviour
    {
        [Header("Bonus")]
        public int healthBonus = 0;
        public int magicBonus = 0;
        public int staminaBonus = 0;

        public int vitalityBonus = 0;
        public int enduranceBonus = 0;
        public int strengthBonus = 0;
        public int dexterityBonus = 0;
        public int intelligenceBonus = 0;

        public int vitalityBonusFromConsumable = 0;
        public int enduranceBonusFromConsumable = 0;
        public int strengthBonusFromConsumable = 0;
        public int dexterityBonusFromConsumable = 0;
        public int intelligenceBonusFromConsumable = 0;
        public float fireDefenseBonus = 0;
        public float frostDefenseBonus = 0;
        public float lightningDefenseBonus = 0;
        public float magicDefenseBonus = 0;
        public float darkDefenseBonus = 0;
        public float waterDefenseBonus = 0;
        public float additionalCoinPercentage = 0;
        public float coinMultiplierPerFallenEnemy = 1f;
        public int parryPostureDamageBonus = 0;
        public float parryPostureWindowBonus = 0;
        public int reputationBonus = 0;
        public float discountPercentage = 0f;
        public float spellDamageBonusMultiplier = 0f;
        public int postureBonus = 0;
        public int movementSpeedBonus = 0;

        public float postureDecreaseRateBonus = 0f;

        public float staminaRegenerationBonus = 0f;
        public bool chanceToRestoreHealthUponDeath = false;
        public bool chanceToNotLoseItemUponConsumption = false;
        public float projectileMultiplierBonus = 0f;
        public bool canRage = false;
        public float backStabAngleBonus = 0f;
        public bool shouldRegenerateMana = false;
        public bool increaseAttackPowerWhenUnarmed = false;
        public float twoHandAttackBonusMultiplier = 0f;
        public float slashDamageMultiplier = 0f;
        public float pierceDamageMultiplier = 0f;
        public float bluntDamageMultiplier = 0f;

        [Header("Equipment Modifiers")]
        public float weightPenalty = 0f;
        public int equipmentPoise = 0;
        public float equipmentPhysicalDefense = 0;
        public bool ignoreWeaponRequirements = false;

        [Header("Status Controller")]
        public StatusController statusController;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;
        public PlayerStatsDatabase playerStatsDatabase;

        [Header("Status Effect Resistances")]
        public Dictionary<StatusEffect, float> statusEffectCancellationRates = new();

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_SHIELD_EQUIPMENT_CHANGED, () =>
            {
                RecalculateEquipmentBonus();
            });
        }

        public void RecalculateEquipmentBonus()
        {
            UpdateStatusEffectCancellationRates();
            UpdateWeightPenalty();
            UpdateArmorPoise();
            UpdateEquipmentPhysicalDefense();
            UpdateStatusEffectResistances();
            UpdateAttributes();
            UpdateAdditionalCoinPercentage();

            UpdateWeaponBonuses();
        }

        void UpdateWeaponBonuses()
        {
            Weapon currentWeapon = equipmentDatabase.GetCurrentWeapon().GetItem();
            if (currentWeapon == null)
            {
                return;
            }

            coinMultiplierPerFallenEnemy += currentWeapon.coinMultiplierPerFallenEnemy;
        }

        void UpdateStatusEffectCancellationRates()
        {
            statusEffectCancellationRates.Clear();
            List<ArmorBase> items = new() {
                equipmentDatabase.helmet.GetItem(), equipmentDatabase.armor.GetItem(), equipmentDatabase.gauntlet.GetItem(), equipmentDatabase.legwear.GetItem(),
            };
            items.AddRange(equipmentDatabase.accessories.Select(accessory => accessory.GetItem()));

            foreach (var item in items)
            {
                if (item != null && item.statusEffectCancellationRates != null && item.statusEffectCancellationRates.Length > 0)
                {
                    EvaluateItemResistance(item.statusEffectCancellationRates);
                }
            }

            foreach (var shieldInstance in equipmentDatabase.shields)
            {
                if (shieldInstance.Exists() && shieldInstance.GetItem().statusEffectCancellationRates != null && shieldInstance.GetItem().statusEffectCancellationRates.Length > 0)
                {
                    EvaluateItemResistance(shieldInstance.GetItem().statusEffectCancellationRates);
                }
            }
        }

        void EvaluateItemResistance(StatusEffectCancellationRate[] itemStatusEffectCancellationRates)
        {
            foreach (var statusEffectCancellationRate in itemStatusEffectCancellationRates)
            {
                if (statusEffectCancellationRates.ContainsKey(statusEffectCancellationRate.statusEffect))
                {
                    statusEffectCancellationRates[statusEffectCancellationRate.statusEffect] += statusEffectCancellationRate.amountToCancelPerSecond;
                }
                else
                {
                    statusEffectCancellationRates.Add(statusEffectCancellationRate.statusEffect, statusEffectCancellationRate.amountToCancelPerSecond);
                }
            }
        }

        void UpdateWeightPenalty()
        {
            weightPenalty = 0f;

            if (equipmentDatabase.GetCurrentWeapon().Exists())
            {
                weightPenalty += equipmentDatabase.GetCurrentWeapon().GetItem().speedPenalty;
            }
            if (equipmentDatabase.GetCurrentShield().Exists())
            {
                weightPenalty += equipmentDatabase.GetCurrentShield().GetItem().speedPenalty;
            }
            if (equipmentDatabase.helmet.Exists())
            {
                weightPenalty += equipmentDatabase.helmet.GetItem().speedPenalty;
            }
            if (equipmentDatabase.armor.Exists())
            {
                weightPenalty += equipmentDatabase.armor.GetItem().speedPenalty;
            }
            if (equipmentDatabase.gauntlet.Exists())
            {
                weightPenalty += equipmentDatabase.gauntlet.GetItem().speedPenalty;
            }
            if (equipmentDatabase.legwear.Exists())
            {
                weightPenalty += equipmentDatabase.legwear.GetItem().speedPenalty;
            }

            weightPenalty += equipmentDatabase.accessories.Sum(equippedAccessory => equippedAccessory.IsEmpty() ? 0 : equippedAccessory.GetItem().speedPenalty);

            weightPenalty = Mathf.Max(0, weightPenalty); // Ensure weightPenalty is non-negative
        }

        void UpdateArmorPoise()
        {
            equipmentPoise = 0;

            if (equipmentDatabase.helmet.Exists())
            {
                equipmentPoise += equipmentDatabase.helmet.GetItem().poiseBonus;
            }
            if (equipmentDatabase.armor.Exists())
            {
                equipmentPoise += equipmentDatabase.armor.GetItem().poiseBonus;
            }
            if (equipmentDatabase.gauntlet.Exists())
            {
                equipmentPoise += equipmentDatabase.gauntlet.GetItem().poiseBonus;
            }
            if (equipmentDatabase.legwear.Exists())
            {
                equipmentPoise += equipmentDatabase.legwear.GetItem().poiseBonus;
            }

            equipmentPoise += equipmentDatabase.accessories.Sum(equippedAccessory => equippedAccessory.IsEmpty() ? 0 : equippedAccessory.GetItem().poiseBonus);
        }

        void UpdateEquipmentPhysicalDefense()
        {
            equipmentPhysicalDefense = 0f;

            if (equipmentDatabase.helmet.Exists())
            {
                equipmentPhysicalDefense += equipmentDatabase.helmet.GetItem().physicalDefense;
            }

            if (equipmentDatabase.armor.Exists())
            {
                equipmentPhysicalDefense += equipmentDatabase.armor.GetItem().physicalDefense;
            }

            if (equipmentDatabase.gauntlet.Exists())
            {
                equipmentPhysicalDefense += equipmentDatabase.gauntlet.GetItem().physicalDefense;
            }

            if (equipmentDatabase.legwear.Exists())
            {
                equipmentPhysicalDefense += equipmentDatabase.legwear.GetItem().physicalDefense;
            }

            equipmentPhysicalDefense += equipmentDatabase.accessories.Sum(equippedAccessory => equippedAccessory.IsEmpty()
                ? 0 : equippedAccessory.GetItem().physicalDefense);
        }

        void UpdateStatusEffectResistances()
        {
            statusController.statusEffectResistanceBonuses.Clear();

            HandleStatusEffectEntries(equipmentDatabase.helmet?.GetItem()?.statusEffectResistances);
            HandleStatusEffectEntries(equipmentDatabase.armor?.GetItem()?.statusEffectResistances);
            HandleStatusEffectEntries(equipmentDatabase.gauntlet?.GetItem()?.statusEffectResistances);
            HandleStatusEffectEntries(equipmentDatabase.legwear?.GetItem()?.statusEffectResistances);

            var accessoryResistances = equipmentDatabase.accessories
                .Where(a => a != null)
                .SelectMany(a => a.GetItem()?.statusEffectResistances ?? Enumerable.Empty<StatusEffectResistance>())
                .ToArray();
            HandleStatusEffectEntries(accessoryResistances);
        }

        void HandleStatusEffectEntries(StatusEffectResistance[] resistances)
        {
            if (resistances != null && resistances.Length > 0)
            {
                foreach (var resistance in resistances)
                {
                    HandleStatusEffectEntry(resistance);
                }
            }
        }

        void HandleStatusEffectEntry(StatusEffectResistance statusEffectResistance)
        {
            if (statusController.statusEffectResistanceBonuses.ContainsKey(statusEffectResistance.statusEffect))
            {
                statusController.statusEffectResistanceBonuses[statusEffectResistance.statusEffect]
                    += (int)statusEffectResistance.resistanceBonus;
            }
            else
            {
                statusController.statusEffectResistanceBonuses.Add(
                    statusEffectResistance.statusEffect, (int)statusEffectResistance.resistanceBonus);
            }
        }

        void UpdateAttributes()
        {
            ResetAttributes();

            ApplyWeaponAttributes(equipmentDatabase.GetCurrentWeapon().GetItem());

            ApplyEquipmentAttributes(equipmentDatabase.helmet.GetItem());
            ApplyEquipmentAttributes(equipmentDatabase.armor.GetItem());
            ApplyEquipmentAttributes(equipmentDatabase.gauntlet.GetItem());
            ApplyEquipmentAttributes(equipmentDatabase.legwear.GetItem());

            ApplyAccessoryAttributes();

            ApplyShieldAttributes();
        }

        void ResetAttributes()
        {

            healthBonus = magicBonus = staminaBonus = vitalityBonus = enduranceBonus = strengthBonus = dexterityBonus = intelligenceBonus = 0;
            fireDefenseBonus = frostDefenseBonus = lightningDefenseBonus = magicDefenseBonus = darkDefenseBonus = waterDefenseBonus = discountPercentage = spellDamageBonusMultiplier = 0;
            reputationBonus = parryPostureDamageBonus = postureBonus = movementSpeedBonus = 0;

            parryPostureWindowBonus = staminaRegenerationBonus = postureDecreaseRateBonus = projectileMultiplierBonus = backStabAngleBonus = 0f;

            shouldRegenerateMana = chanceToRestoreHealthUponDeath = canRage = chanceToNotLoseItemUponConsumption = increaseAttackPowerWhenUnarmed = false;

            twoHandAttackBonusMultiplier = slashDamageMultiplier = pierceDamageMultiplier = bluntDamageMultiplier = 0f;

            coinMultiplierPerFallenEnemy = 1f;
        }

        void ApplyWeaponAttributes(Weapon currentWeapon)
        {
            if (currentWeapon != null)
            {
                shouldRegenerateMana = currentWeapon.shouldRegenerateMana;
            }
        }

        void ApplyEquipmentAttributes(ArmorBase equipment)
        {
            if (equipment != null)
            {
                vitalityBonus += equipment.vitalityBonus;
                enduranceBonus += equipment.enduranceBonus;
                strengthBonus += equipment.strengthBonus;
                dexterityBonus += equipment.dexterityBonus;
                intelligenceBonus += equipment.intelligenceBonus;
                fireDefenseBonus += equipment.fireDefense;
                frostDefenseBonus += equipment.frostDefense;
                lightningDefenseBonus += equipment.lightningDefense;
                magicDefenseBonus += equipment.magicDefense;
                darkDefenseBonus += equipment.darkDefense;
                waterDefenseBonus += equipment.waterDefense;
                reputationBonus += equipment.reputationBonus;
                discountPercentage += equipment.discountPercentage;
                postureBonus += equipment.postureBonus;
                staminaRegenerationBonus += equipment.staminaRegenBonus;
                movementSpeedBonus += equipment.movementSpeedBonus;
                projectileMultiplierBonus += equipment.projectileMultiplierBonus;

                if (equipment.canRage)
                {
                    canRage = true;
                }
            }
        }

        void ApplyAccessoryAttributes()
        {
            foreach (var accessoryInstance in equipmentDatabase.accessories)
            {
                Accessory accessory = accessoryInstance?.GetItem();

                vitalityBonus += accessory?.vitalityBonus ?? 0;
                enduranceBonus += accessory?.enduranceBonus ?? 0;
                strengthBonus += accessory?.strengthBonus ?? 0;
                dexterityBonus += accessory?.dexterityBonus ?? 0;
                intelligenceBonus += accessory?.intelligenceBonus ?? 0;
                fireDefenseBonus += accessory?.fireDefense ?? 0;
                frostDefenseBonus += accessory?.frostDefense ?? 0;
                lightningDefenseBonus += accessory?.lightningDefense ?? 0;
                magicDefenseBonus += accessory?.magicDefense ?? 0;
                darkDefenseBonus += accessory?.darkDefense ?? 0;
                waterDefenseBonus += accessory?.waterDefense ?? 0;
                reputationBonus += accessory?.reputationBonus ?? 0;
                parryPostureDamageBonus += accessory?.postureDamagePerParry ?? 0;

                backStabAngleBonus += accessory?.backStabAngleBonus ?? 0;

                healthBonus += accessory?.healthBonus ?? 0;
                magicBonus += accessory?.magicBonus ?? 0;
                staminaBonus += accessory?.staminaBonus ?? 0;
                spellDamageBonusMultiplier += accessory?.spellDamageBonusMultiplier ?? 0;
                postureBonus += accessory?.postureBonus ?? 0;
                staminaRegenerationBonus += accessory?.staminaRegenBonus ?? 0;

                postureDecreaseRateBonus += accessory?.postureDecreaseRateBonus ?? 0;


                if (accessory != null)
                {
                    if (accessory.chanceToRestoreHealthUponDeath)
                    {
                        chanceToRestoreHealthUponDeath = true;
                    }

                    if (accessory.chanceToNotLoseItemUponConsumption)
                    {
                        chanceToNotLoseItemUponConsumption = true;
                    }

                    if (accessory.increaseAttackPowerWhenUnarmed)
                    {
                        increaseAttackPowerWhenUnarmed = true;
                    }

                    if (accessory.twoHandAttackBonusMultiplier > 0)
                    {
                        twoHandAttackBonusMultiplier += accessory.twoHandAttackBonusMultiplier;
                    }

                    if (accessory.slashDamageMultiplier > 0)
                    {
                        slashDamageMultiplier += accessory.slashDamageMultiplier;
                    }

                    if (accessory.bluntDamageMultiplier > 0)
                    {
                        bluntDamageMultiplier += accessory.bluntDamageMultiplier;
                    }

                    if (accessory.pierceDamageMultiplier > 0)
                    {
                        pierceDamageMultiplier += accessory.pierceDamageMultiplier;
                    }
                }
            }
        }

        void ApplyShieldAttributes()
        {
            Shield currentShield = equipmentDatabase.GetCurrentShield()?.GetItem();
            if (currentShield != null)
            {

                parryPostureWindowBonus += currentShield.parryWindowBonus;
                parryPostureDamageBonus += currentShield.parryPostureDamageBonus;
                staminaRegenerationBonus += currentShield.staminaRegenBonus;
            }
        }

        void UpdateAdditionalCoinPercentage()
        {
            additionalCoinPercentage = GetEquipmentCoinPercentage(equipmentDatabase.helmet?.GetItem())
                                   + GetEquipmentCoinPercentage(equipmentDatabase.armor?.GetItem())
                                   + GetEquipmentCoinPercentage(equipmentDatabase.gauntlet?.GetItem())
                                   + GetEquipmentCoinPercentage(equipmentDatabase.legwear?.GetItem())
                                   + equipmentDatabase.accessories.Sum(equippedAccessory => equippedAccessory.IsEmpty()
                                    ? 0 : equippedAccessory.GetItem().additionalCoinPercentage);
        }

        float GetEquipmentCoinPercentage(ArmorBase equipment)
        {
            return equipment != null ? equipment.additionalCoinPercentage : 0f;
        }

        public int GetCurrentIntelligence()
        {
            return playerStatsDatabase.intelligence + intelligenceBonus + intelligenceBonusFromConsumable;
        }

        public int GetCurrentDexterity()
        {
            return playerStatsDatabase.dexterity + dexterityBonus + dexterityBonusFromConsumable;
        }

        public int GetCurrentStrength()
        {
            return playerStatsDatabase.strength + strengthBonus + strengthBonusFromConsumable;
        }

        public int GetCurrentVitality()
        {
            return playerStatsDatabase.vitality + vitalityBonus + vitalityBonusFromConsumable;
        }

        public int GetCurrentEndurance()
        {
            return playerStatsDatabase.endurance + enduranceBonus + enduranceBonusFromConsumable;
        }

        public int GetCurrentReputation()
        {
            return playerStatsDatabase.GetCurrentReputation() + reputationBonus;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void SetStatsFromConsumable(int value)
        {
            this.vitalityBonusFromConsumable = value;
            this.enduranceBonusFromConsumable = value;
            this.strengthBonusFromConsumable = value;
            this.dexterityBonusFromConsumable = value;
            this.intelligenceBonusFromConsumable = value;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void SetIgnoreNextWeaponToEquipRequirements(bool value)
        {
            ignoreWeaponRequirements = value;
        }
    }
}
